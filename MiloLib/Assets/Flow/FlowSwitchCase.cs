using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowSwitchCase"), Description("a case for a flow switch")]
public class FlowSwitchCase : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("To Value")]
    public FlowValue toValue = new();
    [Name("Operator"), Description("equality case to use for comparison")]
    public int comparison;
    [Name("From Value"), Description("the value we're transitioning from")]
    public FlowValue fromValue = new();
    [Name("Use Last Value"), Description("Use the last value to compare against")]
    public bool useLastValue;
    [Name("Unregister Parent"), Description("When true, the while node will quit listening for events once this case is run")]
    public bool unregisterParent;

    public new FlowSwitchCase Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        toValue.Read(reader, revision >= 2);
        comparison = reader.ReadInt32();
        fromValue.Read(reader, revision >= 2);
        if (revision > 0)
        {
            useLastValue = reader.ReadBoolean();
        }
        if (revision > 2)
        {
            unregisterParent = reader.ReadBoolean();
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
        toValue.Write(writer, revision >= 2);
        writer.WriteInt32(comparison);
        fromValue.Write(writer, revision >= 2);
        if (revision > 0)
        {
            writer.WriteBoolean(useLastValue);
        }
        if (revision > 2)
        {
            writer.WriteBoolean(unregisterParent);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
