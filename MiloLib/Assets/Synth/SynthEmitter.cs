using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("SynthEmitter"), Description("A 3D positional emitter. The volume and surround-panning of a sfx are controlled by its position relative to a listener (usually the camera).")]
public class SynthEmitter : Object
{
    [HideInInspector]
    private ushort revision = 3;
    [HideInInspector]
    private ushort altRevision;

    public RndTrans trans = new();
    public RndDrawable draw = new();


    [Name("Sfx"), Description("sfx this emitter should play")]
    public Symbol sfx = Symbol.Empty;
    [Name("Listener"), Description("object representing the listener's position")]
    public Symbol listener = Symbol.Empty;
    [Name("Enabled"), Description("enable/disable the emitter")]
    public bool enabled;
    [Name("Outer Radius"), Description("the sound starts playing when you cross inside this radius.")]
    public float outerRadius;
    [Name("Inner Radius"), Description("volume and pan are fixed inside this radius.")]
    public float innerRadius;
    [Name("Outer Volume"), Description("volume at outer radius, in dB")]
    public float outerVolume;
    [Name("Inner Volume"), Description("volume at inner radius (and inside)")]
    public float innerVolume;

    public new SynthEmitter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        trans.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        sfx = Symbol.Read(reader);
        listener = Symbol.Read(reader);
        enabled = reader.ReadBoolean();
        if (revision > 1)
        {
            outerRadius = reader.ReadFloat();
            innerRadius = reader.ReadFloat();
        }
        if (revision > 2)
        {
            outerVolume = reader.ReadFloat();
            innerVolume = reader.ReadFloat();
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
        trans.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, sfx);
        Symbol.Write(writer, listener);
        writer.WriteBoolean(enabled);
        if (revision > 1)
        {
            writer.WriteFloat(outerRadius);
            writer.WriteFloat(innerRadius);
        }
        if (revision > 2)
        {
            writer.WriteFloat(outerVolume);
            writer.WriteFloat(innerVolume);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
