using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("StreamRenderer"), Description("Renders Natal stream textures into a texture.")]
public class StreamRenderer : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    [Name("Output Texture"), Description("Texture to write to")]
    public Symbol outputTex = Symbol.Empty;
    [Name("Force Mips"), Description("Generate mip maps for the texture.")]
    public bool forceMips;
    [Name("Display"), Description("Natal buffer to display")]
    public int display;
    [Name("Player1 Depth Color"), Description("Player 1 color")]
    public HmxColor4 player1DepthColor = new();
    [Name("Player2 Depth Color"), Description("Player 2 color")]
    public HmxColor4 player2DepthColor = new();
    [Name("Player3 Depth Color"), Description("Player 3 color")]
    public HmxColor4 player3DepthColor = new();
    [Name("Player Depth Nobody"), Description("Color for non-player pixels (i.e. the background)")]
    public HmxColor4 playerDepthNobody = new();
    [Name("Player1 Depth Palette"), Description("1D palette for player 1 depth")]
    public Symbol player1DepthPalette = Symbol.Empty;
    [Name("Player1 Depth Palette Offset"), Description("Starting point for p1 palette")]
    public float player1DepthPaletteOffset;
    [Name("Background Depth Palette"), Description("1D palette for background depth")]
    public Symbol backgroundDepthPalette = Symbol.Empty;
    [Name("Background Depth Palette Offset"), Description("Starting point for palette")]
    public float backgroundDepthPaletteOffset;
    [Name("Num Blurs"), Description("Number of times to blur the player silhouette texture")]
    public int numBlurs;
    [Name("PC Test Texture"), Description("StreamRenderer will output this test texture. Only works on the PC.")]
    public Symbol pcTestTex = Symbol.Empty;
    [Name("Player2 Depth Palette"), Description("1D palette for player 2 depth")]
    public Symbol player2DepthPalette = Symbol.Empty;
    [Name("Player2 Depth Palette Offset"), Description("Starting point for p2 palette")]
    public float player2DepthPaletteOffset;
    [Name("Player Other Depth Palette"), Description("1D palette for players 3-6")]
    public Symbol playerOtherDepthPalette = Symbol.Empty;
    [Name("Player Other Depth Palette Offset"), Description("Starting point for p3-p6 palettes")]
    public float playerOtherDepthPaletteOffset;
    public bool drawPreClear;
    [Name("Force Draw"), Description("Always render the image, even there is no new depth/color buffer this frame.")]
    public bool forceDraw;
    [Name("Lag Primary Texture"), Description("Used to lag the depth image by one frame to better line up with the color image for greenscreening")]
    public bool lagPrimaryTexture;
    [Name("Player4 Depth Color"), Description("Player 4 color")]
    public HmxColor4 player4DepthColor = new();
    [Name("Player5 Depth Color"), Description("Player 5 color")]
    public HmxColor4 player5DepthColor = new();
    [Name("Player6 Depth Color"), Description("Player 6 color")]
    public HmxColor4 player6DepthColor = new();
    [Name("Static Color Indices"), Description("Assign colors by index instead of giving preference to Player 1 and Player 2.")]
    public bool staticColorIndices;
    [Name("Crew Photo Edge Iterations")]
    public float crewPhotoEdgeIterations;
    [Name("Crew Photo Edge Offset")]
    public float crewPhotoEdgeOffset;
    [Name("Crew Photo Horizontal Color"), Description("note: will be overwritten dynamically")]
    public HmxColor4 crewPhotoHorizontalColor = new();
    [Name("Crew Photo Vertical Color"), Description("note: will be overwritten dynamically")]
    public HmxColor4 crewPhotoVerticalColor = new();
    [Name("Crew Photo Blur Start")]
    public float crewPhotoBlurStart;
    [Name("Crew Photo Blur Width")]
    public float crewPhotoBlurWidth;
    [Name("Crew Photo Blur Iterations")]
    public float crewPhotoBlurIterations;
    [Name("Crew Photo Background Brightness")]
    public float crewPhotoBackgroundBrightness;

    public new StreamRenderer Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 12);
        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        outputTex = Symbol.Read(reader);
        forceMips = reader.ReadBoolean();
        display = reader.ReadInt32();
        if (revision > 1)
        {
            player1DepthColor.Read(reader);
            player2DepthColor.Read(reader);
            player3DepthColor.Read(reader);
            playerDepthNobody.Read(reader);
            player1DepthPalette = Symbol.Read(reader);
            player1DepthPaletteOffset = reader.ReadFloat();
        }
        if (revision > 2)
        {
            backgroundDepthPalette = Symbol.Read(reader);
            backgroundDepthPaletteOffset = reader.ReadFloat();
        }
        if (revision > 3)
        {
            numBlurs = reader.ReadInt32();
        }
        if (revision > 4)
        {
            pcTestTex = Symbol.Read(reader);
        }
        if (revision > 5)
        {
            player2DepthPalette = Symbol.Read(reader);
            player2DepthPaletteOffset = reader.ReadFloat();
        }
        if (revision > 6)
        {
            playerOtherDepthPalette = Symbol.Read(reader);
            playerOtherDepthPaletteOffset = reader.ReadFloat();
        }
        if (revision > 7)
        {
            drawPreClear = reader.ReadBoolean();
        }
        if (revision > 7 && altRevision > 0)
        {
            forceDraw = reader.ReadBoolean();
        }
        if (revision > 8)
        {
            lagPrimaryTexture = reader.ReadBoolean();
        }
        if (revision > 9)
        {
            player4DepthColor.Read(reader);
            player5DepthColor.Read(reader);
            player6DepthColor.Read(reader);
        }
        if (revision > 10)
        {
            staticColorIndices = reader.ReadBoolean();
        }
        if (revision > 11)
        {
            crewPhotoEdgeIterations = reader.ReadFloat();
            crewPhotoEdgeOffset = reader.ReadFloat();
            crewPhotoHorizontalColor.Read(reader);
            crewPhotoVerticalColor.Read(reader);
            crewPhotoBlurStart = reader.ReadFloat();
            crewPhotoBlurWidth = reader.ReadFloat();
            crewPhotoBlurIterations = reader.ReadFloat();
            crewPhotoBackgroundBrightness = reader.ReadFloat();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 12);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, outputTex);
        writer.WriteBoolean(forceMips);
        writer.WriteInt32(display);
        if (revision > 1)
        {
            player1DepthColor.Write(writer);
            player2DepthColor.Write(writer);
            player3DepthColor.Write(writer);
            playerDepthNobody.Write(writer);
            Symbol.Write(writer, player1DepthPalette);
            writer.WriteFloat(player1DepthPaletteOffset);
        }
        if (revision > 2)
        {
            Symbol.Write(writer, backgroundDepthPalette);
            writer.WriteFloat(backgroundDepthPaletteOffset);
        }
        if (revision > 3)
        {
            writer.WriteInt32(numBlurs);
        }
        if (revision > 4)
        {
            Symbol.Write(writer, pcTestTex);
        }
        if (revision > 5)
        {
            Symbol.Write(writer, player2DepthPalette);
            writer.WriteFloat(player2DepthPaletteOffset);
        }
        if (revision > 6)
        {
            Symbol.Write(writer, playerOtherDepthPalette);
            writer.WriteFloat(playerOtherDepthPaletteOffset);
        }
        if (revision > 7)
        {
            writer.WriteBoolean(drawPreClear);
        }
        if (revision > 7 && altRevision > 0)
        {
            writer.WriteBoolean(forceDraw);
        }
        if (revision > 8)
        {
            writer.WriteBoolean(lagPrimaryTexture);
        }
        if (revision > 9)
        {
            player4DepthColor.Write(writer);
            player5DepthColor.Write(writer);
            player6DepthColor.Write(writer);
        }
        if (revision > 10)
        {
            writer.WriteBoolean(staticColorIndices);
        }
        if (revision > 11)
        {
            writer.WriteFloat(crewPhotoEdgeIterations);
            writer.WriteFloat(crewPhotoEdgeOffset);
            crewPhotoHorizontalColor.Write(writer);
            crewPhotoVerticalColor.Write(writer);
            writer.WriteFloat(crewPhotoBlurStart);
            writer.WriteFloat(crewPhotoBlurWidth);
            writer.WriteFloat(crewPhotoBlurIterations);
            writer.WriteFloat(crewPhotoBackgroundBrightness);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

