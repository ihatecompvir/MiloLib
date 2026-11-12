using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowOutPort"), Description("An output port for flow encapsulation")]
public class FlowOutPort : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Label"), Description("Name to show when encapsulated")]
    public Symbol label = Symbol.Empty;
    [Name("Immediate Release"), Description("do not wait around for the label to finish?")]
    public bool immediateRelease;
    [Name("Stop"), Description("When true, we request the label to stop instead of running it")]
    public bool stop;
    [Name("Exposed"), Description("Is this port exposed for possible linking?")]
    public bool exposed;

    public new FlowOutPort Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        label = Symbol.Read(reader);
        immediateRelease = reader.ReadBoolean();
        if (revision > 0)
        {
            stop = reader.ReadBoolean();
        }
        if (revision > 1)
        {
            exposed = reader.ReadBoolean();
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
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, label);
        writer.WriteBoolean(immediateRelease);
        if (revision > 0)
        {
            writer.WriteBoolean(stop);
        }
        if (revision > 1)
        {
            writer.WriteBoolean(exposed);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
