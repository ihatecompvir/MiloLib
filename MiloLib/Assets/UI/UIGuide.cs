using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIGuide"), Description("a guide used to line up UI elements")]
public class UIGuide : Object
{
    public enum Type
    {
        kGuideVertical,
        kGuideHorizontal
    }
    private ushort altRevision;
    private ushort revision;

    [Name("Type"), Description("Horizontal or vertical guide?")]
    public Type type;
    [Name("Position"), Description("position in proportion to screen width (if vertical) or height (if horizontal)")]
    public float pos;

    public new UIGuide Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        type = (Type)reader.ReadInt32();
        pos = reader.ReadFloat();

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

        writer.WriteInt32((int)type);
        writer.WriteFloat(pos);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
