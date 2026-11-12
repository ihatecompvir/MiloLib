using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("Screenshot"), Description("Object to draw a screensot in Milo, to help with positioning. The screenshot will not be drawn or even loaded in-game.")]
public class Screenshot : Object
{
    private ushort altRevision;
    private ushort revision;
    private RndDrawable draw = new();

    [Name("Texture Path"), Description("File containing the screenshot to display. This file will only be loaded in Milo, not in game.")]
    public Symbol texPath = Symbol.Empty;

    public new Screenshot Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        texPath = Symbol.Read(reader);

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
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, texPath);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
