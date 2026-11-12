using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("CamAnim"), Description("A CamAnim object animates Camera properties.")]
public class RndCamAnim : Object
{
    public struct FovKey
    {
        [Name("Frame"), Description("The frame")]
        public float frame;
        [Name("Value"), Description("The fov to set, not sure about units")]
        public float value;

        public void Read(EndianReader reader)
        {
            value = reader.ReadFloat();
            frame = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteFloat(value);
            writer.WriteFloat(frame);
        }
    }

    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();
    [Name("Cam"), Description("The camera whose fov we will animate")]
    public Symbol cam = Symbol.Empty;
    [Name("FOV Keys"), Description("The frames and corresponding fovs")]
    public List<FovKey> fovKeys = new();
    [Name("Keys Owner"), Description("Owner of the keys, usually myself")]
    public Symbol keysOwner = Symbol.Empty;

    public new RndCamAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 0)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);

        cam = Symbol.Read(reader);

        fovKeys = AssetHelpers.ReadList(reader, () => { FovKey key = new(); key.Read(reader); return key; });

        keysOwner = Symbol.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision != 0)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);

        Symbol.Write(writer, cam);

        AssetHelpers.WriteList(writer, fovKeys, key => key.Write(writer));

        Symbol.Write(writer, keysOwner);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
