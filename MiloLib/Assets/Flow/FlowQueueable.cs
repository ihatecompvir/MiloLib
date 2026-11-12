using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowQueueable")]
public class FlowQueueable : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Interrupt"), Description("Determines how we handle re-triggering of this flow")]
    public int interrupt;

    public new FlowQueueable Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        interrupt = reader.ReadInt32();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        writer.WriteInt32(interrupt);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
