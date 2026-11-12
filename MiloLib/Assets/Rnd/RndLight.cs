using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Light"), Description("Light objects are added to environments for drawing.")]
public class RndLight : Object
{
    public enum Type
    {
        kPoint = 0,
        kDirectional = 1,
        kSpot = 2,
        kFloorSpot = 3,
        kShadowRef = 4
    }

    private ushort altRevision;
    private ushort revision;

    private bool UsesLegacyWiiLayout(DirectoryMeta parent)
    {
        return parent.revision == 25 && parent.platform == DirectoryMeta.Platform.Wii && revision == 14;
    }
    [Name("Color"), Description("Color of light")]
    public HmxColor4 color = new();
    [MaxVersion(1)]
    public HmxColor3 oldColor1 = new();
    [MaxVersion(1)]
    public HmxColor3 oldColor2 = new();
    [MaxVersion(2)]
    public int oldInt1;
    [MaxVersion(2)]
    public int oldInt2;
    [Name("Color Owner"), Description("Master for light color and intensity"), MinVersion(11)]
    public Symbol colorOwner = Symbol.Empty;
    [Name("Range"), Description("Falloff distance for point lights"), MinVersion(0)]
    public float range;
    [Name("Falloff Start"), Description("Distance at which falloff starts for point lights"), MinVersion(12)]
    public float falloffStart;
    [Name("Light Type"), Description("Type of dynamic lighting"), MinVersion(1)]
    public Type type;
    [Name("Animate Color From Preset"), Description("Animation authority for LightPreset"), MinVersion(6)]
    public bool animateColorFromPreset;
    [Name("Animate Position From Preset"), Description("Animation authority for LightPreset"), MinVersion(6)]
    public bool animatePositionFromPreset;
    [Name("Animate Range From Preset"), Description("Animate light's range from a LightPreset"), MinVersion(16)]
    public bool animateRangeFromPreset;

    [Name("Texture"), Description("Projected texture"), MinVersion(8)]
    public Symbol texture = Symbol.Empty;
    [Name("Cube Texture"), Description("Projected cube map texture"), MinVersion(14)]
    public Symbol cubeTex = Symbol.Empty;

    private uint shadowObjectsCount;

    [Name("Shadow Objects"), Description("These objects will cast shadows for the projected light"), MinVersion(15)]
    public List<Symbol> shadowObjects = new();

    [Name("Texture Xfm"), MinVersion(13)]
    public Matrix textureTransform = new();
    [Name("Top Radius"), Description("Fake cone small radius at the source"), MinVersion(7)]
    public float topRadius;
    [Name("Bottom Radius"), Description("Fake cone big radius at the far end"), MinVersion(7)]
    public float bottomRadius;
    [Name("Projected Blend"), Description("Specifies blending for the projected light"), MinVersion(15)]
    public int projectedBlend;

    [MinVersion(9), MaxVersion(9)]
    public List<Symbol> drawList = new();
    [MinVersion(8), MaxVersion(8)]
    public Symbol draw = Symbol.Empty;

    public RndTrans trans = new();

    public bool onlyProjection;

    private int rawType;

    [MinVersion(7), MaxVersion(13)]
    public uint unkInt1;
    [MinVersion(7), MaxVersion(13)]
    public uint unkInt2;
    [MaxVersion(2)]
    public uint unkInt3;
    [MaxVersion(2)]
    public uint unkInt4;
    [MaxVersion(2)]
    public uint unkInt5;

