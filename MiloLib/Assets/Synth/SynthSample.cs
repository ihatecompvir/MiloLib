using MiloLib.Utils;
using MiloLib.Classes;

namespace MiloLib.Assets;


[Name("SynthSample"), Description("A single mono waveform residing in a bank.")]
public class SynthSample : Object
{
    public class SampleData
    {
        public class SampleMarker
        {
            [Name("Name")]
            public Symbol name = Symbol.Empty;
            [Name("Sample")]
            public int sample;

            public SampleMarker Read(EndianReader reader)
            {
                name = Symbol.Read(reader);
                sample = reader.ReadInt32();
                return this;
            }

            public void Write(EndianWriter writer)
            {
                Symbol.Write(writer, name);
                writer.WriteInt32(sample);
            }

            public override string ToString()
            {
                return $"{name} {sample}";
            }
        }

        public enum Encoding
        {
            kPCM,
            kBigEndPCM,
            kVAG,
            kXMA,
            kATRAC,
            kMP3,
            kNintendoADPCM
        }
        private ushort altRevision;
        private ushort revision;

        [MinVersion(15)]
        public uint crc;

        [MinVersion(16)]
        public int numChannels = 1;

        [Name("Encoding"), Description("The format of the sample data.")]
        public Encoding encoding;

        public uint sampleCount;

        [Name("Sample Rate"), Description("Sample rate, in Hz")]
        public uint sampleRate;

        private uint samplesSize;

        [Name("Should Read Samples"), Description("Whether or not the game should read the sample data. This must be checked if sample data is present.")]
        public bool readSamples;

        [Name("Samples"), Description("The raw samples of the audio file. These are not playable on their own if extracted.")]
        public byte[] samples = Array.Empty<byte>();

        [Name("Markers"), MinVersion(14)]
        public List<SampleMarker> markers = new();

        public SampleData Read(EndianReader reader)
        {
            AssetHelpers.ReadRevision(reader, out revision, out altRevision);

            if (revision > 14)
            {
                crc = reader.ReadUInt32();
            }

            encoding = (Encoding)reader.ReadUInt32();

            sampleCount = reader.ReadUInt32();
            sampleRate = reader.ReadUInt32();
            samplesSize = reader.ReadUInt32();

            readSamples = true;
            if (revision >= 11)
            {
                readSamples = reader.ReadBoolean();
            }

            if (readSamples)
            {
                samples = reader.ReadBlock(checked((int)samplesSize));
            }
            else
            {
                samples = Array.Empty<byte>();
            }

            if (revision >= 14)
            {
                markers = AssetHelpers.ReadList(reader, () => new SampleMarker().Read(reader));
            }

            if (revision >= 16)
            {
                numChannels = reader.ReadInt32();
            }

            return this;
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteRevision(writer, revision, altRevision);

            if (revision > 14)
            {
                writer.WriteUInt32(crc);
            }

            writer.WriteUInt32((uint)encoding);

            writer.WriteUInt32(sampleCount);
            writer.WriteUInt32(sampleRate);
            if (readSamples)
            {
                samplesSize = (uint)samples.Length;
            }

            writer.WriteUInt32(samplesSize);

            if (revision >= 11)
            {
                writer.WriteBoolean(readSamples);
            }

            if (readSamples)
            {
                writer.WriteBlock(samples);
            }

            if (revision >= 14)
            {
                AssetHelpers.WriteList(writer, markers, (SampleMarker marker) => marker.Write(writer));
            }

            if (revision >= 16)
            {
                writer.WriteInt32(numChannels);
            }
        }
    }
    private ushort altRevision;
    private ushort revision;

    [Name("File Name"), Description("Mono, 16-bit sample file")]
    public Symbol file = Symbol.Empty;

    [Name("Looped"), Description("Loop this sample"), MaxVersion(5)]
    public bool looped;

    [Name("Loop Start Sample"), Description("Start of the loop, in samples. Ignored if \"looped\" is unchecked."), MaxVersion(5)]
    public uint loopStartSample;
    [Name("Loop End Sample"), Description("End of the loop, in samples. Use -1 for the end of the sample."), MinVersion(3)]
    public int loopEndSample;

    [MinVersion(6)]
    public uint unkInt;

    public SampleData sampleData = new();

    // wii sample data, stored like how MoggClip stores it
    public byte[] sampleDataWii = Array.Empty<byte>();

    public new SynthSample Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 1)
        {
            objFields = objFields.Read(reader, parent, entry);
        }

        file = Symbol.Read(reader);

        if (revision < 6)
        {
            looped = reader.ReadBoolean();
            loopStartSample = reader.ReadUInt32();
        }

        if (revision > 2 && revision < 6)
        {
            loopEndSample = reader.ReadInt32();
        }

        if (revision < 5)
        {

        }
        else if (altRevision != 0)
        {
            if (standalone)
            {
                sampleDataWii = BinaryParsingUtils.ReadUntilEndMarker(reader);
                return this;
            }
        }
        else
        {
            sampleData = sampleData.Read(reader);
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

        if (revision > 1)
        {
            objFields.Write(writer, parent);
        }

        Symbol.Write(writer, file);

        if (revision < 6)
        {
            writer.WriteBoolean(looped);
            writer.WriteUInt32(loopStartSample);
        }


        if (revision > 2 && revision < 6)
        {
            writer.WriteInt32(loopEndSample);
        }

        if (revision < 5)
        {

        }
        else if (altRevision != 0)
        {
            if (standalone)
            {
                if (sampleDataWii.Length > 0)
                {
                    long blobStart = writer.Position;
                    writer.WriteBlock(sampleDataWii);
                    AssetHelpers.MarkFileLoaderChunks(writer, blobStart, sampleDataWii);
                }

                writer.WriteEndBytes();
                return;
            }
        }
        else
        {
            sampleData.Write(writer);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
