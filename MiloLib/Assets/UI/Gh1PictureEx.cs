using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("PictureEx")]
public class Gh1PictureEx : Gh1UiComponent
{
    [Name("Texture")]
    public Symbol texture = Symbol.Empty;

    public Gh1PictureEx() : base(1)
    {
    }

    protected override void ReadFields(EndianReader reader)
    {
        texture = Symbol.Read(reader);
    }

    protected override void WriteFields(EndianWriter writer)
    {
        Symbol.Write(writer, texture);
    }
}
