using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Environ"), Description("An environment object is drawable. When drawn it sets up dynamic lighting and fogging for subsequently drawn siblings.")]
public class RndEnviron : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();

    [MinVersion(16)]
    public RndTrans trans = new();

    [Name("Real Dynamic Lights"), Description("Real dynamic lights for this environment"), MinVersion(15)]
    public List<Symbol> lightsReal = new();

    [Name("Approximate Dynamic Lights"), Description("Approximated dynamic lights for this environment"), MinVersion(15)]
    public List<Symbol> lightsApprox = new();

    [Name("Lights"), Description("Dynamic lights for this environment"), MaxVersion(14)]
    public List<Symbol> lightsOld = new();


    [Name("Ambient Color"), Description("Ambient color for this environment")]
    public HmxColor4 ambientColor = new();

    [Name("Fog Start"), Description("Fog start distance")]
    public float fogStart;
    [Name("Fog End"), Description("Fog end distance")]
    public float fogEnd;

    [MinVersion(0), MaxVersion(0)]
    public uint unkInt1;

    [Name("Fog Color"), Description("Fog color")]
    public HmxColor4 fogColor = new();

    [Name("Fog Enabled"), Description("Whether fog is enabled for this environment")]
    public bool fogEnabled;


    [Name("Animate From Preset"), Description("Whether this environment should be animated by light presets"), MinVersion(4)]
    public bool animateFromPreset;

    [Name("Fade Out"), Description("Fade out the scene over distance"), MinVersion(5)]
    public bool fadeOut;
    [Name("Fade Out Start"), Description("World space distance from camera to start fading"), MinVersion(5)]
    public float fadeStart;
    [Name("Fade Out End"), Description("World space distance fade out completely"), MinVersion(5)]
    public float fadeEnd;

    [Name("Fade Max"), Description("Maximum opacity of faded objects"), MinVersion(6)]
    public float fadeMax;

    [Name("Fade Reference"), Description("reference object to left/right fade along x-axis"), MinVersion(9)]
    public Symbol fadeRef = Symbol.Empty;

    [Name("Left Out"), Description("distance along negative x to start fading in"), MinVersion(9)]
    public float left_out;
    [Name("Left Opaque"), Description("distance along negative x to become fully opaque"), MinVersion(9)]
    public float left_opaque;
    [Name("Right Out"), Description("distance along positive x to start fading in"), MinVersion(9)]
    public float right_out;
    [Name("Right Opaque"), Description("distance along positive x to become fully opaque"), MinVersion(9)]
    public float right_opaque;

    [Name("Ambient Fog Owner"), Description("Share ambient and fog parameters with this environ"), MinVersion(7)]
    public Symbol ambientFogOwner = Symbol.Empty;

    [Name("Use Color Adjust"), Description("Enable color adjust"), MinVersion(8)]
    public bool useColorAdjust;
    [Name("Color Adjust"), MinVersion(8)]
    public RndColorXfm colorAdjust = new();

    [Name("Ambient Occlusion Strength"), Description("Strength of the ambient occlusion effect"), MinVersion(10)]
    public float aoStrength;

    [Name("Intensity Rate"), Description("The rate the eye adjusts to scene lighting changes over time. The higher the value, the longer it takes the retina to adapt"), MinVersion(11)]
    public float intensityRate;
    [Name("Exposure"), Description("The exposure value to use when tone-mapping. Adjusts the overall exposure level of the scene"), MinVersion(11)]
    public float exposure;
    [Name("White Point"), Description("The white point value to use when tone-mapping. Pixels brighter than this value will be clamped to white and bloom out"), MinVersion(11)]
    public float whitePoint;
    [Name("Use Tone Mapping"), Description("Enable color adjust"), MinVersion(11)]
    public bool useToneMapping;


    [MinVersion(10), MaxVersion(12)]
    public uint unkInt2;
    [MinVersion(11), MaxVersion(13)]
    public uint unkInt3;

    private byte[] gh2OpaqueBytes = Array.Empty<byte>();



    public new RndEnviron Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (parent.revision == 25 && standalone && revision == 7)
        {
            gh2OpaqueBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
            return this;
        }

        if (revision > 1)
        {
            objFields.Read(reader, parent, entry);
        }

        if (revision < 3)
        {
            draw = draw.Read(reader, false, parent, entry);
        }

        if (revision > 15)
        {
            draw.Read(reader, false, parent, entry);
            trans.Read(reader, false, parent, entry);
        }

        if (revision < 0xF)
        {
            lightsOld = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        else
        {
            lightsReal = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            lightsApprox = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }


        ambientColor = ambientColor.Read(reader);
        fogStart = reader.ReadFloat();
        fogEnd = reader.ReadFloat();


        if (revision < 1)
        {
            unkInt1 = reader.ReadUInt32();
        }

        fogColor = fogColor.Read(reader);


        if (revision < 1)
        {
            fogEnabled = reader.ReadUInt32() == 1;
        }
        else
        {
            fogEnabled = reader.ReadBoolean();
        }

        if (revision > 3)
        {
            animateFromPreset = reader.ReadBoolean();
        }

        if (revision > 4)
        {
            fadeOut = reader.ReadBoolean();
            fadeStart = reader.ReadFloat();
            fadeEnd = reader.ReadFloat();

            if (revision > 5)
            {
                fadeMax = reader.ReadFloat();
            }
        }

        if (revision > 8)
        {
            fadeRef = Symbol.Read(reader);

            left_out = reader.ReadFloat();
            left_opaque = reader.ReadFloat();
            right_out = reader.ReadFloat();
            right_opaque = reader.ReadFloat();
        }

        if (revision > 6)
        {
            ambientFogOwner = Symbol.Read(reader);
        }

        if (revision > 7)
        {
            useColorAdjust = reader.ReadBoolean();
            colorAdjust = colorAdjust.Read(reader);
        }

        if (revision > 9)
        {
            if (revision < 0xD)
            {
                unkInt2 = reader.ReadUInt32();
            }
            aoStrength = reader.ReadFloat();
        }

        if (revision > 10)
        {
            intensityRate = reader.ReadFloat();
            exposure = reader.ReadFloat();
            whitePoint = reader.ReadFloat();
            useToneMapping = reader.ReadBoolean();
        }

        if (revision == 0xB || revision == 12 || revision == 13)
        {
            unkInt3 = reader.ReadUInt32();
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

        if (parent.revision == 25 && standalone && revision == 7)
        {
            writer.WriteBlock(gh2OpaqueBytes);
            writer.WriteEndBytes();
            return;
        }

        if (revision > 1)
        {
            objFields.Write(writer, parent);
        }

        if (revision < 3)
        {
            draw.Write(writer, false, parent, null);
        }

        if (revision > 15)
        {
            draw.Write(writer, false, parent, entry);
            trans.Write(writer, false, parent, entry);
        }

        if (revision < 0xF)
        {
            AssetHelpers.WriteList(writer, lightsOld, light => Symbol.Write(writer, light));
        }
        else
        {
            AssetHelpers.WriteList(writer, lightsReal, light => Symbol.Write(writer, light));

            AssetHelpers.WriteList(writer, lightsApprox, light => Symbol.Write(writer, light));
        }

        ambientColor.Write(writer);

        writer.WriteFloat(fogStart);
        writer.WriteFloat(fogEnd);

        if (revision < 1)
        {
            writer.WriteUInt32(unkInt1);
        }

        fogColor.Write(writer);

        if (revision < 1)
        {
            writer.WriteUInt32(fogEnabled ? 1u : 0u);
        }
        else
        {
            writer.WriteBoolean(fogEnabled);
        }

        if (revision > 3)
        {
            writer.WriteBoolean(animateFromPreset);
        }

        if (revision > 4)
        {
            writer.WriteBoolean(fadeOut);
            writer.WriteFloat(fadeStart);
            writer.WriteFloat(fadeEnd);

            if (revision > 5)
            {
                writer.WriteFloat(fadeMax);
            }
        }

        if (revision > 8)
        {
            Symbol.Write(writer, fadeRef);

            writer.WriteFloat(left_out);
            writer.WriteFloat(left_opaque);
            writer.WriteFloat(right_out);
            writer.WriteFloat(right_opaque);
        }

        if (revision > 6)
        {
            Symbol.Write(writer, ambientFogOwner);
        }

        if (revision > 7)
        {
            writer.WriteBoolean(useColorAdjust);
            colorAdjust.Write(writer);
        }

        if (revision > 9)
        {
            if (revision < 0xD)
            {
                writer.WriteUInt32(unkInt2);
            }
            writer.WriteFloat(aoStrength);
        }

        if (revision > 10)
        {
            writer.WriteFloat(intensityRate);
            writer.WriteFloat(exposure);
            writer.WriteFloat(whitePoint);
            writer.WriteBoolean(useToneMapping);
        }

        if (revision == 0xB || revision == 12 || revision == 13)
        {
            writer.WriteUInt32(unkInt3);
        }


        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
