using MiloLib.Assets;

namespace MiloLib.Utils.Conversion;

public enum PlatformConversionSeverity
{
    Info,
    Warning,
    Error
}

public sealed record PlatformConversionMessage(PlatformConversionSeverity Severity, string Message, string? EntryName = null, string? EntryType = null);

public sealed class PlatformConversionReport
{
    private readonly List<PlatformConversionMessage> messages = new();

    public DirectoryMeta.Platform SourcePlatform { get; }
    public DirectoryMeta.Platform TargetPlatform { get; }
    public int VisitedDirectoryCount { get; internal set; }
    public int VisitedEntryCount { get; internal set; }
    public int ConvertedMeshCount { get; internal set; }
    public int ConvertedBitmapCount { get; internal set; }
    public int ConvertedCharClipCount { get; internal set; }

    public IReadOnlyList<PlatformConversionMessage> Messages => messages;
    public bool HasErrors => messages.Any(message => message.Severity == PlatformConversionSeverity.Error);
    public bool HasWarnings => messages.Any(message => message.Severity == PlatformConversionSeverity.Warning);

    internal PlatformConversionReport(DirectoryMeta.Platform sourcePlatform, DirectoryMeta.Platform targetPlatform)
    {
        SourcePlatform = sourcePlatform;
        TargetPlatform = targetPlatform;
    }

    internal void Add(PlatformConversionSeverity severity, string message, DirectoryMeta.Entry? entry = null)
    {
        messages.Add(new PlatformConversionMessage(severity, message, entry?.name.value, entry?.type.value));
    }
}
