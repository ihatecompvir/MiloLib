using MiloLib.Classes;
using MiloLib.Utils;
using System.Text;

namespace MiloLib.Assets;


public class ObjectFields
{
    public enum NodeType : int
    {
        Int = 0x00,
        Float = 0x01,
        Variable = 0x02,
        Func = 0x03,
        Object = 0x04,
        Symbol = 0x05,
        Unhandled = 0x06,
        IfDef = 0x07,
        Else = 0x08,
        EndIf = 0x09,
        Array = 0x10,
        Command = 0x11,
        String = 0x12,
        Property = 0x13,
        Define = 0x20,
        Include = 0x21,
        Merge = 0x22,
        IfNDef = 0x23,
        Autorun = 0x24,
        Undef = 0x25
    }

    public struct DTBArrayParent
    {
        private ushort childCount;
        public uint id;

        public List<DTBNode> children;
        public override string ToString()
        {
            return ToString(0);
        }
        private string ToString(int indentLevel)
        {
            StringBuilder sb = new();
            string indent = new(' ', indentLevel * 4);
            sb.AppendLine($"{indent}Parent (ID: {id}, Children: {childCount})");

            if (children != null)
            {
                foreach (DTBNode node in children)
                {
                    if (node.value is DTBArrayParent parent)
                    {
                        sb.Append(parent.ToString(indentLevel + 1));
                    }
                    else
                    {
                        sb.AppendLine($"{indent}    {node.ToString()}");
                    }
                }
            }


            return sb.ToString();
        }

        public void Read(EndianReader reader)
        {
            childCount = reader.ReadUInt16();
            id = reader.ReadUInt32();
            children = new List<DTBNode>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                DTBNode node = new();
                node.Read(reader);
                children.Add(node);
            }
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt16((ushort)children.Count);
            writer.WriteUInt32(id);
            foreach (DTBNode node in children)
            {
                node.Write(writer);
            }
        }
    }

    public struct DTBNode
    {
        public NodeType type;
        public object value;

        public void Read(EndianReader reader)
        {
            type = (NodeType)reader.ReadInt32();
            switch (type)
            {
                case NodeType.Int:
                    value = reader.ReadUInt32();
                    break;
                case NodeType.Float:
                    value = reader.ReadFloat();
                    break;
                case NodeType.Variable:
                case NodeType.Object:
                case NodeType.Symbol:
                case NodeType.Unhandled:
                case NodeType.IfDef:
                case NodeType.Else:
                case NodeType.EndIf:
                case NodeType.String:
                case NodeType.Define:
                case NodeType.Include:
                case NodeType.Merge:
                case NodeType.IfNDef:
                case NodeType.Autorun:
                case NodeType.Undef:
                    value = Symbol.Read(reader);
                    break;
                case NodeType.Array:
                case NodeType.Command:
                case NodeType.Property:
                    DTBArrayParent parent = new();
                    parent.Read(reader);
                    value = parent;
                    break;
                default:
                    value = null;
                    break;
            }
        }
        public void Write(EndianWriter writer)
        {
            writer.WriteInt32((int)type);
            switch (type)
            {
                case NodeType.Int:
                    writer.WriteUInt32((uint)value);
                    break;
                case NodeType.Float:
                    writer.WriteFloat((float)value);
                    break;
                case NodeType.Variable:
                case NodeType.Object:
                case NodeType.Symbol:
                case NodeType.Unhandled:
                case NodeType.IfDef:
                case NodeType.Else:
                case NodeType.EndIf:
                case NodeType.String:
                case NodeType.Define:
                case NodeType.Include:
                case NodeType.Merge:
                case NodeType.IfNDef:
                case NodeType.Autorun:
                case NodeType.Undef:
                    Symbol.Write(writer, (Symbol)value);
                    break;
                case NodeType.Array:
                case NodeType.Command:
                case NodeType.Property:
                    ((DTBArrayParent)value).Write(writer);
                    break;
            }
        }
        public override string ToString()
        {
            return $"{type}: {value}";
        }
    }

    public struct DTBParent
    {
        [Name("Has Any TypeProps"), Description("Whether or not the object contains any TypeProps.")]
        public bool hasTree;

        private ushort childCount;
        public uint id;
        public List<DTBNode> children;

        public void Read(EndianReader reader)
        {
            hasTree = reader.ReadBoolean();
            if (!hasTree)
            {
                return;
            }

            childCount = reader.ReadUInt16();
            id = reader.ReadUInt32();
            children = new List<DTBNode>(childCount);
            for (int i = 0; i < childCount; i++)
            {
                DTBNode node = new();
                node.Read(reader);
                children.Add(node);
            }
        }
        public void Write(EndianWriter writer)
        {
            writer.WriteBoolean(hasTree);
            if (!hasTree)
            {
                return;
            }

            writer.WriteUInt16((ushort)children.Count);
            writer.WriteUInt32(id);
            foreach (DTBNode node in children)
            {
                node.Write(writer);
            }
        }

        public override string ToString()
        {
            return ToString(0);
        }
        private string ToString(int indentLevel)
        {
            StringBuilder sb = new();
            string indent = new(' ', indentLevel * 4);
            sb.AppendLine($"{indent}Parent (ID: {id}, Children: {childCount})");

            if (children != null)
            {
                foreach (DTBNode node in children)
                {
                    if (node.value is DTBParent parent)
                    {
                        sb.Append(parent.ToString(indentLevel + 1));
                    }
                    else
                    {
                        sb.AppendLine($"{indent}    {node.ToString()}");
                    }
                }
            }


            return sb.ToString();
        }
    }

    [Name("Alt Revision"), Description("The alternate revision of the Object.")]
    private ushort altRevision;

    [Name("Revision"), Description("The revision of the Object.")]
    private ushort revision;

    public ushort ObjectFieldsRevision => revision;
    public ushort ObjectFieldsAltRevision => altRevision;

    public void SetObjectFieldsRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    [Name("Type"), Description("The subtype of the object. Not the same as the asset type.")]
    public Symbol type = Symbol.Empty;

    [Name("Root Node"), Description("Root node of the DTB tree")]
    public DTBParent root;

    // only appears in GH2 360 and later
    [Name("Note"), Description("A free-form text field that can contain any information."), MinVersion(1)]
    public Symbol note = Symbol.Empty;

    public ObjectFields Read(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        if (parent.revision <= 10)
        {
            return this;
        }

        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        type = Symbol.Read(reader);
        root.Read(reader);

        if (revision > 0)
        {
            note = Symbol.Read(reader);
        }

        return this;
    }

    public void Write(EndianWriter writer, DirectoryMeta parent)
    {
        if (parent.revision <= 10)
        {
            return;
        }

        AssetHelpers.WriteRevision(writer, revision, altRevision);
        Symbol.Write(writer, type);
        root.Write(writer);

        if (revision > 0)
        {
            Symbol.Write(writer, note);
        }
    }

    public override string ToString()
    {
        if (revision < 1)
        {
            return type.ToString();
        }
        else
        {
            return string.Format("{0} {1}", type, note);
        }
    }
}

[Name("Object"), Description("The Object class is the root of the class hierarchy. Every class has Object as a superclass.")]
public class Object
{

    [Name("Object Fields"), Description("The Hmx::Object fields that all Objects have.")]
    public ObjectFields objFields = new();

    public Object Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        objFields = new ObjectFields().Read(reader, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry);
        }

        return this;
    }

    public virtual void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        objFields.Write(writer, parent);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public virtual bool IsDirectory()
    {
        return false;
    }
}
