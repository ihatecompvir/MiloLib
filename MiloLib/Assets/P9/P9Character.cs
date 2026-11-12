using MiloLib.Assets.Char;
using MiloLib.Classes;
using MiloLib.Utils;
using System.Text;

namespace MiloLib.Assets.P9;

[Name("P9Character"), Description("Character with animation group and song tempo support")]
public class P9Character : Character
{
    private const int ProxyNameSize = 0x100;

    private ushort altRevision;
    private ushort revision;

    [HideInInspector, MaxVersion(1)]
    public bool oldPreRev2Bool;

    [HideInInspector, MaxVersion(4)]
    public uint oldPreRev5Int;

    [Name("Head Lookat Weight"), Description("What fraction to drive the head lookat")]
    public float headLookatWeight;

    [Name("Tempo"), Description("song tempo"), MinVersion(3)]
    public Symbol tempo = Symbol.Empty;

    [Name("Era"), Description("era of animation"), MinVersion(3)]
    public Symbol era = Symbol.Empty;

    [Name("Song"), Description("song for sync moves"), MinVersion(4)]
    public Symbol song = Symbol.Empty;

    [Name("Venue"), Description("venue for directed cuts"), MinVersion(4)]
    public Symbol venue = Symbol.Empty;

    [Name("Instrument Index"), Description("which instrument is shown"), MinVersion(6)]
    public int instrumentIndex;

    [Name("Waypoint"), Description("waypoint for the head to use"), MinVersion(7)]
    public Symbol waypoint = Symbol.Empty;

    [HideInInspector, MinVersion(7)]
    public string waypointProxyName = string.Empty;

    [Name("Mic IK"), Description("the mic ik"), MinVersion(8)]
    public Symbol micIk = Symbol.Empty;

    [HideInInspector, MinVersion(8)]
    public string micIkProxyName = string.Empty;

    public P9Character(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public override bool AllowsInlineProxy()
    {
        return false;
    }

    private static string ReadProxyName(EndianReader reader)
    {
        uint length = reader.ReadUInt32();
        if (length >= ProxyNameSize)
        {
            throw new InvalidDataException($"P9Character proxy reference exceeds {ProxyNameSize - 1} bytes.");
        }
        return reader.ReadUTF8((int)length);
    }

    private static void WriteProxyName(EndianWriter writer, string value)
    {
        if (value == null)
        {
            value = string.Empty;
        }
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        if (bytes.Length >= ProxyNameSize)
        {
            throw new InvalidDataException($"P9Character proxy reference exceeds {ProxyNameSize - 1} bytes.");
        }
        writer.WriteUInt32((uint)bytes.Length);
        writer.WriteBlock(bytes);
    }

    public new P9Character Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision < 2)
        {
            oldPreRev2Bool = reader.ReadBoolean();
        }

        if (revision < 5)
        {
            oldPreRev5Int = reader.ReadUInt32();
        }

        headLookatWeight = reader.ReadFloat();

        if (revision > 2)
        {
            tempo = Symbol.Read(reader);
            era = Symbol.Read(reader);
        }

        if (revision > 3)
        {
            song = Symbol.Read(reader);
            venue = Symbol.Read(reader);
        }

        if (revision > 5)
        {
            instrumentIndex = reader.ReadInt32();
        }

        bool isProxyLike = ShouldTreatAsProxyForCharacterBody(entry, standalone);

        if (revision > 6)
        {
            if (isProxyLike)
            {
                waypointProxyName = ReadProxyName(reader);
                waypoint = new Symbol(0, waypointProxyName);
            }
            else
            {
                waypoint = Symbol.Read(reader);
                waypointProxyName = waypoint.value;
            }
        }

        if (revision > 7)
        {
            if (isProxyLike)
            {
                micIkProxyName = ReadProxyName(reader);
                micIk = new Symbol(0, micIkProxyName);
            }
            else
            {
                micIk = Symbol.Read(reader);
                micIkProxyName = micIk.value;
            }
        }

        if (standalone)
        {
            uint expectedEndBytes;
            if (reader.Endianness == Endian.BigEndian)
            {
                expectedEndBytes = 0xADDEADDEu;
            }
            else
            {
                expectedEndBytes = 0xDEADDEADu;
            }
            if (expectedEndBytes != reader.ReadUInt32())
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

        if (revision < 2)
        {
            writer.WriteBoolean(oldPreRev2Bool);
        }

        if (revision < 5)
        {
            writer.WriteUInt32(oldPreRev5Int);
        }

        writer.WriteFloat(headLookatWeight);

        if (revision > 2)
        {
            Symbol.Write(writer, tempo);
            Symbol.Write(writer, era);
        }

        if (revision > 3)
        {
            Symbol.Write(writer, song);
            Symbol.Write(writer, venue);
        }

        if (revision > 5)
        {
            writer.WriteInt32(instrumentIndex);
        }

        bool useProxyStrings = ShouldTreatAsProxyForCharacterBody(entry, standalone);

        if (revision > 6)
        {
            if (useProxyStrings)
            {
                string waypointName;
                if (string.IsNullOrEmpty(waypointProxyName))
                {
                    waypointName = waypoint.value;
                }
                else
                {
                    waypointName = waypointProxyName;
                }
                WriteProxyName(writer, waypointName);
            }
            else
            {
                Symbol.Write(writer, waypoint);
            }
        }

        if (revision > 7)
        {
            if (useProxyStrings)
            {
                string micIkName;
                if (string.IsNullOrEmpty(micIkProxyName))
                {
                    micIkName = micIk.value;
                }
                else
                {
                    micIkName = micIkProxyName;
                }
                WriteProxyName(writer, micIkName);
            }
            else
            {
                Symbol.Write(writer, micIk);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }
}
