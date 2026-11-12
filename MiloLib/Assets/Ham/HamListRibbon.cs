using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamListRibbon"), Description("Top-level resource object for UILists")]
public class HamListRibbon : RndDir
{
    private ushort revision;
    private ushort altRevision;
    [Name("Spacing"), Description("How far apart elements should be spaced")]
    public float spacing = 25.0f;
    [Name("Swell Anim")]
    public Symbol swellAnim = Symbol.Empty;
    [Name("Slide Anim")]
    public Symbol slideAnim = Symbol.Empty;
    [Name("Select Anim")]
    public Symbol selectAnim = Symbol.Empty;
    [Name("Select Inactive Anim")]
    public Symbol selectInactiveAnim = Symbol.Empty;
    [Name("Select All Anim")]
    public Symbol selectAllAnim = Symbol.Empty;
    [Name("Label Placeholder"), Description("Where the label goes")]
    public Symbol labelPlaceholder = Symbol.Empty;
    [Name("Scroll Anim")]
    public Symbol scrollAnim = Symbol.Empty;
    [Name("Scroll Active")]
    public Symbol scrollActive = Symbol.Empty;
    [Name("Scroll Fade")]
    public Symbol scrollFade = Symbol.Empty;
    [Name("Scroll Faded")]
    public Symbol scrollFaded = Symbol.Empty;
    [Name("Disengage Anim")]
    public Symbol disengageAnim = Symbol.Empty;
    public List<Symbol> oldHighlightSounds = new();
    public List<Symbol> oldSelectSounds = new();
    public Symbol oldSlideSound = Symbol.Empty;
    public Symbol oldScrollSound = Symbol.Empty;
    [Name("Slide Sound")]
    public Symbol slideSound = Symbol.Empty;
    [Name("Slide Sound Anim")]
    public Symbol slideSoundAnim = Symbol.Empty;
    [Name("Scroll Sound")]
    public Symbol scrollSound = Symbol.Empty;
    [Name("Scroll Sound Anim")]
    public Symbol scrollSoundAnim = Symbol.Empty;
    [Name("Enter Flow"), Description("Flow to play on enter")]
    public Symbol enterFlow = Symbol.Empty;
    [Name("Enter Anim")]
    public Symbol enterAnim = Symbol.Empty;
    [Name("Padded Size"), Description("Minimum number of ribbons to show")]
    public int paddedSize = 0;
    [Name("Padded Spacing"), Description("Spacing between padded ribbons")]
    public float paddedSpacing = 29.0f;
    [Name("Highlight Sounds")]
    public List<Symbol> highlightSounds = new();
    [Name("Select Sounds")]
    public List<Symbol> selectSounds = new();
    [Name("Select Toggle Anim")]
    public Symbol selectToggleAnim = Symbol.Empty;

    public HamListRibbon(ushort revision = 0, ushort altRevision = 0) : base(0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new HamListRibbon Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 11);
        base.Read(reader, false, parent, entry);
        spacing = reader.ReadFloat();
        swellAnim = Symbol.Read(reader);
        slideAnim = Symbol.Read(reader);
        selectAnim = Symbol.Read(reader);
        selectInactiveAnim = Symbol.Read(reader);
        selectAllAnim = Symbol.Read(reader);
        labelPlaceholder = Symbol.Read(reader);
        if (revision >= 2)
        {
            scrollAnim = Symbol.Read(reader);
            scrollActive = Symbol.Read(reader);
            scrollFade = Symbol.Read(reader);
            scrollFaded = Symbol.Read(reader);
        }
        if (revision >= 3)
        {
            disengageAnim = Symbol.Read(reader);
        }
        if (revision >= 4 && revision < 9)
        {
            oldHighlightSounds = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            oldSelectSounds = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision == 4)
        {
            oldSlideSound = Symbol.Read(reader);
            oldScrollSound = Symbol.Read(reader);
        }
        if (revision >= 5)
        {
            slideSound = Symbol.Read(reader);
            slideSoundAnim = Symbol.Read(reader);
            scrollSound = Symbol.Read(reader);
            scrollSoundAnim = Symbol.Read(reader);
        }
        if (revision >= 10)
        {
            enterFlow = Symbol.Read(reader);
        }
        if (revision >= 6)
        {
            enterAnim = Symbol.Read(reader);
        }
        if (revision >= 7)
        {
            paddedSize = reader.ReadInt32();
        }
        if (revision >= 8)
        {
            paddedSpacing = reader.ReadFloat();
        }
        if (revision >= 9)
        {
            highlightSounds = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            selectSounds = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision >= 11)
        {
            selectToggleAnim = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 11);
        base.Write(writer, false, parent, entry);
        writer.WriteFloat(spacing);
        Symbol.Write(writer, swellAnim);
        Symbol.Write(writer, slideAnim);
        Symbol.Write(writer, selectAnim);
        Symbol.Write(writer, selectInactiveAnim);
        Symbol.Write(writer, selectAllAnim);
        Symbol.Write(writer, labelPlaceholder);
        if (revision >= 2)
        {
            Symbol.Write(writer, scrollAnim);
            Symbol.Write(writer, scrollActive);
            Symbol.Write(writer, scrollFade);
            Symbol.Write(writer, scrollFaded);
        }
        if (revision >= 3)
        {
            Symbol.Write(writer, disengageAnim);
        }
        if (revision >= 4 && revision < 9)
        {
            AssetHelpers.WriteList(writer, oldHighlightSounds, value => Symbol.Write(writer, value));
            AssetHelpers.WriteList(writer, oldSelectSounds, value => Symbol.Write(writer, value));
        }
        if (revision == 4)
        {
            Symbol.Write(writer, oldSlideSound);
            Symbol.Write(writer, oldScrollSound);
        }
        if (revision >= 5)
        {
            Symbol.Write(writer, slideSound);
            Symbol.Write(writer, slideSoundAnim);
            Symbol.Write(writer, scrollSound);
            Symbol.Write(writer, scrollSoundAnim);
        }
        if (revision >= 10)
        {
            Symbol.Write(writer, enterFlow);
        }
        if (revision >= 6)
        {
            Symbol.Write(writer, enterAnim);
        }
        if (revision >= 7)
        {
            writer.WriteInt32(paddedSize);
        }
        if (revision >= 8)
        {
            writer.WriteFloat(paddedSpacing);
        }
        if (revision >= 9)
        {
            AssetHelpers.WriteList(writer, highlightSounds, value => Symbol.Write(writer, value));
            AssetHelpers.WriteList(writer, selectSounds, value => Symbol.Write(writer, value));
        }
        if (revision >= 11)
        {
            Symbol.Write(writer, selectToggleAnim);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
