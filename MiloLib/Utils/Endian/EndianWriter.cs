// originally taken from https://github.com/XboxChaos/Assembly, thanks for originally writing this code
// license in Utils/Endian/LICENSE.md

using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace MiloLib.Utils;

/// <summary>
///     A stream which can be written to and whose endianness can be changed.
/// </summary>
public class EndianWriter : IWriter, IDisposable
{
    private const int StackBufferSize = 256;
    private static readonly Encoding s_latin1 = Encoding.Latin1;
    private static readonly byte[] s_zeroes = new byte[256];
    private readonly byte[] _buffer = new byte[8];
    private readonly Stream? _stream;
    private readonly EndianMemoryWriter? _memoryWriter;
    private readonly Stream? _memoryBackedStream;
    private bool _bigEndian;
    private ChunkMarks? _chunkMarks;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EndianWriter" /> class.
    /// </summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="endianness">The initial endianness to use when writing to the stream.</param>
    public EndianWriter(Stream stream, Endian endianness)
    {
        _stream = stream;
        _bigEndian = (endianness == Endian.BigEndian);
    }

    public EndianWriter(Endian endianness, int initialCapacity = 256)
    {
        _memoryWriter = new EndianMemoryWriter(endianness, initialCapacity);
        _memoryBackedStream = new MemoryWriterStream(_memoryWriter);
        _bigEndian = endianness == Endian.BigEndian;
    }

    /// <summary>
    ///     Gets or sets the endianness used when reading/writing to/from the stream.
    /// </summary>
    public Endian Endianness
    {
        get { return _bigEndian ? Endian.BigEndian : Endian.LittleEndian; }
        set
        {
            _bigEndian = (value == Endian.BigEndian);
            if (_memoryWriter != null)
            {
                _memoryWriter.Endianness = value;
            }
        }
    }

