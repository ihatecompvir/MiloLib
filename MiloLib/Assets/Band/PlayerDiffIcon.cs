using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("PlayerDiffIcon"), Description("Overloaded player/difficulty display")]
public class PlayerDiffIcon : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Num Players"), Description("Number of players")]
    public int numPlayers;
    [Name("Diff"), Description("Difficulty")]
    public int diff;

    public new PlayerDiffIcon Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        numPlayers = reader.ReadInt32();
        diff = reader.ReadInt32();

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

        writer.WriteInt32(numPlayers);
        writer.WriteInt32(diff);

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
