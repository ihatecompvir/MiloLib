using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("RenderDir"), Description("Wraps a PanelDir, to allow it to be used in a rendered tex")]
public class RenderDir : PanelDir
{
    private ushort altRevision;
    private ushort revision;

    [Name("Target"), Description("PanelDir that we wrap")]
    public Symbol target = Symbol.Empty;

    public RenderDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new RenderDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        target = Symbol.Read(reader);

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

        Symbol.Write(writer, target);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
