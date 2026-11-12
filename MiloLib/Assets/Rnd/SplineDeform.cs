using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("SplineDeform"), Description("Applies deformation effects to a spline.")]
public class SplineDeform : Object
{
    private ushort revision = 0;
    private ushort altRevision;
    [Name("Spline"), Description("Spline to be deformed.")]
    public Symbol spline = Symbol.Empty;
    [Name("Nodes")]
    public List<Symbol> nodes = new();

    public new SplineDeform Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        spline = Symbol.Read(reader);
        nodes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, spline);
        AssetHelpers.WriteList(writer, nodes, node => Symbol.Write(writer, node));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

