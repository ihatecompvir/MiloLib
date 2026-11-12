using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("SongDifficultyDisplay"), Description("Song Difficulty Display")]
public class SongDifficultyDisplay : UIComponent
{
    private ushort revision;
    private ushort altRevision;
    public Symbol resourceDir = Symbol.Empty;

    public new SongDifficultyDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        if (revision >= 2)
        {
            resourceDir = Symbol.Read(reader);
        }
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
        if (revision >= 2)
        {
            Symbol.Write(writer, resourceDir);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
