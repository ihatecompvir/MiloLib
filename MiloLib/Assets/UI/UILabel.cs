using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UILabel"), Description("Simple label, provides localization of tokens")]
public partial class UILabel : UIComponent
{
    public enum TextAlignments
    {
        kTopLeft,
        kTopCenter,
        kTopRight,
        kMiddleLeft,
        kMiddleCenter,
        kMiddleRight,
        kBottomLeft,
        kBottomCenter,
        kBottomRight,
    }

    public enum CapsModes
    {
        kCapsModeNone,
        kCapsModeLower,
        kCapsModeUpper,
    }

    public enum LabelFitTypes
    {
        kFitWrap,
        kFitJust,
        kFitEllipsis
    }
    private ushort altRevision;
    private ushort revision;
    private bool usesLegacyAlpha;

    private static bool HasLegacyAlphaTail(EndianReader reader, DirectoryMeta.Entry entry)
    {
        long position = reader.BaseStream.Position;
        try
        {
            float candidate = reader.ReadFloat();
            if (!float.IsFinite(candidate) || candidate < 0 || candidate > 1)
            {
                return false;
            }

            if (entry.type.value == "BandLabel" || entry.type.value == "BandButton")
            {
                uint componentRevision = reader.ReadUInt32();
                if (componentRevision > 2)
                {
                    return false;
                }

                if (componentRevision == 0)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        reader.ReadInt32();
                    }
                }
                Symbol colorType = Symbol.Read(reader);
                if (colorType.value == "custom_colors")
                {
                    uint count = 4;
                    if (componentRevision == 2)
                    {
                        count = reader.ReadUInt32();
                    }

                    if (count > (reader.BaseStream.Length - reader.BaseStream.Position) / 4)
                    {
                        return false;
                    }

                    reader.BaseStream.Position += count * 4L;
                }
                if (entry.type.value == "BandButton")
                {
                    Symbol.Read(reader);
                }
            }
            else if (entry.type.value != "UILabel" && entry.type.value != "UIButton")
            {
                return false;
            }
            uint expected = 0xADDEADDE;
            if (reader.Endianness == Endian.LittleEndian)
            {
                expected = 0xDEADDEAD;
            }

