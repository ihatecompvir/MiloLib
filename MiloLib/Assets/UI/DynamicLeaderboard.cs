using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("DynamicLeaderboard"), Description("The dynamic leaderboard for Blitz.")]
public class DynamicLeaderboard : RndDir
{
    private ushort revision;
    private ushort altRevision;
    [Name("Player Board"), Description("The player's board object.")]
    public Symbol playerBoard = Symbol.Empty;
    [Name("Competitor Boards"), Description("List of competitor board objects.")]
    public List<Symbol> competitorBoards = new();
    [Name("Competitor Animations"), Description("List of animation which position competitor boards.")]
    public List<Symbol> competitorAnims = new();
    [Name("Lower Arrow"), Description("Shown if there is a competitor below the visible range.")]
    public Symbol lowerArrow = Symbol.Empty;
    [Name("Higher Arrow"), Description("Shown if there is a competitor above the visible range.")]
    public Symbol higherArrow = Symbol.Empty;

    public DynamicLeaderboard(ushort revision = 3, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    private void ValidateRevision()
    {
        if (revision != 3)
        {
            throw new NotSupportedException($"Unsupported DynamicLeaderboard revision {revision}.");
        }
    }

    public new DynamicLeaderboard Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        ValidateRevision();
        base.Read(reader, false, parent, entry);
        playerBoard = Symbol.Read(reader);
        competitorBoards = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        competitorAnims = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        lowerArrow = Symbol.Read(reader);
        higherArrow = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateRevision();
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, playerBoard);
        AssetHelpers.WriteList(writer, competitorBoards, item => Symbol.Write(writer, item));
        AssetHelpers.WriteList(writer, competitorAnims, item => Symbol.Write(writer, item));
        Symbol.Write(writer, lowerArrow);
        Symbol.Write(writer, higherArrow);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

