using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("TrackPanelDir"), Description("panel dir that handles tracks & hud")]
public class TrackPanelDir : TrackPanelDirBase
{
    private ushort altRevision;
    private ushort revision;

    [MaxVersion(0)]
    public uint unkInt1;
    [MaxVersion(0)]
    public uint unkInt2;
    [MaxVersion(0)]
    public uint unkInt3;
    [MaxVersion(0)]
    public uint unkInt4;

    public TrackPanelDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new TrackPanelDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision < 1)
        {
            unkInt1 = reader.ReadUInt32();
            unkInt2 = reader.ReadUInt32();
            unkInt3 = reader.ReadUInt32();
            unkInt4 = reader.ReadUInt32();
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

        if (revision < 1)
        {
            writer.WriteUInt32(unkInt1);
            writer.WriteUInt32(unkInt2);
            writer.WriteUInt32(unkInt3);
            writer.WriteUInt32(unkInt4);
        }

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
