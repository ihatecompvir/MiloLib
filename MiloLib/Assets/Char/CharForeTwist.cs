using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharForeTwist"), Description("Does all interpolation for the forearm. Assumes: foretwist1 and forearm are under upperarm. foretwist2 is under foretwist1 and that hand is under forearm. on the left hand offset rotation is usually 90 on the left, and -90 on the right. Feeds the bones when executed.")]
public class CharForeTwist : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Hand"), Description("The hand bone")]
    public Symbol hand = Symbol.Empty;
    [Name("Twist 2"), Description("The twist2 bone")]
    public Symbol twist = Symbol.Empty;

    [Name("Offset"), Description("Usually 180 for right hand, 0 for left hand")]
    public float offset;
    [Name("Bias"), Description("Biases the angle before dividing by 3, typically 45 on the left hand, -45 on the right."), MinVersion(4)]
    public float bias;
    [MinVersion(2), MaxVersion(2)]
    public int unk;

    public new CharForeTwist Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        offset = reader.ReadFloat();
        hand = Symbol.Read(reader);
        twist = Symbol.Read(reader);
        if (revision == 2)
        {
            unk = reader.ReadInt32();
        }
        if (revision > 3)
        {
            bias = reader.ReadFloat();
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
        writer.WriteFloat(offset);
        Symbol.Write(writer, hand);
        Symbol.Write(writer, twist);
        if (revision == 2)
        {
            writer.WriteInt32(unk);
        }
        if (revision > 3)
        {
            writer.WriteFloat(bias);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
