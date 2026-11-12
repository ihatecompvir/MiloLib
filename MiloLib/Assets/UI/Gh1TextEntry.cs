using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("TextEntry")]
public class Gh1TextEntry : Gh1UiComponent
{
    public Symbol entryType = Symbol.Empty;

    public Gh1TextEntry() : base(0)
    {
    }

    protected override void ReadFields(EndianReader reader)
    {
        entryType = Symbol.Read(reader);
    }

    protected override void WriteFields(EndianWriter writer)
    {
        Symbol.Write(writer, entryType);
    }
}
