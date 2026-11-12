using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Distort"), Description("Distortion effect")]
public class RndDistort : RndDrawable
{
    private ushort altRevision;
    private ushort revision;
    private byte[] rawData = Array.Empty<byte>();
    private bool useRawData;

    [Name("Drawables")]
    public new List<Symbol> drawables = new();

    [Name("Shine"), Description("Intensity of specular shine off pulse peaks")]
    public float shine;

    [Name("Pulse Center"), Description("Pulse center")]
    public Vector2 center = new();

    [Name("Pulse Strength"), Description("Strength of pulse")]
    public float strength;

    [Name("Pulse Duration"), Description("Length of time to go from center to max range")]
    public float duration;

    [Name("Pulse Y Size"), Description("Vertical size of pulse")]
    public float ysize;

    [Name("Pulse X Scale"), Description("Horizontal scale of pulse, 1 is round, greater than 1 is wider than tall.")]
    public float xscale;

    [Name("Pulse Wavelength"), Description("Length of one wave")]
    public float wavelength;

    [Name("Pulse Glint"), Description("Strength of specular glint")]
    public float glint;

    [Name("Single Pulse"), Description("True to have a single pulse radiate out, false for multiple ripples")]
    public bool single = true;

    public new RndDistort Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 1)
        {
            useRawData = true;
            rawData = entry.ObjBytesSpan.ToArray();
            reader.BaseStream.Position = entry.objBytesAbsolutePosition + rawData.Length;
        }
        else
        {
            base.Read(reader, false, parent, entry);

            drawables = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            shine = reader.ReadFloat();

            if (revision > 0)
            {
                center = new Vector2().Read(reader);
                strength = reader.ReadFloat();
                duration = reader.ReadFloat();
                ysize = reader.ReadFloat();
                xscale = reader.ReadFloat();
                wavelength = reader.ReadFloat();
                glint = reader.ReadFloat();
                single = reader.ReadBoolean();
            }
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry);
        }

        return this;
    }

    public new void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (useRawData)
        {
            writer.WriteBlock(rawData);
        }
        else
        {
            AssetHelpers.WriteRevision(writer, revision, altRevision);

            base.Write(writer, false, parent, entry);

            AssetHelpers.WriteList(writer, drawables, (Symbol drawable) => Symbol.Write(writer, drawable));

            writer.WriteFloat(shine);

            if (revision > 0)
            {
                center.Write(writer);
                writer.WriteFloat(strength);
                writer.WriteFloat(duration);
                writer.WriteFloat(ysize);
                writer.WriteFloat(xscale);
                writer.WriteFloat(wavelength);
                writer.WriteFloat(glint);
                writer.WriteBoolean(single);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
