namespace MiloLib.Utils.Conversion;

public sealed class PlatformConversionOptions
{
    public static PlatformConversionOptions Default { get; } = new();

    /// <summary>
    /// Updates DirectoryMeta.platform recursively on all dirs
    /// </summary>
    public bool UpdateDirectoryPlatforms { get; init; } = true;

    /// <summary>
    /// Rewrites mesh platform packing stuff without changing the mesh revision
    /// </summary>
    public bool ConvertMeshPlatformLayout { get; init; } = true;

    /// <summary>
    /// Re-encodes texture payloads for the target platform (for example Wii CMP --> BC3). When false, only the bitmap platform metadata is changed
    /// </summary>
    public bool ConvertBitmapPayloads { get; init; } = true;

    /// <summary>
    /// Rewrites CharClip sample blocks between the Wii's compact stride and the stride used by Xbox/PS3
    /// </summary>
    public bool ConvertCharClipSamplePadding { get; init; } = true;
}
