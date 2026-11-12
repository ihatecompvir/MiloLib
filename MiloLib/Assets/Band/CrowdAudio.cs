using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("CrowdAudio"), Description("Crowd Audio, makes the crowd cheer and such")]
public class CrowdAudio : Object
{
    private ushort altRevision;
    private ushort revision;

    public Object poll = new();

    [MinVersion(3), MaxVersion(4)]
    public int oldInt1;
    [MinVersion(4), MaxVersion(4)]
    public int oldInt2;
    [MinVersion(4), MaxVersion(4)]
    public int oldInt3;
    [MinVersion(2), MaxVersion(2)]
    public Symbol oldString = Symbol.Empty;

    public new CrowdAudio Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        poll = poll.Read(reader, false, parent, entry);

        if (revision == 4)
        {
            oldInt1 = reader.ReadInt32();
            oldInt2 = reader.ReadInt32();
            oldInt3 = reader.ReadInt32();
        }
        else if (revision == 3)
        {
            oldInt1 = reader.ReadInt32();
        }
        else if (revision == 2)
        {
            oldString = Symbol.Read(reader);
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

        poll.Write(writer, false, parent, entry);

        if (revision == 4)
        {
            writer.WriteInt32(oldInt1);
            writer.WriteInt32(oldInt2);
            writer.WriteInt32(oldInt3);
        }
        else if (revision == 3)
        {
            writer.WriteInt32(oldInt1);
        }
        else if (revision == 2)
        {
            Symbol.Write(writer, oldString);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
