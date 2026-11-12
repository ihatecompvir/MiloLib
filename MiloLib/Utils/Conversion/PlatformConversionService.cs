using BCnEncoder.Encoder;
using BCnEncoder.Shared;
using Microsoft.Toolkit.HighPerformance;
using MiloLib.Assets;
using MiloLib.Assets.Char;
using MiloLib.Assets.Rnd;

namespace MiloLib.Utils.Conversion;

public static class PlatformConversionService
{
    public static PlatformConversionReport Convert(DirectoryMeta root, DirectoryMeta.Platform targetPlatform, PlatformConversionOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(root);

        options ??= PlatformConversionOptions.Default;

        DirectoryMeta.Platform sourcePlatform = root.platform;
        PlatformConversionReport report = new(sourcePlatform, targetPlatform);

        if (sourcePlatform == targetPlatform)
        {
            report.Add(PlatformConversionSeverity.Info, "Source and target platforms already match, no conversion was needed");
            return report;
        }

        ConvertDirectory(root, sourcePlatform, targetPlatform, options, report);
        return report;
    }

    private static void ConvertDirectory(DirectoryMeta dir, DirectoryMeta.Platform sourcePlatform, DirectoryMeta.Platform targetPlatform, PlatformConversionOptions options, PlatformConversionReport report)
    {
        report.VisitedDirectoryCount++;

        if (options.UpdateDirectoryPlatforms)
        {
            dir.platform = targetPlatform;
        }

        foreach (DirectoryMeta.Entry entry in dir.entries)
        {
            report.VisitedEntryCount++;
            ConvertEntry(entry, sourcePlatform, targetPlatform, options, report);

            if (entry.dir != null)
            {
                ConvertDirectory(entry.dir, sourcePlatform, targetPlatform, options, report);
            }
        }

        if (dir.directory is ObjectDir objectDir)
        {
            foreach (DirectoryMeta inlineSubDir in objectDir.inlineSubDirs)
            {
                ConvertDirectory(inlineSubDir, sourcePlatform, targetPlatform, options, report);
            }
        }
    }

    private static void ConvertEntry(DirectoryMeta.Entry entry, DirectoryMeta.Platform sourcePlatform, DirectoryMeta.Platform targetPlatform, PlatformConversionOptions options, PlatformConversionReport report)
    {
        if (entry.obj is RndMesh mesh && options.ConvertMeshPlatformLayout)
        {
            ConvertMesh(mesh, entry, targetPlatform, report);
            return;
        }

        if (entry.obj is RndTex tex)
        {
            ConvertTexture(tex, entry, sourcePlatform, targetPlatform, options, report);
            return;
        }

        if (entry.obj is CharClip charClip && options.ConvertCharClipSamplePadding)
        {
            ConvertCharClip(charClip, entry, sourcePlatform, targetPlatform, report);
        }
    }

    private static void ConvertCharClip(CharClip clip, DirectoryMeta.Entry entry, DirectoryMeta.Platform sourcePlatform, DirectoryMeta.Platform targetPlatform, PlatformConversionReport report)
    {
        bool sourceUsesCompactSamples = sourcePlatform == DirectoryMeta.Platform.Wii;
        bool targetUsesCompactSamples = targetPlatform == DirectoryMeta.Platform.Wii;

        if (sourceUsesCompactSamples == targetUsesCompactSamples)
        {
            return;
        }

        bool changed = false;
        bool failed = false;

        ConvertSamples(clip.full, "full", targetUsesCompactSamples, entry, report, ref changed, ref failed);
        ConvertSamples(clip.one, "one", targetUsesCompactSamples, entry, report, ref changed, ref failed);

        if (changed)
        {
            report.ConvertedCharClipCount++;
        }

        if (failed)
        {
            report.Add(PlatformConversionSeverity.Warning, "CharClip sample padding conversion was incomplete because at least one sample block did not match the expected compact or padded stride; the resulting milo will likely not load!", entry);
        }
    }

