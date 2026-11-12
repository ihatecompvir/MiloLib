using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("ScreenMask"), Description("Draws full screen quad with material and color.")]
public class RndScreenMask : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();

    [Name("Material"), Description("Material to draw on full screen quad")]
    public Symbol material = Symbol.Empty;
    [Name("Color"), Description("Color of full screen quad")]
    public HmxColor4 color = new();
    [Name("Alpha"), Description("Translucency of full screen quad")]
    public float alpha;

    [Name("Screen Rect"), Description("The area of the screen in normalized coordinates (0 to 1) to draw into."), MinVersion(1)]
    public Rect rect = new();

    [Name("Use Cam Rect"), Description("Use current camera screen_rect instead of the full screen"), MinVersion(2)]
    public bool useCamRect;

    public new RndScreenMask Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        draw = draw.Read(reader, false, parent, entry);

        material = Symbol.Read(reader);
        color = color.Read(reader);

        if (revision > 0)
        {
            rect = rect.Read(reader);
        }

        if (revision > 1)
        {
            useCamRect = reader.ReadBoolean();
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

        draw.Write(writer, false, parent, null);

        Symbol.Write(writer, material);
        color.Write(writer);

        if (revision > 0)
        {
            rect.Write(writer);
        }

        if (revision > 1)
        {
            writer.WriteBoolean(useCamRect);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
