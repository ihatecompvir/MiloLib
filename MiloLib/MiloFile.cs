using MiloLib.Assets;
using MiloLib.Utils;
using System.IO.Compression;
using System.Text;

namespace MiloLib;

/// <summary>
/// Represents a Milo scene (with the extension .milo_{platform} most of the time)
/// </summary>
public class MiloFile
{
    /// <summary>
    /// The block size to aim for. From RB3 on, Wii uses half of this size. Pre-RB3 appears to use this size on all platforms.
    /// </summary>
    private const int MAX_BLOCK_SIZE = 0x20000;

    /// <summary>
    /// The first scene revision that the RB3-era ChunkStream writer saves.
    /// </summary>
    private const uint FIRST_CHUNK_REVISION = 28;

    /// <summary>
    /// Set on a ZlibAlt block entry when the block is stored uncompressed.
    /// </summary>
    private const uint STORED_BLOCK_FLAG = 0x01000000;

    /// <summary>
    /// Block entries keep their flags above the size, so a block must be smaller than this.
    /// </summary>
    private const uint BLOCK_SIZE_LIMIT = 0x01000000;

    /// <summary>
    /// The type of the Milo file. Determines if it's compressed or not and how it's compressed.
    /// </summary>
    public enum Type : uint
    {
        /// <summary>
        /// no compression, root dir starts at startOffset
        /// </summary>
        Uncompressed = 0xCABEDEAF,

        /// <summary>
        /// zlib compressed, without uncompressed size before the start of blocks
        /// </summary>
        CompressedZlib = 0xCBBEDEAF,

        /// <summary>
        /// normal gzip
        /// </summary>
        CompressedGzip = 0xCCBEDEAF,

        /// <summary>
        /// zlib compressed, with uncompressed size before the start of blocks
        /// </summary>
        CompressedZlibAlt = 0xCDBEDEAF,
    }

    /// <summary>
    /// The path to the Milo file.
    /// </summary>
    public string? filePath;

    /// <summary>
    /// The Milo's compression type.
    /// </summary>
    public Type compressionType;

    private bool isHeaderless;

    /// <summary>
    /// The offset to the start of the root asset.
    /// </summary>
    private uint startOffset;

    /// <summary>
    /// Gets the starting offset that was read from the file when it was loaded.
    /// Returns 0 if the file was created new (not loaded from disk).
    /// </summary>
    public uint StartOffset => startOffset;

    /// <summary>
    /// The uncompressed size of the largest block in the Milo file.
    /// </summary>
    private uint largestBlock;

    // body size of the file as loaded, used to size the save buffers
    private int loadedBodyLength;

    /// <summary>
    /// The sizes of each block in the Milo file.
    /// </summary>
    private List<uint> blockSizes = new();

    /// <summary>
    /// The root directory and it's metadata such as the entries and string table data.
    /// </summary>
    public DirectoryMeta dirMeta;

    /// <summary>
    /// The endianness of the body. Header is always little endian.
    /// </summary>
    public Endian endian = Endian.BigEndian;

    private readonly struct BufferSegment
    {
        public readonly byte[] data;
        public readonly int offset;
        public readonly int length;

        public BufferSegment(byte[] buffer, int offset, int length)
        {
            data = buffer;
            this.offset = offset;
            this.length = length;
        }

        public void CopyTo(byte[] destination, int destinationOffset)
        {
            Buffer.BlockCopy(data, offset, destination, destinationOffset, length);
        }

        public byte[] ToArray()
        {
            if (length == 0)
            {
                return Array.Empty<byte>();
            }

            byte[] result = GC.AllocateUninitializedArray<byte>(length);
            CopyTo(result, 0);
            return result;
        }
    }

    private static BufferSegment[] BuildBlockSegments(byte[] buffer, int start, IReadOnlyList<uint> sizes)
    {
        BufferSegment[] segments = new BufferSegment[sizes.Count];
        int offset = start;
        for (int i = 0; i < sizes.Count; i++)
        {
            int length = (int)sizes[i];
            segments[i] = new BufferSegment(buffer, offset, length);
            offset += length;
        }

        return segments;
    }

