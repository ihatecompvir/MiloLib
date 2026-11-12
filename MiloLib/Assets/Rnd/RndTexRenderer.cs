using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("TexRenderer"), Description("TexRender renders a draw and cam into a texture.")]
public class RndTexRenderer : Object
{
    private ushort altRevision;
    private ushort revision;

    private bool UsesGreenDayLayout(DirectoryMeta parent)
    {
        return parent.revision == 25 && revision == 11;
    }

    [Name("Camera Shot"), Description("Camera shot to use for animated camera source, override cam if set")]
    public Symbol camShot = Symbol.Empty;
    [HideInInspector]
    public byte oldCamShotFlag;

    public RndAnimatable anim = new();
    public RndDrawable draw = new();
    public Object poll = new();

    [MaxVersion(0)]
    private Symbol oldFilePath = Symbol.Empty;

    [Name("Draw"), Description("Draw Object to render to texture"), MinVersion(1)]
    public Symbol drawObj = Symbol.Empty;

    [Name("Cam"), Description("Camera to use, if you want specific one, defaults to proxy cam, if none and draw is proxy"), MinVersion(4)]
    public Symbol cam = Symbol.Empty;

    [Name("Output Texture"), Description("Texture to write to")]
    public Symbol outputTexture = Symbol.Empty;

    [Name("Force"), Description("Force rendering every frame"), MinVersion(2)]
    public bool force;

    [Name("Imposter Height"), Description("Height for imposter rendering with current camera"), MinVersion(2)]
    public float imposterHeight;

    [Name("Draw Responsible"), Description("If true, exclusively draws the draw, if false the scene will draw it too, use with caution!"), MinVersion(5)]
    public bool drawResponsible;

    [Name("Draw Pre Clear"), Description("Renders the texture before the rest of the scene is rendered. Useful for rendering large textures"), MinVersion(6)]
    public bool drawPreClear;

    [Name("Draw World Only"), Description("Renders the texture only on 'world' frames, while skipping rendering on post processing frames"), MinVersion(7)]
    public bool drawWorldOnly;

    [Name("Prime Draw"), Description("Check this if rendering multiple characters to a texture. Will draw 2x if checked."), MinVersion(8)]
    public bool primeDraw;

    [Name("Force Mips"), Description("Generate mip maps for the texture."), MinVersion(9)]
    public bool forceMips;

    [Name("Mirror Cam"), Description("We will mirror this cam about whatever mesh is associated with our output texture to automatically position the render-2-tex cam for mirroring"), MinVersion(10)]
    public Symbol mirrorCam = Symbol.Empty;

    [Name("No Poll"), Description("If [draw] will not get enter, exit, or poll automatically, it will be up to script hooks to do any of that"), MinVersion(11)]
    public bool noPoll;

    [Name("Environment"), Description("Environment to set before rendering to texture"), MinVersion(12)]
    public Symbol environ = Symbol.Empty;
    [Name("Clear Buffer"), MinVersion(13)]
    public bool clearBuffer;
    [Name("Clear Color"), MinVersion(13)]
    public HmxColor4 clearColor = new();

    public new RndTexRenderer Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision > 2)
        {
            anim = anim.Read(reader, parent, entry);
            draw = draw.Read(reader, false, parent, entry);
            if (revision > 10 && !UsesGreenDayLayout(parent))
            {
                poll = poll.Read(reader, false, parent, entry);
            }
        }

        if (revision < 1)
        {
            oldFilePath = Symbol.Read(reader);
        }
        else
        {
            drawObj = Symbol.Read(reader);
        }

        if (revision > 3)
        {
            cam = Symbol.Read(reader);
        }

        if (UsesGreenDayLayout(parent))
        {
            camShot = Symbol.Read(reader);
            oldCamShotFlag = reader.ReadByte();
        }

        outputTexture = Symbol.Read(reader);

        if (revision > 1)
        {
            force = reader.ReadBoolean();
            imposterHeight = reader.ReadFloat();
        }

        if (revision > 4)
        {
            drawResponsible = reader.ReadBoolean();
        }

        if (revision > 5)
        {
            drawPreClear = reader.ReadBoolean();
        }

        if (revision > 6)
        {
            drawWorldOnly = reader.ReadBoolean();
        }

        if (revision > 7)
        {
            primeDraw = reader.ReadBoolean();
        }

        if (revision > 8)
        {
            forceMips = reader.ReadBoolean();
        }

        if (revision > 9 && !UsesGreenDayLayout(parent))
        {
            mirrorCam = Symbol.Read(reader);
        }

        if (revision > 10 && !UsesGreenDayLayout(parent))
        {
            noPoll = reader.ReadBoolean();
        }

        if (revision > 11)
        {
            environ = Symbol.Read(reader);
        }
        if (revision > 12)
        {
            clearBuffer = reader.ReadBoolean();
            clearColor.Read(reader);
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

        if (revision > 2)
        {
            anim.Write(writer);
            draw.Write(writer, false, parent, entry);
            if (revision > 10 && !UsesGreenDayLayout(parent))
            {
                poll.Write(writer, false, parent, entry);
            }
        }

        if (revision < 1)
        {
            Symbol.Write(writer, oldFilePath);
        }
        else
        {
            Symbol.Write(writer, drawObj);
        }

        if (revision > 3)
        {
            Symbol.Write(writer, cam);
        }

        if (UsesGreenDayLayout(parent))
        {
            Symbol.Write(writer, camShot);
            writer.WriteByte(oldCamShotFlag);
        }

        Symbol.Write(writer, outputTexture);

        if (revision > 1)
        {
            writer.WriteBoolean(force);
            writer.WriteFloat(imposterHeight);
        }

        if (revision > 4)
        {
            writer.WriteBoolean(drawResponsible);
        }

        if (revision > 5)
        {
            writer.WriteBoolean(drawPreClear);
        }

        if (revision > 6)
        {
            writer.WriteBoolean(drawWorldOnly);
        }

        if (revision > 7)
        {
            writer.WriteBoolean(primeDraw);
        }

        if (revision > 8)
        {
            writer.WriteBoolean(forceMips);
        }

        if (revision > 9 && !UsesGreenDayLayout(parent))
        {
            Symbol.Write(writer, mirrorCam);
        }

        if (revision > 10 && !UsesGreenDayLayout(parent))
        {
            writer.WriteBoolean(noPoll);
        }

        if (revision > 11)
        {
            Symbol.Write(writer, environ);
        }
        if (revision > 12)
        {
            writer.WriteBoolean(clearBuffer);
            clearColor.Write(writer);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
