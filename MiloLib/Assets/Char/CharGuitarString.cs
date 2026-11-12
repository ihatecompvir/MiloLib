using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharGuitarString"), Description("moves a bone based on the position of the hand, nut, and bridge")]
public class CharGuitarString : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Nut"), Description("nut object")]
    public Symbol nut = Symbol.Empty;
    [Name("Bridge"), Description("bridge object")]
    public Symbol bridge = Symbol.Empty;
    [Name("Bend"), Description("object to move between nut and bridge")]
    public Symbol bend = Symbol.Empty;
    [Name("Target"), Description("object to follow")]
    public Symbol target = Symbol.Empty;

    public new CharGuitarString Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        nut = Symbol.Read(reader);
        bridge = Symbol.Read(reader);
        bend = Symbol.Read(reader);
        target = Symbol.Read(reader);

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

        Symbol.Write(writer, nut);
        Symbol.Write(writer, bridge);
        Symbol.Write(writer, bend);
        Symbol.Write(writer, target);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
