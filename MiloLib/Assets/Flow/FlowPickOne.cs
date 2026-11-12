using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowPickOne"), Description("Picks certain children and activates them")]
public class FlowPickOne : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Choice Type"), Description("Style of choice made")]
    public int choiceType;
    [Name("Chance"), Description("0 to 1 value representing the chance this node runs when activated")]
    public float chance;

    public new FlowPickOne Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        choiceType = reader.ReadInt32();
        if (revision > 0)
        {
            chance = reader.ReadFloat();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        writer.WriteInt32(choiceType);
        if (revision > 0)
        {
            writer.WriteFloat(chance);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
