using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamWardrobe"), Description("Manager class that clothes characters and loads animations under different conditions")]
public class HamWardrobe : Object
{
    private ushort revision;
    private ushort altRevision;
    public Symbol crowdClipType = Symbol.Empty;

    public new HamWardrobe Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        if (revision >= 2)
        {
            crowdClipType = Symbol.Read(reader);
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
        if (revision >= 2)
        {
            Symbol.Write(writer, crowdClipType);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
