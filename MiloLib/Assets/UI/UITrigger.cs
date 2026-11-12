using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UITrigger"), Description("Triggers anims based on UI events (enter, exit, etc.)")]
public class UITrigger : EventTrigger
{
    private ushort altRevision;
    private ushort revision;

    [MaxVersion(0)]
    public UIComponent component = new();

    [MaxVersion(0)]
    public Symbol unkSym = Symbol.Empty;

    [MaxVersion(0)]
    public Symbol animRef = Symbol.Empty;

    [Name("Block Transition"), Description("Block enter/exit transition during animation?")]
    public bool blockTransition;

    public new UITrigger Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 1)
        {
            component = component.Read(reader, false, parent, entry);

            unkSym = Symbol.Read(reader);

            animRef = Symbol.Read(reader);
        }
        else
        {
            base.Read(reader, false, parent, entry);
        }
        blockTransition = reader.ReadBoolean();

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision < 1)
        {
            component.Write(writer, false, parent, entry);

            Symbol.Write(writer, unkSym);

            Symbol.Write(writer, animRef);
        }
        else
        {
            base.Write(writer, false, parent, entry);
        }
        writer.WriteBoolean(blockTransition);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
