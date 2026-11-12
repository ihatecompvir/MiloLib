using MiloLib.Assets;
using MiloLib.Classes;
using MiloLib.Utils;

[Name("UIFontImporter"), Description("Class supporting font importing. To be included in font resource file classes.")]
public class UIFontImporter
{
    public enum FontSuperSample
    {
        kFontSuperSample_None,
        kFontSuperSample_2x,
        kFontSuperSample_4x
    }
    private ushort altRevision;
    private ushort revision;

    [Name("Include Lower Case"), Description("include lowercase letters")]
    public bool includeLowerCase;
    [Name("Include Upper Case"), Description("include uppercase letters")]
    public bool includeUpperCase;
    [Name("Include Numbers"), Description("include the number 0-9")]
    public bool includeNumbers;
    [Name("Include Punctuation"), Description("include punctuation characters")]
    public bool includePunctuation;
    [Name("Include Upper Euro"), Description("include uppercase euro chars")]
    public bool includeUpperEuro;
    [Name("Include Lower Euro"), Description("include lowercase euro chars")]
    public bool includeLowerEuro;
    [Name("Russian"), Description("include Russian letters")]
    public bool includeRussian;
    [Name("Polish"), Description("include Polish letters")]
    public bool includePolish;
    [Name("Include Locale"), Description("include all characters from the current language's locale file")]
    public bool includeLocale;
    [Name("Include File"), Description("include all characters from specified file")]
    public Symbol includeFile = Symbol.Empty;
    [Name("Drop Shadow"), Description("drop shadow offset")]
    public int dropShadow;
    [Name("Drop Shadow Opacity"), Description("opacity of drop shadow (0-255)")]
    public int dropShadowOpacity;
    public int oldAltValue;
    [Name("Plus Chars"), Description("type in extra characters to include here")]
    public Symbol plusChars = Symbol.Empty;
    [Name("Minus Chars"), Description("type in characters to exclude here")]
    public Symbol minusChars = Symbol.Empty;
    [Name("Font Name"), Description("name of the font")]
    public Symbol fontName = Symbol.Empty;

    [Name("Font Pct Size"), Description("default font size in percent screen height"), MinVersion(5)]
    public float fontPctSize;
    [Name("Font Weight"), Description("bold variation?")]
    public int fontWeight;

    [Name("Is Font Italic"), Description("italic variation?")]
    public bool isFontItalic;

    [Name("Pitch And Family"), Description("pitch and family of font - comes from font picker")]
    public int pitchAndFamily;

    [Name("Font Quality"), Description("quality of font")]
    public int fontQuality;

    [Name("Font Char Set"), Description("character set for this font - comes from font picker")]
    public int fontCharSet;

    [Name("Font Super Sample"), Description("our own supersampling that draws the font texture at 2 or 4x and scales down like in photoshop. Might improve anti-aliasing at small font sizes."), MinVersion(2)]
    public FontSuperSample fontSupersample;

    [Name("Bitmap Save Path"), Description("path to save bitmap to (i.e. ui/image/)")]
    public Symbol bitmapPath = Symbol.Empty;
    [Name("Bitmap Save Name"), Description("name of the bitmap file (i.e. Arial(12).BMP)")]
    public Symbol bitmapFilename = Symbol.Empty;

    [Name("Texture Padding Left"), Description("pixels of padding on the left side of each character")]
    public int texPadLeft;
    [Name("Texture Padding Right"), Description("pixels of padding on the right side of each character")]
    public int texPadRight;
    [Name("Texture Padding Top"), Description("pixels of padding on the top side of each character")]
    public int texPadTop;
    [Name("Texture Padding Bottom"), Description("pixels of padding on the bottom side of each character")]
    public int texPadBottom;

    [Name("Fill With Safe White"), Description("fill texture with safe white color (235)")]
    public bool fillWithSafeWhite;

    [Name("Generated Fonts"), Description("the font(s) we've gen-ed for this resource file. We will maintain a connection to these objects when you re-generate"), MinVersion(3)]
    public List<Symbol> genFonts = new();

    [Name("Kerning Reference"), Description("A font which we'll transfer the kerning info from for any gen-ed fonts"), MinVersion(3)]
    public Symbol kerningReference = Symbol.Empty;

    [Name("Mat Variations"), Description("A list of materials we will expose for this font"), MinVersion(4)]
    public List<Symbol> matVariations = new();

    [Name("Default Material"), MinVersion(6)]
    public Symbol defaultMat = Symbol.Empty;

    [Name("Handmade Font"), Description("If you want to handmake a font texture, assign it here and the importer will no longer try to generate textures"), MinVersion(7)]
    public Symbol handmadeFont = Symbol.Empty;

    [Name("Sync Resource"), Description("You can pull in all the importer settings from another resource file by selecting it above and hitting the sync button below"), MinVersion(8)]
    public Symbol syncResource = Symbol.Empty;

    [Name("Last Gen Was Next-Gen"), Description("was the texture for this font last genned for an NG platform?"), MinVersion(9)]
    public bool lastGenWasNg;

    [MaxVersion(4)]
    public int unkInt1;


    [Name("Font to Import From"), MaxVersion(7)]
    public Symbol fontToImportFrom = Symbol.Empty;

    [MinVersion(3), MaxVersion(3)]
    public Symbol unkMatSymbol = Symbol.Empty;

