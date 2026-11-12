using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSend"), Description("Base class for all sound FX processors")]
public class FxSend : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Next Send"), Description("The next effect in the chain")]
    public Symbol nextSend = Symbol.Empty;
    [Name("Stage"), Description("The relative order that this send is processed compared to other sends.")]
    public int stage;
    [Name("Bypass"), Description("Bypass the effect and stop it from processing")]
    public bool bypass;
    [Name("Dry Gain"), Description("Gain applied to dry signal (dB)"), MinVersion(5)]
    public float dryGain;
    [Name("Wet Gain"), Description("Gain applied to effect output (dB)"), MinVersion(5)]
    public float wetGain;
    [Name("Input Gain"), Description("Gain applied to effect input (dB)"), MinVersion(5)]
    public float inputGain;
    [Name("Channels"), Description("Which channels the FX applies to"), MinVersion(4)]
    public int channels;
    [Name("Reverb Mix DB"), Description("Reverb send for this effect"), MinVersion(7)]
    public float reverbMixDb;
    [Name("Reverb Enable"), Description("Enable reverb send"), MinVersion(7)]
    public bool reverbEnable;

    [MinVersion(2), MaxVersion(4)]
    private float oldRatio;

    public new FxSend Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        nextSend = Symbol.Read(reader);
        stage = reader.ReadInt32();

        if (revision < 5)
        {
            if (revision >= 2)
            {
                oldRatio = reader.ReadFloat();
            }

            if (revision >= 3)
            {
                bypass = reader.ReadBoolean();
            }
        }

        if (revision >= 4)
        {
            channels = reader.ReadInt32();
        }

        if (revision >= 5)
        {
            dryGain = reader.ReadFloat();
            wetGain = reader.ReadFloat();
            inputGain = reader.ReadFloat();
        }

        if (revision >= 6)
        {
            bypass = reader.ReadBoolean();
        }

        if (revision >= 7)
        {
            reverbMixDb = reader.ReadFloat();
            reverbEnable = reader.ReadBoolean();
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

        Symbol.Write(writer, nextSend);
        writer.WriteInt32(stage);

        if (revision < 5)
        {
            if (revision >= 2)
            {
                writer.WriteFloat(oldRatio);
            }

            if (revision >= 3)
            {
                writer.WriteBoolean(bypass);
            }
        }

        if (revision >= 4)
        {
            writer.WriteInt32(channels);
        }

        if (revision >= 5)
        {
            writer.WriteFloat(dryGain);
            writer.WriteFloat(wetGain);
            writer.WriteFloat(inputGain);
        }

        if (revision >= 6)
        {
            writer.WriteBoolean(bypass);
        }

        if (revision >= 7)
        {
            writer.WriteFloat(reverbMixDb);
            writer.WriteBoolean(reverbEnable);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
