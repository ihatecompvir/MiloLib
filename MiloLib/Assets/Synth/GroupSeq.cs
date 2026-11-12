using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("GroupSeq"), Description("A sequence which plays other sequences. Abstract base class.")]
public class GroupSeq : Object
{
    private ushort altRevision;
    private ushort revision;

    [MinVersion(2)]
    public Sfx.Sequence seq = new();


    [Name("Children"), Description("The children of this sequence"), MinVersion(2)]
    public List<Symbol> children = new();

    [MaxVersion(2)]
    public List<(Symbol, int)> childrenOld = new();

    public new GroupSeq Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision >= 2)
        {
            seq.Read(reader, parent, entry);
        }

        if (revision < 3)
        {
            childrenOld = AssetHelpers.ReadList(reader, () => (Symbol.Read(reader), reader.ReadInt32()));
        }
        else
        {
            children = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
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

        if (revision >= 2)
        {
            seq.Write(writer, false, parent, entry);
        }
        if (revision < 3)
        {
            AssetHelpers.WriteList(writer, childrenOld, child =>
            {
                Symbol.Write(writer, child.Item1);
                writer.WriteInt32(child.Item2);
            });
        }
        else
        {
            AssetHelpers.WriteList(writer, children, child => Symbol.Write(writer, child));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
