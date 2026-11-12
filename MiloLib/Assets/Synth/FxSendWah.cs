using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendWah"), Description("wah-wah effect")]
public class FxSendWah : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Resonance"), Description("amount of resonance (1-10)")]
    public float resonance;
    [Name("Lower Freq"), Description("low frequency peak of resonant filter (Hz)")]
    public float lowerFreq;
    [Name("Upper Freq"), Description("high frequency peak of resonant filter (Hz)")]
    public float upperFreq;
    [Name("LFO Freq"), Description("rate of LFO oscillations (Hz)")]
    public float lfoFreq;
    [Name("Magic"), Description("magic number (0-1)")]
    public float magic;

    [Name("Tempo Sync"), Description("Sync wah to song tempo?"), MinVersion(2)]
    public bool tempoSync;
    [Name("Tempo"), Description("Tempo for delay; can be driven by game code"), MinVersion(2)]
    public float tempo;
    [Name("Sync Type"), Description("Note value of delay"), MinVersion(2)]
    public Symbol syncType = Symbol.Empty;

    [Name("Dist Amount"), Description("Post wah distortion amount"), MinVersion(3)]
    public float distAmount;
    [Name("Auto Wah"), Description("Is the wah sweeping automatically"), MinVersion(3)]
    public bool autoWah;
    [Name("Frequency"), Description("For manual wah control, what frequency is it at (0,1 range)"), MinVersion(3)]
    public float frequency;

    public new FxSendWah Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        resonance = reader.ReadFloat();
        lowerFreq = reader.ReadFloat();
        upperFreq = reader.ReadFloat();
        lfoFreq = reader.ReadFloat();
        magic = reader.ReadFloat();

        if (revision >= 2)
        {
            tempoSync = reader.ReadBoolean();
            tempo = reader.ReadFloat();
            syncType = Symbol.Read(reader);
        }

        if (revision >= 3)
        {
            distAmount = reader.ReadFloat();
            autoWah = reader.ReadBoolean();
            frequency = reader.ReadFloat();
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

        writer.WriteFloat(resonance);
        writer.WriteFloat(lowerFreq);
        writer.WriteFloat(upperFreq);
        writer.WriteFloat(lfoFreq);
        writer.WriteFloat(magic);

        if (revision >= 2)
        {
            writer.WriteBoolean(tempoSync);
            writer.WriteFloat(tempo);
            Symbol.Write(writer, syncType);
        }

        if (revision >= 3)
        {
            writer.WriteFloat(distAmount);
            writer.WriteBoolean(autoWah);
            writer.WriteFloat(frequency);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
