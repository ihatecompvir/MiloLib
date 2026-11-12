using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("EventAnim"), Description("Triggers events on proxy's along a timeline")]
public class EventAnim : Object
{
    public struct EventCall
    {
        [Name("Dir")]
        public Symbol dir;
        [Name("Event")]
        public Symbol eventName;

        public void Read(EndianReader reader)
        {
            dir = Symbol.Read(reader);
            eventName = Symbol.Read(reader);
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, dir ?? Symbol.Empty);
            Symbol.Write(writer, eventName ?? Symbol.Empty);
        }
    }

    public struct KeyFrame
    {
        [Name("Time"), Description("time for event")]
        public float time;
        [Name("Events")]
        public List<EventCall> calls;

        public void Read(EndianReader reader)
        {
            time = reader.ReadFloat();
            calls = AssetHelpers.ReadList(reader, () => { EventCall call = new(); call.Read(reader); return call; });
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteFloat(time);
            AssetHelpers.WriteList(writer, calls, call => call.Write(writer));
        }
    }

    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    [Name("Keys"), Description("Triggers played during a particular keyframe. Do not set key to zero, hoping it will always get fired on start, put them in start instead.")]
    public List<KeyFrame> keys = new();
    [Name("Start"), Description("Triggers played first, during StartAnim"), MinVersion(1)]
    public List<EventCall> start = new();
    [Name("Reset On End"), Description("Resets all start + keys events when done, during EndAnim, normally should be checked"), MinVersion(1)]
    public bool resetStart;
    [Name("End"), Description("Triggers played when done, during EndAnim"), MinVersion(1)]
    public List<EventCall> end = new();

    public new EventAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        anim = anim.Read(reader, parent, entry);

        keys = AssetHelpers.ReadList(reader, () => { KeyFrame key = new(); key.Read(reader); return key; });

        if (revision != 0)
        {
            start = AssetHelpers.ReadList(reader, () => { EventCall call = new(); call.Read(reader); return call; });

            resetStart = reader.ReadBoolean();

            end = AssetHelpers.ReadList(reader, () => { EventCall call = new(); call.Read(reader); return call; });
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);
        anim.Write(writer);

        AssetHelpers.WriteList(writer, keys, key => key.Write(writer));

        if (revision != 0)
        {
            AssetHelpers.WriteList(writer, start, call => call.Write(writer));

            writer.WriteBoolean(resetStart);

            AssetHelpers.WriteList(writer, end, call => call.Write(writer));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
