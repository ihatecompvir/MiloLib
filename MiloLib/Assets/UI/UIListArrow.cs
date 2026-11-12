using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIListArrow"), Description("Arrow widget for use with UIList")]
public class UIListArrow : UIListWidget
{
    public enum UIListArrowPosition
    {
        kUIListArrowBack,
        kUIListArrowNext
    };

    private ushort altRevision;
    private ushort revision;

    [Name("Mesh"), Description("arrow mesh to draw/transform")]
    public Symbol mesh = Symbol.Empty;
    [Name("Scroll Anim"), Description("animation to play on scroll"), MinVersion(1)]
    public Symbol scrollAnim = Symbol.Empty;

    [Name("Position"), Description("whether to position relative to first or last element")]
    public UIListArrowPosition position;

    [Name("Show Only When Scrollable"), Description("show only when list is scrollable")]
    public bool showOnlyScroll;
    [Name("Relative to Highlight"), Description("position arrow relative to higlight")]
    public bool onHighlight;



    public new UIListArrow Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        mesh = Symbol.Read(reader);
        position = (UIListArrowPosition)reader.ReadInt32();
        showOnlyScroll = reader.ReadBoolean();
        onHighlight = reader.ReadBoolean();

        if (revision != 0)
        {
            scrollAnim = Symbol.Read(reader);
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

        Symbol.Write(writer, mesh);
        writer.WriteInt32((int)position);
        writer.WriteBoolean(showOnlyScroll);
        writer.WriteBoolean(onHighlight);

        if (revision != 0)
        {
            Symbol.Write(writer, scrollAnim);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
