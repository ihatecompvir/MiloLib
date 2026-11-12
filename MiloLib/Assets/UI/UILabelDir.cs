using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;


[Name("UILabelDir"), Description("Top-level resource object for UILabels")]
public class UILabelDir : RndDir
{
    private ushort altRevision;
    private ushort revision;

    [Name("Text Object")]
    public Symbol textObject = Symbol.Empty;

    [Name("Font Reference"), MinVersion(3), MaxVersion(8)]
    public Symbol fontReference = Symbol.Empty;

    [Name("Focus Anim"), MinVersion(1)]
    public Symbol focusAnim = Symbol.Empty;
    [Name("Pulse Anim"), MinVersion(2)]
    public Symbol pulseAnim = Symbol.Empty;

    [Name("Highlight Mesh Group"), MinVersion(4)]
    public Symbol highlightMeshGroup = Symbol.Empty;
    [Name("Top Left Highlight Bone"), MinVersion(4)]
    public Symbol topLeftHighlightBone = Symbol.Empty;
    [Name("Top Right Highlight Bone"), MinVersion(4)]
    public Symbol topRightHighlightBone = Symbol.Empty;
    [Name("Bottom Left Highlight Bone"), MinVersion(5)]
    public Symbol bottomLeftHighlightBone = Symbol.Empty;
    [Name("Bottom Right Highlight Bone"), MinVersion(5)]
    public Symbol bottomRightHighlightBone = Symbol.Empty;

    [Name("Focused Background Group"), MinVersion(6)]
    public Symbol focusedBackgroundGroup = Symbol.Empty;
    [Name("Unfocused Background Group"), MinVersion(6)]
    public Symbol unfocusedBackgroundGroup = Symbol.Empty;

    [Name("Allow Edit Text"), Description("allow non-localized text with this resource?"), MinVersion(7)]
    public bool allowEditText;

    [Name("Default Color"), Description("color to use when no other color is defined for a state")]
    public Symbol defaultColor = Symbol.Empty;
    [Name("Normal Color"), Description("color when label is normal")]
    public Symbol normalColor = Symbol.Empty;
    [Name("Focused Color"), Description("color when label is focused")]
    public Symbol focusedColor = Symbol.Empty;
    [Name("Disabled Color"), Description("color when label is disabled")]
    public Symbol disabledColor = Symbol.Empty;
    [Name("Selecting Color"), Description("color when label is selecting")]
    public Symbol selectingColor = Symbol.Empty;
    [Name("Selected Color"), Description("color when label is selected")]
    public Symbol selectedColor = Symbol.Empty;

    [Name("Font Importer"), MinVersion(8)]
    public UIFontImporter fontImporter = new();

    public UILabelDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new UILabelDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision < 10)
        {
            textObject = Symbol.Read(reader);
        }

        if (revision >= 3 && revision <= 8)
        {
            fontReference = Symbol.Read(reader);
        }

        if (revision >= 1)
        {
            focusAnim = Symbol.Read(reader);
        }

        if (revision >= 2)
        {
            pulseAnim = Symbol.Read(reader);
        }

        if (revision >= 4 && revision < 11)
        {
            highlightMeshGroup = Symbol.Read(reader);
            topLeftHighlightBone = Symbol.Read(reader);
            topRightHighlightBone = Symbol.Read(reader);
        }
        if (revision >= 5 && revision < 11)
        {
            bottomLeftHighlightBone = Symbol.Read(reader);
            bottomRightHighlightBone = Symbol.Read(reader);
        }

        if (revision >= 6)
        {
            focusedBackgroundGroup = Symbol.Read(reader);
            unfocusedBackgroundGroup = Symbol.Read(reader);
        }

        if (revision >= 7)
        {
            allowEditText = reader.ReadBoolean();
        }

        defaultColor = Symbol.Read(reader);
        normalColor = Symbol.Read(reader);
        focusedColor = Symbol.Read(reader);
        disabledColor = Symbol.Read(reader);
        selectingColor = Symbol.Read(reader);
        selectedColor = Symbol.Read(reader);

        if (revision >= 8)
        {
            fontImporter = new UIFontImporter().Read(reader, false, parent, entry);
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

        if (revision < 10)
        {
            Symbol.Write(writer, textObject);
        }

        if (revision >= 3 && revision <= 8)
        {
            Symbol.Write(writer, fontReference);
        }

        if (revision >= 1)
        {
            Symbol.Write(writer, focusAnim);
        }

        if (revision >= 2)
        {
            Symbol.Write(writer, pulseAnim);
        }

        if (revision >= 4 && revision < 11)
        {
            Symbol.Write(writer, highlightMeshGroup);
            Symbol.Write(writer, topLeftHighlightBone);
            Symbol.Write(writer, topRightHighlightBone);
        }

        if (revision >= 5 && revision < 11)
        {
            Symbol.Write(writer, bottomLeftHighlightBone);
            Symbol.Write(writer, bottomRightHighlightBone);
        }


        if (revision >= 6)
        {
            Symbol.Write(writer, focusedBackgroundGroup);
            Symbol.Write(writer, unfocusedBackgroundGroup);
        }


        if (revision >= 7)
        {
            writer.WriteBoolean(allowEditText);
        }

        Symbol.Write(writer, defaultColor);
        Symbol.Write(writer, normalColor);
        Symbol.Write(writer, focusedColor);
        Symbol.Write(writer, disabledColor);
        Symbol.Write(writer, selectingColor);
        Symbol.Write(writer, selectedColor);

        if (revision >= 8)
        {
            fontImporter.Write(writer, false, parent, entry);
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
