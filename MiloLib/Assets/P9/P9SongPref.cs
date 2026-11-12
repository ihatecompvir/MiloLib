using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.P9;

[Name("P9SongPref"), Description("P9 Song Specific dreamscape and venue preferences")]
public class P9SongPref : Object
{
    public enum DreamConfig : int
    {
        None = 0,
        kP9DreamSlow = 1 << 16,
        kP9DreamFast = 1 << 17,
        kP9DreamPsych = 1 << 18
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Venue"), Description("Venue is played in")]
    public Symbol venue = Symbol.Empty;

    [Name("Mini Venues"), Description("list of minivenues to load")]
    public List<Symbol> miniVenues = new();

    [Name("Scenes"), Description("list of scenes to load")]
    public List<Symbol> scenes = new();

    [Name("Scene Groups")]
    public List<Symbol> sceneGroups = new();

    [Name("Dreamscape Outfit"), Description("dreamscape outfit in the dreamscape mini venues")]
    public Symbol dreamscapeOutfit = Symbol.Empty;

    [Name("Studio Outfit"), Description("dreamscape outfit in abby road studios")]
    public Symbol studioOutfit = Symbol.Empty;

    public List<Symbol> instruments1 = new();

    public List<Symbol> instruments2 = new();

    public List<Symbol> instruments3 = new();

    public List<Symbol> instruments4 = new();

    [Name("Tempo"), Description("Animation tempo for characters and crowd. Bpms: under 94=slow, 94-135=medium, above 135=fast")]
    public Symbol tempo = Symbol.Empty;

    [Name("Song Clips"), Description("name of song clips to load, usually just same name as song.")]
    public Symbol songClips = Symbol.Empty;

    [Name("Dreamscape Font"), Description("Font used to display lyrics in the dreamscape venue")]
    public Symbol dreamscapeFont = Symbol.Empty;

    [Name("George Amp"), Description("Amp that george uses for this song in Abbey Road Studios"), MaxVersion(20)]
    public Symbol georgeAmp = Symbol.Empty;

    [Name("John Amp"), Description("Amp that john uses for this song in Abbey Road Studios"), MaxVersion(20)]
    public Symbol johnAmp = Symbol.Empty;

    [Name("Paul Amp"), Description("Amp that paul uses for this song in Abbey Road Studios"), MaxVersion(20)]
    public Symbol paulAmp = Symbol.Empty;

    [Name("Mixer"), Description("Mixer used for this song in Abbey Road Studios"), MaxVersion(20)]
    public Symbol mixer = Symbol.Empty;

    [Name("Dreamscape Camera"), Description("What kind of camera shots to use in dreamscapes")]
    public DreamConfig dreamscapeCamera;

    [Name("Lyric Part"), Description("Harmony part to use for the venue lyrics.")]
    public Symbol lyricPart = Symbol.Empty;

    [Name("Normal Outfit"), MinVersion(25)]
    public Symbol normalOutfit = Symbol.Empty;

    [Name("Bonus Outfit"), MinVersion(25)]
    public Symbol bonusOutfit = Symbol.Empty;

    [Name("Drum Set"), MinVersion(25)]
    public Symbol drumSet = Symbol.Empty;

    [Name("Era"), MinVersion(25)]
    public Symbol era = Symbol.Empty;

    [Name("Cam Directory"), Description("Milo file containing song-specific cameras for this song"), MinVersion(25)]
    public Symbol camDirectory = Symbol.Empty;

    [Name("Media Directory"), Description("Milo file containing song-specific media (for a media wall, etc.)"), MinVersion(25)]
    public Symbol mediaDirectory = Symbol.Empty;

    [Name("Song Intro Cam"), MinVersion(25)]
    public Symbol songIntroCam = Symbol.Empty;

    [Name("Win Cam"), MinVersion(25)]
    public Symbol winCam = Symbol.Empty;

    public new P9SongPref Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        venue = Symbol.Read(reader);

        miniVenues = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        scenes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        sceneGroups = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        dreamscapeOutfit = Symbol.Read(reader);
        studioOutfit = Symbol.Read(reader);

        instruments1 = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        instruments2 = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        instruments3 = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        instruments4 = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        tempo = Symbol.Read(reader);
        songClips = Symbol.Read(reader);
        dreamscapeFont = Symbol.Read(reader);

        if (revision <= 20)
        {
            georgeAmp = Symbol.Read(reader);
            johnAmp = Symbol.Read(reader);
            paulAmp = Symbol.Read(reader);
            mixer = Symbol.Read(reader);
        }

        dreamscapeCamera = (DreamConfig)reader.ReadInt32();
        lyricPart = Symbol.Read(reader);

        if (revision >= 25)
        {
            normalOutfit = Symbol.Read(reader);
            bonusOutfit = Symbol.Read(reader);
            drumSet = Symbol.Read(reader);
            era = Symbol.Read(reader);
            camDirectory = Symbol.Read(reader);
            mediaDirectory = Symbol.Read(reader);
            songIntroCam = Symbol.Read(reader);
            winCam = Symbol.Read(reader);
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
        objFields.Write(writer, parent);

        Symbol.Write(writer, venue);

        AssetHelpers.WriteList(writer, miniVenues, s => Symbol.Write(writer, s));

        AssetHelpers.WriteList(writer, scenes, s => Symbol.Write(writer, s));

        AssetHelpers.WriteList(writer, sceneGroups, s => Symbol.Write(writer, s));

        Symbol.Write(writer, dreamscapeOutfit);
        Symbol.Write(writer, studioOutfit);

        AssetHelpers.WriteList(writer, instruments1, s => Symbol.Write(writer, s));

        AssetHelpers.WriteList(writer, instruments2, s => Symbol.Write(writer, s));

        AssetHelpers.WriteList(writer, instruments3, s => Symbol.Write(writer, s));

        AssetHelpers.WriteList(writer, instruments4, s => Symbol.Write(writer, s));

        Symbol.Write(writer, tempo);
        Symbol.Write(writer, songClips);
        Symbol.Write(writer, dreamscapeFont);

        if (revision <= 20)
        {
            Symbol.Write(writer, georgeAmp);
            Symbol.Write(writer, johnAmp);
            Symbol.Write(writer, paulAmp);
            Symbol.Write(writer, mixer);
        }

        writer.WriteInt32((int)dreamscapeCamera);
        Symbol.Write(writer, lyricPart);

        if (revision >= 25)
        {
            Symbol.Write(writer, normalOutfit);
            Symbol.Write(writer, bonusOutfit);
            Symbol.Write(writer, drumSet);
            Symbol.Write(writer, era);
            Symbol.Write(writer, camDirectory);
            Symbol.Write(writer, mediaDirectory);
            Symbol.Write(writer, songIntroCam);
            Symbol.Write(writer, winCam);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
