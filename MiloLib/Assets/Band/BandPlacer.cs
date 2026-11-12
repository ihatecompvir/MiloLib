using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandPlacer"), Description("Stand-in for dir in the UI, polls and draws them, can center about a mesh inside it.")]
public class BandPlacer : Object
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        // no dirs before this
        { Game.MiloGame.GuitarHero2_PS2, 2 },
        { Game.MiloGame.GuitarHero2_360, 2 },
    };
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();
    public RndTrans trans = new();

    [Name("Center"), Description("Name of RndTransformable to center proxy about")]
    public Symbol center = Symbol.Empty;

    public new BandPlacer Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.objFields = base.objFields.Read(reader, parent, entry);

        draw = draw.Read(reader, false, parent, entry);
        trans = trans.Read(reader, false, parent, entry);

        center = Symbol.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        objFields.Write(writer, parent);

        draw.Write(writer, false, parent, null);
        trans.Write(writer, false, parent, null);

        Symbol.Write(writer, center);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
