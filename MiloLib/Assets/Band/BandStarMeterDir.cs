using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandStarMeterDir"), Description("Star meter hud element")]
public class BandStarMeterDir : RndDir
{
    private ushort altRevision;
    private ushort revision;
    public List<Symbol> objects = new();
    public float value;
    public int state;
    public bool firstFlag;
    public bool secondFlag;

    public BandStarMeterDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandStarMeterDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);

        bool isRootDirectory = IsRootDirectory(parent, entry);

        if (revision < 2 || isRootDirectory)
        {
            ReadObjects(reader, 3);
            if (revision > 3)
            {
                ReadObjects(reader, 2);
            }
            if (revision > 4)
            {
                ReadObjects(reader, 1);
            }
        }

        if (!entry.isProxy)
        {
            value = reader.ReadFloat();
            if (revision > 2)
            {
                state = reader.ReadInt32();
            }
            if (revision > 3)
            {
                firstFlag = reader.ReadBoolean();
                secondFlag = reader.ReadBoolean();
            }
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
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);

        bool isProxy = false;
        bool isRootDirectory = false;
        if (entry != null)
        {
            isProxy = entry.isProxy;
            isRootDirectory = IsRootDirectory(parent, entry);
        }

        if (revision < 2 || isRootDirectory)
        {
            WriteObjects(writer, 0, 3);
            if (revision > 3)
            {
                WriteObjects(writer, 3, 2);
            }
            if (revision > 4)
            {
                WriteObjects(writer, 5, 1);
            }
        }

        if (!isProxy)
        {
            writer.WriteFloat(value);
            if (revision > 2)
            {
                writer.WriteInt32(state);
            }
            if (revision > 3)
            {
                writer.WriteBoolean(firstFlag);
                writer.WriteBoolean(secondFlag);
            }
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }

    private void ReadObjects(EndianReader reader, int count)
    {
        for (int i = 0; i < count; i++)
        {
            objects.Add(Symbol.Read(reader));
        }
    }

    private void WriteObjects(EndianWriter writer, int start, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Symbol value = Symbol.Empty;
            int index = start + i;
            if (index < objects.Count)
            {
                value = objects[index];
            }
            Symbol.Write(writer, value);
        }
    }

    private static bool IsRootDirectory(DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        return entry.type.value == parent.type.value && entry.name.value == parent.name.value;
    }
}
