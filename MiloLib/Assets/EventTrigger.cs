using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("EventTrigger"), Description("Triggers animations, sfx, and responses to game events")]
public class EventTrigger : Object
{
    public enum AnimTrigger : uint
    {
        kTriggerAnimNone,
        kTriggerAnimStart,
        kTriggerAnimEnd,
        kTriggerAnimFrame
    }
    public class Anim
    {
        [Name("Anim")]
        public Symbol anim = Symbol.Empty;
        [Name("Blend")]
        public float blend;
        [Name("Delay")]
        public float delay;
        [Name("Wait")]
        public bool wait;
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
        [Name("Scale"), Description("Scale of animation")]
        public float scale;
        [Name("Type"), Description("Type of animation")]
        public Symbol type = Symbol.Empty;

        public Anim Read(EndianReader reader, ushort revision)
        {
            anim = Symbol.Read(reader);
            blend = reader.ReadFloat();
            wait = reader.ReadBoolean();
            delay = reader.ReadFloat();
            if (revision > 9)
            {
                enable = reader.ReadBoolean();
                rate = reader.ReadInt32();
                start = reader.ReadFloat();
                end = reader.ReadFloat();
                period = reader.ReadFloat();
                type = Symbol.Read(reader);
                scale = reader.ReadFloat();
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, anim);
            writer.WriteFloat(blend);
            writer.WriteBoolean(wait);
            writer.WriteFloat(delay);
            if (revision > 9)
            {
                writer.WriteBoolean(enable);
                writer.WriteInt32(rate);
                writer.WriteFloat(start);
                writer.WriteFloat(end);
                writer.WriteFloat(period);
                Symbol.Write(writer, type);
                writer.WriteFloat(scale);
            }
        }

        public override string ToString()
        {
            return $"{anim} {blend} {delay} {wait} {enable} {rate} {scale} {start} {end} {period} {type}";
        }
    }

    public class ProxyCall
    {
        [Name("Proxy")]
        public Symbol proxy = Symbol.Empty;
        [Name("Call")]
        public Symbol call = Symbol.Empty;
        [Name("Event"), Description("event trigger to fire")]
        public Symbol evt = Symbol.Empty;

