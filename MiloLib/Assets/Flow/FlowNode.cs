using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.ObjectFields;

namespace MiloLib.Assets.Flow;

[Name("FlowNode"), Description("A flow node")]
public class FlowNode : Object
{
    private ushort revision;
    private ushort altRevision;
    public List<Symbol> childNodes = new();
    public List<DrivenPropertyEntry> drivenProperties = new();
    [Name("Debug Output"), Description("Causes information about this node to be printed to the console for debugging, please set to false when done!")]
    public bool debugOutput;
    [Name("Debug Comment"), Description("Additional, specific information to print in the debug output (only used if debug_output is set to true)")]
    public Symbol debugComment = Symbol.Empty;

    public class MathOp
    {
        private ushort revision;
        private ushort altRevision;
        public int operation;
        public float value;
        public bool hasObject;
        public Symbol target = Symbol.Empty;
        public DTBNode rhs = new() { value = 0u };
        public Symbol oldSymbol = Symbol.Empty;
        public DTBNode lhs = new() { value = 0u };

        public MathOp Read(EndianReader reader)
        {
            AssetHelpers.ReadRevision(reader, out revision, out altRevision, 6);
            operation = reader.ReadInt32();
            value = reader.ReadFloat();
            if (revision < 6)
            {
                hasObject = reader.ReadBoolean();
            }
            if (revision >= 6 || hasObject)
            {
                target = Symbol.Read(reader);
            }
            rhs.Read(reader);
            if (revision > 3 && revision < 6)
            {
                oldSymbol = Symbol.Read(reader);
            }
            if (revision > 4)
            {
                lhs.Read(reader);
            }
            return this;
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteRevision(writer, revision, altRevision, 6);
            writer.WriteInt32(operation);
            writer.WriteFloat(value);
            if (revision < 6)
            {
                writer.WriteBoolean(hasObject);
            }
            if (revision >= 6 || hasObject)
            {
                Symbol.Write(writer, target);
            }
            rhs.Write(writer);
            if (revision > 3 && revision < 6)
            {
                Symbol.Write(writer, oldSymbol);
            }
            if (revision > 4)
            {
                lhs.Write(writer);
            }
        }
    }

    public class DrivenPropertyEntry
    {
        private ushort revision;
        private ushort altRevision;
        public DTBNode property = new() { type = NodeType.Symbol, value = (Symbol)"none" };
        public List<MathOp> mathOps = new();

        public DrivenPropertyEntry Read(EndianReader reader)
        {
            AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
            property.Read(reader);
            mathOps = AssetHelpers.ReadList(reader, () => new MathOp().Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
            property.Write(writer);
            AssetHelpers.WriteList(writer, mathOps, value => value.Write(writer));
        }
    }

    public new FlowNode Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        if (entry.type.value != "Flow")
        {
            base.Read(reader, false, parent, entry);
        }
        childNodes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        drivenProperties = AssetHelpers.ReadList(reader, () => new DrivenPropertyEntry().Read(reader));
        if (revision > 0)
        {
            debugOutput = reader.ReadBoolean();
        }
        if (revision > 1)
        {
            debugComment = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        if (entry?.type.value != "Flow")
        {
            base.Write(writer, false, parent, entry);
        }
        AssetHelpers.WriteList(writer, childNodes, value => Symbol.Write(writer, value));
        AssetHelpers.WriteList(writer, drivenProperties, value => value.Write(writer));
        if (revision > 0)
        {
            writer.WriteBoolean(debugOutput);
        }
        if (revision > 1)
        {
            Symbol.Write(writer, debugComment);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
