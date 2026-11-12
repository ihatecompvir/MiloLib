using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HollaBackMinigame"), Description("Handles logic of the Holla Back minigame")]
public class HollaBackMinigame : Object
{
    private ushort revision;
    private ushort altRevision;

    public new HollaBackMinigame Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 5);
        base.Read(reader, false, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 5);
        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
