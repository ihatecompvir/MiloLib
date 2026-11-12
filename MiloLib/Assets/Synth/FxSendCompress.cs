using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendCompress"), Description("A compression/expansion effect.")]
public class FxSendCompress : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Threshold"), Description("threshold (in dB) at which compression is applied")]
    public float thresholdDB;
    [Name("Comp Ratio"), Description("Compression factor - ratio of input level to output level")]
    public float ratio;
    [Name("Output Level"), Description("output level for a maxed signal, in dB")]
    public float outputLevel;
    [Name("Attack"), Description("Attack time in seconds")]
    public float attack;
    [Name("Release"), Description("Release time in seconds")]
    public float release;

    [MaxVersion(1)]
    private int oldInt;

    [Name("Exp Ratio"), Description("Expansion factor - ratio of input level to output level. The expander uses the same threshold as the compressor."), MinVersion(3)]
    public float expRatio;
    [Name("Exp Attack"), Description("Attack time in seconds"), MinVersion(3)]
    public float expAttack;
    [Name("Exp Release"), Description("Release time in seconds"), MinVersion(3)]
    public float expRelease;

    [Name("Gate Threshold"), Description("threshold (in dB) at which gating is applied"), MinVersion(4)]
    public float gateThresholdDB;

    public new FxSendCompress Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        thresholdDB = reader.ReadFloat();
        ratio = reader.ReadFloat();
        outputLevel = reader.ReadFloat();
        attack = reader.ReadFloat();
        release = reader.ReadFloat();

        if (revision < 2)
        {
            oldInt = reader.ReadInt32();
        }

        if (revision >= 3)
        {
            expRatio = reader.ReadFloat();
            expAttack = reader.ReadFloat();
            expRelease = reader.ReadFloat();
        }

        if (revision >= 4)
        {
            gateThresholdDB = reader.ReadFloat();
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

        writer.WriteFloat(thresholdDB);
        writer.WriteFloat(ratio);
        writer.WriteFloat(outputLevel);
        writer.WriteFloat(attack);
        writer.WriteFloat(release);

        if (revision < 2)
        {
            writer.WriteInt32(oldInt);
        }

        if (revision >= 3)
        {
            writer.WriteFloat(expRatio);
            writer.WriteFloat(expAttack);
            writer.WriteFloat(expRelease);
        }

        if (revision >= 4)
        {
            writer.WriteFloat(gateThresholdDB);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
