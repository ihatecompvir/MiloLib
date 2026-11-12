using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Wind"), Description("Object representing blowing wind, CharHair and Fur can point at them.")]
public class RndWind : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Prevailing"), Description("Prevailing wind in inches/sec, along each world space axis, adds to random component, 1 mph == 17 inches/sec")]
    public Vector3 prevailing = new();
    [Name("Random"), Description("Random wind speed in inches/sec, along each world axis, adds to prevailing wind, 1 mph == 17 inches/sec")]
    public Vector3 random = new();

    [Name("Time Loop"), Description("how long in seconds before the wind loops, 50 is a nice default")]
    public float timeLoop;
    [Name("Space Loop"), Description("how far in inches before the wind loops, 100 is a nice default")]
    public float spaceLoop;

    [Name("Wind Owner"), Description("Wind owner for the wind, properties shown are not for the owner, however, you must edit it directly"), MinVersion(2)]
    public Symbol windOwner = Symbol.Empty;

    [Name("Trans"), Description("If set, transforms wind by this transform."), MinVersion(3)]
    public Symbol transform = Symbol.Empty;
    [Name("About Z"), Description("If set, rotates wind about z axis of [trans], like a tornado"), MinVersion(3)]
    public bool aboutZ;
    [Name("Min Speed"), Description("Minimum total wind speed"), MinVersion(4)]
    public float minSpeed;
    [Name("Max Speed"), Description("Maximum total wind speed"), MinVersion(4)]
    public float maxSpeed;

    public new RndWind Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        prevailing = prevailing.Read(reader);
        random = random.Read(reader);

        timeLoop = reader.ReadFloat();
        spaceLoop = reader.ReadFloat();

        if (revision > 1)
        {
            windOwner = Symbol.Read(reader);
        }

        if (revision > 2)
        {
            transform = Symbol.Read(reader);
            aboutZ = reader.ReadBoolean();
        }
        if (revision > 3)
        {
            minSpeed = reader.ReadFloat();
            maxSpeed = reader.ReadFloat();
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

        base.Write(writer, false, parent, entry);

        prevailing.Write(writer);
        random.Write(writer);

        writer.WriteFloat(timeLoop);
        writer.WriteFloat(spaceLoop);

        if (revision > 1)
        {
            Symbol.Write(writer, windOwner);
        }

        if (revision > 2)
        {
            Symbol.Write(writer, transform);
            writer.WriteBoolean(aboutZ);
        }
        if (revision > 3)
        {
            writer.WriteFloat(minSpeed);
            writer.WriteFloat(maxSpeed);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
