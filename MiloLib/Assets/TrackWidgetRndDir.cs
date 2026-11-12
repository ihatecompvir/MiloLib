using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("TrackWidgetRndDir")]
public class TrackWidgetRndDir : TrackWidget
{
    private ushort revision;
    private ushort altRevision;
    [Name("RndDir")]
    public Symbol rndDir = Symbol.Empty;

    public TrackWidgetRndDir() : base(1)
    {
    }

    public new TrackWidgetRndDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        rndDir = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, rndDir);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
