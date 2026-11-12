using MiloLib.Classes;
using MiloLib.Utils;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace MiloLib.Assets.Char;

[Name("CharClip"), Description("This is the simple form that stores samples and linearly interpolates between them. Data is grouped by keyframe, for better RAM coherency better storage, interpolation, etc.")]
public class CharClip : Object
{
    public class CharBonesSamples
    {
        // magic numbers related to compression
        private const float ShortUnit = 0.000030518509f;
        private const float CompressedVectorRange = 1300.0f;
        private const float ByteQuaternionUnit = 0.0078740157f;
        private const float CompressedRotationUnit = 0.00061035156f;

        public class Bone
        {
            [Name("Name"), Description("Bone to blend into")]
            public Symbol name = Symbol.Empty;
            [Name("Weight"), Description("Weight to blend with")]
            public float weight = 1.0f;

            public Bone Read(EndianReader reader, bool includeWeight)
            {
                name = Symbol.Read(reader);
                if (includeWeight)
                {
                    weight = reader.ReadFloat();
                }

                return this;
            }

            public void Write(EndianWriter writer, bool includeWeight)
            {
                Symbol.Write(writer, name);
                if (includeWeight)
                {
                    writer.WriteFloat(weight);
                }
            }
        }

        public class VectorChannel
        {
            public Bone bone = new();
            public List<Vector3> samples = new();
        }

        public class QuaternionChannel
        {
            public Bone bone = new();
            public List<Vector4> samples = new();
        }

        public class RotationChannel
        {
            public Bone bone = new();
            public List<float> samples = new();
        }

        public int version;
        public List<Bone> bones = new();
        public int[] counts = new int[10];
        public int compression;
        public int numSamples;
        public List<float> frames = new();
        public bool ver14Bool;
        public byte[] rawData = Array.Empty<byte>();
        public List<VectorChannel> positionChannels = new();
        public List<VectorChannel> scaleChannels = new();
        public List<QuaternionChannel> quaternionChannels = new();
        public List<RotationChannel> rotationXChannels = new();
        public List<RotationChannel> rotationYChannels = new();
        public List<RotationChannel> rotationZChannels = new();
        public bool sampleDataDecoded;
        public string sampleDataDecodeError = string.Empty;

        // revs 6-9 raw ints read before compression
        public int[] oldHeaderInts = Array.Empty<int>();
        public int numCountsInStream;
        public int bytesPerSample;
        public int dataBytesPerSample;
        public bool usePaddedSampleData = true;
        public int sampleDataAlignment = 16;
        public int sampleDataExtraPadding;
        public int uncompressedVectorSize = 12;
        public byte[] sampleDataPrefix = Array.Empty<byte>();
        public Endian? sampleDataEndianness;

        private static int TypeSize(int boneType, int comp, int uncompressedVectorSize)
        {
            switch (boneType)
            {
                case 0: // POS
                case 1: // SCALE
                    return comp >= 2 ? 6 : uncompressedVectorSize;
                case 2: // QUAT
                    if (comp >= 3)
                    {
                        return 4;
                    }

                    if (comp >= 1)
                    {
                        return 8;
                    }

                    return 16;
                case 3: // ROTX
                case 4: // ROTY
                case 5: // ROTZ
                    return comp != 0 ? 2 : 4;
                default:
                    return 0;
            }
        }

        public void ComputeBytesPerSample()
        {
            int offset = 0;
            for (int i = 0; i < 6; i++)
            {
                int numBones = counts[i + 1] - counts[i];
                offset += numBones * TypeSize(i, compression, uncompressedVectorSize);
            }
            dataBytesPerSample = offset;
            bytesPerSample = AlignSampleSize(offset + sampleDataExtraPadding, sampleDataAlignment);
        }

        public static int AlignSampleSize(int size, int alignment)
        {
            return alignment > 1 ? (size + alignment - 1) & ~(alignment - 1) : size;
        }

        public void Load(EndianReader reader)
        {
            version = reader.ReadInt32();
            LoadHeader(reader);
            LoadData(reader);
        }

        public void Save(EndianWriter writer)
        {
            writer.WriteInt32(version);
            WriteHeader(writer);
            WriteData(writer);
        }

        public void LoadHeader(EndianReader reader)
        {
            int numBones = reader.ReadInt32();
            bones = new List<Bone>(Math.Clamp(numBones, 0, 4096));
            bool includeWeight = version > 10;
            for (int i = 0; i < numBones; i++)
            {
                bones.Add(new Bone().Read(reader, includeWeight));
            }

            if (version > 9)
            {
                numCountsInStream = version > 15 ? 7 : 10;
                for (int i = 0; i < numCountsInStream; i++)
                {
                    counts[i] = reader.ReadInt32();
                }

                compression = reader.ReadInt32();
                numSamples = reader.ReadInt32();
            }
            else if (version > 5)
            {
                int intCount;
                if (version > 7)
                {
                    intCount = 9;
                }
                else if (version > 6)
                {
                    intCount = 6;
                }
                else
                {
                    intCount = 10;
                }

                oldHeaderInts = new int[intCount];
                for (int i = 0; i < intCount; i++)
                {
                    oldHeaderInts[i] = reader.ReadInt32();
                }

                compression = reader.ReadInt32();
                numSamples = reader.ReadInt32();
                ComputeCountsFromBones();
            }
            else if (version > 3)
            {
                numSamples = reader.ReadInt32();
                compression = reader.ReadInt32();
                ComputeCountsFromBones();
            }
            else
            {
                numSamples = reader.ReadInt32();
                ComputeCountsFromBones();
            }

            if (version > 11)
            {
                int frameCount = reader.ReadInt32();
                frames = new List<float>(frameCount);
                for (int i = 0; i < frameCount; i++)
                {
                    frames.Add(reader.ReadFloat());
                }
            }
            else
            {
                frames = new List<float>();
            }

            ComputeBytesPerSample();
        }

