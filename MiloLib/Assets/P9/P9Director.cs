using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.P9;

[Name("P9Director"), Description("P9 Director, sits in each song file and manages camera + scene changes")]
public class P9Director : Object
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        { Game.MiloGame.TheBeatlesRockBand, 5 },
        { Game.MiloGame.GreenDayRockBand, 5 },
        { Game.MiloGame.RockBand3, 5 },
    };
    private ushort altRevision;
    private ushort revision;

    public ObjectFields objFields1 = new();
    public ObjectFields objFields2 = new();

    public RndDrawable draw = new();

    [Name("Venue"), Description("The currently loaded venue")]
    public Symbol venue = Symbol.Empty;

    public new P9Director Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        objFields1 = objFields1.Read(reader, parent, entry);
        objFields2 = objFields2.Read(reader, parent, entry);

        draw = new RndDrawable().Read(reader, false, parent, entry);
        venue = Symbol.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        objFields1.Write(writer, parent);
        objFields2.Write(writer, parent);

        draw.Write(writer, false, parent);
        Symbol.Write(writer, venue);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
