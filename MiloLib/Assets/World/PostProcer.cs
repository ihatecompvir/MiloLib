using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("PostProcer"), Description("Forces the PostProc")]
public class PostProcer : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();

    public new PostProcer Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
