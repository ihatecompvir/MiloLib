using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamButton"), Description("Button with app-specific features")]
public class HamButton : UIButton
{
    private ushort revision = 23;
    private ushort altRevision;
    public Symbol oldAnim1 = Symbol.Empty;
    public Symbol oldAnim2 = Symbol.Empty;
    public Symbol oldAnim3 = Symbol.Empty;
    [Name("Is Toggle Button"), Description("True if this button is a toggle that doesn't advance a screen")]
    public bool isToggleButton;

    public new HamButton Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 23);
        if (revision < 19)
        {
            throw new NotSupportedException($"Unsupported HamButton revision {revision}; minimum supported revision is 19.");
        }
        if (revision == 19 || revision == 20)
        {
            oldAnim1 = Symbol.Read(reader);
            oldAnim2 = Symbol.Read(reader);
        }
        if (revision == 20)
        {
            oldAnim3 = Symbol.Read(reader);
        }
        if (revision >= 23)
        {
            isToggleButton = reader.ReadBoolean();
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
        if (revision < 19)
        {
            throw new NotSupportedException($"Unsupported HamButton revision {revision}; minimum supported revision is 19.");
        }
        AssetHelpers.WriteRevision(writer, revision, altRevision, 23);
        if (revision == 19 || revision == 20)
        {
            Symbol.Write(writer, oldAnim1);
            Symbol.Write(writer, oldAnim2);
        }
        if (revision == 20)
        {
            Symbol.Write(writer, oldAnim3);
        }
        if (revision >= 23)
        {
            writer.WriteBoolean(isToggleButton);
        }
        base.Write(writer, false, parent, entry);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
