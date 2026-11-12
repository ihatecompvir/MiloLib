using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("RandomIntervalGroupSeq"), Description("Plays all of its child sequences at random intervals")]
public class RandomIntervalGroupSeq : GroupSeq
{
    private ushort altRevision;
    private ushort revision;

    [Name("Avg Interval Secs"), Description("the number of seconds on average we wait to play a child cue again")]
    public float avgIntervalSecs = 4.0f;
    [Name("Interval Spread"), Description("We randomly deviate + or - this many seconds from the average when picking our wait interval")]
    public float intervalSpread = 2.0f;
    [Name("Max Simul"), Description("the maximum number of sounds we allow at one time")]
    public int maxSimultaneous = 8;

    public new RandomIntervalGroupSeq Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        avgIntervalSecs = reader.ReadFloat();
        intervalSpread = reader.ReadFloat();
        maxSimultaneous = reader.ReadInt32();

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

        writer.WriteFloat(avgIntervalSecs);
        writer.WriteFloat(intervalSpread);
        writer.WriteInt32(maxSimultaneous);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
