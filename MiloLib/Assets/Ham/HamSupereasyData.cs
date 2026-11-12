using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamSupereasyData"), Description("Moves for a super easy dance routine")]
public class HamSupereasyData : Object
{

    private ushort altRevision;
    private ushort revision;

    public class HamSupereasyMeasure
    {
        [Name("First"), Description("MoveVariant to use for transition into measure")]
        Symbol first = Symbol.Empty;
        [Name("Second"), Description("MoveVariant to use for transition out of measure")]
        Symbol second = Symbol.Empty;
        [Name("Preferred"), Description("Preferred MoveVariant for this measure")]
        Symbol preferred = Symbol.Empty;

        public HamSupereasyMeasure Read(EndianReader reader)
        {
            first = Symbol.Read(reader);
            second = Symbol.Read(reader);
            preferred = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, first);
            Symbol.Write(writer, second);
            Symbol.Write(writer, preferred);
        }
    }

    [Name("Routine"), Description("Routine for Supereasy difficulty.")]
    public List<HamSupereasyMeasure> routine = new();

    public new HamSupereasyData Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        routine = AssetHelpers.ReadList(reader, () => new HamSupereasyMeasure().Read(reader));

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

        AssetHelpers.WriteList(writer, routine, measure => measure.Write(writer));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
