using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("PostProc"), Description("A PostProc drives post-processing effects.")]
public class RndPostProc : Object
{
    private byte[] preRev18UnknownBlock = new byte[48];

    public enum HallOfTimeType
    {
        kHOTBlended,
        kHOTSolidRingsDepth,
        kHOTSolidRingsAlpha
    }
    private ushort altRevision;
    private ushort revision;

    public float priority;
    [Name("Bloom Color"), Description("Color tint for bloom effect"), MinVersion(12)]
    public HmxColor4 bloomColor = new();
    [Name("Bloom Threshold"), Description("Luminance intensity at which to bloom"), MinVersion(5)]
    public float bloomThreshold;
    [Name("Bloom Intensity"), Description("Bloom intensity"), MinVersion(5)]
    public float bloomIntensity;
    [Name("Bloom Glare"), Description("Whether or not to use the glare effect"), MinVersion(33)]
    public bool bloomGlare;
    [Name("Bloom Streak"), Description("Whether or not to use directional light streaks"), MinVersion(36)]
    public bool bloomStreak;
    [Name("Bloom Streak Attenuation"), Description("Attenuation (scattering amount) of light streak. 0.9 to 0.95 is the sweet spot."), MinVersion(36)]
    public float bloomStreakAttenuation;
    [Name("Bloom Streak Angle"), Description("Angle for light streak"), MinVersion(36)]
    public float bloomStreakAngle;
    [Name("Luminance Map"), Description("Luminance map"), MinVersion(6)]
    public Symbol luminanceMap = Symbol.Empty;
    public bool forceCurrentInterp;
    [Name("Color Correct"), MinVersion(7)]
    public RndColorXfm colorXfm = new();
    [Name("Num Levels"), Description("Number of levels for posterization, 0 turns off"), MinVersion(10)]
    public float posterLevels;
    [Name("Min Intensity"), Description("Minimum intensity to posterize, 1.0 is posterize all"), MinVersion(14)]
    public float posterMin;
    [Name("Kaleidoscope Complexity"), Description("Number of slices in kaleidoscope, 0 turns off, 2 for vertical mirror"), MinVersion(22)]
    public float kaleidoscopeComplexity;
    [Name("Kaleidoscope Size"), Description("Smaller size means more repeated areas, but each area is smaller"), MinVersion(22)]
    public float kaleidoscopeSize;
    [Name("Kaleidoscope Angle"), Description("Additional clockwise degrees of rotation around center."), MinVersion(22)]
    public float kaleidoscopeAngle;
    [Name("Kaleidoscope Radius"), Description("Additional distance from center"), MinVersion(22)]
    public float kaleidoscopeRadius;
    [Name("Kaleidoscope Flip UVs"), Description("Flip texture UV coords when reflect"), MinVersion(22)]
    public bool kaleidoscopeFlipUVs;
    [Name("Flicker Intensity"), Description("Min and max intensity range [0..1], 0.0 to disable"), MinVersion(7)]
    public Vector2 flickerModBounds = new();
    [Name("Flicker Secs Range"), Description("Min and max number of seconds for a light to dark cycle"), MinVersion(7)]
    public Vector2 flickerTimeBounds = new();
    public Vector2 flickerSeconds = new();

    public float colorModulation;

    [Name("Noise Base Scale"), Description("X and Y tiling of the noise map"), MinVersion(7)]
    public Vector2 noiseBaseScale = new();

    [MinVersion(7)]
    public float noiseTopScale;

    [Name("Noise Intensity"), Description("intensity of the noise, 0.0 to disable"), MinVersion(7)]
    public float noiseIntensity;
    [Name("Noise Stationary"), Description("keep the noise map static over the screen"), MinVersion(13)]
    public bool noiseStationary;
    [Name("Noise Midtone"), Description("Applies the noise using at mid-tones of the scene, using an Overlay blend mode."), MinVersion(37)]
    public bool noiseMidtone;
    [Name("Noise Map"), Description("Optional noise bitmap"), MinVersion(9)]
    public Symbol noiseMap = Symbol.Empty;
    [Name("Trail Threshold"), Description("Min pixel value to leave trails [0..1]"), MinVersion(8)]
    public float trailThreshold;
    [Name("Trail Duration"), Description("Seconds for the trails to last"), MinVersion(8)]
    public float trailDuration;

