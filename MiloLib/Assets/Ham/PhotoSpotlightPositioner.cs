using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("PhotoSpotlightPositioner"), Description("Positions a group at the skeleton's feet")]
public class PhotoSpotlightPositioner : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Player")]
    public int player;
    [Name("Spotlight")]
    public Symbol spotlight = Symbol.Empty;
    [Name("Ref Image")]
    public Symbol refImage = Symbol.Empty;

    public new PhotoSpotlightPositioner Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        player = reader.ReadInt32();
        spotlight = Symbol.Read(reader);
        refImage = Symbol.Read(reader);
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
        writer.WriteInt32(player);
        Symbol.Write(writer, spotlight);
        Symbol.Write(writer, refImage);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
