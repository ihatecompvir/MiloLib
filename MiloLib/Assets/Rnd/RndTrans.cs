using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Trans"), Description("Base class for transformable objects. Trans objects have a 3D position, rotation, and scale.")]
public class RndTrans : Object
{
    public enum Constraint
    {
        kConstraintNone,
        kConstraintLocalRotate,
        kConstraintParentWorld,
        kConstraintLookAtTarget,
        kConstraintShadowTarget,
        kConstraintBillboardZ,
        kConstraintBillboardXZ,
        kConstraintBillboardXYZ,
        kConstraintFastBillboardXYZ,
    }

    private ushort altRevision;
    private ushort revision;
    private Vector3 oldOrigin = new();
    private bool oldRev2To4Bool;
    private Sphere oldSphere = new();

    [Name("Local Xfm"), Description("The local transform of this object.")]
    public Matrix localXfm = new();
    [Name("World Xfm"), Description("The world transform of this object.")]
    public Matrix worldXfm = new();

    [Name("Constraint"), Description("Trans constraint for the object."), MinVersion(7)]
    public Constraint constraint;

    [Name("Target"), Description("Target according to the constraint."), MinVersion(6)]
    public Symbol target = Symbol.Empty;

    [Name("Preserve Scale"), Description("Preserve scale if applying dynamic constraint."), MinVersion(7)]
    public bool preserveScale;

    [Name("Parent"), Description("Object this is linked to.")]
    public Symbol parentObj = Symbol.Empty;

    [MaxVersion(8)]
    public List<Symbol> transObjects = new();
    [MaxVersion(8)]
    public List<string> transObjectsNullTerminated = new();

    public new RndTrans Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (standalone)
        {
            objFields = objFields.Read(reader, parent, entry);
        }

        localXfm = localXfm.Read(reader);
        worldXfm = worldXfm.Read(reader);

        if (revision < 9)
        {
            if (parent.revision <= 6)
            {
                transObjectsNullTerminated = AssetHelpers.ReadList(reader, () => reader.ReadUTF8());
            }
            else
            {
                transObjects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
        }

        if (revision > 0)
        {
            constraint = (Constraint)reader.ReadUInt32();
        }

        if (revision != 0 && revision < 7)
        {
            oldOrigin = oldOrigin.Read(reader);
        }

        if (revision > 1 && revision < 5)
        {
            oldRev2To4Bool = reader.ReadBoolean();
        }

        if (revision > 5 && revision < 8)
        {
            oldSphere = oldSphere.Read(reader);
        }

        if (revision > 5)
        {
            target = Symbol.Read(reader);
        }

        if (revision > 6)
        {
            preserveScale = reader.ReadBoolean();
        }

        if (revision > 6)
        {
            parentObj = Symbol.Read(reader);
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public new void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry = null)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (standalone)
        {
            base.objFields.Write(writer, parent);
        }

        localXfm.Write(writer);
        worldXfm.Write(writer);

        if (revision < 9)
        {
            if (parent.revision <= 6)
            {
                if (transObjectsNullTerminated.Count == 0 && transObjects.Count > 0)
                {
                    foreach (var sym in transObjects)
                    {
                        transObjectsNullTerminated.Add(sym.value);
                    }
                }
                AssetHelpers.WriteList(writer, transObjectsNullTerminated, obj => writer.WriteUTF8(obj));
            }
            else
            {
                if (transObjects.Count == 0 && transObjectsNullTerminated.Count > 0)
                {
                    foreach (var str in transObjectsNullTerminated)
                    {
                        transObjects.Add(new Symbol((uint)str.Length, str));
                    }
                }
                AssetHelpers.WriteList(writer, transObjects, obj => Symbol.Write(writer, obj));
            }
        }

        if (revision > 0)
        {
            writer.WriteUInt32((uint)constraint);
        }

        if (revision != 0 && revision < 7)
        {
            oldOrigin.Write(writer);
        }

        if (revision > 1 && revision < 5)
        {
            writer.WriteBoolean(oldRev2To4Bool);
        }

        if (revision > 5 && revision < 8)
        {
            oldSphere.Write(writer);
        }

        if (revision > 5)
        {
            Symbol.Write(writer, target);
        }

        if (revision > 6)
        {
            writer.WriteBoolean(preserveScale);
        }

        if (revision > 6)
        {
            Symbol.Write(writer, parentObj);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public static RndTrans New(ushort revision, ushort altRevision)
    {
        RndTrans trans = new();
        trans.revision = revision;
        trans.altRevision = altRevision;

        trans.localXfm = new Matrix();
        trans.localXfm.m11 = 1.0f;
        trans.localXfm.m12 = 0.0f;
        trans.localXfm.m13 = 0.0f;
        trans.localXfm.m21 = 0.0f;
        trans.localXfm.m22 = 1.0f;
        trans.localXfm.m23 = 0.0f;
        trans.localXfm.m31 = 0.0f;
        trans.localXfm.m32 = 0.0f;
        trans.localXfm.m33 = 1.0f;
        trans.localXfm.m41 = 0.0f;
        trans.localXfm.m42 = 0.0f;
        trans.localXfm.m43 = 0.0f;

        trans.worldXfm = new Matrix();
        trans.worldXfm.m11 = 1.0f;
        trans.worldXfm.m12 = 0.0f;
        trans.worldXfm.m13 = 0.0f;
        trans.worldXfm.m21 = 0.0f;
        trans.worldXfm.m22 = 1.0f;
        trans.worldXfm.m23 = 0.0f;
        trans.worldXfm.m31 = 0.0f;
        trans.worldXfm.m32 = 0.0f;
        trans.worldXfm.m33 = 1.0f;
        trans.worldXfm.m41 = 0.0f;
        trans.worldXfm.m42 = 0.0f;
        trans.worldXfm.m43 = 0.0f;
        return trans;
    }
}
