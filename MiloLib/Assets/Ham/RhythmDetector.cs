using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("RhythmDetector"), Description("An object that performs rhythmic analysis on Kinect input.")]
public class RhythmDetector : Object
{
    private ushort revision;
    private ushort altRevision;
    public float beats;
    public int fold;
    public float toleranceFactor;
    public Vector3 direction = new();

    public new RhythmDetector Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        if (revision >= 1)
        {
            beats = reader.ReadFloat();
        }
        if (revision >= 2)
        {
            fold = reader.ReadInt32();
            toleranceFactor = reader.ReadFloat();
            direction.Read(reader);
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
            writer.WriteFloat(beats);
        }
        if (revision >= 2)
        {
            writer.WriteInt32(fold);
            writer.WriteFloat(toleranceFactor);
            direction.Write(writer);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
