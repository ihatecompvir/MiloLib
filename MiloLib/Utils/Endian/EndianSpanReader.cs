using System.Buffers.Binary;

namespace MiloLib.Utils;

/// <summary>
/// The span version of EndianReader
/// </summary>
internal ref struct EndianSpanReader
{
    private readonly ReadOnlySpan<byte> data;
    private readonly bool bigEndian;

    public int Position { get; private set; }

    public EndianSpanReader(ReadOnlySpan<byte> data, bool bigEndian)
    {
        this.data = data;
        this.bigEndian = bigEndian;
        Position = 0;
    }

    public byte ReadByte()
    {
        if ((uint)Position >= (uint)data.Length)
        {
            throw new EndOfStreamException("Attempted to read 1 byte past the end of the stream.");
        }

        byte value = data[Position];
        Position++;
        return value;
    }

    public ushort ReadUInt16()
    {
        ReadOnlySpan<byte> bytes = Take(sizeof(ushort));
        if (bigEndian)
        {
            return BinaryPrimitives.ReadUInt16BigEndian(bytes);
        }

        return BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public uint ReadUInt32()
    {
        ReadOnlySpan<byte> bytes = Take(sizeof(uint));
        if (bigEndian)
        {
            return BinaryPrimitives.ReadUInt32BigEndian(bytes);
        }

        return BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public float ReadFloat()
    {
        return BitConverter.UInt32BitsToSingle(ReadUInt32());
    }

    private ReadOnlySpan<byte> Take(int size)
    {
        if (Position > data.Length - size)
        {
            throw new EndOfStreamException($"Attempted to read {size} bytes past the end of the stream.");
        }

        ReadOnlySpan<byte> bytes = data.Slice(Position, size);
        Position += size;
        return bytes;
    }
}
