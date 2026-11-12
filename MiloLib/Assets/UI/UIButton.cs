using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIButton"), Description("Simple button, basically just a label that can be selected")]
public class UIButton : UILabel
{
    private ushort altRevision;
    private ushort revision;

    public ushort UIButtonRevision => revision;
    public ushort UIButtonAltRevision => altRevision;

    public void SetUIButtonRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new UIButton Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
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

}
