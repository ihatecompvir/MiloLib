using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("SyncWorldFx")]
public class SyncWorldFx : Object
{
    private ushort revision = 7;
    private ushort altRevision;
    [Name("World Environ"), Description("Lighting environment for the world.")]
    public Symbol worldEnviron = Symbol.Empty;
    [Name("Attach To Cam Trans")]
    public Symbol attachToCamTrans = Symbol.Empty;
    [Name("Attach To Finish Trans")]
    public Symbol attachToFinishTrans = Symbol.Empty;
    [Name("Attach To Horizon Trans")]
    public Symbol attachToHorizonTrans = Symbol.Empty;
    [Name("Attach To Horizon Dist")]
    public float attachToHorizonDist;
    [Name("Default Preset"), Description("Default lighting preset.")]
    public Symbol defaultPreset = Symbol.Empty;
    [Name("Default Postproc")]
    public Symbol defaultPostproc = Symbol.Empty;
    [Name("Default DOF Focal Dist"), Description("Default depth-of-field focal distance.")]
    public float defaultDofFocalDist;
    [Name("Default DOF Blur Depth"), Description("Default depth-of-field blur depth.")]
    public float defaultDofBlurDepth;
    [Name("Default DOF Min Blur")]
    public float defaultDofMinBlur;
    [Name("Default DOF Max Blur")]
    public float defaultDofMaxBlur;
    [Name("Bloom Intensity Offset")]
    public float bloomIntensityOffset;
    [Name("Motion Blur Blend Offset")]
    public float motionBlurBlendOffset;
    [Name("Color Brightness Offset")]
    public float colorBrightnessOffset;
    [Name("Color Out Hi Offset")]
    public HmxColor4 colorOutHiOffset = new();

    public new SyncWorldFx Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 7);
        if (revision != 7)
        {
            throw new NotSupportedException($"Unsupported SyncWorldFx revision {revision}.");
        }

        base.Read(reader, false, parent, entry);
        worldEnviron = Symbol.Read(reader);
        attachToCamTrans = Symbol.Read(reader);
        attachToFinishTrans = Symbol.Read(reader);
        attachToHorizonTrans = Symbol.Read(reader);
        attachToHorizonDist = reader.ReadFloat();
        defaultPreset = Symbol.Read(reader);
        defaultPostproc = Symbol.Read(reader);
        defaultDofFocalDist = reader.ReadFloat();
        defaultDofBlurDepth = reader.ReadFloat();
        defaultDofMinBlur = reader.ReadFloat();
        defaultDofMaxBlur = reader.ReadFloat();
        bloomIntensityOffset = reader.ReadFloat();
        motionBlurBlendOffset = reader.ReadFloat();
        colorBrightnessOffset = reader.ReadFloat();
        colorOutHiOffset.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (revision != 7)
        {
            throw new NotSupportedException($"Unsupported SyncWorldFx revision {revision}.");
        }

        AssetHelpers.WriteRevision(writer, revision, altRevision, 7);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, worldEnviron);
        Symbol.Write(writer, attachToCamTrans);
        Symbol.Write(writer, attachToFinishTrans);
        Symbol.Write(writer, attachToHorizonTrans);
        writer.WriteFloat(attachToHorizonDist);
        Symbol.Write(writer, defaultPreset);
        Symbol.Write(writer, defaultPostproc);
        writer.WriteFloat(defaultDofFocalDist);
        writer.WriteFloat(defaultDofBlurDepth);
        writer.WriteFloat(defaultDofMinBlur);
        writer.WriteFloat(defaultDofMaxBlur);
        writer.WriteFloat(bloomIntensityOffset);
        writer.WriteFloat(motionBlurBlendOffset);
        writer.WriteFloat(colorBrightnessOffset);
        colorOutHiOffset.Write(writer);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

