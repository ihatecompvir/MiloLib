using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("PollAnim"), Description("Class that drives Anims with time based on their rate.")]
public class RndPollAnim : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();
    public Object poll = new();

    [Name("Anims"), Description("List of anims that will have SetFrame called on them according to their rate and the TheTaskMgr.Seconds or Beat")]
    public List<Symbol> anims = new();

    public new RndPollAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        anim = anim.Read(reader, parent, entry);
        poll = poll.Read(reader, false, parent, entry);

        anims = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

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

        anim.Write(writer);
        poll.Write(writer, false, parent, entry);

        AssetHelpers.WriteList(writer, anims, (Symbol anim) => Symbol.Write(writer, anim));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
