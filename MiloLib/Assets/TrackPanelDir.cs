using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("TrackPanelDirBase")]
public class TrackPanelDirBase : PanelDir
{
    private ushort altRevision;
    private ushort revision;

    public float viewTimeEasy;
    public float viewTimeExpert;
    public float netTrackAlpha;

    public List<Symbol> configurableObjects = new();

    public TrackPanelDirBase(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new TrackPanelDirBase Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (!entry.isProxy)
        {
            viewTimeEasy = reader.ReadFloat();
            viewTimeExpert = reader.ReadFloat();
            netTrackAlpha = reader.ReadFloat();

            configurableObjects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
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

        if (!entry.isProxy)
        {
            writer.WriteFloat(viewTimeEasy);
            writer.WriteFloat(viewTimeExpert);
            writer.WriteFloat(netTrackAlpha);

            AssetHelpers.WriteList(writer, configurableObjects, configurableObject => Symbol.Write(writer, configurableObject));
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
