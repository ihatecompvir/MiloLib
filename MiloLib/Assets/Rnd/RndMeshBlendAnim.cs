using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("MeshBlendAnim")]
public class RndMeshBlendAnim : Object
{
    [HideInInspector]
    private ushort revision = 1;
    [HideInInspector]
    private ushort altRevision;

    public class BlendWeight
    {
        [Name("Index")]
        public int index;
        [Name("Weight")]
        public float weight;

        public BlendWeight Read(EndianReader reader, ushort revision)
        {
            index = reader.ReadInt32();
            weight = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            writer.WriteInt32(index);
            writer.WriteFloat(weight);
        }
    }
    public class Keyframe
    {
        [Name("Frame")]
        public float frame;
        [Name("Weights")]
        public List<BlendWeight> weights = new();

        public Keyframe Read(EndianReader reader, ushort revision)
        {
            frame = reader.ReadFloat();
            weights = AssetHelpers.ReadList(reader, () => new BlendWeight().Read(reader, revision));
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            writer.WriteFloat(frame);
            AssetHelpers.WriteList(writer, weights, value => value.Write(writer, revision));
        }
    }

    [Name("Target"), Description("The mesh blend we will drive")]
    public Symbol target = Symbol.Empty;
    [Name("Keys Owner")]
    public Symbol keysOwner = Symbol.Empty;
    [Name("Keyframes")]
    public List<Keyframe> keyframes = new();

    public new RndMeshBlendAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);

        target = Symbol.Read(reader);
        keysOwner = Symbol.Read(reader);
        keyframes = AssetHelpers.ReadList(reader, () => new Keyframe().Read(reader, revision));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);

        Symbol.Write(writer, target);
        Symbol.Write(writer, keysOwner);
        AssetHelpers.WriteList(writer, keyframes, value => value.Write(writer, revision));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
