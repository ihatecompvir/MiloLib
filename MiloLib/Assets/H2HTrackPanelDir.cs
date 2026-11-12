using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("H2HTrackPanelDir"), Description("panel dir that handles tracks & hud in head-to-head mode")]
public class H2HTrackPanelDir : TrackPanelDirBase
{
    private ushort altRevision;
    private ushort revision;

    public bool unkBool;

    public H2HTrackPanelDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new H2HTrackPanelDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        base.Read(reader, false, parent, entry);

        if (revision < 1 && !entry.isProxy)
        {
            unkBool = reader.ReadBoolean();
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        base.Write(writer, false, parent, entry);

        if (revision < 1 && (entry == null || !entry.isProxy))
        {
            writer.WriteBoolean(unkBool);
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
