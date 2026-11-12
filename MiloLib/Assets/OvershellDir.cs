using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("OvershellDir"), Description("panel dir that handles the overshell")]
public class OvershellDir : PanelDir
{
    private ushort altRevision;
    private ushort revision;

    public OvershellDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new OvershellDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

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

}
