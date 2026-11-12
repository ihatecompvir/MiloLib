using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Morph"), Description("A Morph object animates between multiple Mesh poses using weight keyframes. This is an expensive technique, equivalent to a MeshAnim for each active pose, so use only when skinning or a single MeshAnim isn't enough. For example, we use it for viseme-driven facial animation.")]
public class RndMorph : Object
{
    public class Pose
    {
        [Name("Mesh")]
        public Symbol mesh = Symbol.Empty;

        public List<PropKey.FloatKey> weightKeys = new();

        public Pose Read(EndianReader reader)
        {
            mesh = Symbol.Read(reader);
            weightKeys = AssetHelpers.ReadList(reader, () => new PropKey.FloatKey().Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, mesh);
            AssetHelpers.WriteList(writer, weightKeys, (PropKey.FloatKey key) => key.Write(writer));
        }
    }

    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    [Name("Poses")]
    public List<Pose> poses = new();

    [Name("Target"), Description("Mesh for the morph to occur")]
    public Symbol target = Symbol.Empty;

    [Name("Normals"), Description("Interpolates the normals if set to true, otherwise normals are not affected."), MinVersion(1)]
    public bool normals;

    [Name("Spline"), Description("Smooths the interpolation of the morphing."), MinVersion(1)]
    public bool spline;

    [Name("Intensity"), Description("Modifier for weight interpolation"), MinVersion(3)]
    public float intensity;

    public new RndMorph Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 3)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);

        poses = AssetHelpers.ReadList(reader, () => new Pose().Read(reader));

        target = Symbol.Read(reader);

        if (revision > 0)
        {
            normals = reader.ReadBoolean();
            spline = reader.ReadBoolean();
        }

        if (revision > 2)
        {
            intensity = reader.ReadFloat();
        }

        if (standalone)
        {
            uint expectedEndBytes = 0xDEADDEAD;
            if (reader.Endianness == Endian.BigEndian)
            {
                expectedEndBytes = 0xADDEADDE;
            }
            uint actualEndBytes = reader.ReadUInt32();
            if (actualEndBytes != expectedEndBytes)
            {
                throw MiloLib.Exceptions.MiloAssetReadException.EndBytesNotFound(parent, entry, reader.BaseStream.Position, revision);
            }
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision > 3)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);

        AssetHelpers.WriteList(writer, poses, (Pose pose) => pose.Write(writer));

        Symbol.Write(writer, target);

        if (revision > 0)
        {
            writer.WriteBoolean(normals);
            writer.WriteBoolean(spline);
        }

        if (revision > 2)
        {
            writer.WriteFloat(intensity);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
