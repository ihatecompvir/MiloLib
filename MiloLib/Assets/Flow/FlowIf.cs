using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowIf"), Description("a single case decision node")]
public class FlowIf : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("First Value"), Description("left hand value in comparison")]
    public FlowValue value1 = new();
    [Name("Second Value"), Description("right hand value in comparison")]
    public FlowValue value2 = new();
    [Name("Operator"), Description("equality case to use for comparison")]
    public int comparison;
    public int oldValue;

    public new FlowIf Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        value1.Read(reader, revision >= 3);
        value2.Read(reader, revision >= 3);
        comparison = reader.ReadInt32();
        if (revision == 1)
        {
            oldValue = reader.ReadInt32();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        value1.Write(writer, revision >= 3);
        value2.Write(writer, revision >= 3);
        writer.WriteInt32(comparison);
        if (revision == 1)
        {
            writer.WriteInt32(oldValue);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
