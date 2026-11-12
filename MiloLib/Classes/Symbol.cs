using MiloLib.Utils;
using System.Buffers;
using System.Collections.Concurrent;
using System.Text;

public class Symbol
{
    private uint length;
    private string chars;

    public string value => chars;

    // Intern cache, it holds a Symbol for each different name that Read found, across all files during the lifetime of the process
    // TODO: there should be a cap on this, because eventually after enough time of opening many milo files in the same process the symbol cache will become massive
    // but its just like Milo itself!
    private static readonly ConcurrentDictionary<ulong, Symbol> _internCache = new();

    // empty sym so we can just point to it anytime we need one instead of allocating a brand new empty symbol
    public static readonly Symbol Empty = new(0, string.Empty);

    public Symbol(uint length, string chars)
    {
        this.length = length;
        this.chars = chars;
    }

    // make a new symbol and skip the cache
    public static implicit operator Symbol(string input)
    {
        return new Symbol((uint)input.Length, input);
    }

    public static implicit operator string(Symbol lengthString)
    {
        return lengthString.chars;
    }

    public override string ToString()
    {
        if (String.IsNullOrEmpty(chars))
        {
            return "N/A";
        }
        else
        {
            return chars;
        }
    }

    public static Symbol Read(EndianReader reader)
    {
        uint length = reader.ReadUInt32();

        // sanity check on symbol length, there is no single instance of a symbol being 512+ across every milo so something absolutely blew up if this is tripped
        if (length > 512)
        {
            throw new InvalidDataException($"Symbol length too high: {length}");
        }

        if (length == 0)
        {
            return Empty;
        }

        byte[]? rented = null;
        Span<byte> buffer = length <= 256 ? stackalloc byte[(int)length] : (rented = ArrayPool<byte>.Shared.Rent((int)length)).AsSpan(0, (int)length);
        try
        {
            reader.ReadBlock(buffer);
            return Intern(buffer);
        }
        finally
        {
            if (rented != null)
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }
    }

    // Gives back the cached Symbol for these bytes, or makes a new Symbol and adds it to the cache if it is not yet cached
    private static Symbol Intern(ReadOnlySpan<byte> bytes)
    {
        ulong key = ComputeKey(bytes);

        // cache hit
        if (_internCache.TryGetValue(key, out Symbol? cached) && SpanMatchesLatin1(bytes, cached.chars))
        {
            return cached;
        }

        // Cache miss, make the sym and try to add it to the cache
        string value = Encoding.Latin1.GetString(bytes);
        Symbol sym = new((uint)bytes.Length, value);

        _internCache.TryAdd(key, sym);
        return sym;
    }

    // fnv-1a
    private static ulong ComputeKey(ReadOnlySpan<byte> bytes)
    {
        ulong hash = 14695981039346656037UL;
        for (int i = 0; i < bytes.Length; i++)
        {
            hash ^= bytes[i];
            hash *= 1099511628211UL;
        }
        return hash ^ (ulong)bytes.Length;
    }

    private static bool SpanMatchesLatin1(ReadOnlySpan<byte> bytes, string s)
    {
        if (bytes.Length != s.Length)
        {
            return false;
        }

        for (int i = 0; i < bytes.Length; i++)
        {
            if (bytes[i] != (byte)s[i])
            {
                return false;
            }
        }
        return true;
    }

    public static void Write(EndianWriter writer, Symbol lengthString)
    {
        int len = lengthString.chars.Length;
        writer.WriteUInt32((uint)len);

        if (len == 0)
        {
            return;
        }

        byte[]? rented = null;
        Span<byte> buffer = len <= 256 ? stackalloc byte[len] : (rented = ArrayPool<byte>.Shared.Rent(len)).AsSpan(0, len);
        try
        {
            Encoding.Latin1.GetBytes(lengthString.chars, buffer);
            writer.WriteBlock(buffer);
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
