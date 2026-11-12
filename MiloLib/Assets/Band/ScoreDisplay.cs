using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("ScoreDisplay"), Description("Score Display")]
public class ScoreDisplay : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    public short unk114;
    [Name("Score"), Description("Score")]
    public int score;
    [Name("Text Color"), MinVersion(1)]
    public Symbol textColor = Symbol.Empty;
    [Name("Rank"), Description("Rank"), MinVersion(2)]
    public int rank;
    [Name("Globally"), Description("Globally? (vs Friends)"), MinVersion(2)]
    public bool globally;

    public new ScoreDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        unk114 = reader.ReadInt16();
        score = reader.ReadInt32();
        if (revision >= 1)
        {
            textColor = Symbol.Read(reader);
        }

        if (revision >= 2)
        {
            rank = reader.ReadInt32();
            globally = reader.ReadBoolean();
        }

        base.Read(reader, false, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        writer.WriteInt16(unk114);
        writer.WriteInt32(score);
        if (revision >= 1)
        {
            Symbol.Write(writer, textColor);
        }

        if (revision >= 2)
        {
            writer.WriteInt32(rank);
            writer.WriteBoolean(globally);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
