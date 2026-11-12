using MiloLib.Assets.Band;
using MiloLib.Assets.Flow;
using FlowObject = MiloLib.Assets.Flow.Flow;
using MiloLib.Assets.Band.UI;
using MiloLib.Assets.Char;
using MiloLib.Assets.Ham;
using MiloLib.Assets.P9;
using MiloLib.Assets.Rnd;
using MiloLib.Assets.Synth;
using MiloLib.Assets.UI;
using MiloLib.Assets.World;
using MiloLib.Classes;
using MiloLib.Utils;
using MiloLib.Exceptions;
using System.Collections.Frozen;
using System.Diagnostics;
using System.Text;
using static MiloLib.Assets.Ham.Range;

namespace MiloLib.Assets;

public class DirectoryMeta
{
    public enum Platform
    {
        GameCube,
        PS2,
        PS3,
        Wii,
        Xbox,
        [Name("PC or iPod")]
        PC_or_iPod
    }

    private delegate Object DirectoryFactory(ushort revision);
    private delegate void DirectoryReader(EndianReader reader, Object dir, DirectoryMeta meta, Entry entry);
    private delegate void DirectoryWriter(EndianWriter writer, Object dir, DirectoryMeta meta, Entry entry);
    private delegate Object EntryReader(EndianReader reader, DirectoryMeta meta, Entry entry);
    private delegate void EntryWriter(EndianWriter writer, Object obj, DirectoryMeta meta, Entry entry);

