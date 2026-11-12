using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("TrackPanelDir"), Description("panel dir that handles tracks & hud")]
public class TrackPanelDir : PanelDir
{
    private ushort altRevision;
    private ushort revision;

    private uint unkPreBase;

    public uint viewTimeEasy;
    public uint viewTimeExpert;
    public float netTrackAlpha;

    public List<Symbol> configurableObjects = new();

    public TrackPanelDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new TrackPanelDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        unkPreBase = reader.ReadUInt32();

        base.Read(reader, false, parent, entry);

        viewTimeEasy = reader.ReadUInt32();
        viewTimeExpert = reader.ReadUInt32();
        netTrackAlpha = reader.ReadFloat();

        configurableObjects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        writer.WriteUInt32(unkPreBase);

        base.Write(writer, false, parent, entry);

        writer.WriteUInt32(viewTimeEasy);
        writer.WriteUInt32(viewTimeExpert);
        writer.WriteFloat(netTrackAlpha);

        AssetHelpers.WriteList(writer, configurableObjects, obj => Symbol.Write(writer, obj));

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
