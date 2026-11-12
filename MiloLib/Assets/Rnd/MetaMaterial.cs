using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("MetaMaterial"), Description("MetaMaterials serve as Material templates")]
public class MetaMaterial : RndMat
{
    private ushort revision;
    private ushort altRevision;
    public List<int> propertyEditActions = new();

    public new MetaMaterial Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        ReadBaseMaterial(reader, parent, entry);
        if (revision > 1)
        {
            propertyEditActions = AssetHelpers.ReadList(reader, reader.ReadInt32);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        WriteBaseMaterial(writer, parent, entry);
        if (revision > 1)
        {
            AssetHelpers.WriteList(writer, propertyEditActions, writer.WriteInt32);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
