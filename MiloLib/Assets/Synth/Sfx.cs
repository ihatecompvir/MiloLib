using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("Sfx"), Description("Basic sound effect object. Plays several samples with a given volume, pan, transpose, and envelope settings.")]
public class Sfx : Object
{
    public class MoggClip
    {

        [Name("Mogg Clip"), Description("Which moggclip to play")]
        public Symbol clipName = Symbol.Empty;
        [Name("Pan"), Description("Surround pan, between -4 and 4")]
        public float pan;
        [Name("Pan Width"), Description("Surround pan width, between 0 and 4")]
        public float panWidth;
        [Name("Volume"), Description("Volume in dB (0 is full volume, -96 is silence)")]
        public float volume;
        [Name("Is Stereo"), Description("Is the mogg clip stereo?")]
        public bool isStereo;

        public MoggClip Read(EndianReader reader)
        {

            clipName = Symbol.Read(reader);
            pan = reader.ReadFloat();
            panWidth = reader.ReadFloat();
            volume = reader.ReadFloat();
            isStereo = reader.ReadBoolean();

            return this;
        }

        public void Write(EndianWriter writer)
        {

            Symbol.Write(writer, clipName);
            writer.WriteFloat(pan);
            writer.WriteFloat(panWidth);
            writer.WriteFloat(volume);
            writer.WriteBoolean(isStereo);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Send"), Description("Effect chain to use"), MinVersion(5)]
    public Symbol sendObj = Symbol.Empty;

    [Name("Faders"), Description("Faders affecting this sound effect"), MinVersion(9)]
    public FaderGroup faderGroup = new();

    private uint moggClipCount;
    [Name("Moggclip Maps"), MinVersion(10)]
    public List<MoggClip> moggClips = new();

    [Name("Reverb Mix DB"), Description("Reverb send for this sfx"), MinVersion(12)]
    public float reverbMixDb;

    [Name("Reverb Enable"), Description("Enable reverb send"), MinVersion(12)]
    public bool reverbSendEnable;

    private uint sfxMapsCount;

    [Name("Sfxmaps")]
    public List<SfxMap> sfxMaps = new();

    public Sequence sequence = new();

    public uint unkInt1;

    public Sfx()
    {
        sfxMaps = new List<SfxMap>();
        moggClips = new List<MoggClip>();
    }

    public new Sfx Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 6)
        {
            if (1 < revision)
            {
                objFields.Read(reader, parent, entry);
            }
        }
        else
        {
            sequence = sequence.Read(reader, parent, entry);
        }

        sfxMapsCount = reader.ReadUInt32();

        // sanity check on sfxMaps count
        if (sfxMapsCount > 100)
        {
            throw new InvalidDataException("SfxMap count is too high, Sfx is invalid");
        }

        for (int i = 0; i < sfxMapsCount; i++)
        {
            SfxMap map = new();
            map.Read(reader, revision);
            sfxMaps.Add(map);
        }

        if (9 < revision)
        {
            moggClipCount = reader.ReadUInt32();

            // sanity check on moggClip count
            if (moggClipCount > 100)
            {
                throw new InvalidDataException("MoggClip count is too high, Sfx is invalid");
            }


            for (int i = 0; i < moggClipCount; i++)
            {
                MoggClip clip = new();
                clip.Read(reader);
                moggClips.Add(clip);
            }
        }

        if (4 < revision)
        {
            sendObj = Symbol.Read(reader);
            if (revision <= 7)
            {
                unkInt1 = reader.ReadUInt32();
            }
        }

        if (revision >= 9)
        {
            faderGroup = faderGroup.Read(reader);
        }

        if (revision >= 12)
        {
            reverbMixDb = reader.ReadFloat();
            reverbSendEnable = reader.ReadBoolean();
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision < 6)
        {
            if (1 < revision)
            {
                objFields.Write(writer, parent);
            }
        }
        else
        {
            sequence.Write(writer, false, parent, entry);
        }

        AssetHelpers.WriteList(writer, sfxMaps, map => map.Write(writer, revision));

        if (9 < revision)
        {
            AssetHelpers.WriteList(writer, moggClips, clip => clip.Write(writer));
        }

        if (4 < revision)
        {
            Symbol.Write(writer, sendObj);
            if (revision <= 7)
            {
                writer.WriteUInt32(unkInt1);
            }
        }

        if (revision >= 9)
        {
            faderGroup.Write(writer);
        }

