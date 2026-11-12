using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharHair"), Description("Hair physics, deals with strands of hair")]
public class CharHair : Object
{
    public class Point
    {
        public Vector3 pos = new();
        public Vector3 force = new();
        public Vector3 lastFriction = new();
        public Vector3 lastZ = new();
        [Name("Bone"), Description("hair bone we set the transform of")]
        public Symbol bone = Symbol.Empty;
        [Name("Length"), Description("the length of this strand bone")]
        public float length;
        [Name("Collides"), Description("things to collide against")]
        public List<Symbol> collides = new();
        [Name("Radius"), Description("collision radius")]
        public float radius;
        [Name("Outer Radius"), Description("if > radius, is the distance the hair bone should start aligning itself with the collision primitive, so that once touching it, it will be totally flattened against it.")]
        public float outerRadius;
        [Name("Side Length"), Description("if >= 0 the base length to the side modified by min_slack and max_slack")]
        public float sideLength;
        public Vector3 unk5c = new();

        public Symbol unkSym = Symbol.Empty;
        public Symbol unkSym2 = Symbol.Empty;

        public int unkInt1;
        public int unkInt2;
        public int unkInt3;

        public float addToRadius;

        public bool unkBool;


        public Point Read(EndianReader reader, uint revision)
        {
            pos.Read(reader);
            bone = Symbol.Read(reader);
            length = reader.ReadFloat();
            if (revision < 3)
            {
                unkInt3 = reader.ReadInt32();
                unkSym = Symbol.Read(reader);
            }
            else if (revision == 3)
            {
                unkInt1 = reader.ReadInt32();
            }

            radius = reader.ReadFloat();

            if (revision > 1)
            {
                outerRadius = reader.ReadFloat();
            }

            if (revision == 6 || revision == 7 || revision == 8)
            {
                addToRadius = reader.ReadFloat();
            }

            if (revision == 6)
            {
                unkSym2 = Symbol.Read(reader);
            }

            if (revision < 8)
            {
                if (revision > 5)
                {
                    unkInt2 = reader.ReadInt32();
                    unkInt1 = reader.ReadInt32();
                }
            }
            else
            {
                if (revision < 9)
                {
                    unkBool = reader.ReadBoolean();
                }

                sideLength = reader.ReadFloat();
            }
            if (revision > 9)
            {
                unk5c = unk5c.Read(reader);
            }

            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            pos.Write(writer);
            Symbol.Write(writer, bone);
            writer.WriteFloat(length);
            if (revision < 3)
            {
                writer.WriteInt32(unkInt3);
                Symbol.Write(writer, unkSym);
            }
            else if (revision == 3)
            {
                writer.WriteInt32(unkInt1);
            }

            writer.WriteFloat(radius);

            if (revision > 1)
            {
                writer.WriteFloat(outerRadius);
            }

            if (revision == 6 || revision == 7 || revision == 8)
            {
                writer.WriteFloat(addToRadius);
            }

            if (revision == 6)
            {
                Symbol.Write(writer, unkSym2);
            }

            if (revision < 8)
            {
                if (revision > 5)
                {
                    writer.WriteInt32(unkInt2);
                    writer.WriteInt32(unkInt1);
                }
            }
            else
            {
                if (revision < 9)
                {
                    writer.WriteBoolean(unkBool);
                }

                writer.WriteFloat(sideLength);
            }
            if (revision > 9)
            {
                unk5c.Write(writer);
            }
        }
    }

    public class Strand
    {
        [Flags]
        public enum HookupFlagOptions
        {
            None = 0,
            HairBack = 1,
            HairChest = 2,
            HairForehead = 4,
            HairHead = 8,
            HairLeftShoulder = 16,
            HairRightShoulder = 32,
            HairFace = 64,
            HairNeck = 128,
            HairLeftThigh = 256,
            HairRightThigh = 512,
            HairLeftHip = 1024,
            HairRightHip = 2048
        }

        private const int KnownHookupFlagsMask = (int)(
            HookupFlagOptions.HairBack |
            HookupFlagOptions.HairChest |
            HookupFlagOptions.HairForehead |
            HookupFlagOptions.HairHead |
            HookupFlagOptions.HairLeftShoulder |
            HookupFlagOptions.HairRightShoulder |
            HookupFlagOptions.HairFace |
            HookupFlagOptions.HairNeck |
            HookupFlagOptions.HairLeftThigh |
            HookupFlagOptions.HairRightThigh |
            HookupFlagOptions.HairLeftHip |
            HookupFlagOptions.HairRightHip
        );

        private void SetHookupFlags(int flags)
        {
            var options = (HookupFlagOptions)flags;

            hairBack = options.HasFlag(HookupFlagOptions.HairBack);
            hairChest = options.HasFlag(HookupFlagOptions.HairChest);
            hairForehead = options.HasFlag(HookupFlagOptions.HairForehead);
            hairHead = options.HasFlag(HookupFlagOptions.HairHead);
            hairLeftShoulder = options.HasFlag(HookupFlagOptions.HairLeftShoulder);
            hairRightShoulder = options.HasFlag(HookupFlagOptions.HairRightShoulder);
            hairFace = options.HasFlag(HookupFlagOptions.HairFace);
            hairNeck = options.HasFlag(HookupFlagOptions.HairNeck);
            hairLeftThigh = options.HasFlag(HookupFlagOptions.HairLeftThigh);
            hairRightThigh = options.HasFlag(HookupFlagOptions.HairRightThigh);
            hairLeftHip = options.HasFlag(HookupFlagOptions.HairLeftHip);
            hairRightHip = options.HasFlag(HookupFlagOptions.HairRightHip);
        }

