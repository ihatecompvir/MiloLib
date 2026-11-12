using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Font"), Description("Font objects determine the appearance for Text objects.")]
public partial class RndFont : Object
{
    public class CharInfo
    {
        public int page;
        public float texU;
        public float texV;
        public float charWidth;
        public float charAdvance;

        public CharInfo Read(EndianReader reader, ushort revision)
        {
            texU = reader.ReadFloat();
            texV = reader.ReadFloat();
            charWidth = reader.ReadFloat();
            if (revision > 0xE)
            {
                charAdvance = reader.ReadFloat();
            }
            else
            {
                charAdvance = charWidth;
            }

            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            writer.WriteFloat(texU);
            writer.WriteFloat(texV);
            writer.WriteFloat(charWidth);
            if (revision > 0xE)
            {
                writer.WriteFloat(charAdvance);
            }
        }
    }

    public class KernInfo
    {
        public ushort firstChar;
        public ushort secondChar;
        public byte unkByte0;
        public byte unkByte1;
        public float kerning;

        public KernInfo Read(EndianReader reader, ushort fontRevision)
        {
            if (fontRevision < 17)
            {
                firstChar = reader.ReadByte();
                secondChar = reader.ReadByte();
            }
            else
            {
                firstChar = reader.ReadUInt16();
                secondChar = reader.ReadUInt16();
            }
            if (fontRevision < 6)
            {
                unkByte0 = reader.ReadByte();
                unkByte1 = reader.ReadByte();
            }
            kerning = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort fontRevision)
        {
            if (fontRevision < 17)
            {
                writer.WriteByte((byte)firstChar);
                writer.WriteByte((byte)secondChar);
            }
            else
            {
                writer.WriteUInt16(firstChar);
                writer.WriteUInt16(secondChar);
            }
            if (fontRevision < 6)
            {
                writer.WriteByte(unkByte0);
                writer.WriteByte(unkByte1);
            }
            writer.WriteFloat(kerning);
        }
    }

    public class KerningEntry
    {
        public int key;
        public float kerning;

