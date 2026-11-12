using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharSleeve"), Description("Animates top and bottom sleeve bones. Both must have the same local xfm when transformed, and the parent bone must point along the X axis. The local positions of both bones must only have z be non null.")]
public class CharSleeve : Object
{
    private ushort revision;
    private ushort altRevision;

    [Name("Sleeve"), Description("The sleeve bone")]
    public Symbol sleeve = Symbol.Empty;
    [Name("Top Sleeve"), Description("The top sleeve bone, has zero range")]
    public Symbol topSleeve = Symbol.Empty;
    [Name("Inertia"), Description("Intertia 0-1, .5 is default")]
    public float inertia;
    [Name("Gravity"), Description("Force of gravity, 1 is normal")]
    public float gravity;
    [Name("Stiffness"), Description("how stiff it is for length and range, 1 is max")]
    public float stiffness;
    [Name("Range")]
    public float range;
    [Name("Neg Length"), Description("how much negative length it can go")]
    public float negLength;
    [Name("Pos Length"), Description("how much longer it can stretch than its length")]
    public float posLength;

    public new CharSleeve Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        sleeve = Symbol.Read(reader);
        topSleeve = Symbol.Read(reader);
        inertia = reader.ReadFloat();
        gravity = reader.ReadFloat();
        stiffness = reader.ReadFloat();
        range = reader.ReadFloat();
        negLength = reader.ReadFloat();
        posLength = reader.ReadFloat();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, sleeve);
        Symbol.Write(writer, topSleeve);
        writer.WriteFloat(inertia);
        writer.WriteFloat(gravity);
        writer.WriteFloat(stiffness);
        writer.WriteFloat(range);
        writer.WriteFloat(negLength);
        writer.WriteFloat(posLength);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
