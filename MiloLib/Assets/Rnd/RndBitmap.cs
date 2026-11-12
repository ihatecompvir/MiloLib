using MiloLib.Utils;
using System.Text;
using MiloLib.Classes;

namespace MiloLib.Assets.Rnd;

[Name("Bitmap"), Description("Represents a bitmap.")]
public class RndBitmap
{
    private static readonly byte[] ZeroPad44 = new byte[44];

    public enum TextureEncoding
    {
        ARGB = 1,
        RGBA = 3,
        DXT1_BC1 = 8,
        DXT5_BC3 = 24,
        ATI2_BC5 = 32,
        TPL_CMP = 72,
        TPL_CMP_ALPHA = 328,
        TPL_CMP_2 = 583
    }
    public byte revision;

    [MinVersion(2)]
    public uint crc;

    public byte[] headerPadding = Array.Empty<byte>();

    [MinVersion(2), MaxVersion(2)]
    public byte? uknownData;

    public byte bpp;

    public TextureEncoding encoding;

    public byte mipMaps;

    public ushort width;
    public ushort height;

    public ushort bpl;

    public ushort wiiAlphaNum;

    public List<byte[]> textures = new();

    public DirectoryMeta.Platform platform;

    public byte[] colorPalette = Array.Empty<byte>();

    private int GetColorPaletteSize()
    {
        if (bpp >= 9)
        {
            return 0;
        }
        if (platform == DirectoryMeta.Platform.GameCube && (uint)encoding == 0x10)
        {
            return 2 << bpp;
        }

        uint encodingFlags = (uint)encoding;
        if ((encodingFlags & 0xB8) == 0)
        {
            return 4 << bpp;
        }
        return 0;
    }


