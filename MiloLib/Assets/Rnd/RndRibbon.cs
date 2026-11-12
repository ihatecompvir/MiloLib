using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Ribbon"), Description("Ribbon")]
public class RndRibbon : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    [Name("Num Sides"), Description("Number of sides, the more sides, the more cylindrical it is")]
    public int numSides;
    [Name("Mat"), Description("Material to use")]
    public Symbol mat = Symbol.Empty;
    [Name("Active")]
    public bool active;
    [Name("Width"), Description("Width of the tube")]
    public float width;
    [Name("Num Segments"), Description("Number of segments it's built out of")]
    public int numSegments;
    [Name("Follow A"), Description("Which Trans to follow")]
    public Symbol followA = Symbol.Empty;
    [Name("Follow B"), Description("Another Trans to follow")]
    public Symbol followB = Symbol.Empty;
    [Name("Decay"), Description("The ribbon ends after this many seconds. Each segment gets an equal fraction of this time")]
    public float decay;
    [Name("Taper"), Description("Tapers the end if true, otherwise is squared off")]
    public bool taper;
    [Name("Follow Weight"), Description("Interpolates the follow point between [follow_a] and [follow_b], so for example .5 gives you the average of those two positions")]
    public float followWeight;

    public new RndRibbon Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        draw = new RndDrawable().Read(reader, false, parent, entry);
        numSides = reader.ReadInt32();
        mat = Symbol.Read(reader);
        active = reader.ReadBoolean();
        width = reader.ReadFloat();
        numSegments = reader.ReadInt32();
        followA = Symbol.Read(reader);
        followB = Symbol.Read(reader);
        decay = reader.ReadFloat();
        taper = reader.ReadBoolean();
        followWeight = reader.ReadFloat();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        writer.WriteInt32(numSides);
        Symbol.Write(writer, mat);
        writer.WriteBoolean(active);
        writer.WriteFloat(width);
        writer.WriteInt32(numSegments);
        Symbol.Write(writer, followA);
        Symbol.Write(writer, followB);
        writer.WriteFloat(decay);
        writer.WriteBoolean(taper);
        writer.WriteFloat(followWeight);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
