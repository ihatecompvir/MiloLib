using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("InlineHelp"), Description("Inline Help")]
public class InlineHelp : UIComponent
{
    public class ActionElement
    {
        [Name("Action")]
        public int joypadAction;
        [Name("Text Token")]
        public Symbol symbol = Symbol.Empty;
        [Name("Secondary Token")]
        public Symbol symbol2 = Symbol.Empty;

        public ActionElement Read(EndianReader reader, uint revision)
        {
            joypadAction = reader.ReadInt32();
            symbol = Symbol.Read(reader);
            if (revision >= 2)
            {
                symbol2 = Symbol.Read(reader);
            }

            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            writer.WriteInt32(joypadAction);
            Symbol.Write(writer, symbol);
            if (revision >= 2)
            {
                Symbol.Write(writer, symbol2);
            }
        }

        public override string ToString()
        {
            return $"{joypadAction} {symbol} {symbol2}";
        }
    }
    private ushort altRevision;
    private ushort revision;

    [Name("Horizontal")]
    public bool horizontal;
    [Name("Spacing")]
    public float spacing;

    [MinVersion(2), MaxVersion(3)]
    public int unkInt;

    [Name("Config")]
    public List<ActionElement> elements = new();

    [Name("Text Color"), Description("UIColor of text"), MinVersion(1)]
    public Symbol textColorObject = Symbol.Empty;

    [Name("Use Connected Controllers"), Description("Use all connected controllers for button icon, rather than just joined (rare)"), MinVersion(3)]
    public bool useConnectedControllers;

    [Name("Resource"), Description("resource, contains UILabel specified by (text_label ...) in the typedef"), MinVersion(5)]
    public Symbol resource = Symbol.Empty;

    public new InlineHelp Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        horizontal = reader.ReadBoolean();
        spacing = reader.ReadFloat();

        elements = AssetHelpers.ReadList(reader, () => new ActionElement().Read(reader, revision));

        if (revision >= 1)
        {
            textColorObject = Symbol.Read(reader);
        }

        // ?
        if (revision >= 2 && revision <= 3)
        {
            unkInt = reader.ReadInt32();
        }

        if (revision >= 3)
        {
            useConnectedControllers = reader.ReadBoolean();
        }

        if (revision >= 5)
        {
            resource = Symbol.Read(reader);
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

        writer.WriteBoolean(horizontal);
        writer.WriteFloat(spacing);

        AssetHelpers.WriteList(writer, elements, element => element.Write(writer, revision));

        if (revision >= 1)
        {
            Symbol.Write(writer, textColorObject);
        }

        // ?
        if (revision >= 2 && revision <= 3)
        {
            writer.WriteInt32(unkInt);
        }

        if (revision >= 3)
        {
            writer.WriteBoolean(useConnectedControllers);
        }

        if (revision >= 5)
        {
            Symbol.Write(writer, resource);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
