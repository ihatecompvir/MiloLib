using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("HorizonDir")]
public class HorizonDir : PanelDir
{
    private ushort revision;
    private ushort altRevision;
    public List<Section> sections = new();
    public Symbol gradientMesh = Symbol.Empty;

    public class Section
    {
        public Symbol panelFile = Symbol.Empty;
        public HmxColor4 color1 = new();
        public HmxColor4 color2 = new();
        public HmxColor4 color3 = new();
        public HmxColor4 color4 = new();

        public Section Read(EndianReader reader)
        {
            panelFile = Symbol.Read(reader);
            color1.Read(reader);
            color2.Read(reader);
            color3.Read(reader);
            color4.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, panelFile);
            color1.Write(writer);
            color2.Write(writer);
            color3.Write(writer);
            color4.Write(writer);
        }
    }

    public HorizonDir(ushort revision = 2, ushort altRevision = 0) : base(2)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    private void ValidateLayout(DirectoryMeta parent)
    {
        if (parent.revision != 26 || revision != 2)
        {
            throw new NotSupportedException($"Unsupported HorizonDir layout: scene {parent.revision}, revision {revision}.");
        }
    }

    public new HorizonDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        ValidateLayout(parent);
        sections = AssetHelpers.ReadList(reader, () => new Section().Read(reader));

        gradientMesh = Symbol.Read(reader);
        base.Read(reader, false, parent, entry);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateLayout(parent);
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        AssetHelpers.WriteList(writer, sections, item => item.Write(writer));
        Symbol.Write(writer, gradientMesh);
        base.Write(writer, false, parent, entry);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
