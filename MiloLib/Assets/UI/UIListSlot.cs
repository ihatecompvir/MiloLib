using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIListSlot"), Description("Base functionality for UIList slots")]
public class UIListSlot : UIListWidget
{
    public enum UIListSlotDrawType
    {
        kUIListSlotDrawAlways,
        kUIListSlotDrawHighlight,
        kUIListSlotDrawNoHighlight
    };

    private ushort altRevision;
    private ushort revision;

    public UIListSlotDrawType slotDrawType;

    public new UIListSlot Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        slotDrawType = (UIListSlotDrawType)reader.ReadInt32();

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

        writer.WriteInt32((int)slotDrawType);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
