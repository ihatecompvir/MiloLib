using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("PracticeSection"), Description("Practice section details for skills mode")]
public class PracticeSection : Object
{

    private ushort altRevision;
    private ushort revision;

    public RndAnimatable superAnimatable = new();

    [Name("Display Name"), Description("Display name used to show in selection screens")]
    public Symbol displayName = Symbol.Empty;

    [Name("Difficulty"), Description("Difficulty this section is tied to")]
    public HamDifficulty difficulty;

    public class PracticeStep
    {
        [Name("Step Type"), Description("Type of step. Options are: learn, review, freestyle")]
        Symbol type = Symbol.Empty;
        [Name("Start"), Description("Start of sequence")]
        Symbol start = Symbol.Empty;
        [Name("End"), Description("End of sequence")]
        Symbol end = Symbol.Empty;
        // min version 1
        [Name("Boundary"), Description("True if this step is the START of a subsection"), MinVersion(1)]
        bool boundary;
        [Name("Name Override"), Description("Name to display on the PracticeChoosePanel, if left blank it tries to automatically pick one"), MinVersion(3)]
        Symbol nameOverride = Symbol.Empty; // String in milo's code

        public PracticeStep Read(EndianReader reader, uint stepRev)
        {
            type = Symbol.Read(reader);
            start = Symbol.Read(reader);
            end = Symbol.Read(reader);
            if (stepRev > 0)
            {
                boundary = reader.ReadBoolean();
            }

            if (stepRev > 2)
            {
                nameOverride = Symbol.Read(reader);
            }

            return this;
        }

        public void Write(EndianWriter writer, uint stepRev)
        {
            Symbol.Write(writer, type);
            Symbol.Write(writer, start);
            Symbol.Write(writer, end);
            if (stepRev > 0)
            {
                writer.WriteBoolean(boundary);
            }

            if (stepRev > 2)
            {
                Symbol.Write(writer, nameOverride);
            }
        }
    };

    [Name("Practice Steps"), Description("List of steps for this practice section")]
    public List<PracticeStep> steps = new();

    public List<DancerSequence> seqs = new();

    [Name("Test Step/Sequence Index"), Description("Index of step/sequence to test"), MinVersion(2)]
    public int testStepSequence;

    public new PracticeSection Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        superAnimatable.Read(reader, parent, entry);

        displayName = Symbol.Read(reader);
        difficulty = (HamDifficulty)reader.ReadUInt32();

        steps = AssetHelpers.ReadList(reader, () => new PracticeStep().Read(reader, revision));

        if (revision > 1)
        {
            seqs = AssetHelpers.ReadList(reader, () => new DancerSequence().Read(reader, false, parent, entry));
            testStepSequence = reader.ReadInt32();
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

        base.Write(writer, false, parent, entry);
        superAnimatable.Write(writer);

        Symbol.Write(writer, displayName);
        writer.WriteUInt32((uint)difficulty);

        AssetHelpers.WriteList(writer, steps, step => step.Write(writer, revision));

        if (revision > 1)
        {
            AssetHelpers.WriteList(writer, seqs, seq => seq.Write(writer, false, parent, entry));
            writer.WriteInt32(testStepSequence);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
