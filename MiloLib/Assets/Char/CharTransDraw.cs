using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharTransDraw"), Description("Defers drawing translucent parts of characters until this object is drawn")]
public class CharTransDraw : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();
    [Name("Chars"), Description("The Characters whose translucent bits we will draw")]
    public List<Symbol> chars = new();
    [Name("Force Draw"), Description("True if the transparent pieces should be drawn even if the character is not showing")]
    public bool forceDraw;

    public new CharTransDraw Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        draw = draw.Read(reader, false, parent, entry);

        chars = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        if (altRevision > 0)
        {
            forceDraw = reader.ReadBoolean();
        }

        if (standalone)
        {
            uint expectedEndBytes = 0xDEADDEAD;
            if (reader.Endianness == Endian.BigEndian)
            {
                expectedEndBytes = 0xADDEADDE;
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

        draw.Write(writer, false, parent, null);

        AssetHelpers.WriteList(writer, chars, ch => Symbol.Write(writer, ch));

        if (altRevision > 0)
        {
            writer.WriteBoolean(forceDraw);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
