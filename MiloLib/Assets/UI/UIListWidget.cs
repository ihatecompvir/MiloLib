using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIListWidget"), Description("Base functionality for UIList widgets")]
public class UIListWidget : Object
{
    public enum UIListWidgetState
    {
        kUIListWidgetActive,
        kUIListWidgetHighlight,
        kUIListWidgetInactive,
        kNumUIListWidgetStates
    };

    public enum UIListWidgetDrawType
    {
        kUIListWidgetDrawAlways,
        kUIListWidgetDrawOnlyFocused,
        kUIListWidgetDrawFocusedOrManual,
        kNumUIListWidgetDrawTypes
    };
    private ushort altRevision;
    private ushort revision;

    [Name("Draw Order"), Description("order this widget will be drawn")]
    public float drawOrder;

    [MaxVersion(0)]
    public int unkInt1;
    [MaxVersion(0)]
    public int unkInt2;
    [MaxVersion(0)]
    public int unkInt3;

    [Name("Disabled Alpha Scale"), Description("scale for widget alpha when list is disabled"), MinVersion(2)]
    public float disabledAlphaScale;

    [Name("Widget Draw Type"), Description("under what conditions to draw this widget")]
    public UIListWidgetDrawType drawType;

    public List<Symbol> colorPtrs = new();

    [Name("Default Color"), Description("color applied if no state specific color is set")]
    public Symbol defaultColor = Symbol.Empty;

    public new UIListWidget Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        drawOrder = reader.ReadFloat();

        if (revision < 1)
        {
            unkInt1 = reader.ReadInt32();
            unkInt3 = reader.ReadInt32();
        }

        defaultColor = Symbol.Read(reader);
        drawType = (UIListWidgetDrawType)reader.ReadInt32();

        if (revision >= 2)
        {
            disabledAlphaScale = reader.ReadFloat();
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                colorPtrs.Add(Symbol.Read(reader));
            }
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

        writer.WriteFloat(drawOrder);

        if (revision < 1)
        {
            writer.WriteInt32(unkInt1);
            writer.WriteInt32(unkInt3);
        }

        Symbol.Write(writer, defaultColor);
        writer.WriteInt32((int)drawType);

        if (revision >= 2)
        {
            writer.WriteFloat(disabledAlphaScale);
        }

        while (colorPtrs.Count < 15)
        {
            colorPtrs.Add(Symbol.Empty);
        }
        if (colorPtrs.Count > 15)
        {
            colorPtrs = colorPtrs.Take(15).ToList();
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                Symbol.Write(writer, colorPtrs[i * 5 + j]);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
