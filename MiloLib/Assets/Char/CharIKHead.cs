using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharIKHead"), Description("Puts a head bone into a position, doing IK on the spine to achieve it.")]
public class CharIKHead : Object
{
    private ushort revision = 3;
    private ushort altRevision;

    public CharWeightable weightable = new();
    [Name("Head"), Description("The head bone which will be moved to the target")]
    public Symbol head = Symbol.Empty;
    [Name("Spine"), Description("The first spine bone in the chain to be moved")]
    public Symbol spine = Symbol.Empty;
    [Name("Mouth"), Description("If non null, will be the thing that actually hits the target")]
    public Symbol mouth = Symbol.Empty;
    [Name("Target"), Description("The target to hit")]
    public Symbol target = Symbol.Empty;
    [Name("Target Radius"), Description("allowable head movement relative to the target"), MinVersion(2)]
    public float targetRadius = 0.75f;
    [Name("Head Mat"), Description("how much to blend the original world space head matrix in"), MinVersion(2)]
    public float headMat = 0.5f;
    [Name("Offset"), Description("Another bone to get the same world space delta"), MinVersion(3)]
    public Symbol offset = Symbol.Empty;
    [Name("Offset Scale"), Description("World space axis scaling to apply to delta before applying to offset bone."), MinVersion(3)]
    public Vector3 offsetScale = new(1, 1, 1);

    public new CharIKHead Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        weightable.Read(reader, false, parent, entry);
        head = Symbol.Read(reader);
        spine = Symbol.Read(reader);
        mouth = Symbol.Read(reader);
        target = Symbol.Read(reader);
        if (revision > 1)
        {
            targetRadius = reader.ReadFloat();
            headMat = reader.ReadFloat();
        }
        if (revision > 2)
        {
            offset = Symbol.Read(reader);
            offsetScale.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        weightable.Write(writer, false, parent, entry);
        Symbol.Write(writer, head);
        Symbol.Write(writer, spine);
        Symbol.Write(writer, mouth);
        Symbol.Write(writer, target);
        if (revision > 1)
        {
            writer.WriteFloat(targetRadius);
            writer.WriteFloat(headMat);
        }
        if (revision > 2)
        {
            Symbol.Write(writer, offset);
            offsetScale.Write(writer);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
