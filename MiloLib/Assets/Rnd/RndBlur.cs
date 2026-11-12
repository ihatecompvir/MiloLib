using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Blur"), Description("A Blur object renders an object with more transparency at older positions. It is drawable and should replace the blurred object as a draw child.")]
public class RndBlur : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();

    [Name("Mesh"), Description("Target for the blur to occur on.")]
    public Symbol mesh = Symbol.Empty;

    [Name("Length"), Description("Number of blur keys.")]
    public int length;

    [Name("Rate"), Description("Number of draw updates between keys")]
    public int rate = 1;

    [Name("Falloff"), Description("Alpha falloff along length"), MinVersion(1)]
    public float falloff = 1.0f;

    [Name("Text"), MinVersion(2)]
    public Symbol text = Symbol.Empty;

    public new RndBlur Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        if (revision > 2)
        {
            base.Read(reader, false, parent, entry);
        }
        draw = new RndDrawable().Read(reader, false, parent, entry);
        mesh = Symbol.Read(reader);
        length = reader.ReadInt32();
        rate = reader.ReadInt32();
        if (revision > 0)
        {
            falloff = reader.ReadFloat();
        }
        if (revision > 1)
        {
            text = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        if (revision > 2)
        {
            base.Write(writer, false, parent, entry);
        }
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, mesh);
        writer.WriteInt32(length);
        writer.WriteInt32(rate);
        if (revision > 0)
        {
            writer.WriteFloat(falloff);
        }
        if (revision > 1)
        {
            Symbol.Write(writer, text);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
