using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharPosConstraint"), Description("Forces the targets to be within a world space bounding box relative to source.")]
public class CharPosConstraint : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Source"), Description("Bone to be higher than")]
    public Symbol src = Symbol.Empty;
    [Name("Targets"), Description("Bones to constrain")]
    public List<Symbol> targets = new();

    [Name("Box"), Description("Bounding box, make min > max to ignore that dimension"), MinVersion(2)]
    public float[] box = new float[6];

    public new CharPosConstraint Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        targets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        src = Symbol.Read(reader);
        if (revision > 1)
        {
            box[0] = reader.ReadFloat();
            box[1] = reader.ReadFloat();
            box[2] = reader.ReadFloat();
            box[3] = reader.ReadFloat();
            box[4] = reader.ReadFloat();
            box[5] = reader.ReadFloat();
        }

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
        AssetHelpers.WriteList(writer, targets, (Symbol target) => Symbol.Write(writer, target));

        Symbol.Write(writer, src);
        if (revision > 1)
        {
            writer.WriteFloat(box[0]);
            writer.WriteFloat(box[1]);
            writer.WriteFloat(box[2]);
            writer.WriteFloat(box[3]);
            writer.WriteFloat(box[4]);
            writer.WriteFloat(box[5]);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
