using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamPhotoDisplay"), Description("Photo displayer for Hammer")]
public class HamPhotoDisplay : RndDir
{
    private ushort revision;
    private ushort altRevision;
    [Name("Mesh1")]
    public Symbol mesh1 = Symbol.Empty;
    [Name("Mesh2")]
    public Symbol mesh2 = Symbol.Empty;

    public HamPhotoDisplay(ushort revision = 0, ushort altRevision = 0) : base(0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new HamPhotoDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        if (!entry.isProxy || revision < 1)
        {
            mesh1 = Symbol.Read(reader);
            mesh2 = Symbol.Read(reader);
        }
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
        if (!entry.isProxy || revision < 1)
        {
            Symbol.Write(writer, mesh1);
            Symbol.Write(writer, mesh2);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