        public void LoadData(EndianReader reader, int? exactBytes = null)
        {
            if (version == 14)
            {
                ver14Bool = reader.ReadBoolean();
            }

            if (sampleDataPrefix.Length > 0)
            {
                sampleDataPrefix = reader.ReadBlock(sampleDataPrefix.Length);
            }

            int bytesToRead = usePaddedSampleData ? bytesPerSample : dataBytesPerSample;
            int totalBytes = bytesToRead * numSamples;
            if (exactBytes.HasValue)
            {
                totalBytes = exactBytes.Value;
            }

            rawData = totalBytes > 0 ? reader.ReadBlock(totalBytes) : Array.Empty<byte>();
            DecodeChannels(reader.Endianness);
        }

        public bool DecodeChannels(Endian endianness)
        {
            ClearDecodedChannels();
            if (sampleDataEndianness.HasValue)
            {
                endianness = sampleDataEndianness.Value;
            }

            if (version < 13 || version > 16)
            {
                return SetDecodeError($"CharBonesSamples version {version} is not an RB3 sample version");
            }

            if (compression < 0 || compression > 4)
            {
                return SetDecodeError($"CharBonesSamples compression {compression} is invalid");
            }

            if (numSamples < 0)
            {
                return SetDecodeError($"CharBonesSamples sample count {numSamples} is invalid");
            }

            if (uncompressedVectorSize != 12 && uncompressedVectorSize != 16)
            {
                return SetDecodeError($"CharBonesSamples uncompressed vector size {uncompressedVectorSize} is unsupported");
            }

            if (!ValidateCounts())
            {
                return false;
            }

            ComputeBytesPerSample();
            int sampleStride = dataBytesPerSample;
            if (usePaddedSampleData)
            {
                sampleStride = bytesPerSample;
            }

            long requiredBytes = (long)sampleStride * numSamples;
            if (requiredBytes > rawData.Length)
            {
                return SetDecodeError($"CharBonesSamples has {rawData.Length} sample bytes but needs {requiredBytes} bytes");
            }

            AddVectorChannels(positionChannels, 0);
            AddVectorChannels(scaleChannels, 1);
            AddQuaternionChannels();
            AddRotationChannels(rotationXChannels, 3);
            AddRotationChannels(rotationYChannels, 4);
            AddRotationChannels(rotationZChannels, 5);
            ReadOnlySpan<byte> data = rawData;
            for (int sampleIndex = 0; sampleIndex < numSamples; sampleIndex++)
            {
                int offset = sampleIndex * sampleStride;
                DecodeVectorChannels(data, ref offset, endianness, positionChannels);
                DecodeVectorChannels(data, ref offset, endianness, scaleChannels);
                DecodeQuaternionChannels(data, ref offset, endianness);
                DecodeRotationChannels(data, ref offset, endianness, rotationXChannels);
                DecodeRotationChannels(data, ref offset, endianness, rotationYChannels);
                DecodeRotationChannels(data, ref offset, endianness, rotationZChannels);
                int expectedEnd = sampleIndex * sampleStride + dataBytesPerSample;
                if (offset != expectedEnd)
                {
                    return SetDecodeError($"CharBonesSamples decoded sample {sampleIndex} to byte {offset}, but expected byte {expectedEnd}");
                }
            }

            sampleDataDecoded = true;
            return true;
        }

        private void ClearDecodedChannels()
        {
            positionChannels.Clear();
            scaleChannels.Clear();
            quaternionChannels.Clear();
            rotationXChannels.Clear();
            rotationYChannels.Clear();
            rotationZChannels.Clear();
            sampleDataDecoded = false;
            sampleDataDecodeError = string.Empty;
        }

        private bool ValidateCounts()
        {
            if (counts.Length < 7)
            {
                return SetDecodeError("CharBonesSamples requires seven cumulative channel counts");
            }

            int previousCount = 0;
            for (int i = 0; i < 7; i++)
            {
                int count = counts[i];
                if (count < previousCount || count > bones.Count)
                {
                    return SetDecodeError($"CharBonesSamples channel count {i} has invalid value {count}");
                }

                previousCount = count;
            }

            if (counts[6] != bones.Count)
            {
                return SetDecodeError($"CharBonesSamples has {bones.Count} bones but its final channel count is {counts[6]}");
            }

            return true;
        }

        private bool SetDecodeError(string message)
        {
            // something blew up during decoding, this is to catch what it was exactly since it can fail in different ways
            // TODO: probably would be better to throw
            ClearDecodedChannels();
            sampleDataDecodeError = message;
            return false;
        }

