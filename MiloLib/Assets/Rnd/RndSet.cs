using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Set"), Description("A group of objects to propagate animation and messages")]
public class RndSet : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Objects")]
    public List<Symbol> setObjects = new();

    public new RndSet Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        objFields.Read(reader, parent, entry);

        setObjects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

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

        AssetHelpers.WriteList(writer, setObjects, (Symbol setObj) => Symbol.Write(writer, setObj));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
