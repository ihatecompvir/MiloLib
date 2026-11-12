// originally taken from https://github.com/XboxChaos/Assembly, thanks for originally writing this code
// license in Utils/Endian/LICENSE.md

using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace MiloLib.Utils;

/// <summary>
///     A stream which can be read from and whose endianness can be changed.
/// </summary>
public class EndianReader : IDisposable, IReader
{
    private const int StackBufferSize = 256;
    private static readonly Encoding s_latin1 = Encoding.Latin1;
    private readonly byte[]? _memoryBuffer;
    private readonly int _memoryBufferOffset;
    private readonly int _memoryBufferLength;
    private readonly Stream _stream;
    private bool _bigEndian;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EndianReader" /> class.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="endianness">The initial endianness to use when reading from the stream.</param>
    public EndianReader(Stream stream, Endian endianness)
    {
        _stream = stream;
        _bigEndian = (endianness == Endian.BigEndian);

        if (stream is MemoryStream memoryStream && memoryStream.TryGetBuffer(out ArraySegment<byte> buffer) && buffer.Array != null)
        {
            _memoryBuffer = buffer.Array;
            _memoryBufferOffset = buffer.Offset;
            _memoryBufferLength = buffer.Count;
        }
    }

    public EndianReader(byte[] buffer, Endian endianness)
        : this(new MemoryStream(buffer, 0, buffer.Length, writable: false, publiclyVisible: true), endianness)
    {
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _stream.Dispose();
    }

    /// <summary>
    ///     Gets or sets the endianness used when reading/writing to/from the stream.
    /// </summary>
    public Endian Endianness
    {
        get { return _bigEndian ? Endian.BigEndian : Endian.LittleEndian; }
        set { _bigEndian = (value == Endian.BigEndian); }
    }

    /// <summary>
    ///     Reads a byte from the stream.
    /// </summary>
    /// <returns>
    ///     The byte that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public byte ReadByte()
    {
        if (_memoryBuffer != null)
        {
            long position = _stream.Position;
            if ((ulong)position >= (ulong)_memoryBufferLength)
            {
                throw new EndOfStreamException("Attempted to read past the end of the stream.");
            }

            _stream.Position = position + 1;
            return _memoryBuffer[_memoryBufferOffset + (int)position];
        }

        int result = _stream.ReadByte();
        if (result == -1)
        {
            throw new EndOfStreamException("Attempted to read past the end of the stream.");
        }

        return (byte)result;
    }

    /// <summary>
    ///     Reads a signed byte from the stream.
    /// </summary>
    /// <returns>
    ///     The signed byte that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public sbyte ReadSByte()
    {
        return (sbyte)ReadByte();
    }

    /// <summary>
    ///     Reads a 16-bit unsigned integer from the stream.
    /// </summary>
    /// <returns>
    ///     The 16-bit unsigned integer that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public ushort ReadUInt16()
    {
        if (TryReadFastSpan(sizeof(ushort), out ReadOnlySpan<byte> fastSpan))
        {
            return _bigEndian
                ? BinaryPrimitives.ReadUInt16BigEndian(fastSpan)
                : BinaryPrimitives.ReadUInt16LittleEndian(fastSpan);
        }

        Span<byte> buf = stackalloc byte[sizeof(ushort)];
        _stream.ReadExactly(buf);

        return _bigEndian
            ? BinaryPrimitives.ReadUInt16BigEndian(buf)
            : BinaryPrimitives.ReadUInt16LittleEndian(buf);
    }

    /// <summary>
    ///     Reads a 16-bit signed integer from the stream.
    /// </summary>
    /// <returns>
    ///     The 16-bit signed integer that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public short ReadInt16()
    {
        return (short)ReadUInt16();
    }

    /// <summary>
    ///     Reads a 32-bit unsigned integer from the stream.
    /// </summary>
    /// <returns>
    ///     The 32-bit unsigned integer that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public uint ReadUInt32()
    {
        if (TryReadFastSpan(sizeof(uint), out ReadOnlySpan<byte> fastSpan))
        {
            return _bigEndian
                ? BinaryPrimitives.ReadUInt32BigEndian(fastSpan)
                : BinaryPrimitives.ReadUInt32LittleEndian(fastSpan);
        }

        Span<byte> buf = stackalloc byte[sizeof(uint)];
        _stream.ReadExactly(buf);

        return _bigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(buf)
            : BinaryPrimitives.ReadUInt32LittleEndian(buf);
    }