        private void AddVectorChannels(List<VectorChannel> channels, int type)
        {
            for (int boneIndex = counts[type]; boneIndex < counts[type + 1]; boneIndex++)
            {
                VectorChannel channel = new();
                channel.bone = bones[boneIndex];
                channel.samples = new List<Vector3>(numSamples);
                channels.Add(channel);
            }
        }

        private void AddQuaternionChannels()
        {
            for (int boneIndex = counts[2]; boneIndex < counts[3]; boneIndex++)
            {
                QuaternionChannel channel = new();
                channel.bone = bones[boneIndex];
                channel.samples = new List<Vector4>(numSamples);
                quaternionChannels.Add(channel);
            }
        }

        private void AddRotationChannels(List<RotationChannel> channels, int type)
        {
            for (int boneIndex = counts[type]; boneIndex < counts[type + 1]; boneIndex++)
            {
                RotationChannel channel = new();
                channel.bone = bones[boneIndex];
                channel.samples = new List<float>(numSamples);
                channels.Add(channel);
            }
        }

        private void DecodeVectorChannels(ReadOnlySpan<byte> data, ref int offset, Endian endianness, List<VectorChannel> channels)
        {
            foreach (VectorChannel channel in channels)
            {
                Vector3 value;
                if (compression >= 2)
                {
                    float x = ReadInt16(data, ref offset, endianness) * ShortUnit * CompressedVectorRange;
                    float y = ReadInt16(data, ref offset, endianness) * ShortUnit * CompressedVectorRange;
                    float z = ReadInt16(data, ref offset, endianness) * ShortUnit * CompressedVectorRange;
                    value = new Vector3(x, y, z);
                }
                else
                {
                    float x = ReadFloat(data, ref offset, endianness);
                    float y = ReadFloat(data, ref offset, endianness);
                    float z = ReadFloat(data, ref offset, endianness);
                    value = new Vector3(x, y, z);
                    offset += uncompressedVectorSize - 12;
                }
                channel.samples.Add(value);
            }
        }

        private void DecodeQuaternionChannels(ReadOnlySpan<byte> data, ref int offset, Endian endianness)
        {
            foreach (QuaternionChannel channel in quaternionChannels)
            {
                Vector4 value;
                if (compression >= 3)
                {
                    float x = ReadSByte(data, ref offset) * ByteQuaternionUnit;
                    float y = ReadSByte(data, ref offset) * ByteQuaternionUnit;
                    float z = ReadSByte(data, ref offset) * ByteQuaternionUnit;
                    float w = ReadSByte(data, ref offset) * ByteQuaternionUnit;
                    value = CreateVector4(x, y, z, w);
                }
                else if (compression != 0)
                {
                    float x = ReadInt16(data, ref offset, endianness) * ShortUnit;
                    float y = ReadInt16(data, ref offset, endianness) * ShortUnit;
                    float z = ReadInt16(data, ref offset, endianness) * ShortUnit;
                    float w = ReadInt16(data, ref offset, endianness) * ShortUnit;
                    value = CreateVector4(x, y, z, w);
                }
                else
                {
                    float x = ReadFloat(data, ref offset, endianness);
                    float y = ReadFloat(data, ref offset, endianness);
                    float z = ReadFloat(data, ref offset, endianness);
                    float w = ReadFloat(data, ref offset, endianness);
                    value = CreateVector4(x, y, z, w);
                }
                channel.samples.Add(value);
            }
        }

