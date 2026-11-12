using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Font3d"), Description("Font objects determine the appearance for Text objects.")]
public class RndFont3d : RndFont
{
    private ushort revision;
    private ushort altRevision;
    public Vector3 size = new();
    public Vector3 inverseSize = new();
    public Vector3 offset = new();
    public List<Glyph> glyphs = new();

    public class Glyph
    {
        public ushort character;
        public Vector3 minimum = new();
        public Vector3 maximum = new();
        public float advance;
        public Symbol mesh = Symbol.Empty;
        public bool oldFlag;

        public Glyph Read(EndianReader reader)
        {
            character = reader.ReadUInt16();
            minimum.Read(reader);
            maximum.Read(reader);
            advance = reader.ReadFloat();
            mesh = Symbol.Read(reader);
            oldFlag = reader.ReadBoolean();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt16(character);
            minimum.Write(writer);
            maximum.Write(writer);
            writer.WriteFloat(advance);
            Symbol.Write(writer, mesh);
            writer.WriteBoolean(oldFlag);
        }
    }

    public new RndFont3d Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        ReadFontBase(reader, parent, entry);
        mat = Symbol.Read(reader);
        textureOwner = Symbol.Read(reader);
        size.Read(reader);
        inverseSize.Read(reader);
        offset.Read(reader);
        glyphs = AssetHelpers.ReadList(reader, () => new Glyph().Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        WriteFontBase(writer, parent, entry);
        Symbol.Write(writer, mat);
        Symbol.Write(writer, textureOwner);
        size.Write(writer);
        inverseSize.Write(writer);
        offset.Write(writer);
        AssetHelpers.WriteList(writer, glyphs, glyph => glyph.Write(writer));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
