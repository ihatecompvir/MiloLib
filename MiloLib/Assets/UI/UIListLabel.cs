using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIListLabel"), Description("Custom slot for use with UIList")]
public class UIListLabel : UIListSlot
{
    private ushort altRevision;
    private ushort revision;

    [Name("Label"), Description("label to draw/transform")]
    public Symbol label = Symbol.Empty;

    [MaxVersion(0)]
    public Symbol unkSym = Symbol.Empty;

    [Name("Highlight Alt Styles"), Description("alternate label styles are affected by highlight color")]
    public bool highlightAltStyles;

    public new UIListLabel Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        label = Symbol.Read(reader);

        if (revision == 0)
        {
            unkSym = Symbol.Read(reader);
        }

        if (altRevision >= 1)
        {
            highlightAltStyles = reader.ReadBoolean();
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

        Symbol.Write(writer, label);

        if (revision == 0)
        {
            Symbol.Write(writer, unkSym);
        }

        if (altRevision >= 1)
        {
            writer.WriteBoolean(highlightAltStyles);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
