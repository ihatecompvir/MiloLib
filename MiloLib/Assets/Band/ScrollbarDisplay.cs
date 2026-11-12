using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("ScrollbarDisplay"), Description("Scrollbar Display (used for showing a scrollbar associated with a list)")]
public class ScrollbarDisplay : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Scrollbar List"), Description("list used to control scrollbar")]
    public Symbol list = Symbol.Empty;
    [Name("Always Show"), Description("whether or not to show when there is no scrolling")]
    public bool alwaysShow;
    [MaxVersion(0)]
    public bool unkBool;
    [Name("List X Offset"), Description("X Offset from list (ignored if list not attached)")]
    public float listXOffset;
    [Name("List Y Offset"), Description("Y Offset from list (ignored if list not attached)")]
    public float listYOffset;
    [Name("Scrollbar Height"), Description("Height of scrollbar (ignored if list attached)")]
    public float scrollbarHeight;
    [Name("Min Thumb Height"), Description("Minimum height for thumb"), MinVersion(2)]
    public float minThumbHeight;

    public new ScrollbarDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        list = Symbol.Read(reader);
        alwaysShow = reader.ReadBoolean();
        if (revision < 1)
        {
            unkBool = reader.ReadBoolean();
        }

        listXOffset = reader.ReadFloat();
        listYOffset = reader.ReadFloat();
        scrollbarHeight = reader.ReadFloat();
        if (revision >= 2)
        {
            minThumbHeight = reader.ReadFloat();
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
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        Symbol.Write(writer, list);
        writer.WriteBoolean(alwaysShow);
        if (revision < 1)
        {
            writer.WriteBoolean(unkBool);
        }

        writer.WriteFloat(listXOffset);
        writer.WriteFloat(listYOffset);
        writer.WriteFloat(scrollbarHeight);
        if (revision >= 2)
        {
            writer.WriteFloat(minThumbHeight);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
