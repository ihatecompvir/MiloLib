using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowAnimate"), Description("Plays an animation")]
public class FlowAnimate : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Anim"), Description("Anim object to animate")]
    public Symbol anim = Symbol.Empty;
    [Name("Blend"), Description("Blend time, does not work on Property Animations!")]
    public float blend;
    [Name("Wait"), Description("wait until current animation finishes before starting")]
    public bool wait;
    [Name("Delay"), Description("delay in units before starting this animation")]
    public float delay;
    [Name("Stop Mode"), Description("How should we handle stop requests?")]
    public int stopMode;
    [Name("Enable"), Description("Enable animation filtering")]
    public bool enable;
    [Name("Rate"), Description("Rate to animate")]
    public int rate;
    [Name("Start"), Description("Start frame of animation")]
    public float start;
    [Name("End"), Description("End frame of animation")]
    public float end;
    [Name("Period"), Description("Period of animation if non-zero")]
    public float period;
    [Name("Type"), Description("How the animation is played")]
    public Symbol animationType = Symbol.Empty;
    [Name("Scale"), Description("Scale of animation")]
    public float scale;
    [Name("Ease"), Description("Easing to apply to animation")]
    public int ease;
    [Name("Ease Power"), Description("Modifier to easing equation")]
    public float easePower;
    [Name("Wrap"), Description("Wraps animation frame values into range rather than clamping them. This will make the animation loop when the frame is out of range.")]
    public bool wrap;
    [Name("Immediate Release"), Description("If true, Flow will not track/stop or otherwise affect this animation again.")]
    public bool immediateRelease;

    public new FlowAnimate Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        anim = Symbol.Read(reader);
        blend = reader.ReadFloat();
        wait = reader.ReadBoolean();
        delay = reader.ReadFloat();
        stopMode = reader.ReadInt32();
        enable = reader.ReadBoolean();
        rate = reader.ReadInt32();
        start = reader.ReadFloat();
        end = reader.ReadFloat();
        period = reader.ReadFloat();
        animationType = Symbol.Read(reader);
        scale = reader.ReadFloat();
        if (revision > 0)
        {
            ease = reader.ReadInt32();
            easePower = reader.ReadFloat();
            wrap = reader.ReadBoolean();
        }
        if (revision > 1)
        {
            immediateRelease = reader.ReadBoolean();
        }
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
        Symbol.Write(writer, anim);
        writer.WriteFloat(blend);
        writer.WriteBoolean(wait);
        writer.WriteFloat(delay);
        writer.WriteInt32(stopMode);
        writer.WriteBoolean(enable);
        writer.WriteInt32(rate);
        writer.WriteFloat(start);
        writer.WriteFloat(end);
        writer.WriteFloat(period);
        Symbol.Write(writer, animationType);
        writer.WriteFloat(scale);
        if (revision > 0)
        {
            writer.WriteInt32(ease);
            writer.WriteFloat(easePower);
            writer.WriteBoolean(wrap);
        }
        if (revision > 1)
        {
            writer.WriteBoolean(immediateRelease);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
