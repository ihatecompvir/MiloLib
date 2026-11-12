using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("Sound"), Description("Basic sound effect object. Plays several samples with a given volume, pan, transpose, and envelope settings.")]
public class Sound : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Volume"), Description("Volume in dB (0 is full volume, -96 is silence)")]
    public float volume;
    [Name("Transpose"), Description("Transpose in half steps (SynthSample only)")]
    public float transpose;
    [Name("Pan"), Description("Surround pan, between -4 and 4")]
    public float pan;
    public bool unkBool;
    [Name("Sample"), Description("Which sample to play")]
    public Symbol synthSample = Symbol.Empty;
    [Name("Reverb Mix DB"), Description("Reverb send for this sfx")]
    public float reverbMixDb;
    [Name("Reverb Enable"), Description("Enable reverb send (SynthSample only)")]
    public bool reverbEnable;
    [Name("Envelope"), Description("Envelope for this sound (SynthSample only)")]
    public Symbol envelope = Symbol.Empty;
    [Name("Send"), Description("Effect chain to use")]
    public Symbol send = Symbol.Empty;
    [Name("Mogg"), Description("Which sample to play")]
    public Symbol moggClip = Symbol.Empty;
    [Name("Loop"), Description("Loop this sample")]
    public bool loop;
    [Name("Loop Start"), Description("Start of the loop, in samples. Ignored if \"looped\" is unchecked.")]
    public int loopStart;
    [Name("Loop End"), Description("End of the loop, in samples. Use -1 for the end of the sample.")]
    public int loopEnd;
    [Name("Max Polyphony"), Description("Maximum number of times this sound can be playing simultaneously. Use 0 for no limit. (SynthSample only)")]
    public int maxPolyphony;
    public int faderGroupRevision;
    [Name("Faders"), Description("Faders affecting this sound")]
    public List<Symbol> faders = new();
    [Name("Duckers")]
    public List<AudioDucker> duckers = new();

    public class AudioDucker
    {
        [Name("Fader"), Description("Fader that will be ducked.")]
        public Symbol fader = Symbol.Empty;
        [Name("Level"), Description("Ducking volume level in dB")]
        public float level;

        public AudioDucker Read(EndianReader reader)
        {
            fader = Symbol.Read(reader);
            level = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, fader);
            writer.WriteFloat(level);
        }
    }

    public new Sound Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 9);
        base.Read(reader, false, parent, entry);
        volume = reader.ReadFloat();
        transpose = reader.ReadFloat();
        pan = reader.ReadFloat();
        unkBool = reader.ReadBoolean();
        synthSample = Symbol.Read(reader);
        if (revision >= 2)
        {
            reverbMixDb = reader.ReadFloat();
            reverbEnable = reader.ReadBoolean();
        }
        if (revision >= 3)
        {
            envelope = Symbol.Read(reader);
        }
        if (revision >= 4)
        {
            send = Symbol.Read(reader);
        }
        if (revision >= 5)
        {
            moggClip = Symbol.Read(reader);
        }
        if (revision >= 6)
        {
            loop = reader.ReadBoolean();
            loopStart = reader.ReadInt32();
            loopEnd = reader.ReadInt32();
        }
        if (revision >= 7)
        {
            maxPolyphony = reader.ReadInt32();
        }
        if (revision >= 8)
        {
            faderGroupRevision = reader.ReadInt32();
            faders = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision >= 9)
        {
            duckers = AssetHelpers.ReadList(reader, () => new AudioDucker().Read(reader));
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 9);
        base.Write(writer, false, parent, entry);
        writer.WriteFloat(volume);
        writer.WriteFloat(transpose);
        writer.WriteFloat(pan);
        writer.WriteBoolean(unkBool);
        Symbol.Write(writer, synthSample);
        if (revision >= 2)
        {
            writer.WriteFloat(reverbMixDb);
            writer.WriteBoolean(reverbEnable);
        }
        if (revision >= 3)
        {
            Symbol.Write(writer, envelope);
        }
        if (revision >= 4)
        {
            Symbol.Write(writer, send);
        }
        if (revision >= 5)
        {
            Symbol.Write(writer, moggClip);
        }
        if (revision >= 6)
        {
            writer.WriteBoolean(loop);
            writer.WriteInt32(loopStart);
            writer.WriteInt32(loopEnd);
        }
        if (revision >= 7)
        {
            writer.WriteInt32(maxPolyphony);
        }
        if (revision >= 8)
        {
            writer.WriteInt32(faderGroupRevision);
            AssetHelpers.WriteList(writer, faders, value => Symbol.Write(writer, value));
        }
        if (revision >= 9)
        {
            AssetHelpers.WriteList(writer, duckers, value => value.Write(writer));
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
