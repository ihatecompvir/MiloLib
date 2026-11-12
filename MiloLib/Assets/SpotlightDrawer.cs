using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("SpotlightDrawer"), Description("A SpotlightDrawer draws spotlights.")]
public class SpotlightDrawer : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();

    [Name("Intensity"), Description("global intensity scale")]
    public float intensity;
    [Name("Color"), Description("color of ambient (unlit) fog")]
    public HmxColor4 color = new();
    [Name("Base Intensity"), Description("intensity of smokeless beam")]
    public float baseIntensity;
    [Name("Smoke Intensity"), Description("intensity from smoke")]
    public float smokeIntensity;
    public float halfDistance;
    [Name("Lighting Influence"), Description("The amount the spotlights will influence the real lighting of the world")]
    public float lightingInfluence;
    public float lightingInfluence2;
    public Symbol texture = Symbol.Empty;
    [Name("Proxy Fog Object"), Description("proxy fog object")]
    public Symbol proxy = Symbol.Empty;

    [MaxVersion(3)]
    public float unkFloat1;
    [MaxVersion(3)]
    public float unkFloat2;
    [MaxVersion(3)]
    public float unkFloat3;
    [MaxVersion(3)]
    public float unkFloat4;

    [MaxVersion(3)]
    public uint unkInt1;
    [MaxVersion(3)]
    public uint unkInt2;
    [MaxVersion(3)]
    public uint unkInt3;
    [MaxVersion(3)]
    public uint unkInt4;
    [MaxVersion(3)]
    public uint unkInt5;

    [MaxVersion(2)]
    public bool unkBool;


    public new SpotlightDrawer Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 0)
        {
            if (revision > 5)
            {
                base.Read(reader, false, parent, entry);
            }
            draw.Read(reader, false, parent, entry);
        }
        else
        {
            base.objFields.Read(reader, parent, entry);
        }

        intensity = reader.ReadFloat();

        if (revision > 3)
        {
            smokeIntensity = reader.ReadFloat();
            halfDistance = reader.ReadFloat();
            lightingInfluence = reader.ReadFloat();
        }
        else
        {
            unkFloat1 = reader.ReadFloat();
            unkFloat2 = reader.ReadFloat();
            unkFloat3 = reader.ReadFloat();
            unkFloat4 = reader.ReadFloat();
        }

        color.Read(reader);

        if (revision < 4)
        {
            unkInt1 = reader.ReadUInt32();
            unkInt2 = reader.ReadUInt32();
            unkInt3 = reader.ReadUInt32();
            unkInt4 = reader.ReadUInt32();
            unkInt5 = reader.ReadUInt32();
        }

        texture = Symbol.Read(reader);
        proxy = Symbol.Read(reader);

        if (revision < 3)
        {
            unkBool = reader.ReadBoolean();
        }
        else if (revision > 4)
        {
            lightingInfluence2 = reader.ReadFloat();
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

        if (revision > 0)
        {
            if (revision > 5)
            {
                base.Write(writer, false, parent, entry);
            }
            draw.Write(writer, false, parent, null);
        }
        else
        {
            base.objFields.Write(writer, parent);
        }

        writer.WriteFloat(intensity);

        if (revision > 3)
        {
            writer.WriteFloat(smokeIntensity);
            writer.WriteFloat(halfDistance);
            writer.WriteFloat(lightingInfluence);
        }
        else
        {
            writer.WriteFloat(unkFloat1);
            writer.WriteFloat(unkFloat2);
            writer.WriteFloat(unkFloat3);
            writer.WriteFloat(unkFloat4);
        }

        color.Write(writer);

        if (revision < 4)
        {
            writer.WriteUInt32(unkInt1);
            writer.WriteUInt32(unkInt2);
            writer.WriteUInt32(unkInt3);
            writer.WriteUInt32(unkInt4);
            writer.WriteUInt32(unkInt5);
        }

        Symbol.Write(writer, texture);
        Symbol.Write(writer, proxy);

        if (revision < 3)
        {
            writer.WriteBoolean(unkBool);
        }
        else if (revision > 4)
        {
            writer.WriteFloat(lightingInfluence2);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
