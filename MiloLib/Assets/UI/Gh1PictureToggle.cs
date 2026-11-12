using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("PictureToggle")]
public class Gh1PictureToggle : Gh1UiComponent
{
    [Name("Element")]
    public Symbol element = Symbol.Empty;

    public Gh1PictureToggle() : base(0)
    {
    }

    protected override void ReadFields(EndianReader reader)
    {
        element = Symbol.Read(reader);
    }

    protected override void WriteFields(EndianWriter writer)
    {
        Symbol.Write(writer, element);
    }
}
