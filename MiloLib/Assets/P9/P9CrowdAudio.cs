using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.P9;

[Name("P9CrowdAudio"), Description("P9 Crowd Audio, makes the crowd cheer and such")]
public class P9CrowdAudio : Object
{
    private ushort revision = 5;
    private ushort altRevision;

    public ObjectFields pollObjectFields = new();
    [MinVersion(2), MaxVersion(2)]
    public Symbol oldString = Symbol.Empty;
    [MinVersion(3), MaxVersion(3)]
    public uint oldRevision3Value;
    [MinVersion(4), MaxVersion(4)]
    public uint oldRevision4Value1;
    [MinVersion(4), MaxVersion(4)]
    public uint oldRevision4Value2;
    [MinVersion(4), MaxVersion(4)]
    public uint oldRevision4Value3;

    public new P9CrowdAudio Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 5);
        base.Read(reader, false, parent, entry);
        pollObjectFields.Read(reader, parent, entry);
        if (revision == 2)
        {
            oldString = Symbol.Read(reader);
        }

        if (revision == 3)
        {
            oldRevision3Value = reader.ReadUInt32();
        }

        if (revision == 4)
        {
            oldRevision4Value1 = reader.ReadUInt32();
            oldRevision4Value2 = reader.ReadUInt32();
            oldRevision4Value3 = reader.ReadUInt32();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 5);
        base.Write(writer, false, parent, entry);
        pollObjectFields.Write(writer, parent);
        if (revision == 2)
        {
            Symbol.Write(writer, oldString);
        }

        if (revision == 3)
        {
            writer.WriteUInt32(oldRevision3Value);
        }

        if (revision == 4)
        {
            writer.WriteUInt32(oldRevision4Value1);
            writer.WriteUInt32(oldRevision4Value2);
            writer.WriteUInt32(oldRevision4Value3);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
