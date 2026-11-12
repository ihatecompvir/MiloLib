using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("MiniLeaderboardDisplay"), Description("Mini Leaderboard Display")]
public class MiniLeaderboardDisplay : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Allow Solo Scores"), Description("whether or not solo scores should be displayed when appropriate"), MinVersion(1)]
    public bool allowSoloScores;

    [Name("Resource")]
    public Symbol resource = Symbol.Empty;

    public new MiniLeaderboardDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision >= 1)
        {
            allowSoloScores = reader.ReadBoolean();
        }

        base.Read(reader, false, parent, entry);

        // dc uses a separate rev 0 layout, so we gotta detect it
        if (revision == 0 && parent.revision >= 32)
        {
            resource = Symbol.Read(reader);
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

        if (revision >= 1)
        {
            writer.WriteBoolean(allowSoloScores);
        }

        base.Write(writer, false, parent, entry);

        if (revision == 0 && parent.revision >= 32)
        {
            Symbol.Write(writer, resource);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
