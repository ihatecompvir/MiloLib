using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowSlider"), Description("Ramps intensity up/down on child nodes based on value of slider")]
public class FlowSlider : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Persistent"), Description("Do we listen for changes on our value property?")]
    public bool persistent;
    [Name("Always Run"), Description("If true, we run all children regardless of intensity. If false, we stop and start them based on intensity")]
    public bool alwaysRun;
    [Name("Value"), Description("position on slider")]
    public float value;
    [Name("Ease"), Description("Easing to apply to slider intensity output")]
    public int easeType;
    [Name("Ease Power"), Description("Modifier to easing equation")]
    public float easePower;

    public new FlowSlider Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        persistent = reader.ReadBoolean();
        alwaysRun = reader.ReadBoolean();
        value = reader.ReadFloat();
        easeType = reader.ReadInt32();
        easePower = reader.ReadFloat();
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
        writer.WriteBoolean(persistent);
        writer.WriteBoolean(alwaysRun);
        writer.WriteFloat(value);
        writer.WriteInt32(easeType);
        writer.WriteFloat(easePower);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
