using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("CreditList")]
public class RndCreditList : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();
    public RndTrans trans = new();

    [Name("Display Num")]
    public int displayNum;

    [Name("Element Spacing")]
    public float elementSpacing;

    public new RndCreditList Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        draw = draw.Read(reader, false, parent, entry);
        trans = trans.Read(reader, false, parent, entry);
        displayNum = reader.ReadInt32();
        elementSpacing = reader.ReadFloat();

        if (standalone)
        {
            uint expectedEndBytes = 0xDEADDEAD;
            if (reader.Endianness == Endian.BigEndian)
            {
                expectedEndBytes = 0xADDEADDE;
            }
            uint actualEndBytes = reader.ReadUInt32();
            if (actualEndBytes != expectedEndBytes)
            {
                throw MiloLib.Exceptions.MiloAssetReadException.EndBytesNotFound(parent, entry, reader.BaseStream.Position, revision);
            }
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        draw.Write(writer, false, parent, entry);
        trans.Write(writer, false, parent, entry);
        writer.WriteInt32(displayNum);
        writer.WriteFloat(elementSpacing);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
