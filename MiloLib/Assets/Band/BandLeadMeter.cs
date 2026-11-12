using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandLeadMeter"), Description("Multiplayer lead meter hud element, shows which player is in the lead")]
public class BandLeadMeter : RndDir
{
    private ushort altRevision;
    private ushort revision;
    public int mode;
    public List<Symbol> objects = new();

    public BandLeadMeter(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandLeadMeter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);

        bool isRootDirectory = IsRootDirectory(parent, entry);

        if (revision >= 8)
        {
            if (!entry.isProxy)
            {
                mode = reader.ReadInt32();
            }
            if (isRootDirectory)
            {
                ReadObjects(reader, 10);
            }
        }

        base.Read(reader, false, parent, entry);

        if (revision < 8)
        {
            if (!entry.isProxy)
            {
                mode = reader.ReadInt32();
            }
            if (isRootDirectory)
            {
                ReadObjects(reader, 6);
                if (revision >= 7)
                {
                    ReadObjects(reader, 4);
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
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);

        bool isRootDirectory = false;
        if (entry != null)
        {
            isRootDirectory = IsRootDirectory(parent, entry);
        }

        if (entry != null && revision >= 8)
        {
            if (!entry.isProxy)
            {
                writer.WriteInt32(mode);
            }
            if (isRootDirectory)
            {
                WriteObjects(writer, 0, 10);
            }
        }

        base.Write(writer, false, parent, entry);

        if (entry != null && revision < 8)
        {
            if (!entry.isProxy)
            {
                writer.WriteInt32(mode);
            }
            if (isRootDirectory)
            {
                WriteObjects(writer, 0, 6);
                if (revision >= 7)
                {
                    WriteObjects(writer, 6, 4);
                }
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
