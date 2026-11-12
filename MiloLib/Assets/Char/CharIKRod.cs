using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharIKRod"), Description("Rigs a bone between two other bones and sets the orientation from that. When you set up all the bone pointers, the rig xfm will be computed, an inverse from that to the dst bone will be computed, and everything will come from that. So the dst bone will maintain the exact same position in that pose. That makes it easy to author the bones.")]
public class CharIKRod : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Left End"), Description("Left end of the rod")]
    public Symbol leftEnd = Symbol.Empty;
    [Name("Right End"), Description("Right end of the rod")]
    public Symbol rightEnd = Symbol.Empty;
    [Name("Dest Pos"), Description("Fraction of the way dest is from left (0) to right(1)")]
    public float destPos;
    [Name("Side Axis"), Description("Take the z axis from this bone rather than from rod end delta")]
    public Symbol sideAxis = Symbol.Empty;
    [Name("Vertical"), Description("Force the dest to be vertically upright")]
    public bool vertical;
    [Name("Dest"), Description("The bone to set")]
    public Symbol dest = Symbol.Empty;
    public Matrix xfm = new();


    public new CharIKRod Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        leftEnd = Symbol.Read(reader);
        rightEnd = Symbol.Read(reader);
        destPos = reader.ReadFloat();
        sideAxis = Symbol.Read(reader);
        vertical = reader.ReadBoolean();
        dest = Symbol.Read(reader);
        xfm.Read(reader);

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

        Symbol.Write(writer, leftEnd);
        Symbol.Write(writer, rightEnd);
        writer.WriteFloat(destPos);
        Symbol.Write(writer, sideAxis);
        writer.WriteBoolean(vertical);
        Symbol.Write(writer, dest);
        xfm.Write(writer);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
