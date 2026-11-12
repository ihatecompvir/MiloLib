using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("InstrumentDifficultyDisplay"), Description("Song Instrument Difficulty Display")]
public class InstrumentDifficultyDisplay : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Instrument Type"), Description("Type of instrument")]
    public Symbol instrumentType = new(0, "band");
    [Name("Difficulty"), Description("Difficulty")]
    public int difficulty;
    [Name("Has Part"), Description("whether or not instrument has a part to play")]
    public bool hasPart;
    [Name("Instrument Color Override"), Description("Instrument color override for this instance"), MinVersion(1)]
    public Symbol instrumentColorOverride = Symbol.Empty;
    [Name("Num Vocal Parts"), Description("Number of vocal parts"), MinVersion(2)]
    public int numVocalParts;
    [Name("Instrument State"), Description("how the instrument should be shown"), MinVersion(3)]
    public int instrumentState;

    public new InstrumentDifficultyDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        instrumentType = Symbol.Read(reader);
        difficulty = reader.ReadInt32();
        hasPart = reader.ReadBoolean();
        if (revision >= 1)
        {
            instrumentColorOverride = Symbol.Read(reader);
        }

        if (revision >= 2)
        {
            numVocalParts = reader.ReadInt32();
        }

        if (revision == 3)
        {
            instrumentState = reader.ReadBoolean() ? 1 : 0;
        }
        else if (revision >= 4)
        {
            instrumentState = reader.ReadInt32();
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

        Symbol.Write(writer, instrumentType);
        writer.WriteInt32(difficulty);
        writer.WriteBoolean(hasPart);
        if (revision >= 1)
        {
            Symbol.Write(writer, instrumentColorOverride);
        }

        if (revision >= 2)
        {
            writer.WriteInt32(numVocalParts);
        }

        if (revision == 3)
        {
            writer.WriteBoolean(instrumentState != 0);
        }
        else if (revision >= 4)
        {
            writer.WriteInt32(instrumentState);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
