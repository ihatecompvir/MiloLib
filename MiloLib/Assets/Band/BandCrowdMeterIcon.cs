using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("CrowdMeterIcon"), Description("Individual player icon for crowd meter")]
public class BandCrowdMeterIcon : RndDir
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        { Game.MiloGame.GuitarHero2_PS2, 0 },
        { Game.MiloGame.GuitarHero2_360, 0 },
        { Game.MiloGame.Phase, 0 },
        { Game.MiloGame.RockBand, 0 },
        { Game.MiloGame.RockBand2, 0 },
        { Game.MiloGame.LegoRockBand, 0 },
        { Game.MiloGame.TheBeatlesRockBand, 0 },
        { Game.MiloGame.GreenDayRockBand, 0 },
        { Game.MiloGame.RockBand3, 0 },
        { Game.MiloGame.DanceCentral, 0 },
        { Game.MiloGame.DanceCentral2, 0 },
        { Game.MiloGame.RockBandBlitz, 0 },
        { Game.MiloGame.DanceCentral3, 0 }
    };

    private ushort altRevision;
    private ushort revision;

    public BandCrowdMeterIcon(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandCrowdMeterIcon Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

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
