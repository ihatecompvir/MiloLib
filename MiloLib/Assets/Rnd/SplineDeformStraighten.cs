using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("SplineDeformStraighten"), Description("Spline deform node to straighten the spline.")]
public class SplineDeformStraighten : SplineDeformNode
{
    private ushort revision = 0;
    private ushort altRevision;
    [Name("Start Y")]
    public float startY;
    [Name("Straighten Amount"), Description("Amount to straighten the spline (0 to 1).")]
    public float straightenAmount;

    public new SplineDeformStraighten Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        startY = reader.ReadFloat();
        straightenAmount = reader.ReadFloat();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        writer.WriteFloat(startY);
        writer.WriteFloat(straightenAmount);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

