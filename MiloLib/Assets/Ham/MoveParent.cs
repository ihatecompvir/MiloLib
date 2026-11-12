using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

public class MoveParent
{

    public enum Difficulty
    {
        kDifficultyEasy,
        kDifficultyMedium,
        kDifficultyExpert,
        kDifficultyBeginner,
    }

    public int revision;
    public Symbol name = Symbol.Empty;
    public Difficulty difficulty;

    public List<Symbol> genreFlags = new();

    public List<Symbol> eraFlags = new();

    bool unkc;

    public Symbol displayName = Symbol.Empty;

    public List<MoveVariant> moveVariants = new();

    public override string ToString()
    {
        string str = "MoveParent:\n";
        str += $"\trev {revision}, name {name}, difficulty {difficulty}";
        str += $"\n\tgenre flags ({genreFlags.Count}): ";
        for (int i = 0; i < genreFlags.Count; i++)
        {
            str += genreFlags[i] + " ";
        }
        str += $"\n\tera flags ({eraFlags.Count}): ";
        for (int i = 0; i < eraFlags.Count; i++)
        {
            str += eraFlags[i] + " ";
        }
        str += $"\n\tMoveVariants ({moveVariants.Count}):\n";
        for (int i = 0; i < moveVariants.Count; i++)
        {
            str += moveVariants[i];
        }
        return str;
    }

    public MoveParent Read(EndianReader reader, MoveGraph graph)
    {
        revision = reader.ReadInt32();
        name = Symbol.Read(reader);
        difficulty = (Difficulty)reader.ReadInt32();

        genreFlags = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        eraFlags = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        unkc = reader.ReadBoolean();
        displayName = Symbol.Read(reader);

        moveVariants = AssetHelpers.ReadList(reader, () => new MoveVariant().Read(reader, this, graph));

        return this;
    }

    public void Write(EndianWriter writer)
    {
        writer.WriteInt32(revision);
        Symbol.Write(writer, name);
        writer.WriteInt32((int)difficulty);

        AssetHelpers.WriteList(writer, genreFlags, (Symbol genreFlag) => Symbol.Write(writer, genreFlag));

        AssetHelpers.WriteList(writer, eraFlags, (Symbol eraFlag) => Symbol.Write(writer, eraFlag));

        writer.WriteBoolean(unkc);
        Symbol.Write(writer, displayName);

        AssetHelpers.WriteList(writer, moveVariants, (MoveVariant variant) => variant.Write(writer));
    }
}
