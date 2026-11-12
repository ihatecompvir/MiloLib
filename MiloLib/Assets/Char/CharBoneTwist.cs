using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharBoneTwist"), Description("Rotate a bone to point towards targets")]
public class CharBoneTwist : Object
{
    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();
    [Name("Bone"), Description("Bone to move")]
    public Symbol bone = Symbol.Empty;
    [Name("Targets"), Description("Targets to average to point bone at")]
    public List<Symbol> targets = new();

    public new CharBoneTwist Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        weightable = weightable.Read(reader, false, parent, entry);

        bone = Symbol.Read(reader);

        targets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

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

        weightable.Write(writer, false, parent, entry);

        Symbol.Write(writer, bone);

        AssetHelpers.WriteList(writer, targets, target => Symbol.Write(writer, target));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
