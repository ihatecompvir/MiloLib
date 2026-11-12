namespace MiloLib.Utils;

/// <summary>
/// Used to know when a specific file inside the Milo should be chunked.
/// This is needed because apparently if you have giant ass chunks the game will run into memory stability issues.
/// This is verified to match verbatim how Harmonix's original tools did it.
/// </summary>
internal sealed class ChunkMarks
{
    public readonly List<long> marks = new();
    public readonly List<long> dirLoaderMarks = new();
}

/// <summary>
/// Splits a milo body into the blocks listed in its header. The game reads a milo through buffers the size of the
/// largest block, so a block that is much larger than Harmonix's tools would write needs that much contiguous memory.
/// </summary>
internal static class ChunkLayout
{
    /// <summary>
    /// The recommended chunk size.
    /// Harmonix (or I guess Pi Studios / Backbone) targeted a smaller recommended size on Wii only
    /// </summary>
    public const int WiiRecommendedSize = 0x10000;

    /// <summary>
    /// The maximum number of blocks that can exist.
    /// I believe this is technically higher on LEGO? It has a larger chunk header, but they never come close to filling it so it is moot
    /// </summary>
    public const int MaxBlocks = 512;

    private const int SmallBlockSize = 0x2000;

    // plans the chunk sizes how Harmonix themselves do it
    public static List<uint> PlanHarmonix(List<long> marks, long length, int recommendedSize)
    {
        List<long> sortedMarks = new(marks);
        sortedMarks.Sort();

        HarmonixChunkWriter chunkWriter = new(recommendedSize);
        long previousMark = -1;
        foreach (long mark in sortedMarks)
        {
            if (mark != previousMark && mark <= length)
            {
                chunkWriter.MaybeWriteChunk(mark, false);
            }
            previousMark = mark;
        }
        chunkWriter.MaybeWriteChunk(length, true);
        return chunkWriter.blocks;
    }

    // plans the chunk sizes how Harmonix did it before RB3 (cross-referneced with ChunkStream::MaybeWriteChunk)
    // A block ends at the first mark where the block has chunkSize bytes or more
    // The destructor always writes one more block. This block winds up empty if the last mark ended a block, which is technically
    // a bug/oversight, but since we are aiming to produce byte-identical inputs and outputs, it must be preserved
    public static List<uint> PlanPreRB3(List<long> marks, long length, int chunkSize)
    {
        List<long> sortedMarks = new(marks);
        sortedMarks.Sort();

        List<uint> blocks = new();
        long blockStart = 0;
        foreach (long mark in sortedMarks)
        {
            if (mark <= length && mark - blockStart >= chunkSize)
            {
                blocks.Add((uint)(mark - blockStart));
                blockStart = mark;
            }
        }

        blocks.Add((uint)(length - blockStart));
        return blocks;
    }

    private sealed class HarmonixChunkWriter
    {
        public readonly List<uint> blocks = new();
        private readonly int recommendedSize;
        private long bufferStart;
        private long lastWriteMarker;

        public HarmonixChunkWriter(int recommendedSize)
        {
            this.recommendedSize = recommendedSize;
        }

        public void MaybeWriteChunk(long position, bool force)
        {
            long bufferOffset = position - bufferStart;
            if (blocks.Count < 2 && bufferOffset >= SmallBlockSize)
            {
                force = true;
            }

            if (bufferOffset >= recommendedSize || force)
            {
                bool nearlyFull = blocks.Count == MaxBlocks - 1;
                if (!force && nearlyFull)
                {
                    return;
                }

                // end the block at the previous mark, so a large object jus starts a new block instead of joining the smaller ones before it
                if (bufferOffset >= recommendedSize + SmallBlockSize && lastWriteMarker >= SmallBlockSize && !nearlyFull)
                {
                    long previousMark = bufferStart + lastWriteMarker;
                    lastWriteMarker = 0;
                    MaybeWriteChunk(previousMark, true);
                    if (!force)
                    {
                        return;
                    }
                    bufferOffset = position - bufferStart;
                }

                blocks.Add((uint)bufferOffset);
                bufferStart = position;
                bufferOffset = 0;
            }

            lastWriteMarker = bufferOffset;
        }
    }
}
