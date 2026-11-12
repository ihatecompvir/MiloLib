using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("Waypoint"), Description("A waypoint for character movement. Characters walk to these, start themselves out from these, etc.")]
public class Waypoint : Object
{
    private ushort altRevision;
    private ushort revision;
    private RndDrawable oldDraw = new();

    [Name("Transform")]
    public RndTrans trans = new();

    [Name("Flags"), Description("Flags for this waypoint, should be a bitfield per app")]
    public int flags;

    [Name("Connections"), Description("Waypoints we can walk to")]
    public List<Symbol> connections = new();

    [Name("Radius"), Description("Radius within we can stop from a walk, or be tethered to"), MinVersion(2)]
    public float radius = 12.0f;

    [Name("Y Radius"), Description("If positive, makes this shape a box with radius the x axis X half width, and y_radius the Y axis half width"), MinVersion(3)]
    public float yRadius;

    [Name("Angular Radius"), Description("Angular slop in degrees away from y axis"), MinVersion(3)]
    public float angularRadius;

    [Name("Strict Radius Delta"), Description("how much beyond radius you will never leave, it will forcibly pull you back, ignored if <= 0"), MinVersion(4)]
    public float strictRadiusDelta;

    [Name("Strict Angular Delta"), Description("degrees beyond ang radius you can never rotate past, if >= 0"), MinVersion(4)]
    public float strictAngularDelta;

    public new Waypoint Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        if (revision < 5)
        {
            oldDraw.Read(reader, false, parent, entry);
        }
        trans.Read(reader, false, parent, entry);
        flags = reader.ReadInt32();
        connections = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        if (revision > 1)
        {
            radius = reader.ReadFloat();
        }
        else
        {
            radius = 12.0f;
        }
        if (revision > 2)
        {
            yRadius = reader.ReadFloat();
            angularRadius = reader.ReadFloat();
        }
        if (revision > 3)
        {
            strictRadiusDelta = reader.ReadFloat();
            strictAngularDelta = reader.ReadFloat();
        }

        if (standalone)
        {
            uint expectedEndBytes;
            if (reader.Endianness == Endian.BigEndian)
            {
                expectedEndBytes = 0xADDEADDE;
            }
            else
            {
                expectedEndBytes = 0xDEADDEAD;
            }
            if (reader.ReadUInt32() != expectedEndBytes)
            {
                throw MiloLib.Exceptions.MiloAssetReadException.EndBytesNotFound(parent, entry, reader.BaseStream.Position, revision);
            }
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);
        base.Write(writer, false, parent, entry);
        if (revision < 5)
        {
            oldDraw.Write(writer, false, parent, entry);
        }
        trans.Write(writer, false, parent, entry);
        writer.WriteInt32(flags);
        AssetHelpers.WriteList(writer, connections, (Symbol connection) => Symbol.Write(writer, connection));
        if (revision > 1)
        {
            writer.WriteFloat(radius);
        }
        if (revision > 2)
        {
            writer.WriteFloat(yRadius);
            writer.WriteFloat(angularRadius);
        }
        if (revision > 3)
        {
            writer.WriteFloat(strictRadiusDelta);
            writer.WriteFloat(strictAngularDelta);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
