using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("FaceFxLipSyncServo"), Description("Controls lip-sync animation.")]
public class FaceFxLipSyncServo : Object
{
    public class RegisterMap
    {
        [Name("Trans"), Description("RndTransformable to get register from")]
        public Symbol trans = Symbol.Empty;

        [Name("Op"), Description("what to get from map_trans")]
        public int op;

        [Name("Register"), Description("name of FaceFx register")]
        public Symbol register = Symbol.Empty;

        public static RegisterMap Read(EndianReader reader)
        {
            RegisterMap map = new();
            map.trans = Symbol.Read(reader);
            map.op = reader.ReadInt32();
            map.register = Symbol.Read(reader);
            return map;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, trans);
            writer.WriteInt32(op);
            Symbol.Write(writer, register);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [MinVersion(5)]
    public CharWeightable weightable = new();

    [Name("Facegraph"), Description("Facegraph file"), MinVersion(2)]
    public Symbol faceGraph = Symbol.Empty;

    [Name("Clips"), Description("CharClipSet milo file to load up"), MinVersion(2)]
    public Symbol clips = Symbol.Empty;

    [Name("Registers"), Description("Mappings from RndTransformables to FaceFx Registers"), MinVersion(3)]
    public List<RegisterMap> registers = new();

    [Name("Enabled")]
    public bool enabled = true;

    public new FaceFxLipSyncServo Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        base.Read(reader, false, parent, entry);

        if (revision > 4)
        {
            weightable = weightable.Read(reader, false, parent, entry);
        }

        if (revision > 1)
        {
            faceGraph = Symbol.Read(reader);
            clips = Symbol.Read(reader);
        }

        if (revision > 2)
        {
            registers = AssetHelpers.ReadList(reader, () => RegisterMap.Read(reader));
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
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        base.Write(writer, false, parent, entry);

        if (revision > 4)
        {
            weightable.Write(writer, false, parent, entry);
        }

        if (revision > 1)
        {
            Symbol.Write(writer, faceGraph);
            Symbol.Write(writer, clips);
        }

        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, registers, (RegisterMap registerMap) => registerMap.Write(writer));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
