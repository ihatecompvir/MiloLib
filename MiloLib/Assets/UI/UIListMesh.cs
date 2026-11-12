using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIListMesh"), Description("Custom slot for use with UIList")]
public class UIListMesh : UIListSlot
{
    private ushort altRevision;
    private ushort revision;

    [Name("Mesh"), Description("mesh to draw/transform")]
    public Symbol mesh = Symbol.Empty;
    [Name("Default Material"), Description("default material")]
    public Symbol defaultMat = Symbol.Empty;

    public new UIListMesh Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        mesh = Symbol.Read(reader);
        defaultMat = Symbol.Read(reader);

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
        Symbol.Write(writer, defaultMat);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
