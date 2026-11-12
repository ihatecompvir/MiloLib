using MiloLib.Classes;
using MiloLib.Utils;
using System.Runtime.InteropServices;

namespace MiloLib.Assets.Rnd;

[Name("Mesh"), Description("A Mesh object is composed of triangle faces.")]
public class RndMesh : Object
{
    public class BoneTransform
    {
        [Name("Bone"), Description("Trans of the bone")]
        public Symbol name = Symbol.Empty;
        public Matrix transform = new();

        public BoneTransform Read(EndianReader reader, uint meshRevision)
        {
            name = Symbol.Read(reader);
            transform = transform.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer, uint meshRevision)
        {
            Symbol.Write(writer, name);
            transform.Write(writer);
        }

        public override string ToString()
        {
            return $"{name} {transform}";
        }
    }

    public class Face
    {
        public ushort idx1;
        public ushort idx2;
        public ushort idx3;

        public Face Read(EndianReader reader)
        {
            idx1 = reader.ReadUInt16();
            idx2 = reader.ReadUInt16();
            idx3 = reader.ReadUInt16();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt16(idx1);
            writer.WriteUInt16(idx2);
            writer.WriteUInt16(idx3);
        }

        public override string ToString()
        {
            return $"{idx1} {idx2} {idx3}";
        }
    }

    public class Vertices
    {
        private uint count;
        public bool isNextGen;
        public uint vertexSize;
        public uint compressionType;
        public List<Vertex> vertices = new();

        public Vertices Read(EndianReader reader, uint meshVersion, bool legacyPlatformLayout = false, bool phaseLayout = false)
        {
            count = reader.ReadUInt32();
            if (count > (reader.BaseStream.Length - reader.BaseStream.Position) / 12)
            {
                throw new InvalidDataException("The mesh vertex count exceeds the remaining stream.");
            }
            if (meshVersion >= 36)
            {
                isNextGen = reader.ReadBoolean();
                if (isNextGen)
                {
                    vertexSize = reader.ReadUInt32();
                    compressionType = reader.ReadUInt32();
                }
                ReadVertices(reader, meshVersion, isNextGen, compressionType, legacyPlatformLayout, phaseLayout);
                return this;
            }
            else
            {
                ReadVertices(reader, meshVersion, isNextGen, 0, legacyPlatformLayout, phaseLayout);
            }

            return this;
        }

        public void Write(EndianWriter writer, uint meshVersion, bool legacyPlatformLayout = false, bool phaseLayout = false)
        {
            writer.WriteUInt32((uint)vertices.Count);
            if (meshVersion >= 36)
            {
                writer.WriteBoolean(isNextGen);
                if (isNextGen)
                {
                    writer.WriteUInt32(vertexSize);
                    writer.WriteUInt32(compressionType);
                }
                WriteVertices(writer, meshVersion, isNextGen, compressionType, legacyPlatformLayout, phaseLayout);
                return;
            }
            else
            {
                WriteVertices(writer, meshVersion, isNextGen, 0, legacyPlatformLayout, phaseLayout);
            }
        }

