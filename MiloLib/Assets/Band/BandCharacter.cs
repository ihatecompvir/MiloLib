using MiloLib.Assets.Char;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandCharacter"), Description("Band Character")]
public class BandCharacter : Character
{
    private ushort altRevision;
    private ushort revision;

    public int playFlags;

    [Name("Tempo"), Description("song tempo")]
    public Symbol tempo = Symbol.Empty;

    public uint unkInt1;

    public Symbol unkSymbol = Symbol.Empty;
    public Symbol unkSymbol2 = Symbol.Empty;

    [Name("Drum Venue"), Description("venue type for drums"), MinVersion(7)]
    public Symbol drumVenue = Symbol.Empty;

    public bool unkBool;

    [Name("Instrument Type"), Description("character's current instrument"), MinVersion(8)]
    public Symbol instrumentType = Symbol.Empty;

    [Name("Test Prefab"), Description("prefab to copy from or to"), MinVersion(1)]
    public Symbol testPrefab = Symbol.Empty;

    public BandCharDesc charDesc = new();

    public BandCharacter(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandCharacter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision == 1)
        {
            if (standalone)
            {
                AssetHelpers.ReadEnd(reader, parent, entry, revision);
            }

            return this;
        }

        // PostLoad stores the BandCharDesc body without the base Object
        charDesc = new BandCharDesc();
        charDesc.skipBase = true;
        charDesc.Read(reader, false, parent, entry);

        playFlags = reader.ReadInt32();
        tempo = Symbol.Read(reader);

        if (revision < 6)
        {
            if (revision < 4)
            {
                unkInt1 = reader.ReadUInt32();
                if (revision < 3)
                {
                    unkSymbol = Symbol.Read(reader);
                }
            }
            unkSymbol2 = Symbol.Read(reader);
        }

        if (revision > 6)
        {
            drumVenue = Symbol.Read(reader);
        }

        if (revision != 0)
        {
            testPrefab = Symbol.Read(reader);
        }

        if (revision == 2 || revision == 3 || revision == 4)
        {
            unkBool = reader.ReadBoolean();
        }

        if (revision > 7)
        {
            instrumentType = Symbol.Read(reader);
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

        if (revision == 1)
        {
            if (standalone)
            {
                writer.WriteEndBytes();
            }

            return;
        }

        charDesc.skipBase = true;
        charDesc.Write(writer, false, parent, entry);

        writer.WriteInt32(playFlags);
        Symbol.Write(writer, tempo);

        if (revision < 6)
        {
            if (revision < 4)
            {
                writer.WriteUInt32(unkInt1);
                if (revision < 3)
                {
                    Symbol.Write(writer, unkSymbol);
                }
            }
            Symbol.Write(writer, unkSymbol2);
        }

        if (revision > 6)
        {
            Symbol.Write(writer, drumVenue);
        }

        if (revision != 0)
        {
            Symbol.Write(writer, testPrefab);
        }

        if (revision == 2 || revision == 3 || revision == 4)
        {
            writer.WriteBoolean(unkBool);
        }

        if (revision > 7)
        {
            Symbol.Write(writer, instrumentType);
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
