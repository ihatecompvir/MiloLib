using MiloLib.Assets.Char;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandIKEffector"), Description("Does IK on end effectors and props")]
public class BandIKEffector : Object
{
    public class Constraint
    {
        [Name("Target"), Description("The thing the [effector] will keep relative xfm to, or absolute xfm if weight <= 0.")]
        public Symbol target = Symbol.Empty;

        public Symbol finger = Symbol.Empty;

        [Name("Weight"), Description("Relative weight for this constraint, is same as radius in feet of full effect. If zero or negative [effector] will be constrained directly to the [target]")]
        public float weight;

        public Constraint Read(EndianReader reader, ushort parentRevision)
        {
            target = Symbol.Read(reader);
            finger = Symbol.Read(reader);
            if (parentRevision > 2)
            {
                weight = reader.ReadFloat();
            }

            return this;
        }

        public void Write(EndianWriter writer, ushort parentRevision)
        {
            Symbol.Write(writer, target);
            Symbol.Write(writer, finger);
            if (parentRevision > 2)
            {
                writer.WriteFloat(weight);
            }
        }
    }

    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();

    [Name("Effector"), Description("The character end effector or prop to be constrained.")]
    public Symbol effector = Symbol.Empty;

    [Name("More"), Description("More constraints to evaluate")]
    public Symbol more = Symbol.Empty;

    [Name("Elbow"), Description("If hand, pointer to elbow object"), MinVersion(2)]
    public Symbol elbow = Symbol.Empty;

    private int discardedInt;

    [Name("Constraints")]
    public List<Constraint> constraints = new();

    [Name("Ground"), Description("for pelvis/feet specifies a ground plane height"), MinVersion(4)]
    public Symbol ground = Symbol.Empty;

    public new BandIKEffector Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        weightable = weightable.Read(reader, false, parent, entry);

        effector = Symbol.Read(reader);
        more = Symbol.Read(reader);

        if (revision > 1)
        {
            elbow = Symbol.Read(reader);
        }

        if (revision < 1)
        {
            discardedInt = reader.ReadInt32();
        }

        constraints = AssetHelpers.ReadList(reader, () => new Constraint().Read(reader, revision));

        if (revision > 3)
        {
            ground = Symbol.Read(reader);
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
        weightable.Write(writer, false, parent, entry);

        Symbol.Write(writer, effector);
        Symbol.Write(writer, more);

        if (revision > 1)
        {
            Symbol.Write(writer, elbow);
        }

        if (revision < 1)
        {
            writer.WriteInt32(discardedInt);
        }

        AssetHelpers.WriteList(writer, constraints, constraint => constraint.Write(writer, revision));

        if (revision > 3)
        {
            Symbol.Write(writer, ground);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
