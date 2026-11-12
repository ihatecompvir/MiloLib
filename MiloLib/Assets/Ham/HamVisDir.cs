using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamVisDir"), Description("panel dir that handles the visualizer")]
public class HamVisDir : SkeletonDir
{
    private ushort revision;
    private ushort altRevision;
    public int oldValue2A;
    public int oldValue2B;
    public int oldValue3;
    public List<Symbol> oldDrawables = new();
    public Vector3 oldPosition = new();
    public Symbol oldCamera = Symbol.Empty;
    public int oldCameraValue;
    [Name("Milo Manual Frame"), Description("Allow Milo anim bar to drive the gesture propanim frame, not the player's skeleton.")]
    public bool miloManualFrame;
    [Name("Player1 Left"), Description("Animated from 0 - 100, depending on player one's hand height")]
    public Symbol player1Left = Symbol.Empty;
    [Name("Player1 Right"), Description("Animated from 0 - 100, depending on player one's hand height")]
    public Symbol player1Right = Symbol.Empty;
    [Name("Player2 Left"), Description("Animated from 0 - 100, depending on player two's hand height")]
    public Symbol player2Left = Symbol.Empty;
    [Name("Player2 Right"), Description("Animated from 0 - 100, depending on player two's hand height")]
    public Symbol player2Right = Symbol.Empty;
    public Symbol oldDraw1 = Symbol.Empty;
    public Symbol oldDraw2 = Symbol.Empty;
    public Symbol oldDraw3 = Symbol.Empty;
    public Symbol oldDraw4 = Symbol.Empty;
    public Symbol oldAnim10 = Symbol.Empty;
    public Symbol oldCamera11 = Symbol.Empty;
    public Symbol oldAnim12 = Symbol.Empty;

    public HamVisDir(ushort revision = 0, ushort altRevision = 0) : base(0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new HamVisDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 13);
        if (revision == 0)
        {
            ReadPanelDirBase(reader, parent, entry);
        }
        else
        {
            base.Read(reader, false, parent, entry);
        }
        if (revision == 2)
        {
            oldValue2A = reader.ReadInt32();
            oldValue2B = reader.ReadInt32();
        }
        if (revision >= 3 && revision < 6)
        {
            oldValue3 = reader.ReadInt32();
        }
        if (revision >= 4 && revision < 13)
        {
            oldDrawables = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            oldPosition = oldPosition.Read(reader);
        }
        if (revision >= 5 && revision < 13)
        {
            oldCamera = Symbol.Read(reader);
            oldCameraValue = reader.ReadInt32();
        }
        if (revision >= 7)
        {
            miloManualFrame = reader.ReadBoolean();
        }
        if (revision >= 8)
        {
            player1Left = Symbol.Read(reader);
            player1Right = Symbol.Read(reader);
            player2Left = Symbol.Read(reader);
            player2Right = Symbol.Read(reader);
        }
        if (revision >= 9 && revision < 13)
        {
            oldDraw1 = Symbol.Read(reader);
            oldDraw2 = Symbol.Read(reader);
            oldDraw3 = Symbol.Read(reader);
            oldDraw4 = Symbol.Read(reader);
        }
        if (revision >= 10 && revision < 13)
        {
            oldAnim10 = Symbol.Read(reader);
        }
        if (revision >= 11 && revision < 13)
        {
            oldCamera11 = Symbol.Read(reader);
        }
        if (revision == 12)
        {
            oldAnim12 = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 13);
        if (revision == 0)
        {
            WritePanelDirBase(writer, parent, entry);
        }
        else
        {
            base.Write(writer, false, parent, entry);
        }
        if (revision == 2)
        {
            writer.WriteInt32(oldValue2A);
            writer.WriteInt32(oldValue2B);
        }
        if (revision >= 3 && revision < 6)
        {
            writer.WriteInt32(oldValue3);
        }
        if (revision >= 4 && revision < 13)
        {
            AssetHelpers.WriteList(writer, oldDrawables, value => Symbol.Write(writer, value));
            oldPosition.Write(writer);
        }
        if (revision >= 5 && revision < 13)
        {
            Symbol.Write(writer, oldCamera);
            writer.WriteInt32(oldCameraValue);
        }
        if (revision >= 7)
        {
            writer.WriteBoolean(miloManualFrame);
        }
        if (revision >= 8)
        {
            Symbol.Write(writer, player1Left);
            Symbol.Write(writer, player1Right);
            Symbol.Write(writer, player2Left);
            Symbol.Write(writer, player2Right);
        }
        if (revision >= 9 && revision < 13)
        {
            Symbol.Write(writer, oldDraw1);
            Symbol.Write(writer, oldDraw2);
            Symbol.Write(writer, oldDraw3);
            Symbol.Write(writer, oldDraw4);
        }
        if (revision >= 10 && revision < 13)
        {
            Symbol.Write(writer, oldAnim10);
        }
        if (revision >= 11 && revision < 13)
        {
            Symbol.Write(writer, oldCamera11);
        }
        if (revision == 12)
        {
            Symbol.Write(writer, oldAnim12);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