        if (revision >= 12)
        {
            writer.WriteFloat(reverbMixDb);
            writer.WriteBoolean(reverbSendEnable);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public class Sequence : Object
    {
        private ushort altRevision;
        private ushort revision;
        [Name("Avg Volume"), Description("Average volume this sequence will be played at, in dB")]
        public float avgVol;
        [Name("Volume Spread"), Description("Amount to vary the volume above and below the average, in dB")]
        public float volSpread;
        [Name("Avg Transpose"), Description("Average transpose this sequence will be played at, in semitones")]
        public float avgTranspose;
        [Name("Transpose Spread"), Description("Amount to vary the transpose, in semitones")]
        public float transposeSpread;
        [Name("Avg Pan"), Description("Average pan to apply to this sequence (-1 - 1)")]
        public float avgPan;
        [Name("Pan Spread"), Description("Amount to vary the pan")]
        public float panSpread;
        [Name("Can Stop"), Description("If false, this sequence will play to its end and can't be stopped prematurely")]
        public bool canStop;

        public new Sequence Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
        {
            AssetHelpers.ReadRevision(reader, out revision, out altRevision);

            if (2 < revision)
            {
                base.Read(reader, false, parent, entry);
            }

            avgVol = reader.ReadFloat();
            volSpread = reader.ReadFloat();
            avgTranspose = reader.ReadFloat();
            transposeSpread = reader.ReadFloat();
            avgPan = reader.ReadFloat();
            panSpread = reader.ReadFloat();

            if (1 < revision)
            {
                canStop = reader.ReadBoolean();
            }

            if (standalone)
            {
                AssetHelpers.ReadEnd(reader, parent, entry, revision);
            }

            return this;
        }

        public Sequence Read(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
        {
            return Read(reader, false, parent, entry);
        }

        public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
        {
            AssetHelpers.WriteRevision(writer, revision, altRevision);

            if (2 < revision)
            {
                base.Write(writer, false, parent, entry);
            }

            writer.WriteFloat(avgVol);
            writer.WriteFloat(volSpread);
            writer.WriteFloat(avgTranspose);
            writer.WriteFloat(transposeSpread);
            writer.WriteFloat(avgPan);
            writer.WriteFloat(panSpread);

            if (1 < revision)
            {
                writer.WriteBoolean(canStop);
            }

            if (standalone)
            {
                writer.WriteEndBytes();
            }
        }
    }
    public class SfxMap
    {
        public enum FXCore : int
        {
            kFXCoreNone = -1,
            kFXCore0 = 0,
            kFXCore1 = 1,

        }
        [Name("Sample Name"), Description("Which sample to play")]
        public Symbol sampleName = Symbol.Empty;
        [Name("Volume"), Description("Volume in dB (0 is full volume, -96 is silence)")]
        public float volume { get; set; }
        [Name("Pan"), Description("Surround pan, between -4 and 4")]
        public float pan { get; set; }
        [Name("Transpose"), Description("Transpose in half steps")]
        public float transpose { get; set; }
        [Name("FX Core"), Description("Which core's digital FX should be used in playing this sample")]
        public FXCore fxCore { get; set; }
        [Name("ADSR")]
        public ADSR adsr = new();

        public void Read(EndianReader reader, uint version)
        {
            sampleName = Symbol.Read(reader);

            volume = reader.ReadFloat();
            pan = reader.ReadFloat();
            transpose = reader.ReadFloat();
            fxCore = (FXCore)reader.ReadInt32();
            adsr = new ADSR();
            adsr.Read(reader);
        }

        public void Write(EndianWriter writer, uint version)
        {
            Symbol.Write(writer, sampleName);
            writer.WriteFloat(volume);
            writer.WriteFloat(pan);
            writer.WriteFloat(transpose);
            writer.WriteInt32((int)fxCore);

            adsr.Write(writer);
        }

        public override string ToString()
        {
            return $"{sampleName}, vol: {volume}, pan: {pan}, transpose: {transpose}, fxCore: {fxCore}, ADSR: {adsr}";
        }
    }
}

public class FaderGroup
{
    private ushort altRevision;
    private ushort revision;
    private int fadersCount;
    [Name("Faders"), Description("Faders affecting this sound effect")]
    public List<Symbol> faders = new();

    public FaderGroup Read(EndianReader reader)
    {
        altRevision = reader.ReadUInt16();
        revision = reader.ReadUInt16();
        fadersCount = reader.ReadInt32();

        // sanity check on faders count
        if (fadersCount > 100)
        {
            throw new InvalidDataException("Fader count is too high, FaderGroup is invalid");
        }

        faders = new List<Symbol>();
        for (int i = 0; i < fadersCount; i++)
        {
            faders.Add(Symbol.Read(reader));
        }

        return this;
    }

    public void Write(EndianWriter writer)
    {

        writer.WriteUInt16(altRevision);
        writer.WriteUInt16(revision);
        AssetHelpers.WriteList(writer, faders, fader => Symbol.Write(writer, fader));
    }
}

public class ADSR
{
    private ushort altRevision;
    private ushort revision;
    [Name("Sustain Level"), Description("Level of sustain volume (0-1)")]
    public float sustainLevel;
    [Name("Release Rate"), Description("Duration of release in seconds")]
    public float releaseRate;
    [Name("Sustain Rate"), Description("Duration of sustain in seconds")]
    public float sustainRate;
    [Name("Decay Rate"), Description("Duration of decay in seconds")]
    public float decayRate;
    [Name("Attack Rate"), Description("Duration of attack in seconds")]
    public float attackRate;
    [Name("Release Mode"), Description("Release mode")]
    public uint releaseMode;
    [Name("Sustain Mode"), Description("Sustain mode")]
    public uint sustainMode;
    [Name("Attack Mode"), Description("Attack mode")]
    public uint attackMode;

    public void Read(EndianReader reader)
    {
        altRevision = reader.ReadUInt16();
        revision = reader.ReadUInt16();

        sustainLevel = reader.ReadFloat();
        releaseRate = reader.ReadFloat();
        sustainRate = reader.ReadFloat();
        decayRate = reader.ReadFloat();
        attackRate = reader.ReadFloat();
        releaseMode = reader.ReadUInt32();
        sustainMode = reader.ReadUInt32();
        attackMode = reader.ReadUInt32();

    }

    public void Write(EndianWriter writer)
    {
        writer.WriteUInt16(altRevision);
        writer.WriteUInt16(revision);
        writer.WriteFloat(sustainLevel);
        writer.WriteFloat(releaseRate);
        writer.WriteFloat(sustainRate);
        writer.WriteFloat(decayRate);
        writer.WriteFloat(attackRate);
        writer.WriteUInt32(releaseMode);
        writer.WriteUInt32(sustainMode);
        writer.WriteUInt32(attackMode);
    }

    public override string ToString()
    {
        return $"({sustainLevel}, {releaseRate}, {sustainRate}, {decayRate}, {attackRate}, {releaseMode}, {sustainMode}, {attackMode})";
    }
}

