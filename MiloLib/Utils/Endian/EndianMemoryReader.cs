using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;

namespace MiloLib.Utils;

/// <summary>
/// Reads Milo primitive values directly from an in-memory buffer while avoiding routing each read through Stream
/// </summary>
public sealed class EndianMemoryReader
{
    private const int StackBufferSize = 256;
    private static readonly Encoding s_latin1 = Encoding.Latin1;
    private readonly ReadOnlyMemory<byte> _memory;
    private readonly byte[]? _array;
    private readonly int _arrayOffset;
    private bool _bigEndian;
    private int _position;

    public EndianMemoryReader(ReadOnlyMemory<byte> memory, Endian endianness)
    {
        _memory = memory;
        _bigEndian = endianness == Endian.BigEndian;

        if (MemoryMarshal.TryGetArray(memory, out ArraySegment<byte> segment) && segment.Array != null)
        {
            _array = segment.Array;
            _arrayOffset = segment.Offset;
        }
    }

    public EndianMemoryReader(byte[] buffer, Endian endianness)
        : this(new ReadOnlyMemory<byte>(buffer), endianness)
    {
    }

    public Endian Endianness
    {
        get => _bigEndian ? Endian.BigEndian : Endian.LittleEndian;
        set => _bigEndian = value == Endian.BigEndian;
    }

    public int Position => _position;
    public int Length => _memory.Length;
    public bool EOF => _position >= _memory.Length;

    public bool SeekTo(int offset)
    {
        if ((uint)offset > (uint)_memory.Length)
        {
            return false;
        }

        _position = offset;
        return true;
    }

    public void Skip(int count)
    {
        int newPosition = _position + count;
        if ((uint)newPosition > (uint)_memory.Length)
        {
            throw new EndOfStreamException($"Attempted to skip {count} bytes past the end of the buffer.");
        }

        _position = newPosition;
    }

    public byte ReadByte()
    {
        ReadOnlySpan<byte> span = ReadSpan(sizeof(byte));
        return span[0];
    }

    public sbyte ReadSByte()
    {
        return (sbyte)ReadByte();
    }

    public ushort ReadUInt16()
    {
        ReadOnlySpan<byte> span = ReadSpan(sizeof(ushort));
        return _bigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(span)
            : BinaryPrimitives.ReadUInt16LittleEndian(span);
    }

    public short ReadInt16()
    {
        return (short)ReadUInt16();
    }

    public uint ReadUInt32()
    {
        ReadOnlySpan<byte> span = ReadSpan(sizeof(uint));
        return _bigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(span)
            : BinaryPrimitives.ReadUInt32LittleEndian(span);
    }

    public int ReadInt32()
    {
        return (int)ReadUInt32();
    }

    public ulong ReadUInt64()
    {
        ReadOnlySpan<byte> span = ReadSpan(sizeof(ulong));
        return _bigEndian
            ? BinaryPrimitives.ReadUInt64BigEndian(span)
            : BinaryPrimitives.ReadUInt64LittleEndian(span);
    }

    public long ReadInt64()
    {
        return (long)ReadUInt64();
    }

    public float ReadHalfFloat()
    {
        return (float)BitConverter.UInt16BitsToHalf(ReadUInt16());
    }

    public float ReadFloat()
    {
        uint bits = ReadUInt32();
        return BitConverter.UInt32BitsToSingle(bits);
    }

    public bool ReadBoolean()
    {
        return ReadByte() != 0;
    }

    public byte[] ReadBlock(int size)
    {
        if (size == 0)
        {
            return Array.Empty<byte>();
        }

        byte[] result = GC.AllocateUninitializedArray<byte>(size);
        ReadBlock(result);
        return result;
    }

    public void ReadBlock(Span<byte> destination)
    {
        ReadSpan(destination.Length).CopyTo(destination);
    }

    public int ReadBlock(byte[] output, int offset, int size)
    {
        ReadBlock(output.AsSpan(offset, size));
        return size;
    }

    public ReadOnlySpan<byte> ReadSpan(int size)
    {
        if ((uint)size > (uint)(_memory.Length - _position))
        {
            throw new EndOfStreamException($"Attempted to read {size} bytes past the end of the buffer.");
        }

        ReadOnlySpan<byte> span = _array != null
            ? new ReadOnlySpan<byte>(_array, _arrayOffset + _position, size)
            : _memory.Span.Slice(_position, size);
        _position += size;
        return span;
    }

    public string ReadAscii()
    {
        return ReadNullTerminatedSingleByteString(s_latin1);
    }

    public string ReadAscii(int size)
    {
        return ReadFixedSingleByteString(size, s_latin1);
    }

    public string ReadUTF8()
    {
        return ReadNullTerminatedSingleByteString(Encoding.UTF8);
    }

    public string ReadUTF8(int size)
    {
        return ReadFixedSingleByteString(size, Encoding.UTF8);
    }

    public string ReadUTF16()
    {
        ReadOnlySpan<byte> remaining = RemainingSpan();
        int byteLength = FindUtf16TerminatorLength(remaining, requireTerminator: true);
        _position += byteLength + sizeof(ushort);
        return GetUtf16Encoding().GetString(remaining[..byteLength]);
    }

    public string ReadUTF16(int size)
    {
        ReadOnlySpan<byte> span = ReadSpan(size);
        int byteLength = FindUtf16TerminatorLength(span, requireTerminator: false);
        return GetUtf16Encoding().GetString(span[..byteLength]);
    }

    private string ReadNullTerminatedSingleByteString(Encoding encoding)
    {
        ReadOnlySpan<byte> remaining = RemainingSpan();
        int terminatorIndex = remaining.IndexOf((byte)0);
        if (terminatorIndex < 0)
        {
            throw new EndOfStreamException("Attempted to read past the end of the buffer.");
        }

        _position += terminatorIndex + 1;
        return encoding.GetString(remaining[..terminatorIndex]);
    }

    private string ReadFixedSingleByteString(int size, Encoding encoding)
    {
        if (size <= 0)
        {
            return string.Empty;
        }

        ReadOnlySpan<byte> span = ReadSpan(size);
        int length = span.IndexOf((byte)0);
        if (length < 0)
        {
            length = size;
        }

        return encoding.GetString(span[..length]);
    }

    private Encoding GetUtf16Encoding()
    {
        return _bigEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;
    }

    private ReadOnlySpan<byte> RemainingSpan()
    {
        return _array != null
            ? new ReadOnlySpan<byte>(_array, _arrayOffset + _position, _memory.Length - _position)
            : _memory.Span[_position..];
    }

    private int FindUtf16TerminatorLength(ReadOnlySpan<byte> buffer, bool requireTerminator)
    {
        int length = 0;
        for (; length <= buffer.Length - sizeof(ushort); length += sizeof(ushort))
        {
            ushort value = _bigEndian
                ? BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(length, sizeof(ushort)))
                : BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(length, sizeof(ushort)));

            if (value == 0)
            {
                return length;
            }
        }

        if (requireTerminator)
        {
            throw new EndOfStreamException("Attempted to read past the end of the buffer.");
        }

        return buffer.Length & ~1;
    }
}
