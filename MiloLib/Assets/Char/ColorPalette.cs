using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("ColorPalette"), Description("List of primary/secondary colors for OutfitConfig")]
public class ColorPalette : Object
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        { Game.MiloGame.RockBand2, 1 },
        { Game.MiloGame.LegoRockBand, 1 },
        { Game.MiloGame.TheBeatlesRockBand, 1 },
        { Game.MiloGame.GreenDayRockBand, 1 },
        { Game.MiloGame.RockBand3, 1 },
        { Game.MiloGame.DanceCentral, 1 },

        // assumption on this TODO: double check
        { Game.MiloGame.DanceCentral2, 1 },
        { Game.MiloGame.RockBandBlitz, 1 },
        { Game.MiloGame.DanceCentral3, 1 }
    };

    private ushort altRevision;
    private ushort revision;

    private uint colorCount;

    [Name("Colors"), Description("Color for materials")]
    public List<HmxColor4> colors = new();

    public new ColorPalette Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        colorCount = reader.ReadUInt32();
        // sanity check on color count
        if (colorCount > 0x100)
        {
            throw new InvalidDataException("Color count is too high, ColorPalette is likely invalid");
        }

        for (int i = 0; i < colorCount; i++)
        {
            colors.Add(new HmxColor4().Read(reader));
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

        AssetHelpers.WriteList(writer, colors, color => color.Write(writer));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

