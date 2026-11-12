using MiloLib.Assets.Char;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamDriver"), Description("Class to play back clips, has a tree view of layers")]
public class HamDriver : Object
{
    private ushort revision;
    private ushort altRevision;
    public CharWeightable weightable = new();
    [Name("Bones"), Description("The CharBones object to add into.")]
    public Symbol bones = Symbol.Empty;

    public new HamDriver Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        weightable.Read(reader, false, parent, entry);
        bones = Symbol.Read(reader);
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
        weightable.Write(writer, false, parent, entry);
        Symbol.Write(writer, bones);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