    /// <summary>
    ///     Reads a 32-bit signed integer from the stream.
    /// </summary>
    /// <returns>
    ///     The 32-bit signed integer that was read.
    /// </returns>
    ///  <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public int ReadInt32()
    {
        return (int)ReadUInt32();
    }

    /// <summary>
    ///     Reads a 64-bit unsigned integer from the stream.
    /// </summary>
    /// <returns>
    ///     The 64-bit unsigned integer that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public ulong ReadUInt64()
    {
        if (TryReadFastSpan(sizeof(ulong), out ReadOnlySpan<byte> fastSpan))
        {
            return _bigEndian
                ? BinaryPrimitives.ReadUInt64BigEndian(fastSpan)
                : BinaryPrimitives.ReadUInt64LittleEndian(fastSpan);
        }

        Span<byte> buf = stackalloc byte[sizeof(ulong)];
        _stream.ReadExactly(buf);
        return _bigEndian
            ? BinaryPrimitives.ReadUInt64BigEndian(buf)
            : BinaryPrimitives.ReadUInt64LittleEndian(buf);
    }

    /// <summary>
    ///     Reads a 64-bit signed integer from the stream.
    /// </summary>
    /// <returns>
    ///     The 64-bit signed integer that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public long ReadInt64()
    {
        return (long)ReadUInt64();
    }

    public float ReadHalfFloat()
    {
        ushort bits = ReadUInt16();
        return (float)BitConverter.UInt16BitsToHalf(bits);
    }

    /// <summary>
    ///     Reads a 32-bit floating-point value from the stream.
    /// </summary>
    /// <returns>
    ///     The 32-bit floating-point value that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public float ReadFloat()
    {
        if (TryReadFastSpan(sizeof(float), out ReadOnlySpan<byte> fastSpan))
        {
            uint fastBits = _bigEndian
                ? BinaryPrimitives.ReadUInt32BigEndian(fastSpan)
                : BinaryPrimitives.ReadUInt32LittleEndian(fastSpan);

            return BitConverter.UInt32BitsToSingle(fastBits);
        }

        Span<byte> buf = stackalloc byte[sizeof(float)];
        _stream.ReadExactly(buf);

        uint bits = _bigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(buf)
            : BinaryPrimitives.ReadUInt32LittleEndian(buf);

        return BitConverter.UInt32BitsToSingle(bits);
    }

    /// <summary>
    ///     Seeks to an offset in the stream.
    /// </summary>
    /// <param name="offset">The offset to move the stream pointer to.</param>
    /// <returns>
    ///     true if the seek was successful.
    /// </returns>
    public bool SeekTo(long offset)
    {
        if (offset < 0)
        {
            return false;
        }

        _stream.Seek(offset, SeekOrigin.Begin);
        return true;
    }

    /// <summary>
    ///     Skips over a number of bytes in the stream.
    /// </summary>
    /// <param name="count">The number of bytes to skip.</param>
    public void Skip(long count)
    {
        _stream.Seek(count, SeekOrigin.Current);
    }

    /// <summary>
    ///     Reads a null-terminated ASCII string from the stream.
    /// </summary>
    /// <returns>
    ///     The ASCII string that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public string ReadAscii()
    {
        if (TryReadNullTerminatedSingleByteStringFast(s_latin1, out string? fastValue))
        {
            return fastValue;
        }

        return ReadNullTerminatedSingleByteString(s_latin1);
    }


    /// <summary>
    ///     Reads a fixed-size null-terminated ASCII string from the stream.
    /// </summary>
    /// <param name="size">The size of the string to read, including the null terminator.</param>
    /// <returns>
    ///     The ASCII string that was read, with any 0 padding bytes stripped.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public string ReadAscii(int size)
    {
        if (TryReadFixedSingleByteStringFast(size, s_latin1, out string? fastValue))
        {
            return fastValue;
        }

        return ReadFixedSingleByteString(size, s_latin1);
    }


    /// <summary>
    ///     Reads a null-terminated Windows-1252 string from the stream.
    /// </summary>
    /// <returns>
    ///     The ASCII string that was read.
    /// </returns>
    public string ReadWin1252()
    {
        return "Unsupported";
    }

