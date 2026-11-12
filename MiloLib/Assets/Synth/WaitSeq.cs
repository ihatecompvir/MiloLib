using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("WaitSeq"), Description("A Sequence type which just waits a specified duration, generating no sound. Useful for tweaking the timing of other events.")]
public class WaitSeq : GroupSeq
{
    private ushort altRevision;
    private ushort revision;

    [Name("Avg Wait Seconds"), Description("Average wait time, in seconds"), MaxVersion(2)]
    public float avgWaitSecs;
    [Name("Wait Spread"), Description("Amount to vary the wait time, in seconds"), MinVersion(2), MaxVersion(2)]
    public float waitSpread;

    [MaxVersion(2)]
    public Sfx.Sequence sequence = new();

    public new WaitSeq Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 3)
        {
            sequence = new Sfx.Sequence().Read(reader, false, parent, entry);
            avgWaitSecs = reader.ReadFloat();
            if (1 < revision)
            {
                waitSpread = reader.ReadFloat();
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

        if (revision < 3)
        {
            sequence.Write(writer, false, parent, entry);
            writer.WriteFloat(avgWaitSecs);
            if (1 < revision)
            {
                writer.WriteFloat(waitSpread);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
