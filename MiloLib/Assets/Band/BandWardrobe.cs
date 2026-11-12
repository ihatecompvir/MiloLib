using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandWardrobe"), Description("Manager class that clothes characters and loads animations under different conditions")]
public class BandWardrobe : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Genre"), Description("default genres to load in Milo"), MinVersion(1)]
    public Symbol genre = Symbol.Empty;

    [Name("Tempo"), Description("default tempo to load in Milo"), MinVersion(1)]
    public Symbol tempo = Symbol.Empty;

    [Name("Vocal Gender"), Description("vocalist gender for this song, set to nothing if in Milo and want whatever you've selected"), MinVersion(5)]
    public Symbol vocalGender = Symbol.Empty;

    [Name("Play Mode"), Description("Which play mode to use, will reload characters + re-pick directed cuts"), MinVersion(1)]
    public Symbol playMode = Symbol.Empty;

    [Name("Shot Set Play Mode"), Description("When true, the play_mode will be automatically set based on the name of the camshot you are editting or placed in the anim bar"), MinVersion(1)]
    public bool shotSetPlayMode;

    [Name("Play Shot 5"), Description("Will play the shot_5 camera track, remapping invalid shot categories on the fly"), MinVersion(1)]
    public bool playShot5;

    [MinVersion(2), MaxVersion(3)]
    public Symbol oldSymbol1 = Symbol.Empty;

    [MinVersion(3), MaxVersion(3)]
    public Symbol oldSymbol2 = Symbol.Empty;

    [MinVersion(3), MaxVersion(3)]
    public Symbol oldSymbol3 = Symbol.Empty;

    [MinVersion(3), MaxVersion(3)]
    public Symbol oldSymbol4 = Symbol.Empty;

    public new BandWardrobe Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision != 0)
        {
            genre = Symbol.Read(reader);
            tempo = Symbol.Read(reader);

            if (revision > 4)
            {
                vocalGender = Symbol.Read(reader);
            }

            playMode = Symbol.Read(reader);
            shotSetPlayMode = reader.ReadBoolean();
            playShot5 = reader.ReadBoolean();

            if (revision == 2 || revision == 3)
            {
                oldSymbol1 = Symbol.Read(reader);
                if (revision > 2)
                {
                    oldSymbol2 = Symbol.Read(reader);
                    oldSymbol3 = Symbol.Read(reader);
                    oldSymbol4 = Symbol.Read(reader);
                }
            }
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

        if (revision != 0)
        {
            Symbol.Write(writer, genre);
            Symbol.Write(writer, tempo);

            if (revision > 4)
            {
                Symbol.Write(writer, vocalGender);
            }

            Symbol.Write(writer, playMode);
            writer.WriteBoolean(shotSetPlayMode);
            writer.WriteBoolean(playShot5);

            if (revision == 2 || revision == 3)
            {
                Symbol.Write(writer, oldSymbol1);
                if (revision > 2)
                {
                    Symbol.Write(writer, oldSymbol2);
                    Symbol.Write(writer, oldSymbol3);
                    Symbol.Write(writer, oldSymbol4);
                }
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
