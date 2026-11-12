using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowEventListener"), Description("Listens for/filters events and triggers children")]
public class FlowEventListener : FlowTrigger
{
    private ushort revision;
    private ushort altRevision;
    public bool oldOnce;
    [Name("Event Count"), Description("How many events do we process before disabling ourselves? 0 is infinite")]
    public int eventCount;
    [Name("Start On Activate"), Description("When true, we will start our children when we begin listening for events")]
    public bool startOnActivate;

    public new FlowEventListener Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        if (revision == 1)
        {
            oldOnce = reader.ReadBoolean();
        }
        if (revision > 1)
        {
            eventCount = reader.ReadInt32();
        }
        if (revision > 2)
        {
            startOnActivate = reader.ReadBoolean();
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
        if (revision == 1)
        {
            writer.WriteBoolean(oldOnce);
        }
        if (revision > 1)
        {
            writer.WriteInt32(eventCount);
        }
        if (revision > 2)
        {
            writer.WriteBoolean(startOnActivate);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
