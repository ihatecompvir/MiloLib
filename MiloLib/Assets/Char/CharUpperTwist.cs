using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharUpperTwist"), Description("Does all interpolation for the upperarm, assuming upperArm, upperTwist1 and 2 are under clavicle. Rotation about x is evenly distributed from clavicle->twist1->twist2->upperarm. Feeds the bones when executed.")]
public class CharUpperTwist : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Upper Arm"), Description("The upper arm bone")]
    public Symbol upperArm = Symbol.Empty;
    [Name("Twist 1"), Description("The upper arm twist1 bone")]
    public Symbol twist = Symbol.Empty;
    [Name("Twist 2"), Description("The upper arm twist2 bone")]
    public Symbol twist2 = Symbol.Empty;

    public new CharUpperTwist Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        twist2 = Symbol.Read(reader);
        upperArm = Symbol.Read(reader);
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
        Symbol.Write(writer, twist2);
        Symbol.Write(writer, upperArm);
        Symbol.Write(writer, twist);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
