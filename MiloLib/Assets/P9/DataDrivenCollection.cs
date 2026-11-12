using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.P9;

[Name("DataDrivenCollection"), Description("allows the user to define lists of assets, give those lists titles, and define any effect they have on the metagame without the aid of an engineer.")]
public class DataDrivenCollection : Object
{
    public class CollectionAsset
    {
        [Name("Asset Type"), Description("What type of asset is this")]
        public Symbol assetType = Symbol.Empty;
        [Name("Asset Name"), Description("Name of this asset")]
        public Symbol assetName = Symbol.Empty;

        public CollectionAsset Read(EndianReader reader, ushort revision)
        {
            if (revision > 0)
            {
                assetType = Symbol.Read(reader);
            }
            else
            {
                assetType = "Song";
            }

            assetName = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            if (revision > 0)
            {
                Symbol.Write(writer, assetType);
            }

            Symbol.Write(writer, assetName);
        }
    }

    public class SortOrderSection
    {
        [Name("Sort Order Section Header"), Description("The header of this user defined sort order section")]
        public Symbol header = Symbol.Empty;
        [Name("Sort Order Section Assets"), Description("A list of the assets contained in this user defined sort order section.")]
        public List<Symbol> assets = new();

        public SortOrderSection Read(EndianReader reader)
        {
            header = Symbol.Read(reader);
            assets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, header);
            AssetHelpers.WriteList(writer, assets, asset => Symbol.Write(writer, asset));
        }
    }

    public class CustomSortOrder
    {
        [Name("Sort Order Name"), Description("The name of a custom sort order")]
        public Symbol name = Symbol.Empty;
        [Name("Sort Order Sections")]
        public List<SortOrderSection> sections = new();

        public CustomSortOrder Read(EndianReader reader)
        {
            name = Symbol.Read(reader);
            sections = AssetHelpers.ReadList(reader, () => new SortOrderSection().Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, name);
            AssetHelpers.WriteList(writer, sections, section => section.Write(writer));
        }
    }

    public class UnlockedByNode
    {
        [Name("Unlock Criteria Elem Name"), Description("Names an element that must already be unlocked to pass this test")]
        public Symbol elemName = Symbol.Empty;
        [Name("Unlock Criteria Credits"), Description("The amount of credits required to buy this collection")]
        public int credits;
        [Name("Unlock Criteria Number"), Description("The number of sub criteria that need to be passed for the parent test to be passed")]
        public int number;
        [Name("Unlock Criteria List"), Description("A list of elements/other unlock criteria needed for this criteria test")]
        public List<UnlockedByNode> criteria = new();

        public UnlockedByNode Read(EndianReader reader)
        {
            elemName = Symbol.Read(reader);
            credits = reader.ReadInt32();
            number = reader.ReadInt32();
            criteria = AssetHelpers.ReadList(reader, () => new UnlockedByNode().Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, elemName);
            writer.WriteInt32(credits);
            writer.WriteInt32(number);
            AssetHelpers.WriteList(writer, criteria, criterion => criterion.Write(writer));
        }
    }

    public class CompletionCriterion
    {
        [Name("Completion Criteria Songs"), Description("Songs that need to have the stats in completion_criteria_stats")]
        public List<Symbol> songs = new();
        [Name("Completion Criteria Tests"), Description("Stats that completion_criteria_songs will be tested against.")]
        public List<Symbol> tests = new();

        public CompletionCriterion Read(EndianReader reader)
        {
            songs = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            tests = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteList(writer, songs, song => Symbol.Write(writer, song));
            AssetHelpers.WriteList(writer, tests, test => Symbol.Write(writer, test));
        }
    }

    private ushort revision = 1;
    private ushort altRevision;
    [MaxVersion(0)]
    public Symbol oldString = Symbol.Empty;
    [Name("Collection Name"), Description("Name of this collection")]
    public Symbol collectionName = Symbol.Empty;
    [Name("Asset List"), Description("List of assets contained in this collection")]
    public List<CollectionAsset> assetList = new();
    [Name("User Defined Sort Orders"), Description("A list of user defined sort orders")]
    public List<CustomSortOrder> userDefinedSortOrders = new();
    [Name("Unlocked By"), Description("Specifies what elements must be previously completed/earned in order to unlock this element. Can be nested.")]
    public UnlockedByNode unlockedBy = new();
    [Name("Completion Criteria"), Description("A list of stats/song groupings needed to unlock this collection.")]
    public List<CompletionCriterion> completionCriteria = new();
    [Name("Is Always Unlocked"), Description("Do players always have access to this collection.")]
    public bool isAlwaysUnlocked;
    [Name("Preunlock Visibility"), Description("Whether or not players can see this collection before it's been unlocked")]
    public bool preunlockVisibility;

    public new DataDrivenCollection Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        if (revision == 0)
        {
            oldString = Symbol.Read(reader);
        }

        collectionName = Symbol.Read(reader);
        assetList = AssetHelpers.ReadList(reader, () => new CollectionAsset().Read(reader, revision));
        userDefinedSortOrders = AssetHelpers.ReadList(reader, () => new CustomSortOrder().Read(reader));
        unlockedBy.Read(reader);
        completionCriteria = AssetHelpers.ReadList(reader, () => new CompletionCriterion().Read(reader));
        isAlwaysUnlocked = reader.ReadBoolean();
        preunlockVisibility = reader.ReadBoolean();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        if (revision == 0)
        {
            Symbol.Write(writer, oldString);
        }

        Symbol.Write(writer, collectionName);
        AssetHelpers.WriteList(writer, assetList, asset => asset.Write(writer, revision));
        AssetHelpers.WriteList(writer, userDefinedSortOrders, sortOrder => sortOrder.Write(writer));
        unlockedBy.Write(writer);
        AssetHelpers.WriteList(writer, completionCriteria, criterion => criterion.Write(writer));
        writer.WriteBoolean(isAlwaysUnlocked);
        writer.WriteBoolean(preunlockVisibility);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
