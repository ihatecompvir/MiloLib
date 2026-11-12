using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("List")]
public class Gh1List : Gh1UiComponent
{
    [Name("List Type")]
    public Symbol listType = Symbol.Empty;
    public int unkInteger1;
    public float unkFloat;
    public int unkInteger2;

    public Gh1List() : base(2)
    {
    }

    protected override void ReadFields(EndianReader reader)
    {
        listType = Symbol.Read(reader);
        unkInteger1 = reader.ReadInt32();
        unkFloat = reader.ReadFloat();
        unkInteger2 = reader.ReadInt32();
    }

    protected override void WriteFields(EndianWriter writer)
    {
        Symbol.Write(writer, listType);
        writer.WriteInt32(unkInteger1);
        writer.WriteFloat(unkFloat);
        writer.WriteInt32(unkInteger2);
    }
}