        // TODO:
        // these read and write vertex functions are genuinely horrible. need to refactor these in the future so converting between Mesh versions can be done more reliably
        public void ReadVertices(EndianReader reader, uint meshVersion, bool isNextGen, uint compressionType, bool legacyPlatformLayout = false, bool phaseLayout = false)
        {
            vertices = new List<Vertex>((int)count);
            // the largest vertex layout is well under 256 bytes, so we can speed things up a bit with this
            EndianSpanReader r = reader.CreateSpanReader((int)Math.Min(int.MaxValue, (long)count * 256));
            for (int i = 0; i < count; i++)
            {
                Vertex newVert = new();
                newVert.x = r.ReadFloat();
                newVert.y = r.ReadFloat();
                newVert.z = r.ReadFloat();

                if (meshVersion == 34 && !legacyPlatformLayout)
                {
                    newVert.w = r.ReadFloat();
                }


                // freQ
                if (meshVersion <= 10)
                {
                    newVert.nx = r.ReadFloat();
                    newVert.ny = r.ReadFloat();
                    newVert.nz = r.ReadFloat();

                    newVert.u = r.ReadFloat();
                    newVert.v = r.ReadFloat();

                    newVert.weight0 = r.ReadFloat();
                    newVert.weight1 = r.ReadFloat();
                    newVert.weight2 = r.ReadFloat();
                    newVert.weight3 = r.ReadFloat();

                    newVert.bone0 = r.ReadUInt16();
                    newVert.bone1 = r.ReadUInt16();
                    newVert.bone2 = r.ReadUInt16();
                    newVert.bone3 = r.ReadUInt16();
                }
                else if (meshVersion <= 22)
                {
                    newVert.bone0 = r.ReadUInt16();
                    newVert.bone1 = r.ReadUInt16();
                    newVert.bone2 = r.ReadUInt16();
                    newVert.bone3 = r.ReadUInt16();

                    newVert.nx = r.ReadFloat();
                    newVert.ny = r.ReadFloat();
                    newVert.nz = r.ReadFloat();

                    newVert.weight0 = r.ReadFloat();
                    newVert.weight1 = r.ReadFloat();
                    newVert.weight2 = r.ReadFloat();
                    newVert.weight3 = r.ReadFloat();

                    newVert.u = r.ReadFloat();
                    newVert.v = r.ReadFloat();
                }
                else if (meshVersion < 35 || isNextGen == false)
                {
                    if (meshVersion >= 38)
                    {
                        newVert.packed1 = r.ReadUInt32();
                        newVert.unknown1 = r.ReadFloat();

                        newVert.packed2 = r.ReadUInt32();
                        newVert.unk2 = r.ReadFloat();
                    }

                    newVert.nx = r.ReadFloat();
                    newVert.ny = r.ReadFloat();
                    newVert.nz = r.ReadFloat();

                    if (meshVersion == 34 && !legacyPlatformLayout)
                    {
                        newVert.nw = r.ReadFloat();
                    }

                    if (meshVersion >= 38)
                    {
                        newVert.u = r.ReadFloat();
                        newVert.v = r.ReadFloat();

                        newVert.weight0 = r.ReadFloat();
                        newVert.weight1 = r.ReadFloat();
                        newVert.weight2 = r.ReadFloat();
                        newVert.weight3 = r.ReadFloat();
                    }
                    else
                    {
                        newVert.weight0 = r.ReadFloat();
                        newVert.weight1 = r.ReadFloat();
                        newVert.weight2 = r.ReadFloat();
                        newVert.weight3 = r.ReadFloat();

                        newVert.u = r.ReadFloat();
                        newVert.v = r.ReadFloat();

                    }

                    if (phaseLayout)
                    {
                        newVert.bone0 = r.ReadUInt16();
                        newVert.bone1 = r.ReadUInt16();
                        newVert.bone2 = r.ReadUInt16();
                        newVert.bone3 = r.ReadUInt16();
                    }

                    if (meshVersion >= 33)
                    {
                        newVert.bone0 = r.ReadUInt16();
                        newVert.bone1 = r.ReadUInt16();
                        newVert.bone2 = r.ReadUInt16();
                        newVert.bone3 = r.ReadUInt16();

                        if (meshVersion >= 38)
                        {
                            newVert.packed3 = r.ReadUInt32();
                            newVert.packed4 = r.ReadUInt32();
                            newVert.neg1 = r.ReadFloat();
                            newVert.pos1 = r.ReadFloat();
                        }
                        else
                        {
                            newVert.tangent0 = r.ReadFloat();
                            newVert.tangent1 = r.ReadFloat();
                            newVert.tangent2 = r.ReadFloat();
                            newVert.tangent3 = r.ReadFloat();
                            if (meshVersion > 34 && !legacyPlatformLayout)
                            {
                                newVert.unknown1 = r.ReadFloat();
                                newVert.unk2 = r.ReadFloat();
                            }
                        }
                    }
                }
                else
                {
                    if (compressionType == 1)
                    {
                        uint value = r.ReadUInt32();
                        newVert.hasRawCompressedVertexData = true;
                        newVert.rawCompressedColor = value;
                        newVert.vertexColors.r = (byte)((value >> 24) & 0xFF);
                        newVert.vertexColors.g = (byte)((value >> 16) & 0xFF);
                        newVert.vertexColors.b = (byte)((value >> 8) & 0xFF);
                        newVert.vertexColors.a = (byte)(value & 0xFF);
                        newVert.rawCompressedU = r.ReadUInt16();
                        newVert.rawCompressedV = r.ReadUInt16();
                        newVert.u = (float)BitConverter.UInt16BitsToHalf(newVert.rawCompressedU);
                        newVert.v = (float)BitConverter.UInt16BitsToHalf(newVert.rawCompressedV);

                        newVert.rawCompressedNormal = r.ReadUInt32();
                        Vertex.SignedCompressedVec4.Decode(newVert.rawCompressedNormal, out float nx, out float ny, out float nz, out float nw);
                        newVert.nx = nx;
                        newVert.ny = ny;
                        newVert.nz = nz;
                        newVert.nw = nw;

                        newVert.rawCompressedTangent = r.ReadUInt32();
                        Vertex.SignedCompressedVec4.Decode(newVert.rawCompressedTangent, out float tangent0, out float tangent1, out float tangent2, out float tangent3);
                        newVert.tangent0 = tangent0;
                        newVert.tangent1 = tangent1;
                        newVert.tangent2 = tangent2;
                        newVert.tangent3 = tangent3;

                        newVert.rawCompressedWeights = r.ReadUInt32();
                        Vertex.UnsignedCompressedVec4.Decode(newVert.rawCompressedWeights, out float weight0, out float weight1, out float weight2, out float weight3);
                        newVert.weight0 = weight0;
                        newVert.weight1 = weight1;
                        newVert.weight2 = weight2;
                        newVert.weight3 = weight3;

                        byte bone0 = r.ReadByte();
                        byte bone1 = r.ReadByte();
                        byte bone2 = r.ReadByte();
                        byte bone3 = r.ReadByte();
                        newVert.bone0 = bone0;
                        newVert.bone1 = bone1;
                        newVert.bone2 = bone2;
                        newVert.bone3 = bone3;
                        newVert.rawCompressedBones32 = (uint)(bone0 | (bone1 << 8) | (bone2 << 16) | (bone3 << 24));
                    }
                    else if (compressionType == 2)
                    {
                        newVert.hasRawCompressedVertexData = true;
                        newVert.rawCompressedU = r.ReadUInt16();
                        newVert.rawCompressedV = r.ReadUInt16();
                        newVert.u = (float)BitConverter.UInt16BitsToHalf(newVert.rawCompressedU);
                        newVert.v = (float)BitConverter.UInt16BitsToHalf(newVert.rawCompressedV);

                        newVert.rawCompressedNormal = r.ReadUInt32();
                        Vertex.PS3SignedCompressedVec3.Decode(newVert.rawCompressedNormal, out float nx, out float ny, out float nz, out float nw);
                        newVert.nx = nx;
                        newVert.ny = ny;
                        newVert.nz = nz;
                        newVert.nw = nw;

                        newVert.rawCompressedTangent = r.ReadUInt32();
                        Vertex.PS3SignedCompressedVec3.Decode(newVert.rawCompressedTangent, out float tangent0, out float tangent1, out float tangent2, out float tangent3);
                        newVert.tangent0 = tangent0;
                        newVert.tangent1 = tangent1;
                        newVert.tangent2 = tangent2;
                        newVert.tangent3 = tangent3;

                        newVert.rawCompressedWeights = r.ReadUInt32();
                        Vertex.PS3UnsignedCompressedVec3.Decode(newVert.rawCompressedWeights, out float weight0, out float weight1, out float weight2, out float weight3);

                        uint value = r.ReadUInt32();
                        newVert.rawCompressedColor = value;
                        newVert.vertexColors.a = (byte)((value >> 24) & 0xFF);
                        newVert.vertexColors.r = (byte)((value >> 16) & 0xFF);
                        newVert.vertexColors.g = (byte)((value >> 8) & 0xFF);
                        newVert.vertexColors.b = (byte)(value & 0xFF);

                        newVert.weight0 = weight0;
                        newVert.weight1 = weight1;
                        newVert.weight2 = weight2;
                        newVert.weight3 = weight3;

                        newVert.bone0 = r.ReadUInt16();
                        newVert.bone1 = r.ReadUInt16();
                        newVert.bone2 = r.ReadUInt16();
                        newVert.bone3 = r.ReadUInt16();
                        newVert.rawCompressedBone0 = newVert.bone0;
                        newVert.rawCompressedBone1 = newVert.bone1;
                        newVert.rawCompressedBone2 = newVert.bone2;
                        newVert.rawCompressedBone3 = newVert.bone3;
                    }
                }

                vertices.Add(newVert);
            }

            reader.Advance(r.Position);
        }

