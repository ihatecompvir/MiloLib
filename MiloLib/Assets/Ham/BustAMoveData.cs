using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("BustAMoveData"), Description("Data used to construct a Bustamove session")]
public class BustAMoveData : Object
{

    private ushort altRevision;
    private ushort revision;

    public class BAMPhrase
    {
        [Name("Count"), Description("How many times this bar phrasing repeats")]
        int count;
        [Name("Bars"), Description("How many bars per phrase")]
        int bars;

        public BAMPhrase Read(EndianReader reader)
        {
            count = reader.ReadInt32();
            bars = reader.ReadInt32();
            return this;
        }

        public BAMPhrase Write(EndianWriter writer)
        {
            writer.WriteInt32(count);
            writer.WriteInt32(bars);
            return this;
        }
    };

    [Name("Phrases"), Description("Phrases for Bustamove")]
    public List<BAMPhrase> phrases = new();

    public new BustAMoveData Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        phrases = AssetHelpers.ReadList(reader, () => new BAMPhrase().Read(reader));

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

        AssetHelpers.WriteList(writer, phrases, (BAMPhrase phrase) => phrase.Write(writer));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
