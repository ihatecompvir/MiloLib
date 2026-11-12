using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("TransProxy"), Description("Stand-in for a RndTransformable inside of a proxy, so you can use it")]
public class RndTransProxy : Object
{
    private ushort altRevision;
    private ushort revision;

    [MinVersion(1)]
    public RndTrans trans = new();
    [Name("Proxy"), Description("Proxy object this will look into.")]
    public Symbol proxy = Symbol.Empty;
    [Name("Part"), Description("The part inside it")]
    public Symbol part = Symbol.Empty;

    public new RndTransProxy Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision != 0)
        {
            trans = trans.Read(reader, false, parent, entry);
        }

        proxy = Symbol.Read(reader);
        part = Symbol.Read(reader);

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

        if (revision != 0)
        {
            trans.Write(writer, false, parent, null);
        }

        Symbol.Write(writer, proxy);
        Symbol.Write(writer, part);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
