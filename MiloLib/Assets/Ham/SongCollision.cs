using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("SongCollision"), Description("Contains data for handling potential character collisions")]
public class SongCollision : Object
{
    private ushort revision;
    private ushort altRevision;
    public List<BeatCollisionData>[] data = { new(), new(), new(), new() };

    public class BeatCollisionData
    {
        public float minX;
        public float maxX;
        public Vector3 offset = new();
        public Matrix oldTransform = new();

        public BeatCollisionData Read(EndianReader reader, ushort revision)
        {
            minX = reader.ReadFloat();
            maxX = reader.ReadFloat();
            if (revision > 1)
            {
                offset.Read(reader);
            }
            else if (revision > 0)
            {
                oldTransform.Read(reader);
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            writer.WriteFloat(minX);
            writer.WriteFloat(maxX);
            if (revision > 1)
            {
                offset.Write(writer);
            }
            else if (revision > 0)
            {
                oldTransform.Write(writer);
            }
        }
    }

    public new SongCollision Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        int count = 4;
        if (altRevision < 1)
        {
            count = 3;
        }
        for (int index = 0; index < count; index++)
        {
            data[index] = AssetHelpers.ReadList(reader, () => new BeatCollisionData().Read(reader, revision));
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        base.Write(writer, false, parent, entry);
        int count = 4;
        if (altRevision < 1)
        {
            count = 3;
        }
        for (int index = 0; index < count; index++)
        {
            AssetHelpers.WriteList(writer, data[index], beat => beat.Write(writer, revision));
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