        private int GetHookupFlags()
        {
            var options = HookupFlagOptions.None;

            if (hairBack)
            {
                options |= HookupFlagOptions.HairBack;
            }

            if (hairChest)
            {
                options |= HookupFlagOptions.HairChest;
            }

            if (hairForehead)
            {
                options |= HookupFlagOptions.HairForehead;
            }

            if (hairHead)
            {
                options |= HookupFlagOptions.HairHead;
            }

            if (hairLeftShoulder)
            {
                options |= HookupFlagOptions.HairLeftShoulder;
            }

            if (hairRightShoulder)
            {
                options |= HookupFlagOptions.HairRightShoulder;
            }

            if (hairFace)
            {
                options |= HookupFlagOptions.HairFace;
            }

            if (hairNeck)
            {
                options |= HookupFlagOptions.HairNeck;
            }

            if (hairLeftThigh)
            {
                options |= HookupFlagOptions.HairLeftThigh;
            }

            if (hairRightThigh)
            {
                options |= HookupFlagOptions.HairRightThigh;
            }

            if (hairLeftHip)
            {
                options |= HookupFlagOptions.HairLeftHip;
            }

            if (hairRightHip)
            {
                options |= HookupFlagOptions.HairRightHip;
            }

            return (int)options | (hookupFlags & ~KnownHookupFlagsMask);
        }

        [Name("Root"), Description("The root Trans for the hair strand")]
        public Symbol root = Symbol.Empty;
        [Name("Angle"), Description("Angle in degrees of starting flip")]
        public float angle;
        [Name("Points")]
        public List<Point> points = new();
        public Matrix3 baseMat = new();
        public Matrix3 rootMat = new();
        private int hookupFlags;

        public bool hairBack;
        public bool hairChest;
        public bool hairForehead;
        public bool hairHead;
        public bool hairLeftShoulder;
        public bool hairRightShoulder;
        public bool hairFace;
        public bool hairNeck;
        public bool hairLeftThigh;
        public bool hairRightThigh;
        public bool hairLeftHip;
        public bool hairRightHip;

        public Strand Read(EndianReader reader, uint revision)
        {
            root = Symbol.Read(reader);
            angle = reader.ReadFloat();
            points = AssetHelpers.ReadList(reader, () => new Point().Read(reader, revision));
            baseMat = baseMat.Read(reader);
            rootMat = rootMat.Read(reader);
            if (revision > 2)
            {
                hookupFlags = reader.ReadInt32();
                SetHookupFlags(hookupFlags);
            }

            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            Symbol.Write(writer, root);
            writer.WriteFloat(angle);
            AssetHelpers.WriteList(writer, points, point => point.Write(writer, revision));
            baseMat.Write(writer);
            rootMat.Write(writer);
            if (revision > 2)
            {
                hookupFlags = GetHookupFlags();
                writer.WriteInt32(hookupFlags);
            }
        }
    }
    private ushort altRevision;
    private ushort revision;

    [Name("Stiffness"), Description("stiffness of each strand")]
    public float stiffness;
    [Name("Torsion"), Description("rotational stiffness of each strand")]
    public float torsion;
    [Name("Inertia"), Description("Inertia of the hair, zero means none")]
    public float inertia;
    [Name("Gravity"), Description("Gravity of the hair, one is normal")]
    public float gravity;
    [Name("Weight"), Description("Gravity of the hair, one is normal")]
    public float weight;
    [Name("Friction"), Description("Hair friction against each other")]
    public float friction;
    [Name("Min Slack"), Description("If using sides, determines how far in it could go"), MinVersion(8)]
    public float minSlack;
    [Name("Max Slack"), Description("If using sides, determines how far out it could go"), MinVersion(8)]
    public float maxSlack;
    [Name("Strands")]
    public List<Strand> strands = new();
    [Name("Simulate"), Description("Simulate physics or not")]
    public bool simulate;

    [Name("Wind"), Description("wind object to use"), MinVersion(11)]
    public Symbol wind = Symbol.Empty;

    [MinVersion(12)]
    public float windStrength;
    [Name("Flat"), Description("How flat the surface is, for purposes of wind resistance, 1 means zero wind resistance perpendicular to z axis (the blue highlight segments)"), MinVersion(13)]
    public float flat;

    public new CharHair Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        stiffness = reader.ReadFloat();
        torsion = reader.ReadFloat();
        inertia = reader.ReadFloat();
        gravity = reader.ReadFloat();
        weight = reader.ReadFloat();
        friction = reader.ReadFloat();
        if (revision >= 8)
        {
            minSlack = reader.ReadFloat();
            maxSlack = reader.ReadFloat();
        }

        strands = AssetHelpers.ReadList(reader, () => new Strand().Read(reader, revision));

        simulate = reader.ReadBoolean();

        if (revision > 10)
        {
            wind = Symbol.Read(reader);
        }

        if (revision > 11)
        {
            windStrength = reader.ReadFloat();
        }
        if (revision > 12)
        {
            flat = reader.ReadFloat();
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

        writer.WriteFloat(stiffness);
        writer.WriteFloat(torsion);
        writer.WriteFloat(inertia);
        writer.WriteFloat(gravity);
        writer.WriteFloat(weight);
        writer.WriteFloat(friction);
        if (revision >= 8)
        {
            writer.WriteFloat(minSlack);
            writer.WriteFloat(maxSlack);
        }

        AssetHelpers.WriteList(writer, strands, strand => strand.Write(writer, revision));

        writer.WriteBoolean(simulate);

        if (revision > 10)
        {
            Symbol.Write(writer, wind);
        }

        if (revision > 11)
        {
            writer.WriteFloat(windStrength);
        }
        if (revision > 12)
        {
            writer.WriteFloat(flat);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}