    private static void ConvertSamples(CharClip.CharBonesSamples samples, string sampleName, bool targetUsesCompactSamples, DirectoryMeta.Entry entry, PlatformConversionReport report, ref bool changed, ref bool failed)
    {
        if (samples.rawData.Length == 0 || samples.numSamples == 0)
        {
            return;
        }

        samples.ComputeBytesPerSample();

        int compactStride = samples.dataBytesPerSample;
        int paddedStride = samples.bytesPerSample;

        if (compactStride == paddedStride)
        {
            return;
        }

        int compactSize = compactStride * samples.numSamples;
        int paddedSize = paddedStride * samples.numSamples;

        if (targetUsesCompactSamples)
        {
            if (samples.rawData.Length == compactSize)
            {
                return;
            }

            if (samples.rawData.Length != paddedSize)
            {
                failed = true;
                report.Add(PlatformConversionSeverity.Warning, $"CharClip {sampleName} sample block has {samples.rawData.Length} bytes; expected {compactSize} compact or {paddedSize} padded bytes.", entry);
                return;
            }

            byte[] compact = GC.AllocateUninitializedArray<byte>(compactSize);
            for (int i = 0; i < samples.numSamples; i++)
            {
                Buffer.BlockCopy(samples.rawData, i * paddedStride, compact, i * compactStride, compactStride);
            }

            samples.rawData = compact;
            samples.usePaddedSampleData = false;
            changed = true;
            return;
        }

        if (samples.rawData.Length == paddedSize)
        {
            samples.usePaddedSampleData = true;
            return;
        }

        if (samples.rawData.Length != compactSize)
        {
            failed = true;
            report.Add(PlatformConversionSeverity.Warning, $"CharClip {sampleName} sample block has {samples.rawData.Length} bytes; expected {compactSize} compact or {paddedSize} padded bytes.", entry);
            return;
        }

        byte[] padded = new byte[paddedSize];
        for (int i = 0; i < samples.numSamples; i++)
        {
            Buffer.BlockCopy(samples.rawData, i * compactStride, padded, i * paddedStride, compactStride);
        }

        samples.rawData = padded;
        samples.usePaddedSampleData = true;
        changed = true;
    }

    private static void ConvertMesh(RndMesh mesh, DirectoryMeta.Entry entry, DirectoryMeta.Platform targetPlatform, PlatformConversionReport report)
    {
        bool changed = false;

        if (mesh.RndMeshRevision >= 36 && mesh.vertices.isNextGen)
        {
            (uint compressionType, uint vertexSize)? targetLayout = null;
            if (targetPlatform == DirectoryMeta.Platform.Xbox)
            {
                targetLayout = (1, 36);
            }
            else if (targetPlatform == DirectoryMeta.Platform.PS3)
            {
                targetLayout = (2, 40);
            }

            if (targetLayout is { } layout)
            {
                if (mesh.vertices.compressionType != layout.compressionType)
                {
                    mesh.vertices.compressionType = layout.compressionType;
                    changed = true;
                }

                if (mesh.vertices.vertexSize != layout.vertexSize)
                {
                    mesh.vertices.vertexSize = layout.vertexSize;
                    changed = true;
                }
            }
            else
            {
                report.Add(PlatformConversionSeverity.Warning, "Next-gen mesh vertex compression has no known target layout for this platform; mesh payload was left as-is.", entry);
            }
        }

        if (targetPlatform == DirectoryMeta.Platform.Wii)
        {
            if (mesh.RndMeshAltRevision < 5)
            {
                mesh.SetRndMeshRevision(mesh.RndMeshRevision, 5);
                changed = true;
                report.Add(PlatformConversionSeverity.Warning, "Mesh was marked with Wii alt revision 5 for platform layout, but Wii strip data was not generated.", entry);
            }
        }
        else if (mesh.RndMeshAltRevision != 0)
        {
            mesh.SetRndMeshRevision(mesh.RndMeshRevision, 0);
            changed = true;
        }

        if (changed)
        {
            report.ConvertedMeshCount++;
        }
    }

    private static void ConvertTexture(RndTex tex, DirectoryMeta.Entry entry, DirectoryMeta.Platform sourcePlatform, DirectoryMeta.Platform targetPlatform, PlatformConversionOptions options, PlatformConversionReport report)
    {
        if (!options.ConvertBitmapPayloads)
        {
            if (RequiresBitmapPayloadConversion(tex.bitmap, sourcePlatform, targetPlatform))
            {
                report.Add(PlatformConversionSeverity.Warning, "Texture payload conversion is disabled; bitmap platform metadata was updated but encoded texture bytes were left as-is.", entry);
            }

            tex.bitmap.platform = targetPlatform;
            return;
        }

        try
        {
            if (TryConvertBitmap(tex.bitmap, sourcePlatform, targetPlatform))
            {
                report.ConvertedBitmapCount++;
            }
            else if (RequiresBitmapPayloadConversion(tex.bitmap, sourcePlatform, targetPlatform))
            {
                report.Add(PlatformConversionSeverity.Warning, "No texture payload converter exists for this source encoding and target platform.", entry);
            }
        }
        catch (Exception ex)
        {
            report.Add(PlatformConversionSeverity.Error, $"Texture payload conversion failed: {ex.Message}", entry);
        }

        tex.bitmap.platform = targetPlatform;
    }

    private static bool RequiresBitmapPayloadConversion(RndBitmap bitmap, DirectoryMeta.Platform sourcePlatform, DirectoryMeta.Platform targetPlatform)
    {
        if (sourcePlatform == targetPlatform)
        {
            return false;
        }

        return IsTplEncoding(bitmap.encoding)
            || targetPlatform == DirectoryMeta.Platform.Xbox
            || sourcePlatform == DirectoryMeta.Platform.Xbox;
    }

