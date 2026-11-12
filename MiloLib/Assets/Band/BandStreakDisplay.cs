using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandStreakDisplay"), Description("Streak displaying hud element")]
public class BandStreakDisplay : RndDir
{
    private ushort altRevision;
    private ushort revision;
    public Symbol firstObject = Symbol.Empty;
    public Symbol secondObject = Symbol.Empty;
    public Symbol thirdObject = Symbol.Empty;
    public Symbol fourthObject = Symbol.Empty;
    public List<Symbol> firstObjects = new();
    public List<Symbol> secondObjects = new();
    public Symbol fifthObject = Symbol.Empty;
    public List<Symbol> thirdObjects = new();
    public List<Symbol> remainingObjects = new();
    public float rate;
    public int firstValue;
    public bool flag;
    public int secondValue;
    public int thirdValue;

    public BandStreakDisplay(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandStreakDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
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
                ReadScalarFields(reader);
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
                ReadScalarFields(reader);
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
                WriteScalarFields(writer);
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
                WriteScalarFields(writer);
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
        firstObject = Symbol.Read(reader);
        secondObject = Symbol.Read(reader);
        thirdObject = Symbol.Read(reader);
        fourthObject = Symbol.Read(reader);
        firstObjects = ReadObjectVector(reader);
        secondObjects = ReadObjectVector(reader);
        fifthObject = Symbol.Read(reader);
        thirdObjects = ReadObjectVector(reader);
        ReadRemainingObjects(reader);
        rate = reader.ReadFloat();
    }

    private void WriteObjectFields(EndianWriter writer)
    {
        Symbol.Write(writer, firstObject);
        Symbol.Write(writer, secondObject);
        Symbol.Write(writer, thirdObject);
        Symbol.Write(writer, fourthObject);
        WriteObjectVector(writer, firstObjects);
        WriteObjectVector(writer, secondObjects);
        Symbol.Write(writer, fifthObject);
        WriteObjectVector(writer, thirdObjects);
        WriteRemainingObjects(writer);
        writer.WriteFloat(rate);
    }

    private void ReadRemainingObjects(EndianReader reader)
    {
        remainingObjects.Clear();
        for (int i = 0; i < 4; i++)
        {
            remainingObjects.Add(Symbol.Read(reader));
        }
        if (revision > 3)
        {
            remainingObjects.Add(Symbol.Read(reader));
        }
        remainingObjects.Add(Symbol.Read(reader));
        remainingObjects.Add(Symbol.Read(reader));
    }

    private void WriteRemainingObjects(EndianWriter writer)
    {
        int count = 6;
        if (revision > 3)
        {
            count = 7;
        }
        for (int i = 0; i < count; i++)
        {
            Symbol value = Symbol.Empty;
            if (i < remainingObjects.Count)
            {
                value = remainingObjects[i];
            }
            Symbol.Write(writer, value);
        }
    }

    private void ReadScalarFields(EndianReader reader)
    {
        firstValue = reader.ReadInt32();
        flag = reader.ReadBoolean();
        secondValue = reader.ReadInt32();
        thirdValue = reader.ReadInt32();
    }

    private void WriteScalarFields(EndianWriter writer)
    {
        writer.WriteInt32(firstValue);
        writer.WriteBoolean(flag);
        writer.WriteInt32(secondValue);
        writer.WriteInt32(thirdValue);
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
