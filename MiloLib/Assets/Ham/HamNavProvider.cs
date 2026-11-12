using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamNavProvider"), Description("List of data for HamNavList")]
public class HamNavProvider : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Nav Items")]
    public List<NavItem> navItems = new();

    public class NavItem
    {
        [Name("Label"), Description("used for a list entry with a single label")]
        public Symbol label = Symbol.Empty;
        [Name("Checkbox")]
        public int checkboxState;
        [Name("Labels"), Description("used for a list entry with a list of labels")]
        public List<Symbol> labels = new();

        public NavItem Read(EndianReader reader, ushort revision)
        {
            label = Symbol.Read(reader);
            if (revision > 0)
            {
                checkboxState = reader.ReadInt32();
            }
            if (revision >= 2)
            {
                labels = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, label);
            if (revision > 0)
            {
                writer.WriteInt32(checkboxState);
            }
            if (revision >= 2)
            {
                AssetHelpers.WriteList(writer, labels, label => Symbol.Write(writer, label));
            }
        }
    }

    public new HamNavProvider Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        navItems = AssetHelpers.ReadList(reader, () => new NavItem().Read(reader, revision));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, navItems, item => item.Write(writer, revision));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
