using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("GHTrackDir"), Description("Top-level GH2 track object")]
public class GHTrackDir : RndDir
{
    public enum PlayerSettings : uint
    {
        kPlayer1,
        kPlayer2,
        kPlayerNone,
        kPlayerShared,
    }
    private ushort altRevision;
    private ushort revision;

    [Name("Player"), Description("which player to track; use 'none' for single-player")]
    public PlayerSettings playerSettings;

    [Name("Sp Camera"), Description("camera to use in single-player")]
    public Symbol spCam = Symbol.Empty;

    [Name("Mp Cameras"), Description("cameras to use in multi-player")]
    public List<Symbol> mpCams = new();

    [Name("Gem Widgets"), Description("normal gem widgets; 5 slots + bonus + miss")]
    public List<Symbol> gemWidgets = new();

    [Name("Hopo Widgets"), Description("hopo gem widgets; 5 slots + bonus + miss")]
    public List<Symbol> hopoWidgets = new();

    [Name("Star Widgets"), Description("star gem widgets; 5 slots + miss")]
    public List<Symbol> starWidgets = new();

    [Name("Star Hopo Widgets"), Description("star hopo widgets; 5 slots + miss")]
    public List<Symbol> starHopoWidgets = new();

    private byte[] gh2OpaqueBytes = Array.Empty<byte>();

    public GHTrackDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new GHTrackDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (parent.revision <= 25 && standalone && revision == 3)
        {
            gh2OpaqueBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
            return this;
        }

        playerSettings = (PlayerSettings)reader.ReadUInt32();

        spCam = Symbol.Read(reader);

        mpCams = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        gemWidgets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        hopoWidgets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        starWidgets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        starHopoWidgets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

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

        if (parent.revision <= 25 && standalone && revision == 3)
        {
            writer.WriteBlock(gh2OpaqueBytes);
            writer.WriteEndBytes();
            return;
        }

        writer.WriteUInt32((uint)playerSettings);

        Symbol.Write(writer, spCam);

        AssetHelpers.WriteList(writer, mpCams, (Symbol mpCam) => Symbol.Write(writer, mpCam));

        AssetHelpers.WriteList(writer, gemWidgets, (Symbol gemWidget) => Symbol.Write(writer, gemWidget));

        AssetHelpers.WriteList(writer, hopoWidgets, (Symbol hopoWidget) => Symbol.Write(writer, hopoWidget));

        AssetHelpers.WriteList(writer, starWidgets, (Symbol starWidget) => Symbol.Write(writer, starWidget));

        AssetHelpers.WriteList(writer, starHopoWidgets, (Symbol starHopoWidget) => Symbol.Write(writer, starHopoWidget));

        base.Write(writer, false, parent, entry);

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
