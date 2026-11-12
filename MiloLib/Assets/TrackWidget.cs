using MiloLib.Assets.Rnd;
using MiloLib.Classes; // Added for attributes
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("TrackWidget"), Description("Any object that is placed on the track and scrolls towards the player. Can have any number of meshes and an environment. Drawn efficiently and pruned automatically by TrackDir.")]
public class TrackWidget : Object
{
    public enum WidgetType
    {
        kImmediateWidget = 0,
        kMultiMeshWidget = 1,
        kTextWidget = 2,
        kMatWidget = 3
    };

    private ushort altRevision;
    private ushort revision;

    [MinVersion(1)]
    public RndDrawable draw = new();

    [Name("Meshes"), Description("Meshes used to draw widgets, drawn in order")]
    public List<Symbol> meshes = new();

    [MinVersion(5)]
    public List<Symbol> meshesLeft = new();
    [MinVersion(5)]
    public List<Symbol> meshesSpan = new();
    [MinVersion(5)]
    public List<Symbol> meshesRight = new();

    [MinVersion(5)]
    public bool wideWidget;

    [Name("Environ"), Description("Environment used to draw widget")]
    public Symbol environ = Symbol.Empty;

    [Name("Base Length"), Description("Length of unscaled geometry, should be 0 if no duration"), MinVersion(3)]
    float baseLength;
    [Name("Base Width"), Description("Width of unscaled geometry, should be 0 if no scaling"), MinVersion(9)]
    float baseWidth;
    [Name("X Offset"), Description("X offset to be applied to all widget instances"), MinVersion(13)]
    float xOffset;
    [Name("Y Offset"), Description("Y offset to be applied to all widget instances"), MinVersion(12)]
    float yOffset;
    [Name("Z Offset"), Description("Z offset to be applied to all widget instances"), MinVersion(13)]
    float zOffset;


    [MinVersion(2), MaxVersion(7)]
    public bool shouldUseWideWidget;
    [MinVersion(6), MaxVersion(7)]
    public bool shouldBeTextWidget;

    [Name("Allow Rotation"), Description("Allow meshes to be rotated/scaled"), MinVersion(4)]
    public bool allowRotation;

    [Name("Font"), MinVersion(6)]
    public Symbol font = Symbol.Empty;
    [Name("Text Obj"), MinVersion(7)]
    public Symbol text = Symbol.Empty;

    [Name("Chars Per Inst"), MinVersion(7)]
    public int charsPerInst;
    [Name("Max Text Instances"), MinVersion(7)]
    public int maxTextInstances;

    [Name("Widget Type"), MinVersion(8)]
    public WidgetType widgetType;
    [Name("Mat"), MinVersion(8)]
    public Symbol mat = Symbol.Empty;

    [Name("Text Alignment"), MinVersion(10)]
    public RndText.Alignment textAlignment;

    [Name("Text Color"), Description("Primary color for text instances"), MinVersion(11)]
    public HmxColor4 textColor = new();
    [Name("Alt Text Color"), Description("Secondary color for text instances"), MinVersion(11)]
    public HmxColor4 altTextColor = new();

    [Name("Allow Shift"), Description("Allow widget instances to shift their X/Z coordinates in coordination with their smasher during a keyboard lane shift"), MinVersion(14)]
    public bool allowShift;
    [Name("Allow Line Rotation"), Description("Individual lines can have different rotations"), MinVersion(15)]
    public bool allowLineRotation;


    public TrackWidget(ushort revision = 0, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new TrackWidget Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision != 0)
        {
            draw.Read(reader, false, parent, entry);
        }

        // Blitz moved the widget resources into TrackWidget subclasses
        if (parent.revision == 32 && revision == 1)
        {
            if (standalone)
            {
                AssetHelpers.ReadEnd(reader, parent, entry, revision);
            }

            return this;
        }

