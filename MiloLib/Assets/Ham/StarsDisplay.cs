using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("StarsDisplay"), Description("Stars Display")]
public class StarsDisplay : UIComponent
{
    private ushort revision;
    private ushort altRevision;
    [Name("Alpha")]
    public float alpha = 1;
    public Symbol resourceDir = Symbol.Empty;

    public new StarsDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        if (revision >= 2)
        {
            alpha = reader.ReadFloat();
        }
        base.Read(reader, false, parent, entry);
        if (revision >= 3)
        {
            resourceDir = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        if (revision >= 2)
        {
            writer.WriteFloat(alpha);
        }
        base.Write(writer, false, parent, entry);
        if (revision >= 3)
        {
            Symbol.Write(writer, resourceDir);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
