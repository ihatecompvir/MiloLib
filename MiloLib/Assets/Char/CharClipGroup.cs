using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharClipGroup"), Description("A related group of animations. Gives you the lru one. Usually no extension.")]
public class CharClipGroup : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Clips"), Description("LRU list of clips belonging to this group")]
    public List<Symbol> clips = new();

    public int which;

    [MinVersion(2)]
    public uint flags;

    // green day stores these, i dont know wtf they do
    public uint revision4Flags;
    public float revision4Value = 1.0f;
    public byte revision4Flag;

    public new CharClipGroup Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        clips = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        which = reader.ReadInt32();

        if (revision > 1)
        {
            flags = reader.ReadUInt32();
        }

        if (revision >= 4)
        {
            revision4Flags = reader.ReadUInt32();
            revision4Value = reader.ReadFloat();
            revision4Flag = reader.ReadByte();
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

        AssetHelpers.WriteList(writer, clips, clip => Symbol.Write(writer, clip));

        writer.WriteInt32(which);
        if (revision > 1)
        {
            writer.WriteUInt32(flags);
        }

        if (revision >= 4)
        {
            writer.WriteUInt32(revision4Flags);
            writer.WriteFloat(revision4Value);
            writer.WriteByte(revision4Flag);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
