using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("MeshAnim"), Description("A MeshAnim object animates Mesh vertices. This should be done only when an effect can't be achieved with material animation, because animating vertices directly is less efficient.")]
public class RndMeshAnim : Object
{
    public struct VertVec3Key
    {
        public List<Vector3> vertices;
        public float pos;

        public void Read(EndianReader reader)
        {
            uint vertCount = reader.ReadUInt32();
            vertices = new List<Vector3>((int)vertCount);
            for (int i = 0; i < vertCount; i++)
            {
                Vector3 v = new();
                v = v.Read(reader);
                vertices.Add(v);
            }
            pos = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteList(writer, vertices, v => v.Write(writer));

            writer.WriteFloat(pos);
        }
    }

    public struct VertVec2Key
    {
        public List<Vector2> vertices;
        public float pos;

        public void Read(EndianReader reader)
        {
            uint vertCount = reader.ReadUInt32();
            vertices = new List<Vector2>((int)vertCount);
            for (int i = 0; i < vertCount; i++)
            {
                Vector2 v = new();
                v = v.Read(reader);
                vertices.Add(v);
            }
            pos = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteList(writer, vertices, v => v.Write(writer));

            writer.WriteFloat(pos);
        }
    }

    public struct VertColorKey
    {
        public List<HmxColor4> colors;
        public float pos;

        public void Read(EndianReader reader)
        {
            uint colorCount = reader.ReadUInt32();
            colors = new List<HmxColor4>((int)colorCount);
            for (int i = 0; i < colorCount; i++)
            {
                HmxColor4 c = new();
                c = c.Read(reader);
                colors.Add(c);
            }
            pos = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteList(writer, colors, c => c.Write(writer));

            writer.WriteFloat(pos);
        }
    }

    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();
    public Symbol mesh = Symbol.Empty;
    public List<VertVec3Key> vertPointsKeys = new();
    [MinVersion(2)]
    public List<VertVec3Key> vertNormalsKeys = new();
    public List<VertVec2Key> vertTexsKeys = new();
    public List<VertColorKey> vertColorsKeys = new();
    public Symbol keysOwner = Symbol.Empty;

    public new RndMeshAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 0)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);

        mesh = Symbol.Read(reader);

        vertPointsKeys = AssetHelpers.ReadList(reader, () => { VertVec3Key key = new(); key.Read(reader); return key; });

        if (revision > 1)
        {
            vertNormalsKeys = AssetHelpers.ReadList(reader, () => { VertVec3Key key = new(); key.Read(reader); return key; });
        }

        vertTexsKeys = AssetHelpers.ReadList(reader, () => { VertVec2Key key = new(); key.Read(reader); return key; });

        vertColorsKeys = AssetHelpers.ReadList(reader, () => { VertColorKey key = new(); key.Read(reader); return key; });

        keysOwner = Symbol.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision != 0)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);

        Symbol.Write(writer, mesh);

        AssetHelpers.WriteList(writer, vertPointsKeys, key => key.Write(writer));

        if (revision > 1)
        {
            AssetHelpers.WriteList(writer, vertNormalsKeys, key => key.Write(writer));
        }

        AssetHelpers.WriteList(writer, vertTexsKeys, key => key.Write(writer));

        AssetHelpers.WriteList(writer, vertColorsKeys, key => key.Write(writer));

        Symbol.Write(writer, keysOwner);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
