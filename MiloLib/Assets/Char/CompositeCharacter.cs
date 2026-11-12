using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CompositeCharacter"), Description("Character with outfits and compositing decal texture tech")]
public class CompositeCharacter : Character
{
    [HideInInspector]
    private ushort altRevision;
    [HideInInspector]
    private ushort revision;

    [Name("Frozen"), Description("if true, is frozen in place"), MinVersion(10)]
    public bool compositeFrozen;

    [Name("Clip Mode"), Description("Which clips it will load"), MinVersion(12)]
    public int clipMode;

    [Name("Challenge"), Description("Challenge type"), MinVersion(19)]
    public int challenge;

    [Name("Skin Color Index"), Description("Skin color index")]
    public int skinColorIndex;
    [Name("Skin Color Palette"), Description("Skin color palette")]
    public Symbol skinColorPalette = Symbol.Empty;

    [Name("Eye Color Index"), Description("Eye color index"), MinVersion(6)]
    public int eyeColorIndex;
    [Name("Eye Color Palette"), Description("Eye color palette"), MinVersion(6)]
    public Symbol eyeColorPalette = Symbol.Empty;

    [Name("Eyebrow Color Index"), Description("Eyebrow color index, uses hair.pal"), MinVersion(9)]
    public int eyebrowColorIndex;

    [MinVersion(4)]
    public int flags;

    [Name("Gender"), Description("gender"), MinVersion(5)]
    public Symbol gender = Symbol.Empty;

    [Name("Skin Tattoo Palette"), Description("Color palette to apply to tattoo's"), MinVersion(8)]
    public Symbol skinTattooPalette = Symbol.Empty;

    [Name("Skin Shader"), Description("Always use this skin shader for this character"), MinVersion(16)]
    public Symbol skinShader = Symbol.Empty;

    [MinVersion(14)]
    public Symbol prefabName = Symbol.Empty;

    [Name("Height"), Description("Deformation height 0-1"), MinVersion(18)]
    public float height;
    [Name("Weight"), Description("Deformation weight 0-1"), MinVersion(18)]
    public float weight;

    public CompositeCharacter(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public override bool AllowsInlineProxy()
    {
        // handle lego
        return false;
    }

    public new CompositeCharacter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision <= 2)
        {
            if (standalone)
            {
                AssetHelpers.ReadEnd(reader, parent, entry, revision);
            }

            return this;
        }

        if (revision >= 10)
        {
            compositeFrozen = reader.ReadBoolean();
        }

        if (revision >= 12)
        {
            clipMode = reader.ReadInt32();
        }

        if (revision >= 19)
        {
            challenge = reader.ReadInt32();
        }

        bool readCompositeBody = !ShouldTreatAsProxyForCharacterBody(entry, standalone) || revision > 16;
        if (readCompositeBody)
        {
            skinColorIndex = reader.ReadInt32();
            skinColorPalette = Symbol.Read(reader);

            if (revision >= 6)
            {
                eyeColorIndex = reader.ReadInt32();
                eyeColorPalette = Symbol.Read(reader);
            }

            if (revision >= 9)
            {
                eyebrowColorIndex = reader.ReadInt32();
            }

            if (revision >= 4)
            {
                flags = reader.ReadInt32();
            }

            if (revision >= 5)
            {
                gender = Symbol.Read(reader);
            }

            if (revision >= 8)
            {
                skinTattooPalette = Symbol.Read(reader);
            }

            if (revision == 14)
            {
                prefabName = Symbol.Read(reader);
            }

            if (revision >= 16)
            {
                skinShader = Symbol.Read(reader);
            }
        }

        if (revision > 14)
        {
            prefabName = Symbol.Read(reader);
        }

        if (revision >= 18)
        {
            height = reader.ReadFloat();
            weight = reader.ReadFloat();
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

        if (revision <= 2)
        {
            if (standalone)
            {
                writer.WriteEndBytes();
            }

            return;
        }

        if (revision >= 10)
        {
            writer.WriteBoolean(compositeFrozen);
        }

        if (revision >= 12)
        {
            writer.WriteInt32(clipMode);
        }

        if (revision >= 19)
        {
            writer.WriteInt32(challenge);
        }

        bool writeCompositeBody = !ShouldTreatAsProxyForCharacterBody(entry, standalone) || revision > 16;
        if (writeCompositeBody)
        {
            writer.WriteInt32(skinColorIndex);
            Symbol.Write(writer, skinColorPalette);

            if (revision >= 6)
            {
                writer.WriteInt32(eyeColorIndex);
                Symbol.Write(writer, eyeColorPalette);
            }

            if (revision >= 9)
            {
                writer.WriteInt32(eyebrowColorIndex);
            }

            if (revision >= 4)
            {
                writer.WriteInt32(flags);
            }

            if (revision >= 5)
            {
                Symbol.Write(writer, gender);
            }

            if (revision >= 8)
            {
                Symbol.Write(writer, skinTattooPalette);
            }

            if (revision == 14)
            {
                Symbol.Write(writer, prefabName);
            }

            if (revision >= 16)
            {
                Symbol.Write(writer, skinShader);
            }
        }

        if (revision > 14)
        {
            Symbol.Write(writer, prefabName);
        }

        if (revision >= 18)
        {
            writer.WriteFloat(height);
            writer.WriteFloat(weight);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }
}
