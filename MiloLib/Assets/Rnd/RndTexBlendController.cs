using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("TexBlendController"), Description("Defines the two objects that will be used to determine the distance for the texture blend.")]
public class RndTexBlendController : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Mesh"), Description("The mesh object to render to the texture. This should be an unskinned mesh with UV coordinates that match the source mesh")]
    public Symbol mesh = Symbol.Empty;
    [Name("Object 1"), Description("The first object to use as a distance reference")]
    public Symbol object1 = Symbol.Empty;
    [Name("Object 2"), Description("The second object to use as a distance reference")]
    public Symbol object2 = Symbol.Empty;

    [Name("Base Distance"), Description("The base distance used to compute which blending to use")]
    public float baseDistance;
    [Name("Min Distance"), Description("The distance where the 'near' texture map will be fully visible")]
    public float minDistance;
    [Name("Max Distance"), Description("The distance where the 'far' texture map will be fully visible")]
    public float maxDistance;

    [Name("Override Map"), Description("If set, ignores all other fields and forces 100% blend to it"), MinVersion(2)]
    public Symbol overrideMap = Symbol.Empty;
    public new RndTexBlendController Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        mesh = Symbol.Read(reader);
        object1 = Symbol.Read(reader);
        object2 = Symbol.Read(reader);

        baseDistance = reader.ReadFloat();
        minDistance = reader.ReadFloat();
        maxDistance = reader.ReadFloat();

        if (revision > 1)
        {
            overrideMap = Symbol.Read(reader);
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

        Symbol.Write(writer, mesh);
        Symbol.Write(writer, object1);
        Symbol.Write(writer, object2);

        writer.WriteFloat(baseDistance);
        writer.WriteFloat(minDistance);
        writer.WriteFloat(maxDistance);

        if (revision > 1)
        {
            Symbol.Write(writer, overrideMap);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
