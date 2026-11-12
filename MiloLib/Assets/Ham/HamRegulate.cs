using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamRegulate"), Description("Class to do regulation on a HamCharacter. Has two modes of operation")]
public class HamRegulate : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Left Foot"), Description("left foot ik object, used for clamping regulation when one or more feet are planted")]
    public Symbol leftFoot = Symbol.Empty;
    [Name("Right Foot"), Description("right foot ik object, used for clamping regulation when one or more feet are planted")]
    public Symbol rightFoot = Symbol.Empty;

    public new HamRegulate Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        if (revision > 1)
        {
            leftFoot = Symbol.Read(reader);
            rightFoot = Symbol.Read(reader);
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
        base.Write(writer, false, parent, entry);
        if (revision > 1)
        {
            Symbol.Write(writer, leftFoot);
            Symbol.Write(writer, rightFoot);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
