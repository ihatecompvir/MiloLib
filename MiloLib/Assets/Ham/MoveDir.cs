using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("MoveDir"), Description("Dir for HamMoves, contains debugging functionality")]
public class MoveDir : SkeletonDir
{
    private ushort revision;
    private ushort altRevision;
    public uint unkInt1;
    public uint unkInt2;
    public uint unkInt3;
    public uint unkInt4;
    public bool oldEnabled;
    public Symbol oldPath = Symbol.Empty;
    public Symbol oldClip = Symbol.Empty;
    public Symbol oldMove = Symbol.Empty;
    public Symbol oldCharacter = Symbol.Empty;
    [Name("Filters Enabled"), Description("Allow filter editing?")]
    public bool filtersEnabled;
    public Symbol oldDifficulty = Symbol.Empty;
    public Symbol oldMoveName = Symbol.Empty;
    public Symbol oldVariant = Symbol.Empty;
    public int oldMoveOverlay;
    [Name("Move Overlay"), Description("Show debugging overlay for the current HamMove")]
    public bool moveOverlayEnabled;
    public bool oldBool16;
    public bool oldBool23;
    [Name("Debug Node Types"), Description("Types of error nodes to show")]
    public int debugNodeTypes;
    public Symbol oldDebugNodeType = Symbol.Empty;
    public int oldDebugMode;
    public bool oldBool22;
    public int oldFilterMode;
    public bool oldFilterEnabled;
    public int oldFilterStart;
    public int oldFilterEnd;
    [Name("Import Clip Path"), Description("The pre-recorded .clp file to import")]
    public Symbol importClipPath = Symbol.Empty;
    [Name("Filter Version"), Description("Version of detection used")]
    public Symbol filterVersion = Symbol.Empty;
    public List<Symbol> oldMoves = new();
    public byte[] oldFilterFlags = new byte[6];
    public List<KeyValuePair<Symbol, int>> oldFilterCounts = new();
    public List<int> oldCounts = new();

