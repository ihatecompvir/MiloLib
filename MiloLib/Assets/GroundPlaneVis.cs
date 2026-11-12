using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("GroundPlaneVis")]
public class GroundPlaneVis : PanelDir
{
    private ushort revision;
    private ushort altRevision;
    public List<Symbol> neighborhoods = new();

    public GroundPlaneVis(ushort revision = 2, ushort altRevision = 0) : base(2)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    private void ValidateLayout(DirectoryMeta parent)
    {
        if (parent.revision != 26 || revision != 2)
        {
            throw new NotSupportedException($"Unsupported GroundPlaneVis layout: scene {parent.revision}, revision {revision}.");
        }
    }

    public new GroundPlaneVis Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        ValidateLayout(parent);
        base.Read(reader, false, parent, entry);
        neighborhoods = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateLayout(parent);
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, neighborhoods, item => Symbol.Write(writer, item));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
