using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("PartLauncher"), Description("Event object to launch a specified number of particles off a particle system")]
public class RndPartLauncher : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Part"), Description("Particle sytem to launch from")]
    public Symbol part = Symbol.Empty;

    [Name("Trans"), Description("Optional transformable to use for the particle launch location")]
    public Symbol trans = Symbol.Empty;

    [Name("Num Parts"), Description("Number of particles to launch")]
    public int numParts;

    public uint mask;

    [Name("Life"), Description("Life of the particles at launch")]
    public float life;

    [Name("Speed"), Description("Speed of the particles at launch")]
    public float speed;

    [Name("Size"), Description("Size of the particles at launch")]
    public float size;

    [Name("Delta Size"), Description("Delta size of the particles at launch")]
    public float deltaSize;

    [Name("Start Color"), Description("Start color of the particles at launch")]
    public HmxColor4 startColor = new();

    [Name("Mid Color"), Description("Mid color of the particles at launch")]
    public HmxColor4 midColor = new();

    [Name("End Color"), Description("End color of the particles at launch")]
    public HmxColor4 endColor = new();

    [Name("Pitch")]
    public Vector2 pitch = new();

    [Name("Yaw")]
    public Vector2 yaw = new();

    [Description("Box extent bound 1 for the particle launch"), MinVersion(3)]
    public Vector3 boxMin = new();

    [Description("Box extent bound 1 for the particle launch"), MinVersion(3)]
    public Vector3 boxMax = new();

    [Name("Mesh Emitter"), Description("Mesh emitter override"), MinVersion(3)]
    public Symbol meshEmitter = Symbol.Empty;

    [Name("Emit Rate"), Description("Frame range to generate particles."), MinVersion(4)]
    public Vector2 emitRate = new();

    private Object oldFirstObject = new(); // rev < 2
    private Symbol oldMultiMesh = Symbol.Empty; // rev 0-1

    public new RndPartLauncher Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 2)
        {
            oldFirstObject = new Object();
            oldFirstObject.Read(reader, false, parent, entry);
            base.Read(reader, false, parent, entry);

            oldMultiMesh = Symbol.Read(reader);

            if (revision != 0)
            {
                part = Symbol.Read(reader);
                trans = Symbol.Read(reader);
                numParts = reader.ReadInt32();

                mask = SetMaskBit(mask, 1u, reader.ReadBoolean());
                life = reader.ReadFloat();

                mask = SetMaskBit(mask, 2u, reader.ReadBoolean());
                speed = reader.ReadFloat();

                mask = SetMaskBit(mask, 4u, reader.ReadBoolean());
                size = reader.ReadFloat();

                mask = SetMaskBit(mask, 8u, reader.ReadBoolean());
                deltaSize = reader.ReadFloat();

                mask = SetMaskBit(mask, 0x10u, reader.ReadBoolean());
                startColor = new HmxColor4().Read(reader);

                mask = SetMaskBit(mask, 0x20u, reader.ReadBoolean());
                midColor = new HmxColor4().Read(reader);

                mask = SetMaskBit(mask, 0x40u, reader.ReadBoolean());
                endColor = new HmxColor4().Read(reader);

                mask = SetMaskBit(mask, 0x80u, reader.ReadBoolean());
                pitch = new Vector2().Read(reader);
                yaw = new Vector2().Read(reader);
            }
        }
        else
        {
            base.Read(reader, false, parent, entry);

            part = Symbol.Read(reader);
            trans = Symbol.Read(reader);
            numParts = reader.ReadInt32();
            mask = reader.ReadUInt32();
            life = reader.ReadFloat();
            speed = reader.ReadFloat();
            size = reader.ReadFloat();
            deltaSize = reader.ReadFloat();
            startColor = new HmxColor4().Read(reader);
            midColor = new HmxColor4().Read(reader);
            endColor = new HmxColor4().Read(reader);
            pitch = new Vector2().Read(reader);
            yaw = new Vector2().Read(reader);

            if (revision > 2)
            {
                boxMin = new Vector3().Read(reader);
                boxMax = new Vector3().Read(reader);
                meshEmitter = Symbol.Read(reader);
            }

            if (revision > 3)
            {
                emitRate = new Vector2().Read(reader);
            }
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision < 2)
        {
            oldFirstObject.Write(writer, false, parent, entry);
            base.Write(writer, false, parent, entry);

            Symbol.Write(writer, oldMultiMesh);

            if (revision != 0)
            {
                Symbol.Write(writer, part);
                Symbol.Write(writer, trans);
                writer.WriteInt32(numParts);

                writer.WriteBoolean((mask & 1u) != 0);
                writer.WriteFloat(life);

                writer.WriteBoolean((mask & 2u) != 0);
                writer.WriteFloat(speed);

                writer.WriteBoolean((mask & 4u) != 0);
                writer.WriteFloat(size);

                writer.WriteBoolean((mask & 8u) != 0);
                writer.WriteFloat(deltaSize);

                writer.WriteBoolean((mask & 0x10u) != 0);
                startColor.Write(writer);

                writer.WriteBoolean((mask & 0x20u) != 0);
                midColor.Write(writer);

                writer.WriteBoolean((mask & 0x40u) != 0);
                endColor.Write(writer);

                writer.WriteBoolean((mask & 0x80u) != 0);
                pitch.Write(writer);
                yaw.Write(writer);
            }
        }
        else
        {
            base.Write(writer, false, parent, entry);

            Symbol.Write(writer, part);
            Symbol.Write(writer, trans);
            writer.WriteInt32(numParts);
            writer.WriteUInt32(mask);
            writer.WriteFloat(life);
            writer.WriteFloat(speed);
            writer.WriteFloat(size);
            writer.WriteFloat(deltaSize);
            startColor.Write(writer);
            midColor.Write(writer);
            endColor.Write(writer);
            pitch.Write(writer);
            yaw.Write(writer);

            if (revision > 2)
            {
                boxMin.Write(writer);
                boxMax.Write(writer);
                Symbol.Write(writer, meshEmitter);
            }

            if (revision > 3)
            {
                emitRate.Write(writer);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private static uint SetMaskBit(uint mask, uint bit, bool value)
    {
        if (value)
        {
            return mask | bit;
        }
        return mask & ~bit;
    }
}
