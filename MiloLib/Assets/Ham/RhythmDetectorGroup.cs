using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("RhythmDetectorGroup"), Description("Group of RhythmDetectors that combines their output.")]
public class RhythmDetectorGroup : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Detectors"), Description("The detectors")]
    public List<Symbol> detectors = new();
    [Name("Skeleton Index"), Description("The group's skeleton index")]
    public int skeletonIndex;

    public new RhythmDetectorGroup Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        if (revision >= 1)
        {
            detectors = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision >= 2)
        {
            skeletonIndex = reader.ReadInt32();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        base.Write(writer, false, parent, entry);
        if (revision >= 1)
        {
            AssetHelpers.WriteList(writer, detectors, item => Symbol.Write(writer, item));
        }
        if (revision >= 2)
        {
            writer.WriteInt32(skeletonIndex);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
