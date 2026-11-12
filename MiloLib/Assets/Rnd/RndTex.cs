using MiloLib.Utils;
using MiloLib.Classes;

namespace MiloLib.Assets.Rnd;


// this is a misleading description from Harmonix lol, I guess I could add this though so it is no longer false
[Name("Tex"), Description("Tex objects represent bitmaps used by materials. These can be created automatically with 'import tex' on the file menu.")]
public class RndTex : Object
{
    public enum Type
    {
        kRegular = 1,
        kRendered = 2,
        kMovie = 4,
        kBackBuffer = 8,
        kFrontBuffer = 0x18,
        kRenderedNoZ = 0x22,
        kShadowMap = 0x42,
        kDepthVolumeMap = 0xA2,
        kDensityMap = 0x122,
        kScratch = 0x200,
        kDeviceTexture = 0x1000
    };

    private ushort altRevision;
    private ushort revision;
    private bool usesLegoLayout = true;
    private byte greenDayHeaderByte;
    private int oldMipMapK;
    private byte[] oldWiiAlphaData = Array.Empty<byte>();

    [Name("Width"), Description("Width of the texture in pixels.")]
    public uint width;
    [Name("Height"), Description("Height of the texture in pixels.")]
    public uint height;

    [Name("BPP"), Description("Bits per pixel.")]
    public uint bpp;

    [Name("External Path"), Description("Path to the texture to be loaded externally.")]
    public Symbol externalPath = Symbol.Empty;

    [MinVersion(8)]
    public float mipMapK;
    [MinVersion(6)]
    public Type type;

    [MinVersion(11)]
    public bool optimizeForPS3;

    [Name("Use External Path"), Description("Whether or not to use the external path.")]
    public int useExternalPath;

    [Name("Bitmap"), Description("The bitmap data.")]
    public RndBitmap bitmap = new();

    public bool hasBitmapData = true;

    public uint legoUnk;

    public ushort unkShort;

    public uint unkInt;
    public uint unkInt2;
    public ushort unkShort2;

    [MinVersion(5), MaxVersion(5)]
    public bool isRegular;

    private RndTex ReadScene25(EndianReader reader, bool standalone, DirectoryMeta parent)
    {
        long start = reader.BaseStream.Position;
        Endian endian = reader.Endianness;
        Exception legoError;
        usesLegoLayout = true;
        try
        {
            ReadScene25Body(reader, standalone, parent);
            return this;
        }
        catch (Exception ex) when (ex is InvalidDataException || ex is EndOfStreamException || ex is OverflowException)
        {
            legoError = ex;
            reader.BaseStream.Position = start;
            reader.Endianness = endian;
        }

        usesLegoLayout = false;
        try
        {
            ReadScene25Body(reader, standalone, parent);
            return this;
        }
        catch (Exception ex) when (ex is InvalidDataException || ex is EndOfStreamException || ex is OverflowException)
        {
            reader.BaseStream.Position = start;
            reader.Endianness = endian;
            throw new InvalidDataException("Tex revision 11 matches neither the LEGO nor the Green Day layout.", new AggregateException(legoError, ex));
        }
    }

    private void ReadScene25Body(EndianReader reader, bool standalone, DirectoryMeta parent)
    {
        // These layouts collide at scene revision 25 and Tex revision 11.
        legoUnk = 0;
        greenDayHeaderByte = 0;
        optimizeForPS3 = false;
        bitmap = new RndBitmap();
        if (!usesLegoLayout)
        {
            // Preserve the byte before the dimensions in the Green Day files.
            greenDayHeaderByte = reader.ReadByte();
        }
        width = reader.ReadUInt32();
        height = reader.ReadUInt32();
        bpp = reader.ReadUInt32();
        if (width > ushort.MaxValue || height > ushort.MaxValue || !IsTextureBpp(bpp))
        {
            throw new InvalidDataException("Invalid texture dimensions or bits per pixel.");
        }
        if (usesLegoLayout)
        {
            legoUnk = reader.ReadUInt32();
        }
        externalPath = Symbol.Read(reader);
        mipMapK = reader.ReadFloat();
        type = (Type)reader.ReadUInt32();
        useExternalPath = reader.ReadByte();
        hasBitmapData = !AssetHelpers.IsEndBytes(reader);
        if (hasBitmapData)
        {
            ReadLegacyBitmap(reader, parent);
        }
        if (!AssetHelpers.IsEndBytes(reader))
        {
            throw new InvalidDataException("The texture layout did not reach its end marker.");
        }
        if (standalone)
        {
            reader.BaseStream.Position += 4;
        }
    }

    private static bool IsTextureBpp(uint value)
    {
        return value == 4 || value == 8 || value == 16 || value == 24 || value == 32;
    }

