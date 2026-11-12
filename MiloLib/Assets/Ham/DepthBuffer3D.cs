using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("DepthBuffer3D"), Description("Render the Kinect depth buffer as a 3D mesh")]
public class DepthBuffer3D : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    public RndTrans trans = new();
    [Name("Nobody Color"), Description("Color for non-player pixels (i.e. the background)")]
    public HmxColor4 nobodyColor = new();
    [Name("Player Palette"), Description("1D palette for player depth")]
    public Symbol playerPalette = Symbol.Empty;
    [Name("Player Palette Offset"), Description("Starting point for palette")]
    public float playerPaletteOffset;
    [Name("Minimal Mat"), Description("Some Mat properties are used to render the depth buffer")]
    public Symbol minimalMat = Symbol.Empty;
    public uint oldValue;
    [Name("Draw Sheet"), Description("draw old school depth buffer - 1 plane")]
    public bool drawSheet;
    [Name("Mesh"), Description("Mesh to draw")]
    public Symbol mesh = Symbol.Empty;
    [Name("Player Palette Scale"), Description("Scale the coordinate used to look up the palette value. If the scale is 2, you'll cycle through the palette twice as fast, and so on.")]
    public float playerPaletteScale = 1;
    [Name("Stretch Near Camera"), Description("Stretch the depth buffer along an exponential curve. 1 is the default; values greater than 1 mean more distortion for objects closer to the Kinect camera.")]
    public float stretchNearCamera = 1;
    [Name("Opacity"), Description("Multiply palette alpha by this value.")]
    public float opacity = 1;
    [Name("Draw Player 1"), Description("Whether Player 1 should be drawn in this DepthBuffer3D")]
    public bool drawPlayer1 = true;
    [Name("Draw Player 2"), Description("Whether Player 2 should be drawn in this DepthBuffer3D")]
    public bool drawPlayer2 = true;
    [Name("Draw Non Players"), Description("Whether non-players should be drawn in this DepthBuffer3D")]
    public bool drawNonPlayers = true;
    public Vector2 tile = new(1.5f, 1.5f);
    [Name("Scale Voxel"), Description("Voxel scalar")]
    public float scaleVoxel = 1;
    [Name("Scale Voxelgap"), Description("Voxel gap scalar")]
    public float scaleVoxelGap = 1;
    [Name("Fisheye X"), Description("horizontal fisheye coefficient")]
    public float fishEyeX;
    [Name("Fisheye Y"), Description("vertical fisheye coefficient")]
    public float fishEyeY;
    [Name("Max Zoom"), Description("maximum uv zooming")]
    public float maxZoom = 1;
    [Name("Debug Layout"), Description("enabled alters xbox rendering to display every voxel")]
    public bool debugLayout;
    [Name("Max Depth Zoom"), Description("maximum uv zooming")]
    public float maxDepthZoom = 1;

    public new DepthBuffer3D Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 11);
        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        trans.Read(reader, false, parent, entry);
        if (revision > 1)
        {
            nobodyColor.Read(reader);
            playerPalette = Symbol.Read(reader);
            playerPaletteOffset = reader.ReadFloat();
            minimalMat = Symbol.Read(reader);
            if (revision < 3)
            {
                oldValue = reader.ReadUInt32();
            }
        }
        if (revision > 2)
        {
            drawSheet = reader.ReadBoolean();
            mesh = Symbol.Read(reader);
        }
        if (revision > 3)
        {
            playerPaletteScale = reader.ReadFloat();
            stretchNearCamera = reader.ReadFloat();
        }
        if (revision > 4)
        {
            opacity = reader.ReadFloat();
        }
        if (revision > 5)
        {
            drawPlayer1 = reader.ReadBoolean();
            drawPlayer2 = reader.ReadBoolean();
            drawNonPlayers = reader.ReadBoolean();
        }
        if (revision > 6)
        {
            tile.Read(reader);
            scaleVoxel = reader.ReadFloat();
            scaleVoxelGap = reader.ReadFloat();
        }
        if (revision > 7)
        {
            fishEyeX = reader.ReadFloat();
        }
        if (revision > 8)
        {
            fishEyeY = reader.ReadFloat();
        }
        if (revision > 9)
        {
            maxZoom = reader.ReadFloat();
            debugLayout = reader.ReadBoolean();
        }
        if (revision > 10)
        {
            maxDepthZoom = reader.ReadFloat();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 11);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        trans.Write(writer, false, parent, entry);
        if (revision > 1)
        {
            nobodyColor.Write(writer);
            Symbol.Write(writer, playerPalette);
            writer.WriteFloat(playerPaletteOffset);
            Symbol.Write(writer, minimalMat);
            if (revision < 3)
            {
                writer.WriteUInt32(oldValue);
            }
        }
        if (revision > 2)
        {
            writer.WriteBoolean(drawSheet);
            Symbol.Write(writer, mesh);
        }
        if (revision > 3)
        {
            writer.WriteFloat(playerPaletteScale);
            writer.WriteFloat(stretchNearCamera);
        }
        if (revision > 4)
        {
            writer.WriteFloat(opacity);
        }
        if (revision > 5)
        {
            writer.WriteBoolean(drawPlayer1);
            writer.WriteBoolean(drawPlayer2);
            writer.WriteBoolean(drawNonPlayers);
        }
        if (revision > 6)
        {
            tile.Write(writer);
            writer.WriteFloat(scaleVoxel);
            writer.WriteFloat(scaleVoxelGap);
        }
        if (revision > 7)
        {
            writer.WriteFloat(fishEyeX);
        }
        if (revision > 8)
        {
            writer.WriteFloat(fishEyeY);
        }
        if (revision > 9)
        {
            writer.WriteFloat(maxZoom);
            writer.WriteBoolean(debugLayout);
        }
        if (revision > 10)
        {
            writer.WriteFloat(maxDepthZoom);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
