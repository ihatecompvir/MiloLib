using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("ScoreboardDir"), Description("In-game scoreboard")]
public class ScoreboardDir : RndDir
{
    private ushort revision;
    private ushort altRevision;

    [Name("Max Stars"), MinVersion(1)]
    public int maxStars;
    public int unkInt;
    [Name("Test Score"), Description("Current score.")]
    public float testScore;
    [Name("Test Stars")]
    public float testStars;
    [Name("Test Opponent Score"), Description("BotB opponent's total score.")]
    public float testOpponentScore;
    public float unkFloat1;
    public float unkFloat2;
    public float unkFloat3;
    public float unkFloat4;
    public bool unkBool;
    [Name("Stars")]
    public List<Symbol> stars = new();
    [Name("Gold Stars"), MinVersion(1)]
    public List<Symbol> goldStars = new();
    [Name("Sound"), Description("Play a sound when the stars change.")]
    public bool sound;
    [Name("Config")]
    public Symbol config = Symbol.Empty;

    public ScoreboardDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new ScoreboardDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        if (revision > 1)
        {
            throw new NotSupportedException($"Unsupported ScoreboardDir revision {revision}.");
        }
        if (revision == 1)
        {
            maxStars = reader.ReadInt32();
        }
        unkInt = reader.ReadInt32();
        testScore = reader.ReadFloat();
        testStars = reader.ReadFloat();
        testOpponentScore = reader.ReadFloat();
        unkFloat1 = reader.ReadFloat();
        unkFloat2 = reader.ReadFloat();
        unkFloat3 = reader.ReadFloat();
        unkFloat4 = reader.ReadFloat();
        unkBool = reader.ReadBoolean();
        stars = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        goldStars.Clear();
        if (revision > 0)
        {
            goldStars = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        sound = reader.ReadBoolean();
        config = Symbol.Read(reader);
        base.Read(reader, false, parent, entry);
        if (standalone)
        {
            ReadAndVerifyEndBytes(reader, parent, entry);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        if (revision == 1)
        {
            writer.WriteInt32(maxStars);
        }
        writer.WriteInt32(unkInt);
        writer.WriteFloat(testScore);
        writer.WriteFloat(testStars);
        writer.WriteFloat(testOpponentScore);
        writer.WriteFloat(unkFloat1);
        writer.WriteFloat(unkFloat2);
        writer.WriteFloat(unkFloat3);
        writer.WriteFloat(unkFloat4);
        writer.WriteBoolean(unkBool);
        AssetHelpers.WriteList(writer, stars, (Symbol star) => Symbol.Write(writer, star));
        if (revision > 0)
        {
            AssetHelpers.WriteList(writer, goldStars, (Symbol star) => Symbol.Write(writer, star));
        }
        writer.WriteBoolean(sound);
        Symbol.Write(writer, config);
        base.Write(writer, false, parent, entry);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
