using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("WorldFx"), Description("Drop animatables and drawables into a WorldFx object, TransParent it to something, then start/stop it. Cleans up particles post animation.")]
public class WorldFx : RndDir
{
    private ushort altRevision;
    private ushort revision;

    public WorldFx(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new WorldFx Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

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

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }

}
