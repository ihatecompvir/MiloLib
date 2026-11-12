using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("FileMerger"), Description("Merges files into ObjectDirs, much like a milo file merge.")]
public class FileMerger : Object
{
    public class Merger
    {
        [Name("Name"), Description("Name of the merger, just for identification")]
        public Symbol name = Symbol.Empty;

        [Name("Selected"), Description("The file you want to merge")]
        public Symbol selected = Symbol.Empty;

        [Name("Loaded"), Description("currently loaded file")]
        public Symbol loaded = Symbol.Empty;

        [Name("Dir"), Description("Dir to merge into, proxy, for instance")]
        public Symbol dir = Symbol.Empty;

        [Name("Proxy"), Description("If true, merges the Dir in as a proxy, rather than the individual objects"), MinVersion(1)]
        public bool proxy;

        [Name("Subdirs"), Description("How to treat subdirs in the source"), MinVersion(1)]
        public uint subdirs;

        [Name("Pre Clear"), Description("Delete the old objects right at StartLoad time"), MinVersion(3)]
        public bool preClear;

        public Merger Read(EndianReader reader, ushort fmRevision)
        {
            name = Symbol.Read(reader);
            selected = Symbol.Read(reader);
            loaded = Symbol.Read(reader);
            dir = Symbol.Read(reader);

            if (fmRevision != 0)
            {
                if (fmRevision != 4)
                {
                    proxy = reader.ReadBoolean();
                }

                subdirs = reader.ReadUInt32();
                if (fmRevision > 2)
                {
                    preClear = reader.ReadBoolean();
                }
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort fmRevision)
        {
            Symbol.Write(writer, name);
            Symbol.Write(writer, selected);
            Symbol.Write(writer, loaded);
            Symbol.Write(writer, dir);

            if (fmRevision != 0)
            {
                if (fmRevision != 4)
                {
                    writer.WriteBoolean(proxy);
                }

                writer.WriteUInt32(subdirs);
                if (fmRevision > 2)
                {
                    writer.WriteBoolean(preClear);
                }
            }
        }

        public override string ToString()
        {
            return $"{name} {selected} {loaded} {dir} {proxy} {subdirs} {preClear}";
        }
    }
    private ushort altRevision;
    private ushort revision;

    [MaxVersion(1)]
    public Symbol unkSym = Symbol.Empty;

    [Name("Mergers"), Description("Array of file mergers")]
    public List<Merger> files = new();

    public new FileMerger Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision < 2)
        {
            unkSym = Symbol.Read(reader);
        }

        files = AssetHelpers.ReadList(reader, () => new Merger().Read(reader, revision));

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

        if (revision < 2)
        {
            Symbol.Write(writer, unkSym);
        }

        AssetHelpers.WriteList(writer, files, file => file.Write(writer, revision));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
