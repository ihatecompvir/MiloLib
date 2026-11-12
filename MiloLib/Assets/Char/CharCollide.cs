using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

// this description will always sound very metal
[Name("CharCollide"), Description("Feeds the bones when executed.")]
public class CharCollide : Object
{
    public enum Shape
    {
        kPlane = 0,
        kSphere = 1,
        kInsideSphere = 2,
        kCigar = 3,
        kInsideCigar = 4,
    };

    [Flags]
    public enum CollideFlagOptions
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

    public struct CharCollideStruct
    {
        public int unk0;
        public Vector3 vec;

        public void Read(EndianReader reader)
        {
            unk0 = reader.ReadInt32();
            vec = new Vector3().Read(reader);
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32(unk0);
            vec.Write(writer);
        }
    };

    private ushort altRevision;
    private ushort revision;

    private const int KnownFlagsMask = (int)(
        CollideFlagOptions.HairBack |
        CollideFlagOptions.HairChest |
        CollideFlagOptions.HairForehead |
        CollideFlagOptions.HairHead |
        CollideFlagOptions.HairLeftShoulder |
        CollideFlagOptions.HairRightShoulder |
        CollideFlagOptions.HairFace |
        CollideFlagOptions.HairNeck |
        CollideFlagOptions.HairLeftThigh |
        CollideFlagOptions.HairRightThigh |
        CollideFlagOptions.HairLeftHip |
        CollideFlagOptions.HairRightHip
    );

    private void SetFromFlags(int flags)
    {
        var options = (CollideFlagOptions)flags;

        isBack = options.HasFlag(CollideFlagOptions.HairBack);
        isChest = options.HasFlag(CollideFlagOptions.HairChest);
        isForehead = options.HasFlag(CollideFlagOptions.HairForehead);
        isHead = options.HasFlag(CollideFlagOptions.HairHead);
        isLeftShoulder = options.HasFlag(CollideFlagOptions.HairLeftShoulder);
        isRightShoulder = options.HasFlag(CollideFlagOptions.HairRightShoulder);
        isFace = options.HasFlag(CollideFlagOptions.HairFace);
        isNeck = options.HasFlag(CollideFlagOptions.HairNeck);
        isLeftThigh = options.HasFlag(CollideFlagOptions.HairLeftThigh);
        isRightThigh = options.HasFlag(CollideFlagOptions.HairRightThigh);
        isLeftHip = options.HasFlag(CollideFlagOptions.HairLeftHip);
        isRightHip = options.HasFlag(CollideFlagOptions.HairRightHip);
    }

    private int GetFlags()
    {
        var options = CollideFlagOptions.None;

        if (isBack)
        {
            options |= CollideFlagOptions.HairBack;
        }

        if (isChest)
        {
            options |= CollideFlagOptions.HairChest;
        }

        if (isForehead)
        {
            options |= CollideFlagOptions.HairForehead;
        }

        if (isHead)
        {
            options |= CollideFlagOptions.HairHead;
        }

        if (isLeftShoulder)
        {
            options |= CollideFlagOptions.HairLeftShoulder;
        }

        if (isRightShoulder)
        {
            options |= CollideFlagOptions.HairRightShoulder;
        }

        if (isFace)
        {
            options |= CollideFlagOptions.HairFace;
        }

        if (isNeck)
        {
            options |= CollideFlagOptions.HairNeck;
        }

        if (isLeftThigh)
        {
            options |= CollideFlagOptions.HairLeftThigh;
        }

        if (isRightThigh)
        {
            options |= CollideFlagOptions.HairRightThigh;
        }

        if (isLeftHip)
        {
            options |= CollideFlagOptions.HairLeftHip;
        }

        if (isRightHip)
        {
            options |= CollideFlagOptions.HairRightHip;
        }

        return (int)options | (flags & ~KnownFlagsMask);
    }

    public RndTrans trans = new();

    [Name("Shape"), Description("Type of collision")]
    public Shape shape;

