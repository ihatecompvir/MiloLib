using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowTimer"), Description("Simple timer with child support")]
public class FlowTimer : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Total Time"), Description("total time of timer")]
    public float totalTime;
    [Name("Rate"), Description("Rate to use for time")]
    public int rate;
    [Name("Stop Mode"), Description("How should we handle stop requests?")]
    public int stopMode;

    public new FlowTimer Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        totalTime = reader.ReadFloat();
        rate = reader.ReadInt32();
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
        writer.WriteFloat(totalTime);
        writer.WriteInt32(rate);
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
