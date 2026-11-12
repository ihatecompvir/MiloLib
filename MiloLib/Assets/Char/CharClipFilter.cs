using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharClipFilter"), Description("Callback object for CharClipSet, calls filter_clips on dir at load")]
public class CharClipFilter : Object
{
    private byte[] gh2OpaqueBytes = Array.Empty<byte>();

    public new CharClipFilter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        long start = reader.BaseStream.Position;
        uint possibleRevision = reader.ReadUInt32();
        reader.BaseStream.Position = start;

        if (parent.revision == 25 && possibleRevision == 3)
        {
            gh2OpaqueBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
            return this;
        }

        base.Read(reader, standalone, parent, entry);
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (parent.revision == 25 && gh2OpaqueBytes.Length != 0)
        {
            writer.WriteBlock(gh2OpaqueBytes);
            if (standalone)
            {
                writer.WriteEndBytes();
            }
            return;
        }

        base.Write(writer, standalone, parent, entry);
    }
}
