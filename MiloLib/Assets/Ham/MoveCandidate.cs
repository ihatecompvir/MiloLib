using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

public class MoveCandidate
{
    public int revision;
    public Symbol variantName = Symbol.Empty;
    public Symbol clipName = Symbol.Empty;
    public Symbol unk = Symbol.Empty;
    public uint adjacencyFlag;

    enum AdjacencyFlags
    {
        kOriginalAdjacent = 4,
        kEasyMedAdjacent = 8,
        kAutoScore = 0x10,
        kAnimHotOrNot = 0x20,
        kFake = 0
    };

    public override string ToString()
    {
        return $"MoveCandidate: variant name {variantName} adjacency flags {adjacencyFlag}";
    }
    public MoveCandidate Read(EndianReader reader)
    {
        revision = reader.ReadInt32();
        adjacencyFlag = reader.ReadUInt32();

        clipName = Symbol.Read(reader);
        variantName = Symbol.Read(reader);
        unk = Symbol.Read(reader);

        if (revision < 1)
        {
            // TODO: Some stuff with adjecency flags here
        }
        return this;
    }

    public void Write(EndianWriter writer)
    {
        writer.WriteInt32(revision);
        writer.WriteUInt32(adjacencyFlag);

        Symbol.Write(writer, clipName);
        Symbol.Write(writer, variantName);
        Symbol.Write(writer, unk);
    }
}
