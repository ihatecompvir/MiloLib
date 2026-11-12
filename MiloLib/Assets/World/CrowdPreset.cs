using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("CrowdPreset")]
public class CrowdPreset : Object
{
    private ushort revision = 0;
    private ushort altRevision;
    private uint behaviorRevision;
    [Name("Bounce Noise")]
    public Vector3 bounceNoise = new();
    [Name("Bounce Extents")]
    public Vector3 bounceExtents = new();
    [Name("Surge Noise")]
    public Vector2 surgeNoise = new();
    [Name("Surge Extents")]
    public Vector2 surgeExtents = new();
    [Name("Behavior")]
    public int behavior;
    [Name("Bounce Subdivisions")]
    public int bounceSubdivisions;
    [Name("Bounce Rate")]
    public float bounceRate;
    [Name("Surge Rate")]
    public float surgeRate;
    [Name("Surge Position")]
    public float surgePosition;
    [Name("Surge Width")]
    public float surgeWidth;
    [Name("Surge Cyclical")]
    public bool surgeCyclical;

    public new CrowdPreset Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        behaviorRevision = reader.ReadUInt32();
        if (behaviorRevision != 0)
        {
            throw new NotSupportedException($"Unsupported crowdpreset revision {behaviorRevision}.");
        }

        bounceNoise.Read(reader);
        bounceExtents.Read(reader);
        surgeNoise.Read(reader);
        surgeExtents.Read(reader);
        behavior = reader.ReadInt32();
        bounceSubdivisions = reader.ReadInt32();
        bounceRate = reader.ReadFloat();
        surgeRate = reader.ReadFloat();
        surgePosition = reader.ReadFloat();
        surgeWidth = reader.ReadFloat();
        surgeCyclical = reader.ReadBoolean();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        if (behaviorRevision != 0)
        {
            throw new NotSupportedException($"Unsupported CrpwdPreset revision {behaviorRevision}.");
        }

        writer.WriteUInt32(behaviorRevision);
        bounceNoise.Write(writer);
        bounceExtents.Write(writer);
        surgeNoise.Write(writer);
        surgeExtents.Write(writer);
        writer.WriteInt32(behavior);
        writer.WriteInt32(bounceSubdivisions);
        writer.WriteFloat(bounceRate);
        writer.WriteFloat(surgeRate);
        writer.WriteFloat(surgePosition);
        writer.WriteFloat(surgeWidth);
        writer.WriteBoolean(surgeCyclical);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

