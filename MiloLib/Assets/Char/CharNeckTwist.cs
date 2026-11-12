using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharNeckTwist"), Description("Does all interpolation for the neck.")]
public class CharNeckTwist : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Head"), Description("The head bone, must be descendent of neck")]
    public Symbol head = Symbol.Empty;
    [Name("Twist"), Description("The twist bone, neck must be parent of")]
    public Symbol twist = Symbol.Empty;

    public new CharNeckTwist Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        head = Symbol.Read(reader);
        twist = Symbol.Read(reader);

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

        Symbol.Write(writer, head);
        Symbol.Write(writer, twist);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
