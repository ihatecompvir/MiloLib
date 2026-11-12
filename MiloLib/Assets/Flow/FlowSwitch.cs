using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowSwitch"), Description("activates children based on comparison cases")]
public class FlowSwitch : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("First Valid Case Only"), Description("When true, the first case to be valid is the only one activated")]
    public bool firstValidCaseOnly;

    public new FlowSwitch Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        firstValidCaseOnly = reader.ReadBoolean();
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
        writer.WriteBoolean(firstValidCaseOnly);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