    public Vector3 blendVec = new();
    [Name("Emulate FPS"), Description("Frame rate to emulate, e.g. 24 for film. 0 disables emulation."), MinVersion(8)]
    public float emulateFPS;

    [Name("Hall Of Time Type"), Description("Should the effect be blended, or should it produce solid colors?"), MinVersion(32)]
    public HallOfTimeType hallOfTimeType;
    [Name("Hall Of Time Rate"), Description("Speed of effect. 0 is off. 1 is regular speed."), MinVersion(19)]
    public float hallOfTimeRate;
    [Name("Hall Of Time Color"), Description("Seconds for the trails to last."), MinVersion(19)]
    public HmxColor4 hallOfTimeColor = new();
    [Name("Hall Of Time Mix"), Description("Amount of color to blend. 0 is no color, 1 is solid color. Not applicable if solid rings checked."), MinVersion(19)]
    public float hallOfTimeMix;
    [Name("Motion Blur Weight"), Description("The weighting for individual color channels in the previous frame blend."), MinVersion(27)]
    public HmxColor4 motionBlurWeight = new();
    [Name("Motion Blur Blend"), Description("The amount of the previous frame to blend into the current frame. This can be used to efficiently simulate motion blur or other effects. Set to zero to disable."), MinVersion(21)]
    public float motionBlurBlend;
    [Name("Motion Blur Velocity"), Description("Whether or not to use the velocity motion blur effect. Should be enabled almost all the time."), MinVersion(34)]
    public bool motionBlurVelocity;
    [Name("Gradient Map"), Description("Gradient map; this texture should be layed out horizontally such that the color to use when the pixel is black is on the left and white is on the right."), MinVersion(23)]
    public Symbol gradientMap = Symbol.Empty;
    [Name("Gradient Map Opacity"), Description("The opacity of the gradient map effect."), MinVersion(23)]
    public float gradientMapOpacity;
    [Name("Gradient Map Index"), Description("This indexes veritically into the gradient map texture. This is useful for storing multiple gradient map textures in a single texture, and to blend between them."), MinVersion(23)]
    public float gradientMapIndex;
    [Name("Gradient Map Start"), Description("The depth where the gradient map will begin to take effect."), MinVersion(23)]
    public float gradientMapStart;
    [Name("Gradient Map End"), Description("The depth where the gradient map will no longer take effect."), MinVersion(23)]
    public float gradientMapEnd;
    [Name("Refract Map"), Description("This is a normal map used to distort the screen."), MinVersion(25)]
    public Symbol refractMap = Symbol.Empty;
    [Name("Refract Dist"), Description("The distance to refract each pixel of the screen. This can also be negative to reverse the direction. Set to zero to disable."), MinVersion(25)]
    public float refractDist;
    [Name("Refract Scale"), Description("This scales the refraction texture before distorting the screen, in the X and Y directions."), MinVersion(25)]
    public Vector2 refractScale = new();
    [Name("Refract Panning"), Description("The amount to offset the refraction texture, in the X and Y directions. This is a fixed amount to offset the refraction effect."), MinVersion(25)]
    public Vector2 refractPanning = new();
    [Name("Refract Velocity"), Description("The velocity to scroll the refraction texture, in the X and Y directions. The value is specified in units per second, and will offset the refraction effect over time."), MinVersion(28)]
    public Vector2 refractVelocity = new();
    [Name("Refract Angle"), Description("The angle to rotate the refraction texture, in degrees."), MinVersion(25)]
    public float refractAngle;
    [Name("Chromatic Aberration Offset"), Description("The size, in pixels, of the chromatic aberration or sharpen effect."), MinVersion(26)]
    public float chromaticAberrationOffset;
    [Name("Chromatic Sharpen"), Description("Whether to sharpen the chromatic image or apply the aberration effect."), MinVersion(35)]
    public bool chromaticSharpen;
    [Name("Vignette Color"), Description("Color tint for vignette effect"), MinVersion(30)]
    public HmxColor4 vignetteColor = new();
    [Name("Vignette Intensity"), Description("0 for no effect, 1 for normal, less than one for smaller effect, 2 is full color"), MinVersion(30)]
    public float vignetteIntensity;