        public ProxyCall Read(EndianReader reader, ushort revision)
        {
            proxy = Symbol.Read(reader);
            call = Symbol.Read(reader);
            if (revision > 10)
            {
                evt = Symbol.Read(reader);
            }

            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, proxy);
            Symbol.Write(writer, call);
            if (revision > 10)
            {
                Symbol.Write(writer, evt);
            }
        }
    }

    public class HideDelay
    {
        [Name("Hide"), Description("object to hide")]
        public Symbol hide = Symbol.Empty;
        [Name("Delay"), Description("seconds to delay hiding")]
        public float delay;
        [Name("Rate"), Description("rate by which to measure delay")]
        public int rate;

        public HideDelay Read(EndianReader reader)
        {
            hide = Symbol.Read(reader);
            delay = reader.ReadFloat();
            rate = reader.ReadInt32();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, hide);
            writer.WriteFloat(delay);
            writer.WriteInt32(rate);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [MinVersion(16)]
    public RndAnimatable anim = new();

    [Name("Trigger Events"), Description("Events which enable this trigger"), MinVersion(10)]
    public List<Symbol> triggerEvents = new();

    [MinVersion(7), MaxVersion(9)]
    private Symbol unkSym1 = Symbol.Empty;

    [Name("Anims"), MinVersion(7)]
    public List<Anim> anims = new();

    [Name("Sounds"), MinVersion(7)]
    public List<Symbol> sounds = new();

    [Name("Shows"), Description("objects to show"), MinVersion(7)]
    public List<Symbol> shows = new();

    [Name("Hide Delays"), MinVersion(9)]
    public List<HideDelay> hideDelays = new();

    [Name("Movies"), Description("Movies to unpause."), MinVersion(13), MaxVersion(14)]
    public List<Symbol> movies = new();

    [Name("Unpause Delay"), MinVersion(14), MaxVersion(14)]
    public float unpauseDelay;

    [Name("Enable Events"), Description("event which enable this event trigger"), MinVersion(3)]
    public List<Symbol> enableEvents = new();

    [Name("Disable Events"), Description("event which disable this event trigger"), MinVersion(3)]
    public List<Symbol> disableEvents = new();

    [Name("Wait For Events"), Description("When events are triggered, arm them, and delay fire until this handler is called"), MinVersion(6)]
    public List<Symbol> waitForEvents = new();

    [Name("Next Link"), MinVersion(7)]
    public Symbol nextLink = Symbol.Empty;

    [Name("Proxy Calls"), MinVersion(8)]
    public List<ProxyCall> proxyCalls = new();


    [Name("Trigger Order"), Description("Order of triggering next_link"), MinVersion(12)]
    public uint triggerOrder;

    [Name("Triggers To Reset"), Description("these triggers will be reset (i.e. hide its shows, etc.) when this trigger fires"), MinVersion(14)]
    public List<Symbol> resetTriggers = new();

    [Name("Enabled At Start"), Description("Initial state of the EventTrigger. If TRUE, the EventTrigger will work initially, if FALSE the EventTrigger will not work until an enable_event happens."), MinVersion(15)]
    public bool enabledAtStart;

    [Name("Anim Trigger"), Description("Sets whether and how this will trigger itself based on Anim calls to it"), MinVersion(16)]
    public AnimTrigger animTrigger;
    [Name("Anim Frame"), Description("If anim_trigger is kAnimTriggerFrame, then this is the frame we will trigger ourselves on"), MinVersion(16)]
    public float animFrame;

    [Name("Part Launchers"), MinVersion(17)]
    public List<Symbol> partLaunchers = new();


    [MinVersion(7), MaxVersion(8)]
    public List<Symbol> draws = new();

    private byte[] gh2OpaqueBytes = Array.Empty<byte>();

    private bool UsesLegoMovieLayout(DirectoryMeta parent)
    {
        return parent.revision == 25 && (revision == 13 || revision == 14);
    }


    public new EventTrigger Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (parent.revision <= 25 && standalone && revision < 7)
        {
            gh2OpaqueBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
            return this;
        }

        base.Read(reader, false, parent, entry);

        if (revision > 0xF)
        {
            anim = anim.Read(reader, parent, entry);
        }

        if (revision > 9)
        {
            triggerEvents = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        else if (revision > 6)
        {
            unkSym1 = Symbol.Read(reader);
            triggerEvents.Add(unkSym1);
        }

        if (revision > 6)
        {
            anims = AssetHelpers.ReadList(reader, () => new Anim().Read(reader, revision));

            sounds = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            shows = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (UsesLegoMovieLayout(parent) && revision == 14)
        {
            unpauseDelay = reader.ReadFloat();
            movies = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 0xC && !UsesLegoMovieLayout(parent))
        {
            hideDelays = AssetHelpers.ReadList(reader, () => new HideDelay().Read(reader));
        }
        else if (revision > 8)
        {
            hideDelays = AssetHelpers.ReadList(reader, () => new HideDelay { hide = Symbol.Read(reader), delay = reader.ReadFloat() });
        }
        else if (revision > 6)
        {
            draws = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (UsesLegoMovieLayout(parent) && revision == 13)
        {
            movies = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 2)
        {
            enableEvents = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            disableEvents = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 5)
        {
            waitForEvents = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 6)
        {
            nextLink = Symbol.Read(reader);
        }

        if (revision > 7)
        {
            proxyCalls = AssetHelpers.ReadList(reader, () => new ProxyCall().Read(reader, revision));
        }

        if (revision > 0xB)
        {
            triggerOrder = reader.ReadUInt32();
        }

        if (revision > 0xD && !UsesLegoMovieLayout(parent))
        {
            resetTriggers = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 0xE)
        {
            enabledAtStart = reader.ReadBoolean();
        }

        if (revision > 0xF)
        {
            animTrigger = (AnimTrigger)reader.ReadUInt32();
            animFrame = reader.ReadFloat();
        }

        if (revision > 0x10)
        {
            partLaunchers = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
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

        if (parent.revision <= 25 && standalone && revision < 7 && gh2OpaqueBytes.Length != 0)
        {
            writer.WriteBlock(gh2OpaqueBytes);
            writer.WriteEndBytes();
            return;
        }

        base.Write(writer, false, parent, entry);

        if (revision > 0xF)
        {
            anim.Write(writer);
        }

        if (revision > 9)
        {
            AssetHelpers.WriteList(writer, triggerEvents, triggerEvent => Symbol.Write(writer, triggerEvent));
        }
        else if (revision > 6)
        {
            Symbol.Write(writer, unkSym1);
        }

        if (revision > 6)
        {
            AssetHelpers.WriteList(writer, anims, anim => anim.Write(writer, revision));

            AssetHelpers.WriteList(writer, sounds, sound => Symbol.Write(writer, sound));

            AssetHelpers.WriteList(writer, shows, show => Symbol.Write(writer, show));
        }

        if (UsesLegoMovieLayout(parent) && revision == 14)
        {
            writer.WriteFloat(unpauseDelay);
            AssetHelpers.WriteList(writer, movies, movie => Symbol.Write(writer, movie));
        }

        if (revision > 0xC && !UsesLegoMovieLayout(parent))
        {
            AssetHelpers.WriteList(writer, hideDelays, hideDelay => hideDelay.Write(writer));
        }
        else if (revision > 8)
        {
            AssetHelpers.WriteList(writer, hideDelays, hideDelay => { Symbol.Write(writer, hideDelay.hide); writer.WriteFloat(hideDelay.delay); });
        }
        else if (revision > 6)
        {
            AssetHelpers.WriteList(writer, draws, draw => Symbol.Write(writer, draw));
        }

        if (UsesLegoMovieLayout(parent) && revision == 13)
        {
            AssetHelpers.WriteList(writer, movies, (Symbol movie) => Symbol.Write(writer, movie));
        }

        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, enableEvents, enableEvent => Symbol.Write(writer, enableEvent));

            AssetHelpers.WriteList(writer, disableEvents, disableEvent => Symbol.Write(writer, disableEvent));
        }

        if (revision > 5)
        {
            AssetHelpers.WriteList(writer, waitForEvents, waitForEvent => Symbol.Write(writer, waitForEvent));
        }

        if (revision > 6)
        {
            Symbol.Write(writer, nextLink);
        }

        if (revision > 7)
        {
            AssetHelpers.WriteList(writer, proxyCalls, proxyCall => proxyCall.Write(writer, revision));
        }

        if (revision > 0xB)
        {
            writer.WriteUInt32(triggerOrder);
        }

        if (revision > 0xD && !UsesLegoMovieLayout(parent))
        {
            AssetHelpers.WriteList(writer, resetTriggers, resetTrigger => Symbol.Write(writer, resetTrigger));

        }

        if (revision > 0xE)
        {
            writer.WriteBoolean(enabledAtStart);
        }

        if (revision > 0xF)
        {
            writer.WriteUInt32((uint)animTrigger);
            writer.WriteFloat(animFrame);
        }

        if (revision > 0x10)
        {
            AssetHelpers.WriteList(writer, partLaunchers, partLauncher => Symbol.Write(writer, partLauncher));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
