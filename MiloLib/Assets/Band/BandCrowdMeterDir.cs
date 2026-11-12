using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandCrowdMeterDir"), Description("Crowd meter hud element, has a needle")]
public class BandCrowdMeterDir : RndDir
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        // no dirs before this
        { Game.MiloGame.GuitarHero2_PS2, 3 },
        { Game.MiloGame.GuitarHero2_360, 3 },
        //{ Game.MiloGame.RockBand, 19 },
        //{ Game.MiloGame.RockBand2, 20 },
        //{ Game.MiloGame.LegoRockBand, 20 },
        //{ Game.MiloGame.TheBeatlesRockBand, 22 },
        //{ Game.MiloGame.GreenDayRockBand, 22 },
        { Game.MiloGame.RockBand3, 3 },
    };
    private ushort altRevision;
    private ushort revision;


    [Name("Colors"), Description("The colors that will be shown as the meter decreases."), MinVersion(2)]
    public List<HmxColor4> colors = new();

    [Name("Peak Value"), Description("Peak state value"), MinVersion(1)]
    public float peakValue;

    [MaxVersion(2)]
    private List<Symbol> groups = new();

    private byte[] gh2OpaqueBytes = Array.Empty<byte>();

    public uint unkInt;

    [Name("Needle Anim"), Description("anim to drive the needle")]
    public Symbol needleAnim = Symbol.Empty;
    [Name("Warning Anim"), Description("animation that is played when below the warning level")]
    public Symbol warningAnim = Symbol.Empty;
    [Name("Red Anim"), Description("animation that is played when in the red state")]
    public Symbol redAnim = Symbol.Empty;
    [Name("Yellow Anim"), Description("animation that is played when in the yellow state")]
    public Symbol yellowAnim = Symbol.Empty;
    [Name("Green Anim"), Description("animation that is played when in the green state")]
    public Symbol greenAnim = Symbol.Empty;


    public BandCrowdMeterDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandCrowdMeterDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        // GH2's BandCrowdMeterDir rev 3 is not the RB-era layout so this is a temporary hack until i figure this out proper
        if (parent.revision <= 25 && standalone)
        {
            gh2OpaqueBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
            return this;
        }

        if (parent.revision <= 25)
        {
            unkInt = reader.ReadUInt32();

            warningAnim = Symbol.Read(reader);
            redAnim = Symbol.Read(reader);
            yellowAnim = Symbol.Read(reader);
            greenAnim = Symbol.Read(reader);
            needleAnim = Symbol.Read(reader);

        }
        else
        {
            // fields only read when dir is not proxied
            if (!entry.isProxy)
            {
                if (revision < 3)
                {
                    groups = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
                }

                if (revision >= 2)
                {
                    colors = AssetHelpers.ReadList(reader, () => new HmxColor4().Read(reader));
                }
            }

            if (revision >= 1)
            {
                peakValue = reader.ReadFloat();
            }
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

        if (parent.revision <= 25 && gh2OpaqueBytes.Length != 0)
        {
            writer.WriteBlock(gh2OpaqueBytes);
            if (standalone)
            {
                writer.WriteEndBytes();
            }

            return;
        }

        if (parent.revision <= 25)
        {
            writer.WriteUInt32(unkInt);
            Symbol.Write(writer, warningAnim);
            Symbol.Write(writer, redAnim);
            Symbol.Write(writer, yellowAnim);
            Symbol.Write(writer, greenAnim);
            Symbol.Write(writer, needleAnim);

            base.Write(writer, false, parent, entry);

            if (standalone)
            {
                writer.WriteEndBytes();
            }
            return;
        }
        else
        {
            if (!entry.isProxy)
            {
                if (revision < 3)
                {
                    AssetHelpers.WriteList(writer, groups, group => Symbol.Write(writer, group));
                }

                if (revision >= 2)
                {
                    AssetHelpers.WriteList(writer, colors, color => color.Write(writer));
                }

            }
            if (revision >= 1)
            {
                writer.WriteFloat(peakValue);
            }

            base.Write(writer, false, parent, entry);

            if (standalone)
            {
                writer.WriteEndBytes();
            }
        }
    }


    public override bool IsDirectory()
    {
        return true;
    }
}
