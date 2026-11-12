using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIListCustom"), Description("Custom slot for use with UIList")]
public class UIListCustom : UIListSlot
{
    private ushort altRevision;
    private ushort revision;

    [Name("Object"), Description("custom object to draw/transform")]
    public Symbol listObject = Symbol.Empty;

    public new UIListCustom Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        listObject = Symbol.Read(reader);

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

        Symbol.Write(writer, listObject);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
