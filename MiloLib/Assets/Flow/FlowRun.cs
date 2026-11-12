using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowRun"), Description("Run or stop another Flow")]
public class FlowRun : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Target Dir"), Description("Allows you to target flows inside of proxies")]
    public Symbol targetDir = Symbol.Empty;
    [Name("Target"), Description("Flow to start or stop")]
    public Symbol targetName = Symbol.Empty;
    [Name("Stop"), Description("Stop instead of starting the target flow?")]
    public bool stop;
    [Name("Immediate Release"), Description("If true, we don't track the running state of the target flow")]
    public bool immediateRelease;

    public new FlowRun Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        targetDir = Symbol.Read(reader);
        targetName = Symbol.Read(reader);
        stop = reader.ReadBoolean();
        immediateRelease = reader.ReadBoolean();
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
        Symbol.Write(writer, targetDir);
        Symbol.Write(writer, targetName);
        writer.WriteBoolean(stop);
        writer.WriteBoolean(immediateRelease);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