    // optimization using frozen dictionaries for fast O(1) lookup instead of a giant switch statement or if-else chain
    private static readonly FrozenDictionary<string, DirectoryFactory> DirectoryFactories = new Dictionary<string, DirectoryFactory>
    {
        { "ObjectDir", (rev) => new ObjectDir(rev) },
        { "Flow", (rev) => new FlowObject(rev) },
        { "EndingBonusDir", (rev) => new RndDir(rev) },
        { "RenderDir", (rev) => new RenderDir(rev) },
        { "RndDir", (rev) => new RndDir(rev) },
        { "PanelDir", (rev) => new PanelDir(rev) },
        { "BackgroundDir", (rev) => new BackgroundDir(rev) },
        { "CharClipSet", (rev) => new CharClipSet(rev) },
        { "LoadingPanelDir", (rev) => new LoadingPanelDir(rev) },
        { "WorldFx", (rev) => new WorldFx(rev) },
        { "WorldDir", (rev) => new WorldDir(rev) },
        { "Character", (rev) => new Character(rev) },
        { "CompositeCharacter", (rev) => new CompositeCharacter(rev) },
        { "UILabelDir", (rev) => new UILabelDir(rev) },
        { "UIListDir", (rev) => new UIListDir(rev) },
        { "DynamicLeaderboard", (rev) => new DynamicLeaderboard(rev) },
        { "SingleTrackDir", (rev) => new SingleTrackDir(rev) },
        { "WorldTrackDir", (rev) => new WorldTrackDir(rev) },
        { "PlotDir", (rev) => new PlotDir(rev) },
        { "HorizonDir", (rev) => new HorizonDir(rev) },
        { "GroundPlaneVis", (rev) => new GroundPlaneVis(rev) },
        { "BandCrowdMeterDir", (rev) => new BandCrowdMeterDir(rev) },
        { "BandLeadMeter", (rev) => new BandLeadMeter(rev) },
        { "BandScoreDisplay", (rev) => new BandScoreDisplay(rev) },
        { "BandStarMeterDir", (rev) => new BandStarMeterDir(rev) },
        { "BandStreakDisplay", (rev) => new BandStreakDisplay(rev) },
        { "GHTrackDir", (rev) => new GHTrackDir(rev) },
        { "CrowdMeterIcon", (rev) => new BandCrowdMeterIcon(rev) },
        { "CharBoneDir", (rev) => new CharBoneDir(rev) },
        { "BandCharacter", (rev) => new BandCharacter(rev) },
        { "WorldInstance", (rev) => new WorldInstance(rev) },
        { "TrackDir", (rev) => new TrackDir(rev) },
        { "GemTrackDir", (rev) => new GemTrackDir(rev) },
        { "TrackPanelDir", (rev) => new TrackPanelDir(rev) },
        { "H2HTrackPanelDir", (rev) => new H2HTrackPanelDir(rev) },
        { "EventCompleteDir", (rev) => new EventCompleteDir(rev) },
        { "WorldMapDir", (rev) => new WorldMapDir(rev) },
        { "UnisonIcon", (rev) => new UnisonIcon(rev) },
        { "BandScoreboard", (rev) => new BandScoreboard(rev) },
        { "BandPowerMeterDir", (rev) => new BandPowerMeterDir(rev) },
        { "ScoreboardDir", (rev) => new ScoreboardDir(rev) },
        { "BandStarDisplay", (rev) => new BandStarDisplay(rev) },
        { "VocalTrackDir", (rev) => new VocalTrackDir(rev) },
        { "MoveDir", (rev) => new MoveDir(rev) },
        { "HamListRibbon", (rev) => new HamListRibbon(rev) },
        { "HamPhotoDisplay", (rev) => new HamPhotoDisplay(rev) },
        { "HamPhraseMeter", (rev) => new HamPhraseMeter(rev) },
        { "HamScrollSpeedIndicator", (rev) => new HamScrollSpeedIndicator(rev) },
        { "HamCharacter", (rev) => new HamCharacter(rev) },
        { "HamVisDir", (rev) => new HamVisDir(rev) },
        { "SkeletonDir", (rev) => new SkeletonDir(rev) },
        { "OvershellDir", (rev) => new OvershellDir(rev) },
        { "OverdriveMeterDir", (rev) => new OverdriveMeterDir(rev) },
        { "StreakMeterDir", (rev) => new StreakMeterDir(rev) },
        { "PitchArrowDir", (rev) => new PitchArrowDir(rev) },
        { "SynthDir", (rev) => new SynthDir(rev) },
        { "P9Character", (rev) => new P9Character(rev) },
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, DirectoryReader> DirectoryReaders = new Dictionary<string, DirectoryReader>
    {
        { "ObjectDir", (r, d, m, e) => ((ObjectDir)d).Read(r, true, m, e) },
        { "Flow", (r, d, m, e) => ((FlowObject)d).Read(r, true, m, e) },
        { "EndingBonusDir", (r, d, m, e) => ((RndDir)d).Read(r, true, m, e) },
        { "RenderDir", (r, d, m, e) => ((RenderDir)d).Read(r, true, m, e) },
        { "RndDir", (r, d, m, e) => ((RndDir)d).Read(r, true, m, e) },
        { "PanelDir", (r, d, m, e) => ((PanelDir)d).Read(r, true, m, e) },
        { "BackgroundDir", (r, d, m, e) => ((BackgroundDir)d).Read(r, true, m, e) },
        { "CharClipSet", (r, d, m, e) => ((CharClipSet)d).Read(r, true, m, e) },
        { "LoadingPanelDir", (r, d, m, e) => ((LoadingPanelDir)d).Read(r, true, m, e) },
        { "WorldFx", (r, d, m, e) => ((WorldFx)d).Read(r, true, m, e) },
        { "WorldDir", (r, d, m, e) => ((WorldDir)d).Read(r, true, m, e) },
        { "Character", (r, d, m, e) => ((Character)d).Read(r, true, m, e) },
        { "CompositeCharacter", (r, d, m, e) => ((CompositeCharacter)d).Read(r, true, m, e) },
        { "UILabelDir", (r, d, m, e) => ((UILabelDir)d).Read(r, true, m, e) },
        { "UIListDir", (r, d, m, e) => ((UIListDir)d).Read(r, true, m, e) },
        { "DynamicLeaderboard", (r, d, m, e) => ((DynamicLeaderboard)d).Read(r, true, m, e) },
        { "SingleTrackDir", (r, d, m, e) => ((SingleTrackDir)d).Read(r, true, m, e) },
        { "WorldTrackDir", (r, d, m, e) => ((WorldTrackDir)d).Read(r, true, m, e) },
        { "PlotDir", (r, d, m, e) => ((PlotDir)d).Read(r, true, m, e) },
        { "HorizonDir", (r, d, m, e) => ((HorizonDir)d).Read(r, true, m, e) },
        { "GroundPlaneVis", (r, d, m, e) => ((GroundPlaneVis)d).Read(r, true, m, e) },
        { "BandCrowdMeterDir", (r, d, m, e) => ((BandCrowdMeterDir)d).Read(r, true, m, e) },
        { "BandLeadMeter", (r, d, m, e) => ((BandLeadMeter)d).Read(r, true, m, e) },
        { "BandScoreDisplay", (r, d, m, e) => ((BandScoreDisplay)d).Read(r, true, m, e) },
        { "BandStarMeterDir", (r, d, m, e) => ((BandStarMeterDir)d).Read(r, true, m, e) },
        { "BandStreakDisplay", (r, d, m, e) => ((BandStreakDisplay)d).Read(r, true, m, e) },
        { "GHTrackDir", (r, d, m, e) => ((GHTrackDir)d).Read(r, true, m, e) },
        { "CrowdMeterIcon", (r, d, m, e) => ((BandCrowdMeterIcon)d).Read(r, true, m, e) },
        { "CharBoneDir", (r, d, m, e) => ((CharBoneDir)d).Read(r, true, m, e) },
        { "BandCharacter", (r, d, m, e) => ((BandCharacter)d).Read(r, true, m, e) },
        { "WorldInstance", (r, d, m, e) => ((WorldInstance)d).Read(r, true, m, e) },
        { "TrackDir", (r, d, m, e) => ((TrackDir)d).Read(r, true, m, e) },
        { "GemTrackDir", (r, d, m, e) => ((GemTrackDir)d).Read(r, true, m, e) },
        { "TrackPanelDir", (r, d, m, e) => ((TrackPanelDir)d).Read(r, true, m, e) },
        { "H2HTrackPanelDir", (r, d, m, e) => ((H2HTrackPanelDir)d).Read(r, true, m, e) },
        { "EventCompleteDir", (r, d, m, e) => ((EventCompleteDir)d).Read(r, true, m, e) },
        { "WorldMapDir", (r, d, m, e) => ((WorldMapDir)d).Read(r, true, m, e) },
        { "UnisonIcon", (r, d, m, e) => ((UnisonIcon)d).Read(r, true, m, e) },
        { "BandScoreboard", (r, d, m, e) => ((BandScoreboard)d).Read(r, true, m, e) },
        { "BandPowerMeterDir", (r, d, m, e) => ((BandPowerMeterDir)d).Read(r, true, m, e) },
        { "ScoreboardDir", (r, d, m, e) => ((ScoreboardDir)d).Read(r, true, m, e) },
        { "BandStarDisplay", (r, d, m, e) => ((BandStarDisplay)d).Read(r, true, m, e) },
        { "VocalTrackDir", (r, d, m, e) => ((VocalTrackDir)d).Read(r, true, m, e) },
        { "MoveDir", (r, d, m, e) => ((MoveDir)d).Read(r, true, m, e) },
        { "HamListRibbon", (r, d, m, e) => ((HamListRibbon)d).Read(r, true, m, e) },
        { "HamPhotoDisplay", (r, d, m, e) => ((HamPhotoDisplay)d).Read(r, true, m, e) },
        { "HamPhraseMeter", (r, d, m, e) => ((HamPhraseMeter)d).Read(r, true, m, e) },
        { "HamScrollSpeedIndicator", (r, d, m, e) => ((HamScrollSpeedIndicator)d).Read(r, true, m, e) },
        { "HamCharacter", (r, d, m, e) => ((HamCharacter)d).Read(r, true, m, e) },
        { "HamVisDir", (r, d, m, e) => ((HamVisDir)d).Read(r, true, m, e) },
        { "SkeletonDir", (r, d, m, e) => ((SkeletonDir)d).Read(r, true, m, e) },
        { "OvershellDir", (r, d, m, e) => ((OvershellDir)d).Read(r, true, m, e) },
        { "OverdriveMeterDir", (r, d, m, e) => ((OverdriveMeterDir)d).Read(r, true, m, e) },
        { "StreakMeterDir", (r, d, m, e) => ((StreakMeterDir)d).Read(r, true, m, e) },
        { "PitchArrowDir", (r, d, m, e) => ((PitchArrowDir)d).Read(r, true, m, e) },
        { "SynthDir", (r, d, m, e) => ((SynthDir)d).Read(r, true, m, e) },
        { "P9Character", (r, d, m, e) => ((P9Character)d).Read(r, true, m, e) },
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<string, DirectoryWriter> DirectoryWriters = new Dictionary<string, DirectoryWriter>
    {
        { "ObjectDir", (w, d, m, e) => ((ObjectDir)d).Write(w, true, m, e) },
        { "Flow", (w, d, m, e) => ((FlowObject)d).Write(w, true, m, e) },
        { "RenderDir", (w, d, m, e) => ((RenderDir)d).Write(w, true, m, e) },
        { "RndDir", (w, d, m, e) => ((RndDir)d).Write(w, true, m, e) },
        { "PanelDir", (w, d, m, e) => ((PanelDir)d).Write(w, true, m, e) },
        { "BackgroundDir", (w, d, m, e) => ((BackgroundDir)d).Write(w, true, m, e) },
        { "LoadingPanelDir", (w, d, m, e) => ((LoadingPanelDir)d).Write(w, true, m, e) },
        { "WorldFx", (w, d, m, e) => ((WorldFx)d).Write(w, true, m, e) },
        { "WorldDir", (w, d, m, e) => ((WorldDir)d).Write(w, true, m, e) },
        { "Character", (w, d, m, e) => ((Character)d).Write(w, true, m, e) },
        { "P9Character", (w, d, m, e) => ((P9Character)d).Write(w, true, m, e) },
        { "CharBoneDir", (w, d, m, e) => ((CharBoneDir)d).Write(w, true, m, e) },
        { "CharClipSet", (w, d, m, e) => ((CharClipSet)d).Write(w, true, m, e) },
        { "CompositeCharacter", (w, d, m, e) => ((CompositeCharacter)d).Write(w, true, m, e) },
        { "UILabelDir", (w, d, m, e) => ((UILabelDir)d).Write(w, true, m, e) },
        { "UIListDir", (w, d, m, e) => ((UIListDir)d).Write(w, true, m, e) },
        { "DynamicLeaderboard", (w, d, m, e) => ((DynamicLeaderboard)d).Write(w, true, m, e) },
        { "SingleTrackDir", (w, d, m, e) => ((SingleTrackDir)d).Write(w, true, m, e) },
        { "WorldTrackDir", (w, d, m, e) => ((WorldTrackDir)d).Write(w, true, m, e) },
        { "PlotDir", (w, d, m, e) => ((PlotDir)d).Write(w, true, m, e) },
        { "HorizonDir", (w, d, m, e) => ((HorizonDir)d).Write(w, true, m, e) },
        { "GroundPlaneVis", (w, d, m, e) => ((GroundPlaneVis)d).Write(w, true, m, e) },
        { "BandCrowdMeterDir", (w, d, m, e) => ((BandCrowdMeterDir)d).Write(w, true, m, e) },
        { "BandLeadMeter", (w, d, m, e) => ((BandLeadMeter)d).Write(w, true, m, e) },
        { "BandScoreDisplay", (w, d, m, e) => ((BandScoreDisplay)d).Write(w, true, m, e) },
        { "BandStarMeterDir", (w, d, m, e) => ((BandStarMeterDir)d).Write(w, true, m, e) },
        { "BandStreakDisplay", (w, d, m, e) => ((BandStreakDisplay)d).Write(w, true, m, e) },
        { "GHTrackDir", (w, d, m, e) => ((GHTrackDir)d).Write(w, true, m, e) },
        { "CrowdMeterIcon", (w, d, m, e) => ((BandCrowdMeterIcon)d).Write(w, true, m, e) },
        { "BandCharacter", (w, d, m, e) => ((BandCharacter)d).Write(w, true, m, e) },
        { "WorldInstance", (w, d, m, e) => ((WorldInstance)d).Write(w, true, m, e) },
        { "TrackDir", (w, d, m, e) => ((TrackDir)d).Write(w, true, m, e) },
        { "GemTrackDir", (w, d, m, e) => ((GemTrackDir)d).Write(w, true, m, e) },
        { "TrackPanelDir", (w, d, m, e) => ((TrackPanelDir)d).Write(w, true, m, e) },
        { "H2HTrackPanelDir", (w, d, m, e) => ((H2HTrackPanelDir)d).Write(w, true, m, e) },
        { "EventCompleteDir", (w, d, m, e) => ((EventCompleteDir)d).Write(w, true, m, e) },
        { "WorldMapDir", (w, d, m, e) => ((WorldMapDir)d).Write(w, true, m, e) },
        { "UnisonIcon", (w, d, m, e) => ((UnisonIcon)d).Write(w, true, m, e) },
        { "EndingBonusDir", (w, d, m, e) => ((RndDir)d).Write(w, true, m, e) },
        { "BandStarDisplay", (w, d, m, e) => ((BandStarDisplay)d).Write(w, true, m, e) },
        { "BandScoreboard", (w, d, m, e) => ((BandScoreboard)d).Write(w, true, m, e) },
        { "BandPowerMeterDir", (w, d, m, e) => ((BandPowerMeterDir)d).Write(w, true, m, e) },
        { "ScoreboardDir", (w, d, m, e) => ((ScoreboardDir)d).Write(w, true, m, e) },
        { "VocalTrackDir", (w, d, m, e) => ((VocalTrackDir)d).Write(w, true, m, e) },
        { "MoveDir", (w, d, m, e) => ((MoveDir)d).Write(w, true, m, e) },
        { "HamListRibbon", (w, d, m, e) => ((HamListRibbon)d).Write(w, true, m, e) },
        { "HamPhotoDisplay", (w, d, m, e) => ((HamPhotoDisplay)d).Write(w, true, m, e) },
        { "HamPhraseMeter", (w, d, m, e) => ((HamPhraseMeter)d).Write(w, true, m, e) },
        { "HamScrollSpeedIndicator", (w, d, m, e) => ((HamScrollSpeedIndicator)d).Write(w, true, m, e) },
        { "HamCharacter", (w, d, m, e) => ((HamCharacter)d).Write(w, true, m, e) },
        { "HamVisDir", (w, d, m, e) => ((HamVisDir)d).Write(w, true, m, e) },
        { "SkeletonDir", (w, d, m, e) => ((SkeletonDir)d).Write(w, true, m, e) },
        { "PitchArrowDir", (w, d, m, e) => ((PitchArrowDir)d).Write(w, true, m, e) },
        { "OvershellDir", (w, d, m, e) => ((OvershellDir)d).Write(w, true, m, e) },
        { "OverdriveMeterDir", (w, d, m, e) => ((OverdriveMeterDir)d).Write(w, true, m, e) },
        { "StreakMeterDir", (w, d, m, e) => ((StreakMeterDir)d).Write(w, true, m, e) },
        { "SynthDir", (w, d, m, e) => ((SynthDir)d).Write(w, true, m, e) },
    }.ToFrozenDictionary();

    private delegate void EntryReadAction(EndianReader reader, DirectoryMeta meta, Entry entry);
    private delegate void EntryWriteAction(EndianWriter writer, DirectoryMeta meta, Entry entry);

    private static readonly FrozenDictionary<string, EntryReadAction> EntryReadActions;
    private static readonly FrozenDictionary<string, EntryWriteAction> EntryWriteActions;

    static DirectoryMeta()
    {
        EntryReadActions = InitializeEntryReadActions().ToFrozenDictionary();
        EntryWriteActions = InitializeEntryWriteActions().ToFrozenDictionary();
    }

    private static Dictionary<string, EntryReadAction> InitializeEntryReadActions()
    {
        var actions = new Dictionary<string, EntryReadAction>();

        // simple object entries (don't require special cases or random bullshit - just read the object)
        var simpleObjectReaders = new Dictionary<string, System.Func<EndianReader, DirectoryMeta, Entry, Object>>
        {
            { "AnimFilter", (r, m, e) => new RndAnimFilter().Read(r, true, m, e) },
            { "PlotDef", (r, m, e) => new PlotDef().Read(r, true, m, e) },
            { "CharNeighborhood", (r, m, e) => new CharNeighborhood().Read(r, true, m, e) },
            { "CrowdExtrasManager", (r, m, e) => new CrowdExtrasManager().Read(r, true, m, e) },
            { "CrowdPreset", (r, m, e) => new CrowdPreset().Read(r, true, m, e) },
            { "PlayerPortrait", (r, m, e) => new PlayerPortrait().Read(r, true, m, e) },
            { "SyncWorldFx", (r, m, e) => new SyncWorldFx().Read(r, true, m, e) },
            { "TrackGeom", (r, m, e) => new TrackGeom().Read(r, true, m, e) },
            { "CamShotKeyframePreset", (r, m, e) => new CamShotKeyframePreset().Read(r, true, m, e) },
            { "SplineDeform", (r, m, e) => new SplineDeform().Read(r, true, m, e) },
            { "SplineDeformNode", (r, m, e) => new SplineDeformNode().Read(r, true, m, e) },
            { "SplineDeformStraighten", (r, m, e) => new SplineDeformStraighten().Read(r, true, m, e) },
            { "SplineDeformBend", (r, m, e) => new SplineDeformBend().Read(r, true, m, e) },
            { "SplineDeformTwist", (r, m, e) => new SplineDeformTwist().Read(r, true, m, e) },
            { "TrackRegion", (r, m, e) => new TrackRegion().Read(r, true, m, e) },
            { "PlotBeatAnims", (r, m, e) => new PlotBeatAnims().Read(r, true, m, e) },
            { "Ribbon", (r, m, e) => new RndRibbon().Read(r, true, m, e) },
            { "TriggerGroup", (r, m, e) => new RndTriggerGroup().Read(r, true, m, e) },
            { "CollideableCylinder", (r, m, e) => new CollideableCylinder().Read(r, true, m, e) },
            { "CollideableCapsule", (r, m, e) => new CollideableCapsule().Read(r, true, m, e) },
            { "CollideableSet", (r, m, e) => new CollideableSet().Read(r, true, m, e) },
            { "CollideableMidiOverride", (r, m, e) => new CollideableMidiOverride().Read(r, true, m, e) },
            { "CollideablePercussion", (r, m, e) => new CollideablePercussion().Read(r, true, m, e) },
            { "P9CrowdAudio", (r, m, e) => new P9CrowdAudio().Read(r, true, m, e) },
            { "P9TransDraw", (r, m, e) => new P9TransDraw().Read(r, true, m, e) },
            { "DataDrivenCollection", (r, m, e) => new DataDrivenCollection().Read(r, true, m, e) },
            { "P9WaypointConfiguration", (r, m, e) => new P9WaypointConfiguration().Read(r, true, m, e) },
            { "CharIKHead", (r, m, e) => new CharIKHead().Read(r, true, m, e) },
            { "CharSleeve", (r, m, e) => new CharSleeve().Read(r, true, m, e) },
            { "FxSendBitCrush", (r, m, e) => new FxSendBitCrush().Read(r, true, m, e) },
            { "ButtonEx", (r, m, e) => new Gh1ButtonEx().Read(r, true, m, e) },
            { "PictureEx", (r, m, e) => new Gh1PictureEx().Read(r, true, m, e) },
            { "TextEntry", (r, m, e) => { if (m.revision == 26) { return new PhaseTextEntry().Read(r, true, m, e); } return new Gh1TextEntry().Read(r, true, m, e); } },
            { "Slider", (r, m, e) => new Gh1Slider().Read(r, true, m, e) },
            { "PictureToggle", (r, m, e) => new Gh1PictureToggle().Read(r, true, m, e) },
            { "List", (r, m, e) => new Gh1List().Read(r, true, m, e) },
            { "BandButton", (r, m, e) => new BandButton().Read(r, true, m, e) },
            { "BandSlider", (r, m, e) => new BandSlider().Read(r, true, m, e) },
            { "BandTextEntry", (r, m, e) => new BandTextEntry().Read(r, true, m, e) },
            { "BandLeadMeter", (r, m, e) => new BandLeadMeter(0).Read(r, true, m, e) },
            { "BandScoreDisplay", (r, m, e) => new BandScoreDisplay(0).Read(r, true, m, e) },
            { "BandStarMeterDir", (r, m, e) => new BandStarMeterDir(0).Read(r, true, m, e) },
            { "BandStreakDisplay", (r, m, e) => new BandStreakDisplay(0).Read(r, true, m, e) },
            { "BandCamShot", (r, m, e) => new BandCamShot().Read(r, true, m, e) },
            { "BandCharDesc", (r, m, e) => new BandCharDesc().Read(r, true, m, e) },
            { "BandConfiguration", (r, m, e) => new BandConfiguration().Read(r, true, m, e) },
            { "BandDirector", (r, m, e) => new BandDirector().Read(r, true, m, e) },
            { "BandFaceDeform", (r, m, e) => new BandFaceDeform().Read(r, true, m, e) },
            { "BandLabel", (r, m, e) => new BandLabel().Read(r, true, m, e) },
            { "LabelEx", (r, m, e) => new BandLabel().Read(r, true, m, e) },
            { "BandList", (r, m, e) => new BandList().Read(r, true, m, e) },
            { "BandMeshLauncher", (r, m, e) => new BandMeshLauncher().Read(r, true, m, e) },
            { "BandPlacer", (r, m, e) => new BandPlacer().Read(r, true, m, e) },
            { "Placer", (r, m, e) => new BandPlacer().Read(r, true, m, e) },
            { "BandScreen", (r, m, e) => new Object().Read(r, true, m, e) },
            { "BandSongPref", (r, m, e) => new BandSongPref().Read(r, true, m, e) },
            { "BandSwatch", (r, m, e) => new BandSwatch().Read(r, true, m, e) },
            { "BandWardrobe", (r, m, e) => new BandWardrobe().Read(r, true, m, e) },
            { "BinkClip", (r, m, e) => new BinkClip().Read(r, true, m, e) },
            { "BustAMoveData", (r, m, e) => new BustAMoveData().Read(r, true, m, e) },
            { "Cam", (r, m, e) => new RndCam().Read(r, true, m, e) },
            { "CamShot", (r, m, e) => new CamShot().Read(r, true, m, e) },
            { "CharClip", (r, m, e) => new CharClip().Read(r, true, m, e) },
            { "CharClipFilter", (r, m, e) => new CharClipFilter().Read(r, true, m, e) },
            { "CharClipSamples", (r, m, e) => new CharClip().Read(r, true, m, e) },
            { "CharClipGroup", (r, m, e) => new CharClipGroup().Read(r, true, m, e) },
            { "CharCollide", (r, m, e) => new CharCollide().Read(r, true, m, e) },
            { "CharDriver", (r, m, e) => new CharDriver().Read(r, true, m, e) },
            { "CharDriverMidi", (r, m, e) => new CharDriverMidi().Read(r, true, m, e) },
            { "CharEyes", (r, m, e) => new CharEyes().Read(r, true, m, e) },
            { "CharForeTwist", (r, m, e) => new CharForeTwist().Read(r, true, m, e) },
            { "CharGuitarString", (r, m, e) => new CharGuitarString().Read(r, true, m, e) },
            { "CharHair", (r, m, e) => new CharHair().Read(r, true, m, e) },
            { "CharIKHand", (r, m, e) => new CharIKHand().Read(r, true, m, e) },
            { "CharIKFoot", (r, m, e) => new CharIKFoot().Read(r, true, m, e) },
            { "CharIKMidi", (r, m, e) => new CharIKMidi().Read(r, true, m, e) },
            { "CharIKRod", (r, m, e) => new CharIKRod().Read(r, true, m, e) },
            { "CharIKScale", (r, m, e) => new CharIKScale().Read(r, true, m, e) },
            { "CharInterest", (r, m, e) => new CharInterest().Read(r, true, m, e) },
            { "CharLipSync", (r, m, e) => new CharLipSync().Read(r, true, m, e) },
            { "CharLipSyncDriver", (r, m, e) => new CharLipSyncDriver().Read(r, true, m, e) },
            { "CharLookAt", (r, m, e) => new CharLookAt().Read(r, true, m, e) },
            { "CharMeshHide", (r, m, e) => new CharMeshHide().Read(r, true, m, e) },
            { "CharPosConstraint", (r, m, e) => new CharPosConstraint().Read(r, true, m, e) },
            { "CharServoBone", (r, m, e) => new CharServoBone().Read(r, true, m, e) },
            { "CharUpperTwist", (r, m, e) => new CharUpperTwist().Read(r, true, m, e) },
            { "CharWalk", (r, m, e) => new CharWalk().Read(r, true, m, e) },
            { "CharWeightSetter", (r, m, e) => new CharWeightSetter().Read(r, true, m, e) },
            { "CheckboxDisplay", (r, m, e) => new CheckboxDisplay().Read(r, true, m, e) },
            { "CheckBox", (r, m, e) => new CheckBox().Read(r, true, m, e) },
            { "ColorPalette", (r, m, e) => new ColorPalette().Read(r, true, m, e) },
            { "CreditList", (r, m, e) => new RndCreditList().Read(r, true, m, e) },
            { "DancerSequence", (r, m, e) => new DancerSequence().Read(r, true, m, e) },
            { "DeferDraw", (r, m, e) => new RndDeferDraw().Read(r, true, m, e) },
            { "Environ", (r, m, e) => new RndEnviron().Read(r, true, m, e) },
            { "EventTrigger", (r, m, e) => new EventTrigger().Read(r, true, m, e) },
            { "FileMerger", (r, m, e) => new FileMerger().Read(r, true, m, e) },
            { "Font", (r, m, e) => new RndFont().Read(r, true, m, e) },
            { "Fur", (r, m, e) => new RndFur().Read(r, true, m, e) },
            { "Group", (r, m, e) => new RndGroup().Read(r, true, m, e) },
            { "View", (r, m, e) => new RndGroup().Read(r, true, m, e) },
            { "HamBattleData", (r, m, e) => new HamBattleData().Read(r, true, m, e) },
            { "HamDirector", (r, m, e) => new HamDirector().Read(r, true, m, e) },
            { "HamLabel", (r, m, e) => new HamLabel().Read(r, true, m, e) },
            { "HamNavProvider", (r, m, e) => new HamNavProvider().Read(r, true, m, e) },
            { "HamNavList", (r, m, e) => new HamNavList().Read(r, true, m, e) },
            { "HamIKEffector", (r, m, e) => new HamIKEffector().Read(r, true, m, e) },
            { "HamRibbon", (r, m, e) => new HamRibbon().Read(r, true, m, e) },
            { "SongCollision", (r, m, e) => new SongCollision().Read(r, true, m, e) },
            { "HamCamShot", (r, m, e) => new HamCamShot().Read(r, true, m, e) },
            { "MetaMaterial", (r, m, e) => new MetaMaterial().Read(r, true, m, e) },
            { "Font3d", (r, m, e) => new RndFont3d().Read(r, true, m, e) },
            { "DepthBuffer3D", (r, m, e) => new DepthBuffer3D().Read(r, true, m, e) },
            { "SkeletonViz", (r, m, e) => new SkeletonViz().Read(r, true, m, e) },
            { "HamButton", (r, m, e) => new HamButton().Read(r, true, m, e) },
            { "HamButtonSet", (r, m, e) => new HamButtonSet().Read(r, true, m, e) },
            { "HamButtonList", (r, m, e) => new HamButtonList().Read(r, true, m, e) },
            { "GestureScroller", (r, m, e) => new GestureScroller().Read(r, true, m, e) },
            { "RankProgressBar", (r, m, e) => new RankProgressBar().Read(r, true, m, e) },
            { "HamSongButton", (r, m, e) => new HamButton().Read(r, true, m, e) },
            { "HamIconMan", (r, m, e) => new HamIconMan().Read(r, true, m, e) },
            { "PhotoSpotlightPositioner", (r, m, e) => new PhotoSpotlightPositioner().Read(r, true, m, e) },
            { "RhythmDetector", (r, m, e) => new RhythmDetector().Read(r, true, m, e) },
            { "RhythmDetectorGroup", (r, m, e) => new RhythmDetectorGroup().Read(r, true, m, e) },
            { "RhythmBattlePlayer", (r, m, e) => new RhythmBattlePlayer().Read(r, true, m, e) },
            { "RhythmBattle", (r, m, e) => new RhythmBattle().Read(r, true, m, e) },
            { "HamSkeletonConverter", (r, m, e) => new HamSkeletonConverter().Read(r, true, m, e) },
            { "SongDifficultyDisplay", (r, m, e) => new SongDifficultyDisplay().Read(r, true, m, e) },
            { "HamWardrobe", (r, m, e) => new HamWardrobe().Read(r, true, m, e) },
            { "HollaBackMinigame", (r, m, e) => new HollaBackMinigame().Read(r, true, m, e) },
            { "PracticeOptionsProvider", (r, m, e) => new PracticeOptionsProvider().Read(r, true, m, e) },
            { "PostProcer", (r, m, e) => new PostProcer().Read(r, true, m, e) },
            { "StreamRecorder", (r, m, e) => new StreamRecorder().Read(r, true, m, e) },
            { "MeterEffectMonitor", (r, m, e) => new MeterEffectMonitor().Read(r, true, m, e) },
            { "FxSendMeterEffect", (r, m, e) => new FxSendMeterEffect().Read(r, true, m, e) },
            { "AudioDuckerTrigger", (r, m, e) => new AudioDuckerTrigger().Read(r, true, m, e) },
            { "TexProc", (r, m, e) => new TexProc().Read(r, true, m, e) },
            { "Spline", (r, m, e) => new RndSpline().Read(r, true, m, e) },
            { "HamMeterEffectMonitor", (r, m, e) => new MeterEffectMonitor().Read(r, true, m, e) },
            { "StreamRenderer", (r, m, e) => new StreamRenderer().Read(r, true, m, e) },
            { "HamIKSkeleton", (r, m, e) => new HamIKSkeleton().Read(r, true, m, e) },
            { "HamRegulate", (r, m, e) => new HamRegulate().Read(r, true, m, e) },
            { "HamDriver", (r, m, e) => new HamDriver().Read(r, true, m, e) },
            { "StarsDisplay", (r, m, e) => new StarsDisplay().Read(r, true, m, e) },
            { "HamCamTransform", (r, m, e) => new HamCamTransform().Read(r, true, m, e) },
            { "TransConstraint", (r, m, e) => new TransConstraint().Read(r, true, m, e) },
            { "CharFeedback", (r, m, e) => new CharFeedback().Read(r, true, m, e) },
            { "HamList", (r, m, e) => new HamList().Read(r, true, m, e) },
            { "HamMove", (r, m, e) => new HamMove().Read(r, true, m, e) },
            { "HamPartyJumpData", (r, m, e) => new HamPartyJumpData().Read(r, true, m, e) },
            { "HamSupereasyData", (r, m, e) => new HamSupereasyData().Read(r, true, m, e) },
            { "InlineHelp", (r, m, e) => new InlineHelp().Read(r, true, m, e) },
            { "InterstitialPanel", (r, m, e) => new Object().Read(r, true, m, e) },
            { "Light", (r, m, e) => new RndLight().Read(r, true, m, e) },
            { "LightAnim", (r, m, e) => new RndLightAnim().Read(r, true, m, e) },
            { "LightPreset", (r, m, e) => new LightPreset().Read(r, true, m, e) },
            { "Line", (r, m, e) => new RndLine().Read(r, true, m, e) },
            { "Mat", (r, m, e) => new RndMat().Read(r, true, m, e) },
            { "MatAnim", (r, m, e) => new RndMatAnim().Read(r, true, m, e) },
            { "Mesh", (r, m, e) => new RndMesh().Read(r, true, m, e) },
            { "MeshDeform", (r, m, e) => new RndMeshDeform().Read(r, true, m, e) },
            { "MidiInstrument", (r, m, e) => new MidiInstrument().Read(r, true, m, e) },
            { "MidiParser", (r, m, e) => new MidiParser().Read(r, true, m, e) },
            { "MoggClip", (r, m, e) => new MoggClip().Read(r, true, m, e) },
            { "Movie", (r, m, e) => new RndMovie().Read(r, true, m, e) },
            { "MotionBlur", (r, m, e) => new RndMotionBlur().Read(r, true, m, e) },
            { "Morph", (r, m, e) => new RndMorph().Read(r, true, m, e) },
            { "MoveGraph", (r, m, e) => new MoveGraph().Read(r, true, m, e) },
            { "MsgSource", (r, m, e) => new Object().Read(r, true, m, e) },
            { "Object", (r, m, e) => new Object().Read(r, true, m, e) },
            { "OutfitLoader", (r, m, e) => new OutfitLoader().Read(r, true, m, e) },
            { "OutfitConfig",  (r, m, e) => new OutfitConfig().Read(r, true, m, e) },
            { "P9Director", (r, m, e) => new P9Director().Read(r, true, m, e) },
            { "P9SongPref", (r, m, e) => new P9SongPref().Read(r, true, m, e) },
            { "ParallelGroupSeq", (r, m, e) => new ParallelGroupSeq().Read(r, true, m, e) },
            { "ParticleSys", (r, m, e) => new RndParticleSys().Read(r, true, m, e) },
            { "ParticleSysAnim", (r, m, e) => new RndParticleSysAnim().Read(r, true, m, e) },
            { "PollAnim", (r, m, e) => new RndPollAnim().Read(r, true, m, e) },
            { "PostProc", (r, m, e) => new RndPostProc().Read(r, true, m, e) },
            { "PracticeSection", (r, m, e) => new PracticeSection().Read(r, true, m, e) },
            { "PropAnim", (r, m, e) => new RndPropAnim().Read(r, true, m, e) },
            { "RandomGroupSeq", (r, m, e) => new RandomGroupSeq().Read(r, true, m, e) },
            { "ScreenMask", (r, m, e) => new RndScreenMask().Read(r, true, m, e) },
            { "Screenshot", (r, m, e) => new Screenshot().Read(r, true, m, e) },
            { "Text", (r, m, e) => new RndText().Read(r, true, m, e) },
            { "Set", (r, m, e) => new RndSet().Read(r, true, m, e) },
            { "Sfx", (r, m, e) => new Sfx().Read(r, true, m, e) },
            { "Sound", (r, m, e) => new Sound().Read(r, true, m, e) },
            { "FlowNode", (r, m, e) => new FlowNode().Read(r, true, m, e) },
            { "FlowCommand", (r, m, e) => new FlowCommand().Read(r, true, m, e) },
            { "PropertyEventProvider", (r, m, e) => new PropertyEventProvider().Read(r, true, m, e) },
            { "FlowSlider", (r, m, e) => new FlowSlider().Read(r, true, m, e) },
            { "FlowDistance", (r, m, e) => new FlowDistance().Read(r, true, m, e) },
            { "FlowMultiSetProperty", (r, m, e) => new FlowMultiSetProperty().Read(r, true, m, e) },
            { "FlowEventListener", (r, m, e) => new FlowEventListener().Read(r, true, m, e) },
            { "FlowTrigger", (r, m, e) => new FlowTrigger().Read(r, true, m, e) },
            { "FlowSetProperty", (r, m, e) => new FlowSetProperty().Read(r, true, m, e) },
            { "FlowAnimate", (r, m, e) => new FlowAnimate().Read(r, true, m, e) },
            { "FlowSound", (r, m, e) => new FlowSound().Read(r, true, m, e) },
            { "FlowValueCase", (r, m, e) => new FlowValueCase().Read(r, true, m, e) },
            { "FlowRun", (r, m, e) => new FlowRun().Read(r, true, m, e) },
            { "FlowIf", (r, m, e) => new FlowIf().Read(r, true, m, e) },
            { "FlowSwitchCase", (r, m, e) => new FlowSwitchCase().Read(r, true, m, e) },
            { "FlowWhile", (r, m, e) => new FlowWhile().Read(r, true, m, e) },
            { "FlowOnStop", (r, m, e) => new FlowOnStop().Read(r, true, m, e) },
            { "FlowSequence", (r, m, e) => new FlowSequence().Read(r, true, m, e) },
            { "FlowTimer", (r, m, e) => new FlowTimer().Read(r, true, m, e) },
            { "FlowLabel", (r, m, e) => new FlowLabel().Read(r, true, m, e) },
            { "FlowOutPort", (r, m, e) => new FlowOutPort().Read(r, true, m, e) },
            { "FlowPickOne", (r, m, e) => new FlowPickOne().Read(r, true, m, e) },
            { "FlowSwitch", (r, m, e) => new FlowSwitch().Read(r, true, m, e) },
            { "FlowQueueable", (r, m, e) => new FlowQueueable().Read(r, true, m, e) },
            { "SfxSeq", (r, m, e) => new SfxSeq().Read(r, true, m, e) },
            { "Spotlight", (r, m, e) => new Spotlight().Read(r, true, m, e) },
            { "SpotlightDrawer", (r, m, e) => new SpotlightDrawer().Read(r, true, m, e) },
            { "SynthSample", (r, m, e) => new SynthSample().Read(r, true, m, e) },
            { "Tex", (r, m, e) => new RndTex().Read(r, true, m, e) },
            { "TexBlendController", (r, m, e) => new RndTexBlendController().Read(r, true, m, e) },
            { "TexBlender", (r, m, e) => new RndTexBlender().Read(r, true, m, e) },
            { "TexMovie", (r, m, e) => new RndTexMovie().Read(r, true, m, e) },
            { "TrackWidget", (r, m, e) => new TrackWidget().Read(r, true, m, e) },
            { "TrackWidgetRndDir", (r, m, e) => new TrackWidgetRndDir().Read(r, true, m, e) },
            { "TrackWidgetMultiMesh", (r, m, e) => new TrackWidgetMultiMesh().Read(r, true, m, e) },
            { "TrainerChallenge", (r, m, e) => new Object().Read(r, true, m, e) },
            { "Trans", (r, m, e) => new RndTrans().Read(r, true, m, e) },
            { "TransAnim", (r, m, e) => new RndTransAnim().Read(r, true, m, e) },
            { "TransProxy", (r, m, e) => new RndTransProxy().Read(r, true, m, e) },
            { "UIButton", (r, m, e) => new UIButton().Read(r, true, m, e) },
            { "UIColor", (r, m, e) => new UIColor().Read(r, true, m, e) },
            { "UIGuide", (r, m, e) => new UIGuide().Read(r, true, m, e) },
            { "UILabel", (r, m, e) => new UILabel().Read(r, true, m, e) },
            { "UIList", (r, m, e) => new UIList().Read(r, true, m, e) },
            { "UIListArrow", (r, m, e) => new UIListArrow().Read(r, true, m, e) },
            { "UIListCustom", (r, m, e) => new UIListCustom().Read(r, true, m, e) },
            { "UIListHighlight", (r, m, e) => new UIListHighlight().Read(r, true, m, e) },
            { "UIListLabel", (r, m, e) => new UIListLabel().Read(r, true, m, e) },
            { "UIListMesh", (r, m, e) => new UIListMesh().Read(r, true, m, e) },
            { "UIListSlot", (r, m, e) => new UIListSlot().Read(r, true, m, e) },
            { "UIListWidget", (r, m, e) => new UIListWidget().Read(r, true, m, e) },
            { "UIPanel", (r, m, e) => new Object().Read(r, true, m, e) },
            { "UIPicture", (r, m, e) => new UIPicture().Read(r, true, m, e) },
            { "UISlider", (r, m, e) => new UISlider().Read(r, true, m, e) },
            { "UITrigger", (r, m, e) => new UITrigger().Read(r, true, m, e) },
            { "Wind", (r, m, e) => new RndWind().Read(r, true, m, e) },
            { "WorldCrowd", (r, m, e) => new WorldCrowd().Read(r, true, m, e) },
            { "WorldReflection", (r, m, e) => new WorldReflection().Read(r, true, m, e) },
            { "AmbientOcclusion", (r, m, e) => new RndAmbientOcclusion().Read(r, true, m, e) },
            { "Blur", (r, m, e) => new RndBlur().Read(r, true, m, e) },
            { "BandHighlight", (r, m, e) => new BandHighlight().Read(r, true, m, e) },
            { "BandRetargetVignette", (r, m, e) => new BandRetargetVignette().Read(r, true, m, e) },
            { "CamAnim", (r, m, e) => new RndCamAnim().Read(r, true, m, e) },
            { "CharBoneOffset", (r, m, e) => new CharBoneOffset().Read(r, true, m, e) },
            { "CharBonesBlender", (r, m, e) => new CharBonesBlender().Read(r, true, m, e) },
            { "CharBoneTwist", (r, m, e) => new CharBoneTwist().Read(r, true, m, e) },
            { "CharNeckTwist", (r, m, e) => new CharNeckTwist().Read(r, true, m, e) },
            { "CharTransCopy", (r, m, e) => new CharTransCopy().Read(r, true, m, e) },
            { "CharTransDraw", (r, m, e) => new CharTransDraw().Read(r, true, m, e) },
            { "CrowdAudio", (r, m, e) => new CrowdAudio().Read(r, true, m, e) },
            { "DialogDisplay", (r, m, e) => new DialogDisplay().Read(r, true, m, e) },
            { "Distort", (r, m, e) => new RndDistort().Read(r, true, m, e) },
            { "Flare", (r, m, e) => new RndFlare().Read(r, true, m, e) },
            { "FxSendChorus", (r, m, e) => new FxSendChorus().Read(r, true, m, e) },
            { "FxSendCompress", (r, m, e) => new FxSendCompress().Read(r, true, m, e) },
            { "FxSendDelay", (r, m, e) => new FxSendDelay().Read(r, true, m, e) },
            { "FxSendDistortion", (r, m, e) => new FxSendDistortion().Read(r, true, m, e) },
            { "FxSendEQ", (r, m, e) => new FxSendEQ().Read(r, true, m, e) },
            { "FxSendFlanger", (r, m, e) => new FxSendFlanger().Read(r, true, m, e) },
            { "FxSendPitchShift", (r, m, e) => new FxSendPitchShift().Read(r, true, m, e) },
            { "FxSendReverb", (r, m, e) => new FxSendReverb().Read(r, true, m, e) },
            { "FxSendSynapse", (r, m, e) => new FxSendSynapse().Read(r, true, m, e) },
            { "FxSendWah", (r, m, e) => new FxSendWah().Read(r, true, m, e) },
            { "Label3d", (r, m, e) => new Label3d().Read(r, true, m, e) },
            { "LabelShrinkWrapper", (r, m, e) => new LabelShrinkWrapper().Read(r, true, m, e) },
            { "LightHue", (r, m, e) => new LightHue().Read(r, true, m, e) },
            { "MicInputArrow", (r, m, e) => new MicInputArrow().Read(r, true, m, e) },
            { "MiniLeaderboardDisplay", (r, m, e) => new MiniLeaderboardDisplay().Read(r, true, m, e) },
            { "MultiMesh", (r, m, e) => new RndMultiMesh().Read(r, true, m, e) },
            { "PlayerDiffIcon", (r, m, e) => new PlayerDiffIcon().Read(r, true, m, e) },
            { "ReviewDisplay", (r, m, e) => new ReviewDisplay().Read(r, true, m, e) },
            { "SerialGroupSeq", (r, m, e) => new SerialGroupSeq().Read(r, true, m, e) },
            { "SoftParticles", (r, m, e) => new RndSoftParticles().Read(r, true, m, e) },
            { "SongSectionController", (r, m, e) => new SongSectionController().Read(r, true, m, e) },
            { "SpotlightEnder", (r, m, e) => new SpotlightEnder().Read(r, true, m, e) },
            { "UIListSubList", (r, m, e) => new UIListSubList().Read(r, true, m, e) },
            { "UIProxy", (r, m, e) => new UIProxy().Read(r, true, m, e) },
            { "WaitSeq", (r, m, e) => new WaitSeq().Read(r, true, m, e) },
            { "Waypoint", (r, m, e) => new Waypoint().Read(r, true, m, e) },
            { "CharBlendBone", (r, m, e) => new CharBlendBone().Read(r, true, m, e) },
            { "MeshBlend", (r, m, e) => new RndMeshBlend().Read(r, true, m, e) },
            { "MeshBlendAnim", (r, m, e) => new RndMeshBlendAnim().Read(r, true, m, e) },
            { "CharTrigger", (r, m, e) => new CharTrigger().Read(r, true, m, e) },
            { "CharMarker", (r, m, e) => new CharMarker().Read(r, true, m, e) },
            { "BlendShapesDriver", (r, m, e) => new BlendShapesDriver().Read(r, true, m, e) },
            { "SynthEmitter", (r, m, e) => new SynthEmitter().Read(r, true, m, e) },
            { "CutsceneDriver", (r, m, e) => new CutsceneDriver().Read(r, true, m, e) },
            { "CharIKSliderMidi", (r, m, e) => new CharIKSliderMidi().Read(r, true, m, e) },
            { "CharKeyHandMidi", (r, m, e) => new CharKeyHandMidi().Read(r, true, m, e) },
            { "CharPollGroup", (r, m, e) => new CharPollGroup().Read(r, true, m, e) },
            { "EnvAnim", (r, m, e) => new RndEnvAnim().Read(r, true, m, e) },
            { "EventAnim", (r, m, e) => new EventAnim().Read(r, true, m, e) },
            { "InstrumentDifficultyDisplay", (r, m, e) => new InstrumentDifficultyDisplay().Read(r, true, m, e) },
            { "LabelNumberTicker", (r, m, e) => new LabelNumberTicker().Read(r, true, m, e) },
            { "MeshAnim", (r, m, e) => new RndMeshAnim().Read(r, true, m, e) },
            { "MeterDisplay", (r, m, e) => new MeterDisplay().Read(r, true, m, e) },
            { "RandomIntervalGroupSeq", (r, m, e) => new RandomIntervalGroupSeq().Read(r, true, m, e) },
            { "ScoreDisplay", (r, m, e) => new ScoreDisplay().Read(r, true, m, e) },
            { "ScrollbarDisplay", (r, m, e) => new ScrollbarDisplay().Read(r, true, m, e) },
            { "StarDisplay", (r, m, e) => new StarDisplay().Read(r, true, m, e) },
            { "CharEyeDartRuleset", (r, m, e) => new CharEyeDartRuleset().Read(r, true, m, e) },
            { "CharFaceServo", (r, m, e) => new CharFaceServo().Read(r, true, m, e) },
            { "FaceFxLipSyncServo", (r, m, e) => new FaceFxLipSyncServo().Read(r, true, m, e) },
            { "ChordShapeGenerator", (r, m, e) => new ChordShapeGenerator().Read(r, true, m, e) },
            { "CharIKFingers", (r, m, e) => new CharIKFingers().Read(r, true, m, e) },
            { "BandIKEffector", (r, m, e) => new BandIKEffector().Read(r, true, m, e) },
            { "CharCuff", (r, m, e) => new CharCuff().Read(r, true, m, e) },
            { "CharBone", (r, m, e) => new CharBone().Read(r, true, m, e) },
            { "PartLauncher", (r, m, e) => new RndPartLauncher().Read(r, true, m, e) },
            { "SynthFader", (r, m, e) => new SynthFader().Read(r, true, m, e) },
            { "SkeletonClip", (r, m, e) => new SkeletonClip().Read(r, true, m, e) },
            { "TexRenderer", (r, m, e) => new RndTexRenderer().Read(r, true, m, e) },
            { "PatchRenderer", (r, m, e) => new PatchRenderer().Read(r, true, m, e) },
            { "CubeTex", (r, m, e) => new RndCubeTex().Read(r, true, m, e) },
            { "GHTrackDir", (r, m, e) => new GHTrackDir(0).Read(r, true, m, e) },
        };

        foreach (var kvp in simpleObjectReaders)
        {
            actions[kvp.Key] = (reader, meta, entry) =>
            {
                Debug.WriteLine($"Reading entry {kvp.Key} {entry.name.value}");
                entry.obj = kvp.Value(reader, meta, entry);
            };
        }

        Action<string, Func<EndianReader, DirectoryMeta, Entry, Object>> AddSimpleDirEntry = (typeName, objReader) =>
        {
            actions[typeName] = (reader, meta, entry) =>
            {
                Debug.WriteLine($"Reading entry {typeName} {entry.name.value}");
                entry.isProxy = true;
                entry.obj = objReader(reader, meta, entry);

                bool shouldReadDir = entry.obj is not ObjectDir objectDir || objectDir.HasInlineProxyDirectory;

                if (shouldReadDir)
                {
                    DirectoryMeta dir = new();
                    dir.platform = meta.platform;
                    dir.Read(reader);
                    entry.dir = dir;
                }
            };
        };

        // Simple directory entries (all follow same pattern: read obj, set isProxy, read dir)
        AddSimpleDirEntry("BandCharacter", (r, m, e) => new BandCharacter(0).Read(r, true, m, e));
        AddSimpleDirEntry("BackgroundDir", (r, m, e) => new BackgroundDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("BandCrowdMeterDir", (r, m, e) => new BandCrowdMeterDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("CrowdMeterIcon", (r, m, e) => new BandCrowdMeterIcon(0).Read(r, true, m, e));
        AddSimpleDirEntry("BandScoreboard", (r, m, e) => new BandScoreboard(0).Read(r, true, m, e));
        AddSimpleDirEntry("BandPowerMeterDir", (r, m, e) => new BandPowerMeterDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("ScoreboardDir", (r, m, e) => new ScoreboardDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("BandStarDisplay", (r, m, e) => new BandStarDisplay(0).Read(r, true, m, e));
        AddSimpleDirEntry("CharBoneDir", (r, m, e) => new CharBoneDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("CompositeCharacter", (r, m, e) => new CompositeCharacter(0).Read(r, true, m, e));
        AddSimpleDirEntry("LoadingPanelDir", (r, m, e) => new LoadingPanelDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("MoveDir", (r, m, e) => new MoveDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("HamListRibbon", (r, m, e) => new HamListRibbon().Read(r, true, m, e));
        AddSimpleDirEntry("HamPhotoDisplay", (r, m, e) => new HamPhotoDisplay().Read(r, true, m, e));
        AddSimpleDirEntry("HamPhraseMeter", (r, m, e) => new HamPhraseMeter().Read(r, true, m, e));
        AddSimpleDirEntry("HamScrollSpeedIndicator", (r, m, e) => new HamScrollSpeedIndicator().Read(r, true, m, e));
        AddSimpleDirEntry("HamCharacter", (r, m, e) => new HamCharacter().Read(r, true, m, e));
        AddSimpleDirEntry("HamVisDir", (r, m, e) => new HamVisDir().Read(r, true, m, e));
        AddSimpleDirEntry("Flow", (r, m, e) => new FlowObject().Read(r, true, m, e));
        AddSimpleDirEntry("OverdriveMeterDir", (r, m, e) => new OverdriveMeterDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("OvershellDir", (r, m, e) => new OvershellDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("P9Character", (r, m, e) => new P9Character(0).Read(r, true, m, e));
        AddSimpleDirEntry("PitchArrowDir", (r, m, e) => new PitchArrowDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("PanelDir", (r, m, e) => new PanelDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("RenderDir", (r, m, e) => new RenderDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("SkeletonDir", (r, m, e) => new SkeletonDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("StreakMeterDir", (r, m, e) => new StreakMeterDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("SynthDir", (r, m, e) => new SynthDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("TrackDir", (r, m, e) => new TrackDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("TrackPanelDir", (r, m, e) => new TrackPanelDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("H2HTrackPanelDir", (r, m, e) => new H2HTrackPanelDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("EventCompleteDir", (r, m, e) => new EventCompleteDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("WorldMapDir", (r, m, e) => new WorldMapDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("UILabelDir", (r, m, e) => new UILabelDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("UIListDir", (r, m, e) => new UIListDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("DynamicLeaderboard", (r, m, e) => new DynamicLeaderboard().Read(r, true, m, e));
        AddSimpleDirEntry("SingleTrackDir", (r, m, e) => new SingleTrackDir().Read(r, true, m, e));
        AddSimpleDirEntry("WorldTrackDir", (r, m, e) => new WorldTrackDir().Read(r, true, m, e));
        AddSimpleDirEntry("PlotDir", (r, m, e) => new PlotDir().Read(r, true, m, e));
        AddSimpleDirEntry("HorizonDir", (r, m, e) => new HorizonDir().Read(r, true, m, e));
        AddSimpleDirEntry("GroundPlaneVis", (r, m, e) => new GroundPlaneVis().Read(r, true, m, e));
        AddSimpleDirEntry("UnisonIcon", (r, m, e) => new UnisonIcon(0).Read(r, true, m, e));
        AddSimpleDirEntry("VocalTrackDir", (r, m, e) => new VocalTrackDir(0).Read(r, true, m, e));
        AddSimpleDirEntry("WorldFx", (r, m, e) => new WorldFx(0).Read(r, true, m, e));
        AddSimpleDirEntry("WorldDir", (r, m, e) => new WorldDir(0).Read(r, true, m, e));

        AddSimpleDirEntry("GemTrackDir", (r, m, e) => new GemTrackDir(0).Read(r, true, m, e));

        actions["BandCrowdMeterDir"] = (reader, meta, entry) =>
        {
            Debug.WriteLine($"Reading entry BandCrowdMeterDir {entry.name.value}");
            entry.isProxy = true;
            entry.obj = new BandCrowdMeterDir(0).Read(reader, true, meta, entry);
            if (reader.BaseStream.Length - reader.BaseStream.Position >= 4)
            {
                long nestedDirPosition = reader.BaseStream.Position;
                uint nestedRevision = reader.ReadUInt32();
                reader.BaseStream.Position = nestedDirPosition;
                if (nestedRevision == meta.revision)
                {
                    DirectoryMeta dir = new();
                    dir.platform = meta.platform;
                    dir.Read(reader);
                    entry.dir = dir;
                }
            }
        };

        actions["BandCharacter"] = (reader, meta, entry) =>
        {
            Debug.WriteLine($"Reading entry BandCharacter {entry.name.value}");
            entry.isProxy = true;
            entry.obj = new BandCharacter(0).Read(reader, true, meta, entry);
        };

        actions["Character"] = (reader, meta, entry) =>
        {
            Debug.WriteLine($"Reading entry Character {entry.name.value}");
            entry.isProxy = true;
            entry.obj = new Character(0).Read(reader, true, meta, entry);

            Character character = (Character)entry.obj;
            if (character.HasInlineProxyDirectory)
            {
                DirectoryMeta dir = new();
                dir.platform = meta.platform;
                dir.Read(reader);
                entry.dir = dir;

                if (character.hasDeferredPostLoadFields)
                {
                    character.ReadDeferredPostLoad(reader, true, meta, entry);
                }
            }
        };

        actions["CompositeCharacter"] = (reader, meta, entry) =>
        {
            entry.isProxy = true;
            entry.obj = new CompositeCharacter(0).Read(reader, true, meta, entry);

            if (((Character)entry.obj).HasInlineProxyDirectory)
            {
                DirectoryMeta dir = new();
                dir.platform = meta.platform;
                dir.Read(reader);
                entry.dir = dir;
            }
        };

        actions["CharClipSet"] = (reader, meta, entry) =>
        {
            Debug.WriteLine($"Reading entry CharClipSet {entry.name.value}");
            entry.isProxy = true;
            entry.obj = new CharClipSet(0).Read(reader, true, meta, entry);

            if (((ObjectDir)entry.obj).HasInlineProxyDirectory)
            {
                DirectoryMeta dir = new();
                dir.platform = meta.platform;
                dir.Read(reader);
                entry.dir = dir;

                ObjectDir objectDir = (ObjectDir)entry.obj;
                if (objectDir.hasDeferredPostLoadFields)
                {
                    objectDir.ReadDeferredPostLoad(reader, true, meta, entry);
                }
            }
        };

        actions["RndDir"] = actions["EndingBonusDir"] = (reader, meta, entry) =>
        {
            Debug.WriteLine($"Reading entry RndDir {entry.name.value}");
            entry.isProxy = true;
            entry.obj = new RndDir(0).Read(reader, true, meta, entry);

            if (((ObjectDir)entry.obj).HasInlineProxyDirectory)
            {
                DirectoryMeta dir = new();
                dir.platform = meta.platform;
                dir.Read(reader);
                entry.dir = dir;

                ObjectDir objectDir = (ObjectDir)entry.obj;
                if (objectDir.hasDeferredPostLoadFields)
                {
                    objectDir.ReadDeferredPostLoad(reader, true, meta, entry);
                }
            }
        };

        actions["ObjectDir"] = (reader, meta, entry) =>
        {
            Debug.WriteLine($"Reading entry ObjectDir {entry.name.value}");
            entry.isProxy = true;
            entry.obj = new ObjectDir(0).Read(reader, true, meta, entry);

            if (((ObjectDir)entry.obj).HasInlineProxyDirectory)
            {
                DirectoryMeta dir = new();
                dir.platform = meta.platform;
                dir.Read(reader);
                entry.dir = dir;

                ObjectDir objectDir = (ObjectDir)entry.obj;
                if (objectDir.hasDeferredPostLoadFields)
                {
                    objectDir.ReadDeferredPostLoad(reader, true, meta, entry);
                }
            }
        };

        // WorldInstance - i dont even know
        actions["WorldInstance"] = (reader, meta, entry) =>
        {
            entry.isProxy = true;

            entry.obj = new WorldInstance(0).Read(reader, false, meta, entry);

            if (((WorldInstance)entry.obj).WorldInstanceRevision == 0)
            {
                if ((reader.Endianness == Endian.BigEndian ? 0xADDEADDE : 0xDEADDEAD) != reader.ReadUInt32())
                {
                    throw new Exception($"WorldInstance '{entry.name.value}' revision 0: end bytes not found at 0x{reader.BaseStream.Position:X}");
                }

                WorldInstance instance = (WorldInstance)entry.obj;
                if (instance.HasInlineProxyDirectory)
                {
                    DirectoryMeta dir = new();
                    dir.platform = meta.platform;
                    dir.Read(reader);
                    entry.dir = dir;
                    if (instance.hasDeferredPostLoadFields)
                    {
                        instance.ReadDeferredPostLoad(reader, true, meta, entry);
                    }
                }
                return;
            }

            // if the world instance has no persistent objects, it will have a dir as expected, otherwise it won't
            if (!((WorldInstance)entry.obj).hasPersistentObjects)
            {
                DirectoryMeta dir = new();
                dir.platform = meta.platform;
                dir.Read(reader);
                entry.dir = dir;

                if (entry.dir != null && entry.dir.type.value == "WorldInstance" && ((WorldInstance)dir.directory).hasPersistentObjects)
                {
                    ((WorldInstance)dir.directory).persistentObjects = new WorldInstance.PersistentObjects().Read(reader, meta, entry, ((WorldInstance)entry.obj).WorldInstanceRevision);
                }
                else
                {
                    ((WorldInstance)entry.obj).persistentObjects = new WorldInstance.PersistentObjects().Read(reader, meta, entry, ((WorldInstance)entry.obj).WorldInstanceRevision);
                }
            }
            else
            {
                ((WorldInstance)entry.obj).persistentObjects = new WorldInstance.PersistentObjects().Read(reader, meta, entry, ((WorldInstance)entry.obj).WorldInstanceRevision);
            }

            WorldInstance proxyInstance = (WorldInstance)entry.obj;
            if (proxyInstance.HasInlineProxyDirectory)
            {
                proxyInstance.inlineProxyDirectory = new DirectoryMeta { platform = meta.platform };
                proxyInstance.inlineProxyDirectory.Read(reader);
            }
        };

        return actions;
    }

    private static Dictionary<string, EntryWriteAction> InitializeEntryWriteActions()
    {
        var actions = new Dictionary<string, EntryWriteAction>();

        var simpleObjectWriters = new Dictionary<string, System.Action<EndianWriter, Object, DirectoryMeta, Entry>>
        {
            { "AnimFilter", (w, o, m, e) => ((RndAnimFilter)o).Write(w, true, m, e) },
            { "PlotDef", (w, o, m, e) => ((PlotDef)o).Write(w, true, m, e) },
            { "CharNeighborhood", (w, o, m, e) => ((CharNeighborhood)o).Write(w, true, m, e) },
            { "CrowdExtrasManager", (w, o, m, e) => ((CrowdExtrasManager)o).Write(w, true, m, e) },
            { "CrowdPreset", (w, o, m, e) => ((CrowdPreset)o).Write(w, true, m, e) },
            { "PlayerPortrait", (w, o, m, e) => ((PlayerPortrait)o).Write(w, true, m, e) },
            { "SyncWorldFx", (w, o, m, e) => ((SyncWorldFx)o).Write(w, true, m, e) },
            { "TrackGeom", (w, o, m, e) => ((TrackGeom)o).Write(w, true, m, e) },
            { "CamShotKeyframePreset", (w, o, m, e) => ((CamShotKeyframePreset)o).Write(w, true, m, e) },
            { "SplineDeform", (w, o, m, e) => ((SplineDeform)o).Write(w, true, m, e) },
            { "SplineDeformNode", (w, o, m, e) => ((SplineDeformNode)o).Write(w, true, m, e) },
            { "SplineDeformStraighten", (w, o, m, e) => ((SplineDeformStraighten)o).Write(w, true, m, e) },
            { "SplineDeformBend", (w, o, m, e) => ((SplineDeformBend)o).Write(w, true, m, e) },
            { "SplineDeformTwist", (w, o, m, e) => ((SplineDeformTwist)o).Write(w, true, m, e) },
            { "TrackRegion", (w, o, m, e) => ((TrackRegion)o).Write(w, true, m, e) },
            { "PlotBeatAnims", (w, o, m, e) => ((PlotBeatAnims)o).Write(w, true, m, e) },
            { "Ribbon", (w, o, m, e) => ((RndRibbon)o).Write(w, true, m, e) },
            { "TriggerGroup", (w, o, m, e) => ((RndTriggerGroup)o).Write(w, true, m, e) },
            { "CollideableCylinder", (w, o, m, e) => ((CollideableCylinder)o).Write(w, true, m, e) },
            { "CollideableCapsule", (w, o, m, e) => ((CollideableCapsule)o).Write(w, true, m, e) },
            { "CollideableSet", (w, o, m, e) => ((CollideableSet)o).Write(w, true, m, e) },
            { "CollideableMidiOverride", (w, o, m, e) => ((CollideableMidiOverride)o).Write(w, true, m, e) },
            { "CollideablePercussion", (w, o, m, e) => ((CollideablePercussion)o).Write(w, true, m, e) },
            { "P9CrowdAudio", (w, o, m, e) => ((P9CrowdAudio)o).Write(w, true, m, e) },
            { "P9TransDraw", (w, o, m, e) => ((P9TransDraw)o).Write(w, true, m, e) },
            { "DataDrivenCollection", (w, o, m, e) => ((DataDrivenCollection)o).Write(w, true, m, e) },
            { "P9WaypointConfiguration", (w, o, m, e) => ((P9WaypointConfiguration)o).Write(w, true, m, e) },
            { "CharIKHead", (w, o, m, e) => ((CharIKHead)o).Write(w, true, m, e) },
            { "CharSleeve", (w, o, m, e) => ((CharSleeve)o).Write(w, true, m, e) },
            { "FxSendBitCrush", (w, o, m, e) => ((FxSendBitCrush)o).Write(w, true, m, e) },
            { "ButtonEx", (w, o, m, e) => ((Gh1ButtonEx)o).Write(w, true, m, e) },
            { "PictureEx", (w, o, m, e) => ((Gh1PictureEx)o).Write(w, true, m, e) },
            { "TextEntry", (w, o, m, e) => o.Write(w, true, m, e) },
            { "Slider", (w, o, m, e) => ((Gh1Slider)o).Write(w, true, m, e) },
            { "PictureToggle", (w, o, m, e) => ((Gh1PictureToggle)o).Write(w, true, m, e) },
            { "List", (w, o, m, e) => ((Gh1List)o).Write(w, true, m, e) },
            { "BandButton", (w, o, m, e) => ((BandButton)o).Write(w, true, m, e) },
            { "BandSlider", (w, o, m, e) => ((BandSlider)o).Write(w, true, m, e) },
            { "BandTextEntry", (w, o, m, e) => ((BandTextEntry)o).Write(w, true, m, e) },
            { "BandLeadMeter", (w, o, m, e) => ((BandLeadMeter)o).Write(w, true, m, e) },
            { "BandScoreDisplay", (w, o, m, e) => ((BandScoreDisplay)o).Write(w, true, m, e) },
            { "BandStarMeterDir", (w, o, m, e) => ((BandStarMeterDir)o).Write(w, true, m, e) },
            { "BandStreakDisplay", (w, o, m, e) => ((BandStreakDisplay)o).Write(w, true, m, e) },
            { "BandCamShot", (w, o, m, e) => ((BandCamShot)o).Write(w, true, m, e) },
            { "BandCharDesc", (w, o, m, e) => ((BandCharDesc)o).Write(w, true, m, e) },
            { "BandConfiguration", (w, o, m, e) => ((BandConfiguration)o).Write(w, true, m, e) },
            { "BandDirector", (w, o, m, e) => ((BandDirector)o).Write(w, true, m, e) },
            { "BandFaceDeform", (w, o, m, e) => ((BandFaceDeform)o).Write(w, true, m, e) },
            { "BandLabel", (w, o, m, e) => ((BandLabel)o).Write(w, true, m, e) },
            { "LabelEx", (w, o, m, e) => ((BandLabel)o).Write(w, true, m, e) },
            { "BandList", (w, o, m, e) => ((BandList)o).Write(w, true, m, e) },
            { "BandMeshLauncher", (w, o, m, e) => ((BandMeshLauncher)o).Write(w, true, m, e) },
            { "BandPlacer", (w, o, m, e) => ((BandPlacer)o).Write(w, true, m, e) },
            { "Placer", (w, o, m, e) => ((BandPlacer)o).Write(w, true, m, e) },
            { "BandSongPref", (w, o, m, e) => ((BandSongPref)o).Write(w, true, m, e) },
            { "BandSwatch", (w, o, m, e) => ((BandSwatch)o).Write(w, true, m, e) },
            { "BandWardrobe", (w, o, m, e) => ((BandWardrobe)o).Write(w, true, m, e) },
            { "BinkClip", (w, o, m, e) => ((BinkClip)o).Write(w, true, m, e) },
            { "BustAMoveData", (w, o, m, e) => ((BustAMoveData)o).Write(w, true, m, e) },
            { "Cam", (w, o, m, e) => ((RndCam)o).Write(w, true, m, e) },
            { "CamShot", (w, o, m, e) => ((CamShot)o).Write(w, true, m, e) },
            { "CharClip", (w, o, m, e) => ((CharClip)o).Write(w, true, m, e) },
            { "CharClipSamples", (w, o, m, e) => ((CharClip)o).Write(w, true, m, e) },
            { "CharClipFilter", (w, o, m, e) => ((CharClipFilter)o).Write(w, true, m, e) },
            { "CharClipGroup", (w, o, m, e) => ((CharClipGroup)o).Write(w, true, m, e) },
            { "CharCollide", (w, o, m, e) => ((CharCollide)o).Write(w, true, m, e) },
            { "CharDriver", (w, o, m, e) => ((CharDriver)o).Write(w, true, m, e) },
            { "CharDriverMidi", (w, o, m, e) => ((CharDriverMidi)o).Write(w, true, m, e) },
            { "CharEyes", (w, o, m, e) => ((CharEyes)o).Write(w, true, m, e) },
            { "CharForeTwist", (w, o, m, e) => ((CharForeTwist)o).Write(w, true, m, e) },
            { "CharGuitarString", (w, o, m, e) => ((CharGuitarString)o).Write(w, true, m, e) },
            { "CharHair", (w, o, m, e) => ((CharHair)o).Write(w, true, m, e) },
            { "CharIKHand", (w, o, m, e) => ((CharIKHand)o).Write(w, true, m, e) },
            { "CharIKFoot", (w, o, m, e) => ((CharIKFoot)o).Write(w, true, m, e) },
            { "CharIKMidi", (w, o, m, e) => ((CharIKMidi)o).Write(w, true, m, e) },
            { "CharIKRod", (w, o, m, e) => ((CharIKRod)o).Write(w, true, m, e) },
            { "CharIKScale", (w, o, m, e) => ((CharIKScale)o).Write(w, true, m, e) },
            { "CharInterest", (w, o, m, e) => ((CharInterest)o).Write(w, true, m, e) },
            { "CharLipSync", (w, o, m, e) => ((CharLipSync)o).Write(w, true, m, e) },
            { "CharLipSyncDriver", (w, o, m, e) => ((CharLipSyncDriver)o).Write(w, true, m, e) },
            { "CharLookAt", (w, o, m, e) => ((CharLookAt)o).Write(w, true, m, e) },
            { "CharMeshHide", (w, o, m, e) => ((CharMeshHide)o).Write(w, true, m, e) },
            { "CharPosConstraint", (w, o, m, e) => ((CharPosConstraint)o).Write(w, true, m, e) },
            { "CharServoBone", (w, o, m, e) => ((CharServoBone)o).Write(w, true, m, e) },
            { "CharUpperTwist", (w, o, m, e) => ((CharUpperTwist)o).Write(w, true, m, e) },
            { "CharWalk", (w, o, m, e) => ((CharWalk)o).Write(w, true, m, e) },
            { "CharWeightSetter", (w, o, m, e) => ((CharWeightSetter)o).Write(w, true, m, e) },
            { "CheckboxDisplay", (w, o, m, e) => ((CheckboxDisplay)o).Write(w, true, m, e) },
            { "CheckBox", (w, o, m, e) => ((CheckBox)o).Write(w, true, m, e) },
            { "ColorPalette", (w, o, m, e) => ((ColorPalette)o).Write(w, true, m, e) },
            { "CreditList", (w, o, m, e) => ((RndCreditList)o).Write(w, true, m, e) },
            { "DancerSequence", (w, o, m, e) => ((DancerSequence)o).Write(w, true, m, e) },
            { "DeferDraw", (w, o, m, e) => ((RndDeferDraw)o).Write(w, true, m, e) },
            { "Environ", (w, o, m, e) => ((RndEnviron)o).Write(w, true, m, e) },
            { "EventTrigger", (w, o, m, e) => ((EventTrigger)o).Write(w, true, m, e) },
            { "FileMerger", (w, o, m, e) => ((FileMerger)o).Write(w, true, m, e) },
            { "Font", (w, o, m, e) => ((RndFont)o).Write(w, true, m, e) },
            { "Fur", (w, o, m, e) => ((RndFur)o).Write(w, true, m, e) },
            { "Group", (w, o, m, e) => ((RndGroup)o).Write(w, true, m, e) },
            { "HamBattleData", (w, o, m, e) => ((HamBattleData)o).Write(w, true, m, e) },
            { "HamDirector", (w, o, m, e) => ((HamDirector)o).Write(w, true, m, e) },
            { "HamLabel", (w, o, m, e) => ((HamLabel)o).Write(w, true, m, e) },
            { "HamNavProvider", (w, o, m, e) => ((HamNavProvider)o).Write(w, true, m, e) },
            { "HamNavList", (w, o, m, e) => ((HamNavList)o).Write(w, true, m, e) },
            { "HamIKEffector", (w, o, m, e) => ((HamIKEffector)o).Write(w, true, m, e) },
            { "HamRibbon", (w, o, m, e) => ((HamRibbon)o).Write(w, true, m, e) },
            { "SongCollision", (w, o, m, e) => ((SongCollision)o).Write(w, true, m, e) },
            { "HamCamShot", (w, o, m, e) => ((HamCamShot)o).Write(w, true, m, e) },
            { "MetaMaterial", (w, o, m, e) => ((MetaMaterial)o).Write(w, true, m, e) },
            { "Font3d", (w, o, m, e) => ((RndFont3d)o).Write(w, true, m, e) },
            { "DepthBuffer3D", (w, o, m, e) => ((DepthBuffer3D)o).Write(w, true, m, e) },
            { "SkeletonViz", (w, o, m, e) => ((SkeletonViz)o).Write(w, true, m, e) },
            { "HamButton", (w, o, m, e) => ((HamButton)o).Write(w, true, m, e) },
            { "HamButtonSet", (w, o, m, e) => ((HamButtonSet)o).Write(w, true, m, e) },
            { "HamButtonList", (w, o, m, e) => ((HamButtonList)o).Write(w, true, m, e) },
            { "GestureScroller", (w, o, m, e) => ((GestureScroller)o).Write(w, true, m, e) },
            { "RankProgressBar", (w, o, m, e) => ((RankProgressBar)o).Write(w, true, m, e) },
            { "HamSongButton", (w, o, m, e) => ((HamButton)o).Write(w, true, m, e) },
            { "HamIconMan", (w, o, m, e) => ((HamIconMan)o).Write(w, true, m, e) },
            { "PhotoSpotlightPositioner", (w, o, m, e) => ((PhotoSpotlightPositioner)o).Write(w, true, m, e) },
            { "RhythmDetector", (w, o, m, e) => ((RhythmDetector)o).Write(w, true, m, e) },
            { "RhythmDetectorGroup", (w, o, m, e) => ((RhythmDetectorGroup)o).Write(w, true, m, e) },
            { "RhythmBattlePlayer", (w, o, m, e) => ((RhythmBattlePlayer)o).Write(w, true, m, e) },
            { "RhythmBattle", (w, o, m, e) => ((RhythmBattle)o).Write(w, true, m, e) },
            { "HamSkeletonConverter", (w, o, m, e) => ((HamSkeletonConverter)o).Write(w, true, m, e) },
            { "SongDifficultyDisplay", (w, o, m, e) => ((SongDifficultyDisplay)o).Write(w, true, m, e) },
            { "HamWardrobe", (w, o, m, e) => ((HamWardrobe)o).Write(w, true, m, e) },
            { "HollaBackMinigame", (w, o, m, e) => ((HollaBackMinigame)o).Write(w, true, m, e) },
            { "PracticeOptionsProvider", (w, o, m, e) => ((PracticeOptionsProvider)o).Write(w, true, m, e) },
            { "PostProcer", (w, o, m, e) => ((PostProcer)o).Write(w, true, m, e) },
            { "StreamRecorder", (w, o, m, e) => ((StreamRecorder)o).Write(w, true, m, e) },
            { "MeterEffectMonitor", (w, o, m, e) => ((MeterEffectMonitor)o).Write(w, true, m, e) },
            { "FxSendMeterEffect", (w, o, m, e) => ((FxSendMeterEffect)o).Write(w, true, m, e) },
            { "AudioDuckerTrigger", (w, o, m, e) => ((AudioDuckerTrigger)o).Write(w, true, m, e) },
            { "TexProc", (w, o, m, e) => ((TexProc)o).Write(w, true, m, e) },
            { "Spline", (w, o, m, e) => ((RndSpline)o).Write(w, true, m, e) },
            { "HamMeterEffectMonitor", (w, o, m, e) => ((MeterEffectMonitor)o).Write(w, true, m, e) },
            { "StreamRenderer", (w, o, m, e) => ((StreamRenderer)o).Write(w, true, m, e) },
            { "HamIKSkeleton", (w, o, m, e) => ((HamIKSkeleton)o).Write(w, true, m, e) },
            { "HamRegulate", (w, o, m, e) => ((HamRegulate)o).Write(w, true, m, e) },
            { "HamDriver", (w, o, m, e) => ((HamDriver)o).Write(w, true, m, e) },
            { "StarsDisplay", (w, o, m, e) => ((StarsDisplay)o).Write(w, true, m, e) },
            { "HamCamTransform", (w, o, m, e) => ((HamCamTransform)o).Write(w, true, m, e) },
            { "TransConstraint", (w, o, m, e) => ((TransConstraint)o).Write(w, true, m, e) },
            { "CharFeedback", (w, o, m, e) => ((CharFeedback)o).Write(w, true, m, e) },
            { "HamList", (w, o, m, e) => ((HamList)o).Write(w, true, m, e) },
            { "HamMove", (w, o, m, e) => ((HamMove)o).Write(w, true, m, e) },
            { "HamPartyJumpData", (w, o, m, e) => ((HamPartyJumpData)o).Write(w, true, m, e) },
            { "HamSupereasyData", (w, o, m, e) => ((HamSupereasyData)o).Write(w, true, m, e) },
            { "InlineHelp", (w, o, m, e) => ((InlineHelp)o).Write(w, true, m, e) },
            { "Light", (w, o, m, e) => ((RndLight)o).Write(w, true, m, e) },
            { "LightAnim", (w, o, m, e) => ((RndLightAnim)o).Write(w, true, m, e) },
            { "LightPreset", (w, o, m, e) => ((LightPreset)o).Write(w, true, m, e) },
            { "Line", (w, o, m, e) => ((RndLine)o).Write(w, true, m, e) },
            { "Mat", (w, o, m, e) => ((RndMat)o).Write(w, true, m, e) },
            { "MatAnim", (w, o, m, e) => ((RndMatAnim)o).Write(w, true, m, e) },
            { "Mesh", (w, o, m, e) => ((RndMesh)o).Write(w, true, m, e) },
            { "MeshDeform", (w, o, m, e) => ((RndMeshDeform)o).Write(w, true, m, e) },
            { "MidiParser", (w, o, m, e) => ((MidiParser)o).Write(w, true, m, e) },
            { "MoggClip", (w, o, m, e) => ((MoggClip)o).Write(w, true, m, e) },
            { "Movie", (w, o, m, e) => ((RndMovie)o).Write(w, true, m, e) },
            { "MotionBlur", (w, o, m, e) => ((RndMotionBlur)o).Write(w, true, m, e) },
            { "Morph", (w, o, m, e) => ((RndMorph)o).Write(w, true, m, e) },
            { "MoveGraph", (w, o, m, e) => ((MoveGraph)o).Write(w, true, m, e) },
            { "OutfitLoader", (w, o, m, e) => ((OutfitLoader)o).Write(w, true, m, e) },
            { "OutfitConfig", (w, o, m, e) => ((OutfitConfig)o).Write(w, true, m, e) },
            { "ParallelGroupSeq", (w, o, m, e) => ((ParallelGroupSeq)o).Write(w, true, m, e) },
            { "ParticleSys", (w, o, m, e) => ((RndParticleSys)o).Write(w, true, m, e) },
            { "ParticleSysAnim", (w, o, m, e) => ((RndParticleSysAnim)o).Write(w, true, m, e) },
            { "PollAnim", (w, o, m, e) => ((RndPollAnim)o).Write(w, true, m, e) },
            { "PostProc", (w, o, m, e) => ((RndPostProc)o).Write(w, true, m, e) },
            { "PracticeSection", (w, o, m, e) => ((PracticeSection)o).Write(w, true, m, e) },
            { "PropAnim", (w, o, m, e) => ((RndPropAnim)o).Write(w, true, m, e) },
            { "RandomGroupSeq", (w, o, m, e) => ((RandomGroupSeq)o).Write(w, true, m, e) },
            { "ScreenMask", (w, o, m, e) => ((RndScreenMask)o).Write(w, true, m, e) },
            { "Screenshot", (w, o, m, e) => ((Screenshot)o).Write(w, true, m, e) },
            { "Text", (w, o, m, e) => ((RndText)o).Write(w, true, m, e) },
            { "Set", (w, o, m, e) => ((RndSet)o).Write(w, true, m, e) },
            { "Sfx", (w, o, m, e) => ((Sfx)o).Write(w, true, m, e) },
            { "Sound", (w, o, m, e) => ((Sound)o).Write(w, true, m, e) },
            { "FlowNode", (w, o, m, e) => ((FlowNode)o).Write(w, true, m, e) },
            { "FlowCommand", (w, o, m, e) => ((FlowCommand)o).Write(w, true, m, e) },
            { "PropertyEventProvider", (w, o, m, e) => ((PropertyEventProvider)o).Write(w, true, m, e) },
            { "FlowSlider", (w, o, m, e) => ((FlowSlider)o).Write(w, true, m, e) },
            { "FlowDistance", (w, o, m, e) => ((FlowDistance)o).Write(w, true, m, e) },
            { "FlowMultiSetProperty", (w, o, m, e) => ((FlowMultiSetProperty)o).Write(w, true, m, e) },
            { "FlowEventListener", (w, o, m, e) => ((FlowEventListener)o).Write(w, true, m, e) },
            { "FlowTrigger", (w, o, m, e) => ((FlowTrigger)o).Write(w, true, m, e) },
            { "FlowSetProperty", (w, o, m, e) => ((FlowSetProperty)o).Write(w, true, m, e) },
            { "FlowAnimate", (w, o, m, e) => ((FlowAnimate)o).Write(w, true, m, e) },
            { "FlowSound", (w, o, m, e) => ((FlowSound)o).Write(w, true, m, e) },
            { "FlowValueCase", (w, o, m, e) => ((FlowValueCase)o).Write(w, true, m, e) },
            { "FlowRun", (w, o, m, e) => ((FlowRun)o).Write(w, true, m, e) },
            { "FlowIf", (w, o, m, e) => ((FlowIf)o).Write(w, true, m, e) },
            { "FlowSwitchCase", (w, o, m, e) => ((FlowSwitchCase)o).Write(w, true, m, e) },
            { "FlowWhile", (w, o, m, e) => ((FlowWhile)o).Write(w, true, m, e) },
            { "FlowOnStop", (w, o, m, e) => ((FlowOnStop)o).Write(w, true, m, e) },
            { "FlowSequence", (w, o, m, e) => ((FlowSequence)o).Write(w, true, m, e) },
            { "FlowTimer", (w, o, m, e) => ((FlowTimer)o).Write(w, true, m, e) },
            { "FlowLabel", (w, o, m, e) => ((FlowLabel)o).Write(w, true, m, e) },
            { "FlowOutPort", (w, o, m, e) => ((FlowOutPort)o).Write(w, true, m, e) },
            { "FlowPickOne", (w, o, m, e) => ((FlowPickOne)o).Write(w, true, m, e) },
            { "FlowSwitch", (w, o, m, e) => ((FlowSwitch)o).Write(w, true, m, e) },
            { "FlowQueueable", (w, o, m, e) => ((FlowQueueable)o).Write(w, true, m, e) },
            { "SfxSeq", (w, o, m, e) => ((SfxSeq)o).Write(w, true, m, e) },
            { "Spotlight", (w, o, m, e) => ((Spotlight)o).Write(w, true, m, e) },
            { "SpotlightDrawer", (w, o, m, e) => ((SpotlightDrawer)o).Write(w, true, m, e) },
            { "SynthSample", (w, o, m, e) => ((SynthSample)o).Write(w, true, m, e) },
            { "Tex", (w, o, m, e) => ((RndTex)o).Write(w, true, m, e) },
            { "TexBlendController", (w, o, m, e) => ((RndTexBlendController)o).Write(w, true, m, e) },
            { "TexBlender", (w, o, m, e) => ((RndTexBlender)o).Write(w, true, m, e) },
            { "TexMovie", (w, o, m, e) => ((RndTexMovie)o).Write(w, true, m, e) },
            { "TrackWidget", (w, o, m, e) => ((TrackWidget)o).Write(w, true, m, e) },
            { "TrackWidgetRndDir", (w, o, m, e) => ((TrackWidgetRndDir)o).Write(w, true, m, e) },
            { "TrackWidgetMultiMesh", (w, o, m, e) => ((TrackWidgetMultiMesh)o).Write(w, true, m, e) },
            { "TransAnim", (w, o, m, e) => ((RndTransAnim)o).Write(w, true, m, e) },
            { "TransProxy", (w, o, m, e) => ((RndTransProxy)o).Write(w, true, m, e) },
            { "UIButton", (w, o, m, e) => ((UIButton)o).Write(w, true, m, e) },
            { "UIColor", (w, o, m, e) => ((UIColor)o).Write(w, true, m, e) },
            { "UIComponent", (w, o, m, e) => ((UIComponent)o).Write(w, true, m, e) },
            { "UIGuide", (w, o, m, e) => ((UIGuide)o).Write(w, true, m, e) },
            { "UILabel", (w, o, m, e) => ((UILabel)o).Write(w, true, m, e) },
            { "UIList", (w, o, m, e) => ((UIList)o).Write(w, true, m, e) },
            { "UIListArrow", (w, o, m, e) => ((UIListArrow)o).Write(w, true, m, e) },
            { "UIListCustom", (w, o, m, e) => ((UIListCustom)o).Write(w, true, m, e) },
            { "UIListHighlight", (w, o, m, e) => ((UIListHighlight)o).Write(w, true, m, e) },
            { "UIListLabel", (w, o, m, e) => ((UIListLabel)o).Write(w, true, m, e) },
            { "UIListMesh", (w, o, m, e) => ((UIListMesh)o).Write(w, true, m, e) },
            { "UIListSlot", (w, o, m, e) => ((UIListSlot)o).Write(w, true, m, e) },
            { "UIListWidget", (w, o, m, e) => ((UIListWidget)o).Write(w, true, m, e) },
            { "UIPicture", (w, o, m, e) => ((UIPicture)o).Write(w, true, m, e) },
            { "UISlider", (w, o, m, e) => ((UISlider)o).Write(w, true, m, e) },
            { "UITrigger", (w, o, m, e) => ((UITrigger)o).Write(w, true, m, e) },
            { "Wind", (w, o, m, e) => ((RndWind)o).Write(w, true, m, e) },
            { "WorldReflection", (w, o, m, e) => ((WorldReflection)o).Write(w, true, m, e) },
            { "AmbientOcclusion", (w, o, m, e) => ((RndAmbientOcclusion)o).Write(w, true, m, e) },
            { "Blur", (w, o, m, e) => ((RndBlur)o).Write(w, true, m, e) },
            { "BandHighlight", (w, o, m, e) => ((BandHighlight)o).Write(w, true, m, e) },
            { "BandRetargetVignette", (w, o, m, e) => ((BandRetargetVignette)o).Write(w, true, m, e) },
            { "CamAnim", (w, o, m, e) => ((RndCamAnim)o).Write(w, true, m, e) },
            { "CharBoneOffset", (w, o, m, e) => ((CharBoneOffset)o).Write(w, true, m, e) },
            { "CharBonesBlender", (w, o, m, e) => ((CharBonesBlender)o).Write(w, true, m, e) },
            { "CharBoneTwist", (w, o, m, e) => ((CharBoneTwist)o).Write(w, true, m, e) },
            { "CharNeckTwist", (w, o, m, e) => ((CharNeckTwist)o).Write(w, true, m, e) },
            { "CharTransCopy", (w, o, m, e) => ((CharTransCopy)o).Write(w, true, m, e) },
            { "CharTransDraw", (w, o, m, e) => ((CharTransDraw)o).Write(w, true, m, e) },
            { "CrowdAudio", (w, o, m, e) => ((CrowdAudio)o).Write(w, true, m, e) },
            { "DialogDisplay", (w, o, m, e) => ((DialogDisplay)o).Write(w, true, m, e) },
            { "Distort", (w, o, m, e) => ((RndDistort)o).Write(w, true, m, e) },
            { "Flare", (w, o, m, e) => ((RndFlare)o).Write(w, true, m, e) },
            { "FxSendChorus", (w, o, m, e) => ((FxSendChorus)o).Write(w, true, m, e) },
            { "FxSendCompress", (w, o, m, e) => ((FxSendCompress)o).Write(w, true, m, e) },
            { "FxSendDelay", (w, o, m, e) => ((FxSendDelay)o).Write(w, true, m, e) },
            { "FxSendDistortion", (w, o, m, e) => ((FxSendDistortion)o).Write(w, true, m, e) },
            { "FxSendEQ", (w, o, m, e) => ((FxSendEQ)o).Write(w, true, m, e) },
            { "FxSendFlanger", (w, o, m, e) => ((FxSendFlanger)o).Write(w, true, m, e) },
            { "FxSendPitchShift", (w, o, m, e) => ((FxSendPitchShift)o).Write(w, true, m, e) },
            { "FxSendReverb", (w, o, m, e) => ((FxSendReverb)o).Write(w, true, m, e) },
            { "FxSendSynapse", (w, o, m, e) => ((FxSendSynapse)o).Write(w, true, m, e) },
            { "FxSendWah", (w, o, m, e) => ((FxSendWah)o).Write(w, true, m, e) },
            { "Label3d", (w, o, m, e) => ((Label3d)o).Write(w, true, m, e) },
            { "LabelShrinkWrapper", (w, o, m, e) => ((LabelShrinkWrapper)o).Write(w, true, m, e) },
            { "LightHue", (w, o, m, e) => ((LightHue)o).Write(w, true, m, e) },
            { "MicInputArrow", (w, o, m, e) => ((MicInputArrow)o).Write(w, true, m, e) },
            { "MiniLeaderboardDisplay", (w, o, m, e) => ((MiniLeaderboardDisplay)o).Write(w, true, m, e) },
            { "MultiMesh", (w, o, m, e) => ((RndMultiMesh)o).Write(w, true, m, e) },
            { "PlayerDiffIcon", (w, o, m, e) => ((PlayerDiffIcon)o).Write(w, true, m, e) },
            { "ReviewDisplay", (w, o, m, e) => ((ReviewDisplay)o).Write(w, true, m, e) },
            { "SerialGroupSeq", (w, o, m, e) => ((SerialGroupSeq)o).Write(w, true, m, e) },
            { "SoftParticles", (w, o, m, e) => ((RndSoftParticles)o).Write(w, true, m, e) },
            { "SongSectionController", (w, o, m, e) => ((SongSectionController)o).Write(w, true, m, e) },
            { "SpotlightEnder", (w, o, m, e) => ((SpotlightEnder)o).Write(w, true, m, e) },
            { "UIListSubList", (w, o, m, e) => ((UIListSubList)o).Write(w, true, m, e) },
            { "UIProxy", (w, o, m, e) => ((UIProxy)o).Write(w, true, m, e) },
            { "WaitSeq", (w, o, m, e) => ((WaitSeq)o).Write(w, true, m, e) },
            { "Waypoint", (w, o, m, e) => ((Waypoint)o).Write(w, true, m, e) },
            { "CharBlendBone", (w, o, m, e) => ((CharBlendBone)o).Write(w, true, m, e) },
            { "MeshBlend", (w, o, m, e) => ((RndMeshBlend)o).Write(w, true, m, e) },
            { "MeshBlendAnim", (w, o, m, e) => ((RndMeshBlendAnim)o).Write(w, true, m, e) },
            { "CharTrigger", (w, o, m, e) => ((CharTrigger)o).Write(w, true, m, e) },
            { "CharMarker", (w, o, m, e) => ((CharMarker)o).Write(w, true, m, e) },
            { "BlendShapesDriver", (w, o, m, e) => ((BlendShapesDriver)o).Write(w, true, m, e) },
            { "SynthEmitter", (w, o, m, e) => ((SynthEmitter)o).Write(w, true, m, e) },
            { "CutsceneDriver", (w, o, m, e) => ((CutsceneDriver)o).Write(w, true, m, e) },
            { "CharIKSliderMidi", (w, o, m, e) => ((CharIKSliderMidi)o).Write(w, true, m, e) },
            { "CharKeyHandMidi", (w, o, m, e) => ((CharKeyHandMidi)o).Write(w, true, m, e) },
            { "CharPollGroup", (w, o, m, e) => ((CharPollGroup)o).Write(w, true, m, e) },
            { "EnvAnim", (w, o, m, e) => ((RndEnvAnim)o).Write(w, true, m, e) },
            { "EventAnim", (w, o, m, e) => ((EventAnim)o).Write(w, true, m, e) },
            { "InstrumentDifficultyDisplay", (w, o, m, e) => ((InstrumentDifficultyDisplay)o).Write(w, true, m, e) },
            { "LabelNumberTicker", (w, o, m, e) => ((LabelNumberTicker)o).Write(w, true, m, e) },
            { "MeshAnim", (w, o, m, e) => ((RndMeshAnim)o).Write(w, true, m, e) },
            { "MeterDisplay", (w, o, m, e) => ((MeterDisplay)o).Write(w, true, m, e) },
            { "RandomIntervalGroupSeq", (w, o, m, e) => ((RandomIntervalGroupSeq)o).Write(w, true, m, e) },
            { "ScoreDisplay", (w, o, m, e) => ((ScoreDisplay)o).Write(w, true, m, e) },
            { "ScrollbarDisplay", (w, o, m, e) => ((ScrollbarDisplay)o).Write(w, true, m, e) },
            { "StarDisplay", (w, o, m, e) => ((StarDisplay)o).Write(w, true, m, e) },
            { "CharEyeDartRuleset", (w, o, m, e) => ((CharEyeDartRuleset)o).Write(w, true, m, e) },
            { "CharFaceServo", (w, o, m, e) => ((CharFaceServo)o).Write(w, true, m, e) },
            { "FaceFxLipSyncServo", (w, o, m, e) => ((FaceFxLipSyncServo)o).Write(w, true, m, e) },
            { "ChordShapeGenerator", (w, o, m, e) => ((ChordShapeGenerator)o).Write(w, true, m, e) },
            { "CharIKFingers", (w, o, m, e) => ((CharIKFingers)o).Write(w, true, m, e) },
            { "BandIKEffector", (w, o, m, e) => ((BandIKEffector)o).Write(w, true, m, e) },
            { "CharCuff", (w, o, m, e) => ((CharCuff)o).Write(w, true, m, e) },
            { "CharBone", (w, o, m, e) => ((CharBone)o).Write(w, true, m, e) },
            { "PartLauncher", (w, o, m, e) => ((RndPartLauncher)o).Write(w, true, m, e) },
            { "SynthFader", (w, o, m, e) => ((SynthFader)o).Write(w, true, m, e) },
            { "SkeletonClip", (w, o, m, e) => ((SkeletonClip)o).Write(w, true, m, e) },
            { "TexRenderer", (w, o, m, e) => ((RndTexRenderer)o).Write(w, true, m, e) },
            { "WorldCrowd", (w, o, m, e) => ((WorldCrowd)o).Write(w, true, m, e) },
            { "PatchRenderer", (w, o, m, e) => ((PatchRenderer)o).Write(w, true, m, e) },
            { "CubeTex", (w, o, m, e) => ((RndCubeTex)o).Write(w, true, m, e) },
            { "GHTrackDir", (w, o, m, e) => ((GHTrackDir)o).Write(w, true, m, e) },
            { "P9Director", (w, o, m, e) => ((P9Director)o).Write(w, true, m, e) },
            { "P9SongPref", (w, o, m, e) => ((P9SongPref)o).Write(w, true, m, e) },
        };

        foreach (var kvp in simpleObjectWriters)
        {
            actions[kvp.Key] = (writer, meta, entry) =>
            {
                kvp.Value(writer, entry.obj, meta, entry);
            };
        }

        actions["Trans"] = (writer, meta, entry) =>
        {
            ((RndTrans)entry.obj).Write(writer, true, meta, entry);
        };


        Action<string, Action<EndianWriter, Object, DirectoryMeta, Entry>> AddSimpleDirEntryWriter = (typeName, objWriter) =>
        {
            actions[typeName] = (writer, meta, entry) =>
            {
                objWriter(writer, entry.obj, meta, entry);
                if (entry.dir != null)
                {
                    WriteProxyDir(writer, entry.dir);
                }
            };
        };

        // Simple directory entry writers
        AddSimpleDirEntryWriter("BandScoreboard", (w, o, m, e) => ((BandScoreboard)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("BandPowerMeterDir", (w, o, m, e) => ((BandPowerMeterDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("ScoreboardDir", (w, o, m, e) => ((ScoreboardDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("BackgroundDir", (w, o, m, e) => ((BackgroundDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("BandCrowdMeterDir", (w, o, m, e) => ((BandCrowdMeterDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("BandStarDisplay", (w, o, m, e) => ((BandStarDisplay)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("CharBoneDir", (w, o, m, e) => ((CharBoneDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("CompositeCharacter", (w, o, m, e) => ((CompositeCharacter)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("CrowdMeterIcon", (w, o, m, e) => ((BandCrowdMeterIcon)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("EndingBonusDir", (w, o, m, e) => ((RndDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("GemTrackDir", (w, o, m, e) => ((GemTrackDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("LoadingPanelDir", (w, o, m, e) => ((LoadingPanelDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("HamListRibbon", (w, o, m, e) => ((HamListRibbon)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("HamPhotoDisplay", (w, o, m, e) => ((HamPhotoDisplay)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("HamPhraseMeter", (w, o, m, e) => ((HamPhraseMeter)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("HamScrollSpeedIndicator", (w, o, m, e) => ((HamScrollSpeedIndicator)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("HamCharacter", (w, o, m, e) => ((HamCharacter)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("HamVisDir", (w, o, m, e) => ((HamVisDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("OverdriveMeterDir", (w, o, m, e) => ((OverdriveMeterDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("OvershellDir", (w, o, m, e) => ((OvershellDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("P9Character", (w, o, m, e) => ((P9Character)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("PanelDir", (w, o, m, e) => ((PanelDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("PitchArrowDir", (w, o, m, e) => ((PitchArrowDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("RenderDir", (w, o, m, e) => ((RenderDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("SkeletonDir", (w, o, m, e) => ((SkeletonDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("Flow", (w, o, m, e) => ((FlowObject)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("StreakMeterDir", (w, o, m, e) => ((StreakMeterDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("SynthDir", (w, o, m, e) => ((SynthDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("TrackDir", (w, o, m, e) => ((TrackDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("TrackPanelDir", (w, o, m, e) => ((TrackPanelDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("H2HTrackPanelDir", (w, o, m, e) => ((H2HTrackPanelDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("EventCompleteDir", (w, o, m, e) => ((EventCompleteDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("WorldMapDir", (w, o, m, e) => ((WorldMapDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("UILabelDir", (w, o, m, e) => ((UILabelDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("UIListDir", (w, o, m, e) => ((UIListDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("DynamicLeaderboard", (w, o, m, e) => ((DynamicLeaderboard)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("SingleTrackDir", (w, o, m, e) => ((SingleTrackDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("WorldTrackDir", (w, o, m, e) => ((WorldTrackDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("PlotDir", (w, o, m, e) => ((PlotDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("HorizonDir", (w, o, m, e) => ((HorizonDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("GroundPlaneVis", (w, o, m, e) => ((GroundPlaneVis)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("UnisonIcon", (w, o, m, e) => ((UnisonIcon)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("VocalTrackDir", (w, o, m, e) => ((VocalTrackDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("WorldFx", (w, o, m, e) => ((WorldFx)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("WorldDir", (w, o, m, e) => ((WorldDir)o).Write(w, true, m, e));
        AddSimpleDirEntryWriter("BandCharacter", (w, o, m, e) => ((BandCharacter)o).Write(w, true, m, e));

        actions["BandCharacter"] = (writer, meta, entry) =>
        {
            ((BandCharacter)entry.obj).Write(writer, true, meta, entry);
        };

        actions["Character"] = (writer, meta, entry) =>
        {
            Character character = (Character)entry.obj;
            character.Write(writer, true, meta, entry);
            if (character.HasInlineProxyDirectory && entry.dir != null)
            {
                WriteInlineDir(writer, entry.dir, character.hasDeferredPostLoadFields);

                if (character.hasDeferredPostLoadFields)
                {
                    character.WriteDeferredPostLoad(writer, true, meta, entry);
                }
            }
        };

        actions["CompositeCharacter"] = (writer, meta, entry) =>
        {
            ((CompositeCharacter)entry.obj).Write(writer, true, meta, entry);
            if (((Character)entry.obj).HasInlineProxyDirectory)
            {
                WriteProxyDir(writer, entry.dir);
            }
        };

        actions["CharClipSet"] = (writer, meta, entry) =>
        {
            ((CharClipSet)entry.obj).Write(writer, true, meta, entry);
            if (entry.dir != null)
            {
                WriteProxyDir(writer, entry.dir);
            }
        };

        actions["MoveDir"] = (writer, meta, entry) =>
        {
            ((MoveDir)entry.obj).Write(writer, true, meta, entry);
            if (entry.dir != null)
            {
                WriteProxyDir(writer, entry.dir);
            }
        };

        actions["ObjectDir"] = (writer, meta, entry) =>
        {
            ObjectDir objectDir = (ObjectDir)entry.obj;
            objectDir.Write(writer, true, meta, entry);
            if (entry.dir != null)
            {
                WriteInlineDir(writer, entry.dir, objectDir.hasDeferredPostLoadFields);

                if (objectDir.hasDeferredPostLoadFields)
                {
                    objectDir.WriteDeferredPostLoad(writer, true, meta, entry);
                }
            }
        };

        actions["RndDir"] = (writer, meta, entry) =>
        {
            RndDir rndDir = (RndDir)entry.obj;
            rndDir.Write(writer, true, meta, entry);
            if (entry.dir != null)
            {
                WriteInlineDir(writer, entry.dir, rndDir.hasDeferredPostLoadFields);

                if (rndDir.hasDeferredPostLoadFields)
                {
                    rndDir.WriteDeferredPostLoad(writer, true, meta, entry);
                }
            }
        };

        // WorldInstance - fuck this shit
        actions["WorldInstance"] = (writer, meta, entry) =>
        {
            ((WorldInstance)entry.obj).Write(writer, false, meta, entry);

            if (((WorldInstance)entry.obj).WorldInstanceRevision == 0)
            {
                writer.WriteEndBytes();
                WorldInstance instance = (WorldInstance)entry.obj;
                if (instance.HasInlineProxyDirectory)
                {
                    if (entry.dir == null)
                    {
                        throw new InvalidDataException("WorldInstance inline proxy directory is missing.");
                    }
                    entry.dir.Write(writer);
                    if (instance.hasDeferredPostLoadFields)
                    {
                        instance.WriteDeferredPostLoad(writer, true, meta, entry);
                    }
                }
                return;
            }

            if (!((WorldInstance)entry.obj).hasPersistentObjects)
            {
                entry.dir.Write(writer);

                if (entry.dir.type.value == "WorldInstance" && ((WorldInstance)entry.dir.directory).hasPersistentObjects)
                {
                    ((WorldInstance)entry.dir.directory).persistentObjects.Write(writer, meta, entry, ((WorldInstance)entry.dir.directory).WorldInstanceRevision);
                }
                else
                {
                    ((WorldInstance)entry.obj).persistentObjects.Write(writer, meta, entry, ((WorldInstance)entry.obj).WorldInstanceRevision);
                }
            }
            else
            {
                ((WorldInstance)entry.obj).persistentObjects.Write(writer, meta, entry, ((WorldInstance)entry.obj).WorldInstanceRevision);
            }

            WorldInstance proxyInstance = (WorldInstance)entry.obj;
            if (proxyInstance.HasInlineProxyDirectory)
            {
                if (proxyInstance.inlineProxyDirectory == null)
                {
                    throw new InvalidDataException("WorldInstance inline proxy directory is missing.");
                }
                proxyInstance.inlineProxyDirectory.Write(writer);
            }
        };

        return actions;
    }

    private static void WriteProxyDir(EndianWriter writer, DirectoryMeta dir)
    {
        writer.MarkChunk();
        dir.Write(writer);
    }

    private static void WriteInlineDir(EndianWriter writer, DirectoryMeta dir, bool objectContinuesAfter)
    {
        if (objectContinuesAfter)
        {
            dir.Write(writer);
            return;
        }

        WriteProxyDir(writer, dir);
    }

    public class Entry
    {
        public class EntryOperationEventArgs : EventArgs
        {
            public EndianReader? Reader { get; }
            public EndianWriter? Writer { get; }
            public Entry Entry { get; }


            public EntryOperationEventArgs(EndianReader reader, Entry entry)
            {
                Reader = reader;
                Entry = entry;
            }
            public EntryOperationEventArgs(EndianWriter writer, Entry entry)
            {
                Writer = writer;
                Entry = entry;
            }

        }

        /// <summary>
        /// The type of the object, i.e. "Object", "RndDir", etc.
        /// </summary>
        public Symbol type = Symbol.Empty;

        /// <summary>
        /// The name of the object, i.e. "uniq0", "NewRndDir", "palette.pal", etc.
        /// </summary>
        public Symbol name = Symbol.Empty;

        /// <summary>
        /// The object itself, can be a directory or an asset
        /// </summary>
        public Object obj;

        /// <summary>
        /// The directory of the object, if it's a directory (confusing, indeed)
        /// </summary>
        public DirectoryMeta? dir;

        /// <summary>
        /// Whether or not the entry is an entry inside the root directory (aka proxied). Used to handle writing directories that are entries inside of another directory and not inlined subdirectories.
        /// Non-directories do not have the concept of being proxied so it will always be false for objects that are not directories.
        /// </summary>
        public bool isProxy;

        /// <summary>
        /// Set when the object has been added or otherwise created through a non-serialized fashion (i.e. as raw bytes)
        /// </summary>
        public bool dirty = false;

        /// <summary>
        /// Whether or not the type is recognized by MiloLib.
        /// Used to handle duplicating and other actions on assets that aren't yet supported.
        /// </summary>
        public bool typeRecognized = true;

        /// <summary>
        /// The raw bytes of the object. If we can't deserialize/serialize a particular type yet, we just read and write this directly.
        /// Also used to enable extracting assets.
        /// </summary>
        private byte[] _objBytes = Array.Empty<byte>();
        private ReadOnlyMemory<byte> _deferredObjBytes;
        private bool _hasDeferredObjBytes;

        public byte[] objBytes
        {
            get
            {
                if (_hasDeferredObjBytes)
                {
                    _objBytes = _deferredObjBytes.ToArray();
                    _deferredObjBytes = _objBytes;
                    _hasDeferredObjBytes = false;
                }

                return _objBytes;
            }
            set
            {
                _objBytes = value ?? Array.Empty<byte>();
                _deferredObjBytes = _objBytes;
                _hasDeferredObjBytes = false;
            }
        }

        internal ReadOnlySpan<byte> ObjBytesSpan => _hasDeferredObjBytes ? _deferredObjBytes.Span : _objBytes;
        public ReadOnlyMemory<byte> ObjBytesMemory => _hasDeferredObjBytes ? _deferredObjBytes : _objBytes;

        public long objBytesAbsolutePosition = -1;

        internal bool hasSerializedData = true;

        public Entry(Symbol type, Symbol name, Object obj)
        {
            this.type = type;
            this.name = name;
            this.obj = obj;
        }

        public static Entry CreateDirtyAssetFromBytes(string type, string name, byte[] bytes)
        {
            Entry entry = new(Symbol.Empty, Symbol.Empty, null);
            entry.type = type;
            entry.name = name;
            entry.objBytes = bytes;
            entry.dirty = true;
            return entry;
        }

        internal void SetDeferredObjBytes(ReadOnlyMemory<byte> bytes)
        {
            _deferredObjBytes = bytes;
            _objBytes = Array.Empty<byte>();
            _hasDeferredObjBytes = !bytes.IsEmpty;
        }

        public event EventHandler<EntryOperationEventArgs> BeforeRead;
        public event EventHandler<EntryOperationEventArgs> AfterRead;
        public event EventHandler<EntryOperationEventArgs> BeforeWrite;
        public event EventHandler<EntryOperationEventArgs> AfterWrite;


        internal void OnBeforeRead(EndianReader reader)
        {
            BeforeRead?.Invoke(this, new EntryOperationEventArgs(reader, this));
        }

        internal void OnAfterRead(EndianReader reader)
        {
            AfterRead?.Invoke(this, new EntryOperationEventArgs(reader, this));
        }

        internal void OnBeforeWrite(EndianWriter writer)
        {
            BeforeWrite?.Invoke(this, new EntryOperationEventArgs(writer, this));
        }

        internal void OnAfterWrite(EndianWriter writer)
        {
            AfterWrite?.Invoke(this, new EntryOperationEventArgs(writer, this));
        }
    }

    /// <summary>
    /// The revision of the Milo scene.
    /// </summary>
    public uint revision;

    /// <summary>
    /// The type of the directory, i.e. "ObjectDir", "RndDir", etc.
    /// </summary>
    public Symbol type = Symbol.Empty;

    /// <summary>
    /// The name of the directory, i.e. "uniq0", "NewRndDir", etc.
    /// </summary>
    public Symbol name = Symbol.Empty;

    /// <summary>
    /// Hash table reservation hint, as read from the file. HMX writes HashTableUsedSize() * 2, i.e. twice the
    /// number of names in the directory's hash table. Auto-recomputed on write, see <see cref="ComputeHashTableSize"/>.
    /// </summary>
    public uint stringTableCount;

    /// <summary>
    /// String table reservation hint, as read from the file. HMX writes StrTableUsedSize(), the bytes of
    /// null-terminated names in the directory's string table. Recomputed on write, see <see cref="ComputeStringTableSize"/>.
    /// </summary>
    public uint stringTableSize;

    private int hashTableSizeAdjustment;
    private int stringTableSizeAdjustment;

    /// <summary>
    /// Absolute stream position where stringTableCount was read. Used by MiloVerifier to exclude from hash.
    /// </summary>
    public long stringTableAbsolutePosition = -1;

    /// <summary>
    /// Absolute stream position where DirObjBytes starts.
    /// </summary>
    public long dirDataStartAbsolutePosition = -1;

    public bool unkBool32;


    /// <summary>
    /// The external resources of the directory. Only used in GH1-era scenes (and prob before).
    /// </summary>
    public List<Symbol> externalResources = new();

    private uint entryCount;

    /// <summary>
    /// The entries of the directory.
    /// </summary>
    public List<Entry> entries = new();

    public Object directory;

    /// <summary>
    /// The platform the Milo scene is for. Determines certain platform specific things (texture swapping on Xbox 360, etc.)
    /// </summary>
    public Platform platform = Platform.PS3;

    private byte[] _dirObjBytes = Array.Empty<byte>();
    private ReadOnlyMemory<byte> _deferredDirObjBytes;
    private bool _hasDeferredDirObjBytes;

    public byte[] DirObjBytes
    {
        get
        {
            if (_hasDeferredDirObjBytes)
            {
                _dirObjBytes = _deferredDirObjBytes.ToArray();
                _deferredDirObjBytes = _dirObjBytes;
                _hasDeferredDirObjBytes = false;
            }

            return _dirObjBytes;
        }
        set
        {
            _dirObjBytes = value ?? Array.Empty<byte>();
            _deferredDirObjBytes = _dirObjBytes;
            _hasDeferredDirObjBytes = false;
        }
    }

    internal ReadOnlySpan<byte> DirObjBytesSpan => _hasDeferredDirObjBytes ? _deferredDirObjBytes.Span : _dirObjBytes;
    public ReadOnlyMemory<byte> DirObjBytesMemory => _hasDeferredDirObjBytes ? _deferredDirObjBytes : _dirObjBytes;

    public DirectoryMeta Read(EndianReader reader)
    {
        revision = reader.ReadUInt32();

        // if the revision is over 50, switch to little endian and attempt the read again to guess endianness
        // this works well since the highest known revision before switch to Forge was 32
        if (revision > 50)
        {
            reader.Endianness = Endian.LittleEndian;
            reader.BaseStream.Position -= 4;
            revision = reader.ReadUInt32();
        }

        // support freq<-->dc3 versions
        if (revision != 6 && revision != 10 && revision != 24 && revision != 25 && revision != 26 && revision != 28 && revision != 32)
        {
            throw new UnsupportedMiloSceneRevision(revision);
        }

        if (revision > 10)
        {
            type = Symbol.Read(reader);
            name = Symbol.Read(reader);

            stringTableAbsolutePosition = reader.BaseStream.Position;
            stringTableCount = reader.ReadUInt32();
            stringTableSize = reader.ReadUInt32();

            if (revision >= 32)
            {
                unkBool32 = reader.ReadBoolean();
            }
        }

        entryCount = reader.ReadUInt32();

        for (int i = 0; i < entryCount; i++)
        {
            Entry entry = new(Symbol.Read(reader), Symbol.Read(reader), null);
            entries.Add(entry);
        }

        if (revision > 10)
        {
            hashTableSizeAdjustment = (int)stringTableCount - ComputeHashTableSize();
            stringTableSizeAdjustment = (int)stringTableSize - ComputeStringTableSize();
        }

        // only gh1-era stuff seems to have this
        if (revision == 10)
        {
            externalResources = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        dirDataStartAbsolutePosition = reader.BaseStream.Position;
        long dirDataStart = dirDataStartAbsolutePosition;

        if (type.value == "")
        {
            Debug.WriteLine("GH1-style empty directory detected, just reading children");
        }
        else if (DirectoryFactories.TryGetValue(type.value, out var factory) &&
                 DirectoryReaders.TryGetValue(type.value, out var readerFunc))
        {
            Debug.WriteLine($"Reading {type.value} {name.value}");
            directory = factory(0);
            readerFunc(reader, directory, this, new Entry(type, name, directory));
        }
        else
        {
            throw new Exception("Unknown directory type: " + type.value + ", cannot continue reading Milo scene");
        }

        long dirDataEnd = reader.BaseStream.Position;
        if (dirDataEnd > dirDataStart)
        {
            int dirObjectLength = (int)(dirDataEnd - dirDataStart);
            if (reader.BaseStream is MemoryStream dirStream &&
                BinaryParsingUtils.TryGetMemorySlice(dirStream, (int)dirDataStart, dirObjectLength, out ReadOnlyMemory<byte> dirSlice))
            {
                _deferredDirObjBytes = dirSlice;
                _dirObjBytes = Array.Empty<byte>();
                _hasDeferredDirObjBytes = !dirSlice.IsEmpty;
            }
            else
            {
                reader.BaseStream.Position = dirDataStart;
                DirObjBytes = reader.ReadBlock(dirObjectLength);
            }
        }

        // HMX's DEADDEAD magic value used to signify an object has ended
        ReadOnlySpan<byte> endMarker = stackalloc byte[] { 0xAD, 0xDE, 0xAD, 0xDE };

        foreach (Entry entry in entries)
        {
            if (reader.BaseStream.Position >= reader.BaseStream.Length)
            {
                if (revision == 25)
                {
                    entry.hasSerializedData = false;
                    continue;
                }
                throw new EndOfStreamException("The Milo directory ended before all entry payloads were read.");
            }

            long startPos = reader.BaseStream.Position;
            entry.objBytesAbsolutePosition = startPos;

            if (reader.BaseStream is MemoryStream ms &&
                BinaryParsingUtils.TryGetMemorySliceUntilEndMarker(ms, (int)ms.Position, out ReadOnlyMemory<byte> entrySlice, out _))
            {
                entry.SetDeferredObjBytes(entrySlice);
            }
            else
            {
                entry.objBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
            }

            reader.BaseStream.Position = startPos;

            ReadEntry(reader, entry);

            if (entry.dir != null)
            {
                entry.dir.platform = platform;
            }
        }

        return this;
    }

    // i cant tell you how long it took until I finally understood how to actually calculate this properly
    public int ComputeHashTableSize()
    {
        int names = entries.Count + (string.IsNullOrEmpty(name.value) ? 0 : 1);
        return names * 2;
    }

    public int ComputeStringTableSize()
    {
        int size = NullTerminatedLength(name);
        foreach (Entry entry in entries)
        {
            size += NullTerminatedLength(entry.name);
        }
        return size;
    }

    private static int NullTerminatedLength(Symbol symbol)
    {
        string value = symbol.value;
        return string.IsNullOrEmpty(value) ? 0 : Encoding.Latin1.GetByteCount(value) + 1;
    }

    public void Write(EndianWriter writer)
    {
        writer.WriteUInt32(revision);

        if (revision > 10)
        {
            Symbol.Write(writer, type);
            Symbol.Write(writer, name);

            writer.WriteInt32(Math.Max(0, ComputeHashTableSize() + hashTableSizeAdjustment));
            writer.WriteInt32(Math.Max(0, ComputeStringTableSize() + stringTableSizeAdjustment));

            if (revision >= 32)
            {
                writer.WriteBoolean(unkBool32);
            }
        }

        writer.WriteInt32((int)entries.Count);

        foreach (Entry entry in entries)
        {
            Symbol.Write(writer, entry.type);
            Symbol.Write(writer, entry.name);
        }

        // only gh1-era stuff seems to have this
        if (revision == 10)
        {
            AssetHelpers.WriteList(writer, externalResources, externalResource => Symbol.Write(writer, externalResource));
        }

        // Handle GH1-style empty directory types (no root directory object)
        if (type.value == "")
        {
            Debug.WriteLine("GH1-style empty directory detected, skipping directory object write");
        }
        // Use dictionary-based factory pattern for O(1) lookup
        else if (DirectoryWriters.TryGetValue(type.value, out var writerFunc))
        {
            writerFunc(writer, directory, this, new Entry(type, name, directory));
            writer.MarkChunk();
        }
        else
        {
            throw new Exception("Unknown directory type: " + type.value + ", cannot continue writing Milo scene");
        }

        // write the children entries
        foreach (Entry entry in entries)
        {
            if (!entry.hasSerializedData)
            {
                continue;
            }
            WriteEntry(writer, entry);
        }
    }

    public void ReadEntry(EndianReader reader, DirectoryMeta.Entry entry)
    {
        entry.OnBeforeRead(reader);

        try
        {
            if (EntryReadActions.TryGetValue(entry.type.value, out var readAction))
            {
                readAction(reader, this, entry);
            }
            else
            {
                // Unknown entry type - read until we see 0xADDEADDE to skip over it
                Debug.WriteLine("Unknown entry type " + entry.type.value + " of name " + entry.name.value + ", read an Object and then read until we see 0xADDEADDE to skip over it, curpos" + reader.BaseStream.Position);

                entry.typeRecognized = false;

                if (reader.BaseStream is MemoryStream ums &&
                    BinaryParsingUtils.TryGetMemorySliceUntilEndMarker(ums, (int)ums.Position, out _, out int bytesConsumed))
                {
                    ums.Position += bytesConsumed;
                }
                else
                {
                    BinaryParsingUtils.SkipUntilEndMarker(reader);
                }

                Debug.WriteLine("Found ending of file, new position: " + reader.BaseStream.Position);
            }
        }
        catch (Exception ex)
        {
            // Every asset's Read starts by reading a uint32 combinedRevision
            ushort? objRev = null;
            ReadOnlySpan<byte> objBytes = entry.ObjBytesSpan;
            if (objBytes.Length >= 4)
            {
                uint raw = reader.Endianness == Endian.BigEndian
                    ? (uint)(objBytes[0] << 24 | objBytes[1] << 16 | objBytes[2] << 8 | objBytes[3])
                    : (uint)(objBytes[0] | objBytes[1] << 8 | objBytes[2] << 16 | objBytes[3] << 24);
                objRev = BitConverter.IsLittleEndian
                    ? (ushort)(raw & 0xFFFF)
                    : (ushort)((raw >> 16) & 0xFFFF);
            }
            throw MiloAssetReadException.WrapException(ex, this, entry, reader.BaseStream.Position, revision, objRev);
        }

        entry.OnAfterRead(reader);
    }

    public void WriteEntry(EndianWriter writer, DirectoryMeta.Entry entry)
    {
        // handle dirty assets

        entry.OnBeforeWrite(writer);

        if (entry.dirty)
        {
            writer.WriteBlock(entry.ObjBytesSpan);
            writer.WriteEndBytes();
            writer.MarkChunk();
            return;
        }

        Debug.WriteLine("Writing " + entry.type.value + " " + entry.name.value);

        // Use dictionary-based factory pattern for O(1) lookup
        if (EntryWriteActions.TryGetValue(entry.type.value, out var writeAction))
        {
            writeAction(writer, this, entry);
        }
        else
        {
            // Unknown entry type - check if it's a directory type (which would be a problem)
            if (entry.type.value.Contains("Dir"))
            {
                throw new Exception("Trying to write an unsupported dir entry of type: " + entry.type.value + ", this Milo cannot be saved");
            }

            Debug.WriteLine("Unknown entry type, dumping raw bytes for " + entry.type.value + " of name " + entry.name.value);

            // this should allow saving Milos with types that have yet to be implemented
            // TODO: get rid of this once literally everything is supported, soon :tm:
            writer.WriteBlock(entry.ObjBytesSpan);

            // write the ending bytes
            writer.WriteEndBytes();
        }

        writer.MarkChunk();
        entry.OnAfterWrite(writer);
    }

    public static Object CreateDefaultObject(string typeName)
    {
        if (EntryObjectFactories.TryGetValue(typeName, out var factory))
        {
            return factory();
        }

        return null;
    }

    public static string[] GetSupportedEntryTypes()
    {
        var types = EntryReadActions.Keys.ToArray();
        Array.Sort(types, StringComparer.OrdinalIgnoreCase);
        return types;
    }

    public static string[] GetSupportedDirectoryTypes()
    {
        var types = DirectoryFactories.Keys.ToArray();
        Array.Sort(types, StringComparer.OrdinalIgnoreCase);
        return types;
    }

    public static bool IsDirectoryType(string typeName)
    {
        return DirectoryFactories.ContainsKey(typeName);
    }

    // factories for constructing various objs
    private static readonly FrozenDictionary<string, Func<Object>> EntryObjectFactories = new Dictionary<string, Func<Object>>
    {
        { "AnimFilter", () => new RndAnimFilter() },
        { "PlotDef", () => new PlotDef() },
        { "CharNeighborhood", () => new CharNeighborhood() },
        { "CrowdExtrasManager", () => new CrowdExtrasManager() },
        { "CrowdPreset", () => new CrowdPreset() },
        { "PlayerPortrait", () => new PlayerPortrait() },
        { "SyncWorldFx", () => new SyncWorldFx() },
        { "TrackGeom", () => new TrackGeom() },
        { "CamShotKeyframePreset", () => new CamShotKeyframePreset() },
        { "SplineDeform", () => new SplineDeform() },
        { "SplineDeformNode", () => new SplineDeformNode() },
        { "SplineDeformStraighten", () => new SplineDeformStraighten() },
        { "SplineDeformBend", () => new SplineDeformBend() },
        { "SplineDeformTwist", () => new SplineDeformTwist() },
        { "TrackRegion", () => new TrackRegion() },
        { "PlotBeatAnims", () => new PlotBeatAnims() },
        { "Ribbon", () => new RndRibbon() },
        { "TriggerGroup", () => new RndTriggerGroup() },
        { "CollideableCylinder", () => new CollideableCylinder() },
        { "CollideableCapsule", () => new CollideableCapsule() },
        { "CollideableSet", () => new CollideableSet() },
        { "CollideableMidiOverride", () => new CollideableMidiOverride() },
        { "CollideablePercussion", () => new CollideablePercussion() },
        { "P9CrowdAudio", () => new P9CrowdAudio() },
        { "P9TransDraw", () => new P9TransDraw() },
        { "DataDrivenCollection", () => new DataDrivenCollection() },
        { "P9WaypointConfiguration", () => new P9WaypointConfiguration() },
        { "CharIKHead", () => new CharIKHead() },
        { "CharSleeve", () => new CharSleeve() },
        { "FxSendBitCrush", () => new FxSendBitCrush() },
        { "ButtonEx", () => new Gh1ButtonEx() },
        { "PictureEx", () => new Gh1PictureEx() },
        { "TextEntry", () => new Gh1TextEntry() },
        { "Slider", () => new Gh1Slider() },
        { "PictureToggle", () => new Gh1PictureToggle() },
        { "List", () => new Gh1List() },
        { "BandButton", () => new BandButton() },
        { "BandSlider", () => new BandSlider() },
        { "BandTextEntry", () => new BandTextEntry() },
        { "BandCamShot", () => new BandCamShot() },
        { "BandCharDesc", () => new BandCharDesc() },
        { "BandConfiguration", () => new BandConfiguration() },
        { "BandDirector", () => new BandDirector() },
        { "BandFaceDeform", () => new BandFaceDeform() },
        { "BandLabel", () => new BandLabel() },
        { "LabelEx", () => new BandLabel() },
        { "BandList", () => new BandList() },
        { "BandMeshLauncher", () => new BandMeshLauncher() },
        { "BandPlacer", () => new BandPlacer() },
        { "Placer", () => new BandPlacer() },
        { "BandScreen", () => new Object() },
        { "BandSongPref", () => new BandSongPref() },
        { "BandSwatch", () => new BandSwatch() },
        { "BandWardrobe", () => new BandWardrobe() },
        { "BinkClip", () => new BinkClip() },
        { "BustAMoveData", () => new BustAMoveData() },
        { "Cam", () => new RndCam() },
        { "CamShot", () => new CamShot() },
        { "CharClip", () => new CharClip() },
        { "CharClipFilter", () => new CharClipFilter() },
        { "CharClipSamples", () => new CharClip() },
        { "CharClipGroup", () => new CharClipGroup() },
        { "CharCollide", () => new CharCollide() },
        { "CharDriver", () => new CharDriver() },
        { "CharDriverMidi", () => new CharDriverMidi() },
        { "CharEyes", () => new CharEyes() },
        { "CharForeTwist", () => new CharForeTwist() },
        { "CharGuitarString", () => new CharGuitarString() },
        { "CharHair", () => new CharHair() },
        { "CharIKHand", () => new CharIKHand() },
        { "CharIKFoot", () => new CharIKFoot() },
        { "CharIKMidi", () => new CharIKMidi() },
        { "CharIKRod", () => new CharIKRod() },
        { "CharIKScale", () => new CharIKScale() },
        { "CharInterest", () => new CharInterest() },
        { "CharLipSync", () => new CharLipSync() },
        { "CharLipSyncDriver", () => new CharLipSyncDriver() },
        { "FaceFxLipSyncServo", () => new FaceFxLipSyncServo() },
        { "CharLookAt", () => new CharLookAt() },
        { "CharMeshHide", () => new CharMeshHide() },
        { "CharPosConstraint", () => new CharPosConstraint() },
        { "CharServoBone", () => new CharServoBone() },
        { "CharUpperTwist", () => new CharUpperTwist() },
        { "CharWalk", () => new CharWalk() },
        { "CharWeightSetter", () => new CharWeightSetter() },
        { "CheckboxDisplay", () => new CheckboxDisplay() },
        { "CheckBox", () => new CheckBox() },
        { "ColorPalette", () => new ColorPalette() },
        { "CreditList", () => new RndCreditList() },
        { "DancerSequence", () => new DancerSequence() },
        { "DeferDraw", () => new RndDeferDraw() },
        { "Distort", () => new RndDistort() },
        { "Environ", () => new RndEnviron() },
        { "EventTrigger", () => new EventTrigger() },
        { "FileMerger", () => new FileMerger() },
        { "Flare", () => new RndFlare() },
        { "Font", () => new RndFont() },
        { "Fur", () => new RndFur() },
        { "Group", () => new RndGroup() },
        { "View", () => new RndGroup() },
        { "HamBattleData", () => new HamBattleData() },
        { "HamDirector", () => new HamDirector() },
        { "HamLabel", () => new HamLabel() },
        { "HamNavProvider", () => new HamNavProvider() },
        { "HamNavList", () => new HamNavList() },
        { "HamIKEffector", () => new HamIKEffector() },
        { "HamRibbon", () => new HamRibbon() },
        { "SongCollision", () => new SongCollision() },
        { "HamCamShot", () => new HamCamShot() },
        { "MetaMaterial", () => new MetaMaterial() },
        { "Font3d", () => new RndFont3d() },
        { "DepthBuffer3D", () => new DepthBuffer3D() },
        { "SkeletonViz", () => new SkeletonViz() },
        { "HamButton", () => new HamButton() },
        { "HamButtonSet", () => new HamButtonSet() },
        { "HamButtonList", () => new HamButtonList() },
        { "GestureScroller", () => new GestureScroller() },
        { "RankProgressBar", () => new RankProgressBar() },
        { "HamSongButton", () => new HamButton() },
        { "HamIconMan", () => new HamIconMan() },
        { "PhotoSpotlightPositioner", () => new PhotoSpotlightPositioner() },
        { "RhythmDetector", () => new RhythmDetector() },
        { "RhythmDetectorGroup", () => new RhythmDetectorGroup() },
        { "RhythmBattlePlayer", () => new RhythmBattlePlayer() },
        { "RhythmBattle", () => new RhythmBattle() },
        { "HamSkeletonConverter", () => new HamSkeletonConverter() },
        { "SongDifficultyDisplay", () => new SongDifficultyDisplay() },
        { "HamWardrobe", () => new HamWardrobe() },
        { "HollaBackMinigame", () => new HollaBackMinigame() },
        { "PracticeOptionsProvider", () => new PracticeOptionsProvider() },
        { "PostProcer", () => new PostProcer() },
        { "StreamRecorder", () => new StreamRecorder() },
        { "MeterEffectMonitor", () => new MeterEffectMonitor() },
        { "FxSendMeterEffect", () => new FxSendMeterEffect() },
        { "AudioDuckerTrigger", () => new AudioDuckerTrigger() },
        { "TexProc", () => new TexProc() },
        { "Spline", () => new RndSpline() },
        { "HamMeterEffectMonitor", () => new MeterEffectMonitor() },
        { "StreamRenderer", () => new StreamRenderer() },
        { "HamIKSkeleton", () => new HamIKSkeleton() },
        { "HamRegulate", () => new HamRegulate() },
        { "HamDriver", () => new HamDriver() },
        { "StarsDisplay", () => new StarsDisplay() },
        { "HamCamTransform", () => new HamCamTransform() },
        { "TransConstraint", () => new TransConstraint() },
        { "CharFeedback", () => new CharFeedback() },
        { "HamList", () => new HamList() },
        { "HamMove", () => new HamMove() },
        { "HamPartyJumpData", () => new HamPartyJumpData() },
        { "HamSupereasyData", () => new HamSupereasyData() },
        { "InlineHelp", () => new InlineHelp() },
        { "InterstitialPanel", () => new Object() },
        { "Label3d", () => new Label3d() },
        { "Light", () => new RndLight() },
        { "LightAnim", () => new RndLightAnim() },
        { "LightPreset", () => new LightPreset() },
        { "Line", () => new RndLine() },
        { "Mat", () => new RndMat() },
        { "MatAnim", () => new RndMatAnim() },
        { "Mesh", () => new RndMesh() },
        { "MeshDeform", () => new RndMeshDeform() },
        { "MidiInstrument", () => new MidiInstrument() },
        { "MidiParser", () => new MidiParser() },
        { "MoggClip", () => new MoggClip() },
        { "Movie", () => new RndMovie() },
        { "MotionBlur", () => new RndMotionBlur() },
        { "Morph", () => new RndMorph() },
        { "MoveGraph", () => new MoveGraph() },
        { "MsgSource", () => new Object() },
        { "Object", () => new Object() },
        { "OutfitConfig", () => new OutfitConfig() },
        { "OutfitLoader", () => new OutfitLoader() },
        { "P9Director", () => new P9Director() },
        { "P9SongPref", () => new P9SongPref() },
        { "ParallelGroupSeq", () => new ParallelGroupSeq() },
        { "ParticleSys", () => new RndParticleSys() },
        { "ParticleSysAnim", () => new RndParticleSysAnim() },
        { "PollAnim", () => new RndPollAnim() },
        { "PostProc", () => new RndPostProc() },
        { "PracticeSection", () => new PracticeSection() },
        { "PropAnim", () => new RndPropAnim() },
        { "RandomGroupSeq", () => new RandomGroupSeq() },
        { "ScreenMask", () => new RndScreenMask() },
        { "Screenshot", () => new Screenshot() },
        { "Text", () => new RndText() },
        { "Set", () => new RndSet() },
        { "Sfx", () => new Sfx() },
        { "Sound", () => new Sound() },
        { "FlowNode", () => new FlowNode() },
        { "FlowCommand", () => new FlowCommand() },
        { "PropertyEventProvider", () => new PropertyEventProvider() },
        { "FlowSlider", () => new FlowSlider() },
        { "FlowDistance", () => new FlowDistance() },
        { "FlowMultiSetProperty", () => new FlowMultiSetProperty() },
        { "FlowEventListener", () => new FlowEventListener() },
        { "FlowTrigger", () => new FlowTrigger() },
        { "FlowSetProperty", () => new FlowSetProperty() },
        { "FlowAnimate", () => new FlowAnimate() },
        { "FlowSound", () => new FlowSound() },
        { "FlowValueCase", () => new FlowValueCase() },
        { "FlowRun", () => new FlowRun() },
        { "FlowIf", () => new FlowIf() },
        { "FlowSwitchCase", () => new FlowSwitchCase() },
        { "FlowWhile", () => new FlowWhile() },
        { "FlowOnStop", () => new FlowOnStop() },
        { "FlowSequence", () => new FlowSequence() },
        { "FlowTimer", () => new FlowTimer() },
        { "FlowLabel", () => new FlowLabel() },
        { "FlowOutPort", () => new FlowOutPort() },
        { "FlowPickOne", () => new FlowPickOne() },
        { "FlowSwitch", () => new FlowSwitch() },
        { "FlowQueueable", () => new FlowQueueable() },
        { "Flow", () => new FlowObject() },
        { "SfxSeq", () => new SfxSeq() },
        { "SongSectionController", () => new SongSectionController() },
        { "Spotlight", () => new Spotlight() },
        { "SpotlightDrawer", () => new SpotlightDrawer() },
        { "SynthFader", () => new SynthFader() },
        { "SkeletonClip", () => new SkeletonClip() },
        { "SynthSample", () => new SynthSample() },
        { "Tex", () => new RndTex() },
        { "TexBlendController", () => new RndTexBlendController() },
        { "TexBlender", () => new RndTexBlender() },
        { "TexMovie", () => new RndTexMovie() },
        { "TrackWidget", () => new TrackWidget() },
        { "TrackWidgetRndDir", () => new TrackWidgetRndDir() },
        { "TrackWidgetMultiMesh", () => new TrackWidgetMultiMesh() },
        { "TrainerChallenge", () => new Object() },
        { "Trans", () => new RndTrans() },
        { "TransAnim", () => new RndTransAnim() },
        { "TransProxy", () => new RndTransProxy() },
        { "UIButton", () => new UIButton() },
        { "UIColor", () => new UIColor() },
        { "UIComponent", () => new UIComponent() },
        { "UIGuide", () => new UIGuide() },
        { "UILabel", () => new UILabel() },
        { "UIList", () => new UIList() },
        { "UIListArrow", () => new UIListArrow() },
        { "UIListCustom", () => new UIListCustom() },
        { "UIListHighlight", () => new UIListHighlight() },
        { "UIListLabel", () => new UIListLabel() },
        { "UIListMesh", () => new UIListMesh() },
        { "UIListSlot", () => new UIListSlot() },
        { "UIListWidget", () => new UIListWidget() },
        { "UIPanel", () => new Object() },
        { "UIPicture", () => new UIPicture() },
        { "UISlider", () => new UISlider() },
        { "UITrigger", () => new UITrigger() },
        { "Wind", () => new RndWind() },
        { "Waypoint", () => new Waypoint() },
        { "MeshBlend", () => new RndMeshBlend() },
        { "MeshBlendAnim", () => new RndMeshBlendAnim() },
        { "CharTrigger", () => new CharTrigger() },
        { "CharMarker", () => new CharMarker() },
        { "BlendShapesDriver", () => new BlendShapesDriver() },
        { "SynthEmitter", () => new SynthEmitter() },
        { "CutsceneDriver", () => new CutsceneDriver() },
        { "Blur", () => new RndBlur() },
        { "WorldCrowd", () => new WorldCrowd() },
        { "WorldReflection", () => new WorldReflection() },
        // Directory types (use rev 0 as default)
        { "BandCharacter", () => new BandCharacter(0) },
        { "BackgroundDir", () => new BackgroundDir(0) },
        { "BandCrowdMeterDir", () => new BandCrowdMeterDir(0) },
        { "BandLeadMeter", () => new BandLeadMeter(0) },
        { "BandScoreDisplay", () => new BandScoreDisplay(0) },
        { "BandStarMeterDir", () => new BandStarMeterDir(0) },
        { "BandStreakDisplay", () => new BandStreakDisplay(0) },
        { "GHTrackDir", () => new GHTrackDir(0) },
        { "BandScoreboard", () => new BandScoreboard(0) },
        { "BandPowerMeterDir", () => new BandPowerMeterDir(0) },
        { "ScoreboardDir", () => new ScoreboardDir(0) },
        { "BandStarDisplay", () => new BandStarDisplay(0) },
        { "Character", () => new Character(0) },
        { "CharBoneDir", () => new CharBoneDir(0) },
        { "CharClipSet", () => new CharClipSet(0) },
        { "CompositeCharacter", () => new CompositeCharacter(0) },
        { "CrowdMeterIcon", () => new BandCrowdMeterIcon(0) },
        { "EndingBonusDir", () => new RndDir(0) },
        { "GemTrackDir", () => new GemTrackDir(0) },
        { "LoadingPanelDir", () => new LoadingPanelDir(0) },
        { "MoveDir", () => new MoveDir(0) },
        { "HamListRibbon", () => new HamListRibbon() },
        { "HamPhotoDisplay", () => new HamPhotoDisplay() },
        { "HamPhraseMeter", () => new HamPhraseMeter() },
        { "HamScrollSpeedIndicator", () => new HamScrollSpeedIndicator() },
        { "HamCharacter", () => new HamCharacter() },
        { "HamVisDir", () => new HamVisDir() },
        { "ObjectDir", () => new ObjectDir(0) },
        { "OverdriveMeterDir", () => new OverdriveMeterDir(0) },
        { "OvershellDir", () => new OvershellDir(0) },
        { "P9Character", () => new P9Character(0) },
        { "PanelDir", () => new PanelDir(0) },
        { "PitchArrowDir", () => new PitchArrowDir(0) },
        { "RenderDir", () => new RenderDir(0) },
        { "RndDir", () => new RndDir(0) },
        { "SkeletonDir", () => new SkeletonDir(0) },
        { "StreakMeterDir", () => new StreakMeterDir(0) },
        { "SynthDir", () => new SynthDir(0) },
        { "TrackDir", () => new TrackDir(0) },
        { "TrackPanelDir", () => new TrackPanelDir(0) },
        { "H2HTrackPanelDir", () => new H2HTrackPanelDir(0) },
        { "EventCompleteDir", () => new EventCompleteDir(0) },
        { "WorldMapDir", () => new WorldMapDir(0) },
        { "UILabelDir", () => new UILabelDir(0) },
        { "UIListDir", () => new UIListDir(0) },
        { "DynamicLeaderboard", () => new DynamicLeaderboard() },
        { "SingleTrackDir", () => new SingleTrackDir() },
        { "WorldTrackDir", () => new WorldTrackDir() },
        { "PlotDir", () => new PlotDir() },
        { "HorizonDir", () => new HorizonDir() },
        { "GroundPlaneVis", () => new GroundPlaneVis() },
        { "UnisonIcon", () => new UnisonIcon(0) },
        { "VocalTrackDir", () => new VocalTrackDir(0) },
        { "WorldDir", () => new WorldDir(0) },
        { "WorldInstance", () => new WorldInstance(0) },
    }.ToFrozenDictionary();

    public static DirectoryMeta New(string type, string name, uint sceneRevision, ushort rootDirRevision)
    {
        DirectoryMeta dir = new();
        dir.type = type;
        dir.name = name;

        // Use dictionary-based factory pattern for O(1) lookup
        if (DirectoryFactories.TryGetValue(type, out var factory))
        {
            dir.directory = factory(rootDirRevision);
        }
        else
        {
            throw new Exception("Unknown directory type: " + type + ", cannot continue creating directory");
        }

        dir.entries = new List<Entry>();
        dir.revision = sceneRevision;
        return dir;
    }

    public override string ToString()
    {
        return string.Format("{1} ({0})", type, name);
    }
}