        public void WriteVertices(EndianWriter writer, uint meshVersion, bool isNextGen, uint compressionType, bool legacyPlatformLayout = false, bool phaseLayout = false)
        {
            foreach (ref readonly Vertex vertex in CollectionsMarshal.AsSpan(vertices))
            {
                // Always write position
                writer.WriteFloat(vertex.x);
                writer.WriteFloat(vertex.y);
                writer.WriteFloat(vertex.z);

                // Possible W
                if (meshVersion == 34 && !legacyPlatformLayout)
                {
                    writer.WriteFloat(vertex.w);
                }

                if (meshVersion <= 10)
                {
                    writer.WriteFloat(vertex.nx);
                    writer.WriteFloat(vertex.ny);
                    writer.WriteFloat(vertex.nz);

                    writer.WriteFloat(vertex.u);
                    writer.WriteFloat(vertex.v);

                    writer.WriteFloat(vertex.weight0);
                    writer.WriteFloat(vertex.weight1);
                    writer.WriteFloat(vertex.weight2);
                    writer.WriteFloat(vertex.weight3);

                    writer.WriteUInt16(vertex.bone0);
                    writer.WriteUInt16(vertex.bone1);
                    writer.WriteUInt16(vertex.bone2);
                    writer.WriteUInt16(vertex.bone3);
                }
                else if (meshVersion <= 22)
                {
                    writer.WriteUInt16(vertex.bone0);
                    writer.WriteUInt16(vertex.bone1);
                    writer.WriteUInt16(vertex.bone2);
                    writer.WriteUInt16(vertex.bone3);

                    writer.WriteFloat(vertex.nx);
                    writer.WriteFloat(vertex.ny);
                    writer.WriteFloat(vertex.nz);

                    writer.WriteFloat(vertex.weight0);
                    writer.WriteFloat(vertex.weight1);
                    writer.WriteFloat(vertex.weight2);
                    writer.WriteFloat(vertex.weight3);

                    writer.WriteFloat(vertex.u);
                    writer.WriteFloat(vertex.v);
                }
                else if (meshVersion < 35 || !isNextGen)
                {
                    if (meshVersion >= 38)
                    {
                        writer.WriteUInt32(vertex.packed1);
                        writer.WriteFloat(vertex.unknown1);
                        writer.WriteUInt32(vertex.packed2);
                        writer.WriteFloat(vertex.unk2);
                    }

                    writer.WriteFloat(vertex.nx);
                    writer.WriteFloat(vertex.ny);
                    writer.WriteFloat(vertex.nz);

                    if (meshVersion == 34 && !legacyPlatformLayout)
                    {
                        writer.WriteFloat(vertex.nw);
                    }

                    if (meshVersion >= 38)
                    {
                        writer.WriteFloat(vertex.u);
                        writer.WriteFloat(vertex.v);

                        writer.WriteFloat(vertex.weight0);
                        writer.WriteFloat(vertex.weight1);
                        writer.WriteFloat(vertex.weight2);
                        writer.WriteFloat(vertex.weight3);
                    }
                    else
                    {
                        writer.WriteFloat(vertex.weight0);
                        writer.WriteFloat(vertex.weight1);
                        writer.WriteFloat(vertex.weight2);
                        writer.WriteFloat(vertex.weight3);

                        writer.WriteFloat(vertex.u);
                        writer.WriteFloat(vertex.v);
                    }

                    if (phaseLayout)
                    {
                        writer.WriteUInt16(vertex.bone0);
                        writer.WriteUInt16(vertex.bone1);
                        writer.WriteUInt16(vertex.bone2);
                        writer.WriteUInt16(vertex.bone3);
                    }

                    if (meshVersion >= 33)
                    {
                        writer.WriteUInt16(vertex.bone0);
                        writer.WriteUInt16(vertex.bone1);
                        writer.WriteUInt16(vertex.bone2);
                        writer.WriteUInt16(vertex.bone3);

                        if (meshVersion >= 38)
                        {
                            writer.WriteUInt32(vertex.packed3);
                            writer.WriteUInt32(vertex.packed4);
                            writer.WriteFloat(vertex.neg1);
                            writer.WriteFloat(vertex.pos1);
                        }
                        else
                        {
                            writer.WriteFloat(vertex.tangent0);
                            writer.WriteFloat(vertex.tangent1);
                            writer.WriteFloat(vertex.tangent2);
                            writer.WriteFloat(vertex.tangent3);
                            if (meshVersion > 34 && !legacyPlatformLayout)
                            {
                                writer.WriteFloat(vertex.unknown1);
                                writer.WriteFloat(vertex.unk2);
                            }


                        }
                    }
                }
                else
                {
                    if (compressionType == 1)
                    {
                        if (CanWriteRawXboxCompressedVertex(vertex))
                        {
                            writer.WriteUInt32(vertex.rawCompressedColor);
                            writer.WriteUInt16(vertex.rawCompressedU);
                            writer.WriteUInt16(vertex.rawCompressedV);
                            writer.WriteUInt32(vertex.rawCompressedNormal);
                            writer.WriteUInt32(vertex.rawCompressedTangent);
                            writer.WriteUInt32(vertex.rawCompressedWeights);
                            writer.WriteByte((byte)(vertex.rawCompressedBones32 & 0xFF));
                            writer.WriteByte((byte)((vertex.rawCompressedBones32 >> 8) & 0xFF));
                            writer.WriteByte((byte)((vertex.rawCompressedBones32 >> 16) & 0xFF));
                            writer.WriteByte((byte)((vertex.rawCompressedBones32 >> 24) & 0xFF));
                            continue;
                        }

                        // vertex colors in RGBA?
                        uint value = ((uint)vertex.vertexColors.r << 24) | ((uint)vertex.vertexColors.g << 16) | ((uint)vertex.vertexColors.b << 8) | (uint)vertex.vertexColors.a;
                        writer.WriteUInt32(value);
                        writer.WriteHalfFloat(vertex.u);
                        writer.WriteHalfFloat(vertex.v);

                        Vertex.SignedCompressedVec4.WriteValues(writer, vertex.nx, vertex.ny, vertex.nz, vertex.nw);
                        Vertex.SignedCompressedVec4.WriteValues(writer, vertex.tangent0, vertex.tangent1, vertex.tangent2, vertex.tangent3);
                        Vertex.UnsignedCompressedVec4.WriteValues(writer, vertex.weight0, vertex.weight1, vertex.weight2, vertex.weight3);

                        writer.WriteByte((byte)vertex.bone0);
                        writer.WriteByte((byte)vertex.bone1);
                        writer.WriteByte((byte)vertex.bone2);
                        writer.WriteByte((byte)vertex.bone3);
                    }
                    else if (compressionType == 2)
                    {
                        if (CanWriteRawPs3CompressedVertex(vertex))
                        {
                            writer.WriteUInt16(vertex.rawCompressedU);
                            writer.WriteUInt16(vertex.rawCompressedV);
                            writer.WriteUInt32(vertex.rawCompressedNormal);
                            writer.WriteUInt32(vertex.rawCompressedTangent);
                            writer.WriteUInt32(vertex.rawCompressedWeights);
                            writer.WriteUInt32(vertex.rawCompressedColor);
                            writer.WriteUInt16(vertex.rawCompressedBone0);
                            writer.WriteUInt16(vertex.rawCompressedBone1);
                            writer.WriteUInt16(vertex.rawCompressedBone2);
                            writer.WriteUInt16(vertex.rawCompressedBone3);
                            continue;
                        }

                        writer.WriteHalfFloat(vertex.u);
                        writer.WriteHalfFloat(vertex.v);

                        Vertex.PS3SignedCompressedVec3 norms = new()
                        {
                            x = vertex.nx,
                            y = vertex.ny,
                            z = vertex.nz,
                            w = vertex.nw
                        };
                        norms.Write(writer);

                        Vertex.PS3SignedCompressedVec3 tangents = new()
                        {
                            x = vertex.tangent0,
                            y = vertex.tangent1,
                            z = vertex.tangent2,
                            w = vertex.tangent3
                        };
                        tangents.Write(writer);

                        Vertex.PS3UnsignedCompressedVec3 weights = new()
                        {
                            x = vertex.weight0,
                            y = vertex.weight1,
                            z = vertex.weight2,
                            w = vertex.weight3
                        };
                        weights.Write(writer);
                        uint value = ((uint)vertex.vertexColors.a << 24) | ((uint)vertex.vertexColors.r << 16) | ((uint)vertex.vertexColors.g << 8) | (uint)vertex.vertexColors.b;
                        writer.WriteUInt32(value);

                        writer.WriteUInt16(vertex.bone0);
                        writer.WriteUInt16(vertex.bone1);
                        writer.WriteUInt16(vertex.bone2);
                        writer.WriteUInt16(vertex.bone3);
                    }
                }
            }
        }

