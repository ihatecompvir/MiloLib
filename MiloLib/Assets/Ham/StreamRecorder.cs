using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("StreamRecorder"), Description("Stores a series of depth buffer textures")]
public class StreamRecorder : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    public Object poll = new();
    public Symbol oldInput = Symbol.Empty;
    [Name("Output Mat"), Description("Material for playback")]
    public Symbol outputMat = Symbol.Empty;
    [Name("Max Frames"), Description("Max number of depth buffer textures stored")]
    public int maxFrames;
    public int oldValue;
    [Name("Use Alpha"), Description("Should the next recording have transparent texels?")]
    public bool useAlpha;
    [Name("Playback Speed"), Description("Speed at which to play back depth buffer textures")]
    public int playbackSpeed;
    public int outputWidth;
    public int outputHeight;

    public new StreamRecorder Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 5);
        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        if (revision >= 3)
        {
            poll.Read(reader, false, parent, entry);
        }
        if (revision < 4)
        {
            oldInput = Symbol.Read(reader);
        }
        outputMat = Symbol.Read(reader);
        maxFrames = reader.ReadInt32();
        if (revision < 2)
        {
            oldValue = reader.ReadInt32();
        }
        useAlpha = reader.ReadBoolean();
        playbackSpeed = reader.ReadInt32();
        if (revision >= 5)
        {
            outputWidth = reader.ReadInt32();
            outputHeight = reader.ReadInt32();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 5);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        if (revision >= 3)
        {
            poll.Write(writer, false, parent, entry);
        }
        if (revision < 4)
        {
            Symbol.Write(writer, oldInput);
        }
        Symbol.Write(writer, outputMat);
        writer.WriteInt32(maxFrames);
        if (revision < 2)
        {
            writer.WriteInt32(oldValue);
        }
        writer.WriteBoolean(useAlpha);
        writer.WriteInt32(playbackSpeed);
        if (revision >= 5)
        {
            writer.WriteInt32(outputWidth);
            writer.WriteInt32(outputHeight);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
