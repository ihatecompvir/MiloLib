using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Flare"), Description("Flare objects are particles that are rendered based on the visibility of their center. Use them to implement sun flare peeking through tree leaves and around buildings, flares on lamps and stage lights that get occluded by characters, etc. Flares should be drawn last in a frame, and since they depend on the z-buffer for visibility testing, previously drawn translucent or cutout objects must properly use translucent or SrcAlphaCutout materials.")]
public class RndFlare : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndTrans trans = new();
    public RndDrawable draw = new();

    [Name("Material"), Description("Material for flare"), MinVersion(1)]
    public Symbol material = Symbol.Empty;

    [Name("Sizes"), Description("Size range of the flare. If the sizes are unequal, the size will change depending on how much the camera faces the flare's Y axis."), MinVersion(3)]
    public Vector2 sizes = new();
    [Name("Range"), Description("Fade range of the flare. The flare will fade within this distance from the camera. Past the max range, it will fade to 0."), MinVersion(2)]
    public Vector2 range = new();

    [Name("Steps"), Description("Number of steps for the flare to fade in and out."), MinVersion(2)]
    public int steps;

    [Name("Point Test"), Description("Controls whether or not a point test is performed for the flare. The test takes one world frame to take effect."), MinVersion(5)]
    public bool pointTest;

    [Name("Offset"), Description("Offsets position of rendered flare towards the camera."), MinVersion(7)]
    public float offset;

    public new RndFlare Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        trans.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);

        if (revision != 0)
        {
            material = Symbol.Read(reader);
        }

        if (revision > 2)
        {
            sizes.Read(reader);
        }

        if (revision > 1)
        {
            range.Read(reader);
            steps = reader.ReadInt32();
        }

        if (revision > 4)
        {
            pointTest = reader.ReadBoolean();
        }

        if (revision > 6)
        {
            offset = reader.ReadFloat();
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);

        trans.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);

        if (revision != 0)
        {
            Symbol.Write(writer, material);
        }

        if (revision > 2)
        {
            sizes.Write(writer);
        }

        if (revision > 1)
        {
            range.Write(writer);
            writer.WriteInt32(steps);
        }

        if (revision > 4)
        {
            writer.WriteBoolean(pointTest);
        }

        if (revision > 6)
        {
            writer.WriteFloat(offset);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
