using MiloLib.Assets.World;
using MiloLib.Utils;
using MiloLib.Classes;

namespace MiloLib.Assets.Rnd;

[Name("RndDir"), Description("A RndDir specially tracks drawable and animatable objects.")]
public class RndDir : ObjectDir
{
    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();
    public RndDrawable draw = new();
    [MinVersion(1)]
    public RndTrans trans = new();

    [Name("Environ"), MinVersion(9)]
    public Symbol environ = Symbol.Empty;

    [Name("Test Event"), Description("Test event"), MinVersion(10)]
    public Symbol testEvent = Symbol.Empty;

    [MinVersion(6), MaxVersion(6)]
    public List<float> unkFloats = new();

    [MinVersion(0), MaxVersion(8)]
    public RndPollable poll = new();
    [MinVersion(0), MaxVersion(8)]
    public Symbol oldSymbol1 = Symbol.Empty;
    [MinVersion(0), MaxVersion(8)]
    public Symbol oldSymbol2 = Symbol.Empty;

    public RndDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new RndDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (hasDeferredPostLoadFields)
        {
            return this;
        }

        // the RndDir ends immediately when it is an entry unless the entry is a RndDir or Character, probably others too, why?
        // TODO: investigate if this is just for RB3/DC1 or others too
        // also refactor this shite
        if (entry.isProxy && entry.type.value != "Character" && entry.type.value != "RndDir" && entry.type.value != "RenderDir" && entry.type.value != "BandCrowdMeterDir" && entry.type.value != "CrowdMeterIcon" && entry.type.value != "EndingBonusDir" && entry.type.value != "UnisonIcon" && entry.type.value != "BandScoreboard" && entry.type.value != "BandStarDisplay" && entry.type.value != "PanelDir" && entry.type.value != "LoadingPanelDir" && entry.type.value != "MoveDir" && entry.type.value != "SkeletonDir" && entry.type.value != "WorldDir" && entry.type.value != "WorldFx" && entry.type.value != "VocalTrackDir" && entry.type.value != "OvershellDir" && entry.type.value != "BandCharacter" && entry.type.value != "GemTrackDir" && entry.type.value != "TrackDir" && entry.type.value != "OverdriveMeterDir" && entry.type.value != "StreakMeterDir" && entry.type.value != "PitchArrowDir" && entry.type.value != "CompositeCharacter" && entry.type.value != "UILabelDir" && entry.type.value != "P9Character" && entry.type.value != "H2HTrackPanelDir" && entry.type.value != "BackgroundDir" && entry.type.value != "BandPowerMeterDir" && entry.type.value != "ScoreboardDir" && entry.type.value != "EventCompleteDir" && entry.type.value != "HamListRibbon" && entry.type.value != "HamPhotoDisplay" && entry.type.value != "HamPhraseMeter" && entry.type.value != "HamScrollSpeedIndicator" && entry.type.value != "HamCharacter" && entry.type.value != "HamVisDir" && entry.type.value != "UIListDir" && entry.type.value != "HorizonDir" && entry.type.value != "GroundPlaneVis" && entry.type.value != "PlotDir" && entry.type.value != "WorldTrackDir" && entry.type.value != "SingleTrackDir" && entry.type.value != "DynamicLeaderboard")
        {
            if (!(this is WorldInstance wiRev0Read && wiRev0Read.WorldInstanceRevision == 0))
            {
                return this;
            }
        }

        ReadRndDirPostLoadFields(reader, parent, entry);

        if (standalone)
        {
            ReadAndVerifyEndBytes(reader, parent, entry);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);

        if (hasDeferredPostLoadFields)
        {
            return;
        }

