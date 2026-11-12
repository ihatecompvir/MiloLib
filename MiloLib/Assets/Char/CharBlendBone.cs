using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharBlendBone"), Description("Set up a constraint between a series of target bones")]
public class CharBlendBone : Object
{
    public struct ConstraintSystem
    {
        [Name("Target"), Description("object to constrain")]
        public Symbol target;
        [Name("Weight"), Description("influence value, from 0 to 1")]
        public float weight;

        public void Read(EndianReader reader)
        {
            target = Symbol.Read(reader);
            weight = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, target ?? Symbol.Empty);
            writer.WriteFloat(weight);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Targets")]
    public List<ConstraintSystem> targets = new();
    [Name("Source One"), Description("Source object to drive constraint")]
    public Symbol src1 = Symbol.Empty;
    [Name("Source Two"), Description("Source object to drive constraint")]
    public Symbol src2 = Symbol.Empty;
    [Name("Trans X"), Description("Apply x translation?")]
    public bool transX;
    [Name("Trans Y"), Description("Apply x translation?")]
    public bool transY;
    [Name("Trans Z"), Description("Apply x translation?")]
    public bool transZ;
    [Name("Rotation"), Description("Apply rotation constraint?")]
    public bool rotation;

    [Name("Set Local"), Description("Sets the local xfm, not world xfm, needed for crowd"), MinVersion(4)]
    public bool setLocal;

    public new CharBlendBone Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        targets = AssetHelpers.ReadList(reader, () =>
        {
            ConstraintSystem cs = new();
            cs.Read(reader);
            return cs;
        });

        src1 = Symbol.Read(reader);
        src2 = Symbol.Read(reader);
        transX = reader.ReadBoolean();
        transY = reader.ReadBoolean();
        transZ = reader.ReadBoolean();
        rotation = reader.ReadBoolean();

        if (revision > 3)
        {
            setLocal = reader.ReadBoolean();
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

        AssetHelpers.WriteList(writer, targets, cs => cs.Write(writer));

        Symbol.Write(writer, src1);
        Symbol.Write(writer, src2);
        writer.WriteBoolean(transX);
        writer.WriteBoolean(transY);
        writer.WriteBoolean(transZ);
        writer.WriteBoolean(rotation);

        if (revision > 3)
        {
            writer.WriteBoolean(setLocal);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
