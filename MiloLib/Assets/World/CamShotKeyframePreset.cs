using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("CamShotKeyframePreset")]
public class CamShotKeyframePreset : Object
{
    public class Preset
    {
        [Name("Label")]
        public Symbol label = Symbol.Empty;
        [Name("Entity")]
        public Symbol entity = Symbol.Empty;
        [Name("Subpart Name")]
        public Symbol subpartName = Symbol.Empty;
        [Name("Blur Depth")]
        public float blurDepth;
        [Name("Max Blur"), Description("Maximum blur amount.")]
        public float maxBlur;
        [Name("Min Blur")]
        public float minBlur;
        [Name("Focus Blur Multiplier")]
        public float focusBlurMultiplier;

        public Preset Read(EndianReader reader)
        {
            label = Symbol.Read(reader);
            entity = Symbol.Read(reader);
            subpartName = Symbol.Read(reader);
            blurDepth = reader.ReadFloat();
            maxBlur = reader.ReadFloat();
            minBlur = reader.ReadFloat();
            focusBlurMultiplier = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, label);
            Symbol.Write(writer, entity);
            Symbol.Write(writer, subpartName);
            writer.WriteFloat(blurDepth);
            writer.WriteFloat(maxBlur);
            writer.WriteFloat(minBlur);
            writer.WriteFloat(focusBlurMultiplier);
        }
    }

    private ushort revision = 1;
    private ushort altRevision;
    [Name("Presets")]
    public List<Preset> presets = new();

    public new CamShotKeyframePreset Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        presets = AssetHelpers.ReadList(reader, () => new Preset().Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, presets, preset => preset.Write(writer));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
