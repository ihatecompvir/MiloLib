using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamRibbon"), Description("Ribbon")]
public class HamRibbon : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    [Name("Num Sides")]
    public int numSides = 4;
    [Name("Mat")]
    public Symbol mat = Symbol.Empty;
    [Name("Active")]
    public bool active = true;
    [Name("Width"), Description("Width of the tube")]
    public float width = 1;
    [Name("Num Segments")]
    public int numSegments;
    [Name("Follow A")]
    public Symbol followA = Symbol.Empty;
    [Name("Follow B")]
    public Symbol followB = Symbol.Empty;
    [Name("Follow Weight")]
    public float followWeight;
    [Name("Taper")]
    public bool taper;
    [Name("Decay")]
    public float decay = 1;

    public new HamRibbon Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        numSides = reader.ReadInt32();
        mat = Symbol.Read(reader);
        active = reader.ReadBoolean();
        width = reader.ReadFloat();
        numSegments = reader.ReadInt32();
        followA = Symbol.Read(reader);
        followB = Symbol.Read(reader);
        followWeight = reader.ReadFloat();
        taper = reader.ReadBoolean();
        if (revision > 0)
        {
            decay = reader.ReadFloat();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        writer.WriteInt32(numSides);
        Symbol.Write(writer, mat);
        writer.WriteBoolean(active);
        writer.WriteFloat(width);
        writer.WriteInt32(numSegments);
        Symbol.Write(writer, followA);
        Symbol.Write(writer, followB);
        writer.WriteFloat(followWeight);
        writer.WriteBoolean(taper);
        if (revision > 0)
        {
            writer.WriteFloat(decay);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
