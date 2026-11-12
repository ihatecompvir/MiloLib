using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("Slider")]
public class Gh1Slider : Gh1UiComponent
{
    public Symbol sliderType = Symbol.Empty;
    [Name("Text")]
    public Symbol text = Symbol.Empty;
    public int unkInteger;

    public Gh1Slider() : base(1)
    {
    }

    protected override void ReadFields(EndianReader reader)
    {
        sliderType = Symbol.Read(reader);
        text = Symbol.Read(reader);
        unkInteger = reader.ReadInt32();
    }

    protected override void WriteFields(EndianWriter writer)
    {
        Symbol.Write(writer, sliderType);
        Symbol.Write(writer, text);
        writer.WriteInt32(unkInteger);
    }
}
