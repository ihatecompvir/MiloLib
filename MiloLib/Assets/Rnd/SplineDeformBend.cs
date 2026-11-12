using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("SplineDeformBend"), Description("Spline deform node to apply a bend to the spline.")]
public class SplineDeformBend : SplineDeformNode
{
    private ushort revision = 0;
    private ushort altRevision;
    [Name("Start Y")]
    public float startY;
    [Description("Degrees of ever-increasing bend per 100 units of distance in Y.")]
    public float horizontalBendRate;
    [Description("Degrees of periodic bend per 100 units of distance in Y.")]
    public float horizontalPeriodicBendRate;
    [Name("Horizontal Periodic Bend Frequency"), Description("Frequency of the periodic bend (in terms of 100 units of distance in Y).")]
    public float horizontalPeriodicBendFrequency;
    [Description("Degrees of ever-increasing bend per 100 units of distance in Y.")]
    public float verticalBendRate;
    [Description("Degrees of periodic bend per 100 units of distance in Y.")]
    public float verticalPeriodicBendRate;
    [Name("Vertical Periodic Bend Frequency"), Description("Frequency of the periodic bend (in terms of 100 units of distance in Y).")]
    public float verticalPeriodicBendFrequency;

    public new SplineDeformBend Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        startY = reader.ReadFloat();
        horizontalBendRate = reader.ReadFloat();
        horizontalPeriodicBendRate = reader.ReadFloat();
        horizontalPeriodicBendFrequency = reader.ReadFloat();
        verticalBendRate = reader.ReadFloat();
        verticalPeriodicBendRate = reader.ReadFloat();
        verticalPeriodicBendFrequency = reader.ReadFloat();
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
        writer.WriteFloat(horizontalBendRate);
        writer.WriteFloat(horizontalPeriodicBendRate);
        writer.WriteFloat(horizontalPeriodicBendFrequency);
        writer.WriteFloat(verticalBendRate);
        writer.WriteFloat(verticalPeriodicBendRate);
        writer.WriteFloat(verticalPeriodicBendFrequency);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

