using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

public struct Vertex
{
    // Basic fields
    public float x;
    public float y;
    public float z;
    public float w;

    // Normals
    public float nx;
    public float ny;
    public float nz;
    public float nw;

    // UVs
    public float u;
    public float v;

    // Weights
    public float weight0;
    public float weight1;
    public float weight2;
    public float weight3;

    // Bone indices
    public ushort bone0;
    public ushort bone1;
    public ushort bone2;
    public ushort bone3;

    // Tangents
    public float tangent0;
    public float tangent1;
    public float tangent2;
    public float tangent3;

    // Packed data
    public uint packed1;
    public uint packed2;
    public uint packed3;
    public uint packed4;

    // Special fields
    public float unknown1;
    public float unk2;
    public float neg1;
    public float pos1;

    public Color vertexColors = new();

    public bool hasRawCompressedVertexData;
    public uint rawCompressedColor;
    public ushort rawCompressedU;
    public ushort rawCompressedV;
    public uint rawCompressedNormal;
    public uint rawCompressedTangent;
    public uint rawCompressedWeights;
    public uint rawCompressedBones32;
    public ushort rawCompressedBone0;
    public ushort rawCompressedBone1;
    public ushort rawCompressedBone2;
    public ushort rawCompressedBone3;

    public Vertex()
    {
    }

    public override string ToString()
    {
        return $"<{x}, {y}, {z}>";
    }

    /// <summary>
    /// Vertex color channels, holding the byte values read from compressed vertices. Each channel defaults to 1.
    /// </summary>
    public struct Color
    {
        public float r = 1.0f;
        public float g = 1.0f;
        public float b = 1.0f;
        public float a = 1.0f;

        public Color()
        {
        }
    }

    // Xbox 360: 10-10-10-2 format
    public class SignedCompressedVec4
    {
        public uint origValue { get; private set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        private static int ToSNormBits(float f, int n)
        {
            f = Math.Clamp(f, -1f, 1f);
            int max = (1 << (n - 1)) - 1;
            int s = (int)MathF.Truncate(f * max);
            if (s < 0)
            {
                s += (1 << n);
            }

            return s & ((1 << n) - 1);
        }

        private static float FromSNormBits(int bits, int n)
        {
            int max = (1 << (n - 1)) - 1;
            int s = bits;
            if (s > max)
            {
                s -= (1 << n);
            }

            return Math.Max(s / (float)max, -1f);
        }

        public static void ReadValues(EndianReader reader, out float x, out float y, out float z, out float w)
        {
            uint value = reader.ReadUInt32();
            Decode(value, out x, out y, out z, out w);
        }

        public static void WriteValues(EndianWriter writer, float x, float y, float z, float w)
        {
            writer.WriteUInt32(Encode(x, y, z, w));
        }

        public static void Decode(uint value, out float x, out float y, out float z, out float w)
        {
            int xb = (int)(value & 0x3FF);
            int yb = (int)((value >> 10) & 0x3FF);
            int zb = (int)((value >> 20) & 0x3FF);
            int wb = (int)((value >> 30) & 0x003);

            x = FromSNormBits(xb, 10);
            y = FromSNormBits(yb, 10);
            z = FromSNormBits(zb, 10);
            w = FromSNormBits(wb, 2);
        }

        public static uint Encode(float x, float y, float z, float w)
        {
            int xBits = ToSNormBits(x, 10);
            int yBits = ToSNormBits(y, 10);
            int zBits = ToSNormBits(z, 10);
            int wBits = ToSNormBits(w, 2);

            return (uint)(xBits & 0x3FF)
                 | (uint)((yBits & 0x3FF) << 10)
                 | (uint)((zBits & 0x3FF) << 20)
                 | (uint)((wBits & 0x003) << 30);
        }

        public SignedCompressedVec4 Read(EndianReader reader)
        {
            uint value = reader.ReadUInt32();
            origValue = value;

            Decode(value, out float readX, out float readY, out float readZ, out float readW);
            x = readX;
            y = readY;
            z = readZ;
            w = readW;

            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt32(Encode(x, y, z, w));
        }
    }

    // PS3: 11-11-10 format (signed, for norms)
    public class PS3SignedCompressedVec3
    {
        public uint origValue { get; private set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        private static int ToSNormBits(float f, int n)
        {
            f = Math.Clamp(f, -1f, 1f);
            int max = (1 << (n - 1)) - 1;
            int s = (int)MathF.Truncate(f * max);
            if (s < 0)
            {
                s += (1 << n);
            }

            return s & ((1 << n) - 1);
        }

        private static float FromSNormBits(int bits, int n)
        {
            int max = (1 << (n - 1)) - 1;
            int s = bits;
            if (s > max)
            {
                s -= (1 << n);
            }

            return Math.Max(s / (float)max, -1f);
        }

        public static void ReadValues(EndianReader reader, out float x, out float y, out float z, out float w)
        {
            uint value = reader.ReadUInt32();
            Decode(value, out x, out y, out z, out w);
        }

        public static void WriteValues(EndianWriter writer, float x, float y, float z)
        {
            writer.WriteUInt32(Encode(x, y, z));
        }

        public static void Decode(uint value, out float x, out float y, out float z, out float w)
        {
            int xb = (int)(value & 0x7FF);
            int yb = (int)((value >> 11) & 0x7FF);
            int zb = (int)((value >> 22) & 0x3FF);

            x = FromSNormBits(xb, 11);
            y = FromSNormBits(yb, 11);
            z = FromSNormBits(zb, 10);
            w = 0.0f;
        }

        public static uint Encode(float x, float y, float z)
        {
            int xBits = ToSNormBits(x, 11);
            int yBits = ToSNormBits(y, 11);
            int zBits = ToSNormBits(z, 10);

            return (uint)(xBits & 0x7FF)
                 | (uint)((yBits & 0x7FF) << 11)
                 | (uint)((zBits & 0x3FF) << 22);
        }

        public PS3SignedCompressedVec3 Read(EndianReader reader)
        {
            uint value = reader.ReadUInt32();
            origValue = value;

            Decode(value, out float readX, out float readY, out float readZ, out float readW);
            x = readX;
            y = readY;
            z = readZ;
            w = readW;

            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt32(Encode(x, y, z));
        }
    }

