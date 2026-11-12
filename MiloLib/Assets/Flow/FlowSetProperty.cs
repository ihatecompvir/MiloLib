using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.ObjectFields;

namespace MiloLib.Assets.Flow;

[Name("FlowSetProperty"), Description("Set a property on an object")]
public class FlowSetProperty : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Target"), Description("object to set property on")]
    public Symbol target = Symbol.Empty;
    [Name("Prop")]
    public DTBNode propertyPath = new() { value = 0u };
    [Name("Value"), Description("value to apply to this property")]
    public FlowValue value = new();
    [Name("Rate"), Description("Rate to animate")]
    public int rate;
    [Name("Blend Time"), Description("total time of blend to value")]
    public float blendTime;
    [Name("Change Per Unit"), Description("base time of blend of distance to value")]
    public float changePerUnit;
    [Name("Ease"), Description("easing style to use for blend")]
    public int ease;
    [Name("Ease Power"), Description("used in regular, elasic, and back easing")]
    public float easePower;
    public bool oldFlag;
    [Name("Persistent"), Description("Makes this node act like While node, updating automatically when a driven property changes")]
    public bool persistent;
    [Name("Stop Mode"), Description("How should we handle stop requests?")]
    public int stopMode;

    public new FlowSetProperty Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        target = Symbol.Read(reader);
        propertyPath.Read(reader);
        value.Read(reader, revision >= 1);
        rate = reader.ReadInt32();
        blendTime = reader.ReadFloat();
        changePerUnit = reader.ReadFloat();
        ease = reader.ReadInt32();
        easePower = reader.ReadFloat();
        oldFlag = reader.ReadBoolean();
        persistent = reader.ReadBoolean();
        stopMode = reader.ReadInt32();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, target);
        propertyPath.Write(writer);
        value.Write(writer, revision >= 1);
        writer.WriteInt32(rate);
        writer.WriteFloat(blendTime);
        writer.WriteFloat(changePerUnit);
        writer.WriteInt32(ease);
        writer.WriteFloat(easePower);
        writer.WriteBoolean(oldFlag);
        writer.WriteBoolean(persistent);
        writer.WriteInt32(stopMode);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