    private static byte[] DecompressBlocks(BufferSegment[] sourceBlocks, bool[]? uncompressedFlags, int[]? decompressedLengths, bool useGzip, int estimatedBlockSize)
    {
        int blockCount = sourceBlocks.Length;
        if (blockCount == 0)
        {
            return Array.Empty<byte>();
        }

        ParallelOptions parallelOptions = new();
        if (blockCount < 3)
        {
            parallelOptions.MaxDegreeOfParallelism = 1;
        }

        if (decompressedLengths != null)
        {
            bool sizesValid = true;
            long total = 0;
            int[] offsets = new int[blockCount];
            for (int i = 0; i < blockCount; i++)
            {
                if (decompressedLengths[i] < 0 || decompressedLengths[i] > Math.Max(estimatedBlockSize, sourceBlocks[i].length))
                {
                    sizesValid = false;
                }
                offsets[i] = (int)total;
                total += decompressedLengths[i];
            }

            if (sizesValid && total <= Array.MaxLength)
            {
                byte[] result = GC.AllocateUninitializedArray<byte>((int)total);
                int mismatches = 0;
                Parallel.For(0, blockCount, parallelOptions, i =>
                {
                    Span<byte> destination = result.AsSpan(offsets[i], decompressedLengths[i]);
                    BufferSegment block = sourceBlocks[i];
                    if (uncompressedFlags != null && uncompressedFlags[i])
                    {
                        if (block.length != destination.Length)
                        {
                            Interlocked.Increment(ref mismatches);
                            return;
                        }
                        block.data.AsSpan(block.offset, block.length).CopyTo(destination);
                        return;
                    }

                    if (!InflateExactly(block, useGzip, destination))
                    {
                        Interlocked.Increment(ref mismatches);
                    }
                });

                if (mismatches == 0)
                {
                    return result;
                }
            }
        }

        byte[][] inflated = new byte[blockCount][];
        int[] lengths = new int[blockCount];
        Parallel.For(0, blockCount, parallelOptions, i =>
        {
            BufferSegment block = sourceBlocks[i];
            if (uncompressedFlags != null && uncompressedFlags[i])
            {
                inflated[i] = block.data;
                lengths[i] = block.length;
                return;
            }

            inflated[i] = Inflate(block, useGzip, estimatedBlockSize, out lengths[i]);
        });

        if (blockCount == 1 && (uncompressedFlags == null || !uncompressedFlags[0]) && inflated[0].Length == lengths[0])
        {
            return inflated[0];
        }

        long combinedLength = 0;
        foreach (int length in lengths)
        {
            combinedLength += length;
        }

        byte[] combined = GC.AllocateUninitializedArray<byte>((int)combinedLength);
        int destinationOffset = 0;
        for (int i = 0; i < blockCount; i++)
        {
            int sourceOffset = 0;
            if (uncompressedFlags != null && uncompressedFlags[i])
            {
                sourceOffset = sourceBlocks[i].offset;
            }
            Buffer.BlockCopy(inflated[i], sourceOffset, combined, destinationOffset, lengths[i]);
            destinationOffset += lengths[i];
        }

        return combined;
    }

    private static Stream OpenInflater(BufferSegment block, bool useGzip)
    {
        MemoryStream source = new(block.data, block.offset, block.length, writable: false);
        if (useGzip)
        {
            return new GZipStream(source, CompressionMode.Decompress);
        }

        return new DeflateStream(source, CompressionMode.Decompress);
    }

    private static byte[] Inflate(BufferSegment block, bool useGzip, int capacity, out int length)
    {
        using Stream inflater = OpenInflater(block, useGzip);
        byte[] buffer = GC.AllocateUninitializedArray<byte>(Math.Max(capacity, 4096));
        length = 0;
        while (true)
        {
            if (length == buffer.Length)
            {
                int next = inflater.ReadByte();
                if (next < 0)
                {
                    break;
                }
                Array.Resize(ref buffer, buffer.Length * 2);
                buffer[length] = (byte)next;
                length++;
            }

            int read = inflater.Read(buffer, length, buffer.Length - length);
            if (read == 0)
            {
                break;
            }
            length += read;
        }

        return buffer;
    }

