using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("TrackRegion")]
public class TrackRegion : Object
{
    private ushort revision = 3;
    private ushort altRevision;
    [Name("Mesh")]
    public Symbol mesh = Symbol.Empty;
    [Name("Start Bone")]
    public Symbol startBone = Symbol.Empty;
    [Name("End Bone")]
    public Symbol endBone = Symbol.Empty;
    [Name("Z Adjustment")]
    public float zAdjustment;
    [Name("Segment Length")]
    public float segmentLength;
    [Name("Current Duration"), Description("Duration of the current overlay in seconds.")]
    public float currentDuration;

    public new TrackRegion Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        if (revision != 3)
        {
            throw new NotSupportedException($"Unsupported TrackRegion revision {revision}.");
        }

        base.Read(reader, false, parent, entry);
        mesh = Symbol.Read(reader);
        startBone = Symbol.Read(reader);
        endBone = Symbol.Read(reader);
        zAdjustment = reader.ReadFloat();
        segmentLength = reader.ReadFloat();
        currentDuration = reader.ReadFloat();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (revision != 3)
        {
            throw new NotSupportedException($"Unsupported TrackRegion revision {revision}.");
        }

        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, mesh);
        Symbol.Write(writer, startBone);
        Symbol.Write(writer, endBone);
        writer.WriteFloat(zAdjustment);
        writer.WriteFloat(segmentLength);
        writer.WriteFloat(currentDuration);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

