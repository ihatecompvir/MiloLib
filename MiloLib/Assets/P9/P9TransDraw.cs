using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.P9;

[Name("P9TransDraw"), Description("Defers drawing translucent parts of characters until this object is drawn")]
public class P9TransDraw : Object
{
    private ushort revision = 1;
    private ushort altRevision;

    public RndDrawable draw = new();
    [Name("Chars"), Description("The Characters whose translucent bits we will draw")]
    public List<Symbol> chars = new();

    public new P9TransDraw Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        chars = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, chars, character => Symbol.Write(writer, character));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
