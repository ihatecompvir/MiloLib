using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CollideableSet"), Description("A CollideableSet contains an array of Collideable objects attached to base Trans objects that define the world frame of the Collideables.")]
public class CollideableSet : Object
{
    public class CollisionEntry
    {
        [Name("Collider"), Description("Collideable object.")]
        public Symbol collider = Symbol.Empty;
        [Name("Bone"), Description("Bone that defines the world frame of this Collideable.")]
        public Symbol bone = Symbol.Empty;
        [Name("Mesh"), Description("Mesh that defines the initial shape of the Collideable. Also used at runtime to setup polling dependencies and check bone hieararchies.")]
        public Symbol mesh = Symbol.Empty;

        public CollisionEntry Read(EndianReader reader)
        {
            collider = Symbol.Read(reader);
            bone = Symbol.Read(reader);
            mesh = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, collider);
            Symbol.Write(writer, bone);
            Symbol.Write(writer, mesh);
        }
    }

    private ushort revision = 1;
    private ushort altRevision;
    [Name("Colliders")]
    public List<CollisionEntry> colliders = new();

    public new CollideableSet Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        // seems that collideable shapes don't serialize the Object
        colliders = AssetHelpers.ReadList(reader, () => new CollisionEntry().Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        AssetHelpers.WriteList(writer, colliders, collider => collider.Write(writer));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
