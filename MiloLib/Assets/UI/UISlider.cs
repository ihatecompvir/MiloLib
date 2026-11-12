using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UISlider"), Description("A component with animatable whose frames correspond to a range of values. The resources don't have to look like a slider; they could easily be a knob, dial, etc.")]
public class UISlider : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Require Selection To Scroll"), Description("Does slider need to be selected before user can scroll?"), MinVersion(1)]
    public bool selectToShow;

    [Name("Slider Resource"), MinVersion(3)]
    public Symbol sliderResource = Symbol.Empty;

    [Name("Vertical"), Description("vertical or horizontal scrolling?"), MinVersion(2)]
    public bool vertical;

    public new UISlider Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision >= 3)
        {
            sliderResource = Symbol.Read(reader);
        }

        if (revision != 0)
        {
            selectToShow = reader.ReadBoolean();
        }

        if (revision >= 2)
        {
            vertical = reader.ReadBoolean();
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

        if (revision >= 3)
        {
            Symbol.Write(writer, sliderResource);
        }

        if (revision != 0)
        {
            writer.WriteBoolean(selectToShow);
        }

        if (revision >= 2)
        {
            writer.WriteBoolean(vertical);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