        private void DecodeRotationChannels(ReadOnlySpan<byte> data, ref int offset, Endian endianness, List<RotationChannel> channels)
        {
            foreach (RotationChannel channel in channels)
            {
                float value;
                if (compression != 0)
                {
                    value = ReadInt16(data, ref offset, endianness) * CompressedRotationUnit;
                }
                else
                {
                    value = ReadFloat(data, ref offset, endianness);
                }

                channel.samples.Add(value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static short ReadInt16(ReadOnlySpan<byte> data, ref int offset, Endian endianness)
        {
            short value;
            if (endianness == Endian.BigEndian)
            {
                value = BinaryPrimitives.ReadInt16BigEndian(data.Slice(offset, 2));
            }
            else
            {
                value = BinaryPrimitives.ReadInt16LittleEndian(data.Slice(offset, 2));
            }

            offset += 2;
            return value;
        }

        private static Vector4 CreateVector4(float x, float y, float z, float w)
        {
            Vector4 value = new();
            value.x = x;
            value.y = y;
            value.z = z;
            value.w = w;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static sbyte ReadSByte(ReadOnlySpan<byte> data, ref int offset)
        {
            sbyte value = unchecked((sbyte)data[offset]);
            offset++;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float ReadFloat(ReadOnlySpan<byte> data, ref int offset, Endian endianness)
        {
            float value;
            if (endianness == Endian.BigEndian)
            {
                value = BinaryPrimitives.ReadSingleBigEndian(data.Slice(offset, 4));
            }
            else
            {
                value = BinaryPrimitives.ReadSingleLittleEndian(data.Slice(offset, 4));
            }

            offset += 4;
            return value;
        }

        public void WriteHeader(EndianWriter writer)
        {
            writer.WriteInt32(bones.Count);
            bool includeWeight = version > 10;
            foreach (var bone in bones)
            {
                bone.Write(writer, includeWeight);
            }

            if (version > 9)
            {
                int numToWrite = version > 15 ? 7 : 10;
                for (int i = 0; i < numToWrite; i++)
                {
                    writer.WriteInt32(counts[i]);
                }

                writer.WriteInt32(compression);
                writer.WriteInt32(numSamples);
            }
            else if (version > 5)
            {
                foreach (int val in oldHeaderInts)
                {
                    writer.WriteInt32(val);
                }

                writer.WriteInt32(compression);
                writer.WriteInt32(numSamples);
            }
            else if (version > 3)
            {
                writer.WriteInt32(numSamples);
                writer.WriteInt32(compression);
            }
            else
            {
                writer.WriteInt32(numSamples);
            }

            if (version > 11)
            {
                AssetHelpers.WriteList(writer, frames, (float f) => writer.WriteFloat(f));
            }
        }

        public void WriteData(EndianWriter writer)
        {
            if (version == 14)
            {
                writer.WriteBoolean(ver14Bool);
            }

            if (sampleDataPrefix.Length > 0)
            {
                writer.WriteBlock(sampleDataPrefix);
            }

            if (rawData.Length > 0)
            {
                writer.WriteBlock(rawData);
            }
        }

        private void ComputeCountsFromBones()
        {
            for (int i = 0; i < 7; i++)
            {
                counts[i] = 0;
            }

            foreach (var bone in bones)
            {
                int type = GetBoneType(bone.name.value);
                if (type >= 0 && type < 6)
                {
                    counts[type + 1]++;
                }
            }
            for (int i = 1; i < 7; i++)
            {
                counts[i] += counts[i - 1];
            }
        }

        private static int GetBoneType(string name)
        {
            int dotIdx = name.LastIndexOf('.');
            if (dotIdx < 0 || dotIdx >= name.Length - 1)
            {
                return -1;
            }

            switch (name[dotIdx + 1])
            {
                case 'p': return 0;
                case 's': return 1;
                case 'q': return 2;
                case 'r':
                    if (dotIdx + 3 < name.Length)
                    {
                        switch (name[dotIdx + 3])
                        {
                            case 'x': return 3;
                            case 'y': return 4;
                            case 'z': return 5;
                        }
                    }
                    return -1;
                default: return -1;
            }
        }
    }

    public class GraphNode
    {
        [Name("Current Beat"), Description("where to blend from in my clip")]
        public float curBeat;
        [Name("Next Beat"), Description("where to blend to in clip")]
        public float nextBeat;
    }

    public class NodeVector
    {
        [Name("Clip"), Description("clip it's transitioning to")]
        public Symbol clip = Symbol.Empty;
        [Name("Nodes")]
        public List<GraphNode> nodes = new();
    }

    public class BeatEvent
    {
        [Name("Event"), Description("The handler to call on the CharClip")]
        public Symbol event_ = Symbol.Empty;
        [Name("Beat"), Description("Beat the event should trigger")]
        public float beat;
    }

    public class FrameEvent
    {
        public float frame;
        public Symbol script = Symbol.Empty;
    }

    public class BeatTrackKey
    {
        public float frame;
        public float value;
    }

    public enum DefaultBlend
    {
        kPlayNoDefault = 0,
        kPlayNow = 1,
        kPlayNoBlend = 2,
        kPlayFirst = 3,
        kPlayLast = 4,
        kPlayDirty = 8
    }

    public enum DefaultLoop
    {
        kPlayNoLoop = 0x10,
        kPlayLoop = 0x20,
        kPlayGraphLoop = 0x30,
        kPlayNodeLoop = 0x40
    }

    public enum BeatAlignMode
    {
        kPlayBeatTime = 0,
        kPlayRealTime = 0x200,
        kPlayUserTime = 0x400,
        kPlayBeatAlign1 = 0x1000,
        kPlayBeatAlign2 = 0x2000,
        kPlayBeatAlign4 = 0x4000,
        kPlayBeatAlign8 = 0x8000
    }

    private ushort altRevision;
    private ushort revision;
    [MaxVersion(15)]
    private int oldRev;

    [Name("Frames Per Second"), Description("Frames per second")]
    public float framesPerSec = 30.0f;

    [Name("Flags"), Description("Search flags, app specific")]
    public int flags;

    public int playFlags;

    [Name("Range"), Description("Range in frames to randomly offset by when playing")]
    public float range;

    [Name("Relative"), Description("Make the clip all relative to this other clip's first frame")]
    public Symbol relative = Symbol.Empty;

    [Name("Do Not Compress"), Description("Check this to prevent any compression from happening on this clip")]
    public bool doNotCompress;

    public int transitionVersion;

    public int transitionsByteHint;
    [Name("Transitions")]
    public List<NodeVector> transitions = new();

    [Name("Events"), Description("Events that get triggered during play")]
    public List<BeatEvent> beatEvents = new();

    [Name("Sync Anim"), Description("An animatable, like a PropAnim, you'd like play in sync with this clip"), MinVersion(19)]
    public Symbol syncAnim = Symbol.Empty;

    [MinVersion(21)]
    public List<List<KeyValuePair<int, float>>> sampleClipWeights = new();

    [MinVersion(22)]
    public int unk198;

    [Name("Full")]
    public CharBonesSamples full = new();

    [Name("One")]
    public CharBonesSamples one = new();

    [MinVersion(15)]
    public List<CharBonesSamples.Bone> zeros = new();

    [MinVersion(18)]
    public List<BeatTrackKey> beatTrack = new();

    // old format shit preserved for round-tripping
    [MaxVersion(17)]
    private int oldStartBeat;
    private bool hasLegacyClipRevision;
    private bool usesLegoWiiSamples;
    [MinVersion(16), MaxVersion(16)]
    public byte oldSampleFlag;
    [MaxVersion(17)]
    private int oldEndBeat;
    private int blendWidth;
    private bool relativeBool;
    private bool unkBool;
    private List<Symbol> oldStrings = new();
    private Symbol enterEvent = Symbol.Empty;
    private Symbol exitEvent = Symbol.Empty;
    private List<FrameEvent> oldFrameEvents = new();

    [MaxVersion(12)]
    private byte[] oneInitialRawData = Array.Empty<byte>();
    [MaxVersion(12)]
    private bool oneInitialVer14Bool;
    [MinVersion(8), MaxVersion(13)]
    private CharBonesSamples extraBonesSamples;
    [MaxVersion(12)]
    private int fullDataVersion;

    public new CharClip Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        hasLegacyClipRevision = revision < 0x10 || (revision == 0x10 && parent.revision == 25 && entry.type.value == "CharClipSamples");
        usesLegoWiiSamples = revision == 14 && parent.revision == 25 && parent.platform == DirectoryMeta.Platform.Wii && reader.Endianness == Endian.LittleEndian && entry.type.value == "CharClipSamples";
        if (hasLegacyClipRevision)
        {
            oldRev = reader.ReadInt32();
        }
        else
        {
            oldRev = 0xD;
        }

        base.Read(reader, false, parent, entry);

        if (revision < 0x12)
        {
            oldStartBeat = reader.ReadInt32();
            oldEndBeat = reader.ReadInt32();
        }

        framesPerSec = reader.ReadFloat();
        flags = reader.ReadInt32();
        playFlags = reader.ReadInt32();

        if (oldRev < 0xD)
        {
            blendWidth = reader.ReadInt32();
        }

        if (oldRev > 3)
        {
            range = reader.ReadFloat();
        }

        if (oldRev > 5)
        {
            relative = Symbol.Read(reader);
        }
        else if (oldRev > 4)
        {
            relativeBool = reader.ReadBoolean();
        }

        if (oldRev == 9 || oldRev == 10)
        {
            unkBool = reader.ReadBoolean();
        }

        if (oldRev > 9)
        {
            transitionVersion = reader.ReadInt32();
        }

        if (oldRev > 0xB)
        {
            doNotCompress = reader.ReadBoolean();
        }

        ReadTransitions(reader);

        if (oldRev < 3)
        {
            oldStrings = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (oldRev > 6)
        {
            beatEvents = AssetHelpers.ReadList(reader, () => new BeatEvent
            {
                event_ = Symbol.Read(reader),
                beat = reader.ReadFloat()
            });
        }
        else
        {
            enterEvent = Symbol.Read(reader);
            exitEvent = Symbol.Read(reader);
            oldFrameEvents = AssetHelpers.ReadList(reader, () => new FrameEvent
            {
                frame = reader.ReadFloat(),
                script = Symbol.Read(reader)
            });
        }

        if (revision == 16 && hasLegacyClipRevision)
        {
            oldSampleFlag = reader.ReadByte();
        }

        if (revision > 0xC)
        {
            if (revision == 0xD)
            {
                extraBonesSamples = new CharBonesSamples();
            }

            SetSampleDataFormat(parent);
            full.version = reader.ReadInt32();
            full.LoadHeader(reader);
            if (usesLegoWiiSamples)
            {
                DetectWiiSampleLayout(reader, standalone);
            }
            else
            {
                (int detectedAlignment, int detectedExtraPadding, int detectedVectorSize) = DetectFullSampleStride(reader, full);
                full.sampleDataAlignment = detectedAlignment;
                full.sampleDataExtraPadding = detectedExtraPadding;
                full.uncompressedVectorSize = detectedVectorSize;
                one.sampleDataAlignment = detectedAlignment;
                one.sampleDataExtraPadding = detectedExtraPadding;
                one.uncompressedVectorSize = detectedVectorSize;
            }
            full.ComputeBytesPerSample();
            full.LoadData(reader);
            one.Load(reader);

            if (revision == 0xD)
            {
                extraBonesSamples.Load(reader);
            }
        }
        else
        {
            SetSampleDataFormat(parent);
            full.version = revision;
            full.LoadHeader(reader);

            one.version = revision <= 0xB ? revision : reader.ReadInt32();
            one.LoadHeader(reader);

            if (revision > 0xB)
            {
                extraBonesSamples = new CharBonesSamples { version = one.version };
                extraBonesSamples.LoadHeader(reader);
            }

            one.LoadData(reader);

            oneInitialRawData = (byte[])one.rawData.Clone();
            oneInitialVer14Bool = one.ver14Bool;

            fullDataVersion = one.version;
            int savedVer = full.version;
            full.version = one.version;
            if (standalone && revision <= 0xB)
            {
                int? sampleBytes = null;
                if (reader.BaseStream is MemoryStream stream && BinaryParsingUtils.TryGetMemorySliceUntilEndMarker(stream, (int)stream.Position, out _, out int bytesConsumed))
                {
                    sampleBytes = bytesConsumed - BinaryParsingUtils.EndMarker.Length;
                }
                full.LoadData(reader, sampleBytes);
            }
            else
            {
                full.LoadData(reader);
            }

            full.version = savedVer;

            if (!(standalone && revision <= 0xB))
            {
                one.LoadData(reader);
            }
        }

        ReadTrailingFields(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision < 0x10 || hasLegacyClipRevision)
        {
            writer.WriteInt32(oldRev);
        }

        base.Write(writer, false, parent, entry);

        if (revision < 0x12)
        {
            writer.WriteInt32(oldStartBeat);
            writer.WriteInt32(oldEndBeat);
        }

        writer.WriteFloat(framesPerSec);
        writer.WriteInt32(flags);
        writer.WriteInt32(playFlags);

        if (oldRev < 0xD)
        {
            writer.WriteInt32(blendWidth);
        }

        if (oldRev > 3)
        {
            writer.WriteFloat(range);
        }

        if (oldRev > 5)
        {
            Symbol.Write(writer, relative);
        }
        else if (oldRev > 4)
        {
            writer.WriteBoolean(relativeBool);
        }

        if (oldRev == 9 || oldRev == 10)
        {
            writer.WriteBoolean(unkBool);
        }

        if (oldRev > 9)
        {
            writer.WriteInt32(transitionVersion);
        }

        if (oldRev > 0xB)
        {
            writer.WriteBoolean(doNotCompress);
        }

        WriteTransitions(writer);

        if (oldRev < 3)
        {
            AssetHelpers.WriteList(writer, oldStrings, sym => Symbol.Write(writer, sym));
        }

        if (oldRev > 6)
        {
            AssetHelpers.WriteList(writer, beatEvents, evt =>
            {
                Symbol.Write(writer, evt.event_);
                writer.WriteFloat(evt.beat);
            });
        }
        else
        {
            Symbol.Write(writer, enterEvent);
            Symbol.Write(writer, exitEvent);
            AssetHelpers.WriteList(writer, oldFrameEvents, fe =>
            {
                writer.WriteFloat(fe.frame);
                Symbol.Write(writer, fe.script);
            });
        }

        if (revision == 16 && hasLegacyClipRevision)
        {
            writer.WriteByte(oldSampleFlag);
        }

        if (revision > 0xC)
        {
            SetSampleDataFormat(parent);
            full.Save(writer);
            one.Save(writer);

            if (revision == 0xD)
            {
                extraBonesSamples.Save(writer);
            }
        }
        else
        {
            SetSampleDataFormat(parent);
            full.WriteHeader(writer);

            if (revision > 0xB)
            {
                writer.WriteInt32(one.version);
            }

            one.WriteHeader(writer);

            if (revision > 0xB)
            {
                extraBonesSamples.WriteHeader(writer);
            }

            if (one.version == 14)
            {
                writer.WriteBoolean(oneInitialVer14Bool);
            }

            if (oneInitialRawData.Length > 0)
            {
                writer.WriteBlock(oneInitialRawData);
            }

            int savedVer = full.version;
            full.version = fullDataVersion;
            full.WriteData(writer);
            full.version = savedVer;

            if (!(standalone && revision <= 0xB))
            {
                one.WriteData(writer);
            }
        }

        if (revision > 0xE)
        {
            writer.WriteUInt32((uint)zeros.Count);
            foreach (var bone in zeros)
            {
                Symbol.Write(writer, bone.name);
                writer.WriteFloat(bone.weight);
            }
        }

        if (revision > 0x11)
        {
            writer.WriteUInt32((uint)beatTrack.Count);
            foreach (var key in beatTrack)
            {
                writer.WriteFloat(key.value);
                writer.WriteFloat(key.frame);
            }
        }

        if (revision > 0x12)
        {
            Symbol.Write(writer, syncAnim);
        }

        if (revision > 0x14)
        {
            AssetHelpers.WriteList(writer, sampleClipWeights, weights => AssetHelpers.WriteList(writer, weights, weight => { writer.WriteInt32(weight.Key); writer.WriteFloat(weight.Value); }));
        }
        if (revision > 0x15)
        {
            writer.WriteInt32(unk198);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private void ReadTransitions(EndianReader reader)
    {
        if (oldRev < 8)
        {
            transitionsByteHint = -1;
        }
        else
        {
            transitionsByteHint = reader.ReadInt32();
        }

        transitions = AssetHelpers.ReadList(reader, () => new NodeVector { clip = Symbol.Read(reader), nodes = AssetHelpers.ReadList(reader, () => new GraphNode { curBeat = reader.ReadFloat(), nextBeat = reader.ReadFloat() }) });
    }

    private void WriteTransitions(EndianWriter writer)
    {
        if (oldRev >= 8)
        {
            // i think this is right? seems so anyway
            // idk
            int byteHint = 0;
            foreach (var nv in transitions)
            {
                byteHint += 8 + 8 * nv.nodes.Count;
            }

            writer.WriteInt32(transitionsByteHint >= 0 ? transitionsByteHint : byteHint);
        }

        AssetHelpers.WriteList(writer, transitions, nv => { Symbol.Write(writer, nv.clip); AssetHelpers.WriteList(writer, nv.nodes, node => { writer.WriteFloat(node.curBeat); writer.WriteFloat(node.nextBeat); }); });
    }

    private void ReadTrailingFields(EndianReader reader)
    {
        zeros.Clear();
        beatTrack.Clear();
        syncAnim = Symbol.Empty;

        if (revision > 0xE)
        {
            uint zeroCount = reader.ReadUInt32();
            if (zeroCount > 4096)
            {
                throw new InvalidDataException($"CharClip zero count is too high: {zeroCount}");
            }

            for (int i = 0; i < zeroCount; i++)
            {
                zeros.Add(new CharBonesSamples.Bone
                {
                    name = Symbol.Read(reader),
                    weight = reader.ReadFloat()
                });
            }
        }

        if (revision > 0x11)
        {
            uint keyCount = reader.ReadUInt32();
            if (keyCount > 32767)
            {
                throw new InvalidDataException($"CharClip beat track count is too high: {keyCount}");
            }

            for (int i = 0; i < keyCount; i++)
            {
                beatTrack.Add(new BeatTrackKey
                {
                    value = reader.ReadFloat(),
                    frame = reader.ReadFloat()
                });
            }
        }

        if (revision > 0x12)
        {
            syncAnim = Symbol.Read(reader);
        }

        if (revision > 0x14)
        {
            sampleClipWeights = AssetHelpers.ReadList(reader, () => AssetHelpers.ReadList(reader, () => new KeyValuePair<int, float>(reader.ReadInt32(), reader.ReadFloat())));
        }
        if (revision > 0x15)
        {
            unk198 = reader.ReadInt32();
        }
    }

    private static (int Alignment, int ExtraPadding, int VectorSize) DetectFullSampleStride(EndianReader reader, CharBonesSamples samples)
    {
        if (!samples.usePaddedSampleData || samples.numSamples <= 0)
        {
            return (samples.sampleDataAlignment, samples.sampleDataExtraPadding, samples.uncompressedVectorSize);
        }

        // i hate this
        long dataStart = reader.BaseStream.Position;
        foreach ((int alignment, int extraPadding) in new[] { (16, 0), (16, 16), (32, 0), (32, 16), (64, 0), (64, 16) })
        {
            int stride = CharBonesSamples.AlignSampleSize(samples.dataBytesPerSample + extraPadding, alignment);
            long nextHeader = dataStart + (long)stride * samples.numSamples;
            if (LooksLikeCharBonesSamplesHeader(reader, nextHeader))
            {
                return (alignment, extraPadding, samples.uncompressedVectorSize);
            }

            if (alignment == 16 && extraPadding == 0 && samples.uncompressedVectorSize == 12 && samples.compression < 2)
            {
                int vectorCount = samples.counts[2] - samples.counts[0];
                int vectorStride = CharBonesSamples.AlignSampleSize(samples.dataBytesPerSample + vectorCount * 4, 16);
                long vectorNextHeader = dataStart + (long)vectorStride * samples.numSamples;
                if (LooksLikeCharBonesSamplesHeader(reader, vectorNextHeader))
                {
                    return (16, 0, 16);
                }
            }
        }

        return (samples.sampleDataAlignment, samples.sampleDataExtraPadding, samples.uncompressedVectorSize);
    }

    private static bool LooksLikeCharBonesSamplesHeader(EndianReader reader, long position)
    {
        long saved = reader.BaseStream.Position;
        try
        {
            if (position < 0 || reader.BaseStream.Length - position < 8)
            {
                return false;
            }

            reader.BaseStream.Position = position;
            int version = reader.ReadInt32();
            int boneCount = reader.ReadInt32();
            if (version < 13 || version > 16 || boneCount < 0 || boneCount > 4096)
            {
                return false;
            }

            bool includeWeight = version > 10;
            for (int i = 0; i < boneCount; i++)
            {
                if (reader.BaseStream.Length - reader.BaseStream.Position < 4)
                {
                    return false;
                }

                uint symbolLength = reader.ReadUInt32();
                if (symbolLength > 512 || reader.BaseStream.Length - reader.BaseStream.Position < symbolLength)
                {
                    return false;
                }

                reader.BaseStream.Position += symbolLength;
                if (includeWeight)
                {
                    if (reader.BaseStream.Length - reader.BaseStream.Position < 4)
                    {
                        return false;
                    }

                    reader.BaseStream.Position += 4;
                }
            }

            int countValues = version > 15 ? 7 : 10;
            if (reader.BaseStream.Length - reader.BaseStream.Position < (countValues + 3) * 4)
            {
                return false;
            }

            int previousCount = 0;
            for (int i = 0; i < countValues; i++)
            {
                int count = reader.ReadInt32();
                if (i < 7)
                {
                    if (count < previousCount || count > boneCount)
                    {
                        return false;
                    }

                    previousCount = count;
                }
                else if (count != previousCount)
                {
                    return false;
                }
            }

            int compression = reader.ReadInt32();
            int samples = reader.ReadInt32();
            int frameCount = reader.ReadInt32();
            if (compression < 0 || compression > 4 || samples < 0 || frameCount < 0 || frameCount > 100000)
            {
                return false;
            }

            if (reader.BaseStream.Length - reader.BaseStream.Position < (long)frameCount * 4)
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            reader.BaseStream.Position = saved;
        }
    }

    private void DetectWiiSampleLayout(EndianReader reader, bool standalone)
    {
        long dataStart = reader.BaseStream.Position;
        try
        {
            for (int prefixLength = 0; prefixLength < 4; prefixLength++)
            {
                long nextHeader = dataStart + prefixLength + (long)full.bytesPerSample * full.numSamples;
                if (!HasZeroSamplePrefix(reader, dataStart, prefixLength) || !LooksLikeCharBonesSamplesHeader(reader, nextHeader))
                {
                    continue;
                }

                reader.BaseStream.Position = nextHeader;
                CharBonesSamples candidate = new() { version = reader.ReadInt32(), sampleDataAlignment = 4, uncompressedVectorSize = 12 };
                candidate.LoadHeader(reader);
                long oneDataStart = reader.BaseStream.Position;
                int onePrefixLength = (int)((dataStart + prefixLength - oneDataStart) & 3);
                long endPosition = oneDataStart + onePrefixLength + (long)candidate.bytesPerSample * candidate.numSamples;
                if (!HasZeroSamplePrefix(reader, oneDataStart, onePrefixLength) || endPosition > reader.BaseStream.Length)
                {
                    continue;
                }

                reader.BaseStream.Position = endPosition;
                if (standalone && !AssetHelpers.IsEndBytes(reader))
                {
                    continue;
                }

                full.sampleDataPrefix = new byte[prefixLength];
                one.sampleDataPrefix = new byte[onePrefixLength];
                return;
            }

            if (HasUnpaddedWiiSampleLayout(reader, dataStart, standalone))
            {
                usesLegoWiiSamples = false;
                full.usePaddedSampleData = false;
                one.usePaddedSampleData = false;
                full.sampleDataAlignment = 16;
                one.sampleDataAlignment = 16;
                full.sampleDataEndianness = null;
                one.sampleDataEndianness = null;
                return;
            }

            throw new InvalidDataException("Wii CharClipSamples blocks do not match the next sample header and end marker");
        }
        finally
        {
            reader.BaseStream.Position = dataStart;
        }
    }

    private bool HasUnpaddedWiiSampleLayout(EndianReader reader, long dataStart, bool standalone)
    {
        long nextHeader = dataStart + (long)full.dataBytesPerSample * full.numSamples;
        if (full.version == 14)
        {
            nextHeader++;
        }

        if (!LooksLikeCharBonesSamplesHeader(reader, nextHeader))
        {
            return false;
        }

        reader.BaseStream.Position = nextHeader;
        CharBonesSamples candidate = new() { version = reader.ReadInt32(), usePaddedSampleData = false, uncompressedVectorSize = 12 };
        candidate.LoadHeader(reader);
        long endPosition = reader.BaseStream.Position + (long)candidate.dataBytesPerSample * candidate.numSamples;
        if (candidate.version == 14)
        {
            endPosition++;
        }

        if (endPosition > reader.BaseStream.Length)
        {
            return false;
        }

        reader.BaseStream.Position = endPosition;
        if (standalone && !AssetHelpers.IsEndBytes(reader))
        {
            return false;
        }

        return true;
    }

    private static bool HasZeroSamplePrefix(EndianReader reader, long position, int length)
    {
        if (position < 0 || reader.BaseStream.Length - position < length)
        {
            return false;
        }

        reader.BaseStream.Position = position;
        for (int i = 0; i < length; i++)
        {
            if (reader.ReadByte() != 0)
            {
                return false;
            }
        }
        return true;
    }

    private void SetSampleDataFormat(DirectoryMeta parent)
    {
        bool usePaddedSampleData = parent.platform != DirectoryMeta.Platform.Wii;
        int uncompressedVectorSize = 12;
        if (parent.platform is DirectoryMeta.Platform.Xbox or DirectoryMeta.Platform.PS3)
        {
            uncompressedVectorSize = 16;
        }

        if (usesLegoWiiSamples)
        {
            usePaddedSampleData = true;
            full.sampleDataAlignment = 4;
            one.sampleDataAlignment = 4;
            full.sampleDataEndianness = Endian.BigEndian;
            one.sampleDataEndianness = Endian.BigEndian;
        }
        full.usePaddedSampleData = usePaddedSampleData;
        one.usePaddedSampleData = usePaddedSampleData;
        full.uncompressedVectorSize = uncompressedVectorSize;
        one.uncompressedVectorSize = uncompressedVectorSize;
        if (extraBonesSamples != null)
        {
            extraBonesSamples.usePaddedSampleData = usePaddedSampleData;
            extraBonesSamples.uncompressedVectorSize = uncompressedVectorSize;
        }
    }

}
