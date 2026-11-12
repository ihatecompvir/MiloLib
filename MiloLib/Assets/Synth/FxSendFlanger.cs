using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendFlanger"), Description("A flanger effect.")]
public class FxSendFlanger : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Delay Ms"), Description("Maximum delay time in milliseconds")]
    public float delayMs;
    [Name("Rate"), Description("Rate at which delay is modulated (Hz)")]
    public float rate;
    [Name("Depth Pct"), Description("Percent depth of delay modulation"), MinVersion(4)]
    public int depthPct;
    [Name("Feedback Pct"), Description("Percent of output that is fed back to input"), MinVersion(2)]
    public int feedbackPct;
    [Name("Offset Pct"), Description("LFO phase offset between channels (for wider stereo effect)"), MinVersion(3)]
    public int offsetPct;

    [Name("Tempo Sync"), Description("Sync modulation to song tempo?"), MinVersion(6)]
    public bool tempoSync;
    [Name("Sync Type"), Description("Note value to synchronize modulation to"), MinVersion(6)]
    public Symbol syncType = Symbol.Empty;
    [Name("Tempo"), Description("Tempo for modulation; can be driven by game code"), MinVersion(6)]
    public float tempo;

    [MaxVersion(3)]
    private int oldDepth;

    public new FxSendFlanger Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        delayMs = reader.ReadFloat();
        rate = reader.ReadFloat();

        if (revision >= 4)
        {
            depthPct = reader.ReadInt32();
        }
        else
        {
            oldDepth = reader.ReadInt32();
        }

        if (revision >= 2)
        {
            feedbackPct = reader.ReadInt32();
        }

        if (revision >= 3)
        {
            offsetPct = reader.ReadInt32();
        }

        if (revision >= 6)
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

        if (revision >= 4)
        {
            writer.WriteInt32(depthPct);
        }
        else
        {
            writer.WriteInt32(oldDepth);
        }

        if (revision >= 2)
        {
            writer.WriteInt32(feedbackPct);
        }

        if (revision >= 3)
        {
            writer.WriteInt32(offsetPct);
        }

        if (revision >= 6)
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
