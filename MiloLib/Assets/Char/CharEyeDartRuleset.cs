using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharEyeDartRuleset"), Description("Defines a procedural ruleset that can be used to generate a variety of realistic eye darting behavior")]
public class CharEyeDartRuleset : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Min Radius"), Description("Dart locations will be at least this distance from target")]
    public float minRadius;

    [Name("Max Radius"), Description("Dart locations will be at most this distance from target")]
    public float maxRadius;

    public float onTargetAngleThresh;

    [Name("Min Darts Per Sequence"), Description("Each sequence of darts will have at least this many locations before returning to target")]
    public int minDartsPerSequence;

    [Name("Max Darts Per Sequence"), Description("Each sequence of darts will have at most this many locations before returning to target")]
    public int maxDartsPerSequence;

    [Name("Min Secs Between Darts"), Description("In seconds, the minimum time between each dart in a sequence. should be very small")]
    public float minSecsBetweenDarts;

    [Name("Max Secs Between Darts"), Description("In seconds, the maximum time between each dart in a sequence. should be very small")]
    public float maxSecsBetweenDarts;

    [Name("Min Secs Between Sequences"), Description("In seconds, the minimum time to wait to start another dart sequence after the previous one finishes")]
    public float minSecsBetweenSequences;

    [Name("Max Secs Between Sequences"), Description("In seconds, the maximum time to wait to start another dart sequence after the previous one finishes")]
    public float maxSecsBetweenSequences;

    [Name("Scale With Distance"), Description("Sets whether the dart radius scales with the distance to the character - meaning this dart sequence looks the same regardless of distance")]
    public bool scaleWithDistance;

    [Name("Reference Distance"), Description("When scale_with_distance is true, this distance is used as a reference to scale up the radius.")]
    public float referenceDistance;

    public new CharEyeDartRuleset Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        minRadius = reader.ReadFloat();
        maxRadius = reader.ReadFloat();
        onTargetAngleThresh = reader.ReadFloat();
        minDartsPerSequence = reader.ReadInt32();
        maxDartsPerSequence = reader.ReadInt32();
        minSecsBetweenDarts = reader.ReadFloat();
        maxSecsBetweenDarts = reader.ReadFloat();
        minSecsBetweenSequences = reader.ReadFloat();
        maxSecsBetweenSequences = reader.ReadFloat();
        scaleWithDistance = reader.ReadBoolean();
        referenceDistance = reader.ReadFloat();

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);

        writer.WriteFloat(minRadius);
        writer.WriteFloat(maxRadius);
        writer.WriteFloat(onTargetAngleThresh);
        writer.WriteInt32(minDartsPerSequence);
        writer.WriteInt32(maxDartsPerSequence);
        writer.WriteFloat(minSecsBetweenDarts);
        writer.WriteFloat(maxSecsBetweenDarts);
        writer.WriteFloat(minSecsBetweenSequences);
        writer.WriteFloat(maxSecsBetweenSequences);
        writer.WriteBoolean(scaleWithDistance);
        writer.WriteFloat(referenceDistance);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
