using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("LoadingPanelDir")]
public class LoadingPanelDir : WorldDir
{
    private ushort altRevision;
    private ushort revision;

    [Name("Clip File Path")]
    public Symbol clipFilePath = Symbol.Empty;

    [Name("H2H Char Type"), MinVersion(2)]
    public Symbol h2hCharType = Symbol.Empty;

    public LoadingPanelDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new LoadingPanelDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (!entry.isProxy)
        {
            clipFilePath = Symbol.Read(reader);

            if (revision > 1)
            {
                h2hCharType = Symbol.Read(reader);
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

        if (entry != null && !entry.isProxy)
        {
            Symbol.Write(writer, clipFilePath);

            if (revision > 1)
            {
                Symbol.Write(writer, h2hCharType);
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
}
