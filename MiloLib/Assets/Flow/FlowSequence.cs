using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowSequence"), Description("A sequence of flow objects")]
public class FlowSequence : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Looping"), Description("Loop forever?")]
    public bool looping;
    [Name("Repeats"), Description("how many times to reapeat this sequence")]
    public int repeats;
    [Name("Stop Mode"), Description("How should we handle stop requests?")]
    public int stopMode;

    public new FlowSequence Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        looping = reader.ReadBoolean();
        repeats = reader.ReadInt32();
        if (revision > 0)
        {
            stopMode = reader.ReadInt32();
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
        writer.WriteBoolean(looping);
        writer.WriteInt32(repeats);
        if (revision > 0)
        {
            writer.WriteInt32(stopMode);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