    public UIFontImporter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        includeLowerCase = reader.ReadBoolean();
        includeUpperCase = reader.ReadBoolean();
        includeNumbers = reader.ReadBoolean();
        includePunctuation = reader.ReadBoolean();
        includeUpperEuro = reader.ReadBoolean();
        includeLowerEuro = reader.ReadBoolean();

        if (altRevision > 0)
        {
            includeRussian = reader.ReadBoolean();
            includePolish = reader.ReadBoolean();
            includeLocale = reader.ReadBoolean();
            includeFile = Symbol.Read(reader);
        }

        plusChars = Symbol.Read(reader);
        minusChars = Symbol.Read(reader);
        fontName = Symbol.Read(reader);

        if (revision <= 4)
        {
            unkInt1 = reader.ReadInt32();
        }
        else
        {
            fontPctSize = reader.ReadFloat();
        }
        fontWeight = reader.ReadInt32();

        isFontItalic = reader.ReadBoolean();

        if (altRevision > 1)
        {
            dropShadow = reader.ReadInt32();
        }
        if (altRevision > 2)
        {
            dropShadowOpacity = reader.ReadInt32();
        }

        pitchAndFamily = reader.ReadInt32();

        fontQuality = reader.ReadInt32();
        fontCharSet = reader.ReadInt32();

        if (revision > 1)
        {
            fontSupersample = (FontSuperSample)reader.ReadInt32();
        }

        bitmapPath = Symbol.Read(reader);
        bitmapFilename = Symbol.Read(reader);

        texPadLeft = reader.ReadInt32();
        texPadRight = reader.ReadInt32();
        texPadTop = reader.ReadInt32();
        texPadBottom = reader.ReadInt32();

        fillWithSafeWhite = reader.ReadBoolean();

        if (revision < 8)
        {
            fontToImportFrom = Symbol.Read(reader);
        }

        if (revision > 2)
        {
            genFonts = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            kerningReference = Symbol.Read(reader);
        }

        if (revision == 3)
        {
            unkMatSymbol = Symbol.Read(reader);
        }

        if (revision > 3)
        {
            matVariations = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 5 && revision < 10)
        {
            defaultMat = Symbol.Read(reader);
        }

        if (revision > 6)
        {
            handmadeFont = Symbol.Read(reader);
        }

        if (revision > 7)
        {
            syncResource = Symbol.Read(reader);
        }

        if (revision > 8)
        {
            lastGenWasNg = reader.ReadBoolean();
        }

        if (altRevision == 1)
        {
            oldAltValue = reader.ReadInt32();
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public static UIFontImporter New(ushort revision, ushort altRevision = 0)
    {
        UIFontImporter importer = new();
        importer.revision = revision;
        importer.altRevision = altRevision;
        return importer;
    }

    public void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        writer.WriteBoolean(includeLowerCase);
        writer.WriteBoolean(includeUpperCase);
        writer.WriteBoolean(includeNumbers);
        writer.WriteBoolean(includePunctuation);
        writer.WriteBoolean(includeUpperEuro);
        writer.WriteBoolean(includeLowerEuro);

        if (altRevision > 0)
        {
            writer.WriteBoolean(includeRussian);
            writer.WriteBoolean(includePolish);
            writer.WriteBoolean(includeLocale);
            Symbol.Write(writer, includeFile);
        }

        Symbol.Write(writer, plusChars);
        Symbol.Write(writer, minusChars);
        Symbol.Write(writer, fontName);

        if (revision <= 4)
        {
            writer.WriteInt32(unkInt1);
        }
        else
        {
            writer.WriteFloat(fontPctSize);
        }
        writer.WriteInt32(fontWeight);

        writer.WriteBoolean(isFontItalic);

        if (altRevision > 1)
        {
            writer.WriteInt32(dropShadow);
        }
        if (altRevision > 2)
        {
            writer.WriteInt32(dropShadowOpacity);
        }

        writer.WriteInt32(pitchAndFamily);

        writer.WriteInt32(fontQuality);

        writer.WriteInt32(fontCharSet);
        if (revision > 1)
        {
            writer.WriteInt32((int)fontSupersample);
        }

        Symbol.Write(writer, bitmapPath);
        Symbol.Write(writer, bitmapFilename);

        writer.WriteInt32(texPadLeft);
        writer.WriteInt32(texPadRight);
        writer.WriteInt32(texPadTop);
        writer.WriteInt32(texPadBottom);

        writer.WriteBoolean(fillWithSafeWhite);

        if (revision < 8)
        {
            Symbol.Write(writer, fontToImportFrom);
        }

        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, genFonts, genFont => Symbol.Write(writer, genFont));

            Symbol.Write(writer, kerningReference);
        }

        if (revision == 3)
        {
            Symbol.Write(writer, unkMatSymbol);
        }

        if (revision > 3)
        {
            AssetHelpers.WriteList(writer, matVariations, matVariation => Symbol.Write(writer, matVariation));
        }

        if (revision > 5 && revision < 10)
        {
            Symbol.Write(writer, defaultMat);
        }

        if (revision > 6)
        {
            Symbol.Write(writer, handmadeFont);
        }

        if (revision > 7)
        {
            Symbol.Write(writer, syncResource);
        }

        if (revision > 8)
        {
            writer.WriteBoolean(lastGenWasNg);
        }

        if (altRevision == 1)
        {
            writer.WriteInt32(oldAltValue);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
