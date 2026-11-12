using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("PlayerPortrait"), Description("Displays player portrait and info")]
public class PlayerPortrait : UIComponent
{
    private ushort revision = 1;
    private ushort altRevision;
    [Name("Resource File")]
    public Symbol resourceFile = Symbol.Empty;
    [Name("Level"), Description("The player's current level")]
    public int level;
    [Name("Portrait Index"), Description("This player's id for his picture")]
    public int portraitIndex;

    public new PlayerPortrait Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        if (revision != 1)
        {
            throw new NotSupportedException($"Unsupported PlayerPortrait revision {revision}.");
        }

        resourceFile = Symbol.Read(reader);
        level = reader.ReadInt32();
        portraitIndex = reader.ReadInt32();
        base.Read(reader, false, parent, entry);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (revision != 1)
        {
            throw new NotSupportedException($"Unsupported PlayerPortrait revision {revision}.");
        }

        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        Symbol.Write(writer, resourceFile);
        writer.WriteInt32(level);
        writer.WriteInt32(portraitIndex);
        base.Write(writer, false, parent, entry);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

