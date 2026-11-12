using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("MeshDeform"), Description("Reskins target mesh according to exobones.")]
public class RndMeshDeform : Object
{
    public class BoneDesc
    {
        [Name("Bone")]
        public Symbol bone = Symbol.Empty;

        [HideInInspector]
        public Matrix transform1 = new();

        [HideInInspector]
        public Matrix transform2 = new();

        public BoneDesc Read(EndianReader reader)
        {
            bone = Symbol.Read(reader);
            transform1 = new Matrix().Read(reader);
            transform2 = new Matrix().Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, bone);
            transform1.Write(writer);
            transform2.Write(writer);
        }
    }

    public class VertArray
    {
        [HideInInspector]
        public byte[] data = Array.Empty<byte>();

        public VertArray Read(EndianReader reader)
        {
            int size = reader.ReadInt32();
            data = size > 0 ? reader.ReadBlock(size) : Array.Empty<byte>();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32(data.Length);
            if (data.Length > 0)
            {
                writer.WriteBlock(data);
            }
        }

        public int NumVerts()
        {
            int count = 0;
            int offset = 0;

            while (offset < data.Length)
            {
                int weightCount = data[offset];
                offset += (weightCount * 2) + 1;
                if (offset > data.Length)
                {
                    throw new InvalidDataException("MeshDeform vert array is truncated.");
                }

                count++;
            }

            return count;
        }
    }

    private ushort altRevision;
    private ushort revision;

    private bool UsesKaraokeLayout(DirectoryMeta parent)
    {
        return parent.revision == 10 && revision == 3;
    }

    [Name("Mesh"), Description("The mesh we will change, set you can make a zero vert meshdeform just to clean up mutable character meshes")]
    public Symbol mesh = Symbol.Empty;

    [HideInInspector]
    public Matrix meshInverse = new();

    [HideInInspector]
    public List<BoneDesc> bones = new();

    [HideInInspector]
    public VertArray verts = new();

    [HideInInspector]
    public int oldWeightVertexCount;

    [HideInInspector]
    public byte[] oldWeights = Array.Empty<byte>();

    public List<Symbol> oldReferences = new();

    [HideInInspector]
    public bool skipInverse;

    [HideInInspector]
    public bool deformed;

    public new RndMeshDeform Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        mesh = Symbol.Read(reader);

        // kr specific handling, split it out into its own funcs to keep it a bit cleaner
        if (UsesKaraokeLayout(parent))
        {
            ReadKaraokeBones(reader);
        }
        else
        {
            if (revision < 1)
            {
                oldWeightVertexCount = reader.ReadInt32();
            }

            int boneCount = reader.ReadInt32();

            if (revision < 1)
            {
                int legacyWeightBytes = checked(oldWeightVertexCount * boneCount * sizeof(float));
                if (legacyWeightBytes > 0)
                {
                    oldWeights = reader.ReadBlock(legacyWeightBytes);
                }
                else
                {
                    oldWeights = Array.Empty<byte>();
                }
            }

            bones = new List<BoneDesc>(boneCount);
            for (int i = 0; i < boneCount; i++)
            {
                BoneDesc boneDesc = new();
                boneDesc.Read(reader);
                bones.Add(boneDesc);
            }

            if (revision != 0)
            {
                verts = new VertArray().Read(reader);
            }
            else
            {
                verts = new VertArray();
            }
        }

        meshInverse = new Matrix().Read(reader);
        skipInverse = IsIdentityTransform(meshInverse);

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

        Symbol.Write(writer, mesh);

        if (UsesKaraokeLayout(parent))
        {
            WriteKaraokeBones(writer);
        }
        else
        {
            if (revision < 1)
            {
                writer.WriteInt32(oldWeightVertexCount);
            }

            writer.WriteInt32(bones.Count);

            if (revision < 1)
            {
                int expectedLegacyBytes = checked(oldWeightVertexCount * bones.Count * sizeof(float));
                if (oldWeights.Length != expectedLegacyBytes)
                {
                    throw new InvalidOperationException($"MeshDeform legacy weight block is {oldWeights.Length} bytes, expected {expectedLegacyBytes}.");
                }

                if (oldWeights.Length > 0)
                {
                    writer.WriteBlock(oldWeights);
                }
            }

            foreach (BoneDesc boneDesc in bones)
            {
                boneDesc.Write(writer);
            }

            if (revision != 0)
            {
                verts.Write(writer);
            }
        }

        meshInverse.Write(writer);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private void ReadKaraokeBones(EndianReader reader)
    {
        oldWeightVertexCount = reader.ReadInt32();
        int boneCount = reader.ReadInt32();
        long remaining = reader.BaseStream.Length - reader.BaseStream.Position;
        if (oldWeightVertexCount < 0 || boneCount < 0 || boneCount > remaining / 52)
        {
            throw new InvalidDataException("MeshDeform legacy vertex or bone count is invalid.");
        }

        long weightBytes = (long)oldWeightVertexCount * boneCount * sizeof(float);
        if (weightBytes > int.MaxValue || weightBytes > remaining - (long)boneCount * 52 - 52)
        {
            throw new InvalidDataException("MeshDeform legacy weights exceed the remaining data.");
        }

        oldWeights = reader.ReadBlock((int)weightBytes);
        bones = new List<BoneDesc>(boneCount);
        for (int i = 0; i < boneCount; i++)
        {
            bones.Add(new BoneDesc { bone = Symbol.Read(reader) });
        }

        foreach (BoneDesc bone in bones)
        {
            bone.transform1.Read(reader);
        }

        oldReferences = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        verts = new VertArray();
    }

    private void WriteKaraokeBones(EndianWriter writer)
    {
        long expectedWeightBytes = (long)oldWeightVertexCount * bones.Count * sizeof(float);
        if (oldWeightVertexCount < 0 || oldWeights.Length != expectedWeightBytes)
        {
            throw new InvalidOperationException("MeshDeform legacy weights do not match the vertex and bone counts.");
        }

        writer.WriteInt32(oldWeightVertexCount);
        writer.WriteInt32(bones.Count);
        writer.WriteBlock(oldWeights);
        foreach (BoneDesc bone in bones)
        {
            Symbol.Write(writer, bone.bone);
        }

        foreach (BoneDesc bone in bones)
        {
            bone.transform1.Write(writer);
        }

        AssetHelpers.WriteList(writer, oldReferences, (Symbol reference) => Symbol.Write(writer, reference));
    }

    private static bool IsIdentityTransform(Matrix transform)
    {
        return transform.m41 == 0.0f && transform.m42 == 0.0f && transform.m43 == 0.0f
            && transform.m11 == 1.0f && transform.m12 == 0.0f && transform.m13 == 0.0f
            && transform.m21 == 0.0f && transform.m22 == 1.0f && transform.m23 == 0.0f
            && transform.m31 == 0.0f && transform.m32 == 0.0f && transform.m33 == 1.0f;
    }
}
