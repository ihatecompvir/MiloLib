using MiloLib.Assets.Rnd;
using MiloLib.Utils;
using MiloLib.Classes;

namespace MiloLib.Assets.UI;

[Name("PanelDir"), Description("Top-level UI Object, contains UI components and an optional camera")]
public class PanelDir : RndDir
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        // no dirs before this
        { Game.MiloGame.GuitarHero2_PS2, 2 },
        { Game.MiloGame.GuitarHero2_360, 2 },
        { Game.MiloGame.Phase, 2 },
        { Game.MiloGame.RockBand, 2 },
        { Game.MiloGame.RockBand2, 7 },
        { Game.MiloGame.LegoRockBand, 7 },
        { Game.MiloGame.TheBeatlesRockBand, 7 },
        { Game.MiloGame.GreenDayRockBand, 7 },
        { Game.MiloGame.RockBand3, 8 },
        { Game.MiloGame.DanceCentral, 8 },
        { Game.MiloGame.DanceCentral2, 8 },
        { Game.MiloGame.RockBandBlitz, 8 },
        { Game.MiloGame.DanceCentral3, 8 }
    };
    private ushort altRevision;
    private ushort revision;
    [Name("Camera"), Description("Camera to use in game, else standard UI cam"), MinVersion(1)]
    public Symbol cam = Symbol.Empty;

    [Name("Use Specified Camera"), Description("Forces the usage of the 'cam' property to render in milo. This is a milo only feature.")]
    public bool useSpecifiedCam;

    [Name("Back View Only Panels"), Description("Additional panels to display behind this panel."), MinVersion(5)]
    public List<Symbol> backPanels = new();
    [Name("Front View Only Panels"), Description("Additional panels to display in front of this panel."), MinVersion(5)]
    public List<Symbol> frontPanels = new();
    [Name("Show View Only Panels"), Description("Whether or no this panel displays its view only panels"), MinVersion(6)]
    public bool showViewOnlyPanels;

    [Name("Post Processes Before Draw"), Description("Trigger postprocs before drawing this panel. If checked, this panel will not be affected by the postprocs."), MinVersion(4)]
    public bool canEndWorld;

    [Name("Test Event"), Description("event to test"), MinVersion(2), MaxVersion(2)]
    public Symbol oldTestEvent = Symbol.Empty;

    protected void ReadRndDirBase(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        base.Read(reader, false, parent, entry);
    }

    protected void WriteRndDirBase(EndianWriter writer, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        base.Write(writer, false, parent, entry);
    }

    public PanelDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new PanelDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        // if this is not an entry inside another directory (as in, not an inlined subdir), read the camera
        if (!entry.isProxy)
        {
            if (revision != 0)
            {
                cam = Symbol.Read(reader);
            }
        }

        if (revision <= 1)
        {
            return this;
        }

        if (revision == 2)
        {
            oldTestEvent = Symbol.Read(reader);
            if (standalone)
            {
                AssetHelpers.ReadEnd(reader, parent, entry, revision);
            }
            return this;
        }

        if (revision > 3)
        {
            canEndWorld = reader.ReadBoolean();
        }

        if (revision > 4)
        {
            backPanels = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            frontPanels = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 5)
        {
            showViewOnlyPanels = reader.ReadBoolean();
        }

        if (revision > 7)
        {
            useSpecifiedCam = reader.ReadBoolean();
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

        if (!entry.isProxy)
        {
            if (revision != 0)
            {
                Symbol.Write(writer, cam);
            }
        }

        if (revision <= 1)
        {
            if (standalone)
            {
                writer.WriteEndBytes();
            }
            return;
        }

        if (revision == 2)
        {
            Symbol.Write(writer, oldTestEvent);
            if (standalone)
            {
                writer.WriteEndBytes();
            }
            return;
        }

        if (revision > 3)
        {
            writer.WriteBoolean(canEndWorld);
        }

        if (revision > 4)
        {
            AssetHelpers.WriteList(writer, backPanels, panel => Symbol.Write(writer, panel));

            AssetHelpers.WriteList(writer, frontPanels, panel => Symbol.Write(writer, panel));
        }

        if (revision > 5)
        {
            writer.WriteBoolean(showViewOnlyPanels);
        }

        if (revision > 7)
        {
            writer.WriteBoolean(useSpecifiedCam);
        }

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