    /// <summary>
    ///     Writes a byte to the stream.
    /// </summary>
    /// <param name="value">The byte to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteByte(byte value)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteByte(value);
            return;
        }

        _buffer[0] = value;
        _stream!.Write(_buffer.AsSpan(0, sizeof(byte)));
    }

    /// <summary>
    ///     Writes a signed byte to the stream.
    /// </summary>
    /// <param name="value">The signed byte to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteSByte(sbyte value)
    {
        WriteByte((byte)value);
    }

    /// <summary>
    ///     Writes an unsigned 16-bit integer to the stream.
    /// </summary>
    /// <param name="value">The unsigned 16-bit integer to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteUInt16(ushort value)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteUInt16(value);
            return;
        }

        if (_bigEndian)
        {
            BinaryPrimitives.WriteUInt16BigEndian(_buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt16LittleEndian(_buffer, value);
        }

        _stream!.Write(_buffer.AsSpan(0, sizeof(ushort)));
    }

    /// <summary>
    ///     Writes a signed 16-bit integer to the stream.
    /// </summary>
    /// <param name="value">The signed 16-bit integer to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteInt16(short value)
    {
        WriteUInt16((ushort)value);
    }

    /// <summary>
    ///     Writes an unsigned 32-bit integer to the stream.
    /// </summary>
    /// <param name="value">The unsigned 32-bit integer to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteUInt32(uint value)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteUInt32(value);
            return;
        }

        if (_bigEndian)
        {
            BinaryPrimitives.WriteUInt32BigEndian(_buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt32LittleEndian(_buffer, value);
        }

        _stream!.Write(_buffer.AsSpan(0, sizeof(uint)));
    }

    /// <summary>
    ///     Writes a signed 32-bit integer to the stream.
    /// </summary>
    /// <param name="value">The signed 32-bit integer to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteInt32(int value)
    {
        WriteUInt32((uint)value);
    }

    /// <summary>
    ///     Writes an unsigned 64-bit integer to the stream.
    /// </summary>
    /// <param name="value">The unsigned 64-bit integer to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteUInt64(ulong value)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteUInt64(value);
            return;
        }

        if (_bigEndian)
        {
            BinaryPrimitives.WriteUInt64BigEndian(_buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteUInt64LittleEndian(_buffer, value);
        }

        _stream!.Write(_buffer.AsSpan(0, sizeof(ulong)));
    }

    /// <summary>
    ///     Writes a signed 64-bit integer to the stream.
    /// </summary>
    /// <param name="value">The signed 64-bit integer to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteInt64(long value)
    {
        WriteUInt64((ulong)value);
    }

    /// <summary>
    ///     Writes a 16-bit floating-point value (half-precision) to the stream.
    /// </summary>
    /// <param name="value">The 16-bit floating point value to write.</param>
    /// <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteHalfFloat(float value)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteHalfFloat(value);
            return;
        }

        Half half = (Half)value;
        if (_bigEndian)
        {
            BinaryPrimitives.WriteHalfBigEndian(_buffer, half);
        }
        else
        {
            BinaryPrimitives.WriteHalfLittleEndian(_buffer, half);
        }

        _stream!.Write(_buffer.AsSpan(0, sizeof(ushort)));
    }

    /// <summary>
    ///     Writes a 32-bit floating-point value to the stream.
    /// </summary>
    /// <param name="value">The 32-bit floating-point value to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteFloat(float value)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteFloat(value);
            return;
        }

        if (_bigEndian)
        {
            BinaryPrimitives.WriteSingleBigEndian(_buffer, value);
        }
        else
        {
            BinaryPrimitives.WriteSingleLittleEndian(_buffer, value);
        }

        _stream!.Write(_buffer.AsSpan(0, sizeof(float)));
    }

    /// <summary>
    ///     Writes a boolean value to the stream.
    /// </summary>
    /// <param name="value">The boolean value to write.</param>
    /// <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteBoolean(bool value)
    {
        WriteByte(value ? (byte)1 : (byte)0);
    }

    /// <summary>
    ///     Writes an ASCII string to the stream, followed by a null terminator.
    /// </summary>
    /// <param name="str">The ASCII string to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteAscii(string str)
    {
        WriteEncodedString(str.AsSpan(), s_latin1);
        WriteByte(0);
    }

    /// <summary>
    ///     Writes an ASCII string to the stream and pads it with trailing null bytes to the specified length.
    /// </summary>
    /// <param name="str"></param>
    /// <param name="length"></param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteAscii(string str, int length)
    {
        int byteCount = Encoding.ASCII.GetByteCount(str);
        if (byteCount >= length)
        {
            throw new ArgumentException("The strings exceeds the specified length.");
        }

        WriteEncodedString(str.AsSpan(), Encoding.ASCII);
        WriteZeroes(length - byteCount);
    }

    /// <summary>
    ///     Writes a Windows-1252 string to the stream, followed by a null terminator.
    /// </summary>
    /// <param name="str">The Windows-1252 string to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteWin1252(string str)
    {
        WriteEncodedString(str.AsSpan(), Encoding.GetEncoding(1252));
        WriteByte(0);
    }

    /// <summary>
    ///     Writes a UTF-8 string to the stream, followed by a null terminator.
    /// </summary>
    /// <param name="str">The UTF-8 string to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteUTF8(string str)
    {
        WriteEncodedString(str.AsSpan(), Encoding.UTF8);
        WriteByte(0);
    }

    /// <summary>
    ///     Writes a UTF-16 string to the stream, followed by a null terminator.
    /// </summary>
    /// <param name="str">The UTF-16 string to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteUTF16(string str)
    {
        WriteEncodedString(str.AsSpan(), _bigEndian ? Encoding.BigEndianUnicode : Encoding.Unicode);
        WriteInt16(0x0000);
    }

    /// <summary>
    ///     Writes an array of bytes to the stream.
    /// </summary>
    /// <param name="data">The bytes to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteBlock(byte[] data)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteBlock(data);
            return;
        }

        _stream!.Write(data);
    }

    /// <summary>
    ///     Writes a span of bytes to the stream.
    /// </summary>
    /// <param name="buffer">The span to write.</param>
    public void WriteBlock(ReadOnlySpan<byte> buffer)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteBlock(buffer);
            return;
        }

        _stream!.Write(buffer);
    }

    private static readonly byte[] _endBytes = { 0xAD, 0xDE, 0xAD, 0xDE };

    /// <summary>
    ///     Writes the Milo end-of-object marker bytes (0xADDEADDE) to the stream.
    /// </summary>
    public void WriteEndBytes()
    {
        WriteBlock(_endBytes);
    }

    /// <summary>
    ///     Writes an array of bytes to the stream.
    /// </summary>
    /// <param name="data">The bytes to write.</param>
    /// <param name="offset">The starting index in the array to write.</param>
    /// <param name="size">The number of bytes to write.</param>
    ///  <exception cref="IOException">Thrown if there is an issue writing to the stream.</exception>
    public void WriteBlock(byte[] data, int offset, int size)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteBlock(data, offset, size);
            return;
        }

        _stream!.Write(data.AsSpan(offset, size));
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

        if (_memoryWriter != null)
        {
            if (offset > int.MaxValue)
            {
                return false;
            }

            _memoryWriter.SeekTo((int)offset);
            return true;
        }

        _stream!.Seek(offset, SeekOrigin.Begin);
        return true;
    }

    /// <summary>
    ///     Skips over a number of bytes in the stream.
    /// </summary>
    /// <param name="count">The number of bytes to skip.</param>
    public void Skip(long count)
    {
        if (_memoryWriter != null)
        {
            if (count > int.MaxValue || count < int.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            _memoryWriter.Skip((int)count);
            return;
        }

        _stream!.Seek(count, SeekOrigin.Current);
    }

    /// <summary>
    ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        _stream?.Dispose();
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
        get { return _memoryWriter?.Position ?? _stream!.Position; }
    }

    /// <summary>
    ///     Gets the length of the stream in bytes.
    /// </summary>
    public long Length
    {
        get { return _memoryWriter?.Length ?? _stream!.Length; }
    }

    /// <summary>
    ///     The places where a block of a saved milo may end, or null when this writer is not recording them.
    /// </summary>
    internal ChunkMarks? ChunkMarks
    {
        get { return _chunkMarks; }
        set { _chunkMarks = value; }
    }

    /// <summary>
    ///     Records the current position as a place where a block may end, like Harmonix's MarkChunk.
    /// </summary>
    internal void MarkChunk()
    {
        if (_chunkMarks != null)
        {
            _chunkMarks.marks.Add(Position);
        }
    }

    /// <summary>
    ///     Records the current position as the end of a directory entry, which is also a place where a block may end.
    /// </summary>
    internal void MarkEntryEnd()
    {
        if (_chunkMarks != null)
        {
            _chunkMarks.marks.Add(Position);
            _chunkMarks.entryEnds.Add(Position);
        }
    }

    /// <summary>
    ///     Records the places where a block may end inside data the game reads in pieces of pieceSize bytes: after
    ///     every piece from start, and at end.
    /// </summary>
    internal void MarkChunks(long start, long end, int pieceSize)
    {
        if (_chunkMarks == null || end <= start)
        {
            return;
        }

        for (long position = start + pieceSize; position < end; position += pieceSize)
        {
            _chunkMarks.marks.Add(position);
        }
        _chunkMarks.marks.Add(end);
    }

    /// <summary>
    ///     Gets the base Stream object the stream was constructed from.
    /// </summary>
    public Stream BaseStream
    {
        get { return _stream ?? _memoryBackedStream!; }
    }

    public ReadOnlySpan<byte> WrittenSpan
    {
        get
        {
            if (_memoryWriter == null)
            {
                throw new InvalidOperationException("WrittenSpan is only available for memory-backed writers.");
            }

            return _memoryWriter.WrittenSpan;
        }
    }

    public byte[] ToArray()
    {
        if (_memoryWriter == null)
        {
            throw new InvalidOperationException("ToArray is only available for memory-backed writers.");
        }

        return _memoryWriter.ToArray();
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

    public void WriteZeroes(int count)
    {
        if (_memoryWriter != null)
        {
            _memoryWriter.WriteZeroes(count);
            return;
        }

        while (count > 0)
        {
            int chunk = Math.Min(count, s_zeroes.Length);
            _stream!.Write(s_zeroes.AsSpan(0, chunk));
            count -= chunk;
        }
    }

    private sealed class MemoryWriterStream : Stream
    {
        private readonly EndianMemoryWriter _writer;

        public MemoryWriterStream(EndianMemoryWriter writer)
        {
            _writer = writer;
        }

        public override bool CanRead => false;
        public override bool CanSeek => true;
        public override bool CanWrite => true;
        public override long Length => _writer.Length;

        public override long Position
        {
            get => _writer.Position;
            set
            {
                if (value > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(value));
                }

                _writer.Position = (int)value;
            }
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            long target = origin switch
            {
                SeekOrigin.Begin => offset,
                SeekOrigin.Current => Position + offset,
                SeekOrigin.End => Length + offset,
                _ => throw new ArgumentOutOfRangeException(nameof(origin)),
            };

            Position = target;
            return Position;
        }

        public override void SetLength(long value)
        {
            if (value > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _writer.SetLength((int)value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _writer.WriteBlock(buffer, offset, count);
        }

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            _writer.WriteBlock(buffer);
        }
    }
}