        if (entry.isProxy && entry.type.value != "Character" && entry.type.value != "RndDir" && entry.type.value != "RenderDir" && entry.type.value != "BandCrowdMeterDir" && entry.type.value != "CrowdMeterIcon" && entry.type.value != "EndingBonusDir" && entry.type.value != "UnisonIcon" && entry.type.value != "BandScoreboard" && entry.type.value != "BandStarDisplay" && entry.type.value != "PanelDir" && entry.type.value != "LoadingPanelDir" && entry.type.value != "MoveDir" && entry.type.value != "SkeletonDir" && entry.type.value != "WorldDir" && entry.type.value != "WorldFx" && entry.type.value != "VocalTrackDir" && entry.type.value != "OvershellDir" && entry.type.value != "BandCharacter" && entry.type.value != "GemTrackDir" && entry.type.value != "TrackDir" && entry.type.value != "OverdriveMeterDir" && entry.type.value != "StreakMeterDir" && entry.type.value != "PitchArrowDir" && entry.type.value != "CompositeCharacter" && entry.type.value != "UILabelDir" && entry.type.value != "P9Character" && entry.type.value != "H2HTrackPanelDir" && entry.type.value != "BackgroundDir" && entry.type.value != "BandPowerMeterDir" && entry.type.value != "ScoreboardDir" && entry.type.value != "EventCompleteDir" && entry.type.value != "HamListRibbon" && entry.type.value != "HamPhotoDisplay" && entry.type.value != "HamPhraseMeter" && entry.type.value != "HamScrollSpeedIndicator" && entry.type.value != "HamCharacter" && entry.type.value != "HamVisDir" && entry.type.value != "UIListDir" && entry.type.value != "HorizonDir" && entry.type.value != "GroundPlaneVis" && entry.type.value != "PlotDir" && entry.type.value != "WorldTrackDir" && entry.type.value != "SingleTrackDir" && entry.type.value != "DynamicLeaderboard")
        {
            // hack
            if (!(this is WorldInstance rev0WorldInstance && rev0WorldInstance.WorldInstanceRevision == 0))
            {
                return;
            }
        }

        WriteRndDirPostLoadFields(writer, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }

    }

    protected void ReadRndDirPostLoadFields(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        anim = anim.Read(reader, parent, entry);
        draw = draw.Read(reader, false, parent, entry);
        if (revision != 0)
        {
            trans = trans.Read(reader, false, parent, entry);
        }

        if (revision < 9)
        {
            poll = poll.Read(reader, false, parent, entry);
            oldSymbol1 = Symbol.Read(reader);
            oldSymbol2 = Symbol.Read(reader);
        }
        else if (!(parent.revision == 26 && revision == 10))
        {
            environ = Symbol.Read(reader);
            if (revision > 2 && revision != 9)
            {
                testEvent = Symbol.Read(reader);
            }
        }

        if (revision == 6)
        {
            for (int i = 0; i < 8; i++)
            {
                unkFloats.Add(reader.ReadFloat());
            }
        }
    }

    protected void WriteRndDirPostLoadFields(EndianWriter writer, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        anim.Write(writer);
        draw.Write(writer, false, parent, null);
        if (revision != 0)
        {
            trans.Write(writer, false, parent, null);
        }

        if (revision < 9)
        {
            poll.Write(writer, false, parent, entry);
            Symbol.Write(writer, oldSymbol1);
            Symbol.Write(writer, oldSymbol2);
        }
        else if (!(parent.revision == 26 && revision == 10))
        {
            Symbol.Write(writer, environ);
            if (revision > 2 && revision != 9)
            {
                Symbol.Write(writer, testEvent);
            }
        }

        if (revision == 6)
        {
            for (int i = 0; i < 8; i++)
            {
                writer.WriteFloat(unkFloats[i]);
            }
        }
    }

    protected override void ReadDeferredPostLoadFields(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        base.ReadDeferredPostLoadFields(reader, parent, entry);
        ReadRndDirPostLoadFields(reader, parent, entry);
    }

    protected override void WriteDeferredPostLoadFields(EndianWriter writer, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        base.WriteDeferredPostLoadFields(writer, parent, entry);
        WriteRndDirPostLoadFields(writer, parent, entry);
    }


    public override bool IsDirectory()
    {
        return true;
    }
}
