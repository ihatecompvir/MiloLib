using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("BackgroundDir"), Description("Subclass of PanelDir, for a persistent shell background")]
public class BackgroundDir : PanelDir
{
    private ushort altRevision;
    private ushort revision;

    [HideInInspector, MaxVersion(0)]
    public Symbol oldBackground = Symbol.Empty;

    [Name("Isolate View"), Description("Hide views not directly reachable from the current view"), MinVersion(2)]
    public bool isolateView;

    public BackgroundDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BackgroundDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        base.Read(reader, false, parent, entry);

        if (revision < 1)
        {
            oldBackground = Symbol.Read(reader);
        }

        if (revision > 1)
        {
            isolateView = reader.ReadBoolean();
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
        base.Write(writer, false, parent, entry);

        if (revision < 1)
        {
            Symbol.Write(writer, oldBackground);
        }

        if (revision > 1)
        {
            writer.WriteBoolean(isolateView);
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
