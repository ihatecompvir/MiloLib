using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CollideableCapsule"), Description("A CollideableCapsule is a cylinder with two half-hemisphere endcaps.")]
public class CollideableCapsule : Object
{
    private ushort revision = 1;
    private ushort altRevision;

    [Name("P0"), Description("start position of the capsule.")]
    public Vector3 p0 = new();
    [Name("P1"), Description("end position of the capsule.")]
    public Vector3 p1 = new();
    [Name("Radius"), Description("radius of the capsule.")]
    public float radius;

    public new CollideableCapsule Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        // seems that collideable shapes don't serialize the Object
        p0.Read(reader);
        p1.Read(reader);
        radius = reader.ReadFloat();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        p0.Write(writer);
        p1.Write(writer);
        writer.WriteFloat(radius);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
