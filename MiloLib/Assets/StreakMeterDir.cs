using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("StreakMeterDir"), Description("streak meter for band tracks")]
public class StreakMeterDir : RndDir
{
    private ushort altRevision;
    private ushort revision;

    public int streakMultiplier;
    public int bandMultiplier;
    public int maxMultiplier;

    public Symbol newStreakTrig = Symbol.Empty;
    public Symbol endStreakTrig = Symbol.Empty;
    [MaxVersion(2)]
    public Symbol unkTrig = Symbol.Empty;
    public Symbol multiMeterAnim = Symbol.Empty;
    [MinVersion(1)]
    public Symbol multiplierLabel = Symbol.Empty;
    [MaxVersion(0)]
    public Symbol textObject = Symbol.Empty;
    [MinVersion(2)]
    public Symbol meterWipeAnim = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol matObject = Symbol.Empty;
    public Symbol starDeployTrig = Symbol.Empty;
    public Symbol endOverdriveTrig = Symbol.Empty;
    public Symbol resetTrig = Symbol.Empty;

    public StreakMeterDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new StreakMeterDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        streakMultiplier = reader.ReadInt32();
        bandMultiplier = reader.ReadInt32();
        maxMultiplier = reader.ReadInt32();
        if (!entry.isProxy)
        {
            newStreakTrig = Symbol.Read(reader);
            endStreakTrig = Symbol.Read(reader);
            if (revision < 3)
            {
                unkTrig = Symbol.Read(reader);
            }

            multiMeterAnim = Symbol.Read(reader);
            if (revision >= 1)
            {
                multiplierLabel = Symbol.Read(reader);
            }
            else
            {
                textObject = Symbol.Read(reader);
            }

            if (revision >= 2)
            {
                meterWipeAnim = Symbol.Read(reader);
            }
            else
            {
                matObject = Symbol.Read(reader);
            }

            starDeployTrig = Symbol.Read(reader);
            endOverdriveTrig = Symbol.Read(reader);
            resetTrig = Symbol.Read(reader);
        }

        base.Read(reader, false, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        writer.WriteInt32(streakMultiplier);
        writer.WriteInt32(bandMultiplier);
        writer.WriteInt32(maxMultiplier);
        if (!entry.isProxy)
        {
            Symbol.Write(writer, newStreakTrig);
            Symbol.Write(writer, endStreakTrig);
            if (revision < 3)
            {
                Symbol.Write(writer, unkTrig);
            }

            Symbol.Write(writer, multiMeterAnim);
            if (revision >= 1)
            {
                Symbol.Write(writer, multiplierLabel);
            }
            else
            {
                Symbol.Write(writer, textObject);
            }

            if (revision >= 2)
            {
                Symbol.Write(writer, meterWipeAnim);
            }
            else
            {
                Symbol.Write(writer, matObject);
            }

            Symbol.Write(writer, starDeployTrig);
            Symbol.Write(writer, endOverdriveTrig);
            Symbol.Write(writer, resetTrig);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }

}
