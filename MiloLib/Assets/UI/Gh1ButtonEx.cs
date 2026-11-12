using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("ButtonEx")]
public class Gh1ButtonEx : Gh1UiComponent
{
    public int unkInteger;
    public Vector3 unkVector = new();
    public Symbol textStyle = Symbol.Empty;
    public byte unkFlag1;
    public byte unkFlag2;
    public float unkFloat1;
    public float unkFloat2;
    [Name("Text")]
    public Symbol text = Symbol.Empty;

    public Gh1ButtonEx() : base(4)
    {
    }

    protected override void ReadFields(EndianReader reader)
    {
        unkInteger = reader.ReadInt32();
        unkVector = new Vector3().Read(reader);
        textStyle = Symbol.Read(reader);
        unkFlag1 = reader.ReadByte();
        unkFlag2 = reader.ReadByte();
        unkFloat1 = reader.ReadFloat();
        unkFloat2 = reader.ReadFloat();
        text = Symbol.Read(reader);
    }

    protected override void WriteFields(EndianWriter writer)
    {
        writer.WriteInt32(unkInteger);
        unkVector.Write(writer);
        Symbol.Write(writer, textStyle);
        writer.WriteByte(unkFlag1);
        writer.WriteByte(unkFlag2);
        writer.WriteFloat(unkFloat1);
        writer.WriteFloat(unkFloat2);
        Symbol.Write(writer, text);
    }
}
