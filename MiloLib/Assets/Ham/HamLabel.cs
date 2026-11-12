using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamLabel"), Description("Label with Hammer-specific features")]
public class HamLabel : UILabel
{
    private ushort revision = 17;
    private ushort altRevision;
    public Symbol inAnim = Symbol.Empty;
    public Symbol outAnim = Symbol.Empty;

    public new HamLabel Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 17);
        if (revision != 17)
        {
            throw new NotSupportedException($"Unsupported HamLabel revision {revision}; expected revision 17.");
        }
        base.Read(reader, false, parent, entry);
        inAnim = Symbol.Read(reader);
        outAnim = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (revision != 17)
        {
            throw new NotSupportedException($"Unsupported HamLabel revision {revision}; expected revision 17.");
        }
        AssetHelpers.WriteRevision(writer, revision, altRevision, 17);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, inAnim);
        Symbol.Write(writer, outAnim);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
