using System.Buffers;

namespace MiloLib.Utils;

internal static class BinaryParsingUtils
{
    private static readonly byte[] _endMarker = { 0xAD, 0xDE, 0xAD, 0xDE };

    internal static ReadOnlySpan<byte> EndMarker => _endMarker;

    internal static bool TryGetMemorySlice(MemoryStream stream, int start, int length, out ReadOnlyMemory<byte> slice)
    {
        if (stream.TryGetBuffer(out ArraySegment<byte> segment))
        {
            slice = new ReadOnlyMemory<byte>(segment.Array!, segment.Offset + start, length);
            return true;
        }

        slice = default;
        return false;
    }

    internal static bool TryGetMemorySliceUntilEndMarker(MemoryStream stream, int start, out ReadOnlyMemory<byte> slice, out int bytesConsumed)
    {
        if (stream.TryGetBuffer(out ArraySegment<byte> segment))
        {
            var span = segment.AsSpan(start, (int)stream.Length - start);
            int markerIndex = span.IndexOf(EndMarker);
            if (markerIndex >= 0)
            {
                slice = new ReadOnlyMemory<byte>(segment.Array!, segment.Offset + start, markerIndex);
                bytesConsumed = markerIndex + EndMarker.Length;
                return true;
            }
        }

        slice = default;
        bytesConsumed = 0;
        return false;
    }

    internal static byte[] ReadUntilEndMarkerAllocating(EndianReader reader)
    {
        ArrayBufferWriter<byte> writer = new();

        while (true)
        {
            byte b = reader.ReadByte();
            if (b == 0xAD)
            {
                long markerStart = reader.BaseStream.Position;
                if (reader.ReadByte() == 0xDE &&
                    reader.ReadByte() == 0xAD &&
                    reader.ReadByte() == 0xDE)
                {
                    break;
                }

                reader.BaseStream.Position = markerStart;
            }

            Span<byte> destination = writer.GetSpan(1);
            destination[0] = b;
            writer.Advance(1);
        }

        return writer.WrittenCount == 0 ? Array.Empty<byte>() : writer.WrittenMemory.ToArray();
    }

    internal static byte[] ReadUntilEndMarker(EndianReader reader)
    {
        if (reader.BaseStream is MemoryStream stream)
        {
            int start = (int)stream.Position;
            if (TryGetMemorySliceUntilEndMarker(stream, start, out ReadOnlyMemory<byte> slice, out int bytesConsumed))
            {
                stream.Position = start + bytesConsumed;
                return slice.ToArray();
            }
        }

        return ReadUntilEndMarkerAllocating(reader);
    }

    internal static void SkipUntilEndMarker(EndianReader reader)
    {
        while (true)
        {
            byte b = reader.ReadByte();
            if (b != 0xAD)
            {
                continue;
            }

            long markerStart = reader.BaseStream.Position;
            if (reader.ReadByte() == 0xDE &&
                reader.ReadByte() == 0xAD &&
                reader.ReadByte() == 0xDE)
            {
                return;
            }

            reader.BaseStream.Position = markerStart;
        }
    }
}
