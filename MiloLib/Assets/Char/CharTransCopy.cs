using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharTransCopy"), Description("Copies local xfm from one bone to another at poll time")]
public class CharTransCopy : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Src"), Description("Object to copy the local xfm from")]
    public Symbol src = Symbol.Empty;
    [Name("Dest"), Description("Object to copy the local xfm to")]
    public Symbol dest = Symbol.Empty;

    public new CharTransCopy Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        src = Symbol.Read(reader);
        dest = Symbol.Read(reader);

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

        Symbol.Write(writer, src);
        Symbol.Write(writer, dest);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
