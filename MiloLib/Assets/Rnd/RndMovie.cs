using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Movie"), Description("A Movie object animates a compressed video through a texture. It works on Xbox and PS2 now, soon GC.")]
public class RndMovie : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    [Name("Movie File"), Description("File - extension is automatically converted to platform")]
    public Symbol movieFile = Symbol.Empty;

    [Name("Stream"), Description("Stream the file for this movie?"), MinVersion(5)]
    public bool stream;

    [Name("Loop"), Description("Loop this movie?"), MinVersion(6)]
    public bool loop = true;

    [Name("Tex"), Description("Texture to animate."), MinVersion(4)]
    public Symbol tex = Symbol.Empty;

    public new RndMovie Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 6)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);
        movieFile = Symbol.Read(reader);

        if (revision > 3)
        {
            tex = Symbol.Read(reader);
        }

        if (revision > 4)
        {
            stream = reader.ReadBoolean();
        }

        if (revision > 5)
        {
            loop = reader.ReadBoolean();
        }
        else if (revision == 5)
        {
            loop = !stream;
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision > 6)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);
        Symbol.Write(writer, movieFile);

        if (revision > 3)
        {
            Symbol.Write(writer, tex);
        }

        if (revision > 4)
        {
            writer.WriteBoolean(stream);
        }

        if (revision > 5)
        {
            writer.WriteBoolean(loop);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
