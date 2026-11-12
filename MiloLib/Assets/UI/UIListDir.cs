using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIListDir"), Description("Top-level resource object for UILists")]
public class UIListDir : RndDir
{
    public enum UIListOrientation
    {
        kUIListVertical,
        kUIListHorizontal
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Orientation"), Description("scroll direction of list")]
    public UIListOrientation orientation;

    [Name("Fade Offset"), Description("Number of elements to fade from beginning/end of list")]
    public int fadeOffset;
    [Name("Element Spacing"), Description("spacing between elements")]
    public float elementSpacing;
    [Name("Scroll Highlight Change"), Description("point during scroll when highlight changes"), MinVersion(1)]
    public float scrollHighlightChange;
    [Name("Test Mode"), Description("draw widgets in preview mode?")]
    public bool testMode;
    [Name("Test Num Data"), Description("total number of data elements")]
    public int testNumData;
    [Name("Test Gap Size"), Description("test gaps between elements")]
    public float testGapSize;
    [Name("Test Disable Elements"), Description("test disable every other element")]
    public bool testDisableElements;
    [Name("Test Num Display"), Description("number of elements to draw")]
    public int numDisplay;
    [Name("Test Scroll Time"), Description("test scroll time")]
    public float speed;
    [Name("Test List State")]
    public uint compState;

    public UIListDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new UIListDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        orientation = (UIListOrientation)reader.ReadUInt32();
        fadeOffset = reader.ReadInt32();
        testMode = reader.ReadBoolean();
        numDisplay = reader.ReadInt32();
        elementSpacing = reader.ReadFloat();
        speed = reader.ReadFloat();
        testNumData = reader.ReadInt32();
        compState = reader.ReadUInt32();
        testGapSize = reader.ReadFloat();
        testDisableElements = reader.ReadBoolean();

        if (revision != 0)
        {
            scrollHighlightChange = reader.ReadFloat();
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

        writer.WriteUInt32((uint)orientation);
        writer.WriteInt32(fadeOffset);
        writer.WriteBoolean(testMode);
        writer.WriteInt32(numDisplay);
        writer.WriteFloat(elementSpacing);
        writer.WriteFloat(speed);
        writer.WriteInt32(testNumData);
        writer.WriteUInt32(compState);
        writer.WriteFloat(testGapSize);
        writer.WriteBoolean(testDisableElements);

        if (revision != 0)
        {
            writer.WriteFloat(scrollHighlightChange);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }
}
