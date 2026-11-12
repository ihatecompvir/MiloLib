using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowLabel"), Description("A label for flow encapsulation")]
public class FlowLabel : FlowQueueable
{
    private ushort revision;
    private ushort altRevision;
    public Symbol label = Symbol.Empty;
    public Symbol flowParent = Symbol.Empty;

    public new FlowLabel Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        label = Symbol.Read(reader);
        if (revision > 0)
        {
            flowParent = Symbol.Read(reader);
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
        Symbol.Write(writer, label);
        if (revision > 0)
        {
            Symbol.Write(writer, flowParent);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