        meshes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        if (revision > 4)
        {
            wideWidget = reader.ReadBoolean();
            meshesLeft = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            meshesSpan = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            meshesRight = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        else
        {
            wideWidget = false;
            meshesLeft.Clear();
            meshesSpan.Clear();
            meshesRight.Clear();
        }

        environ = Symbol.Read(reader);

        if (revision > 2)
        {
            baseLength = reader.ReadFloat();
        }
        else
        {
            baseLength = 0f;
        }

        if (revision > 8)
        {
            baseWidth = reader.ReadFloat();
        }
        else
        {
            baseWidth = 0f;
        }

        if (revision >= 2 && revision <= 7)
        {
            shouldUseWideWidget = reader.ReadBoolean();
        }
        else
        {
            shouldUseWideWidget = false;
        }

        if (revision > 3)
        {
            allowRotation = reader.ReadBoolean();
        }
        else
        {
            allowRotation = false;
        }

        if (revision > 5)
        {
            font = Symbol.Read(reader);
            if (revision < 8)
            {
                shouldBeTextWidget = reader.ReadBoolean();
            }
            else
            {
                shouldBeTextWidget = false;
            }
        }
        else
        {
            font = Symbol.Empty;
            shouldBeTextWidget = false;
        }

        if (revision > 6)
        {
            text = Symbol.Read(reader);
            charsPerInst = reader.ReadInt32();
            maxTextInstances = reader.ReadInt32();
        }
        else
        {
            text = Symbol.Empty;
            charsPerInst = 0;
            maxTextInstances = 0;
        }

        if (revision > 7)
        {
            widgetType = (WidgetType)reader.ReadInt32();
            mat = Symbol.Read(reader);
        }

        if (revision > 9)
        {
            textAlignment = (RndText.Alignment)reader.ReadInt32();
        }
        else
        {
            textAlignment = RndText.Alignment.kMiddleCenter;
        }

        if (revision > 10)
        {
            textColor = new HmxColor4().Read(reader);
            altTextColor = new HmxColor4().Read(reader);
        }
        else
        {
            textColor = new HmxColor4(1, 1, 1, 1);
            altTextColor = new HmxColor4(1, 1, 1, 1);
        }

        if (revision > 0xB)
        {
            yOffset = reader.ReadFloat();
        }
        else
        {
            yOffset = 0f;
        }

        if (revision > 0xC)
        {
            xOffset = reader.ReadFloat();
            zOffset = reader.ReadFloat();
        }
        else
        {
            xOffset = 0f;
            zOffset = 0f;
        }

        if (revision > 0xD)
        {
            allowShift = reader.ReadBoolean();
        }
        else
        {
            allowShift = false;
        }

        if (revision > 0xE)
        {
            allowLineRotation = reader.ReadBoolean();
        }
        else
        {
            allowLineRotation = false;
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

        if (revision != 0)
        {
            draw.Write(writer, false, parent, null);
        }

        if (parent.revision == 32 && revision == 1)
        {
            if (standalone)
            {
                writer.WriteEndBytes();
            }

            return;
        }

        AssetHelpers.WriteList(writer, meshes, mesh => Symbol.Write(writer, mesh));

        if (revision > 4)
        {
            writer.WriteBoolean(wideWidget);
            AssetHelpers.WriteList(writer, meshesLeft, mesh => Symbol.Write(writer, mesh));
            AssetHelpers.WriteList(writer, meshesSpan, mesh => Symbol.Write(writer, mesh));
            AssetHelpers.WriteList(writer, meshesRight, mesh => Symbol.Write(writer, mesh));
        }

        Symbol.Write(writer, environ);

        if (revision > 2)
        {
            writer.WriteFloat(baseLength);
        }

        if (revision > 8)
        {
            writer.WriteFloat(baseWidth);
        }

        if (revision >= 2 && revision <= 7)
        {
            writer.WriteBoolean(shouldUseWideWidget);
        }

        if (revision > 3)
        {
            writer.WriteBoolean(allowRotation);
        }

        if (revision > 5)
        {
            Symbol.Write(writer, font);
            if (revision < 8)
            {
                writer.WriteBoolean(shouldBeTextWidget);
            }
        }

        if (revision > 6)
        {
            Symbol.Write(writer, text);
            writer.WriteInt32(charsPerInst);
            writer.WriteInt32(maxTextInstances);
        }

        if (revision > 7)
        {
            writer.WriteInt32((int)widgetType);
            Symbol.Write(writer, mat);
        }

        if (revision > 9)
        {
            writer.WriteInt32((int)textAlignment);
        }

        if (revision > 10)
        {
            textColor.Write(writer);
            altTextColor.Write(writer);
        }

        if (revision > 0xB)
        {
            writer.WriteFloat(yOffset);
        }

        if (revision > 0xC)
        {
            writer.WriteFloat(xOffset);
            writer.WriteFloat(zOffset);
        }

        if (revision > 0xD)
        {
            writer.WriteBoolean(allowShift);
        }

        if (revision > 0xE)
        {
            writer.WriteBoolean(allowLineRotation);
        }


        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public static TrackWidget New(ushort revision, ushort altRevision)
    {
        TrackWidget widget = new();
        widget.revision = revision;
        widget.altRevision = altRevision;
        return widget;
    }
}
