using MiloLib.Assets;
using System.Buffers.Binary;

namespace MiloLib.Utils;

internal static class AssetHelpers
{
    // stuff for chunk header sizes
    internal const int FileLoaderPieceSize = 0x10000;
    internal static void MarkFileLoaderChunks(EndianWriter writer, long blobStart, ReadOnlySpan<byte> blob)
    {
        if (blob.Length < 12)
        {
            return;
        }

        int first;
        int pieceFlag;
        int size;
        if (writer.Endianness == Endian.BigEndian)
        {
            first = BinaryPrimitives.ReadInt32BigEndian(blob);
            pieceFlag = BinaryPrimitives.ReadInt32BigEndian(blob.Slice(4));
            size = BinaryPrimitives.ReadInt32BigEndian(blob.Slice(8));
        }
        else
        {
            first = BinaryPrimitives.ReadInt32LittleEndian(blob);
            pieceFlag = BinaryPrimitives.ReadInt32LittleEndian(blob.Slice(4));
            size = BinaryPrimitives.ReadInt32LittleEndian(blob.Slice(8));
        }

        if (first != -1 || pieceFlag <= 0 || size < 0 || 12 + (long)size > blob.Length)
        {
            return;
        }

        writer.MarkChunks(blobStart + 12, blobStart + 12 + size, FileLoaderPieceSize);
    }

    internal static void ReadRevision(EndianReader reader, out ushort revision, out ushort altRevision)
    {
        uint combined = reader.ReadUInt32();
        revision = (ushort)(combined & 0xFFFF);
        altRevision = (ushort)(combined >> 16);
        if (!BitConverter.IsLittleEndian)
        {
            (revision, altRevision) = (altRevision, revision);
        }
    }

    // reads the revision and alt revision, throwing if the revision is newer than the maximum supported
    internal static void ReadRevision(EndianReader reader, out ushort revision, out ushort altRevision, ushort maximum)
    {
        ReadRevision(reader, out revision, out altRevision);
        ValidateRevision(revision, maximum);
    }

    internal static void WriteRevision(EndianWriter writer, ushort revision, ushort altRevision)
    {
        uint combined = (uint)((altRevision << 16) | revision);
        if (!BitConverter.IsLittleEndian)
        {
            combined = (uint)((revision << 16) | altRevision);
        }
        writer.WriteUInt32(combined);
    }

    // writes the revision and alt revision, throwing if the revision is newer than the maximum supported
    internal static void WriteRevision(EndianWriter writer, ushort revision, ushort altRevision, ushort maximum)
    {
        ValidateRevision(revision, maximum);
        WriteRevision(writer, revision, altRevision);
    }

    private static void ValidateRevision(ushort revision, ushort maximum)
    {
        if (revision > maximum)
        {
            throw new NotSupportedException($"Unsupported object revision {revision}; maximum supported revision is {maximum}.");
        }
    }

    internal static List<T> ReadList<T>(EndianReader reader, Func<T> readItem)
    {
        int count = reader.ReadInt32();
        if (count < 0 || count > reader.BaseStream.Length - reader.BaseStream.Position)
        {
            throw new InvalidDataException($"Invalid serialized list count {count}.");
        }
        List<T> values = new(count);
        for (int i = 0; i < count; i++)
        {
            values.Add(readItem());
        }
        return values;
    }

    internal static void WriteList<T>(EndianWriter writer, List<T> values, Action<T> writeItem)
    {
        writer.WriteInt32(values.Count);
        foreach (T value in values)
        {
            writeItem(value);
        }
    }

    // returns true if the next four bytes are the end marker; the stream position does not change
    internal static bool IsEndBytes(EndianReader reader)
    {
        long position = reader.BaseStream.Position;
        if (reader.BaseStream.Length - position < 4)
        {
            return false;
        }

        uint value = reader.ReadUInt32();
        reader.BaseStream.Position = position;
        if (reader.Endianness == Endian.BigEndian)
        {
            return value == 0xADDEADDE;
        }
        return value == 0xDEADDEAD;
    }

    internal static void ReadEnd(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry, ushort? revision = null)
    {
        uint expected = 0xADDEADDE;
        if (reader.Endianness == Endian.LittleEndian)
        {
            expected = 0xDEADDEAD;
        }
        if (reader.ReadUInt32() != expected)
        {
            throw MiloLib.Exceptions.MiloAssetReadException.EndBytesNotFound(parent, entry, reader.BaseStream.Position, revision);
        }
    }
}
