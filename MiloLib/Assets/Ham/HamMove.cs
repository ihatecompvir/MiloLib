using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamMove"), Description("Data associated with a ham Move")]
public class HamMove : RndPropAnim
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        { Game.MiloGame.DanceCentral, 28 },
        { Game.MiloGame.DanceCentral3, 50 }
    };
    public class LocalizedName
    {
        public Symbol locale = Symbol.Empty;
        public Symbol name = Symbol.Empty;

        public LocalizedName Read(EndianReader reader)
        {
            locale = Symbol.Read(reader);
            name = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, locale);
            Symbol.Write(writer, name);
        }

        public override string ToString()
        {
            return $"{locale} - {name}";
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Mirror"), Description("Move to mirror"), MinVersion(8)]
    public Symbol mirror = Symbol.Empty;

    [Name("Texture"), Description("Texture to describe the move")]
    public Symbol tex = Symbol.Empty;
    private Symbol unkPreRev5TexSymbol = Symbol.Empty;
    public Symbol oldTexture = Symbol.Empty;
    public Symbol oldObjectName = Symbol.Empty;
    public List<Symbol> oldObjects = new();
    public Symbol oldClip = Symbol.Empty;
    public Symbol oldClip2 = Symbol.Empty;
    public Symbol oldClip3 = Symbol.Empty;
    public List<Symbol> oldMoveSymbols = new();

    [Name("Scored"), Description("True if this is move is scored. False if it's a rest or some kind of indicator (like freestyle)"), MinVersion(2)]
    public bool scored;
    [Name("Final Pose"), Description("True if this move is the final pose in the song"), MinVersion(20)]
    public bool finalPose;

    [Name("Localized Names"), Description("This move's name for each language"), MinVersion(5)]
    public List<LocalizedName> languages = new();

    public enum TexState
    {
        kTexNone,
        kTexNormal,
        kTexFlip,
        kTexDblFlip
    }
    [Name("Texture State"), Description("Texture state describes how to display the tex"), MinVersion(12)]
    public TexState texState;

    public List<MoveFrame> frames = new();

    [Name("Paradiddle"), Description("True if this move is a paradiddle"), MinVersion(18)]
    public bool paradiddle;
    [Name("Supress Guide"), Description("Prevent the Guide Gesture from appearing for the duration of this move"), MinVersion(21)]
    public bool suppressGuide;
    [Name("Supress Practice Options"), Description("Prevent the Practice Options from appearing for the duration of this move"), MinVersion(50)]
    public bool suppressPracticeOptions;
    [Name("Omit from Minigame"), Description("Prevent this move from appear in the dance battle minigame"), MinVersion(34)]
    public bool omitMinigame;

    public List<float> ratingStates = new();

    [Name("Shoulder Displacements"), Description("Whether to use shoulder displacements for detection - specific to Ham1!"), MinVersion(27)]
    public bool shoulderDisplacements;

    [Name("Thresholds"), Description("Generated threshold for super perfect / perfect/flawless / awesome/nice / ok/almost"), MinVersion(36)]
    public float[] thresholds = new float[4]; // enum MoveRating
    [Name("Thresholds"), Description("Override threshold for super perfect / perfect/flawless / awesome/nice / ok/almost (0 means no override)"), MinVersion(45)]
    public float[] overrides = new float[4]; // enum MoveRating

    private uint numConfusabilities;
    public Dictionary<uint, float> confusabilities = new();

    public int unk94;

    public Symbol dancerSeq = Symbol.Empty;

    [Name("Confusability ID"), Description("id used when comparing to other moves"), MinVersion(49)]
    public uint confusabilityID;

    private string PrintMoveKeys()
    {
        string str = "RndPropAnim: ";
        str += $"revs ({revision}, {altRevision}) ";
        str += anim.ToString();
        for (int i = 0; i < propKeys.Count; i++)
        {
            str += propKeys[i].dtb.children[0].value.ToString() + $" keys ({propKeys[i].keys.Count}): \n";
            for (int j = 0; j < propKeys[i].keys.Count; j++)
            {
                str += "\t" + propKeys[i].keys[j].ToString() + "\n";
            }
        }
        // str += $"Loop: {mLoop}\n";
        // str += $"Intensity: {mIntensity}\n";
        return str;
    }

    public override string ToString()
    {
        string str = "HamMove: ";
        str += $"revs ({revision}, {altRevision}) ";
        // condensed dump of RndPropAnim specifically for HamMove
        str += PrintMoveKeys();

        if (revision > 7)
        {
            str += $"Move to mirror: {mirror}\n";
        }

        str += $"Texture: {tex}\n";
        if (revision > 1)
        {
            str += $"Scored: {scored}\n";
        }

        if (revision > 19)
        {
            str += $"Final pose: {finalPose}\n";
        }

        if (revision > 4)
        {
            str += $"Localized names ({languages.Count}): \n";
            for (int i = 0; i < languages.Count; i++)
            {
                str += $"\t {languages[i]}\n";
            }
        }
        if (revision > 11)
        {
            str += $"Tex state: {texState}\n";
        }

        if (revision > 12)
        {
            str += $"MoveFrames ({frames.Count}):\n";
            for (int i = 0; i < frames.Count; i++)
            {
                str += frames[i];
            }
        }

        if (revision > 17)
        {
            str += $"Paradiddle: {paradiddle}\n";
        }

        if (revision > 20)
        {
            str += $"Suppress Guide: {suppressGuide}\n";
        }

        if (revision > 49)
        {
            str += $"Suppress Practice Options: {suppressPracticeOptions}\n";
        }

        if (revision > 33)
        {
            str += $"Omit from Minigame: {omitMinigame}\n";
        }

        if (revision > 21)
        {
            str += $"Rating states ({ratingStates.Count}): ";
            for (int i = 0; i < ratingStates.Count; i++)
            {
                str += $" {ratingStates[i]} ";
            }
            str += "\n";
        }

        if (revision > 26)
        {
            str += $"Use shoulder displacements (DC1 only): {shoulderDisplacements}\n";
        }

        if (revision > 35)
        {
            str += $"Thresholds: \n";
            str += $"\tSuper perfect: {thresholds[0]}\n";
            str += $"\tPerfect/flawless: {thresholds[1]}\n";
            str += $"\tAwesome/nice: {thresholds[2]}\n";
            str += $"\tOk/almost: {thresholds[3]}\n";
        }

        if (revision > 44)
        {
            str += $"Overrides: \n";
            str += $"\tSuper perfect: {overrides[0]}\n";
            str += $"\tPerfect/flawless: {overrides[1]}\n";
            str += $"\tAwesome/nice: {overrides[2]}\n";
            str += $"\tOk/almost: {overrides[3]}\n";
        }

        if (revision > 42)
        {
            str += $"Confusabilities ({confusabilities.Count}): \n\t";
            foreach (var item in confusabilities)
            {
                str += $"({item.Key}, {item.Value}) ";
            }
            str += "\n";
        }

        if (revision > 46)
        {
            str += $"Unknown value at 0x94: {unk94}\n";
        }

        if (revision > 47)
        {
            str += $"Dancer sequence: {dancerSeq}\n";
        }

        if (revision > 48)
        {
            str += $"Confusability ID: {confusabilityID}\n";
        }

        return str;
    }

    public new HamMove Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 3)
        {
            base.Read(reader, false, parent, entry);
        }
        else
        {
            objFields.Read(reader, parent, entry);
        }

        if (revision > 7)
        {
            mirror = Symbol.Read(reader);
        }

        if (revision < 5)
        {
            unkPreRev5TexSymbol = Symbol.Read(reader);
        }

        tex = Symbol.Read(reader);

        if (revision > 1)
        {
            scored = reader.ReadBoolean();
        }

        if (revision > 19)
        {
            finalPose = reader.ReadBoolean();
        }

        if (revision > 2 && revision < 11)
        {
            oldTexture = Symbol.Read(reader);
        }
        if (revision < 16)
        {
            if (revision > 6)
            {
                oldObjects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            else if (revision > 5)
            {
                oldObjectName = Symbol.Read(reader);
            }
        }

        if (revision > 4)
        {
            languages = AssetHelpers.ReadList(reader, () => new LocalizedName().Read(reader));
        }
        if (revision < 39)
        {
            if (revision > 8)
            {
                oldClip = Symbol.Read(reader);
            }
            if (revision > 9)
            {
                oldClip2 = Symbol.Read(reader);
                oldClip3 = Symbol.Read(reader);
            }
        }

        if (revision > 11)
        {
            texState = (TexState)reader.ReadUInt32();
        }

        if (revision > 12)
        {
            frames = AssetHelpers.ReadList(reader, () => new MoveFrame().Read(reader, revision));
        }

        if (revision > 14 && revision < 42)
        {
            oldMoveSymbols = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision > 17)
        {
            paradiddle = reader.ReadBoolean();
        }

        if (revision > 20)
        {
            suppressGuide = reader.ReadBoolean();
        }

        if (revision > 49)
        {
            suppressPracticeOptions = reader.ReadBoolean();
        }

        if (revision > 33)
        {
            omitMinigame = reader.ReadBoolean();
        }

        if (revision > 21)
        {
            ratingStates = AssetHelpers.ReadList(reader, () => reader.ReadFloat());
        }

        // if gRev < 26, unused

        if (revision > 26)
        {
            shoulderDisplacements = reader.ReadBoolean();
        }

        if (revision > 35)
        {
            for (int i = 0; i < 4; i++)
            {
                thresholds[i] = reader.ReadFloat();
                if (revision > 44)
                {
                    overrides[i] = reader.ReadFloat();
                }
            }
        }

        if (revision > 42)
        {
            numConfusabilities = reader.ReadUInt32();
            for (int i = 0; i < numConfusabilities; i++)
            {
                confusabilities.Add(reader.ReadUInt32(), reader.ReadFloat());
            }
        }

        if (revision > 46)
        {
            unk94 = reader.ReadInt32();
        }

        if (revision > 47)
        {
            dancerSeq = Symbol.Read(reader);
        }

        if (revision > 48)
        {
            confusabilityID = reader.ReadUInt32();
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

        if (revision > 3)
        {
            base.Write(writer, false, parent, entry);
        }
        else
        {
            objFields.Write(writer, parent);
        }

        if (revision > 7)
        {
            Symbol.Write(writer, mirror);
        }

        if (revision < 5)
        {
            Symbol.Write(writer, unkPreRev5TexSymbol);
        }

        Symbol.Write(writer, tex);

        if (revision > 1)
        {
            writer.WriteBoolean(scored);
        }

        if (revision > 19)
        {
            writer.WriteBoolean(finalPose);
        }

        if (revision > 2 && revision < 11)
        {
            Symbol.Write(writer, oldTexture);
        }
        if (revision < 16)
        {
            if (revision > 6)
            {
                AssetHelpers.WriteList(writer, oldObjects, value => Symbol.Write(writer, value));
            }
            else if (revision > 5)
            {
                Symbol.Write(writer, oldObjectName);
            }
        }

        if (revision > 4)
        {
            AssetHelpers.WriteList(writer, languages, lang => lang.Write(writer));
        }

        if (revision < 39)
        {
            if (revision > 8)
            {
                Symbol.Write(writer, oldClip);
            }
            if (revision > 9)
            {
                Symbol.Write(writer, oldClip2);
                Symbol.Write(writer, oldClip3);
            }
        }

        if (revision > 11)
        {
            writer.WriteUInt32((uint)texState);
        }

        if (revision > 12)
        {
            AssetHelpers.WriteList(writer, frames, frame => frame.Write(writer, revision));
        }

        if (revision > 14 && revision < 42)
        {
            AssetHelpers.WriteList(writer, oldMoveSymbols, value => Symbol.Write(writer, value));
        }
        if (revision > 17)
        {
            writer.WriteBoolean(paradiddle);
        }

        if (revision > 20)
        {
            writer.WriteBoolean(suppressGuide);
        }

        if (revision > 49)
        {
            writer.WriteBoolean(suppressPracticeOptions);
        }

        if (revision > 33)
        {
            writer.WriteBoolean(omitMinigame);
        }

        if (revision > 21)
        {
            AssetHelpers.WriteList(writer, ratingStates, state => writer.WriteFloat(state));
        }

        if (revision > 26)
        {
            writer.WriteBoolean(shoulderDisplacements);
        }

        if (revision > 35)
        {
            for (int i = 0; i < 4; i++)
            {
                writer.WriteFloat(thresholds[i]);
                if (revision > 44)
                {
                    writer.WriteFloat(overrides[i]);
                }
            }
        }

        if (revision > 42)
        {
            writer.WriteUInt32((uint)confusabilities.Count);
            foreach (var confusability in confusabilities)
            {
                writer.WriteUInt32(confusability.Key);
                writer.WriteFloat(confusability.Value);
            }
        }

        if (revision > 46)
        {
            writer.WriteInt32(unk94);
        }

        if (revision > 47)
        {
            Symbol.Write(writer, dancerSeq);
        }

        if (revision > 48)
        {
            writer.WriteUInt32(confusabilityID);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
