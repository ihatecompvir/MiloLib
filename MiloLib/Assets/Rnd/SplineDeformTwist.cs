using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("SplineDeformTwist"), Description("Spline deform node to apply a twist along the spline.")]
public class SplineDeformTwist : SplineDeformNode
{
    private ushort revision = 0;
    private ushort altRevision;
    [Name("Start Y")]
    public float startY;
    [Description("Degrees of ever-increasing roll per 100 units of distance in Y.")]
    public float rollRate;
    [Description("Degrees of periodic roll per 100 units of distance in Y.")]
    public float periodicRollRate;
    [Name("Periodic Roll Frequency"), Description("Frequency of the periodic roll (in terms of 100 units of distance in Y).")]
    public float periodicRollFrequency;

    public new SplineDeformTwist Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        startY = reader.ReadFloat();
        rollRate = reader.ReadFloat();
        periodicRollRate = reader.ReadFloat();
        periodicRollFrequency = reader.ReadFloat();
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
        writer.WriteFloat(rollRate);
        writer.WriteFloat(periodicRollRate);
        writer.WriteFloat(periodicRollFrequency);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

