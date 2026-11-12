using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("LabelShrinkWrapper"), Description("a mesh shrink wrapped to selected label")]
public class LabelShrinkWrapper : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Label"), Description("label to be shrink wrapped")]
    public Symbol label = Symbol.Empty;
    [Name("Show"), Description("Should this be shown?")]
    public bool show;

    [Name("Resource"), Description("Resource file, contains corner bones"), MinVersion(1)]
    public Symbol resource = Symbol.Empty;
    [Name("Left Border"), Description("Left border amount"), MinVersion(2)]
    public float leftBorder;
    [Name("Right Border"), Description("Right border amount"), MinVersion(2)]
    public float rightBorder;
    [Name("Top Border"), Description("Top border amount"), MinVersion(2)]
    public float topBorder;
    [Name("Bottom Border"), Description("Bottom border amount"), MinVersion(2)]
    public float bottomBorder;

    public new LabelShrinkWrapper Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        label = Symbol.Read(reader);
        show = reader.ReadBoolean();

        if (revision >= 1)
        {
            resource = Symbol.Read(reader);
        }
        if (revision >= 2)
        {
            leftBorder = reader.ReadFloat();
            rightBorder = reader.ReadFloat();
            topBorder = reader.ReadFloat();
            bottomBorder = reader.ReadFloat();
        }

        base.Read(reader, false, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        Symbol.Write(writer, label);
        writer.WriteBoolean(show);

        if (revision >= 1)
        {
            Symbol.Write(writer, resource);
        }
        if (revision >= 2)
        {
            writer.WriteFloat(leftBorder);
            writer.WriteFloat(rightBorder);
            writer.WriteFloat(topBorder);
            writer.WriteFloat(bottomBorder);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
