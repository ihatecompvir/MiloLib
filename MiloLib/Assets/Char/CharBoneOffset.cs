using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharBoneOffset"), Description("Offsets a dest bone.")]
public class CharBoneOffset : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Dest"), Description("The bone to offset")]
    public Symbol dest = Symbol.Empty;
    [Name("Offset"), Description("the offset")]
    public Vector3 offset = new();

    public new CharBoneOffset Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        dest = Symbol.Read(reader);
        offset = offset.Read(reader);

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

        Symbol.Write(writer, dest);
        offset.Write(writer);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
