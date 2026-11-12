using MiloLib.Utils;
using static MiloLib.Assets.ObjectFields;

namespace MiloLib.Assets.Flow;

public class FlowValue
{
    public NodeType serializedType;
    public DTBNode node = new() { value = 0u };

    public void Read(EndianReader reader, bool tagged)
    {
        if (tagged)
        {
            serializedType = (NodeType)reader.ReadInt32();
        }
        if (tagged && serializedType == NodeType.Object)
        {
            node = new DTBNode { type = NodeType.Object, value = Symbol.Read(reader) };
        }
        else
        {
            node.Read(reader);
        }
    }

    public void Write(EndianWriter writer, bool tagged)
    {
        if (tagged)
        {
            writer.WriteInt32((int)serializedType);
        }
        if (tagged && serializedType == NodeType.Object)
        {
            Symbol.Write(writer, (Symbol)node.value);
        }
        else
        {
            node.Write(writer);
        }
    }
}
