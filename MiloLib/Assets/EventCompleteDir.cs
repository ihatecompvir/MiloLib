using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("EventCompleteDir"), Description("PanelDir for tour event complete screens")]
public class EventCompleteDir : PanelDir
{
    public class Item
    {
        [Name("Anim")]
        public Symbol animation = Symbol.Empty;
        [Name("Icon Label")]
        public Symbol label1 = Symbol.Empty;
        [Name("Icon Label Str")]
        public string text1 = string.Empty;
        [Name("Text Label")]
        public Symbol label2 = Symbol.Empty;
        [Name("Text Label Str")]
        public string text2 = string.Empty;
        [Name("Text Label Start")]
        public uint unkInt1;
        [Name("Text Label End")]
        public uint unkInt2;
        [Name("Sfx"), MinVersion(3)]
        public Symbol sequence1 = Symbol.Empty;
        [Name("Active")]
        public bool unkBool;
        [Name("Moment Success Group"), MinVersion(1)]
        public Symbol group1 = Symbol.Empty;
        [Name("Moment Fail Group"), MinVersion(1)]
        public Symbol group2 = Symbol.Empty;
        [Name("Moment Name"), MinVersion(4)]
        public Symbol unkSymbol = Symbol.Empty;
        [Name("Moment Success Label"), MinVersion(4)]
        public Symbol label3 = Symbol.Empty;
        [Name("Moment Fail Label"), MinVersion(4)]
        public Symbol label4 = Symbol.Empty;
        [Name("Moment Str"), MinVersion(4)]
        public string text3 = string.Empty;
        [Name("Moment End"), MinVersion(4)]
        public uint unkInt3;
        [Name("Moment Sfx"), MinVersion(3)]
        public Symbol sequence2 = Symbol.Empty;
        [Name("Moment State"), MinVersion(1)]
        public uint unkInt4;

        public void Read(EndianReader reader, ushort revision)
        {
            animation = Symbol.Read(reader);
            label1 = Symbol.Read(reader);
            text1 = ReadString(reader);
            label2 = Symbol.Read(reader);
            text2 = ReadString(reader);
            unkInt1 = reader.ReadUInt32();
            unkInt2 = reader.ReadUInt32();
            if (revision > 2)
            {
                sequence1 = Symbol.Read(reader);
            }
            unkBool = reader.ReadBoolean();
            if (revision > 0)
            {
                group1 = Symbol.Read(reader);
                group2 = Symbol.Read(reader);
            }
            if (revision > 3)
            {
                unkSymbol = Symbol.Read(reader);
                label3 = Symbol.Read(reader);
                label4 = Symbol.Read(reader);
                text3 = ReadString(reader);
                unkInt3 = reader.ReadUInt32();
            }
            if (revision > 2)
            {
                sequence2 = Symbol.Read(reader);
            }
            if (revision > 0)
            {
                unkInt4 = reader.ReadUInt32();
            }
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, animation);
            Symbol.Write(writer, label1);
            WriteString(writer, text1);
            Symbol.Write(writer, label2);
            WriteString(writer, text2);
            writer.WriteUInt32(unkInt1);
            writer.WriteUInt32(unkInt2);
            if (revision > 2)
            {
                Symbol.Write(writer, sequence1);
            }
            writer.WriteBoolean(unkBool);
            if (revision > 0)
            {
                Symbol.Write(writer, group1);
                Symbol.Write(writer, group2);
            }
            if (revision > 3)
            {
                Symbol.Write(writer, unkSymbol);
                Symbol.Write(writer, label3);
                Symbol.Write(writer, label4);
                WriteString(writer, text3);
                writer.WriteUInt32(unkInt3);
            }
            if (revision > 2)
            {
                Symbol.Write(writer, sequence2);
            }
            if (revision > 0)
            {
                writer.WriteUInt32(unkInt4);
            }
        }

        private static string ReadString(EndianReader reader)
        {
            return Symbol.Read(reader).value;
        }

        private static void WriteString(EndianWriter writer, string value)
        {
            Symbol.Write(writer, value ?? string.Empty);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Items")]
    public List<Item> items = new();
    [MinVersion(2)]
    public bool unkBool;
    [Name("Initial Delay")]
    public float unkFloat1;
    [Name("Anim Time")]
    public float unkFloat2;
    [Name("Count Time")]
    public float unkFloat3;
    [Name("Moment Time"), MinVersion(1)]
    public float unkFloat4;
    [Name("Challenge List")]
    public Symbol unkObject = Symbol.Empty;

    public EventCompleteDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new EventCompleteDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        base.Read(reader, false, parent, entry);
        items = AssetHelpers.ReadList(reader, () => { Item item = new(); item.Read(reader, revision); return item; });
        if (revision > 1)
        {
            unkBool = reader.ReadBoolean();
        }
        unkFloat1 = reader.ReadFloat();
        unkFloat2 = reader.ReadFloat();
        unkFloat3 = reader.ReadFloat();
        if (revision > 0)
        {
            unkFloat4 = reader.ReadFloat();
        }
        unkObject = Symbol.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, items, (Item item) => item.Write(writer, revision));
        if (revision > 1)
        {
            writer.WriteBoolean(unkBool);
        }
        writer.WriteFloat(unkFloat1);
        writer.WriteFloat(unkFloat2);
        writer.WriteFloat(unkFloat3);
        if (revision > 0)
        {
            writer.WriteFloat(unkFloat4);
        }
        Symbol.Write(writer, unkObject);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }
}