        public KerningEntry Read(EndianReader reader)
        {
            key = reader.ReadInt32();
            kerning = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32(key);
            writer.WriteFloat(kerning);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Material"), Description("Material for font"), MinVersion(1)]
    public Symbol mat = Symbol.Empty;

    [MinVersion(10), MaxVersion(11)]
    public Symbol matName = Symbol.Empty;

    [MinVersion(4)]
    public Vector2 cellSize = new(1, 1);

    [MinVersion(1)]
    public float deprecatedSize;

    [Name("Base Kerning"), Description("Base kerning of the font."), MinVersion(1)]
    public float baseKerning;

    [Name("Characters In Map"), Description("List of characters this font uses"), MinVersion(17)]
    public List<ushort> chars = new();

    [Name("Characters In Map"), Description("List of characters this font uses"), MinVersion(2), MaxVersion(16)]
    public Symbol charsAsString = Symbol.Empty;

    [MinVersion(5)]
    public bool hasKerningTable;

    [MinVersion(5), MaxVersion(6)]
    public List<KernInfo> kerningInfos = new();

    [MinVersion(7)]
    public List<KerningEntry> kerningEntries = new();

    [Name("Texture Owner"), Description("Font owner of the texture to use"), MinVersion(9)]
    public Symbol textureOwner = Symbol.Empty;

    [Name("Monospace"), Description("Font is monospaced."), MinVersion(11)]
    public bool monospace;

    [Name("Packed"), Description("Font texture is packed"), MinVersion(15)]
    public bool packed;

    [MinVersion(13)]
    public int bitmapWidth;

    [MinVersion(13)]
    public int bitmapHeight;

    [MinVersion(14)]
    public Vector2 texCellSize = new();

    [MinVersion(17)]
    public List<KeyValuePair<ushort, CharInfo>> charInfoMap = new();

    [MinVersion(17)]
    public Symbol nextFont = Symbol.Empty;

    // old stuff for early revisions, TODO: figure out what these actually do so we can properly name them
    [MaxVersion(2)]
    public int oldA;
    [MaxVersion(2)]
    public int oldB;
    [MaxVersion(2)]
    public int oldC;
    [MaxVersion(2)]
    public int oldD;
    [MaxVersion(2)]
    public bool oldE;
    [MaxVersion(2)]
    public Symbol oldStr = Symbol.Empty;
    [MinVersion(1), MaxVersion(1)]
    public int oldCellWidthInt;
    [MinVersion(1), MaxVersion(1)]
    public int oldCellHeightInt;
    [MinVersion(2), MaxVersion(3)]
    public float oldCellWidthFloat;
    [MinVersion(2), MaxVersion(3)]
    public float oldCellHeightFloat;
    [MaxVersion(0)]
    public List<(byte key, Symbol name, float width, float height)> oldMatChars = new();

    [MinVersion(14), MaxVersion(16)]
    public CharInfo[] charInfoArray;

    public new RndFont Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (altRevision >= 2)
        {
            ReadFontBase(reader, parent, entry);
        }
        else if (revision > 7)
        {
            base.Read(reader, false, parent, entry);
        }

        if (revision < 3)
        {
            oldA = reader.ReadInt32();
            oldB = reader.ReadInt32();
            oldC = reader.ReadInt32();
            oldE = reader.ReadBoolean();
            oldD = reader.ReadInt32();
            oldStr = Symbol.Read(reader);
        }

        if (revision < 1)
        {
            oldMatChars = AssetHelpers.ReadList(reader, () =>
            {
                byte key = reader.ReadByte();
                Symbol name = Symbol.Read(reader);
                float width = reader.ReadFloat();
                float height = reader.ReadFloat();
                return (key, name, width, height);
            });
        }
        else
        {
            if (altRevision < 1)
            {
                mat = Symbol.Read(reader);
                if (revision == 10 || revision == 11)
                {
                    matName = Symbol.Read(reader);
                }
            }
            else
            {
                materials = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            if (revision < 4)
            {
                if (revision < 2)
                {
                    oldCellWidthInt = reader.ReadInt32();
                    oldCellHeightInt = reader.ReadInt32();
                }
                else
                {
                    oldCellWidthFloat = reader.ReadFloat();
                    oldCellHeightFloat = reader.ReadFloat();
                }
            }
            else
            {
                cellSize = cellSize.Read(reader);
            }
            deprecatedSize = reader.ReadFloat();
            if (altRevision < 2)
            {
                baseKerning = reader.ReadFloat();
            }
        }

        if (revision > 1)
        {
            if (revision <= 0x10)
            {
                charsAsString = Symbol.Read(reader);
            }
            else if (altRevision < 2)
            {
                chars = AssetHelpers.ReadList(reader, () => reader.ReadUInt16());
            }
        }

        if (revision > 4 && altRevision < 2)
        {
            hasKerningTable = reader.ReadBoolean();
            if (hasKerningTable)
            {
                if (revision < 7)
                {
                    kerningInfos = AssetHelpers.ReadList(reader, () => new KernInfo().Read(reader, revision));
                }
                else
                {
                    kerningEntries = AssetHelpers.ReadList(reader, () => new KerningEntry().Read(reader));
                }
            }
        }

        if (revision > 8)
        {
            textureOwner = Symbol.Read(reader);
        }

        if (revision > 10 && altRevision < 2)
        {
            monospace = reader.ReadBoolean();
        }

        if (revision > 0xE)
        {
            packed = reader.ReadBoolean();
        }

        if (revision > 0xC)
        {
            bitmapWidth = reader.ReadInt32();
            bitmapHeight = reader.ReadInt32();
        }

        if (revision > 0xD)
        {
            if (altRevision < 1)
            {
                texCellSize = texCellSize.Read(reader);
            }
            else
            {
                textureCellSizes = AssetHelpers.ReadList(reader, () => new Vector2().Read(reader));
            }
            if (revision < 0x11)
            {
                charInfoArray = new CharInfo[256];
                for (int i = 0; i < 256; i++)
                {
                    charInfoArray[i] = new CharInfo().Read(reader, revision);
                }
            }
            else
            {
                charInfoMap = AssetHelpers.ReadList(reader, () =>
                {
                    ushort key = reader.ReadUInt16();
                    CharInfo info = new();
                    if (altRevision > 0)
                    {
                        info.page = reader.ReadInt32();
                    }
                    info.texU = reader.ReadFloat();
                    info.texV = reader.ReadFloat();
                    info.charWidth = reader.ReadFloat();
                    info.charAdvance = reader.ReadFloat();
                    return new KeyValuePair<ushort, CharInfo>(key, info);
                });
            }
        }

        if (revision > 0x10 && altRevision < 1)
        {
            nextFont = Symbol.Read(reader);
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

        if (altRevision >= 2)
        {
            WriteFontBase(writer, parent, entry);
        }
        else if (revision > 7)
        {
            base.Write(writer, false, parent, entry);
        }

        if (revision < 3)
        {
            writer.WriteInt32(oldA);
            writer.WriteInt32(oldB);
            writer.WriteInt32(oldC);
            writer.WriteBoolean(oldE);
            writer.WriteInt32(oldD);
            Symbol.Write(writer, oldStr);
        }

        if (revision < 1)
        {
            AssetHelpers.WriteList(writer, oldMatChars, mc =>
            {
                writer.WriteByte(mc.key);
                Symbol.Write(writer, mc.name);
                writer.WriteFloat(mc.width);
                writer.WriteFloat(mc.height);
            });
        }
        else
        {
            if (altRevision < 1)
            {
                Symbol.Write(writer, mat);
                if (revision == 10 || revision == 11)
                {
                    Symbol.Write(writer, matName);
                }
            }
            else
            {
                AssetHelpers.WriteList(writer, materials, material => Symbol.Write(writer, material));
            }
            if (revision < 4)
            {
                if (revision < 2)
                {
                    writer.WriteInt32(oldCellWidthInt);
                    writer.WriteInt32(oldCellHeightInt);
                }
                else
                {
                    writer.WriteFloat(oldCellWidthFloat);
                    writer.WriteFloat(oldCellHeightFloat);
                }
            }
            else
            {
                cellSize.Write(writer);
            }
            writer.WriteFloat(deprecatedSize);
            if (altRevision < 2)
            {
                writer.WriteFloat(baseKerning);
            }
        }

        if (revision > 1)
        {
            if (revision <= 0x10)
            {
                Symbol.Write(writer, charsAsString);
            }
            else if (altRevision < 2)
            {
                AssetHelpers.WriteList(writer, chars, (ushort c) => writer.WriteUInt16(c));
            }
        }

        if (revision > 4 && altRevision < 2)
        {
            writer.WriteBoolean(hasKerningTable);
            if (hasKerningTable)
            {
                if (revision < 7)
                {
                    AssetHelpers.WriteList(writer, kerningInfos, ki => ki.Write(writer, revision));
                }
                else
                {
                    AssetHelpers.WriteList(writer, kerningEntries, ke => ke.Write(writer));
                }
            }
        }

        if (revision > 8)
        {
            Symbol.Write(writer, textureOwner);
        }

        if (revision > 10 && altRevision < 2)
        {
            writer.WriteBoolean(monospace);
        }

        if (revision > 0xE)
        {
            writer.WriteBoolean(packed);
        }

        if (revision > 0xC)
        {
            writer.WriteInt32(bitmapWidth);
            writer.WriteInt32(bitmapHeight);
        }

        if (revision > 0xD)
        {
            if (altRevision < 1)
            {
                texCellSize.Write(writer);
            }
            else
            {
                AssetHelpers.WriteList(writer, textureCellSizes, cell => cell.Write(writer));
            }
            if (revision < 0x11)
            {
                if (charInfoArray == null)
                {
                    charInfoArray = new CharInfo[256];
                }

                for (int i = 0; i < 256; i++)
                {
                    if (charInfoArray[i] == null)
                    {
                        charInfoArray[i] = new CharInfo();
                    }

                    charInfoArray[i].Write(writer, revision);
                }
            }
            else
            {
                AssetHelpers.WriteList(writer, charInfoMap, kvp =>
                {
                    writer.WriteUInt16(kvp.Key);
                    if (altRevision > 0)
                    {
                        writer.WriteInt32(kvp.Value.page);
                    }
                    writer.WriteFloat(kvp.Value.texU);
                    writer.WriteFloat(kvp.Value.texV);
                    writer.WriteFloat(kvp.Value.charWidth);
                    writer.WriteFloat(kvp.Value.charAdvance);
                });
            }
        }

        if (revision > 0x10 && altRevision < 1)
        {
            Symbol.Write(writer, nextFont);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public static RndFont New(ushort revision, ushort altRevision = 0)
    {
        RndFont font = new();
        font.revision = revision;
        font.altRevision = altRevision;
        return font;
    }
}
