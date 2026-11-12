using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharNeighborhood")]
public class CharNeighborhood : Object
{
    public class Member
    {
        [Name("Mesh")]
        public Symbol mesh = Symbol.Empty;
        [Name("Mesh Anim")]
        public Symbol meshAnim = Symbol.Empty;
        // ?
        public int unkMemberValue = 65536;

        public Member Read(EndianReader reader)
        {
            mesh = Symbol.Read(reader);
            meshAnim = Symbol.Read(reader);
            unkMemberValue = reader.ReadInt32();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, mesh);
            Symbol.Write(writer, meshAnim);
            writer.WriteInt32(unkMemberValue);
        }
    }

    private ushort revision = 3;
    private ushort altRevision;
    [Name("Members")]
    public List<Member> members = new();
    public Symbol groundMesh = Symbol.Empty;
    public int unkValue1;
    public int unkValue2;
    public int unkValue3;
    public int unkValue4;

    public new CharNeighborhood Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        if (revision != 3)
        {
            throw new NotSupportedException($"Unsupported CharNeighborhood revision {revision}.");
        }

        base.Read(reader, false, parent, entry);
        members = AssetHelpers.ReadList(reader, () => new Member().Read(reader));
        groundMesh = Symbol.Read(reader);
        unkValue1 = reader.ReadInt32();
        unkValue2 = reader.ReadInt32();
        unkValue3 = reader.ReadInt32();
        unkValue4 = reader.ReadInt32();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (revision != 3)
        {
            throw new NotSupportedException($"Unsupported CharNeighborhood revision {revision}.");
        }

        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, members, member => member.Write(writer));
        Symbol.Write(writer, groundMesh);
        writer.WriteInt32(unkValue1);
        writer.WriteInt32(unkValue2);
        writer.WriteInt32(unkValue3);
        writer.WriteInt32(unkValue4);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
