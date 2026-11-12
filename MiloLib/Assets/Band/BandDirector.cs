using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandDirector"), Description("Band Director, sits in each song file and manages camera + scene changes")]
public class BandDirector : Object
{
    private ushort altRevision;
    private ushort revision;

    public Object poll = new();
    public RndDrawable draw = new();
    [MaxVersion(4)]
    public Object obj3 = new();
    [MaxVersion(5)]
    public Symbol unkSym = Symbol.Empty;
    [MaxVersion(3)]
    public Symbol unkSym2 = Symbol.Empty;

    public new BandDirector Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        poll = poll.Read(reader, false, parent, entry);
        draw = draw.Read(reader, false, parent, entry);
        if (revision < 5)
        {
            obj3 = obj3.Read(reader, false, parent, entry);
        }

        if (revision < 6)
        {
            unkSym = Symbol.Read(reader);
        }

        if (revision < 4)
        {
            unkSym2 = Symbol.Read(reader);
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

        base.Write(writer, false, parent, entry);

        poll.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, null);
        if (revision < 5)
        {
            obj3.Write(writer, false, parent, entry);
        }

        if (revision < 6)
        {
            Symbol.Write(writer, unkSym);
        }

        if (revision < 4)
        {
            Symbol.Write(writer, unkSym2);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
