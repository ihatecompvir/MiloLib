using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowSound"), Description("Plays a sound cue")]
public class FlowSound : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("Immediate Release"), Description("do not wait for sound to finish before finishing flow execution")]
    public bool immediateRelease;
    [Name("Sound"), Description("The sound file to play")]
    public Symbol sound = Symbol.Empty;
    [Name("Volume"), Description("Volume of the sound, in Db")]
    public float volume;
    [Name("Pan"), Description("Pan of the sound, -4 to +4")]
    public float pan;
    [Name("Transpose"), Description("pitch adjustment of the sound in semitones")]
    public float transpose;
    [Name("Stop Mode"), Description("How should we handle stop requests?")]
    public int stopMode;
    [Name("Force Stop"), Description("If true, we stop all instances of this sound from playing")]
    public bool forceStop;
    [Name("Use Intensity"), Description("Do we pass on running intensity to volume?")]
    public bool useIntensity;

    public new FlowSound Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        immediateRelease = reader.ReadBoolean();
        sound = Symbol.Read(reader);
        volume = reader.ReadFloat();
        pan = reader.ReadFloat();
        transpose = reader.ReadFloat();
        stopMode = reader.ReadInt32();
        if (revision > 0)
        {
            forceStop = reader.ReadBoolean();
        }
        if (revision > 2)
        {
            useIntensity = reader.ReadBoolean();
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
        writer.WriteBoolean(immediateRelease);
        Symbol.Write(writer, sound);
        writer.WriteFloat(volume);
        writer.WriteFloat(pan);
        writer.WriteFloat(transpose);
        writer.WriteInt32(stopMode);
        if (revision > 0)
        {
            writer.WriteBoolean(forceStop);
        }
        if (revision > 2)
        {
            writer.WriteBoolean(useIntensity);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
