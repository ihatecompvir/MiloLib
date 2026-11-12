using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

public partial class RndFont
{
    private ushort fontBaseRevision;
    private ushort fontBaseAltRevision;
    [Name("Mats"), Description("Multiple materials for multi-page fonts")]
    public List<Symbol> materials = new();
    public List<Vector2> textureCellSizes = new();

    protected void ReadFontBase(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out fontBaseRevision, out fontBaseAltRevision, 0);
        base.Read(reader, false, parent, entry);
        chars = AssetHelpers.ReadList(reader, () => reader.ReadUInt16());
        monospace = reader.ReadBoolean();
        baseKerning = reader.ReadFloat();
        hasKerningTable = reader.ReadBoolean();
        if (hasKerningTable)
        {
            kerningEntries = AssetHelpers.ReadList(reader, () => new KerningEntry().Read(reader));
        }
    }

    protected void WriteFontBase(EndianWriter writer, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, fontBaseRevision, fontBaseAltRevision, 0);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, chars, character => writer.WriteUInt16(character));
        writer.WriteBoolean(monospace);
        writer.WriteFloat(baseKerning);
        writer.WriteBoolean(hasKerningTable);
        if (hasKerningTable)
        {
            AssetHelpers.WriteList(writer, kerningEntries, kerning => kerning.Write(writer));
        }
    }
}
