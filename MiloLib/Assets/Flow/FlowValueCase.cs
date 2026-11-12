using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.ObjectFields;

namespace MiloLib.Assets.Flow;

[Name("FlowValueCase"), Description("A value case")]
public class FlowValueCase : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    public DTBNode oldValue = new() { value = 0u };
    [Name("Value"), Description("Key frame value")]
    public float value;

    public new FlowValueCase Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        if (revision < 1)
        {
            oldValue.Read(reader);
        }
        if (revision >= 1)
        {
            value = reader.ReadFloat();
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
            oldValue.Write(writer);
        }
        if (revision >= 1)
        {
            writer.WriteFloat(value);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
