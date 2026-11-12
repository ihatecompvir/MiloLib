using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("OutfitLoader"), Description("Loads new outfits into a character by merging, much like a milo file merge.")]
public class OutfitLoader : Object
{
    public class OutfitState
    {
        [MaxVersion(1)]
        public sbyte oldIndex;

        public sbyte excluded;
        public sbyte hidden;
    }

    public class CategoryState
    {
        [Name("Selected Outfit"), Description("Selected outfit")]
        public sbyte selectedOutfit;
        [Name("Shown Outfit"), Description("Shown outfit")]
        public sbyte shownOutfit;

        [MinVersion(2), MaxVersion(2)]
        public Symbol oldName = Symbol.Empty;

        public List<OutfitState> outfits = new();
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Directory"), Description("Directory to look in.")]
    public Symbol directory = Symbol.Empty;

    [Name("Categories"), Description("All the outfit categories")]
    public List<CategoryState> categories = new();

    public new OutfitLoader Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        base.Read(reader, false, parent, entry);
        directory = Symbol.Read(reader);
        short categoryCount = reader.ReadInt16();
        categories = new List<CategoryState>(categoryCount);

        for (int categoryIndex = 0; categoryIndex < categoryCount; categoryIndex++)
        {
            CategoryState category = new();
            category.selectedOutfit = reader.ReadSByte();
            category.shownOutfit = reader.ReadSByte();
            if (revision == 2)
            {
                category.oldName = Symbol.Read(reader);
            }
            short outfitCount = reader.ReadInt16();
            category.outfits = new List<OutfitState>(outfitCount);
            for (int outfitIndex = 0; outfitIndex < outfitCount; outfitIndex++)
            {
                OutfitState outfit = new();
                if (revision < 2)
                {
                    outfit.oldIndex = reader.ReadSByte();
                }
                outfit.excluded = reader.ReadSByte();
                outfit.hidden = reader.ReadSByte();
                category.outfits.Add(outfit);
            }
            categories.Add(category);
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, directory);
        writer.WriteInt16((short)categories.Count);

        foreach (CategoryState category in categories)
        {
            writer.WriteSByte(category.selectedOutfit);
            writer.WriteSByte(category.shownOutfit);
            if (revision == 2)
            {
                Symbol.Write(writer, category.oldName);
            }
            writer.WriteInt16((short)category.outfits.Count);
            foreach (OutfitState outfit in category.outfits)
            {
                if (revision < 2)
                {
                    writer.WriteSByte(outfit.oldIndex);
                }
                writer.WriteSByte(outfit.excluded);
                writer.WriteSByte(outfit.hidden);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