    private static bool InflateExactly(BufferSegment block, bool useGzip, Span<byte> destination)
    {
        using Stream inflater = OpenInflater(block, useGzip);
        int filled = 0;
        while (filled < destination.Length)
        {
            int read = inflater.Read(destination.Slice(filled));
            if (read == 0)
            {
                return false;
            }
            filled += read;
        }

        return inflater.ReadByte() < 0;
    }

    /// <summary>
    /// Loads a Milo file from a file path.
    /// </summary>
    public MiloFile(string path)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        filePath = path;

        // buffer entire file into memory first
        byte[] fileBuffer = File.ReadAllBytes(path);
        LoadFromBuffer(fileBuffer);
    }

    /// <summary>
    /// Loads a Milo file from an in-memory byte buffer.
    /// </summary>
    /// <param name="fileBuffer">The full contents of the Milo file.</param>
    /// <param name="sourcePath">Optional source path used for platform detection.</param>
    public MiloFile(byte[] fileBuffer, string? sourcePath = null)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        filePath = sourcePath;
        LoadFromBuffer(fileBuffer);
    }

    private void LoadFromBuffer(byte[] fileBuffer)
    {
        isHeaderless = false;
        startOffset = 0;
        largestBlock = 0;
        blockSizes = new List<uint>();

        using EndianReader reader = new(fileBuffer, Endian.LittleEndian);
        {
            compressionType = (Type)reader.ReadUInt32();

            // detect if the type is one of the compressed types
            if (compressionType != Type.Uncompressed && compressionType != Type.CompressedZlib && compressionType != Type.CompressedGzip && compressionType != Type.CompressedZlibAlt)
            {
                isHeaderless = true;
                // this might be a headerless milo (e.g. Phase .milo_pc) so treat it as such and just start reading the root directory
                reader.SeekTo(0);
                reader.Endianness = Endian.BigEndian;
                DirectoryMeta meta = new();
                if (!string.IsNullOrEmpty(filePath))
                {
                    meta.platform = DetectPlatform();
                }

                dirMeta = meta.Read(reader);
                loadedBodyLength = fileBuffer.Length;
                endian = reader.Endianness;
                return;
            }

            startOffset = reader.ReadUInt32();

            uint numBlocks = reader.ReadUInt32();

            largestBlock = reader.ReadUInt32();

            for (int i = 0; i < numBlocks; i++)
            {
                blockSizes.Add(reader.ReadUInt32());
            }

            switch (compressionType)
            {
                case Type.CompressedZlib:
                    {
                        BufferSegment[] rawBlocks = BuildBlockSegments(fileBuffer, (int)startOffset, blockSizes);
                        byte[] decompressedBuffer = DecompressBlocks(rawBlocks, null, null, useGzip: false, (int)largestBlock);
                        using EndianReader decompressedReader = new(decompressedBuffer, Endian.BigEndian);

                        DirectoryMeta meta = new();
                        meta.platform = DetectPlatform();
                        dirMeta = meta.Read(decompressedReader);
                        loadedBodyLength = (int)decompressedReader.BaseStream.Length;
                        endian = decompressedReader.Endianness;
                        break;
                    }
                case Type.CompressedZlibAlt:
                    {
                        BufferSegment[] rawBlocks = new BufferSegment[numBlocks];
                        bool[] blockUncompressed = new bool[numBlocks];
                        int[] decompressedLengths = new int[numBlocks];
                        bool allUncompressed = true;
                        int rawOffset = (int)startOffset;
                        for (int i = 0; i < numBlocks; i++)
                        {
                            blockUncompressed[i] = (blockSizes[i] & 0xFF000000) != 0;

                            if (blockUncompressed[i])
                            {
                                blockSizes[i] &= 0x00FFFFFF;
                                rawBlocks[i] = new BufferSegment(fileBuffer, rawOffset, (int)blockSizes[i]);
                                decompressedLengths[i] = (int)blockSizes[i];
                                rawOffset += (int)blockSizes[i];
                            }
                            else
                            {
                                // each compressed block starts with its decompressed size
                                allUncompressed = false;
                                decompressedLengths[i] = BitConverter.ToInt32(fileBuffer, rawOffset);
                                rawOffset += 4;
                                rawBlocks[i] = new BufferSegment(fileBuffer, rawOffset, (int)blockSizes[i] - 4);
                                rawOffset += (int)blockSizes[i] - 4;
                            }
                        }

                        // stored blocks sit back to back in the file, so read them in place
                        MemoryStream bodyStream;
                        if (allUncompressed)
                        {
                            bodyStream = new MemoryStream(fileBuffer, (int)startOffset, rawOffset - (int)startOffset, writable: false, publiclyVisible: true);
                        }
                        else
                        {
                            byte[] decompressedBuffer = DecompressBlocks(rawBlocks, blockUncompressed, decompressedLengths, useGzip: false, (int)largestBlock);
                            bodyStream = new MemoryStream(decompressedBuffer, 0, decompressedBuffer.Length, writable: false, publiclyVisible: true);
                        }
                        using EndianReader decompressedReader = new(bodyStream, Endian.BigEndian);

                        DirectoryMeta meta = new();
                        meta.platform = DetectPlatform();
                        dirMeta = meta.Read(decompressedReader);
                        loadedBodyLength = (int)decompressedReader.BaseStream.Length;
                        endian = decompressedReader.Endianness;
                        break;
                    }
                case Type.CompressedGzip:
                    {
                        BufferSegment[] rawBlocks = BuildBlockSegments(fileBuffer, (int)startOffset, blockSizes);
                        byte[] decompressedBuffer = DecompressBlocks(rawBlocks, null, null, useGzip: true, (int)largestBlock);
                        using EndianReader decompressedReader = new(decompressedBuffer, Endian.BigEndian);

                        DirectoryMeta meta = new();
                        meta.platform = DetectPlatform();
                        dirMeta = meta.Read(decompressedReader);
                        loadedBodyLength = (int)decompressedReader.BaseStream.Length;
                        endian = decompressedReader.Endianness;
                        break;
                    }
                case Type.Uncompressed:
                    {
                        reader.SeekTo(startOffset);

                        reader.Endianness = Endian.BigEndian;

                        DirectoryMeta meta = new();
                        meta.platform = DetectPlatform();
                        dirMeta = meta.Read(reader);
                        loadedBodyLength = fileBuffer.Length - (int)startOffset;
                        endian = reader.Endianness;
                        break;
                    }
                default:
                    break;
            }

            System.Diagnostics.Debug.WriteLine("Done reading Milo file " + filePath);
        }
    }

    private DirectoryMeta.Platform DetectPlatform()
    {
        // detect the platform from the file extension
        string extension = Path.GetExtension(filePath);
        if (extension == null)
        {
            throw new Exception("Could not detect platform from file extension");
        }

        if (extension.EndsWith("_ps2"))
        {
            return DirectoryMeta.Platform.PS2;
        }

        if (extension.EndsWith("_xbox"))
        {
            return DirectoryMeta.Platform.Xbox;
        }

        if (extension.EndsWith("_pc"))
        {
            return DirectoryMeta.Platform.PC_or_iPod;
        }

        if (extension.EndsWith("_wii"))
        {
            return DirectoryMeta.Platform.Wii;
        }

        if (extension.EndsWith("_ps3"))
        {
            return DirectoryMeta.Platform.PS3;
        }

        if (extension.EndsWith("_gc"))
        {
            return DirectoryMeta.Platform.GameCube;
        }

        return DirectoryMeta.Platform.PS3;
    }

    /// <summary>
    /// Constructs a new MiloFile.
    /// </summary>
    /// <param name="meta">The root directory to create the MiloFile with.</param>
    public MiloFile(DirectoryMeta meta)
    {
        this.compressionType = Type.Uncompressed;
        this.dirMeta = meta;
        this.startOffset = 0x810;
        this.blockSizes = new List<uint>();
    }

    /// <summary>
    /// Saves a Milo scene to disk.
    /// </summary>
    /// <param name="path">The path at which the Milo scene will be saved.</param>
    /// <param name="type">The compression type to use.</param>
    /// <param name="startingOffset">The offset at which the root directory starts. If null, uses the original startOffset from the loaded file, or 0x810 for new files.</param>
    /// <param name="headerEndian">The endianness of the header.</param>
    /// <param name="bodyEndian">The endianness of the body. Certain games require little endian bodies, such as GH2.</param>
    public void Save(string? path, Type? type, uint? startingOffset = null, Endian headerEndian = Endian.LittleEndian, Endian bodyEndian = Endian.BigEndian)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        if (path == null)
        {
            path = filePath;
        }

        using FileStream fileStream = new(path!, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
        using BufferedStream bufferedStream = new(fileStream, 65536);
        SaveCore(bufferedStream, type, startingOffset, headerEndian, bodyEndian);
    }

    private void Save(Stream stream, Type? type, uint? startingOffset = null, Endian headerEndian = Endian.LittleEndian, Endian bodyEndian = Endian.BigEndian)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        SaveCore(stream, type, startingOffset, headerEndian, bodyEndian);
    }

    public byte[] SaveToBytes(Type? type, uint? startingOffset = null, Endian headerEndian = Endian.LittleEndian, Endian bodyEndian = Endian.BigEndian)
    {
        using MemoryStream stream = new();
        Save(stream, type, startingOffset, headerEndian, bodyEndian);

        int length = (int)stream.Length;
        if (length == 0)
        {
            return Array.Empty<byte>();
        }

        if (stream.TryGetBuffer(out ArraySegment<byte> segment) && segment.Array != null)
        {
            // the stream was sized exactly, so its buffer is the result
            if (segment.Offset == 0 && segment.Array.Length == length)
            {
                return segment.Array;
            }

            byte[] bytes = GC.AllocateUninitializedArray<byte>(length);
            Buffer.BlockCopy(segment.Array, segment.Offset, bytes, 0, length);
            return bytes;
        }

        return stream.ToArray();
    }

    private void SaveCore(Stream stream, Type? type, uint? startingOffset, Endian headerEndian, Endian bodyEndian)
    {
        if (!stream.CanWrite)
        {
            throw new NotSupportedException("Cannot save Milo data to a non-writable stream.");
        }

        if (!stream.CanSeek)
        {
            throw new NotSupportedException("Cannot save Milo data to a non-seekable stream.");
        }

        endian = bodyEndian;
        stream.SetLength(0);
        stream.Position = 0;

        uint actualStartingOffset = startingOffset ?? (startOffset != 0 ? startOffset : 0x810);
        Type effectiveType = type ?? compressionType;

        if (effectiveType != Type.Uncompressed && effectiveType != Type.CompressedZlib && effectiveType != Type.CompressedGzip && effectiveType != Type.CompressedZlibAlt)
        {
            if (!isHeaderless || effectiveType != compressionType)
            {
                throw new NotSupportedException($"Unsupported Milo compression type {effectiveType}.");
            }

            EndianWriter bodyWriter = new(stream, bodyEndian);
            dirMeta.Write(bodyWriter);
            return;
        }

        EndianWriter writer = new(stream, headerEndian);
        writer.WriteUInt32((uint)effectiveType);
        writer.WriteUInt32(actualStartingOffset);
        writer.WriteUInt32(1);

        writer.WriteUInt32(0);
        writer.WriteUInt32(0);
        writer.WriteZeroes((int)actualStartingOffset - (int)writer.Position);

        writer.Endianness = bodyEndian;

        // start at the loaded body size (plus a bit of room for edits) so the buffer rarely has to grow
        int initialCapacity = Math.Max(MAX_BLOCK_SIZE, loadedBodyLength + loadedBodyLength / 8);
        EndianWriter uncompressedWriter = new(bodyEndian, initialCapacity: initialCapacity);
        ChunkMarks chunkMarks = new();
        uncompressedWriter.ChunkMarks = chunkMarks;
        dirMeta.Write(uncompressedWriter);
        uncompressedWriter.ChunkMarks = null;

        List<uint> uncompressedBlockSizes = PlanBlocks(chunkMarks, uncompressedWriter.Length);
        int headerCapacity = ((int)actualStartingOffset - 16) / 4;
        if (uncompressedBlockSizes.Count > headerCapacity)
        {
            throw new InvalidOperationException($"The Milo scene needs {uncompressedBlockSizes.Count} blocks, but a header that starts the body at 0x{actualStartingOffset:X} only has room for {headerCapacity}.");
        }

        int[] blockOffsets = new int[uncompressedBlockSizes.Count];
        int runningOffset = 0;
        for (int i = 0; i < uncompressedBlockSizes.Count; i++)
        {
            if (uncompressedBlockSizes[i] >= BLOCK_SIZE_LIMIT)
            {
                throw new InvalidOperationException($"Block {i} of the Milo scene is {uncompressedBlockSizes[i]} bytes, but a block must be smaller than 0x{BLOCK_SIZE_LIMIT:X} bytes.");
            }

            blockOffsets[i] = runningOffset;
            runningOffset += (int)uncompressedBlockSizes[i];
        }

        switch (effectiveType)
        {
            case Type.Uncompressed:
                if (stream is MemoryStream memoryStream)
                {
                    long finalLength = memoryStream.Position + uncompressedWriter.Length;
                    if (memoryStream.Capacity < finalLength)
                    {
                        memoryStream.Capacity = (int)finalLength;
                    }
                }
                writer.WriteBlock(uncompressedWriter.WrittenSpan);
                writer.SeekTo(0x8);
                writer.Endianness = headerEndian;
                writer.WriteUInt32((uint)uncompressedBlockSizes.Count);
                writer.WriteUInt32(uncompressedBlockSizes.Max());

                foreach (uint blockSize in uncompressedBlockSizes)
                {
                    writer.WriteUInt32(blockSize);
                }
                break;
            case Type.CompressedZlib:
            case Type.CompressedGzip:
                {
                    byte[] uncompressedBuffer = uncompressedWriter.ToArray();
                    byte[][] compressedBlocks = new byte[uncompressedBlockSizes.Count][];
                    Parallel.For(0, uncompressedBlockSizes.Count, i =>
                    {
                        compressedBlocks[i] = CompressBlock(uncompressedBuffer, blockOffsets[i], (int)uncompressedBlockSizes[i], effectiveType == Type.CompressedGzip);
                    });

                    // the game reads a compressed block into the end of a buffer of the largest size plus 0x800, so the largest size covers both forms
                    uint maxBlockSize = 0;
                    for (int i = 0; i < compressedBlocks.Length; i++)
                    {
                        writer.WriteBlock(compressedBlocks[i]);
                        maxBlockSize = Math.Max(maxBlockSize, Math.Max(uncompressedBlockSizes[i], (uint)compressedBlocks[i].Length));
                    }

                    writer.SeekTo(0x8);
                    writer.Endianness = headerEndian;
                    writer.WriteUInt32((uint)compressedBlocks.Length);
                    writer.WriteUInt32(maxBlockSize);
                    foreach (var block in compressedBlocks)
                    {
                        writer.WriteUInt32((uint)block.Length);
                    }
                    break;
                }
            case Type.CompressedZlibAlt:
                {
                    byte[] uncompressedBuffer = uncompressedWriter.ToArray();
                    byte[]?[] compressedBlocks = new byte[uncompressedBlockSizes.Count][];

                    // the first block is always stored, like ChunkStream::WriteChunk does
                    Parallel.For(1, uncompressedBlockSizes.Count, i =>
                    {
                        byte[] compressed = CompressBlock(uncompressedBuffer, blockOffsets[i], (int)uncompressedBlockSizes[i], false);

                        // ChunkStream::WriteChunk seems to only keeps a block compressed when that makes it more than 10% smaller
                        if ((float)uncompressedBlockSizes[i] / (float)compressed.Length > 1.1f)
                        {
                            compressedBlocks[i] = compressed;
                        }
                    });

                    uint[] blockEntries = new uint[compressedBlocks.Length];
                    uint maxBlockSize = 0;
                    for (int i = 0; i < compressedBlocks.Length; i++)
                    {
                        uint blockSize = uncompressedBlockSizes[i];
                        byte[]? compressed = compressedBlocks[i];
                        uint writtenSize;
                        if (compressed == null)
                        {
                            writer.WriteBlock(uncompressedBuffer, blockOffsets[i], (int)blockSize);
                            writtenSize = blockSize;
                            blockEntries[i] = blockSize | STORED_BLOCK_FLAG;
                        }
                        else
                        {
                            // a compressed block starts with its uncompressed size, in the header's byte order
                            writer.Endianness = headerEndian;
                            writer.WriteUInt32(blockSize);
                            writer.Endianness = bodyEndian;
                            writer.WriteBlock(compressed);
                            writtenSize = (uint)compressed.Length + 4;
                            blockEntries[i] = writtenSize;
                        }
                        maxBlockSize = Math.Max(maxBlockSize, Math.Max(blockSize, writtenSize));
                    }

                    writer.SeekTo(0x8);
                    writer.Endianness = headerEndian;
                    writer.WriteUInt32((uint)compressedBlocks.Length);
                    writer.WriteUInt32(maxBlockSize);
                    foreach (uint blockEntry in blockEntries)
                    {
                        writer.WriteUInt32(blockEntry);
                    }
                    break;
                }
        }

        stream.Flush();
    }

    /// <summary>
    /// Splits the saved body into blocks at the places recorded while it was written.
    /// </summary>
    private List<uint> PlanBlocks(ChunkMarks chunkMarks, long bodyLength)
    {
        if (dirMeta.revision < FIRST_CHUNK_REVISION)
        {
            return ChunkLayout.PlanPreRB3(chunkMarks.dirLoaderMarks, bodyLength, MAX_BLOCK_SIZE);
        }

        int recommendedSize = MAX_BLOCK_SIZE;
        if (dirMeta.platform == DirectoryMeta.Platform.Wii)
        {
            recommendedSize = ChunkLayout.WiiRecommendedSize;
        }
        return ChunkLayout.PlanHarmonix(chunkMarks.marks, bodyLength, recommendedSize);
    }

    /// <summary>
    /// A raw deflate stream with no data, as zlib writes it (Harmonix had empty blocks in the milos which .NETs compression would just otherwise drop)
    /// </summary>
    private static readonly byte[] EmptyDeflateBlock = { 0x03, 0x00 };

    /// <summary>
    /// A gzip member with no data.
    /// </summary>
    private static readonly byte[] EmptyGzipBlock = { 0x1F, 0x8B, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02, 0x0A, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    private static byte[] CompressBlock(byte[] buffer, int offset, int length, bool useGzip)
    {
        // .NET writes no bytes for an empty block. The game must get a stream that it can inflate.
        if (length == 0)
        {
            if (useGzip)
            {
                return EmptyGzipBlock;
            }
            return EmptyDeflateBlock;
        }

        using MemoryStream blockStream = new();
        if (useGzip)
        {
            using GZipStream gzipStream = new(blockStream, CompressionLevel.SmallestSize, leaveOpen: true);
            gzipStream.Write(buffer, offset, length);
            gzipStream.Close();
        }
        else
        {
            using DeflateStream deflater = new(blockStream, CompressionLevel.SmallestSize, leaveOpen: true);
            deflater.Write(buffer, offset, length);
            deflater.Close();
        }
        return blockStream.ToArray();
    }

    public override string ToString()
    {
        return string.Format("{0}", filePath);
    }
}
