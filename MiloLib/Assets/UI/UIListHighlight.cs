using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIListHighlight"), Description("Highlight widget for use with UIList")]
public class UIListHighlight : UIListWidget
{
    private ushort altRevision;
    private ushort revision;

    [Name("Mesh"), Description("arrow mesh to draw/transform")]
    public Symbol mesh = Symbol.Empty;

    public new UIListHighlight Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        mesh = Symbol.Read(reader);

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

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
