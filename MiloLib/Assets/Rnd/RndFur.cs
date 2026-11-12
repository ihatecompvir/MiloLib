using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Fur"), Description("Parameters for fur shading, to be set on a material")]
public class RndFur : Object
{
    private ushort altRevision;
    private ushort revision;


    [Name("Layers"), Description("Number of passes")]
    public int layers = 12;
    [Name("Thickness"), Description("Length of fur")]
    public float thickness = 3;
    [Name("Curvature"), Description("Curvature exponent")]
    public float curvature = 2;
    [Name("Shell Out"), Description("Bunch shells towards surface"), MinVersion(2)]
    public float shellOut = 1;
    [Name("Alpha Falloff"), Description("Bunch opacity towards surface"), MinVersion(2)]
    public float alphaFalloff = 0.5f;
    [Name("Stretch"), Description("Maximum stretch")]
    public float stretch = 1;
    [Name("Slide"), Description("Maximum lateral motion")]
    public float slide = 1;
    [Name("Gravity"), Description("Strength of gravity")]
    public float gravity = 1;
    [Name("Fluidity"), Description("Langor of motion")]
    public float fluidity = 0.25f;

    [Name("Roots Tint"), Description("Tint at hair roots")]
    public HmxColor4 rootsTint = new(0, 0, 0, 1);
    [Name("Ends Tint"), Description("Tint at hair ends")]
    public HmxColor4 endsTint = new(1, 1, 1, 1);

    [Name("Fur Detail"), Description("Detail map for finer fur. Only the alpha channel is used.")]
    public Symbol furDetailMap = Symbol.Empty;
    [Name("Fur Tiling"), Description("Tiling for fur detail map. UVs of fur_detail are multiplied by this value.")]
    public float furTiling = 1;

    [Name("Wind"), Description("Wind Object, if set, blows on the fur."), MinVersion(3)]
    public Symbol wind = Symbol.Empty;



    public new RndFur Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        layers = reader.ReadInt32();
        thickness = reader.ReadFloat();
        curvature = reader.ReadFloat();
        if (revision > 1)
        {
            shellOut = reader.ReadFloat();
            alphaFalloff = reader.ReadFloat();
        }

        stretch = reader.ReadFloat();
        slide = reader.ReadFloat();
        gravity = reader.ReadFloat();
        fluidity = reader.ReadFloat();

        rootsTint = new HmxColor4().Read(reader);
        endsTint = new HmxColor4().Read(reader);

        furDetailMap = Symbol.Read(reader);
        furTiling = reader.ReadFloat();

        if (revision > 2)
        {
            wind = Symbol.Read(reader);
        }


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

        writer.WriteInt32(layers);
        writer.WriteFloat(thickness);
        writer.WriteFloat(curvature);
        if (revision > 1)
        {
            writer.WriteFloat(shellOut);
            writer.WriteFloat(alphaFalloff);
        }
        writer.WriteFloat(stretch);
        writer.WriteFloat(slide);
        writer.WriteFloat(gravity);
        writer.WriteFloat(fluidity);

        rootsTint.Write(writer);
        endsTint.Write(writer);

        Symbol.Write(writer, furDetailMap);
        writer.WriteFloat(furTiling);

        if (revision > 2)
        {
            Symbol.Write(writer, wind);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
