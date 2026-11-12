using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

public partial class RndMat
{
    public ushort baseMaterialRevision;
    public ushort baseMaterialAltRevision;
    [Name("Metamaterial"), Description("MetaMaterials set the Material properties to be edited")]
    public Symbol metaMaterial = Symbol.Empty;
    [Name("Cull"), Description("Cull backface polygons")]
    public int cullMode = 1;
    [Name("Bloom Multiplier"), Description("Multiply bloom amount by this much. 1 is the default, 0 means no bloom. Not applicable for alpha_write, alpha_cut and alpha blend Mats.")]
    public float bloomMultiplier = 1.0f;
    [Name("Never Fit To Spline"), Description("Don't alter mesh geometry in order to fit to a spline (most materials should leave this un-checked).")]
    public bool neverFitToSpline;
    public bool oldBaseMaterialBool1;
    public bool oldBaseMaterialBool2;
    [Name("Allow Distortion Effects"), Description("Whether or not any vertex-based distortion effects should be allowed.")]
    public bool allowDistortionEffects = true;
    [Name("Shockwave Mult"), Description("Multiplier on shockwave amplitude when a shockwave is being applied. 1 gives default behavior. 0 to disable shockwave.")]
    public float shockwaveMultiplier = 1.0f;
    [Name("World Projection Tiling"), Description("World projection texture coordinate tiling scale")]
    public float worldProjectionTiling;
    [Name("World Projection Start Blend"), Description("The top/bottom texture starts blending in when the normal z component is at this value")]
    public float worldProjectionStartBlend;
    [Name("World Projection End Blend"), Description("The top/bottom texture is fully blended in when the normal z component is at this value")]
    public float worldProjectionEndBlend;
    [Name("Diffuse Tex2"), Description("Top/bottom texture")]
    public Symbol diffuseTex2 = Symbol.Empty;
    [Name("Force Alpha Write"), Description("Always honor alpha write flag, regardless of offscreenness")]
    public bool forceAlphaWrite;

