using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("SpotlightEnder"), Description("Object that ends spotlights, must put after the last spotlight and before anything that would render using those spotlights")]
public class SpotlightEnder : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();

    public new SpotlightEnder Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        draw = draw.Read(reader, false, parent, entry);

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

        draw.Write(writer, false, parent, null);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
