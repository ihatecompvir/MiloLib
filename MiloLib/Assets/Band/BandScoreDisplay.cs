using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandScoreDisplay"), Description("Score displaying hud element")]
public class BandScoreDisplay : RndDir
{
    private ushort altRevision;
    private ushort revision;
    public List<Symbol> firstObjects = new();
    public List<Symbol> secondObjects = new();
    public Symbol scoreObject = Symbol.Empty;
    public int firstValue;
    public int secondValue;

    public BandScoreDisplay(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandScoreDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);

        bool isRootDirectory = IsRootDirectory(parent, entry);

        if (revision >= 3)
        {
            if (isRootDirectory)
            {
                ReadObjectFields(reader);
            }
            if (!entry.isProxy)
            {
                ReadScalarFields(reader, true);
            }
        }

        base.Read(reader, false, parent, entry);

        if (revision < 3)
        {
            if (isRootDirectory)
            {
                ReadObjectFields(reader);
            }
            if (!entry.isProxy)
            {
                ReadScalarFields(reader, revision >= 2);
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
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);

        bool isRootDirectory = false;
        if (entry != null)
        {
            isRootDirectory = IsRootDirectory(parent, entry);
        }

        if (entry != null && revision >= 3)
        {
            if (isRootDirectory)
            {
                WriteObjectFields(writer);
            }
            if (!entry.isProxy)
            {
                WriteScalarFields(writer, true);
            }
        }

        base.Write(writer, false, parent, entry);

        if (entry != null && revision < 3)
        {
            if (isRootDirectory)
            {
                WriteObjectFields(writer);
            }
            if (!entry.isProxy)
            {
                WriteScalarFields(writer, revision >= 2);
            }
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

    private void ReadObjectFields(EndianReader reader)
    {
        firstObjects = ReadObjectVector(reader);
        secondObjects = ReadObjectVector(reader);
        scoreObject = Symbol.Read(reader);
    }

    private void ReadScalarFields(EndianReader reader, bool hasSecondValue)
    {
        firstValue = reader.ReadInt32();
        if (hasSecondValue)
        {
            secondValue = reader.ReadInt32();
        }
    }

    private void WriteObjectFields(EndianWriter writer)
    {
        WriteObjectVector(writer, firstObjects);
        WriteObjectVector(writer, secondObjects);
        Symbol.Write(writer, scoreObject);
    }

    private void WriteScalarFields(EndianWriter writer, bool hasSecondValue)
    {
        writer.WriteInt32(firstValue);
        if (hasSecondValue)
        {
            writer.WriteInt32(secondValue);
        }
    }

    private static List<Symbol> ReadObjectVector(EndianReader reader)
    {
        return AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
    }

    private static void WriteObjectVector(EndianWriter writer, List<Symbol> values)
    {
        AssetHelpers.WriteList(writer, values, (Symbol value) => Symbol.Write(writer, value));
    }

    private static bool IsRootDirectory(DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        return entry.type.value == parent.type.value && entry.name.value == parent.name.value;
    }
}