    private void ReadLegacyBitmap(EndianReader reader, DirectoryMeta parent)
    {
        oldWiiAlphaData = Array.Empty<byte>();
        // LEGO and RB2 Wii bitmap revision 2 use the legacy 32-byte header without a CRC.
        // Legacy bitmap headers use the scene endianness, including on Wii.
        bitmap.platform = parent.platform;
        bitmap.revision = reader.ReadByte();
        bitmap.bpp = reader.ReadByte();
        if (bitmap.revision < 1 || bitmap.revision > 2 || !IsTextureBpp(bitmap.bpp))
        {
            throw new InvalidDataException("Invalid legacy texture bitmap header.");
        }
        bitmap.encoding = (RndBitmap.TextureEncoding)reader.ReadUInt32();
        bitmap.mipMaps = reader.ReadByte();
        bitmap.width = reader.ReadUInt16();
        bitmap.height = reader.ReadUInt16();
        bitmap.bpl = reader.ReadUInt16();
        bitmap.wiiAlphaNum = reader.ReadUInt16();
        bitmap.headerPadding = reader.ReadBlock(17);

        int paletteSize = 0;
        if (bitmap.bpp < 9 && ((uint)bitmap.encoding & 0xB8) == 0)
        {
            paletteSize = 4 << bitmap.bpp;
        }
        int dataSize = checked(bitmap.bpl * bitmap.height);
        int alphaSize = 0;
        if (parent.platform == DirectoryMeta.Platform.Wii && ((uint)bitmap.encoding & ~0x200u) == (uint)RndBitmap.TextureEncoding.TPL_CMP && bitmap.wiiAlphaNum == 4)
        {
            // These Wii bitmaps store a separate alpha image after the color mip chain.
            // LEGO also uses the 0x200 encoding flag with the same alpha-image layout.
            alphaSize = dataSize;
        }
        long payloadSize = (long)paletteSize + alphaSize;
        int mipSize = dataSize;
        for (int i = 0; i <= bitmap.mipMaps; i++)
        {
            payloadSize += mipSize;
            mipSize >>= 2;
        }
        if (payloadSize > reader.BaseStream.Length - reader.BaseStream.Position - 4)
        {
            throw new InvalidDataException("The texture bitmap payload exceeds the remaining stream.");
        }

        // Check the calculated boundary before allocating pixel data. Do not scan for a marker inside pixels.
        long payloadStart = reader.BaseStream.Position;
        reader.BaseStream.Position += payloadSize;
        bool validEnd = AssetHelpers.IsEndBytes(reader);
        reader.BaseStream.Position = payloadStart;
        if (!validEnd)
        {
            throw new InvalidDataException("The texture bitmap size does not reach its end marker.");
        }
        bitmap.colorPalette = reader.ReadBlock(paletteSize);
        for (int i = 0; i <= bitmap.mipMaps; i++)
        {
            bitmap.textures.Add(reader.ReadBlock(dataSize));
            dataSize >>= 2;
        }
        oldWiiAlphaData = reader.ReadBlock(alphaSize);
    }

