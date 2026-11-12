using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band.UI;

[Name("BandSwatch"), Description("Special list for selecting colors")]
public class BandSwatch : UIList
{
    private ushort altRevision;
    private ushort revision;

    [Name("Color Palette"), Description("Palette containing our color options"), MinVersion(1)]
    public Symbol colorPalette = Symbol.Empty;

    public new BandSwatch Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 0)
        {
            colorPalette = Symbol.Read(reader);
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
            Symbol.Write(writer, colorPalette);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
