using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamSkeletonConverter"), Description("Class to convert from a camera to a character skeleton")]
public class HamSkeletonConverter : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Bones"), Description("The CharBones object to add into.")]
    public Symbol bones = Symbol.Empty;

    public new HamSkeletonConverter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        bones = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, bones);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
