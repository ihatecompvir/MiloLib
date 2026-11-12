using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamPhraseMeter"), Description("Custom component for move feedback progress")]
public class HamPhraseMeter : RndDir
{
    private ushort revision;
    private ushort altRevision;
    [Name("Anim"), Description("what do animate")]
    public Symbol meterAnim = Symbol.Empty;
    [Name("Rating Frac"), Description("Current rating frac (for testing)")]
    public float ratingFrac;
    [Name("Rating"), Description("Current rating symbol (for testing)")]
    public Symbol rating = Symbol.Empty;
    [Name("Desired FPB"), Description("Desired animation speed (frames per beat)")]
    public float desiredFpb;
    [Name("First Perfect Frame"), Description("Value of the first frame of a perfect rating")]
    public float firstPerfectFrame;
    [Name("Player Index"), Description("Player index this phrase meter is for")]
    public int playerIndex;

    public HamPhraseMeter(ushort revision = 0, ushort altRevision = 0) : base(0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new HamPhraseMeter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        meterAnim = Symbol.Read(reader);
        ratingFrac = reader.ReadFloat();
        rating = Symbol.Read(reader);
        desiredFpb = reader.ReadFloat();
        firstPerfectFrame = reader.ReadFloat();
        if (revision > 2 && entry.isProxy)
        {
            playerIndex = reader.ReadInt32();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, meterAnim);
        writer.WriteFloat(ratingFrac);
        Symbol.Write(writer, rating);
        writer.WriteFloat(desiredFpb);
        writer.WriteFloat(firstPerfectFrame);
        if (revision > 2 && entry.isProxy)
        {
            writer.WriteInt32(playerIndex);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
