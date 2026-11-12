using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamList"), Description("List with Hammer-specific features")]
public class HamList : UIList
{
    private ushort revision;
    private ushort altRevision;
    public Symbol oldInAnim = Symbol.Empty;
    public Symbol oldOutAnim = Symbol.Empty;

    public new HamList Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 19);
        if (revision <= 17)
        {
            ReadWithExternalRev(reader, revision, false, parent, entry);
        }
        else
        {
            base.Read(reader, false, parent, entry);
        }
        if (revision == 18)
        {
            oldInAnim = Symbol.Read(reader);
            oldOutAnim = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 19);
        if (revision <= 17)
        {
            WriteWithoutRev(writer, false, parent, entry);
        }
        else
        {
            base.Write(writer, false, parent, entry);
        }
        if (revision == 18)
        {
            Symbol.Write(writer, oldInAnim);
            Symbol.Write(writer, oldOutAnim);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
