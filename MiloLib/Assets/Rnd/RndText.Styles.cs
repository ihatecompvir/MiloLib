using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

public partial class RndText
{
    [Name("Height"), Description("Height of the text, used for [fit_type] kFitJust")]
    public float height;
    [Name("Circle"), Description("Lay text around circle of this circumference. Negative values face other way.")]
    public float circle;
    [Name("Fit Type"), Description("How to fit text in the width/height specified")]
    public int fitType;
    [Name("Styles"), Description("The different styles this text can have")]
    public List<TextStyle> styles = new();
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

    public class TextStyle
    {
        [Name("Font"), Description("Font to use for this style")]
        public Symbol font = Symbol.Empty;
        [Name("Size"), Description("Size of the text")]
        public float size;
        [Name("Text Color"), Description("Color of the text, put into mesh verts. Modifed by <color=r,g,b,a> markup . This will only work if the font mat has [prelit] set true and [use_environment] set false")]
        public HmxColor4 textColor = new();
        [Name("Font Color Override"), Description("If true, and if there's a font, you can change color and alpha during the draw")]
        public bool fontColorOverride;
        [Name("Font Color"), Description("Color of the font during draw, can be changed dynamically")]
        public HmxColor4 fontColor = new();
        [Name("Italics"), Description("Defines the slant of the text, changed by <it> tag")]
        public float italics;
        [Name("Kerning"), Description("Extra kerning for the text")]
        public float kerning;
        [Name("Z Offset"), Description("vertical offset as fraction of size")]
        public float zOffset;
        [Name("Blacklight"), Description("draw in blacklight pass?")]
        public bool blacklight;

        internal TextStyle Read(EndianReader reader, ushort revision)
        {
            font = Symbol.Read(reader);
            size = reader.ReadFloat();
            textColor.Read(reader);
            fontColorOverride = reader.ReadBoolean();
            fontColor.Read(reader);
            italics = reader.ReadFloat();
            kerning = reader.ReadFloat();
            zOffset = reader.ReadFloat();
            if (revision >= 25)
            {
                blacklight = reader.ReadBoolean();
            }
            return this;
        }

        internal void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, font);
            writer.WriteFloat(size);
            textColor.Write(writer);
            writer.WriteBoolean(fontColorOverride);
            fontColor.Write(writer);
            writer.WriteFloat(italics);
            writer.WriteFloat(kerning);
            writer.WriteFloat(zOffset);
            if (revision >= 25)
            {
                writer.WriteBoolean(blacklight);
            }
        }
    }

    private void ReadStyles(EndianReader reader)
    {
        if (revision >= 24)
        {
            height = reader.ReadFloat();
            if (altRevision > 0)
            {
                circle = reader.ReadFloat();
            }
            fitType = reader.ReadInt32();
        }
        styles = AssetHelpers.ReadList(reader, () => new TextStyle().Read(reader, revision));
        if (revision >= 26)
        {
            scrollDelay = reader.ReadFloat();
            scrollRate = reader.ReadFloat();
            scrollPause = reader.ReadFloat();
        }
        if (revision >= 27)
        {
            indentation = reader.ReadFloat();
        }
        if (revision >= 28)
        {
            basicMarkup = reader.ReadBoolean();
        }
    }

    private void WriteStyles(EndianWriter writer)
    {
        if (revision >= 24)
        {
            writer.WriteFloat(height);
            if (altRevision > 0)
            {
                writer.WriteFloat(circle);
            }
            writer.WriteInt32(fitType);
        }
        AssetHelpers.WriteList(writer, styles, style => style.Write(writer, revision));
        if (revision >= 26)
        {
            writer.WriteFloat(scrollDelay);
            writer.WriteFloat(scrollRate);
            writer.WriteFloat(scrollPause);
        }
        if (revision >= 27)
        {
            writer.WriteFloat(indentation);
        }
        if (revision >= 28)
        {
            writer.WriteBoolean(basicMarkup);
        }
    }
}
