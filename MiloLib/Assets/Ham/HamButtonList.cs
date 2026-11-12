using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamButtonList"), Description("Collection of HamButtons that are controlled on a single hand, in a scrollable list")]
public class HamButtonList : HamButtonSet
{
    private ushort revision;
    private ushort altRevision;
    [Name("First Selectable"), Description("Index of first selectable button in list")]
    public int firstSelectable;
    [Name("Num Selectable"), Description("Number of selectable buttons in list")]
    public int numSelectable;
    [Name("Scroll Anim")]
    public AnimSet scrollAnim = new();
    [Name("Scroll Speed Anim")]
    public AnimSet scrollSpeedAnim = new();
    [Name("Scroll Fast Up Trigger"), Description("Trigger when scrolling up in fast mode")]
    public Symbol scrollFastUpTrigger = Symbol.Empty;
    [Name("Scroll Up Trigger"), Description("Trigger when scrolling up")]
    public Symbol scrollUpTrigger = Symbol.Empty;
    [Name("Scroll None Trigger"), Description("Trigger when not scrolling")]
    public Symbol scrollNoneTrigger = Symbol.Empty;
    [Name("Scroll Down Trigger"), Description("Trigger when scrolling down")]
    public Symbol scrollDownTrigger = Symbol.Empty;
    [Name("Scroll Fast Down Trigger"), Description("Trigger when scrolling down in fast mode")]
    public Symbol scrollFastDownTrigger = Symbol.Empty;
    [Name("Normal Color Anim")]
    public Symbol normalColorAnim = Symbol.Empty;
    [Name("Special Color Anim")]
    public Symbol specialColorAnim = Symbol.Empty;

    public new HamButtonList Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 4);
        base.Read(reader, false, parent, entry);
        firstSelectable = reader.ReadInt32();
        numSelectable = reader.ReadInt32();
        scrollAnim.Read(reader);
        if (revision >= 2)
        {
            scrollSpeedAnim.Read(reader);
        }
        if (revision >= 3)
        {
            scrollFastUpTrigger = Symbol.Read(reader);
            scrollUpTrigger = Symbol.Read(reader);
            scrollNoneTrigger = Symbol.Read(reader);
            scrollDownTrigger = Symbol.Read(reader);
            scrollFastDownTrigger = Symbol.Read(reader);
        }
        if (revision >= 4)
        {
            normalColorAnim = Symbol.Read(reader);
            specialColorAnim = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 4);
        base.Write(writer, false, parent, entry);
        writer.WriteInt32(firstSelectable);
        writer.WriteInt32(numSelectable);
        scrollAnim.Write(writer);
        if (revision >= 2)
        {
            scrollSpeedAnim.Write(writer);
        }
        if (revision >= 3)
        {
            Symbol.Write(writer, scrollFastUpTrigger);
            Symbol.Write(writer, scrollUpTrigger);
            Symbol.Write(writer, scrollNoneTrigger);
            Symbol.Write(writer, scrollDownTrigger);
            Symbol.Write(writer, scrollFastDownTrigger);
        }
        if (revision >= 4)
        {
            Symbol.Write(writer, normalColorAnim);
            Symbol.Write(writer, specialColorAnim);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
