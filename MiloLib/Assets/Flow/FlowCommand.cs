using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.ObjectFields;

namespace MiloLib.Assets.Flow;

[Name("FlowCommand"), Description("Runs a handler exposed by coders on an object")]
public class FlowCommand : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    public List<DTBNode> typeProperties = new();
    public DTBNode oldProperties = new() { value = 0u };
    [Name("Target"), Description("The object which I'm going to trigger a handler on")]
    public Symbol target = Symbol.Empty;
    [Name("Handler"), Description("Handler to fire on the target object")]
    public Symbol handler = Symbol.Empty;

    public new FlowCommand Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        if (revision > 2)
        {
            typeProperties = AssetHelpers.ReadList(reader, () => ReadNode(reader));
        }
        base.Read(reader, false, parent, entry);
        target = Symbol.Read(reader);
        handler = Symbol.Read(reader);
        if (revision < 2)
        {
            oldProperties.Read(reader);
        }
        else if (revision < 3)
        {
            typeProperties = AssetHelpers.ReadList(reader, () => ReadNode(reader));
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
        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, typeProperties, value => value.Write(writer));
        }
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, target);
        Symbol.Write(writer, handler);
        if (revision < 2)
        {
            oldProperties.Write(writer);
        }
        else if (revision < 3)
        {
            AssetHelpers.WriteList(writer, typeProperties, value => value.Write(writer));
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private static DTBNode ReadNode(EndianReader reader)
    {
        DTBNode node = new();
        node.Read(reader);
        return node;
    }
}
