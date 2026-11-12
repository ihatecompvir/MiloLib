using MiloLib.Classes;
using MiloLib.Utils;
using System.Runtime.InteropServices;

namespace MiloLib.Assets.Band;

[Name("BandFaceDeform"), Description("Band Face Deformation object for face creator, basically a compact MeshAnim with position deltas")]
public class BandFaceDeform : Object
{
    public class DeltaArray
    {
        [Name("Size"), Description("Size in bytes this takes up")]
        private uint size;
        public List<byte> data = new();

        public DeltaArray Read(EndianReader reader)
        {
            size = reader.ReadUInt32();
            data = new List<byte>(reader.ReadBlock((int)size));

            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt32((uint)data.Count);
            writer.WriteBlock(CollectionsMarshal.AsSpan(data));
        }
    }
    private ushort altRevision;
    private ushort revision;

    [Name("Frames"), Description("the keyframes")]
    public List<DeltaArray> frames = new();

    public new BandFaceDeform Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        frames = AssetHelpers.ReadList(reader, () => new DeltaArray().Read(reader));

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

        AssetHelpers.WriteList(writer, frames, frame => frame.Write(writer));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
