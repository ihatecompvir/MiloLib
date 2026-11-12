using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendEQ"), Description("An equalizer effect.")]
public class FxSendEQ : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Description("High frequency cutoff, in Hz")]
    public float highFreqCutoff;
    [Description("High frequency gain, in dB")]
    public float highFreqGain;
    [Description("Mid frequency cutoff, in Hz")]
    public float midFreqCutoff;
    [Description("Mid frequency bandwidth, in Hz")]
    public float midFreqBandwidth;
    [Description("Mid frequency gain, in dB")]
    public float midFreqGain;
    [Description("Low frequency cutoff, in Hz")]
    public float lowFreqCutoff;
    [Description("Low frequency gain, in dB")]
    public float lowFreqGain;

    [Name("Low Pass Cutoff"), Description("Low pass filter cutoff, in Hz"), MinVersion(2)]
    public float lowPassCutoff;
    [Name("Low Pass Resonance"), Description("Low pass filter resonance, in dB"), MinVersion(2)]
    public float lowPassReso;
    [Name("High Pass Cutoff"), Description("High pass filter cutoff, in Hz"), MinVersion(2)]
    public float highPassCutoff;
    [Name("High Pass Resonance"), Description("High pass filter resonance, in dB"), MinVersion(2)]
    public float highPassReso;

    [Name("LR Mode"), Description("Enable or disable Linkwitz-Riley mode"), MinVersion(3), MaxVersion(3)]
    public bool lrMode;
    [Name("Transition Time"), Description("Transition time for gain changes, in ms"), MinVersion(3)]
    public float transitionTime = 25.0f;

    public new FxSendEQ Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        highFreqCutoff = reader.ReadFloat();
        highFreqGain = reader.ReadFloat();
        midFreqCutoff = reader.ReadFloat();
        midFreqBandwidth = reader.ReadFloat();
        midFreqGain = reader.ReadFloat();
        lowFreqCutoff = reader.ReadFloat();
        lowFreqGain = reader.ReadFloat();

        if (revision >= 2)
        {
            lowPassCutoff = reader.ReadFloat();
            lowPassReso = reader.ReadFloat();
            highPassCutoff = reader.ReadFloat();
            highPassReso = reader.ReadFloat();
        }

        if (revision >= 3)
        {
            if (revision == 3)
            {
                lrMode = reader.ReadBoolean();
            }

            transitionTime = reader.ReadFloat();
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

        writer.WriteFloat(highFreqCutoff);
        writer.WriteFloat(highFreqGain);
        writer.WriteFloat(midFreqCutoff);
        writer.WriteFloat(midFreqBandwidth);
        writer.WriteFloat(midFreqGain);
        writer.WriteFloat(lowFreqCutoff);
        writer.WriteFloat(lowFreqGain);

        if (revision >= 2)
        {
            writer.WriteFloat(lowPassCutoff);
            writer.WriteFloat(lowPassReso);
            writer.WriteFloat(highPassCutoff);
            writer.WriteFloat(highPassReso);
        }

        if (revision >= 3)
        {
            if (revision == 3)
            {
                writer.WriteBoolean(lrMode);
            }

            writer.WriteFloat(transitionTime);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
