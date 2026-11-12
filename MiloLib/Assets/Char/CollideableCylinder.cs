using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CollideableCylinder"), Description("A Collideable defined by two end points that form an axis and a radius around that axis.")]
public class CollideableCylinder : Object
{
    private ushort revision = 1;
    private ushort altRevision;

    [Name("Center"), Description("Center position of the cylinder.")]
    public Vector3 center = new();
    [Name("Radius"), Description("radius of the cylinder.")]
    public float radius;
    [Name("Axis")]
    public Vector3 axis = new(0, 0, 1);
    [Name("Half Length"), Description("Length from the center to either endcap of the cylinder.")]
    public float halfLength;

    public new CollideableCylinder Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        // seems that collideable shapes don't serialize the Object
        center.Read(reader);
        radius = reader.ReadFloat();
        axis.Read(reader);
        halfLength = reader.ReadFloat();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        center.Write(writer);
        writer.WriteFloat(radius);
        axis.Write(writer);
        writer.WriteFloat(halfLength);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