    [Name("Hue Target"), Description("Which color to converge on: -180 to 180, 0.0 is red, 120 is green, -120 is blue. Follows a standard HSV color wheel.")]
    public float hueTarget = -75.0f;
    [Name("Hue Focus"), Description("0.0 - 1.0. The higher this value is, the more monochromatic the output becomes.")]
    public float hueFocus = 0.958f;
    [Name("Blend Amount"), Description("0.0 - 1.0. interpolates from original input color at 0 to the fully remapped color at 1.")]
    public float blendAmount;
    [Name("Brightness Power"), Description("v component of the color gets raised to this power. 1.0 = no change.")]
    public float brightnessPower = 1.0f;

    [MinVersion(12), MaxVersion(23)]
    private int unkInt1;
    [MinVersion(5), MaxVersion(11)]
    private uint unkUInt1;
    [MinVersion(5), MaxVersion(11)]
    private HmxColor4 unkColor = new();

    public bool unkBool;

    private uint unkPreRev16;
    private uint unkPreRev1F;
    private uint unkPreRev11;


    public new RndPostProc Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision > 4)
        {
            if (revision > 11)
            {
                bloomColor = bloomColor.Read(reader);
                if (revision < 0x18)
                {
                    unkInt1 = reader.ReadInt32();
                }
                bloomIntensity = reader.ReadFloat();
            }
            else
            {
                unkColor = unkColor.Read(reader);
                unkUInt1 = reader.ReadUInt32();
                bloomIntensity = reader.ReadFloat();
            }
            bloomThreshold = reader.ReadFloat();
        }
        if (revision > 5 && altRevision < 1)
        {
            luminanceMap = Symbol.Read(reader);
        }

        if (revision > 6)
        {
            if (revision < 0x12)
            {
                reader.ReadBlock(preRev18UnknownBlock);
            }
            else
            {
                colorXfm = colorXfm.Read(reader);
            }

            flickerModBounds = flickerModBounds.Read(reader);
            flickerTimeBounds = flickerTimeBounds.Read(reader);



            noiseBaseScale = noiseBaseScale.Read(reader);
            noiseTopScale = reader.ReadFloat();
            noiseIntensity = reader.ReadFloat();
            if (0xc < revision)
            {
                noiseStationary = reader.ReadBoolean();
            }

            if (8 < revision)
            {
                noiseMap = Symbol.Read(reader);
            }

            if (revision > 36)
            {
                noiseMidtone = reader.ReadBoolean();
            }

            if (revision < 0x12)
            {
                colorXfm.hue = reader.ReadFloat();
                colorXfm.saturation = reader.ReadFloat();
                colorXfm.lightness = reader.ReadFloat();
                colorXfm.contrast = reader.ReadFloat();
                colorXfm.brightness = reader.ReadFloat();
            }
        }

        if (revision > 7)
        {
            trailThreshold = reader.ReadFloat();
            trailDuration = reader.ReadFloat();
            emulateFPS = reader.ReadFloat();


        }

        if (9 < revision)
        {
            if (revision < 0x12)
            {
                colorXfm.levelInLo = colorXfm.levelInLo.Read(reader);
                colorXfm.levelInHi = colorXfm.levelInHi.Read(reader);
                colorXfm.levelOutLo = colorXfm.levelOutLo.Read(reader);
                colorXfm.levelOutHi = colorXfm.levelOutHi.Read(reader);
            }
            posterLevels = reader.ReadFloat();
        }

        if (0xD < revision)
        {
            posterMin = reader.ReadFloat();
        }

        if (0xB < revision)
        {
            if (revision < 0x16)
            {
                unkPreRev16 = reader.ReadUInt32();
            }
            else
            {
                kaleidoscopeComplexity = reader.ReadFloat();
                kaleidoscopeSize = reader.ReadFloat();
                kaleidoscopeAngle = reader.ReadFloat();
                kaleidoscopeRadius = reader.ReadFloat();
                kaleidoscopeFlipUVs = reader.ReadBoolean();
            }
        }

        if (((0xe < revision) && (revision < 0x1f)))
        {
            unkPreRev1F = reader.ReadUInt32();
            if (revision < 0x11)
            {
                unkPreRev11 = reader.ReadUInt32();
            }
        }

        if (revision > 0x12)
        {
            hallOfTimeRate = reader.ReadFloat();
            hallOfTimeColor = hallOfTimeColor.Read(reader);
            hallOfTimeMix = reader.ReadFloat();
            if ((revision < 0x14) || (0x1f < revision))
            {
                if (0x1f < revision)
                {
                    hallOfTimeType = (HallOfTimeType)reader.ReadInt32();
                }
            }
            else
            {
                unkBool = reader.ReadBoolean();
            }
        }


        if (0x14 < revision)
        {
            motionBlurBlend = reader.ReadFloat();
            if (0x1a < revision)
            {
                motionBlurWeight = motionBlurWeight.Read(reader);
                if (0x21 < revision)
                {
                    motionBlurVelocity = reader.ReadBoolean();
                }
            }
        }

        if (0x16 < revision)
        {
            gradientMap = Symbol.Read(reader);
            gradientMapOpacity = reader.ReadFloat();
            gradientMapIndex = reader.ReadFloat();
            gradientMapStart = reader.ReadFloat();
            gradientMapEnd = reader.ReadFloat();
        }

        if (0x18 < revision)
        {
            refractMap = Symbol.Read(reader);
            refractDist = reader.ReadFloat();
            refractScale = refractScale.Read(reader);
            refractPanning = refractPanning.Read(reader);
            refractAngle = reader.ReadFloat();
            if (0x1b < revision)
            {
                refractVelocity = refractVelocity.Read(reader);
            }
        }

        if (0x19 < revision)
        {
            chromaticAberrationOffset = reader.ReadFloat();
            if (0x22 < revision)
            {
                chromaticSharpen = reader.ReadBoolean();
            }
        }

        if (0x1d < revision)
        {
            vignetteColor = vignetteColor.Read(reader);
            vignetteIntensity = reader.ReadFloat();
        }

        if (0x20 < revision)
        {
            bloomGlare = reader.ReadBoolean();
        }

        if (0x23 < revision)
        {
            bloomStreak = reader.ReadBoolean();
            bloomStreakAttenuation = reader.ReadFloat();
            bloomStreakAngle = reader.ReadFloat();
        }

        if (altRevision > 1)
        {
            hueTarget = reader.ReadFloat();
            hueFocus = reader.ReadFloat();
            blendAmount = reader.ReadFloat();
            brightnessPower = reader.ReadFloat();
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);
        base.Write(writer, false, parent, entry);

        if (revision > 4)
        {
            if (revision > 11)
            {
                bloomColor.Write(writer);
                if (revision < 0x18)
                {
                    writer.WriteInt32(unkInt1);
                }
                writer.WriteFloat(bloomIntensity);
            }
            else
            {
                unkColor.Write(writer);
                writer.WriteUInt32(unkUInt1);
                writer.WriteFloat(bloomIntensity);
            }
            writer.WriteFloat(bloomThreshold);
        }
        if (revision > 5 && altRevision < 1)
        {
            Symbol.Write(writer, luminanceMap);
        }

        if (revision > 6)
        {
            if (revision < 0x12)
            {
                writer.WriteBlock(preRev18UnknownBlock);
            }
            else
            {
                colorXfm.Write(writer);
            }

            flickerModBounds.Write(writer);
            flickerTimeBounds.Write(writer);

            noiseBaseScale.Write(writer);
            writer.WriteFloat(noiseTopScale);
            writer.WriteFloat(noiseIntensity);
            if (0xc < revision)
            {
                writer.WriteBoolean(noiseStationary);
            }

            if (8 < revision)
            {
                Symbol.Write(writer, noiseMap);
            }

            if (revision > 36)
            {
                writer.WriteBoolean(noiseMidtone);
            }

            if (revision < 0x12)
            {
                writer.WriteFloat(colorXfm.hue);
                writer.WriteFloat(colorXfm.saturation);
                writer.WriteFloat(colorXfm.lightness);
                writer.WriteFloat(colorXfm.contrast);
                writer.WriteFloat(colorXfm.brightness);
            }
        }

        if (revision > 7)
        {
            writer.WriteFloat(trailThreshold);
            writer.WriteFloat(trailDuration);
            writer.WriteFloat(emulateFPS);
        }

        if (9 < revision)
        {
            if (revision < 0x12)
            {
                colorXfm.levelInLo.Write(writer);
                colorXfm.levelInHi.Write(writer);
                colorXfm.levelOutLo.Write(writer);
                colorXfm.levelOutHi.Write(writer);
            }
            writer.WriteFloat(posterLevels);
        }

        if (0xD < revision)
        {
            writer.WriteFloat(posterMin);
        }

        if (0xB < revision)
        {
            if (revision < 0x16)
            {
                writer.WriteUInt32(unkPreRev16);
            }
            else
            {
                writer.WriteFloat(kaleidoscopeComplexity);
                writer.WriteFloat(kaleidoscopeSize);
                writer.WriteFloat(kaleidoscopeAngle);
                writer.WriteFloat(kaleidoscopeRadius);
                writer.WriteBoolean(kaleidoscopeFlipUVs);
            }
        }

        if (((0xe < revision) && (revision < 0x1f)))
        {
            writer.WriteUInt32(unkPreRev1F);
            if (revision < 0x11)
            {
                writer.WriteUInt32(unkPreRev11);
            }
        }

        if (revision > 0x12)
        {
            writer.WriteFloat(hallOfTimeRate);
            hallOfTimeColor.Write(writer);
            writer.WriteFloat(hallOfTimeMix);
            if ((revision < 0x14) || (0x1f < revision))
            {
                if (0x1f < revision)
                {
                    writer.WriteInt32((int)hallOfTimeType);
                }
            }
            else
            {
                writer.WriteBoolean(unkBool);
            }
        }

        if (0x14 < revision)
        {
            writer.WriteFloat(motionBlurBlend);
            if (0x1a < revision)
            {
                motionBlurWeight.Write(writer);
                if (0x21 < revision)
                {
                    writer.WriteBoolean(motionBlurVelocity);
                }
            }
        }

        if (0x16 < revision)
        {
            Symbol.Write(writer, gradientMap);
            writer.WriteFloat(gradientMapOpacity);
            writer.WriteFloat(gradientMapIndex);
            writer.WriteFloat(gradientMapStart);
            writer.WriteFloat(gradientMapEnd);
        }

        if (0x18 < revision)
        {
            Symbol.Write(writer, refractMap);
            writer.WriteFloat(refractDist);
            refractScale.Write(writer);
            refractPanning.Write(writer);
            writer.WriteFloat(refractAngle);
            if (0x1b < revision)
            {
                refractVelocity.Write(writer);
            }
        }

        if (0x19 < revision)
        {
            writer.WriteFloat(chromaticAberrationOffset);
            if (0x22 < revision)
            {
                writer.WriteBoolean(chromaticSharpen);
            }
        }

        if (0x1d < revision)
        {
            vignetteColor.Write(writer);
            writer.WriteFloat(vignetteIntensity);
        }

        if (0x20 < revision)
        {
            writer.WriteBoolean(bloomGlare);
        }

        if (0x23 < revision)
        {
            writer.WriteBoolean(bloomStreak);
            writer.WriteFloat(bloomStreakAttenuation);
            writer.WriteFloat(bloomStreakAngle);
        }

        if (altRevision > 1)
        {
            writer.WriteFloat(hueTarget);
            writer.WriteFloat(hueFocus);
            writer.WriteFloat(blendAmount);
            writer.WriteFloat(brightnessPower);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