        private static bool CanWriteRawXboxCompressedVertex(in Vertex vertex)
        {
            if (!vertex.hasRawCompressedVertexData)
            {
                return false;
            }

            if (vertex.rawCompressedColor != (((uint)vertex.vertexColors.r << 24) | ((uint)vertex.vertexColors.g << 16) | ((uint)vertex.vertexColors.b << 8) | (uint)vertex.vertexColors.a))
            {
                return false;
            }

            if (vertex.rawCompressedBones32 != ((uint)((byte)vertex.bone0) | ((uint)((byte)vertex.bone1) << 8) | ((uint)((byte)vertex.bone2) << 16) | ((uint)((byte)vertex.bone3) << 24)))
            {
                return false;
            }

            Vertex.SignedCompressedVec4.Decode(vertex.rawCompressedNormal, out float nx, out float ny, out float nz, out float nw);
            Vertex.SignedCompressedVec4.Decode(vertex.rawCompressedTangent, out float tangent0, out float tangent1, out float tangent2, out float tangent3);
            Vertex.UnsignedCompressedVec4.Decode(vertex.rawCompressedWeights, out float weight0, out float weight1, out float weight2, out float weight3);

            return SameHalf(vertex.rawCompressedU, vertex.u)
                && SameHalf(vertex.rawCompressedV, vertex.v)
                && SameFloat(nx, vertex.nx)
                && SameFloat(ny, vertex.ny)
                && SameFloat(nz, vertex.nz)
                && SameFloat(nw, vertex.nw)
                && SameFloat(tangent0, vertex.tangent0)
                && SameFloat(tangent1, vertex.tangent1)
                && SameFloat(tangent2, vertex.tangent2)
                && SameFloat(tangent3, vertex.tangent3)
                && SameFloat(weight0, vertex.weight0)
                && SameFloat(weight1, vertex.weight1)
                && SameFloat(weight2, vertex.weight2)
                && SameFloat(weight3, vertex.weight3);
        }

        private static bool CanWriteRawPs3CompressedVertex(in Vertex vertex)
        {
            if (!vertex.hasRawCompressedVertexData)
            {
                return false;
            }

            if (vertex.rawCompressedColor != (((uint)vertex.vertexColors.a << 24) | ((uint)vertex.vertexColors.r << 16) | ((uint)vertex.vertexColors.g << 8) | (uint)vertex.vertexColors.b))
            {
                return false;
            }

            if (vertex.rawCompressedBone0 != vertex.bone0 || vertex.rawCompressedBone1 != vertex.bone1 ||
                vertex.rawCompressedBone2 != vertex.bone2 || vertex.rawCompressedBone3 != vertex.bone3)
            {
                return false;
            }

            Vertex.PS3SignedCompressedVec3.Decode(vertex.rawCompressedNormal, out float nx, out float ny, out float nz, out float nw);
            Vertex.PS3SignedCompressedVec3.Decode(vertex.rawCompressedTangent, out float tangent0, out float tangent1, out float tangent2, out float tangent3);
            Vertex.PS3UnsignedCompressedVec3.Decode(vertex.rawCompressedWeights, out float weight0, out float weight1, out float weight2, out float weight3);

            return SameHalf(vertex.rawCompressedU, vertex.u)
                && SameHalf(vertex.rawCompressedV, vertex.v)
                && SameFloat(nx, vertex.nx)
                && SameFloat(ny, vertex.ny)
                && SameFloat(nz, vertex.nz)
                && SameFloat(nw, vertex.nw)
                && SameFloat(tangent0, vertex.tangent0)
                && SameFloat(tangent1, vertex.tangent1)
                && SameFloat(tangent2, vertex.tangent2)
                && SameFloat(tangent3, vertex.tangent3)
                && SameFloat(weight0, vertex.weight0)
                && SameFloat(weight1, vertex.weight1)
                && SameFloat(weight2, vertex.weight2)
                && SameFloat(weight3, vertex.weight3);
        }

