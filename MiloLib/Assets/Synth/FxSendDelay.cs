using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendDelay"), Description("A delay effect.")]
public class FxSendDelay : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Delay Time"), Description("Delay time, in seconds")]
    public float delayTime;
    [Name("Feedback"), Description("Feedback of delayed signal, in dB")]
    public float gain;

    [Name("Tempo Sync"), Description("Sync delay to song tempo?"), MinVersion(2)]
    public bool tempoSync;
    [Name("Sync Type"), Description("Note value of delay"), MinVersion(2)]
    public Symbol syncType = Symbol.Empty;
    [Name("Tempo"), Description("Tempo for delay; can be driven by game code"), MinVersion(2)]
    public float tempo;

    [Name("Ping Pong Pct"), Description("Depth of ping pong effect (percent)"), MinVersion(3)]
    public float pingPongPct;

    public new FxSendDelay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        delayTime = reader.ReadFloat();
        gain = reader.ReadFloat();

        if (revision >= 2)
        {
            tempoSync = reader.ReadBoolean();
            syncType = Symbol.Read(reader);
            tempo = reader.ReadFloat();
        }

        if (revision >= 3)
        {
            pingPongPct = reader.ReadFloat();
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

        writer.WriteFloat(delayTime);
        writer.WriteFloat(gain);

        if (revision >= 2)
        {
            writer.WriteBoolean(tempoSync);
            Symbol.Write(writer, syncType);
            writer.WriteFloat(tempo);
        }

        if (revision >= 3)
        {
            writer.WriteFloat(pingPongPct);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
