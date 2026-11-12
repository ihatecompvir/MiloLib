using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("CubeTex"), Description("This class implements a cube map intended for environment mapping.")]
public class RndCubeTex : Object
{
    public class CubeTexProperties
    {
        public int bpp;
        public int height;
        public int width;
        public int numMips;
        public int order;

        public void Read(EndianReader reader)
        {
            bpp = reader.ReadInt32();
            height = reader.ReadInt32();
            width = reader.ReadInt32();
            numMips = reader.ReadInt32();
            order = reader.ReadInt32();
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32(bpp);
            writer.WriteInt32(height);
            writer.WriteInt32(width);
            writer.WriteInt32(numMips);
            writer.WriteInt32(order);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [MinVersion(2)]
    public CubeTexProperties props = new();
    [MinVersion(2)]
    public CubeTexProperties[] faceProps = new CubeTexProperties[6];
    public Symbol[] filePaths = new Symbol[6];
    public RndBitmap[] bitmaps = new RndBitmap[6];
    public byte[][] oldWiiAlphaData = new byte[6][];

    // Rev < 2 fields
    private int oldWidth;
    private int oldBpp;
    private bool oldPostLoadBool;
    private bool usesLegacyBitmapHeaders;

    public RndCubeTex()
    {
        for (int i = 0; i < 6; i++)
        {
            faceProps[i] = new CubeTexProperties();
            filePaths[i] = Symbol.Empty;
            bitmaps[i] = new RndBitmap();
            oldWiiAlphaData[i] = Array.Empty<byte>();
        }
    }

    public new RndCubeTex Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision > 1)
        {
            props.Read(reader);
            for (int i = 0; i < 6; i++)
            {
                faceProps[i].Read(reader);
            }
        }
        else
        {
            oldWidth = reader.ReadInt32();
            oldBpp = reader.ReadInt32();
            props.width = oldWidth;
            props.height = oldWidth;
            props.bpp = oldBpp;
        }

        for (int i = 0; i < 6; i++)
        {
            filePaths[i] = Symbol.Read(reader);
        }

        if (revision < 2)
        {
            oldPostLoadBool = reader.ReadBoolean();
        }

        ReadBitmaps(reader, parent, entry);

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

        if (revision > 1)
        {
            props.Write(writer);
            for (int i = 0; i < 6; i++)
            {
                faceProps[i].Write(writer);
            }
        }
        else
        {
            writer.WriteInt32(oldWidth);
            writer.WriteInt32(oldBpp);
        }

        for (int i = 0; i < 6; i++)
        {
            Symbol.Write(writer, filePaths[i]);
        }

        if (revision < 2)
        {
            writer.WriteBoolean(oldPostLoadBool);
        }

        for (int i = 0; i < 6; i++)
        {
            bitmaps[i].Write(writer, false, parent, entry, usesLegacyBitmapHeaders);
            writer.WriteBlock(oldWiiAlphaData[i]);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private void ReadBitmaps(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        usesLegacyBitmapHeaders = false;
        if (parent.revision == 25 && revision == 1)
        {
            long start = reader.BaseStream.Position;
            try
            {
                RndBitmap[] candidate = ReadBitmapFaces(reader, parent, entry, true, out byte[][] candidateAlphaData);
                long end = reader.BaseStream.Position;
                uint expected = 0xADDEADDE;
                if (reader.Endianness == Endian.LittleEndian)
                {
                    expected = 0xDEADDEAD;
                }

                if (reader.ReadUInt32() != expected)
                {
                    throw new InvalidDataException("Legacy cube faces do not end at the object boundary.");
                }

                reader.BaseStream.Position = end;
                bitmaps = candidate;
                oldWiiAlphaData = candidateAlphaData;
                usesLegacyBitmapHeaders = true;
                return;
            }
            catch (Exception exception) when (exception is InvalidDataException || exception is EndOfStreamException || exception is OverflowException)
            {
                reader.BaseStream.Position = start;
            }
        }
        bitmaps = ReadBitmapFaces(reader, parent, entry, false, out oldWiiAlphaData);
    }

    private static RndBitmap[] ReadBitmapFaces(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry, bool legacyHeader, out byte[][] alphaData)
    {
        RndBitmap[] faces = new RndBitmap[6];
        alphaData = new byte[6][];
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i] = new RndBitmap().Read(reader, false, parent, entry, legacyHeader);
            alphaData[i] = Array.Empty<byte>();
            RndBitmap face = faces[i];
            if (legacyHeader && parent.platform == DirectoryMeta.Platform.Wii && ((uint)face.encoding & ~0x200u) == (uint)RndBitmap.TextureEncoding.TPL_CMP && face.wiiAlphaNum == 4)
            {
                int alphaSize = checked(face.bpl * face.height);
                if (alphaSize > reader.BaseStream.Length - reader.BaseStream.Position)
                {
                    throw new InvalidDataException("Cube face alpha image exceeds the remaining stream.");
                }

                alphaData[i] = reader.ReadBlock(alphaSize);
            }
        }
        return faces;
    }
}
