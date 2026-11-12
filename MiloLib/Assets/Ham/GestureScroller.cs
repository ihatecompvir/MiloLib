using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("GestureScroller"), Description("Gesture scroller")]
public class GestureScroller : Object
{
    private ushort revision;
    private ushort altRevision;
    public Symbol scrollAnim = Symbol.Empty;
    public float scrollSpeed;
    public Symbol scrollFastUpTrigger = Symbol.Empty;
    public Symbol scrollUpTrigger = Symbol.Empty;
    public Symbol scrollNoneTrigger = Symbol.Empty;
    public Symbol scrollDownTrigger = Symbol.Empty;
    public Symbol scrollFastDownTrigger = Symbol.Empty;

    public new GestureScroller Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        scrollAnim = Symbol.Read(reader);
        scrollSpeed = reader.ReadFloat();
        scrollFastUpTrigger = Symbol.Read(reader);
        scrollUpTrigger = Symbol.Read(reader);
        scrollNoneTrigger = Symbol.Read(reader);
        scrollDownTrigger = Symbol.Read(reader);
        scrollFastDownTrigger = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        Symbol.Write(writer, scrollAnim);
        writer.WriteFloat(scrollSpeed);
        Symbol.Write(writer, scrollFastUpTrigger);
        Symbol.Write(writer, scrollUpTrigger);
        Symbol.Write(writer, scrollNoneTrigger);
        Symbol.Write(writer, scrollDownTrigger);
        Symbol.Write(writer, scrollFastDownTrigger);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