    protected void ReadBaseMaterial(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out baseMaterialRevision, out baseMaterialAltRevision, 8);
        base.Read(reader, false, parent, entry);
        blend = (Blend)reader.ReadInt32();
        color = new HmxColor4().Read(reader);
        useEnviron = reader.ReadBoolean();
        preLit = reader.ReadBoolean();
        zMode = (ZMode)reader.ReadInt32();
        alphaCut = reader.ReadBoolean();
        alphaThreshold = reader.ReadInt32();
        alphaWrite = reader.ReadBoolean();
        texGen = (TexGen)reader.ReadInt32();
        texWrap = (TexWrap)reader.ReadInt32();
        texXfm = new Matrix().Read(reader);
        diffuseTex = Symbol.Read(reader);
        nextPass = Symbol.Read(reader);
        intensify = reader.ReadBoolean();
        if (baseMaterialRevision < 3)
        {
            cull = reader.ReadBoolean();
        }
        else
        {
            cullMode = reader.ReadInt32();
            cull = cullMode != 0;
        }
        emissiveMultiplier = reader.ReadFloat();
        specularRGB = new HmxColor3().Read(reader);
        specularPower = reader.ReadFloat();
        normalMap = Symbol.Read(reader);
        emissiveMap = Symbol.Read(reader);
        specularMap = Symbol.Read(reader);
        environMap = Symbol.Read(reader);
        unkBool_0x3C = reader.ReadBoolean();
        unkBool_0x42 = reader.ReadBoolean();
        perPixelLit = reader.ReadBoolean();
        stencilMode = (StencilMode)reader.ReadInt32();
        fur = Symbol.Read(reader);
        deNormal = reader.ReadFloat();
        anisotropy = reader.ReadFloat();
        normalDetailTiling = reader.ReadFloat();
        normalDetailStrength = reader.ReadFloat();
        normalDetailMap = Symbol.Read(reader);
        pointLights = reader.ReadBoolean();
        fog = reader.ReadBoolean();
        fadeout = reader.ReadBoolean();
        colorAdjust = reader.ReadBoolean();
        rimRGB = new HmxColor3().Read(reader);
        rimPower = reader.ReadFloat();
        rimMap = Symbol.Read(reader);
        rimAlwaysShow = reader.ReadBoolean();
        screenAligned = reader.ReadBoolean();
        shaderVariation = (ShaderVariation)reader.ReadInt32();
        specular2RGB = new HmxColor3().Read(reader);
        specular2Power = reader.ReadFloat();
        perfSettings.Read(reader, 70);
        refractEnabled = reader.ReadBoolean();
        refractStrength = reader.ReadFloat();
        refractNormalMap = Symbol.Read(reader);
        if (baseMaterialRevision > 1)
        {
            bloomMultiplier = reader.ReadFloat();
        }
        if (baseMaterialRevision > 3)
        {
            neverFitToSpline = reader.ReadBoolean();
            if (baseMaterialRevision < 5)
            {
                oldBaseMaterialBool1 = reader.ReadBoolean();
                oldBaseMaterialBool2 = reader.ReadBoolean();
            }
            if (baseMaterialRevision >= 6)
            {
                allowDistortionEffects = reader.ReadBoolean();
                shockwaveMultiplier = reader.ReadFloat();
            }
        }
        if (baseMaterialRevision > 6)
        {
            worldProjectionTiling = reader.ReadFloat();
            worldProjectionStartBlend = reader.ReadFloat();
            worldProjectionEndBlend = reader.ReadFloat();
            diffuseTex2 = Symbol.Read(reader);
        }
        if (baseMaterialRevision > 7)
        {
            forceAlphaWrite = reader.ReadBoolean();
        }
    }

    protected void WriteBaseMaterial(EndianWriter writer, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, baseMaterialRevision, baseMaterialAltRevision, 8);
        base.Write(writer, false, parent, entry);
        writer.WriteInt32((int)blend);
        color.Write(writer);
        writer.WriteBoolean(useEnviron);
        writer.WriteBoolean(preLit);
        writer.WriteInt32((int)zMode);
        writer.WriteBoolean(alphaCut);
        writer.WriteInt32(alphaThreshold);
        writer.WriteBoolean(alphaWrite);
        writer.WriteInt32((int)texGen);
        writer.WriteInt32((int)texWrap);
        texXfm.Write(writer);
        Symbol.Write(writer, diffuseTex);
        Symbol.Write(writer, nextPass);
        writer.WriteBoolean(intensify);
        if (baseMaterialRevision < 3)
        {
            writer.WriteBoolean(cull);
        }
        else if (!cull)
        {
            writer.WriteInt32(0);
        }
        else if (cullMode == 0)
        {
            writer.WriteInt32(1);
        }
        else
        {
            writer.WriteInt32(cullMode);
        }
        writer.WriteFloat(emissiveMultiplier);
        specularRGB.Write(writer);
        writer.WriteFloat(specularPower);
        Symbol.Write(writer, normalMap);
        Symbol.Write(writer, emissiveMap);
        Symbol.Write(writer, specularMap);
        Symbol.Write(writer, environMap);
        writer.WriteBoolean(unkBool_0x3C);
        writer.WriteBoolean(unkBool_0x42);
        writer.WriteBoolean(perPixelLit);
        writer.WriteInt32((int)stencilMode);
        Symbol.Write(writer, fur);
        writer.WriteFloat(deNormal);
        writer.WriteFloat(anisotropy);
        writer.WriteFloat(normalDetailTiling);
        writer.WriteFloat(normalDetailStrength);
        Symbol.Write(writer, normalDetailMap);
        writer.WriteBoolean(pointLights);
        writer.WriteBoolean(fog);
        writer.WriteBoolean(fadeout);
        writer.WriteBoolean(colorAdjust);
        rimRGB.Write(writer);
        writer.WriteFloat(rimPower);
        Symbol.Write(writer, rimMap);
        writer.WriteBoolean(rimAlwaysShow);
        writer.WriteBoolean(screenAligned);
        writer.WriteInt32((int)shaderVariation);
        specular2RGB.Write(writer);
        writer.WriteFloat(specular2Power);
        perfSettings.Write(writer, 70);
        writer.WriteBoolean(refractEnabled);
        writer.WriteFloat(refractStrength);
        Symbol.Write(writer, refractNormalMap);
        if (baseMaterialRevision > 1)
        {
            writer.WriteFloat(bloomMultiplier);
        }
        if (baseMaterialRevision > 3)
        {
            writer.WriteBoolean(neverFitToSpline);
            if (baseMaterialRevision < 5)
            {
                writer.WriteBoolean(oldBaseMaterialBool1);
                writer.WriteBoolean(oldBaseMaterialBool2);
            }
            if (baseMaterialRevision >= 6)
            {
                writer.WriteBoolean(allowDistortionEffects);
                writer.WriteFloat(shockwaveMultiplier);
            }
        }
        if (baseMaterialRevision > 6)
        {
            writer.WriteFloat(worldProjectionTiling);
            writer.WriteFloat(worldProjectionStartBlend);
            writer.WriteFloat(worldProjectionEndBlend);
            Symbol.Write(writer, diffuseTex2);
        }
        if (baseMaterialRevision > 7)
        {
            writer.WriteBoolean(forceAlphaWrite);
        }
    }
}