    /// <summary>
    ///     Reads a null-terminated UTF-8 string from the stream.
    /// </summary>
    /// <returns>
    ///     The null-terminated UTF-8 string that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public string ReadUTF8()
    {
        if (TryReadNullTerminatedSingleByteStringFast(Encoding.UTF8, out string? fastValue))
        {
            return fastValue;
        }

        Span<byte> stackBuf = stackalloc byte[256];
        byte[]? rented = null;
        int index = 0;

        try
        {
            while (true)
            {
                int b = _stream.ReadByte();
                if (b < 0)
                {
                    throw new EndOfStreamException();
                }

                if (b == 0)
                {
                    break;
                }

                if (index == stackBuf.Length)
                {
                    var newArr = ArrayPool<byte>.Shared.Rent(stackBuf.Length * 2);
                    stackBuf.CopyTo(newArr);
                    if (rented != null)
                    {
                        ArrayPool<byte>.Shared.Return(rented);
                    }

                    rented = newArr;
                    stackBuf = rented;
                }

                stackBuf[index++] = (byte)b;
            }

            return Encoding.UTF8.GetString(stackBuf[..index]);
        }
        finally
        {
            if (rented != null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    /// <summary>
    ///     Reads a fixed-size null-terminated UTF-8 string from the stream.
    /// </summary>
    /// <param name="size">The size in bytes of the string to read, including the null terminator.</param>
    /// <returns>
    ///     The UTF-8 string that was read, with any padding bytes stripped.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public string ReadUTF8(int size)
    {
        if (TryReadFixedSingleByteStringFast(size, Encoding.UTF8, out string? fastValue))
        {
            return fastValue;
        }

        return ReadFixedSingleByteString(size, Encoding.UTF8);
    }

    /// <summary>
    ///     Reads a null-terminated UTF-16 string from the stream.
    /// </summary>
    /// <returns>
    ///     The UTF-16 string that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public string ReadUTF16()
    {
        if (TryReadNullTerminatedUtf16StringFast(out string? fastValue))
        {
            return fastValue;
        }

        Span<byte> buffer = stackalloc byte[StackBufferSize * sizeof(char)];
        byte[]? rented = null;
        int length = 0;

        try
        {
            while (true)
            {
                if (length + sizeof(ushort) > buffer.Length)
                {
                    byte[] newBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length * 2);
                    buffer[..length].CopyTo(newBuffer);
                    if (rented != null)
                    {
                        ArrayPool<byte>.Shared.Return(rented);
                    }

                    rented = newBuffer;
                    buffer = rented;
                }

                Span<byte> codeUnit = buffer.Slice(length, sizeof(ushort));
                _stream.ReadExactly(codeUnit);
                ushort value = _bigEndian
                    ? BinaryPrimitives.ReadUInt16BigEndian(codeUnit)
                    : BinaryPrimitives.ReadUInt16LittleEndian(codeUnit);

                if (value == 0)
                {
                    break;
                }

                length += sizeof(ushort);
            }

            return GetUtf16Encoding().GetString(buffer[..length]);
        }
        finally
        {
            if (rented != null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    /// <summary>
    ///     Reads a fixed-size null-terminated UTF-16 string from the stream.
    /// </summary>
    /// <param name="size">The size in bytes of the string to read, including the null terminator.</param>
    /// <returns>
    ///     The UTF-16 string that was read, with any padding bytes stripped.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public string ReadUTF16(int size)
    {
        if (TryReadFixedUtf16StringFast(size, out string? fastValue))
        {
            return fastValue;
        }

        return ReadFixedUtf16String(size);
    }

    public string ReadBytesWithEncoding(int size, Encoding encoding)
    {
        byte[] bytes = ReadBlock(size);

        int nullIndex = Array.IndexOf<byte>(bytes, 0);
        if (nullIndex >= 0)
        {
            size = nullIndex;
        }

        return encoding.GetString(bytes, 0, size);
    }


    /// <summary>
    ///     Reads an array of bytes from the stream.
    /// </summary>
    /// <param name="size">The number of bytes to read.</param>
    /// <returns>
    ///     The bytes that were read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public byte[] ReadBlock(int size)
    {
        if (size == 0)
        {
            return Array.Empty<byte>();
        }

        var result = GC.AllocateUninitializedArray<byte>(size);

        if (_memoryBuffer != null)
        {
            ReadBlock(result, 0, size);
            return result;
        }

        _stream.ReadExactly(result.AsSpan());
        return result;
    }

    /// <summary>
    ///     Reads bytes into a span.
    /// </summary>
    /// <param name="buffer">The span to read bytes into.</param>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public void ReadBlock(Span<byte> buffer)
    {
        if (_memoryBuffer != null)
        {
            if (!TryGetFastSourceSpan(buffer.Length, advance: true, out ReadOnlySpan<byte> fastSpan))
            {
                throw new EndOfStreamException($"Attempted to read {buffer.Length} bytes past the end of the stream.");
            }

            fastSpan.CopyTo(buffer);
            return;
        }

        _stream.ReadExactly(buffer);
    }

    /// <summary>
    ///     Reads an array of bytes from the stream.
    /// </summary>
    /// <param name="output">The array to store the read bytes to.</param>
    /// <param name="offset">The starting index in the array to read to.</param>
    /// <param name="size">The number of bytes to read.</param>
    /// <returns>
    ///     The number of bytes that were actually read.
    /// </returns>
    /// <exception cref="IOException">Thrown if there is an issue reading the stream.</exception>
    public int ReadBlock(byte[] output, int offset, int size)
    {
        if (_memoryBuffer != null)
        {
            if (!TryGetFastSourceSpan(size, advance: true, out ReadOnlySpan<byte> fastSpan))
            {
                throw new EndOfStreamException($"Attempted to read {size} bytes past the end of the stream.");
            }

            fastSpan.CopyTo(output.AsSpan(offset, size));
            return size;
        }

        _stream.ReadExactly(output.AsSpan(offset, size));
        return size;
    }

    /// <summary>
    ///     Reads a boolean value from the stream.
    /// </summary>
    /// <returns>
    ///     The boolean value that was read.
    /// </returns>
    /// <exception cref="EndOfStreamException">Thrown if the end of the stream is reached.</exception>
    public bool ReadBoolean()
    {
        byte value = ReadByte();
        return value != 0;
    }

    /// <summary>
    ///     Gets whether or not the stream pointer is at the end of the stream.
    /// </summary>
    public bool EOF
    {
        get { return (Position >= Length); }
    }

    /// <summary>
    ///     Gets the current position of the stream pointer.
    /// </summary>
    public long Position
    {
        get { return _stream.Position; }
    }

    /// <summary>
    ///     Returns a span reader over the next bytes of the stream, at most maxLength of them, without moving this
    ///     reader. Call <see cref="Advance"/> with the span reader's position afterwards.
    /// </summary>
    internal EndianSpanReader CreateSpanReader(int maxLength)
    {
        long position = _stream.Position;
        int length = (int)Math.Min(maxLength, _stream.Length - position);
        if (_memoryBuffer != null)
        {
            return new EndianSpanReader(new ReadOnlySpan<byte>(_memoryBuffer, _memoryBufferOffset + (int)position, length), _bigEndian);
        }

        byte[] bytes = new byte[length];
        _stream.ReadExactly(bytes);
        _stream.Position = position;
        return new EndianSpanReader(bytes, _bigEndian);
    }

    /// <summary>
    ///     Moves the stream position forward by count bytes.
    /// </summary>
    internal void Advance(int count)
    {
        _stream.Position += count;
    }

    /// <summary>
    ///     Gets the length of the stream in bytes.
    /// </summary>
    public long Length
    {
        get { return _stream.Length; }
    }

    /// <summary>
    ///     Gets the base Stream object the stream was constructed from.
    /// </summary>
    public Stream BaseStream
    {
        get { return _stream; }
    }

    private string ReadNullTerminatedSingleByteString(Encoding encoding)
    {
        Span<byte> buffer = stackalloc byte[StackBufferSize];
        byte[]? rented = null;
        int length = 0;

        try
        {
            while (true)
            {
                int value = _stream.ReadByte();
                if (value == -1)
                {
                    throw new EndOfStreamException("Attempted to read past the end of the stream.");
                }

                if (value == 0)
                {
                    break;
                }

                if (length == buffer.Length)
                {
                    byte[] newBuffer = ArrayPool<byte>.Shared.Rent(buffer.Length * 2);
                    buffer[..length].CopyTo(newBuffer);
                    if (rented != null)
                    {
                        ArrayPool<byte>.Shared.Return(rented);
                    }

                    rented = newBuffer;
                    buffer = rented;
                }

                buffer[length++] = (byte)value;
            }

            return encoding.GetString(buffer[..length]);
        }
        finally
        {
            if (rented != null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    private string ReadFixedSingleByteString(int size, Encoding encoding)
    {
        if (size <= 0)
        {
            return string.Empty;
        }

        byte[]? rented = null;
        Span<byte> buffer = size <= StackBufferSize
            ? stackalloc byte[size]
            : (rented = ArrayPool<byte>.Shared.Rent(size)).AsSpan(0, size);

        try
        {
            _stream.ReadExactly(buffer);
            int length = buffer.IndexOf((byte)0);
            if (length < 0)
            {
                length = size;
            }

            return encoding.GetString(buffer[..length]);
        }
        finally
        {
            if (rented != null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    private string ReadFixedUtf16String(int size)
    {
        if (size <= 0)
        {
            return string.Empty;
        }

        byte[]? rented = null;
        Span<byte> buffer = size <= StackBufferSize * sizeof(char)
            ? stackalloc byte[size]
            : (rented = ArrayPool<byte>.Shared.Rent(size)).AsSpan(0, size);

        try
        {
            _stream.ReadExactly(buffer);

            int length = 0;
            for (; length <= size - sizeof(ushort); length += sizeof(ushort))
            {
                ushort value = _bigEndian
                    ? BinaryPrimitives.ReadUInt16BigEndian(buffer.Slice(length, sizeof(ushort)))
                    : BinaryPrimitives.ReadUInt16LittleEndian(buffer.Slice(length, sizeof(ushort)));

                if (value == 0)
                {
                    break;
                }
            }

            return GetUtf16Encoding().GetString(buffer[..length]);
        }
        finally
        {
            if (rented != null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    private Encoding GetUtf16Encoding()
    {
        return _bigEndian ? Encoding.BigEndianUnicode : Encoding.Unicode;
    }

    private bool TryReadFastSpan(int size, out ReadOnlySpan<byte> span)
    {
        if (!TryGetFastSourceSpan(size, advance: true, out span))
        {
            return false;
        }

        return true;
    }

    private bool TryGetFastSourceSpan(int size, bool advance, out ReadOnlySpan<byte> span)
    {
        if (_memoryBuffer == null)
        {
            span = default;
            return false;
        }

        long position = _stream.Position;
        if ((ulong)position + (ulong)size > (ulong)_memoryBufferLength)
        {
            throw new EndOfStreamException($"Attempted to read {size} bytes past the end of the stream.");
        }

        span = new ReadOnlySpan<byte>(_memoryBuffer, _memoryBufferOffset + (int)position, size);

        if (advance)
        {
            _stream.Position = position + size;
        }

        return true;
    }

    private bool TryReadNullTerminatedSingleByteStringFast(Encoding encoding, out string value)
    {
        value = string.Empty;
        if (_memoryBuffer == null)
        {
            return false;
        }

        long position = _stream.Position;
        if ((ulong)position > (ulong)_memoryBufferLength)
        {
            throw new EndOfStreamException("Attempted to read past the end of the stream.");
        }

        ReadOnlySpan<byte> remaining = new(_memoryBuffer, _memoryBufferOffset + (int)position, _memoryBufferLength - (int)position);
        int terminatorIndex = remaining.IndexOf((byte)0);
        if (terminatorIndex < 0)
        {
            throw new EndOfStreamException("Attempted to read past the end of the stream.");
        }

        _stream.Position = position + terminatorIndex + 1L;
        value = encoding.GetString(remaining[..terminatorIndex]);
        return true;
    }

    private bool TryReadFixedSingleByteStringFast(int size, Encoding encoding, out string value)
    {
        value = string.Empty;
        if (!TryGetFastSourceSpan(size, advance: true, out ReadOnlySpan<byte> fastSpan))
        {
            return false;
        }

        int length = fastSpan.IndexOf((byte)0);
        if (length < 0)
        {
            length = size;
        }

        value = encoding.GetString(fastSpan[..length]);
        return true;
    }

    private bool TryReadNullTerminatedUtf16StringFast(out string value)
    {
        value = string.Empty;
        if (_memoryBuffer == null)
        {
            return false;
        }

        long position = _stream.Position;
        if ((ulong)position > (ulong)_memoryBufferLength)
        {
            throw new EndOfStreamException("Attempted to read past the end of the stream.");
        }

        ReadOnlySpan<byte> remaining = new(_memoryBuffer, _memoryBufferOffset + (int)position, _memoryBufferLength - (int)position);
        int byteLength = FindUtf16TerminatorLength(remaining, requireTerminator: true);
        _stream.Position = position + byteLength + sizeof(ushort);
        value = GetUtf16Encoding().GetString(remaining[..byteLength]);
        return true;
    }

    private bool TryReadFixedUtf16StringFast(int size, out string value)
    {
        value = string.Empty;
        if (!TryGetFastSourceSpan(size, advance: true, out ReadOnlySpan<byte> fastSpan))
        {
            return false;
        }

        int byteLength = FindUtf16TerminatorLength(fastSpan, requireTerminator: false);
        value = GetUtf16Encoding().GetString(fastSpan[..byteLength]);
        return true;
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
            throw new EndOfStreamException("Attempted to read past the end of the stream.");
        }

        return buffer.Length & ~1;
    }
}