    private void WriteScene25(EndianWriter writer, bool standalone)
    {
        if (!usesLegoLayout)
        {
            writer.WriteByte(greenDayHeaderByte);
        }
        writer.WriteUInt32(width);
        writer.WriteUInt32(height);
        writer.WriteUInt32(bpp);
        if (usesLegoLayout)
        {
            writer.WriteUInt32(legoUnk);
        }
        Symbol.Write(writer, externalPath);
        writer.WriteFloat(mipMapK);
        writer.WriteUInt32((uint)type);
        writer.WriteByte((byte)useExternalPath);
        if (hasBitmapData)
        {
            WriteLegacyBitmap(writer);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private void WriteLegacyBitmap(EndianWriter writer)
    {
        writer.WriteByte(bitmap.revision);
        writer.WriteByte(bitmap.bpp);
        writer.WriteUInt32((uint)bitmap.encoding);
        writer.WriteByte(bitmap.mipMaps);
        writer.WriteUInt16(bitmap.width);
        writer.WriteUInt16(bitmap.height);
        writer.WriteUInt16(bitmap.bpl);
        writer.WriteUInt16(bitmap.wiiAlphaNum);
        if (bitmap.headerPadding.Length == 17)
        {
            writer.WriteBlock(bitmap.headerPadding);
        }
        else
        {
            writer.WriteBlock(new byte[17]);
        }
        writer.WriteBlock(bitmap.colorPalette);
        foreach (byte[] texture in bitmap.textures)
        {
            writer.WriteBlock(texture);
        }
        writer.WriteBlock(oldWiiAlphaData);
    }


    public new RndTex Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 8)
        {
            base.Read(reader, false, parent, entry);
        }

        if (parent.revision == 25 && revision == 11)
        {
            return ReadScene25(reader, standalone, parent);
        }

        width = reader.ReadUInt32();
        height = reader.ReadUInt32();

        bpp = reader.ReadUInt32();

        externalPath = Symbol.Read(reader);

        if (revision >= 8)
        {
            mipMapK = reader.ReadFloat();
        }
        else if (revision == 5)
        {
            oldMipMapK = reader.ReadInt32();
            mipMapK = oldMipMapK / 16.0f;
        }

        if (revision > 6)
        {
            type = (Type)reader.ReadUInt32();
        }
        else if (revision > 5)
        {
            type = (Type)reader.ReadUInt32();
        }
        else if (revision > 4)
        {
            isRegular = reader.ReadBoolean();
        }

        if (revision >= 11 && parent.revision != 25)
        {
            optimizeForPS3 = reader.ReadBoolean();
        }

        if (revision == 5)
        {
            useExternalPath = 0;
        }
        else if (revision != 7)
        {
            useExternalPath = reader.ReadByte();
        }
        else
        {
            useExternalPath = reader.ReadInt32();
        }

        if (revision == 5)
        {
            if (standalone)
            {
                uint expected = 0xDEADDEAD;
                if (reader.Endianness == Endian.BigEndian)
                {
                    expected = 0xADDEADDE;
                }
                if (expected != reader.ReadUInt32())
                {
                    throw MiloLib.Exceptions.MiloAssetReadException.EndBytesNotFound(parent, entry, reader.BaseStream.Position, revision);
                }
            }
            hasBitmapData = false;
            return this;
        }

        hasBitmapData = !AssetHelpers.IsEndBytes(reader);
        if (!hasBitmapData)
        {
            if (standalone)
            {
                reader.BaseStream.Position += 4;
            }
            return this;
        }


        if (parent.platform == DirectoryMeta.Platform.Wii && revision > 10)
        {
            unkShort = reader.ReadUInt16();
        }


        // bitmaps are stored as Little endian on Wii? wack
        Endian origEndian = reader.Endianness;

        if (parent.platform == DirectoryMeta.Platform.Wii && revision > 10)
        {
            reader.Endianness = Endian.LittleEndian;
        }

        if (parent.revision == 25 && revision == 10 && parent.platform == DirectoryMeta.Platform.Wii)
        {
            bitmap = new RndBitmap();
            ReadLegacyBitmap(reader, parent);
        }
        else
        {
            bitmap = new RndBitmap().Read(reader, false, parent, entry);
        }

        reader.Endianness = origEndian;

        if (parent.platform == DirectoryMeta.Platform.Wii && revision > 10)
        {
            unkInt = reader.ReadUInt32();
            unkInt2 = reader.ReadUInt32();
            unkShort2 = reader.ReadUInt16();
        }

        if (standalone)
        {
            uint expected = 0xDEADDEAD;
            if (reader.Endianness == Endian.BigEndian)
            {
                expected = 0xADDEADDE;
            }
            if (expected != reader.ReadUInt32())
            {
                throw MiloLib.Exceptions.MiloAssetReadException.EndBytesNotFound(parent, entry, reader.BaseStream.Position, revision);
            }
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision > 8)
        {
            base.Write(writer, false, parent, entry);
        }

        if (parent.revision == 25 && revision == 11)
        {
            WriteScene25(writer, standalone);
            return;
        }

        writer.WriteUInt32(width);
        writer.WriteUInt32(height);

        writer.WriteUInt32(bpp);

        Symbol.Write(writer, externalPath);

        if (revision >= 8)
        {
            writer.WriteFloat(mipMapK);
        }
        else if (revision == 5)
        {
            int value = oldMipMapK;
            if (mipMapK != oldMipMapK / 16.0f)
            {
                value = checked((int)(mipMapK * 16.0f));
            }
            writer.WriteInt32(value);
        }
        if (revision > 6)
        {
            writer.WriteUInt32((uint)type);
        }
        else if (revision > 5)
        {
            writer.WriteUInt32((uint)type);
        }
        else if (revision > 4)
        {
            writer.WriteBoolean(isRegular);
        }

        if (revision >= 11 && parent.revision != 25)
        {
            writer.WriteBoolean(optimizeForPS3);
        }

        if (revision != 5)
        {
            if (revision != 7)
            {
                writer.WriteByte((byte)useExternalPath);
            }
            else
            {
                writer.WriteInt32(useExternalPath);
            }
        }

        if (revision == 5)
        {
            if (standalone)
            {
                writer.WriteEndBytes();
            }

            return;
        }

        if (!hasBitmapData)
        {
            if (standalone)
            {
                writer.WriteEndBytes();
            }
            return;
        }

        if (parent.platform == DirectoryMeta.Platform.Wii && revision > 10)
        {
            writer.WriteUInt16(unkShort);
        }

        Endian origEndian = writer.Endianness;

        if (parent.platform == DirectoryMeta.Platform.Wii && revision > 10)
        {
            writer.Endianness = Endian.LittleEndian;
        }

        if (parent.revision == 25 && revision == 10 && parent.platform == DirectoryMeta.Platform.Wii)
        {
            WriteLegacyBitmap(writer);
        }
        else
        {
            bitmap.Write(writer, false, parent, entry);
        }

        writer.Endianness = origEndian;

        if (parent.platform == DirectoryMeta.Platform.Wii && revision > 10)
        {
            writer.WriteUInt32(unkInt);
            writer.WriteUInt32(unkInt2);
            writer.WriteUInt16(unkShort2);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }

    }

    public static RndTex New(ushort revision, ushort altRevision)
    {
        RndTex newRndTex = new();
        newRndTex.revision = revision;
        newRndTex.altRevision = altRevision;
        return newRndTex;
    }

}