    // Xbox 360: 10-10-10-2 format (unsigned, for weights)
    public class UnsignedCompressedVec4
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public static void ReadValues(EndianReader reader, out float x, out float y, out float z, out float w)
        {
            uint value = reader.ReadUInt32();
            Decode(value, out x, out y, out z, out w);
        }

        public static void WriteValues(EndianWriter writer, float x, float y, float z, float w)
        {
            writer.WriteUInt32(Encode(x, y, z, w));
        }

        public static void Decode(uint value, out float x, out float y, out float z, out float w)
        {
            uint xb = value & 0x3FF;
            uint yb = (value >> 10) & 0x3FF;
            uint zb = (value >> 20) & 0x3FF;
            uint wb = (value >> 30) & 0x003;

            x = xb / 1023f;
            y = yb / 1023f;
            z = zb / 1023f;
            w = wb / 3f;
        }

        public static uint Encode(float x, float y, float z, float w)
        {
            int xBits = (int)MathF.Truncate(Math.Clamp(x, 0f, 1f) * 1023f) & 0x3FF;
            int yBits = (int)MathF.Truncate(Math.Clamp(y, 0f, 1f) * 1023f) & 0x3FF;
            int zBits = (int)MathF.Truncate(Math.Clamp(z, 0f, 1f) * 1023f) & 0x3FF;
            int wBits = (int)MathF.Truncate(Math.Clamp(w, 0f, 1f) * 3f) & 0x003;

            return (uint)xBits
                 | (uint)(yBits << 10)
                 | (uint)(zBits << 20)
                 | (uint)(wBits << 30);
        }

        public UnsignedCompressedVec4 Read(EndianReader reader)
        {
            uint value = reader.ReadUInt32();

            Decode(value, out float readX, out float readY, out float readZ, out float readW);
            x = readX;
            y = readY;
            z = readZ;
            w = readW;

            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt32(Encode(x, y, z, w));
        }
    }

    // PS3: 11-11-10 format (unsigned, for weights)
    public class PS3UnsignedCompressedVec3
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
        public float w { get; set; }

        public static void ReadValues(EndianReader reader, out float x, out float y, out float z, out float w)
        {
            uint value = reader.ReadUInt32();
            Decode(value, out x, out y, out z, out w);
        }

        public static void WriteValues(EndianWriter writer, float x, float y, float z)
        {
            writer.WriteUInt32(Encode(x, y, z));
        }

        public static void Decode(uint value, out float x, out float y, out float z, out float w)
        {
            uint xb = value & 0x7FF;
            uint yb = (value >> 11) & 0x7FF;
            uint zb = (value >> 22) & 0x3FF;

            x = xb / 1023f;
            y = yb / 1023f;
            z = zb / 511f;
            w = 0.0f;
        }

        public static uint Encode(float x, float y, float z)
        {
            int xBits = (int)MathF.Truncate(Math.Clamp(x, 0f, 1f) * 1023f) & 0x7FF;
            int yBits = (int)MathF.Truncate(Math.Clamp(y, 0f, 1f) * 1023f) & 0x7FF;
            int zBits = (int)MathF.Truncate(Math.Clamp(z, 0f, 1f) * 511f) & 0x3FF;

            return (uint)xBits
                 | (uint)(yBits << 11)
                 | (uint)(zBits << 22);
        }

        public PS3UnsignedCompressedVec3 Read(EndianReader reader)
        {
            uint value = reader.ReadUInt32();

            Decode(value, out float readX, out float readY, out float readZ, out float readW);
            x = readX;
            y = readY;
            z = readZ;
            w = readW;

            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt32(Encode(x, y, z));
        }
    }

    public class QTangent
    {
        public SNorm x { get; set; } = new SNorm();
        public SNorm y { get; set; } = new SNorm();
        public SNorm z { get; set; } = new SNorm();
        public SNorm w { get; set; } = new SNorm();

        public class SNorm
        {
            public short value { get; set; }
            public float fValue => Math.Max((float)value / 32767.0f, -1.0f);

            public SNorm() { }
            public SNorm(short value)
            {
                this.value = value;
            }
        }

        public QTangent Read(EndianReader reader)
        {
            x.value = reader.ReadInt16();
            y.value = reader.ReadInt16();
            z.value = reader.ReadInt16();
            w.value = reader.ReadInt16();

            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt16(x.value);
            writer.WriteInt16(y.value);
            writer.WriteInt16(z.value);
            writer.WriteInt16(w.value);
        }
    }
}
