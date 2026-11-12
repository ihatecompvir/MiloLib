using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharPollGroup"), Description("Group of Charpollable, polled in the order given, use when the automatic CharPollable sorting is not correct or sufficient.")]
public class CharPollGroup : Object
{
    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();

    [Name("Polls"), Description("Ordered list of CharPollables, will be polled in this order.")]
    public List<Symbol> polls = new();
    [Name("Changed By"), Description("Explicit thing I am changed by, to force sorting, if set, ignores polls"), MinVersion(2)]
    public Symbol changedBy = Symbol.Empty;
    [Name("Changes"), Description("Explicit thing I change, to force sorting, if set, ignores polls"), MinVersion(2)]
    public Symbol changes = Symbol.Empty;

    public new CharPollGroup Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision >= 3)
        {
            weightable = weightable.Read(reader, false, parent, entry);
        }

        polls = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        if (revision >= 2)
        {
            changedBy = Symbol.Read(reader);
            changes = Symbol.Read(reader);
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

        if (revision >= 3)
        {
            weightable.Write(writer, false, parent, entry);
        }

        AssetHelpers.WriteList(writer, polls, poll => Symbol.Write(writer, poll));

        if (revision >= 2)
        {
            Symbol.Write(writer, changedBy);
            Symbol.Write(writer, changes);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