        private static bool SameHalf(ushort rawHalf, float value)
        {
            return SameFloat((float)BitConverter.UInt16BitsToHalf(rawHalf), value);
        }

        private static bool SameFloat(float left, float right)
        {
            return BitConverter.SingleToUInt32Bits(left) == BitConverter.SingleToUInt32Bits(right);
        }
    }

    public class LegacyPatchGroup
    {
        public uint patchValue;
        public List<ushort> ushortVec = new();
        public List<uint> uintVec = new();

        public LegacyPatchGroup Read(EndianReader reader)
        {
            patchValue = reader.ReadUInt32();
            ushortVec = AssetHelpers.ReadList(reader, () => reader.ReadUInt16());

            uintVec = AssetHelpers.ReadList(reader, () => reader.ReadUInt32());

            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt32(patchValue);
            AssetHelpers.WriteList(writer, ushortVec, s => writer.WriteUInt16(s));

            AssetHelpers.WriteList(writer, uintVec, u => writer.WriteUInt32(u));
        }
    }

    public class GroupSection
    {
        public List<int> sections = new();
        public List<ushort> vertOffsets = new();

        public GroupSection Read(EndianReader reader, uint meshRevision)
        {
            uint sectionCount = reader.ReadUInt32();
            uint vertCount = reader.ReadUInt32();
            for (int i = 0; i < sectionCount; i++)
            {
                sections.Add(reader.ReadInt32());
            }
            for (int i = 0; i < vertCount; i++)
            {
                vertOffsets.Add(reader.ReadUInt16());
            }
            return this;
        }

        public void Write(EndianWriter writer, uint meshRevision)
        {
            writer.WriteUInt32((uint)sections.Count);
            writer.WriteUInt32((uint)vertOffsets.Count);
            foreach (var section in sections)
            {
                writer.WriteInt32(section);
            }
            foreach (var vert in vertOffsets)
            {
                writer.WriteUInt16(vert);
            }
        }

        public override string ToString()
        {
            return $"{sections.Count} sections, {vertOffsets.Count} vertOffsets";
        }
    }

    public enum Mutable : uint
    {
        kMutableNone = 0,
        kMutableVerts = 31,
        kMutableFaces = 32,
        kMutableAll = 63,
    }

    public enum Volume : uint
    {
        kVolumeEmpty,
        kVolumeTriangles,
        kVolumeBSP,
        kVolumeBox,
    }

    public class BSPNode
    {
        public bool hasValue;
        public MiloLib.Classes.Vector4 vec = new();
        public BSPNode? left;
        public BSPNode? right;

        public BSPNode Read(EndianReader reader)
        {
            Stack<BSPNode> pending = new(64);
            pending.Push(this);
            while (pending.Count > 0)
            {
                BSPNode node = pending.Pop();
                byte presence = reader.ReadByte();
                if (presence > 1)
                {
                    throw new InvalidDataException("Mesh BSP node presence must be zero or one.");
                }

                node.hasValue = presence == 1;
                node.left = null;
                node.right = null;
                if (node.hasValue)
                {
                    node.vec = node.vec.Read(reader);
                    node.left = new BSPNode();
                    node.right = new BSPNode();
                    pending.Push(node.right);
                    pending.Push(node.left);
                }
            }
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Stack<BSPNode> pending = new(64);
            pending.Push(this);
            while (pending.Count > 0)
            {
                BSPNode node = pending.Pop();
                writer.WriteBoolean(node.hasValue);
                if (node.hasValue)
                {
                    node.vec.Write(writer);
                    if (node.right != null)
                    {
                        pending.Push(node.right);
                    }

                    if (node.left != null)
                    {
                        pending.Push(node.left);
                    }
                }
            }
        }
    }

    [HideInInspector]
    private ushort altRevision;
    [HideInInspector]
    private ushort revision;

    public ushort RndMeshRevision => revision;
    public ushort RndMeshAltRevision => altRevision;

    public void SetRndMeshRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public RndTrans trans = new();
    public RndDrawable draw = new();

    [Name("Material"), Description("Material used for rendering the Mesh")]
    public Symbol mat = Symbol.Empty;
    [MinVersion(27), MaxVersion(27)]
    public Symbol mat2 = Symbol.Empty;
    [Name("Geometry Owner"), Description("Geometry owner for the mesh")]
    public Symbol geomOwner = Symbol.Empty;
    [MaxVersion(12)]
    public Symbol altGeomOwner = Symbol.Empty;
    [MaxVersion(14)]
    public Symbol transParent = Symbol.Empty;

    [MaxVersion(13)]
    public Symbol unkTransReference1 = Symbol.Empty;
    [MaxVersion(13)]
    public Symbol unkTransReference2 = Symbol.Empty;

    [MaxVersion(14)]
    public Symbol unkSym1 = Symbol.Empty;

    [Name("Mutable"), MinVersion(16)]
    public Mutable mutable;
    [Name("Volume"), Description("Volume of the Mesh"), MinVersion(18)]
    public Volume volume;

    [MinVersion(19)]
    public BSPNode bspNode = new();

    [Name("Verts")]
    public Vertices vertices = new();
    public List<Face> faces = new();

    private uint groupSizesCount;
    public List<byte> groupSizes = new();

    [Name("Bones")]
    public List<BoneTransform> boneTransforms = new();

