using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIColor"), Description("Just a color, used by UI components")]
public class UIColor : Object
{

    private ushort altRevision;
    private ushort revision;

    [Name("Color"), Description("color")]
    public HmxColor4 color = new();

    public new UIColor Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.objFields.Read(reader, parent, entry);

        color.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.objFields.Write(writer, parent);

        color.Write(writer);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