    public RndBitmap Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry, bool legacyHeader = false)
    {
        platform = parent.platform;

        revision = reader.ReadByte();

        if (revision > 1 && !legacyHeader)
        {
            crc = reader.ReadUInt32();
        }

        bpp = reader.ReadByte();

        if (revision == 0)
        {
            encoding = (TextureEncoding)reader.ReadByte();
        }
        else
        {
            encoding = (TextureEncoding)reader.ReadUInt32();
        }

        mipMaps = reader.ReadByte();

        width = reader.ReadUInt16();
        height = reader.ReadUInt16();

        bpl = reader.ReadUInt16();

        wiiAlphaNum = reader.ReadUInt16();

        headerPadding = reader.ReadBlock(GetHeaderPaddingLength(legacyHeader));

        int paletteSize = GetColorPaletteSize();
        int dataSize = checked(bpl * height);
        long payloadSize = paletteSize;
        int mipSize = dataSize;
        for (int i = 0; i <= mipMaps; i++)
        {
            payloadSize += mipSize;
            mipSize >>= 2;
        }
        if (payloadSize > reader.BaseStream.Length - reader.BaseStream.Position)
        {
            throw new InvalidDataException("Bitmap payload exceeds the remaining stream.");
        }

        if (paletteSize > 0)
        {
            colorPalette = reader.ReadBlock(paletteSize);
        }
        else
        {
            colorPalette = Array.Empty<byte>();
        }

        textures = new List<byte[]>(mipMaps + 1);
        for (int i = 0; i < mipMaps + 1; i++)
        {
            textures.Add(reader.ReadBlock(dataSize));
            dataSize >>= 2;
        }

        return this;
    }

    public void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry, bool legacyHeader = false)
    {
        writer.WriteByte(revision);

        if (revision > 1 && !legacyHeader)
        {
            writer.WriteUInt32(crc);
        }

        writer.WriteByte(bpp);

        if (revision == 0)
        {
            writer.WriteByte((byte)encoding);
        }
        else
        {
            writer.WriteUInt32((uint)encoding);
        }

        writer.WriteByte(mipMaps);

        writer.WriteUInt16(width);
        writer.WriteUInt16(height);

        writer.WriteUInt16(bpl);

        writer.WriteUInt16(wiiAlphaNum);

        int paddingLength = GetHeaderPaddingLength(legacyHeader);
        if (headerPadding.Length == paddingLength)
        {
            writer.WriteBlock(headerPadding);
        }
        else
        {
            writer.WriteBlock(new byte[paddingLength]);
        }

        if (GetColorPaletteSize() > 0)
        {
            writer.WriteBlock(colorPalette);
        }

        foreach (byte[] texture in textures)
        {
            long pixelStart = writer.Position;
            writer.WriteBlock(texture);

            if (parent.platform == DirectoryMeta.Platform.Wii)
            {
                writer.MarkChunks(pixelStart, writer.Position, 0x8000);
            }
        }
    }

    private int GetHeaderPaddingLength(bool legacyHeader)
    {
        if (revision == 0)
        {
            return 4;
        }
        if (revision == 1 || legacyHeader)
        {
            return 17;
        }
        return 13;
    }

    public override string ToString()
    {
        return $"Width: {width}, Height: {height}";
    }

    /// <summary>
    /// Converts the bitmap to an image.
    /// </summary>
    /// <returns>The image, as a list of bytes. The actual format will depend on the encoding.</returns>
    public byte[] ConvertToImage()
    {
        // check if the encoding is some form of DDS
        if (encoding == TextureEncoding.DXT1_BC1 || encoding == TextureEncoding.DXT5_BC3 || encoding == TextureEncoding.ATI2_BC5)
        {
            // build a DDS header based on the information in the bitmap, then append all the texture bytes
            byte[] header = CreateDDSHeader();
            int totalTextureBytes = textures.Sum(texture => texture.Length);
            byte[] ddsData = new byte[header.Length + totalTextureBytes];
            Buffer.BlockCopy(header, 0, ddsData, 0, header.Length);
            int writeOffset = header.Length;

            if (platform == DirectoryMeta.Platform.Xbox)
            {
                foreach (byte[] texture in textures)
                {
                    for (int i = 0; i < texture.Length; i += 4)
                    {
                        ddsData[writeOffset + i + 0] = texture[i + 1];
                        ddsData[writeOffset + i + 1] = texture[i + 0];
                        ddsData[writeOffset + i + 2] = texture[i + 3];
                        ddsData[writeOffset + i + 3] = texture[i + 2];
                    }
                    writeOffset += texture.Length;
                }

                return ddsData;
            }
            else
            {
                foreach (byte[] texture in textures)
                {
                    Buffer.BlockCopy(texture, 0, ddsData, writeOffset, texture.Length);
                    writeOffset += texture.Length;
                }

                return ddsData;
            }
        }
        return Array.Empty<byte>();
    }

    private byte[] CreateDDSHeader()
    {
        using (MemoryStream stream = new())
        using (EndianWriter writer = new(stream, Endian.LittleEndian))
        {
            writer.WriteBlock(Encoding.ASCII.GetBytes("DDS "));
            writer.WriteUInt32(124);
            uint flags = 0x1 | 0x2 | 0x4 | 0x1000 | 0x80000;
            if (mipMaps > 0)
            {
                flags |= 0x20000;
            }
            writer.WriteUInt32(flags);
            writer.WriteUInt32(height);
            writer.WriteUInt32(width);

            uint pitchOrLinearSize = 0;
            if (encoding == TextureEncoding.DXT1_BC1)
            {
                pitchOrLinearSize = (uint)(Math.Max(1, ((width + 3) / 4)) * Math.Max(1, ((height + 3) / 4)) * 8);
            }
            else if (encoding == TextureEncoding.DXT5_BC3 || encoding == TextureEncoding.ATI2_BC5)
            {
                pitchOrLinearSize = (uint)(Math.Max(1, ((width + 3) / 4)) * Math.Max(1, ((height + 3) / 4)) * 16);
            }
            else if (encoding == TextureEncoding.ARGB || encoding == TextureEncoding.RGBA)
            {
                pitchOrLinearSize = (uint)(width * height * bpp / 8);
            }

            writer.WriteUInt32(pitchOrLinearSize);
            writer.WriteUInt32(0);
            writer.WriteUInt32((uint)(mipMaps > 0 ? mipMaps + 1 : 0));
            writer.WriteBlock(ZeroPad44);

            writer.WriteUInt32(32);

            if (encoding == TextureEncoding.DXT1_BC1 || encoding == TextureEncoding.DXT5_BC3 || encoding == TextureEncoding.ATI2_BC5)
            {
                writer.WriteUInt32(0x00000004);
                if (encoding == TextureEncoding.DXT1_BC1)
                {
                    writer.WriteBlock(Encoding.ASCII.GetBytes("DXT1"));
                }
                else if (encoding == TextureEncoding.DXT5_BC3)
                {
                    writer.WriteBlock(Encoding.ASCII.GetBytes("DXT5"));
                }
                else if (encoding == TextureEncoding.ATI2_BC5)
                {
                    writer.WriteBlock(Encoding.ASCII.GetBytes("ATI2"));
                }
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
                writer.WriteUInt32(0);
            }
            else if (encoding == TextureEncoding.ARGB || encoding == TextureEncoding.RGBA)
            {
                writer.WriteUInt32(0x00000040);
                writer.WriteUInt32(0);
                writer.WriteUInt32((uint)bpp);
                if (encoding == TextureEncoding.ARGB)
                {
                    writer.WriteUInt32(0x00ff0000);
                    writer.WriteUInt32(0x0000ff00);
                    writer.WriteUInt32(0x000000ff);
                    writer.WriteUInt32(0xff000000);
                }
                else if (encoding == TextureEncoding.RGBA)
                {
                    writer.WriteUInt32(0x00ff0000);
                    writer.WriteUInt32(0x0000ff00);
                    writer.WriteUInt32(0x000000ff);
                    writer.WriteUInt32(0xff000000);
                }
            }

            writer.WriteUInt32(0x00000008);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);
            writer.WriteUInt32(0);

            return stream.ToArray();
        }
    }

    public static RndBitmap New(byte revision, ushort altRevision)
    {
        RndBitmap bitmap = new();
        bitmap.revision = revision;
        return bitmap;
    }


}
