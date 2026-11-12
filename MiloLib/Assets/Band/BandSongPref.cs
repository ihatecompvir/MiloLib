using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandSongPref"), Description("Band Song Preferences, per song file settable properties")]
public class BandSongPref : Object
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        // this is only for RB3
        { Game.MiloGame.RockBand3, 3 },
    };

    private ushort altRevision;
    private ushort revision;

    [Name("Part 2 Instrument"), Description("Who should sing the vocal part2?")]
    public Symbol part2Instrument = Symbol.Empty;

    [Name("Part 3 Instrument"), Description("Who should sing the vocal part3?")]
    public Symbol part3Instrument = Symbol.Empty;

    [Name("Part 4 Instrument"), Description("Who should sing the vocal part4?")]
    public Symbol part4Instrument = Symbol.Empty;

    [MinVersion(1), MaxVersion(2)]
    public byte oldDump;

    [Name("Animation Genre"), Description("Animation genre for the song"), MinVersion(2)]
    public Symbol animationGenre = Symbol.Empty;

    public new BandSongPref Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        part2Instrument = Symbol.Read(reader);
        part3Instrument = Symbol.Read(reader);
        part4Instrument = Symbol.Read(reader);

        if (revision != 0 && revision < 3)
        {
            oldDump = reader.ReadByte();
        }

        if (revision > 1)
        {
            animationGenre = Symbol.Read(reader);
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

        Symbol.Write(writer, part2Instrument);
        Symbol.Write(writer, part3Instrument);
        Symbol.Write(writer, part4Instrument);

        if (revision != 0 && revision < 3)
        {
            writer.WriteByte(oldDump);
        }

        if (revision > 1)
        {
            Symbol.Write(writer, animationGenre);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
