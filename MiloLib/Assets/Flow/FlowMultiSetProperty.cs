using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.ObjectFields;

namespace MiloLib.Assets.Flow;

[Name("FlowMultiSetProperty"), Description("Set a property a bunch of objects at once")]
public class FlowMultiSetProperty : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Targets"), Description("object to set property on")]
    public List<Symbol> targets = new();
    [Name("Prop")]
    public DTBNode propertyPath = new() { value = 0u };
    [Name("Value"), Description("value to apply to all objects")]
    public DTBNode value = new() { value = 0u };

    public new FlowMultiSetProperty Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        targets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        propertyPath.Read(reader);
        value.Read(reader);
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
        AssetHelpers.WriteList(writer, targets, target => Symbol.Write(writer, target));
        propertyPath.Write(writer);
        value.Write(writer);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