    public new RndLight Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 3)
        {
            base.objFields.Read(reader, parent, entry);
        }

        trans.Read(reader, false, parent, entry);

        color = color.Read(reader);

        if (revision < 2)
        {
            oldColor1 = new HmxColor3().Read(reader);
            oldColor2 = new HmxColor3().Read(reader);
        }

        if (revision < 3)
        {
            oldInt1 = reader.ReadInt32();
            oldInt2 = reader.ReadInt32();
        }

        range = reader.ReadFloat();

        if (revision < 3)
        {
            unkInt3 = reader.ReadUInt32();
            unkInt4 = reader.ReadUInt32();
            unkInt5 = reader.ReadUInt32();
        }

        if (revision != 0)
        {
            int count = reader.ReadInt32();
            rawType = count;
            if (revision < 0xE || UsesLegacyWiiLayout(parent))
            {
                if (count > 1)
                {
                    count--;
                }
            }
            type = (Type)count;
        }

        if (revision > 0xB)
        {
            falloffStart = reader.ReadFloat();
        }

        if (revision > 5)
        {
            animateColorFromPreset = reader.ReadBoolean();
            animatePositionFromPreset = reader.ReadBoolean();
        }

        if (revision > 6)
        {
            topRadius = reader.ReadFloat();
            bottomRadius = reader.ReadFloat();
            if (revision < 0xE || UsesLegacyWiiLayout(parent))
            {
                unkInt1 = reader.ReadUInt32();
                unkInt2 = reader.ReadUInt32();
            }
        }

        if (revision > 7)
        {
            texture = Symbol.Read(reader);
            if (revision == 9)
            {
                drawList = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            else if (revision == 8)
            {
                draw = Symbol.Read(reader);
            }
        }

        if (revision > 10)
        {
            colorOwner = Symbol.Read(reader);
        }

        if (revision > 0xC)
        {
            textureTransform = textureTransform.Read(reader);
        }

        if (revision > 0xD && !UsesLegacyWiiLayout(parent))
        {
            cubeTex = Symbol.Read(reader);
        }

        if (altRevision != 0 || UsesLegacyWiiLayout(parent))
        {
            onlyProjection = reader.ReadBoolean();
        }

        if (revision > 0xE)
        {
            shadowObjects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            projectedBlend = reader.ReadInt32();
        }

        if (revision > 0xF)
        {
            animateRangeFromPreset = reader.ReadBoolean();
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

        if (revision > 3)
        {
            base.objFields.Write(writer, parent);
        }

        trans.Write(writer, false, parent, null);

        color.Write(writer);

        if (revision < 2)
        {
            oldColor1.Write(writer);
            oldColor2.Write(writer);
        }

        if (revision < 3)
        {
            writer.WriteInt32(oldInt1);
            writer.WriteInt32(oldInt2);
        }

        writer.WriteFloat(range);

        if (revision < 3)
        {
            writer.WriteUInt32(unkInt3);
            writer.WriteUInt32(unkInt4);
            writer.WriteUInt32(unkInt5);
        }

        if (revision != 0)
        {
            if (revision < 0xE || UsesLegacyWiiLayout(parent))
            {
                writer.WriteInt32(rawType);
            }
            else
            {
                writer.WriteInt32((int)type);
            }
        }

        if (revision > 0xB)
        {
            writer.WriteFloat(falloffStart);
        }

        if (revision > 5)
        {
            writer.WriteBoolean(animateColorFromPreset);
            writer.WriteBoolean(animatePositionFromPreset);
        }

        if (revision > 6)
        {
            writer.WriteFloat(topRadius);
            writer.WriteFloat(bottomRadius);
            if (revision < 0xE || UsesLegacyWiiLayout(parent))
            {
                writer.WriteUInt32(unkInt1);
                writer.WriteUInt32(unkInt2);
            }
        }

        if (revision > 7)
        {
            Symbol.Write(writer, texture);
            if (revision == 9)
            {
                AssetHelpers.WriteList(writer, drawList, (Symbol draw) => Symbol.Write(writer, draw));
            }
            else if (revision == 8)
            {
                Symbol.Write(writer, draw);
            }
        }

        if (revision > 10)
        {
            Symbol.Write(writer, colorOwner);
        }

        if (revision > 0xC)
        {
            textureTransform.Write(writer);
        }

        if (revision > 0xD && !UsesLegacyWiiLayout(parent))
        {
            Symbol.Write(writer, cubeTex);
        }

        if (altRevision != 0 || UsesLegacyWiiLayout(parent))
        {
            writer.WriteBoolean(onlyProjection);
        }

        if (revision > 0xE)
        {
            shadowObjectsCount = (uint)shadowObjects.Count;
            AssetHelpers.WriteList(writer, shadowObjects, (Symbol shadowObject) => Symbol.Write(writer, shadowObject));
            writer.WriteInt32(projectedBlend);
        }

        if (revision > 0xF)
        {
            writer.WriteBoolean(animateRangeFromPreset);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
