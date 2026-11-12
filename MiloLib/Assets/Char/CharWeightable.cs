using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharWeightable"), Description("Base class for any object that can have weight set on it, or have a Weightable parent from which it gets that state.")]
public class CharWeightable : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Weight"), Description("Weight to blend in by")]
    public float weight;
    [MinVersion(2), Name("Weight Owner"), Description("object to get weight from")]
    public Symbol weightOwner = Symbol.Empty;

    public new CharWeightable Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        weight = reader.ReadFloat();
        if (revision > 1)
        {
            weightOwner = Symbol.Read(reader);
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

        writer.WriteFloat(weight);
        if (revision > 1)
        {
            Symbol.Write(writer, weightOwner);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
