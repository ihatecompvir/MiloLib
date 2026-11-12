using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace MiloLib.Utils;

/// <summary>
/// Writes Milo primitive values directly to a seekable in-memory buffer
/// </summary>
public sealed class EndianMemoryWriter
{
    private const int StackBufferSize = 256;
    private static readonly Encoding s_latin1 = Encoding.Latin1;
    private static readonly byte[] s_zeroes = new byte[256];
    private static readonly byte[] s_endBytes = { 0xAD, 0xDE, 0xAD, 0xDE };
    private byte[] _buffer;
    private bool _bigEndian;
    private int _position;
    private int _length;

    public EndianMemoryWriter(Endian endianness, int initialCapacity = 256)
    {
        _buffer = GC.AllocateUninitializedArray<byte>(Math.Max(initialCapacity, 1));
        _bigEndian = endianness == Endian.BigEndian;
    }

    public Endian Endianness
    {
        get => _bigEndian ? Endian.BigEndian : Endian.LittleEndian;
        set => _bigEndian = value == Endian.BigEndian;
    }

    public int Position
    {
        get => _position;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            EnsureCapacity(value);
            _position = value;
            if (_position > _length)
            {
                Array.Clear(_buffer, _length, _position - _length);
                _length = _position;
            }
        }
    }

    public int Length => _length;
    public ReadOnlyMemory<byte> WrittenMemory => new(_buffer, 0, _length);
    public ReadOnlySpan<byte> WrittenSpan => new(_buffer, 0, _length);

    public void Clear()
    {
        _position = 0;
        _length = 0;
    }

    public void SetLength(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length));
        }

        EnsureCapacity(length);
        if (length > _length)
        {
            Array.Clear(_buffer, _length, length - _length);
        }

        _length = length;
        if (_position > length)
        {
            _position = length;
        }
    }

    public bool SeekTo(int offset)
    {
        if (offset < 0)
        {
            return false;
        }

        Position = offset;
        return true;
    }

    public void Skip(int count)
    {
        Position = checked(_position + count);
    }

    public byte[] ToArray()
    {
        if (_length == 0)
        {
            return Array.Empty<byte>();
        }

        byte[] result = GC.AllocateUninitializedArray<byte>(_length);
        Buffer.BlockCopy(_buffer, 0, result, 0, _length);
        return result;
    }

    public void WriteByte(byte value)
    {
        Span<byte> span = GetDestination(sizeof(byte));
        span[0] = value;
        Advance(sizeof(byte));
    }

    public void WriteSByte(sbyte value)
    {
        WriteByte((byte)value);
    }

    public void WriteUInt16(ushort value)
    {
        Span<byte> span = GetDestination(sizeof(ushort));
        if (_bigEndian)
        {
            BinaryPrimitives.WriteUInt16BigEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);
        }

        Advance(sizeof(ushort));
    }

    public void WriteInt16(short value)
    {
        WriteUInt16((ushort)value);
    }

    public void WriteUInt32(uint value)
    {
        Span<byte> span = GetDestination(sizeof(uint));
        if (_bigEndian)
        {
            BinaryPrimitives.WriteUInt32BigEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
        }

        Advance(sizeof(uint));
    }

    public void WriteInt32(int value)
    {
        WriteUInt32((uint)value);
    }

    public void WriteUInt64(ulong value)
    {
        Span<byte> span = GetDestination(sizeof(ulong));
        if (_bigEndian)
        {
            BinaryPrimitives.WriteUInt64BigEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt64LittleEndian(span, value);
        }

        Advance(sizeof(ulong));
    }

    public void WriteInt64(long value)
    {
        WriteUInt64((ulong)value);
    }

    public void WriteHalfFloat(float value)
    {
        Half half = (Half)value;
        Span<byte> span = GetDestination(sizeof(ushort));
        if (_bigEndian)
        {
            BinaryPrimitives.WriteHalfBigEndian(span, half);
        }
        else
        {
            BinaryPrimitives.WriteHalfLittleEndian(span, half);
        }

        Advance(sizeof(ushort));
    }

    public void WriteFloat(float value)
    {
        Span<byte> span = GetDestination(sizeof(float));
        if (_bigEndian)
        {
            BinaryPrimitives.WriteSingleBigEndian(span, value);
        }
        else
        {
            BinaryPrimitives.WriteSingleLittleEndian(span, value);
        }

        Advance(sizeof(float));
    }

    public void WriteBoolean(bool value)
    {
        WriteByte(value ? (byte)1 : (byte)0);
    }

    public void WriteAscii(string value)
    {
        WriteEncodedString(value.AsSpan(), s_latin1);
        WriteByte(0);
    }

    public void WriteAscii(string value, int length)
    {
        int byteCount = s_latin1.GetByteCount(value);
        if (byteCount >= length)
        {
            throw new ArgumentException("The string exceeds the specified length.", nameof(value));
        }

        WriteEncodedString(value.AsSpan(), s_latin1);
        WriteZeroes(length - byteCount);
    }

    public void WriteUTF8(string value)
    {
        WriteEncodedString(value.AsSpan(), Encoding.UTF8);
        WriteByte(0);
    }

    public void WriteUTF16(string value)
    {
        WriteEncodedString(value.AsSpan(), _bigEndian ? Encoding.BigEndianUnicode : Encoding.Unicode);
        WriteInt16(0);
    }

    public void WriteBlock(byte[] data)
    {
        WriteBlock(data.AsSpan());
    }

    public void WriteBlock(byte[] data, int offset, int size)
    {
        WriteBlock(data.AsSpan(offset, size));
    }

    public void WriteBlock(ReadOnlySpan<byte> data)
    {
        if (data.IsEmpty)
        {
            return;
        }

        data.CopyTo(GetDestination(data.Length));
        Advance(data.Length);
    }

    public void WriteEndBytes()
    {
        WriteBlock(s_endBytes);
    }

    public void WriteZeroes(int count)
    {
        while (count > 0)
        {
            int chunk = Math.Min(count, s_zeroes.Length);
            WriteBlock(s_zeroes.AsSpan(0, chunk));
            count -= chunk;
        }
    }

    private Span<byte> GetDestination(int size)
    {
        EnsureCapacity(checked(_position + size));
        return _buffer.AsSpan(_position, size);
    }

    private void Advance(int count)
    {
        _position += count;
        if (_position > _length)
        {
            _length = _position;
        }
    }

    private void EnsureCapacity(int capacity)
    {
        if (capacity <= _buffer.Length)
        {
            return;
        }

        int newSize = _buffer.Length;
        while (newSize < capacity)
        {
            newSize *= 2;
        }

        Array.Resize(ref _buffer, newSize);
    }

    private void WriteEncodedString(ReadOnlySpan<char> value, Encoding encoding)
    {
        int byteCount = encoding.GetByteCount(value);
        if (byteCount == 0)
        {
            return;
        }

        byte[]? rented = null;
        Span<byte> buffer = byteCount <= StackBufferSize
            ? stackalloc byte[byteCount]
            : (rented = ArrayPool<byte>.Shared.Rent(byteCount)).AsSpan(0, byteCount);

        try
        {
            int written = encoding.GetBytes(value, buffer);
            WriteBlock(buffer[..written]);
        }
        finally
        {
            if (rented != null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }
}
