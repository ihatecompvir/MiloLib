using MiloLib.Utils;
using Vector3 = MiloLib.Classes.Vector3;

namespace MiloLib.Assets.Ham;

public class MoveVariant
{
    public int revision;
    public Vector3 positionOffset;
    MoveParent moveParent;

    Symbol index = Symbol.Empty;

    public List<MoveCandidate> prevCandidates = new();
    public List<MoveCandidate> nextCandidates = new();

    public Symbol hamMoveName = Symbol.Empty;
    public Symbol hamMoveMiloname = Symbol.Empty;
    public Symbol? linkedTo;
    public Symbol? linkedFrom;
    public Symbol genre = Symbol.Empty;
    public Symbol era = Symbol.Empty;
    public Symbol songName = Symbol.Empty;
    public float avgBeatsPerSecond;

    uint flags;

    enum MoveVariantFlags
    {
        kScored = 2,
        kFinalPose = 8,
        kSuppressGuideGesture = 0x10,
        kOmitFromMinigame = 0x20,
        kUseful = 0x40,
        kSuppressPracticeOptions = 0x80
    };

    public override string ToString()
    {
        string str = "MoveVariant";
        str += $"\trev: {revision} position offset: {positionOffset}\n";
        str += $"\tindex: {index} hamMoveName: {hamMoveName} hamMoveMiloName: {hamMoveMiloname}\n";
        str += $"\tgenre: {genre} era: {era} songName: {songName} avgBeatsPerSec: {avgBeatsPerSecond} flags: {flags}\n";
        str += $"\tLinked to: {linkedTo}; Linked from: {linkedFrom}\n";
        str += $"\tPrev candidates ({prevCandidates.Count})\n";
        for (int i = 0; i < prevCandidates.Count; i++)
        {
            str += $"\t\t{prevCandidates[i]}\n";
        }
        str += $"\tNext candidates ({nextCandidates.Count})\n";
        for (int i = 0; i < nextCandidates.Count; i++)
        {
            str += $"\t\t{nextCandidates[i]}\n";
        }
        return str;
    }

    public MoveVariant Read(EndianReader reader, MoveParent parent, MoveGraph moveGraph)
    {
        revision = reader.ReadInt32();
        moveParent = parent;
        positionOffset = new Vector3().Read(reader);
        index = Symbol.Read(reader);
        hamMoveName = Symbol.Read(reader);
        hamMoveMiloname = Symbol.Read(reader);
        genre = Symbol.Read(reader);
        era = Symbol.Read(reader);
        songName = Symbol.Read(reader);
        avgBeatsPerSecond = reader.ReadFloat();
        flags = reader.ReadUInt32();

        bool hasLinksTo = reader.ReadBoolean();
        linkedTo = null;
        if (hasLinksTo)
        {
            linkedTo = Symbol.Read(reader);
        }

        bool hasLinksFrom = reader.ReadBoolean();
        linkedFrom = null;
        if (hasLinksFrom)
        {
            linkedFrom = Symbol.Read(reader);
        }

        prevCandidates = AssetHelpers.ReadList(reader, () => new MoveCandidate().Read(reader));

        nextCandidates = AssetHelpers.ReadList(reader, () => new MoveCandidate().Read(reader));

        moveGraph.moveVariants.Add(index.value, this);

        return this;
    }

    public void Write(EndianWriter writer)
    {
        writer.WriteInt32(revision);
        positionOffset.Write(writer);
        Symbol.Write(writer, index);
        Symbol.Write(writer, hamMoveName);
        Symbol.Write(writer, hamMoveMiloname);
        Symbol.Write(writer, genre);
        Symbol.Write(writer, era);
        Symbol.Write(writer, songName);
        writer.WriteFloat(avgBeatsPerSecond);
        writer.WriteUInt32(flags);

        writer.WriteBoolean(linkedTo != null);
        if (linkedTo != null)
        {
            Symbol.Write(writer, linkedTo);
        }

        writer.WriteBoolean(linkedFrom != null);
        if (linkedFrom != null)
        {
            Symbol.Write(writer, linkedFrom);
        }

        AssetHelpers.WriteList(writer, prevCandidates, candidate => candidate.Write(writer));

        AssetHelpers.WriteList(writer, nextCandidates, candidate => candidate.Write(writer));
    }
}
