using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("SongSectionController"), Description("Syncronizes effects to structural song elements like verse, derived from practice sections")]
public class SongSectionController : Object
{
    public enum TriggerOrder
    {
        kPoolTriggerSequence = 0,
        kPoolTriggerRandom = 1,
        kPoolTriggerLoop = 2
    }

    public class PracticeSectionMapping
    {
        [Name("Pool Category"), Description("the name of the content pool category we are mapping practice section names to")]
        public Symbol poolCategory = Symbol.Empty;

        [Name("Valid Practice Sections"), Description("a list of valid practice section names. Can use expressions like '*verse*' to mean all sections with 'verse' in the name.")]
        public List<Symbol> validPracticeSections = new();

        [Name("Invalid Practice Sections"), Description("Any practice section appearing in this list will cause a failed match. Can use expressions like '*intro_verse*' if you like"), MinVersion(3)]
        public List<Symbol> invalidPracticeSections = new();

        public PracticeSectionMapping Read(EndianReader reader, ushort revision)
        {
            poolCategory = Symbol.Read(reader);

            validPracticeSections = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            if (revision > 2)
            {
                invalidPracticeSections = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }

            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, poolCategory);

            AssetHelpers.WriteList(writer, validPracticeSections, (Symbol practiceSection) => Symbol.Write(writer, practiceSection));

            if (revision > 2)
            {
                AssetHelpers.WriteList(writer, invalidPracticeSections, (Symbol practiceSection) => Symbol.Write(writer, practiceSection));
            }
        }
    }

    public class ContentPoolMapping
    {
        [Name("Pool Category"), Description("the name of the content pool category we are mapping practice section names to")]
        public Symbol poolCategory = Symbol.Empty;

        [Name("Trigger Order"), Description("when a category is newly activated, this rule determines how we choose which trigger_pool is next.")]
        public TriggerOrder triggerOrder;

        [Name("Trigger Pools"), Description("Pools of content associate with this pool category - they should be EventTriggers of type 'trigger_pool'. If you add more than one, then each time the category is newly activated, we go to another one in the list using the trigger_order rule below.")]
        public List<Symbol> triggerPools = new();

        public ContentPoolMapping Read(EndianReader reader)
        {
            poolCategory = Symbol.Read(reader);
            triggerOrder = (TriggerOrder)reader.ReadInt32();

            triggerPools = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, poolCategory);
            writer.WriteInt32((int)triggerOrder);
            AssetHelpers.WriteList(writer, triggerPools, (Symbol triggerPool) => Symbol.Write(writer, triggerPool));
        }
    }

    private ushort altRevision;
    private ushort revision;

    public Object pollable = new();

    [Name("Section Mappings"), Description("Define how practice section names map to content pool categories (i.e. verse_1d -> verse)")]
    public List<PracticeSectionMapping> sectionMappings = new();

    [Name("Mappings Owner"), Description("Set this to get your section mappings from another SongSectionController. Useful for moving this logic into a common subdir")]
    public Symbol mappingsOwner = Symbol.Empty;

    [Name("Pool To Activate"), Description("Select a pool here and hit the 'activate' button to test it out")]
    public Symbol debugPoolCategory = Symbol.Empty;

    [Name("Pratice Section Debug"), Description("Type in a pratice section name here, and pool_debug will show you which pool we'd activate")]
    public Symbol debugSectionName = Symbol.Empty;

    [Name("Trigger Mappings"), Description("Define which triggers get activated for each content pool category")]
    public List<ContentPoolMapping> contentPoolMappings = new();

    [Name("Wait For Event"), Description("An optional event to wait for before swapping in a new pool"), MinVersion(2)]
    public Symbol waitForEvent = Symbol.Empty;

    public new SongSectionController Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        pollable = pollable.Read(reader, false, parent, entry);

        sectionMappings = AssetHelpers.ReadList(reader, () => new PracticeSectionMapping().Read(reader, revision));

        mappingsOwner = Symbol.Read(reader);
        debugPoolCategory = Symbol.Read(reader);
        debugSectionName = Symbol.Read(reader);

        contentPoolMappings = AssetHelpers.ReadList(reader, () => new ContentPoolMapping().Read(reader));

        if (revision > 1)
        {
            waitForEvent = Symbol.Read(reader);
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
        pollable.Write(writer, false, parent, entry);

        AssetHelpers.WriteList(writer, sectionMappings, (PracticeSectionMapping sectionMapping) => sectionMapping.Write(writer, revision));

        Symbol.Write(writer, mappingsOwner);
        Symbol.Write(writer, debugPoolCategory);
        Symbol.Write(writer, debugSectionName);

        AssetHelpers.WriteList(writer, contentPoolMappings, (ContentPoolMapping contentPoolMapping) => contentPoolMapping.Write(writer));

        if (revision > 1)
        {
            Symbol.Write(writer, waitForEvent);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
