using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendChorus"), Description("A chorus effect.")]
public class FxSendChorus : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Delay Ms"), Description("Maximum delay time in milliseconds")]
    public float delayMs;
    [Name("Rate"), Description("Rate at which pitch is modulated (Hz)")]
    public float rate;
    [Name("Depth"), Description("Depth of pitch modulation, in centitones")]
    public float depth;
    [Name("Feedback Pct"), Description("Percent of output that is fed back to input")]
    public int feedbackPct;
    [Name("Offset Pct"), Description("LFO phase offset between channels (for wider stereo effect)")]
    public int offsetPct;

    [Name("Tempo Sync"), Description("Sync modulation to song tempo?"), MinVersion(3)]
    public bool tempoSync;
    [Name("Sync Type"), Description("Note value to synchronize modulation to"), MinVersion(3)]
    public Symbol syncType = Symbol.Empty;
    [Name("Tempo"), Description("Tempo for modulation; can be driven by game code"), MinVersion(3)]
    public float tempo;

    public new FxSendChorus Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        delayMs = reader.ReadFloat();
        rate = reader.ReadFloat();
        depth = reader.ReadFloat();
        feedbackPct = reader.ReadInt32();
        offsetPct = reader.ReadInt32();

        if (revision >= 3)
        {
            tempoSync = reader.ReadBoolean();
            syncType = Symbol.Read(reader);
            tempo = reader.ReadFloat();
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

        writer.WriteFloat(delayMs);
        writer.WriteFloat(rate);
        writer.WriteFloat(depth);
        writer.WriteInt32(feedbackPct);
        writer.WriteInt32(offsetPct);

        if (revision >= 3)
        {
            writer.WriteBoolean(tempoSync);
            Symbol.Write(writer, syncType);
            writer.WriteFloat(tempo);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