            return reader.ReadUInt32() == expected;
        }
        catch (InvalidDataException)
        {
            return false;
        }
        catch (EndOfStreamException)
        {
            return false;
        }
        finally
        {
            reader.BaseStream.Position = position;
        }
    }

    [Name("Text Token"), Description("Localization token if localize is true")]
    public Symbol textToken = Symbol.Empty;
    [Name("Icon"), Description("Single-character icon"), MinVersion(0xF)]
    public Symbol icon = Symbol.Empty;
    [MinVersion(25)]
    public byte iconByte;


    [Name("Text Size"), Description("Text size in percentage of screen height (i.e. 50% is half the screen height for the largest glyph)"), MinVersion(2)]
    public float textSize;
    [Name("Alignment"), Description("Text alignment"), MinVersion(2)]
    public TextAlignments alignment;
    [Name("Caps Mode"), Description("Text case setting"), MinVersion(2)]
    public CapsModes capsMode;

    [Name("Supports Markup"), Description("Support markup?"), MinVersion(8)]
    public bool supportsMarkup;
    [Name("Leading"), Description("Space between lines"), MinVersion(2)]
    public float leading;
    [Name("Kerning"), Description("Additional kerning applied to text object"), MinVersion(2)]
    public float kerning;
    [Name("Italics"), Description("Italics for text object"), MinVersion(5)]
    public float italics;

    [Name("Fit Type"), Description("How to fit text in the width/height specified"), MinVersion(3)]
    public LabelFitTypes fitType;
    [Name("Width"), Description("Width of label"), MinVersion(3)]
    public float width;
    [Name("Height"), Description("Height of label"), MinVersion(3)]
    public float height;

    [Name("Fixed Length"), Description("Preallocated size of internal text object"), MinVersion(6)]
    public int fixedLength;
    [Name("Reserved Lines"), Description("Preallocated number of lines in internal text object"), MinVersion(7)]
    public int reservedLines;

    [Name("Preserve Truncated Text"), Description("Optional text to append after truncation with kFitEllipsis"), MinVersion(10)]
    public Symbol preserveTruncText = Symbol.Empty;


    [Name("Alpha"), Description("Controls transparency of label"), MinVersion(11)]
    public float alpha;
    [Name("Color Override"), Description("Color override for this instance"), MinVersion(0xD)]
    public Symbol colorOverride = Symbol.Empty;
    [Name("Font Material Variation"), Description("the material variation to use, populated from those setup in the resource file. selecting none gives you the default material."), MinVersion(0x15)]
    public Symbol fontMatVariation = Symbol.Empty;
    [Name("Alternate Material Variation"), Description("the material variation to use from the alternate font. selecting none gives you the default material."), MinVersion(0x17)]
    public Symbol altMatVariation = Symbol.Empty;
    [Name("Alternate Text Size"), Description("Text size of alternate style in percentage of screen height (i.e. 50% is half the screen height for the largest glyph)"), MinVersion(0x12)]
    public float altTextSize;
    [Name("Alternate Kerning"), Description("Additional kerning applied to alt text object"), MinVersion(0x13)]
    public float altKerning;
    [Name("Alternate Text Color"), Description("Color to use when in alt style"), MinVersion(0x12)]
    public Symbol altTextColor = Symbol.Empty;
    [Name("Alternate Z Offset"), Description("Z-offset for alt text (to manually match up baselines)"), MinVersion(0x14)]
    public float altZOffset;
    [Name("Alternate Italics"), Description("Italics for text object's alt font"), MinVersion(0x18)]
    public float altItalics;
    [Name("Alternate Alpha"), Description("Controls transparency of label's alt font"), MinVersion(0x18)]
    public float altAlpha;
    [Name("Use Highlight Mesh"), Description("whether or not to use highlight mesh (if available)"), MinVersion(0x11)]
    public bool useHighlightMesh;
    [Name("Alternate Style Enabled"), Description("Whether to parse <alt> tags for alt style"), MinVersion(0x12)]
    public bool altStyleEnabled;
    [Name("Alternate Font Resource Name"), Description("path to alt font resource file for this component"), MinVersion(0x16)]
    public Symbol altFontResourceName = Symbol.Empty;

    [MinVersion(14)]
    public Symbol unkSymbol = Symbol.Empty;
    [MinVersion(1), MaxVersion(13)]
    public bool unkBool;
    [MinVersion(9), MaxVersion(15)]
    public bool unkBool2;
    [MinVersion(9), MaxVersion(15)]
    public int unkIntA;
    [MinVersion(9), MaxVersion(15)]
    public int unkIntC;
    [MinVersion(9), MaxVersion(15)]
    public int unkIntD;

    public ushort UILabelRevision => revision;
    public ushort UILabelAltRevision => altRevision;

    public void SetUILabelRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }


    public new UILabel Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        usesLegacyAlpha = false;
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        if (revision >= 28)
        {
            ReadStyledLabel(reader);
            if (standalone)
            {
                AssetHelpers.ReadEnd(reader, parent, entry, revision);
            }
            return this;
        }
        if (revision != 0 && revision < 0xE)
        {
            unkBool = reader.ReadBoolean();
        }
        textToken = Symbol.Read(reader);

        if (revision > 0xD)
        {
            unkSymbol = Symbol.Read(reader);
        }

        if (revision > 0xE)
        {
            if (revision < 25)
            {
                icon = Symbol.Read(reader);
            }
            else
            {
                iconByte = reader.ReadByte();
            }
        }

        if (revision > 1)
        {
            textSize = reader.ReadFloat();
            alignment = (TextAlignments)reader.ReadInt32();
            capsMode = (CapsModes)reader.ReadInt32();

            if (revision > 7)
            {
                supportsMarkup = reader.ReadBoolean();
            }
            leading = reader.ReadFloat();
            kerning = reader.ReadFloat();
        }
        if (revision > 4)
        {
            italics = reader.ReadFloat();
        }
        if (revision > 2)
        {
            fitType = (LabelFitTypes)reader.ReadInt32();
            width = reader.ReadFloat();
            height = reader.ReadFloat();
        }
        if (revision > 5)
        {
            fixedLength = reader.ReadInt32();
        }
        if (revision > 6 && revision < 27)
        {
            reservedLines = reader.ReadInt32();
        }
        if (revision >= 9 && revision <= 15)
        {
            usesLegacyAlpha = parent.revision == 25 && revision == 9 && HasLegacyAlphaTail(reader, entry);
            if (usesLegacyAlpha)
            {
                alpha = reader.ReadFloat();
            }
            else
            {
                unkBool2 = reader.ReadBoolean();
                unkIntA = reader.ReadInt32();
                unkIntC = reader.ReadInt32();
                unkIntD = reader.ReadInt32();
            }
        }

        if (revision > 9 && revision < 26)
        {
            preserveTruncText = Symbol.Read(reader);
        }
        if (revision > 10)
        {
            alpha = reader.ReadFloat();
        }
        if (revision > 0xC)
        {
            colorOverride = Symbol.Read(reader);
        }
        if (revision > 0x10)
        {
            useHighlightMesh = reader.ReadBoolean();
        }
        if (revision > 0x11)
        {
            altTextSize = reader.ReadFloat();
            altTextColor = Symbol.Read(reader);
            altStyleEnabled = reader.ReadBoolean();
        }
        if (revision > 0x12)
        {
            altKerning = reader.ReadFloat();
        }
        else
        {
            altKerning = kerning;
        }
        if (revision > 0x13)
        {
            altZOffset = reader.ReadFloat();
        }
        if (revision > 0x14)
        {
            fontMatVariation = Symbol.Read(reader);
        }
        if (revision > 0x15)
        {
            altFontResourceName = Symbol.Read(reader);
        }
        if (revision > 0x16)
        {
            altMatVariation = Symbol.Read(reader);
        }
        if (revision > 0x17)
        {
            altItalics = reader.ReadFloat();
            altAlpha = reader.ReadFloat();
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
        if (revision >= 28)
        {
            WriteStyledLabel(writer);
            if (standalone)
            {
                writer.WriteEndBytes();
            }
            return;
        }

        if (revision != 0 && revision < 0xE)
        {
            writer.WriteBoolean(unkBool);
        }
        Symbol.Write(writer, textToken);

        if (revision > 0xD)
        {
            Symbol.Write(writer, unkSymbol);
        }

        if (revision > 0xE)
        {
            if (revision < 25)
            {
                Symbol.Write(writer, icon);
            }
            else
            {
                writer.WriteByte(iconByte);
            }
        }

        if (revision > 1)
        {
            writer.WriteFloat(textSize);
            writer.WriteInt32((int)alignment);
            writer.WriteInt32((int)capsMode);

            if (revision > 7)
            {
                writer.WriteBoolean(supportsMarkup);
            }
            writer.WriteFloat(leading);
            writer.WriteFloat(kerning);
        }
        if (revision > 4)
        {
            writer.WriteFloat(italics);
        }
        if (revision > 2)
        {
            writer.WriteInt32((int)fitType);
            writer.WriteFloat(width);
            writer.WriteFloat(height);
        }
        if (revision > 5)
        {
            writer.WriteInt32(fixedLength);
        }
        if (revision > 6 && revision < 27)
        {
            writer.WriteInt32(reservedLines);
        }
        if (revision >= 9 && revision <= 15)
        {
            if (usesLegacyAlpha)
            {
                writer.WriteFloat(alpha);
            }
            else
            {
                writer.WriteBoolean(unkBool2);
                writer.WriteInt32(unkIntA);
                writer.WriteInt32(unkIntC);
                writer.WriteInt32(unkIntD);
            }
        }
        if (revision > 9 && revision < 26)
        {
            Symbol.Write(writer, preserveTruncText);
        }
        if (revision > 10)
        {
            writer.WriteFloat(alpha);
        }
        if (revision > 0xC)
        {
            Symbol.Write(writer, colorOverride);
        }
        if (revision > 0x10)
        {
            writer.WriteBoolean(useHighlightMesh);
        }
        if (revision > 0x11)
        {
            writer.WriteFloat(altTextSize);
            Symbol.Write(writer, altTextColor);
            writer.WriteBoolean(altStyleEnabled);
        }
        if (revision > 0x12)
        {
            writer.WriteFloat(altKerning);
        }
        if (revision > 0x13)
        {
            writer.WriteFloat(altZOffset);
        }
        if (revision > 0x14)
        {
            Symbol.Write(writer, fontMatVariation);
        }
        if (revision > 0x15)
        {
            Symbol.Write(writer, altFontResourceName);
        }
        if (revision > 0x16)
        {
            Symbol.Write(writer, altMatVariation);
        }
        if (revision > 0x17)
        {
            writer.WriteFloat(altItalics);
            writer.WriteFloat(altAlpha);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