    [Name("Radius0"), Description("Radius of the sphere, or of length0 hemisphere if cigar")]
    public float[] origRadius = new float[2] { 0, 0 };
    [Name("Length0"), Description("cigar: placement of radius0 hemisphere along X axis, must be < than length0, not used for sphere shapes"), MinVersion(3)]
    public float[] origLength = new float[2] { 0, 0 };
    [Name("Radius1"), Description("cigar: Radius of length1 hemisphere"), MinVersion(4)]
    public float[] curRadius = new float[2] { 0, 0 };
    [Name("Length1"), Description("cigar: placement of radius1 hemisphere along X axis, must be >= length0"), MinVersion(6)]
    public float[] curLength = new float[2] { 0, 0 };

    [MinVersion(2)]
    public int flags;

    public bool isBack;
    public bool isChest;
    public bool isForehead;
    public bool isHead;
    public bool isLeftShoulder;
    public bool isRightShoulder;
    public bool isFace;
    public bool isNeck;
    public bool isLeftThigh;
    public bool isRightThigh;
    public bool isLeftHip;
    public bool isRightHip;

    [MinVersion(6)]
    public Matrix unkTransform = new();

    [Name("Mesh"), Description("Optional mesh that will deform, used to resize ourselves. If this is set, make sure you are not parented to any bone with scale, such as an exo bone"), MinVersion(6)]
    public Symbol mesh = Symbol.Empty;

    [MinVersion(6)]
    public List<CharCollideStruct> structs = new(8);

    [MinVersion(6)]
    public byte[] sha1Digest = new byte[20];

    [Name("Mesh Y Bias"), Description("For spheres + cigars, finds mesh points along positive y axis (the green one), makes a better fit for spheres where only one side should be the fit, like for chest and back collision volumes"), MinVersion(6)]
    public bool meshYBias;

    public new CharCollide Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        trans.Read(reader, false, parent, entry);

        shape = (Shape)reader.ReadUInt32();

        origRadius[0] = reader.ReadFloat();

        if (revision > 4)
        {
            origLength[0] = reader.ReadFloat();
        }

        if (revision > 2)
        {
            origLength[1] = reader.ReadFloat();
        }

        if (revision > 1)
        {
            flags = reader.ReadInt32();
            SetFromFlags(flags);
        }
        if (revision > 3)
        {
            curRadius[0] = reader.ReadFloat();
        }

        if (revision > 5)
        {
            origRadius[1] = reader.ReadFloat();
            curRadius[1] = reader.ReadFloat();
            curLength[0] = reader.ReadFloat();
            curLength[1] = reader.ReadFloat();
            unkTransform.Read(reader);
            mesh = Symbol.Read(reader);
            for (int i = 0; i < 8; i++)
            {
                CharCollideStruct unkStruct = new();
                unkStruct.Read(reader);
                structs.Add(unkStruct);
            }

            sha1Digest = reader.ReadBlock(20);
            meshYBias = reader.ReadBoolean();
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
        trans.Write(writer, false, parent, null);

        writer.WriteUInt32((uint)shape);

        writer.WriteFloat(origRadius[0]);

        if (revision > 4)
        {
            writer.WriteFloat(origLength[0]);
        }

        if (revision > 2)
        {
            writer.WriteFloat(origLength[1]);
        }

        if (revision > 1)
        {
            flags = GetFlags();
            writer.WriteInt32(flags);
        }
        if (revision > 3)
        {
            writer.WriteFloat(curRadius[0]);
        }

        if (revision > 5)
        {
            writer.WriteFloat(origRadius[1]);
            writer.WriteFloat(curRadius[1]);
            writer.WriteFloat(curLength[0]);
            writer.WriteFloat(curLength[1]);
            unkTransform.Write(writer);
            Symbol.Write(writer, mesh);

            while (structs.Count < 8)
            {
                structs.Add(new CharCollideStruct());
            }
            for (int i = 0; i < 8; i++)
            {
                structs[i].Write(writer);
            }

            writer.WriteBlock(sha1Digest);
            writer.WriteBoolean(meshYBias);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
