using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharServoBone"), Description("Sets bone transforms and regulates Character center to a spot.")]
public class CharServoBone : Object
{
    private ushort altRevision;
    private ushort revision;

    [MinVersion(2)]
    public Symbol unkSym = Symbol.Empty;


    public new CharServoBone Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 0)
        {
            base.Read(reader, false, parent, entry);
        }

        if (revision > 1)
        {
            unkSym = Symbol.Read(reader);
        }

        if (standalone && revision != 0)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision != 0)
        {
            base.Write(writer, false, parent, entry);
        }

        if (revision > 1)
        {
            Symbol.Write(writer, unkSym);
        }

        if (standalone && revision != 0)
        {
            writer.WriteEndBytes();
        }
    }

}
