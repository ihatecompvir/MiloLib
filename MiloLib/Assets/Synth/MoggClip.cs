using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("MoggClip"), Description("Allows dynamic playback of Mogg-based audio clips, most notably crowd audio loops.")]
public class MoggClip : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("File"), Description("The mogg audio file to be played.")]
    public Symbol filePath = Symbol.Empty;

    [Name("Volume"), Description("Volume in dB (0 is full volume, -96 is silence).")]
    public float volume;

    [Name("Loop"), Description("If true, this audio clip should loop during playback."), MaxVersion(2)]
    public bool loop;

    [Name("Loop Start Sample"), Description("Start of the loop, in samples. Ignored if \"looped\" is unchecked."), MinVersion(2), MaxVersion(2)]
    public int loopStart;
    [Name("Loop End Sample"), Description("End of the loop, in samples. Use -1 for the end of the sample."), MinVersion(2), MaxVersion(2)]
    public int loopEnd;

    [Name("Buffer Seconds"), Description("Number of seconds to buffer (uses default value if set to 0.0)")]
    public float bufferSeconds;

    public byte[] bytes = Array.Empty<byte>();

    public new MoggClip Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        filePath = Symbol.Read(reader);
        volume = reader.ReadFloat();
        if (revision <= 2)
        {
            loop = reader.ReadBoolean();
            if (revision > 1)
            {
                loopStart = reader.ReadInt32();
                loopEnd = reader.ReadInt32();
            }
        }

        if (altRevision > 1)
        {
            bufferSeconds = reader.ReadFloat();
        }

        if (standalone)
        {
            bytes = BinaryParsingUtils.ReadUntilEndMarker(reader);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);

        Symbol.Write(writer, filePath);
        writer.WriteFloat(volume);
        if (revision <= 2)
        {
            writer.WriteBoolean(loop);
            if (revision > 1)
            {
                writer.WriteInt32(loopStart);
                writer.WriteInt32(loopEnd);
            }
        }

        if (altRevision > 1)
        {
            writer.WriteFloat(bufferSeconds);
        }

        if (standalone)
        {
            if (bytes.Length > 0)
            {
                long blobStart = writer.Position;
                writer.WriteBlock(bytes);
                AssetHelpers.MarkFileLoaderChunks(writer, blobStart, bytes);
            }

            writer.WriteEndBytes();
        }
    }

}
