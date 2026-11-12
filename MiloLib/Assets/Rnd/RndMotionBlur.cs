using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("MotionBlur"), Description("Contains a list of objects to apply object based motion blur")]
public class RndMotionBlur : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();

    [Name("Draw List"), Description("The list of objects to render motion blur")]
    public List<Symbol> draws = new();

    public new RndMotionBlur Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);

        draws = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

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
        draw.Write(writer, false, parent, null);

        AssetHelpers.WriteList(writer, draws, (Symbol draw) => Symbol.Write(writer, draw));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
