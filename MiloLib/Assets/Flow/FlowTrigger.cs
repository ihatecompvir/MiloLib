using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowTrigger")]
public class FlowTrigger : FlowQueueable
{
    private ushort revision;
    private ushort altRevision;
    public bool oldHasProvider;
    [Name("Event Provider"), Description("The Object which I listen to for events")]
    public Symbol eventProvider = Symbol.Empty;
    [Name("Trigger Events"), Description("Events which run this flow")]
    public List<Symbol> triggerEvents = new();
    [Name("Stop Events"), Description("Events which stop this flow")]
    public List<Symbol> stopEvents = new();
    [Name("Hard Stop"), Description("force things to stop immediately?")]
    public bool hardStop;
    [Name("Trigger Properties")]
    public List<Flow.PropertyTrigger> triggerProperties = new();
    [Name("Stop Properties")]
    public List<Flow.PropertyTrigger> stopProperties = new();

    public new FlowTrigger Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        if (revision < 1)
        {
            oldHasProvider = reader.ReadBoolean();
        }
        if (revision >= 1 || oldHasProvider)
        {
            eventProvider = Symbol.Read(reader);
        }
        triggerEvents = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        stopEvents = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        hardStop = reader.ReadBoolean();
        if (revision > 0)
        {
            triggerProperties = AssetHelpers.ReadList(reader, () => new Flow.PropertyTrigger().Read(reader));
            stopProperties = AssetHelpers.ReadList(reader, () => new Flow.PropertyTrigger().Read(reader));
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
        if (revision < 1)
        {
            writer.WriteBoolean(oldHasProvider);
        }
        if (revision >= 1 || oldHasProvider)
        {
            Symbol.Write(writer, eventProvider);
        }
        AssetHelpers.WriteList(writer, triggerEvents, value => Symbol.Write(writer, value));
        AssetHelpers.WriteList(writer, stopEvents, value => Symbol.Write(writer, value));
        writer.WriteBoolean(hardStop);
        if (revision > 0)
        {
            AssetHelpers.WriteList(writer, triggerProperties, value => value.Write(writer));
            AssetHelpers.WriteList(writer, stopProperties, value => value.Write(writer));
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
