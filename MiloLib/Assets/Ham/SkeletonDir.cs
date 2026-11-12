using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("SkeletonDir"), Description("Dir with skeleton tracking/debugging functionality")]
public class SkeletonDir : PanelDir
{
    private ushort altRevision;
    private ushort revision;

    [Name("Test Clip"), Description("Test clip for in-milo visualization")]
    public Symbol unkSym = Symbol.Empty;

    public bool unkBool;
    [Name("Use Smoothing"), Description("Apply joint smoothing?"), MinVersion(2), MaxVersion(3)]
    public bool useSmoothing;
    public bool unkBool3;

    [Name("Smoothing"), Description("0.0 means raw data is returned. Increasing leads to more highly smoothed position values. As it's increased, responsiveness to raw data decreases. Increased smoothing leads to increased latency in the returned values. Must be within [0.0 .. 1.0]"), MinVersion(2), MaxVersion(3)]
    public float smoothing;
    [Name("Correction"), Description("Correction parameter. Lower values are slower to correct towards the raw data, and appear smoother, while higher values will correct toward the raw data more quickly. Values must be in the range [0.0 .. 1.0]."), MinVersion(2), MaxVersion(3)]
    public float correction;
    [Name("Prediction"), Description("The number of frames to predict into the future. Values must be greater than or equal to zero. Values greater than 0.5 will likely lead to overshooting when moving quickly. This effect can be damped by using small values of fMaxDeviationRadius."), MinVersion(2), MaxVersion(3)]
    public float prediction;
    [Name("Jitter Radius"), Description("The radius in meters for jitter reduction. Any jitter beyond this radius is clamped to the radius"), MinVersion(2), MaxVersion(3)]
    public float jitterRadius;
    [Name("Max Deviation Radius"), Description("The maximum radius in meters that filtered positions are allowed to deviate from raw data. Filtered values that would be more than this radius from the raw data are clamped at this distance, in the direction of the filtered value."), MinVersion(2), MaxVersion(3)]
    public float maxDeviationRadius;

    public SkeletonDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new SkeletonDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        // don't read these fields if the entry is in another directory
        if (entry != null && !entry.isProxy)
        {
            unkSym = Symbol.Read(reader);
            if (revision < 4)
            {

                if (revision != 0)
                {
                    unkBool = reader.ReadBoolean();
                }

                if (revision > 1)
                {
                    useSmoothing = reader.ReadBoolean();
                    smoothing = reader.ReadFloat();
                    correction = reader.ReadFloat();
                    prediction = reader.ReadFloat();
                    jitterRadius = reader.ReadFloat();
                    maxDeviationRadius = reader.ReadFloat();
                }

                if (revision > 2)
                {
                    unkBool3 = reader.ReadBoolean();
                }
            }
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

        if (entry != null && !entry.isProxy)
        {
            Symbol.Write(writer, unkSym);

            if (revision < 4)
            {
                if (revision != 0)
                {
                    writer.WriteBoolean(unkBool);
                }

                if (revision > 1)
                {
                    writer.WriteBoolean(useSmoothing);
                    writer.WriteFloat(smoothing);
                    writer.WriteFloat(correction);
                    writer.WriteFloat(prediction);
                    writer.WriteFloat(jitterRadius);
                    writer.WriteFloat(maxDeviationRadius);
                }

                if (revision > 2)
                {
                    writer.WriteBoolean(unkBool3);
                }
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }

    protected void ReadPanelDirBase(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        base.Read(reader, false, parent, entry);
    }

    protected void WritePanelDirBase(EndianWriter writer, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        base.Write(writer, false, parent, entry);
    }

}
