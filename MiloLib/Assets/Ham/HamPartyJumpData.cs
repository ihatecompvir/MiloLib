using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamPartyJumpData"), Description("Handles jump points for this song in party mode")]
public class HamPartyJumpData : Object
{

    private ushort altRevision;
    private ushort revision;

    // pair<int from_measure, int to_measure>
    // "When song reaches 'from_measure', jump to start of 'to_measure'. 'from_measure' is not played."
    [Name("Jumps"), Description("Jump points for this song in party mode")]
    public List<Tuple<int, int>> jumps = new();

    public new HamPartyJumpData Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        jumps = AssetHelpers.ReadList(reader, () => new Tuple<int, int>(reader.ReadInt32(), reader.ReadInt32()));

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

        AssetHelpers.WriteList(writer, jumps, jump =>
        {
            writer.WriteInt32(jump.Item1);
            writer.WriteInt32(jump.Item2);
        });

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
