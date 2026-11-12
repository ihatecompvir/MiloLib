using MiloLib.Assets.Char;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamIKEffector"), Description("Does IK on end effectors and props")]
public class HamIKEffector : Object
{
    private ushort revision;
    private ushort altRevision;
    public CharWeightable weightable = new();
    [Name("Effector"), Description("The character end effector or prop to be constrained. When hilit, the big red circles are the \"natural\" positions, the blue circles are the desired \"destination,\" and the green circle is where it was \"IK'd\" to. Each \"target\" shows up as an axis display with the current weight shown next to it.")]
    public Symbol effector = Symbol.Empty;
    [Name("More"), Description("More constraints to evaluate")]
    public Symbol more = Symbol.Empty;
    [Name("Elbow"), Description("If hand, pointer to elbow object")]
    public Symbol elbow = Symbol.Empty;
    public int oldValue;
    [Name("Constraints")]
    public List<Constraint> constraints = new();
    [Name("Ground"), Description("for pelvis/feet specifies a ground plane height")]
    public Symbol ground = Symbol.Empty;
    [Name("Other"), Description("Can call others to do ik on the same bone before doing shoulder fixup")]
    public Symbol other = Symbol.Empty;
    [Name("Finger"), Description("The actual trans to IK to constraints. Effector is still the thing that gets moved, if NULL, same as effector")]
    public Symbol finger = Symbol.Empty;
    [Name("Skeleton"), Description("pointer to a HamIKSkeleton object")]
    public Symbol skeleton = Symbol.Empty;

    public class Constraint
    {
        [Name("Target"), Description("The thing the [effector] will keep relative xfm to, or absolute xfm if weight <= 0.")]
        public Symbol target = Symbol.Empty;
        public Symbol oldSymbol = Symbol.Empty;
        [Name("Weight"), Description("Relative weight for this constraint, is same as radius in feet of full effect. If zero or negative [effector] will be constrained directly to the [target]")]
        public float weight;

        public Constraint Read(EndianReader reader, ushort revision)
        {
            target = Symbol.Read(reader);
            if (revision < 6)
            {
                oldSymbol = Symbol.Read(reader);
            }
            if (revision > 2)
            {
                weight = reader.ReadFloat();
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, target);
            if (revision < 6)
            {
                Symbol.Write(writer, oldSymbol);
            }
            if (revision > 2)
            {
                writer.WriteFloat(weight);
            }
        }
    }

    public new HamIKEffector Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 7);
        base.Read(reader, false, parent, entry);
        weightable.Read(reader, false, parent, entry);
        effector = Symbol.Read(reader);
        more = Symbol.Read(reader);
        if (revision > 1)
        {
            elbow = Symbol.Read(reader);
        }
        if (revision < 1)
        {
            oldValue = reader.ReadInt32();
        }
        constraints = AssetHelpers.ReadList(reader, () => new Constraint().Read(reader, revision));
        if (revision > 3)
        {
            ground = Symbol.Read(reader);
        }
        if (revision > 4)
        {
            other = Symbol.Read(reader);
        }
        if (revision > 5)
        {
            finger = Symbol.Read(reader);
        }
        if (revision > 6)
        {
            skeleton = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 7);
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
            writer.WriteInt32(oldValue);
        }
        AssetHelpers.WriteList(writer, constraints, constraint => constraint.Write(writer, revision));
        if (revision > 3)
        {
            Symbol.Write(writer, ground);
        }
        if (revision > 4)
        {
            Symbol.Write(writer, other);
        }
        if (revision > 5)
        {
            Symbol.Write(writer, finger);
        }
        if (revision > 6)
        {
            Symbol.Write(writer, skeleton);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