    [Name("Keep Mesh Data"), Description("Whether or not to keep mesh vertices and faces. Mutable meshes will always keep this data."), MinVersion(35)]
    public bool keepMeshData;
    [Name("Has AO Calculation"), Description("Whether or not this mesh has had ambient occlusion calculated on it."), MinVersion(38)]
    public bool hasAOCalculation;
    public bool noQuant;

    public bool unkBool3;

    [Name("Exclude From Self Shadow"), Description("Whether to include this mesh in a parent Character's self-shadow generation or not.")]
    public bool excludeFromSelfShadow;

    [MinVersion(12), MaxVersion(15)]
    public bool unkBool1;
    [MinVersion(7), MaxVersion(7)]
    public bool unkBool2;
    [MaxVersion(10)]
    public uint unkInt1;

    [MaxVersion(14)]
    public Sphere sphere = new();

    [MaxVersion(7)]
    public bool unkBool;
    [MaxVersion(14)]
    public float unkFloat;

    [MaxVersion(2)]
    public MiloLib.Classes.Vector3 unkVector3 = new();

    [MinVersion(5), MaxVersion(23)]
    public List<(ushort s1, ushort s2)> legacyFacePairs = new();

    public List<LegacyPatchGroup> oldPatchGroups = new();

    [MaxVersion(3)]
    public List<List<ushort>> oldUshortVecs = new();

    [MaxVersion(0)]
    public bool rev0Bool;
    [MaxVersion(0)]
    public int rev0Int0;
    [MaxVersion(0)]
    public int rev0Int1;
    [MaxVersion(0)]
    public int rev0Int2;
    [MaxVersion(0)]
    public int rev0Int3;

    public List<GroupSection> groupSections = new();
    public bool hasGroupSections = true;

    private bool vertexBlockIsChunked;
    private int vertexBlockChunkInfo;
    private byte? revision37TailByte;
    public int oldDepthMode;
    public int oldDepthCompare;
    public Symbol oldMeshString = Symbol.Empty;
    public int oldMeshValue;

    private bool UsesKaraokeMesh(DirectoryMeta parent)
    {
        return parent.revision == 10 && revision == 17;
    }

    public List<StriperResult> striperResults = new();

    public class StriperResult
    {
        public int nbStrips;
        public int totalRuns;
        public int[] stripLengths = Array.Empty<int>();
        public ushort[] stripRuns = Array.Empty<ushort>();

        public StriperResult Read(EndianReader reader)
        {
            nbStrips = reader.ReadInt32();
            totalRuns = reader.ReadInt32();
            stripLengths = new int[nbStrips];
            for (int i = 0; i < nbStrips; i++)
            {
                stripLengths[i] = reader.ReadInt32();
            }

            stripRuns = new ushort[totalRuns];
            for (int i = 0; i < totalRuns; i++)
            {
                stripRuns[i] = reader.ReadUInt16();
            }

            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32(nbStrips);
            writer.WriteInt32(totalRuns);
            for (int i = 0; i < nbStrips; i++)
            {
                writer.WriteInt32(stripLengths[i]);
            }

            for (int i = 0; i < totalRuns; i++)
            {
                writer.WriteUInt16(stripRuns[i]);
            }
        }
    }

    private bool HasSelfGeometryOwner(DirectoryMeta.Entry? entry)
    {
        string geomOwnerName = geomOwner.value;
        if (string.IsNullOrEmpty(geomOwnerName))
        {
            return true;
        }

        return entry != null && geomOwnerName == entry.name.value;
    }

    private static bool UsesLegacyVertexLayout(DirectoryMeta parent)
    {
        // Older console vertices omit the vector padding and extra tangent fields used on Xbox 360 and PS3.
        return parent.platform == DirectoryMeta.Platform.PS2 || parent.platform == DirectoryMeta.Platform.Wii || parent.platform == DirectoryMeta.Platform.GameCube;
    }

    private bool ShouldSerializeStriperResults(DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        return altRevision > 5
            && parent.platform == DirectoryMeta.Platform.Wii
            && HasSelfGeometryOwner(entry)
            && faces.Count != 0
            && vertices.vertices.Count != 0
            && (mutable & Mutable.kMutableFaces) == 0;
    }

