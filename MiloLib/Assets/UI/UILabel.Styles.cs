using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

public partial class UILabel
{
    public class LabelStyle
    {
        [Name("Font Resource"), Description("path to font resource file for this style, click the open button to open the file")]
        public Symbol fontResource = Symbol.Empty;
        [Name("Color Override"), Description("Color override for this style text")]
        public Symbol colorOverride = Symbol.Empty;
        [Name("Text Size"), Description("Text size in percentage of screen height (i.e. 50% is half the screen height for the largest glyph)")]
        public float size;
        [Name("Kerning"), Description("Additional kerning applied to text")]
        public float kerning;
        [Name("Z Offset"), Description("Z-offset for text")]
        public float zOffset;
        [Name("Italics"), Description("Italics for text")]
        public float italics;
        [Name("Font Alpha"), Description("transparency of font")]
        public float alpha = 1.0f;
        [Name("Blacklight"), Description("draw in blacklight pass?")]
        public bool blacklight;
        [Name("Font Mat Variation"), Description("the material variation to use, populated from the resource file. selecting <none> gives you the default material.")]
        public Symbol fontMat = Symbol.Empty;

        public LabelStyle Read(EndianReader reader, ushort revision)
        {
            fontResource = Symbol.Read(reader);
            colorOverride = Symbol.Read(reader);
            size = reader.ReadFloat();
            kerning = reader.ReadFloat();
            zOffset = reader.ReadFloat();
            italics = reader.ReadFloat();
            alpha = reader.ReadFloat();
            if (revision >= 30)
            {
                blacklight = reader.ReadBoolean();
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, fontResource);
            Symbol.Write(writer, colorOverride);
            writer.WriteFloat(size);
            writer.WriteFloat(kerning);
            writer.WriteFloat(zOffset);
            writer.WriteFloat(italics);
            writer.WriteFloat(alpha);
            if (revision >= 30)
            {
                writer.WriteBoolean(blacklight);
            }
        }
    }

    [Name("Styles"), Description("Alternate styles, set [markup] true and use <alt> tag for styles[1], <alt2> for styles[2], <alt3> for style[3], etc")]
    public List<LabelStyle> labelStyles = new();
    [Name("Circle"), Description("Lay text around circle of this circumference. Negative values face other way.")]
    public float circle;
    [Name("Scroll Delay"), Description("If scrolling oversized text - delay this many seconds before starting")]
    public float scrollDelay;
    [Name("Scroll Rate"), Description("If scrolling oversized text - scroll this many characters per second")]
    public float scrollRate;
    [Name("Scroll Pause"), Description("If scrolling oversized text - delay this many seconds between scrolls. When the fit type is kFitScrollMarqueeWrapAlways, this value will be ignored.")]
    public float scrollPause;
    [Name("Indentation"), Description("Space between continuous scrolling messages. This value is only considered when the fit type is set to kFitScrollMarqueeWrapAlways.")]
    public float indentation;
    [Name("Basic Markup"), Description("Support basic markup or not. It converts \\p to double-quotes. Furthur support can be added.")]
    public bool basicMarkup;

    private void ReadStyledLabel(EndianReader reader)
    {
        textToken = Symbol.Read(reader);
        unkSymbol = Symbol.Read(reader);
        iconByte = reader.ReadByte();
        alignment = (TextAlignments)reader.ReadInt32();
        width = reader.ReadFloat();
        leading = reader.ReadFloat();
        fixedLength = reader.ReadInt32();
        supportsMarkup = reader.ReadBoolean();
        capsMode = (CapsModes)reader.ReadInt32();
        height = reader.ReadFloat();
        if (altRevision > 0)
        {
            circle = reader.ReadFloat();
        }
        fitType = (LabelFitTypes)reader.ReadInt32();
        labelStyles = AssetHelpers.ReadList(reader, () => new LabelStyle().Read(reader, revision));
        if (revision >= 31)
        {
            scrollDelay = reader.ReadFloat();
            scrollRate = reader.ReadFloat();
            scrollPause = reader.ReadFloat();
        }
        if (revision >= 32)
        {
            indentation = reader.ReadFloat();
        }
        if (revision >= 33)
        {
            basicMarkup = reader.ReadBoolean();
        }
        foreach (LabelStyle style in labelStyles)
        {
            style.fontMat = Symbol.Read(reader);
        }
    }

    private void WriteStyledLabel(EndianWriter writer)
    {
        Symbol.Write(writer, textToken);
        Symbol.Write(writer, unkSymbol);
        writer.WriteByte(iconByte);
        writer.WriteInt32((int)alignment);
        writer.WriteFloat(width);
        writer.WriteFloat(leading);
        writer.WriteInt32(fixedLength);
        writer.WriteBoolean(supportsMarkup);
        writer.WriteInt32((int)capsMode);
        writer.WriteFloat(height);
        if (altRevision > 0)
        {
            writer.WriteFloat(circle);
        }
        writer.WriteInt32((int)fitType);
        AssetHelpers.WriteList(writer, labelStyles, value => value.Write(writer, revision));
        if (revision >= 31)
        {
            writer.WriteFloat(scrollDelay);
            writer.WriteFloat(scrollRate);
            writer.WriteFloat(scrollPause);
        }
        if (revision >= 32)
        {
            writer.WriteFloat(indentation);
        }
        if (revision >= 33)
        {
            writer.WriteBoolean(basicMarkup);
        }
        foreach (LabelStyle style in labelStyles)
        {
            Symbol.Write(writer, style.fontMat);
        }
    }
}
