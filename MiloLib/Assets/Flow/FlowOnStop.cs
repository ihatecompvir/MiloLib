using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowOnStop"), Description("Runs code when a flow is deactivated or requested to stop")]
public class FlowOnStop : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Mode"), Description("Do we only work on interuption request, or when forced to stop immediately")]
    public int mode;

    public new FlowOnStop Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        mode = reader.ReadInt32();
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
        writer.WriteInt32(mode);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
