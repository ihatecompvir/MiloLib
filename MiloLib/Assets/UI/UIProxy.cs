using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIProxy"), Description("A UIProxy object allows artists to position dynamically loaded resources (e.g. a character) in Milo. The app will then load the appropriate resources into it.")]
public class UIProxy : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [MinVersion(1), MaxVersion(1)]
    private bool oldBool;
    [Name("Environ"), Description("environment to use on it"), MinVersion(3)]
    public Symbol env = Symbol.Empty;

    public new UIProxy Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision == 1)
        {
            oldBool = reader.ReadBoolean();
        }

        if (revision > 2)
        {
            env = Symbol.Read(reader);
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

        if (revision == 1)
        {
            writer.WriteBoolean(oldBool);
        }

        if (revision > 2)
        {
            Symbol.Write(writer, env);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
