using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIPicture"), Description("A picture object with asynchronously loading texture")]
public class UIPicture : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Texture File"), Description("Path of texture to load"), MinVersion(1)]
    public Symbol texFile = Symbol.Empty;
    [Name("Mesh"), Description("Mesh to show loaded tex on (should have Mat)"), MinVersion(1)]
    public Symbol mesh = Symbol.Empty;


    [Name("In Animation"), Description("animation kicked off before texture change"), MinVersion(2)]
    public Symbol inAnim = Symbol.Empty;
    [Name("Out Animation"), Description("animation kicked off after texture change"), MinVersion(2)]
    public Symbol outAnim = Symbol.Empty;

    public new UIPicture Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 0)
        {
            texFile = Symbol.Read(reader);
            mesh = Symbol.Read(reader);
        }

        if (revision >= 2)
        {
            inAnim = Symbol.Read(reader);
            outAnim = Symbol.Read(reader);
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

        if (revision != 0)
        {
            Symbol.Write(writer, texFile);
            Symbol.Write(writer, mesh);
        }

        if (revision >= 2)
        {
            Symbol.Write(writer, inAnim);
            Symbol.Write(writer, outAnim);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
