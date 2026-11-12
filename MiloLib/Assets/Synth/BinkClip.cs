using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("BinkClip"), Description("Allows dynamic playback of Bink-based audio clips.")]
public class BinkClip : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("File"), Description("The bik audio file to be played.")]
    public Symbol filePath = Symbol.Empty;

    [Name("Volume"), Description("Volume in dB (0 is full volume, -96 is silence).")]
    public float volume;

    [Name("Loop"), Description("If true, this audio clip should loop during playback.")]
    public bool loop;

    [Name("Preload")]
    public bool preload;

    public byte[] bytes = Array.Empty<byte>();

    public new BinkClip Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        filePath = Symbol.Read(reader);
        volume = reader.ReadFloat();
        loop = reader.ReadBoolean();
        preload = reader.ReadBoolean();

        if (standalone)
        {
            if (revision > 0)
            {
                bytes = BinaryParsingUtils.ReadUntilEndMarker(reader);
            }
            else
            {
                AssetHelpers.ReadEnd(reader, parent, entry, revision);
            }
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);

        Symbol.Write(writer, filePath);
        writer.WriteFloat(volume);
        writer.WriteBoolean(loop);
        writer.WriteBoolean(preload);

        if (standalone)
        {
            if (revision > 0 && bytes.Length > 0)
            {
                long blobStart = writer.Position;
                writer.WriteBlock(bytes);
                AssetHelpers.MarkFileLoaderChunks(writer, blobStart, bytes);
            }

            writer.WriteEndBytes();
        }
    }
}
