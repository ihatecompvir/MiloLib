using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamButtonSet"), Description("Collection of HamButtons that are controlled on a single hand")]
public class HamButtonSet : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    [Name("Layout Group"), Description("Group that contains the template button")]
    public Symbol layoutGroup = Symbol.Empty;
    [Name("Layout Trans"), Description("Trans to show the button set where to draw")]
    public Symbol layoutTrans = Symbol.Empty;
    [Name("Layout Button"), Description("Dummy button to show where to place the other buttons")]
    public Symbol layoutButton = Symbol.Empty;
    [Name("Buttons"), Description("Buttons in group")]
    public List<Symbol> buttons = new();
    [Name("Spacing"), Description("Vertical space between buttons")]
    public float spacing;
    [Name("Swell Anim")]
    public Symbol swellAnim = Symbol.Empty;
    [Name("Slide Anim")]
    public AnimSet slideAnim = new();
    [Name("Select Anim")]
    public AnimSet selectAnim = new();
    [Name("Select Toggle Anim")]
    public AnimSet selectToggleAnim = new();
    [Name("Layout Anim")]
    public AnimSet layoutAnim = new();
    [Name("Use Alternate Slide Sound"), Description("For helpbar buttons")]
    public bool useAlternateSlideSound;
    [Name("Button Spacer"), Description("Trans where the x component is used to determine spacing. This overrides the value in the spacing property during selection.")]
    public Symbol buttonSpacer = Symbol.Empty;
    [Name("Special Select Anim"), Description("Animation to run in parallel with select_anim")]
    public Symbol specialSelectAnim = Symbol.Empty;

    public class AnimSet
    {
        [Name("Highlighted Anim"), Description("Animation that plays on the highlighted button.")]
        public Symbol highlightedAnim = Symbol.Empty;
        [Name("Button Anims"), Description("Optional anims for each of the other buttons.")]
        public List<Symbol> buttonAnims = new();

        public AnimSet Read(EndianReader reader)
        {
            highlightedAnim = Symbol.Read(reader);
            buttonAnims = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, highlightedAnim);
            AssetHelpers.WriteList(writer, buttonAnims, item => Symbol.Write(writer, item));
        }
    }

    public new HamButtonSet Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 7);
        draw.Read(reader, false, parent, entry);
        layoutGroup = Symbol.Read(reader);
        if (revision >= 3)
        {
            layoutTrans = Symbol.Read(reader);
        }
        layoutButton = Symbol.Read(reader);
        buttons = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        spacing = reader.ReadFloat();
        swellAnim = Symbol.Read(reader);
        slideAnim.Read(reader);
        selectAnim.Read(reader);
        if (revision >= 4)
        {
            selectToggleAnim.Read(reader);
        }
        if (revision >= 2)
        {
            layoutAnim.Read(reader);
        }
        if (revision >= 5)
        {
            useAlternateSlideSound = reader.ReadBoolean();
        }
        if (revision >= 6)
        {
            buttonSpacer = Symbol.Read(reader);
        }
        if (revision >= 7)
        {
            specialSelectAnim = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 7);
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, layoutGroup);
        if (revision >= 3)
        {
            Symbol.Write(writer, layoutTrans);
        }
        Symbol.Write(writer, layoutButton);
        AssetHelpers.WriteList(writer, buttons, item => Symbol.Write(writer, item));
        writer.WriteFloat(spacing);
        Symbol.Write(writer, swellAnim);
        slideAnim.Write(writer);
        selectAnim.Write(writer);
        if (revision >= 4)
        {
            selectToggleAnim.Write(writer);
        }
        if (revision >= 2)
        {
            layoutAnim.Write(writer);
        }
        if (revision >= 5)
        {
            writer.WriteBoolean(useAlternateSlideSound);
        }
        if (revision >= 6)
        {
            Symbol.Write(writer, buttonSpacer);
        }
        if (revision >= 7)
        {
            Symbol.Write(writer, specialSelectAnim);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
