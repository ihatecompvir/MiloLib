using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("UIComponent"), Description("Base class of all UI components, defines navigation and component state")]
public class UIComponent : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndTrans trans = new();
    public RndDrawable draw = new();

    [Name("Nav Right"), Description("Object to navigate to when the right button is pressed"), MinVersion(1)]
    public Symbol navRight = Symbol.Empty;
    [Name("Nav Down"), Description("Object to navigate to when the down button is pressed"), MinVersion(1)]
    public Symbol navDown = Symbol.Empty;
    [Name("Resource Name"), Description("path to resource file for this component"), MinVersion(2)]
    public Symbol resourceName = Symbol.Empty;

    public ushort UIComponentRevision => revision;
    public ushort UIComponentAltRevision => altRevision;

    public void SetUIComponentRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public void SetResourceName(string resourceName)
    {
        this.resourceName = string.IsNullOrEmpty(resourceName)
            ? Symbol.Empty
            : new Symbol((uint)resourceName.Length, resourceName);
    }


    public new UIComponent Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        trans = trans.Read(reader, false, parent, entry);
        draw = draw.Read(reader, false, parent, entry);

        if (revision > 0)
        {
            navRight = Symbol.Read(reader);
            navDown = Symbol.Read(reader);
        }

        if (revision == 2)
        {
            resourceName = Symbol.Read(reader);
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

        trans.Write(writer, false, parent, null);
        draw.Write(writer, false, parent, null);

        if (revision > 0)
        {
            Symbol.Write(writer, navRight);
            Symbol.Write(writer, navDown);
        }

        if (revision == 2)
        {
            Symbol.Write(writer, resourceName);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
