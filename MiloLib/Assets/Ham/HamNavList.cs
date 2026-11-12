using MiloLib.Assets.Rnd;
using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamNavList"), Description("List of navigation actions controlled by a single hand with gestures")]
public class HamNavList : UIComponent
{
    private ushort revision;
    private ushort altRevision;
    public RndAnimatable anim = new();
    [Name("List Ribbon Resource"), Description("HamListRibbon resource file")]
    public Symbol listRibbonResource = Symbol.Empty;
    [Name("List Dir Resource"), Description("UIListDir resource file")]
    public Symbol listDirResource = Symbol.Empty;
    [Name("Nav Provider")]
    public Symbol navProvider = Symbol.Empty;
    public bool disableSelectSound;
    public bool disableSlideSound;
    [Name("Enabled")]
    public bool enabled = true;
    [Name("Always Use Active Skeleton"), Description("Automatically tie this navlist to the active skeleton")]
    public bool alwaysUseActiveSkeleton = true;
    [Name("Nav Input Type")]
    public int navInputType;
    [Name("Only Use When Focused"), Description("This list can only be used when it is focused")]
    public bool onlyUseWhenFocused = true;
    [Name("Scroll Speed Anim")]
    public Symbol scrollSpeedAnim = Symbol.Empty;
    [Name("Suppress Automatic Enter"), Description("Don't automatically play the enter anim when this component enters")]
    public bool suppressAutomaticEnter;
    [Name("Big Elements"), Description("Elements that match these will be bigger than the other elements")]
    public List<Symbol> bigElements = new();
    [Name("Header Ribbon Resource"), Description("HamListRibbon resource file")]
    public Symbol headerRibbonResource = Symbol.Empty;
    [Name("Scroll Speed Indicator Resource"), Description("HamScrollSpeedIndicator resource file")]
    public Symbol scrollSpeedIndicatorResource = Symbol.Empty;
    [Name("Skip Enter Anim"), Description("Skip the enter anim altogether")]
    public bool skipEnterAnim;

    public new HamNavList Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 10);
        base.Read(reader, false, parent, entry);
        if (revision >= 2)
        {
            anim.Read(reader, parent, entry);
        }
        if (revision >= 1)
        {
            listRibbonResource = Symbol.Read(reader);
        }
        listDirResource = Symbol.Read(reader);
        navProvider = Symbol.Read(reader);
        if (revision >= 3)
        {
            disableSelectSound = reader.ReadBoolean();
            disableSlideSound = reader.ReadBoolean();
            enabled = reader.ReadBoolean();
            alwaysUseActiveSkeleton = reader.ReadBoolean();
            navInputType = reader.ReadInt32();
        }
        if (revision >= 5)
        {
            onlyUseWhenFocused = reader.ReadBoolean();
        }
        if (revision >= 4)
        {
            scrollSpeedAnim = Symbol.Read(reader);
        }
        if (revision >= 6)
        {
            suppressAutomaticEnter = reader.ReadBoolean();
        }
        if (revision >= 7)
        {
            bigElements = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision >= 8)
        {
            headerRibbonResource = Symbol.Read(reader);
        }
        if (revision >= 9)
        {
            scrollSpeedIndicatorResource = Symbol.Read(reader);
        }
        if (revision >= 10)
        {
            skipEnterAnim = reader.ReadBoolean();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 10);
        base.Write(writer, false, parent, entry);
        if (revision >= 2)
        {
            anim.Write(writer);
        }
        if (revision >= 1)
        {
            Symbol.Write(writer, listRibbonResource);
        }
        Symbol.Write(writer, listDirResource);
        Symbol.Write(writer, navProvider);
        if (revision >= 3)
        {
            writer.WriteBoolean(disableSelectSound);
            writer.WriteBoolean(disableSlideSound);
            writer.WriteBoolean(enabled);
            writer.WriteBoolean(alwaysUseActiveSkeleton);
            writer.WriteInt32(navInputType);
        }
        if (revision >= 5)
        {
            writer.WriteBoolean(onlyUseWhenFocused);
        }
        if (revision >= 4)
        {
            Symbol.Write(writer, scrollSpeedAnim);
        }
        if (revision >= 6)
        {
            writer.WriteBoolean(suppressAutomaticEnter);
        }
        if (revision >= 7)
        {
            AssetHelpers.WriteList(writer, bigElements, element => Symbol.Write(writer, element));
        }
        if (revision >= 8)
        {
            Symbol.Write(writer, headerRibbonResource);
        }
        if (revision >= 9)
        {
            Symbol.Write(writer, scrollSpeedIndicatorResource);
        }
        if (revision >= 10)
        {
            writer.WriteBoolean(skipEnterAnim);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
