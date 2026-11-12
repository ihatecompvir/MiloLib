using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendReverb"), Description("A reverb effect.")]
public class FxSendReverb : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Environment"), Description("The reverb environment to simulate.")]
    public Symbol environmentPreset = Symbol.Empty;

    [Name("Pre Delay Ms"), Description("Pre-delay in milliseconds"), MinVersion(2)]
    public float preDelayMs;
    [Name("High Cut"), Description("High cutoff frequency in Hz"), MinVersion(2)]
    public float highCut;
    [Name("Low Cut"), Description("Low cutoff frequency in Hz"), MinVersion(2)]
    public float lowCut;
    [Name("Room Size"), Description("Room size (0-1)"), MinVersion(2)]
    public float roomSize;
    [Name("Damping"), Description("Amount of damping (0-1)"), MinVersion(2)]
    public float damping;
    [Name("Diffusion"), Description("Amount of dffusion (0-1)"), MinVersion(2)]
    public float diffusion;
    [Name("Early Late"), Description("Balance of early and late reflections; 0=early only, 1=late only"), MinVersion(2)]
    public float earlyLate;

    public new FxSendReverb Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        environmentPreset = Symbol.Read(reader);

        if (revision >= 2)
        {
            preDelayMs = reader.ReadFloat();
            highCut = reader.ReadFloat();
            lowCut = reader.ReadFloat();
            roomSize = reader.ReadFloat();
            damping = reader.ReadFloat();
            diffusion = reader.ReadFloat();
            earlyLate = reader.ReadFloat();
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

        Symbol.Write(writer, environmentPreset);

        if (revision >= 2)
        {
            writer.WriteFloat(preDelayMs);
            writer.WriteFloat(highCut);
            writer.WriteFloat(lowCut);
            writer.WriteFloat(roomSize);
            writer.WriteFloat(damping);
            writer.WriteFloat(diffusion);
            writer.WriteFloat(earlyLate);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
