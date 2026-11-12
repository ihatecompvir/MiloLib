using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("TrackWidgetMultiMesh")]
public class TrackWidgetMultiMesh : TrackWidget
{
    private ushort revision = 1;
    private ushort altRevision;
    [Name("MultiMeshes")]
    public List<Symbol> multiMeshes = new();

    public TrackWidgetMultiMesh() : base(1)
    {
    }

    private void ValidateRevision()
    {
        if (revision != 1)
        {
            throw new NotSupportedException($"Unsupported TrackWidgetMultiMesh revision {revision}.");
        }
    }

    public new TrackWidgetMultiMesh Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        ValidateRevision();
        base.Read(reader, false, parent, entry);
        multiMeshes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateRevision();
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, multiMeshes, item => Symbol.Write(writer, item));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
