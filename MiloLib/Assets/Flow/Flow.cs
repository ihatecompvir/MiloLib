using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.ObjectFields;

namespace MiloLib.Assets.Flow;

[Name("Flow"), Description("Top level flow")]
public class Flow : ObjectDir
{
    private ushort revision;
    private ushort altRevision;
    public FlowQueueable queueable = new();
    [Name("Dynamic Properties")]
    public List<DynamicProperty> dynamicProperties = new();
    public List<ProxyProperty> proxyProperties = new();
    [Name("Hard Stop"), Description("force things to stop immediately?")]
    public bool hardStop;
    public bool oldStartOnEnter;
    [Name("Start On Enter"), Description("Should we automatically start on enter?")]
    public int startOnEnter;
    [Name("Private"), Description("Are we hidden from run nodes?")]
    public bool isPrivate;
    public int oldTriggerRevision;
    public bool oldHasProvider;
    public Symbol oldProvider = Symbol.Empty;
    public List<Symbol> oldTriggerEvents = new();
    public List<Symbol> oldStopEvents = new();
    public List<PropertyTrigger> oldTriggerProperties = new();
    public List<PropertyTrigger> oldStopProperties = new();

    public Flow(ushort revision = 0, ushort altRevision = 0) : base(0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public class PropertyTrigger
    {
        [Name("Provider"), Description("The object providing the properties")]
        public Symbol provider = Symbol.Empty;
        [Name("Path")]
        public DTBNode property = new() { value = 0u };

        public PropertyTrigger Read(EndianReader reader)
        {
            provider = Symbol.Read(reader);
            property.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, provider);
            property.Write(writer);
        }
    }

    public class ProxyProperty
    {
        public Symbol name = Symbol.Empty;
        public NodeType serializedType;
        public DTBNode value = new() { value = 0u };

        public ProxyProperty Read(EndianReader reader, ushort revision)
        {
            name = Symbol.Read(reader);
            if (revision >= 5)
            {
                serializedType = (NodeType)reader.ReadInt32();
            }
            if (revision >= 5 && serializedType == NodeType.Object)
            {
                value = new DTBNode { type = NodeType.Object, value = Symbol.Read(reader) };
            }
            else
            {
                value.Read(reader);
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, name);
            if (revision >= 5)
            {
                writer.WriteInt32((int)serializedType);
            }
            if (revision >= 5 && serializedType == NodeType.Object)
            {
                Symbol.Write(writer, (Symbol)value.value);
            }
            else
            {
                value.Write(writer);
            }
        }
    }

    public class DynamicProperty
    {
        [Name("Name"), Description("Name for the property")]
        public Symbol name = Symbol.Empty;
        [Name("Type"), Description("type of the property")]
        public int type;
        public DTBNode defaultValue = new() { value = 0u };
        [Name("Help"), Description("Help string for the user")]
        public Symbol help = Symbol.Empty;
        [Name("Object Class")]
        public Symbol objectClass = Symbol.Empty;
        [Name("Exposed"), Description("Is this property exposed to the proxy using this flow?")]
        public bool exposed;
        [Name("Symbol List"), Description("Space seperated list of symbols")]
        public DTBNode symbolList = new() { value = 0u };
        [Name("Object Type")]
        public Symbol objectType = Symbol.Empty;

        public DynamicProperty Read(EndianReader reader, ushort revision)
        {
            name = Symbol.Read(reader);
            type = reader.ReadInt32();
            defaultValue.Read(reader);
            help = Symbol.Read(reader);
            objectClass = Symbol.Read(reader);
            if (revision > 1)
            {
                exposed = reader.ReadBoolean();
                symbolList.Read(reader);
            }
            if (revision > 5)
            {
                objectType = Symbol.Read(reader);
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, name);
            writer.WriteInt32(type);
            defaultValue.Write(writer);
            Symbol.Write(writer, help);
            Symbol.Write(writer, objectClass);
            if (revision > 1)
            {
                writer.WriteBoolean(exposed);
                symbolList.Write(writer);
            }
            if (revision > 5)
            {
                Symbol.Write(writer, objectType);
            }
        }
    }

    public new Flow Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 7);
        base.Read(reader, false, parent, entry);
        if (entry.isProxy)
        {
            proxyProperties = AssetHelpers.ReadList(reader, () => new ProxyProperty().Read(reader, revision));
        }
        else
        {
            if (revision < 3)
            {
                oldTriggerRevision = reader.ReadInt32();
            }
            queueable.Read(reader, false, parent, entry);
            if (revision < 3)
            {
                if (oldTriggerRevision < 1)
                {
                    oldHasProvider = reader.ReadBoolean();
                }
                if (oldTriggerRevision >= 1 || oldHasProvider)
                {
                    oldProvider = Symbol.Read(reader);
                }
                oldTriggerEvents = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
                oldStopEvents = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            hardStop = reader.ReadBoolean();
            if (revision < 3 && oldTriggerRevision > 0)
            {
                oldTriggerProperties = AssetHelpers.ReadList(reader, () => new PropertyTrigger().Read(reader));
                oldStopProperties = AssetHelpers.ReadList(reader, () => new PropertyTrigger().Read(reader));
            }
            dynamicProperties = AssetHelpers.ReadList(reader, () => new DynamicProperty().Read(reader, revision));
            if (revision < 7)
            {
                oldStartOnEnter = reader.ReadBoolean();
            }
            else
            {
                startOnEnter = reader.ReadInt32();
            }
            if (revision > 0)
            {
                isPrivate = reader.ReadBoolean();
            }
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 7);
        base.Write(writer, false, parent, entry);
        if (entry != null && entry.isProxy)
        {
            AssetHelpers.WriteList(writer, proxyProperties, value => value.Write(writer, revision));
        }
        else
        {
            if (revision < 3)
            {
                writer.WriteInt32(oldTriggerRevision);
            }
            queueable.Write(writer, false, parent, entry);
            if (revision < 3)
            {
                if (oldTriggerRevision < 1)
                {
                    writer.WriteBoolean(oldHasProvider);
                }
                if (oldTriggerRevision >= 1 || oldHasProvider)
                {
                    Symbol.Write(writer, oldProvider);
                }
                AssetHelpers.WriteList(writer, oldTriggerEvents, value => Symbol.Write(writer, value));
                AssetHelpers.WriteList(writer, oldStopEvents, value => Symbol.Write(writer, value));
            }
            writer.WriteBoolean(hardStop);
            if (revision < 3 && oldTriggerRevision > 0)
            {
                AssetHelpers.WriteList(writer, oldTriggerProperties, value => value.Write(writer));
                AssetHelpers.WriteList(writer, oldStopProperties, value => value.Write(writer));
            }
            AssetHelpers.WriteList(writer, dynamicProperties, value => value.Write(writer, revision));
            if (revision < 7)
            {
                writer.WriteBoolean(oldStartOnEnter);
            }
            else
            {
                writer.WriteInt32(startOnEnter);
            }
            if (revision > 0)
            {
                writer.WriteBoolean(isPrivate);
            }
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
