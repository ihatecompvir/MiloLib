using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandConfiguration"), Description("Band Configuration, sits in each venue and stores xfms per target per play mode")]
public class BandConfiguration : Object
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        { Game.MiloGame.RockBand3, 3 },
    };

    private ushort altRevision;
    private ushort revision;

    private uint targTransformCount;
    public List<TargTransform> transforms = new();

    public class TargTransform
    {
        public Symbol target = Symbol.Empty;
        public Matrix xfm = new();

        public TargTransform Read(EndianReader reader)
        {
            target = Symbol.Read(reader);
            xfm = new Matrix().Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, target);
            xfm.Write(writer);
        }

        public override string ToString()
        {
            return $"{target} {xfm}";
        }
    }

    public new BandConfiguration Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        targTransformCount = reader.ReadUInt32();
        // read 4 targtransforms per count
        for (int i = 0; i < targTransformCount * 4; i++)
        {
            TargTransform transform = new();
            transform.Read(reader);
            transforms.Add(transform);
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

        // write targ transforms to match read
        targTransformCount = (uint)(transforms.Count / 4);
        writer.WriteUInt32(targTransformCount);
        foreach (TargTransform transform in transforms)
        {
            transform.Write(writer);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