    private static bool TryConvertBitmap(RndBitmap bitmap, DirectoryMeta.Platform sourcePlatform, DirectoryMeta.Platform targetPlatform)
    {
        if (IsTplEncoding(bitmap.encoding) && targetPlatform is DirectoryMeta.Platform.Xbox or DirectoryMeta.Platform.PS3 or DirectoryMeta.Platform.PC_or_iPod)
        {
            byte[] rgba = DecodeTplToRgba(bitmap);
            byte[] encoded = EncodeRgbaToBc3(rgba, bitmap.width, bitmap.height, targetPlatform);

            bitmap.encoding = RndBitmap.TextureEncoding.DXT5_BC3;
            bitmap.bpp = 8;
            bitmap.bpl = (ushort)(bitmap.width);
            bitmap.mipMaps = 0;
            bitmap.wiiAlphaNum = 0;
            bitmap.colorPalette = Array.Empty<byte>();
            bitmap.textures = [encoded];
            return true;
        }

        if (sourcePlatform != DirectoryMeta.Platform.Xbox && targetPlatform == DirectoryMeta.Platform.Xbox && IsDxtEncoding(bitmap.encoding))
        {
            SwapDxtWords(bitmap);
            return true;
        }

        if (sourcePlatform == DirectoryMeta.Platform.Xbox && targetPlatform != DirectoryMeta.Platform.Xbox && IsDxtEncoding(bitmap.encoding))
        {
            SwapDxtWords(bitmap);
            return true;
        }

        return false;
    }

    private static bool IsTplEncoding(RndBitmap.TextureEncoding encoding)
    {
        return encoding is RndBitmap.TextureEncoding.TPL_CMP
            or RndBitmap.TextureEncoding.TPL_CMP_ALPHA
            or RndBitmap.TextureEncoding.TPL_CMP_2;
    }

    private static bool IsDxtEncoding(RndBitmap.TextureEncoding encoding)
    {
        return encoding is RndBitmap.TextureEncoding.DXT1_BC1
            or RndBitmap.TextureEncoding.DXT5_BC3
            or RndBitmap.TextureEncoding.ATI2_BC5;
    }

    private static byte[] DecodeTplToRgba(RndBitmap bitmap)
    {
        if (bitmap.textures.Count == 0 || bitmap.textures[0].Length == 0)
        {
            throw new InvalidOperationException("No texture data is available.");
        }

        int width = bitmap.width;
        int height = bitmap.height;
        byte[] rawData = bitmap.textures[0];

        if (bitmap.bpp == 4 && bitmap.wiiAlphaNum == 4)
        {
            int cmpSize = width * height * bitmap.bpp / 8;
            byte[] rgbData = rawData[..cmpSize];
            byte[] alphaData = rawData[^cmpSize..];
            return CombineRgbAndAlpha(WiiTextureCodec.DecodeCmp(rgbData, width, height), WiiTextureCodec.DecodeCmp(alphaData, width, height));
        }

        if (bitmap.bpp == 8)
        {
            int halfSize = rawData.Length / 2;
            byte[] rgbData = rawData[..halfSize];
            byte[] alphaData = rawData[halfSize..];
            return CombineRgbAndAlpha(WiiTextureCodec.DecodeCmp(rgbData, width, height), WiiTextureCodec.DecodeCmp(alphaData, width, height));
        }

        return WiiTextureCodec.DecodeCmp(rawData, width, height);
    }

    private static byte[] CombineRgbAndAlpha(byte[] decodedRgb, byte[] decodedAlpha)
    {
        byte[] rgbaData = new byte[decodedRgb.Length];
        for (int i = 0; i < decodedRgb.Length; i += 4)
        {
            rgbaData[i + 0] = decodedRgb[i + 0];
            rgbaData[i + 1] = decodedRgb[i + 1];
            rgbaData[i + 2] = decodedRgb[i + 2];
            rgbaData[i + 3] = decodedAlpha[i + 1];
        }

        return rgbaData;
    }

    private static byte[] EncodeRgbaToBc3(byte[] rgba, int width, int height, DirectoryMeta.Platform targetPlatform)
    {
        ColorRgba32[] pixels = new ColorRgba32[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            int offset = i * 4;
            pixels[i] = new ColorRgba32(rgba[offset + 0], rgba[offset + 1], rgba[offset + 2], rgba[offset + 3]);
        }

        BcEncoder encoder = new();
        encoder.OutputOptions.GenerateMipMaps = false;
        encoder.OutputOptions.Quality = CompressionQuality.BestQuality;
        encoder.OutputOptions.Format = CompressionFormat.Bc3;

        byte[] encoded = encoder.EncodeToRawBytes(pixels.AsMemory().AsMemory2D(height, width))[0];
        if (targetPlatform == DirectoryMeta.Platform.Xbox)
        {
            SwapDxtWords(encoded);
        }

        return encoded;
    }

    private static void SwapDxtWords(RndBitmap bitmap)
    {
        foreach (byte[] texture in bitmap.textures)
        {
            SwapDxtWords(texture);
        }
    }

    private static void SwapDxtWords(byte[] data)
    {
        for (int i = 0; i + 1 < data.Length; i += 2)
        {
            (data[i], data[i + 1]) = (data[i + 1], data[i]);
        }
    }
}
