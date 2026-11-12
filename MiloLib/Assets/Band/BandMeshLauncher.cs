using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandMeshLauncher"), Description("A MeshLauncher object emits meshes along a path.")]
public class BandMeshLauncher : Object
{
    private ushort altRevision;
    private ushort revision;

    private Object firstObject = new();

    [Name("Multi Mesh"), Description("Controlled MultiMesh to emit from.")]
    public Symbol multiMesh = Symbol.Empty;

    [Name("Part"), Description("Particle system to launch from.")]
    public Symbol part = Symbol.Empty;

    [Name("Trans")]
    public Symbol trans = Symbol.Empty;

    [Name("Num Parts"), Description("Number of particles to launch.")]
    public int numParts;

    [Name("Use Life")]
    public bool useLife;

    [Name("Life")]
    public float life;

    [Name("Use Speed")]
    public bool useSpeed;

    [Name("Speed")]
    public float speed;

    [Name("Use Size")]
    public bool useSize;

    [Name("Size")]
    public float size;

    [Name("Use Delta Size")]
    public bool useDeltaSize;

    [Name("Delta Size")]
    public float deltaSize;

    [Name("Use Start Color")]
    public bool useStartColor;

    [Name("Start Color")]
    public HmxColor4 startColor = new();

    [Name("Use Mid Color")]
    public bool useMidColor;

    [Name("Mid Color")]
    public HmxColor4 midColor = new();

    [Name("Use End Color")]
    public bool useEndColor;

    [Name("End Color")]
    public HmxColor4 endColor = new();

    [Name("Use Emit Direction")]
    public bool useEmitDirection;

    [Name("Pitch")]
    public Vector2 pitch = new();

    [Name("Yaw")]
    public Vector2 yaw = new();

    public new BandMeshLauncher Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        firstObject = new Object();
        if (revision < 2)
        {
            firstObject.Read(reader, false, parent, entry);
        }

        base.Read(reader, false, parent, entry);
        multiMesh = Symbol.Read(reader);

        if (revision > 0)
        {
            part = Symbol.Read(reader);
            trans = Symbol.Read(reader);
            numParts = reader.ReadInt32();
            useLife = reader.ReadBoolean();
            life = reader.ReadFloat();
            useSpeed = reader.ReadBoolean();
            speed = reader.ReadFloat();
            useSize = reader.ReadBoolean();
            size = reader.ReadFloat();
            useDeltaSize = reader.ReadBoolean();
            deltaSize = reader.ReadFloat();
            useStartColor = reader.ReadBoolean();
            startColor = new HmxColor4().Read(reader);
            useMidColor = reader.ReadBoolean();
            midColor = new HmxColor4().Read(reader);
            useEndColor = reader.ReadBoolean();
            endColor = new HmxColor4().Read(reader);
            useEmitDirection = reader.ReadBoolean();
            pitch = new Vector2().Read(reader);
            yaw = new Vector2().Read(reader);
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        if (revision < 2)
        {
            firstObject.Write(writer, false, parent, entry);
        }

        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, multiMesh);

        if (revision > 0)
        {
            Symbol.Write(writer, part);
            Symbol.Write(writer, trans);
            writer.WriteInt32(numParts);
            writer.WriteBoolean(useLife);
            writer.WriteFloat(life);
            writer.WriteBoolean(useSpeed);
            writer.WriteFloat(speed);
            writer.WriteBoolean(useSize);
            writer.WriteFloat(size);
            writer.WriteBoolean(useDeltaSize);
            writer.WriteFloat(deltaSize);
            writer.WriteBoolean(useStartColor);
            startColor.Write(writer);
            writer.WriteBoolean(useMidColor);
            midColor.Write(writer);
            writer.WriteBoolean(useEndColor);
            endColor.Write(writer);
            writer.WriteBoolean(useEmitDirection);
            pitch.Write(writer);
            yaw.Write(writer);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