    public new RndMesh Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        revision37TailByte = null;
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);


        trans = trans.Read(reader, false, parent, entry);
        draw = draw.Read(reader, false, parent, entry);

        if (UsesKaraokeMesh(parent))
        {
            oldDepthMode = reader.ReadInt32();
            oldDepthCompare = reader.ReadInt32();
        }

        mat = Symbol.Read(reader);

        if (revision == 27)
        {
            mat2 = Symbol.Read(reader);
        }

        geomOwner = Symbol.Read(reader);

        if (revision < 13)
        {
            altGeomOwner = Symbol.Read(reader);
        }

        if (revision < 15)
        {
            transParent = Symbol.Read(reader);
        }

        if (revision < 14)
        {
            unkTransReference1 = Symbol.Read(reader);
            unkTransReference2 = Symbol.Read(reader);
        }

        if (revision < 3)
        {
            unkVector3 = unkVector3.Read(reader);
        }

        if (revision < 15)
        {
            sphere = sphere.Read(reader);
        }

        if (revision < 8)
        {
            unkBool = reader.ReadBoolean();
        }

        if (revision < 15)
        {
            unkSym1 = Symbol.Read(reader);
            unkFloat = reader.ReadFloat();
        }



        if (revision < 16)
        {
            if (revision > 11)
            {
                unkBool1 = reader.ReadBoolean();
            }
        }
        else
        {
            if (UsesKaraokeMesh(parent))
            {
                oldMeshString = Symbol.Read(reader);
                oldMeshValue = reader.ReadInt32();
            }
            mutable = (Mutable)reader.ReadUInt32();
        }

        if (revision > 17)
        {
            volume = (Volume)reader.ReadUInt32();
        }

        if (revision > 18 && !(parent.revision == 26 && revision == 30))
        {
            bspNode = bspNode.Read(reader);
        }

        if (revision == 7)
        {
            unkBool2 = reader.ReadBoolean();
        }

        if (revision < 11)
        {
            unkInt1 = reader.ReadUInt32();
        }

        if (altRevision > 4)
        {
            int firstInt = reader.ReadInt32();
            int blockSize;
            if (firstInt == -1)
            {
                vertexBlockIsChunked = true;
                vertexBlockChunkInfo = reader.ReadInt32();
                blockSize = reader.ReadInt32();
            }
            else
            {
                blockSize = firstInt;
            }
            long blockStart = reader.BaseStream.Position;
            Endian origEndian = reader.Endianness;
            reader.Endianness = Endian.LittleEndian;
            vertices = vertices.Read(reader, revision, UsesLegacyVertexLayout(parent), parent.revision == 26 && revision == 30);
            reader.Endianness = origEndian;
            reader.BaseStream.Position = blockStart + blockSize;
        }
        else
        {
            vertices = vertices.Read(reader, revision, UsesLegacyVertexLayout(parent), parent.revision == 26 && revision == 30);
        }

        uint faceCount = reader.ReadUInt32();
        if (faceCount > (reader.BaseStream.Length - reader.BaseStream.Position) / 6)
        {
            throw new InvalidDataException("The mesh face count exceeds the remaining stream.");
        }
        faces = new List<Face>((int)faceCount);
        EndianSpanReader faceReader = reader.CreateSpanReader((int)faceCount * 6);
        for (int i = 0; i < faceCount; i++)
        {
            Face face = new();
            face.idx1 = faceReader.ReadUInt16();
            face.idx2 = faceReader.ReadUInt16();
            face.idx3 = faceReader.ReadUInt16();
            faces.Add(face);
        }
        reader.Advance(faceReader.Position);

        if (revision >= 5 && revision <= 23)
        {
            int pairCount = reader.ReadInt32();
            legacyFacePairs = new List<(ushort, ushort)>(pairCount);
            for (int i = 0; i < pairCount; i++)
            {
                ushort s1 = reader.ReadUInt16();
                ushort s2 = reader.ReadUInt16();
                legacyFacePairs.Add((s1, s2));
            }
        }

        if (revision > 0x17)
        {
            groupSizesCount = reader.ReadUInt32();
            groupSizes = new List<byte>(reader.ReadBlock((int)groupSizesCount));
        }
        else if (revision > 0x15)
        {
            int patchCount = reader.ReadInt32();
            oldPatchGroups = new List<LegacyPatchGroup>();
            groupSizes = new List<byte>();
            for (int i = 0; i < patchCount; i++)
            {
                var pg = new LegacyPatchGroup().Read(reader);
                oldPatchGroups.Add(pg);
                groupSizes.Add((byte)pg.patchValue);
            }
            groupSizesCount = (uint)groupSizes.Count;
        }
        else if (revision > 0x10)
        {
            // todo double check this
            groupSizesCount = reader.ReadUInt32();
            groupSizes = new List<byte>(reader.ReadBlock((int)groupSizesCount));
        }



        if (reader.ReadInt32() > 0)
        {
            reader.BaseStream.Position -= 4;
            if (revision >= 33)
            {
                boneTransforms = AssetHelpers.ReadList(reader, () => new BoneTransform().Read(reader, revision));
            }
            else
            {
                int legacyBoneCount = 4;
                if (UsesKaraokeMesh(parent))
                {
                    legacyBoneCount = 2;
                }

                boneTransforms = new List<BoneTransform>(legacyBoneCount);
                for (int i = 0; i < legacyBoneCount; i++)
                {
                    boneTransforms.Add(new BoneTransform());
                    boneTransforms[i].name = Symbol.Read(reader);
                }
                for (int i = 0; i < legacyBoneCount; i++)
                {
                    boneTransforms[i].transform = boneTransforms[i].transform.Read(reader);
                }
            }
        }

        if (ShouldSerializeStriperResults(parent, entry))
        {
            striperResults = new List<StriperResult>();
            for (int i = 0; i < groupSizes.Count; i++)
            {
                striperResults.Add(new StriperResult().Read(reader));
            }
        }

        if (revision != 0 && revision < 4)
        {
            oldUshortVecs = AssetHelpers.ReadList(reader, () => AssetHelpers.ReadList(reader, () => reader.ReadUInt16()));
        }

        if (revision == 0)
        {
            rev0Bool = reader.ReadBoolean();
            rev0Int0 = reader.ReadInt32();
            rev0Int1 = reader.ReadInt32();
            rev0Int2 = reader.ReadInt32();
            rev0Int3 = reader.ReadInt32();
        }


        if (revision > 34)
        {
            keepMeshData = reader.ReadBoolean();
        }

        if (revision > 0x25)
        {
            hasAOCalculation = reader.ReadBoolean();
        }

        if (altRevision > 1)
        {
            noQuant = reader.ReadBoolean();
        }

        if (altRevision > 3)
        {
            unkBool3 = reader.ReadBoolean();
        }

        // weird thing on last-gen, from Cisco's notes
        if (groupSizesCount > 0 && groupSizes[0] > 0 && (parent.revision < 25 || parent.platform == DirectoryMeta.Platform.PS2))
        {
            hasGroupSections = true;
            if (standalone && AssetHelpers.IsEndBytes(reader))
            {
                hasGroupSections = false;
            }

            if (hasGroupSections)
            {
                for (int i = 0; i < groupSizesCount; i++)
                {
                    GroupSection section = new();
                    groupSections.Add(section.Read(reader, revision));
                }
            }
        }
        if (parent.revision == 25 && revision == 37 && !AssetHelpers.IsEndBytes(reader) && reader.BaseStream.Length - reader.BaseStream.Position >= 5)
        {
            // Green Day meshes have one more tail byte. Accept it only at the exact object boundary.
            long position = reader.BaseStream.Position;
            byte value = reader.ReadByte();
            if (AssetHelpers.IsEndBytes(reader))
            {
                revision37TailByte = value;
            }
            else
            {
                reader.BaseStream.Position = position;
            }
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
        draw.Write(writer, false, parent, null);
        if (UsesKaraokeMesh(parent))
        {
            writer.WriteInt32(oldDepthMode);
            writer.WriteInt32(oldDepthCompare);
        }
        Symbol.Write(writer, mat);

        if (revision == 27)
        {
            Symbol.Write(writer, mat2);
        }

        Symbol.Write(writer, geomOwner);

        if (revision < 13)
        {
            Symbol.Write(writer, altGeomOwner);
        }

        if (revision < 15)
        {
            Symbol.Write(writer, transParent);
        }

        if (revision < 14)
        {
            Symbol.Write(writer, unkTransReference1);
            Symbol.Write(writer, unkTransReference2);
        }

        if (revision < 3)
        {
            unkVector3.Write(writer);
        }

        if (revision < 15)
        {
            sphere.Write(writer);
        }

        if (revision < 8)
        {
            writer.WriteBoolean(unkBool);
        }

        if (revision < 15)
        {
            Symbol.Write(writer, unkSym1);
            writer.WriteFloat(unkFloat);
        }
        if (revision < 16)
        {
            if (revision > 11)
            {
                writer.WriteBoolean(unkBool1);
            }
        }
        else
        {
            if (UsesKaraokeMesh(parent))
            {
                Symbol.Write(writer, oldMeshString);
                writer.WriteInt32(oldMeshValue);
            }
            writer.WriteUInt32((uint)mutable);
        }

        if (revision > 17)
        {
            writer.WriteUInt32((uint)volume);
        }

        if (revision > 18 && !(parent.revision == 26 && revision == 30))
        {
            bspNode.Write(writer);
        }

        if (revision == 7)
        {
            writer.WriteBoolean(unkBool2);
        }

        if (revision < 11)
        {
            writer.WriteUInt32(unkInt1);
        }

        if (altRevision > 4)
        {
            if (vertexBlockIsChunked)
            {
                writer.WriteInt32(-1);
                writer.WriteInt32(vertexBlockChunkInfo);
            }
            long sizePos = writer.BaseStream.Position;
            writer.WriteInt32(0); // placeholder for block size
            long dataStart = writer.BaseStream.Position;
            Endian origEndian = writer.Endianness;
            writer.Endianness = Endian.LittleEndian;
            vertices.Write(writer, revision, UsesLegacyVertexLayout(parent), parent.revision == 26 && revision == 30);
            writer.Endianness = origEndian;
            long dataEnd = writer.BaseStream.Position;
            int blockSize = (int)(dataEnd - dataStart);

            if (vertexBlockIsChunked && vertexBlockChunkInfo > 0)
            {
                writer.MarkChunks(dataStart, dataEnd, AssetHelpers.FileLoaderPieceSize);
            }

            writer.BaseStream.Position = sizePos;
            writer.WriteInt32(blockSize);
            writer.BaseStream.Position = dataEnd;
        }
        else
        {
            vertices.Write(writer, revision, UsesLegacyVertexLayout(parent), parent.revision == 26 && revision == 30);
        }

        AssetHelpers.WriteList(writer, faces, (Face face) => face.Write(writer));

        if (revision >= 5 && revision <= 23)
        {
            writer.WriteInt32(legacyFacePairs.Count);
            foreach (var pair in legacyFacePairs)
            {
                writer.WriteUInt16(pair.s1);
                writer.WriteUInt16(pair.s2);
            }
        }

        if (revision > 0x17)
        {
            groupSizesCount = (uint)groupSizes.Count;
            writer.WriteUInt32(groupSizesCount);
            writer.WriteBlock(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(groupSizes));
        }
        else if (revision > 0x15)
        {
            AssetHelpers.WriteList(writer, oldPatchGroups, pg => pg.Write(writer));
        }
        else if (revision > 0x10)
        {
            groupSizesCount = (uint)groupSizes.Count;
            writer.WriteUInt32(groupSizesCount);
            writer.WriteBlock(System.Runtime.InteropServices.CollectionsMarshal.AsSpan(groupSizes));
        }


        if (revision >= 33)
        {
            AssetHelpers.WriteList(writer, boneTransforms, (BoneTransform boneTransform) => boneTransform.Write(writer, revision));
        }
        else
        {

            if (boneTransforms.Count > 0)
            {
                int legacyBoneCount = 4;
                if (UsesKaraokeMesh(parent))
                {
                    legacyBoneCount = 2;
                }

                while (boneTransforms.Count < legacyBoneCount)
                {
                    boneTransforms.Add(new BoneTransform());
                }

                for (int i = 0; i < legacyBoneCount; i++)
                {
                    Symbol.Write(writer, boneTransforms[i].name);
                }
                for (int i = 0; i < legacyBoneCount; i++)
                {
                    boneTransforms[i].transform.Write(writer);
                }
            }
            else
            {
                writer.WriteUInt32(0);
            }
        }

        if (ShouldSerializeStriperResults(parent, entry))
        {
            foreach (var sr in striperResults)
            {
                sr.Write(writer);
            }
        }

        if (revision != 0 && revision < 4)
        {
            AssetHelpers.WriteList(writer, oldUshortVecs, inner => AssetHelpers.WriteList(writer, inner, s => writer.WriteUInt16(s)));
        }

        if (revision == 0)
        {
            writer.WriteBoolean(rev0Bool);
            writer.WriteInt32(rev0Int0);
            writer.WriteInt32(rev0Int1);
            writer.WriteInt32(rev0Int2);
            writer.WriteInt32(rev0Int3);
        }

        if (revision > 34)
        {
            writer.WriteBoolean(keepMeshData);
        }

        if (revision > 0x25)
        {
            writer.WriteBoolean(hasAOCalculation);
        }

        if (altRevision > 1)
        {
            writer.WriteBoolean(noQuant);
        }

        if (altRevision > 3)
        {
            writer.WriteBoolean(unkBool3);
        }

        if (hasGroupSections && groupSizesCount > 0 && groupSizes.Count > 0 && groupSizes[0] > 0 && (parent.revision < 25 || parent.platform == DirectoryMeta.Platform.PS2))
        {
            while (groupSections.Count < groupSizesCount)
            {
                groupSections.Add(new GroupSection());
            }
            for (int i = 0; i < groupSizesCount; i++)
            {
                groupSections[i].Write(writer, revision);
            }
        }

        if (parent.revision == 25 && revision == 37 && revision37TailByte.HasValue)
        {
            writer.WriteByte(revision37TailByte.Value);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public static RndMesh New(ushort revision, ushort altRevision, uint vertexCount, uint faceCount)
    {
        RndMesh newRndMesh = new();
        newRndMesh.revision = revision;
        newRndMesh.altRevision = altRevision;
        return newRndMesh;
    }

}
