using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("DeferDraw"), Description("Defers drawing a groups within a bunch of ObjectDirs until it's drawn")]
public class RndDeferDraw : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();

    [Name("Draw Name"), Description("The name of the drawable to draw from each object")]
    public Symbol drawName = Symbol.Empty;

    [Name("Objects"), Description("The objects to draw the groups from")]
    public List<Symbol> objects = new();

    public new RndDeferDraw Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        base.Read(reader, false, parent, entry);
        draw = new RndDrawable().Read(reader, false, parent, entry);
        drawName = Symbol.Read(reader);
        objects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, drawName);
        AssetHelpers.WriteList(writer, objects, (Symbol objectName) => Symbol.Write(writer, objectName));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