    public MoveDir(ushort revision, ushort altRevision = 0) : base(0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new MoveDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 35);
        if (revision < 9)
        {
            ReadRndDirBase(reader, parent, entry);
        }
        else
        {
            base.Read(reader, false, parent, entry);
        }
        if (revision < 5)
        {
            oldEnabled = reader.ReadBoolean();
        }
        if (revision > 0 && revision < 2)
        {
            oldPath = Symbol.Read(reader);
        }
        if ((!entry.isProxy || revision < 8) && revision > 3 && revision < 9)
        {
            oldClip = Symbol.Read(reader);
        }
        if ((!entry.isProxy || revision < 8) && revision > 5 && revision <= 26)
        {
            oldMove = Symbol.Read(reader);
        }
        if ((!entry.isProxy || revision < 8) && revision > 26 && revision < 32)
        {
            oldMoves = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (entry.isProxy && revision > 10 && revision < 13)
        {
            oldCharacter = Symbol.Read(reader);
        }
        if (entry.isProxy && revision > 12)
        {
            filtersEnabled = reader.ReadBoolean();
        }
        if (entry.isProxy && revision > 14 && revision < 35)
        {
            oldDifficulty = Symbol.Read(reader);
        }
        if (entry.isProxy && revision > 13 && revision < 35)
        {
            oldMoveName = Symbol.Read(reader);
        }
        if (entry.isProxy && revision > 23 && revision < 35)
        {
            oldVariant = Symbol.Read(reader);
        }
        if (revision > 9 && revision < 18)
        {
            oldMoveOverlay = reader.ReadInt32();
        }
        if (revision > 6 && (revision <= 9 || revision >= 18))
        {
            moveOverlayEnabled = reader.ReadBoolean();
        }
        if (revision > 15 && revision < 31)
        {
            oldBool16 = reader.ReadBoolean();
        }
        if (revision > 22 && revision < 34)
        {
            oldBool23 = reader.ReadBoolean();
        }
        if (revision > 27)
        {
            debugNodeTypes = reader.ReadInt32();
        }
        if (revision > 20 && revision <= 27)
        {
            oldDebugNodeType = Symbol.Read(reader);
        }
        if (revision > 17 && revision <= 20)
        {
            oldDebugMode = reader.ReadInt32();
        }
        if (revision > 21 && revision < 29)
        {
            oldBool22 = reader.ReadBoolean();
        }
        if (revision > 25 && revision < 33)
        {
            oldFilterMode = reader.ReadInt32();
            oldFilterEnabled = reader.ReadBoolean();
            oldFilterStart = reader.ReadInt32();
            oldFilterEnd = reader.ReadInt32();
            oldFilterFlags = reader.ReadBlock(6);
        }
        if (revision > 19)
        {
            importClipPath = Symbol.Read(reader);
        }
        if (revision > 20 && revision < 25)
        {
            oldFilterCounts = AssetHelpers.ReadList(reader, () => new KeyValuePair<Symbol, int>(Symbol.Read(reader), reader.ReadInt32()));
        }
        if (revision > 11 && revision <= 20)
        {
            int count = 5;
            if (revision < 17)
            {
                count = 4;
            }
            oldCounts.Clear();
            for (int i = 0; i < count; i++)
            {
                oldCounts.Add(reader.ReadInt32());
            }
        }
        if (revision >= 30)
        {
            filterVersion = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 35);
        if (revision < 9)
        {
            WriteRndDirBase(writer, parent, entry);
        }
        else
        {
            base.Write(writer, false, parent, entry);
        }
        if (revision < 5)
        {
            writer.WriteBoolean(oldEnabled);
        }
        if (revision > 0 && revision < 2)
        {
            Symbol.Write(writer, oldPath);
        }
        if ((!entry.isProxy || revision < 8) && revision > 3 && revision < 9)
        {
            Symbol.Write(writer, oldClip);
        }
        if ((!entry.isProxy || revision < 8) && revision > 5 && revision <= 26)
        {
            Symbol.Write(writer, oldMove);
        }
        if ((!entry.isProxy || revision < 8) && revision > 26 && revision < 32)
        {
            AssetHelpers.WriteList(writer, oldMoves, value => Symbol.Write(writer, value));
        }
        if (entry.isProxy && revision > 10 && revision < 13)
        {
            Symbol.Write(writer, oldCharacter);
        }
        if (entry.isProxy && revision > 12)
        {
            writer.WriteBoolean(filtersEnabled);
        }
        if (entry.isProxy && revision > 14 && revision < 35)
        {
            Symbol.Write(writer, oldDifficulty);
        }
        if (entry.isProxy && revision > 13 && revision < 35)
        {
            Symbol.Write(writer, oldMoveName);
        }
        if (entry.isProxy && revision > 23 && revision < 35)
        {
            Symbol.Write(writer, oldVariant);
        }
        if (revision > 9 && revision < 18)
        {
            writer.WriteInt32(oldMoveOverlay);
        }
        if (revision > 6 && (revision <= 9 || revision >= 18))
        {
            writer.WriteBoolean(moveOverlayEnabled);
        }
        if (revision > 15 && revision < 31)
        {
            writer.WriteBoolean(oldBool16);
        }
        if (revision > 22 && revision < 34)
        {
            writer.WriteBoolean(oldBool23);
        }
        if (revision > 27)
        {
            writer.WriteInt32(debugNodeTypes);
        }
        if (revision > 20 && revision <= 27)
        {
            Symbol.Write(writer, oldDebugNodeType);
        }
        if (revision > 17 && revision <= 20)
        {
            writer.WriteInt32(oldDebugMode);
        }
        if (revision > 21 && revision < 29)
        {
            writer.WriteBoolean(oldBool22);
        }
        if (revision > 25 && revision < 33)
        {
            writer.WriteInt32(oldFilterMode);
            writer.WriteBoolean(oldFilterEnabled);
            writer.WriteInt32(oldFilterStart);
            writer.WriteInt32(oldFilterEnd);
            writer.WriteBlock(oldFilterFlags);
        }
        if (revision > 19)
        {
            Symbol.Write(writer, importClipPath);
        }
        if (revision > 20 && revision < 25)
        {
            AssetHelpers.WriteList(writer, oldFilterCounts, item =>
            {
                Symbol.Write(writer, item.Key);
                writer.WriteInt32(item.Value);
            });
        }
        if (revision > 11 && revision <= 20)
        {
            int count = 5;
            if (revision < 17)
            {
                count = 4;
            }
            for (int i = 0; i < count; i++)
            {
                writer.WriteInt32(oldCounts[i]);
            }
        }
        if (revision >= 30)
        {
            Symbol.Write(writer, filterVersion);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
