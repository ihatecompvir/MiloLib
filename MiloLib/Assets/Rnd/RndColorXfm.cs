using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

public class RndColorXfm
{
    private ushort altRevision;
    private ushort revision;

    [Name("Color Xfm"), Description("Generated color transformation matrix"), MaxVersion(0)]
    public Matrix colorTransform = new();

    [Name("Lightness"), Description("Lightness: -100 to 100, 0.0 is neutral"), MaxVersion(0)]
    public float lightness;
    [Name("Saturation"), Description("Saturation: -100 to 100, 0.0 is neutral"), MaxVersion(0)]
    public float saturation;
    [Name("Hue"), Description("Hue: -180 to 180, 0.0 is neutral"), MaxVersion(0)]
    public float hue;
    [Name("Brightness"), Description("Brightness: -100 to 100, 0.0 is neutral"), MaxVersion(0)]
    public float brightness;
    [Name("Contrast"), Description("Contrast: -100 to 100, 0.0 is neutral"), MaxVersion(0)]
    public float contrast;

    [Name("In Lo"), Description("Input low end"), MaxVersion(0)]
    public HmxColor4 levelInLo = new();
    [Name("In Hi"), Description("Input high end"), MaxVersion(0)]
    public HmxColor4 levelInHi = new();
    [Name("Out Lo"), Description("Output low end"), MaxVersion(0)]
    public HmxColor4 levelOutLo = new();
    [Name("Out Hi"), Description("Output high end"), MaxVersion(0)]
    public HmxColor4 levelOutHi = new();

    public RndColorXfm Read(EndianReader reader)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 0)
        {
            return this;
        }
        colorTransform = colorTransform.Read(reader);

        hue = reader.ReadFloat();
        saturation = reader.ReadFloat();
        lightness = reader.ReadFloat();
        contrast = reader.ReadFloat();
        brightness = reader.ReadFloat();

        levelInLo = levelInLo.Read(reader);
        levelInHi = levelInHi.Read(reader);
        levelOutLo = levelOutLo.Read(reader);
        levelOutHi = levelOutHi.Read(reader);
        return this;
    }

    public void Write(EndianWriter writer)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision > 0)
        {
            return;
        }

        colorTransform.Write(writer);

        writer.WriteFloat(hue);
        writer.WriteFloat(saturation);
        writer.WriteFloat(lightness);
        writer.WriteFloat(contrast);
        writer.WriteFloat(brightness);

        levelInLo.Write(writer);
        levelInHi.Write(writer);
        levelOutLo.Write(writer);
        levelOutHi.Write(writer);
    }
}
