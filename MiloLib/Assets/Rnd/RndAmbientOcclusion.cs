using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("AmbientOcclusion"), Description("Computes ambient occlusion and automatic tessellation. Also stores AO configuration options.")]
public class RndAmbientOcclusion : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Don't Receive AO"), Description("These objects will NOT receive shadows.")]
    public List<Symbol> objList1 = new();
    [Name("Don't Cast AO"), Description("These objects will NOT cast shadows.")]
    public List<Symbol> objList2 = new();
    [Name("Tessellate"), Description("These objects will be automatically tessellated.")]
    public List<Symbol> objList3 = new();
    [Name("Ignore Transparent"), Description("Ignore transparent objects when casting shadows.")]
    public bool bool1;
    [Name("Ignore Prelit"), Description("Ignore prelit materials when receiving shadows.")]
    public bool bool2;
    [Name("Ignore Hidden"), Description("Ignore hidden objects when casting or receiving shadows.")]
    public bool bool3;
    [Name("Use Mesh Normals"), Description("Use the mesh normals when calculating. Otherwise, smoothed normals will be re-generated for each mesh.")]
    public bool bool4;
    [Name("Intersect Back Faces"), Description("If true, this will cause back faces of triangles to be added. This will more than double the calculation time."), MinVersion(4)]
    public bool bool5;
    [Name("Tessellate Tri Limit"), Description("The maximum number of polys tessellation will generate. This is a multiplier of the current poly count. A value of 2 will generate a maximum of 2x the polys."), MinVersion(2)]
    public int int1;
    [Name("Tessellate Tri Error"), Description("The error threshold for the ambient occlusion calculation. Error larger than this value will result in a triangle being split."), MinVersion(2)]
    public int int2;
    [Name("Tessellate Tri Large"), Description("Triangles larger than this size will always be split."), MinVersion(2)]
    public int int3;
    [Name("Tessellate Tri Small"), Description("Triangles smaller than this size will not be split any further."), MinVersion(2)]
    public int int4;
    [MinVersion(3)]
    public int int5;

    public new RndAmbientOcclusion Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        objList1 = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        objList2 = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        objList3 = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        bool1 = reader.ReadBoolean();
        bool2 = reader.ReadBoolean();
        bool3 = reader.ReadBoolean();
        bool4 = reader.ReadBoolean();

        if (3 < revision)
        {
            bool5 = reader.ReadBoolean();
        }

        if (revision > 1)
        {
            int1 = reader.ReadInt32();
            int2 = reader.ReadInt32();
            int3 = reader.ReadInt32();
            int4 = reader.ReadInt32();
        }

        if (revision > 2)
        {
            int5 = reader.ReadInt32();
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

        AssetHelpers.WriteList(writer, objList1, s => Symbol.Write(writer, s));

        AssetHelpers.WriteList(writer, objList2, s => Symbol.Write(writer, s));

        AssetHelpers.WriteList(writer, objList3, s => Symbol.Write(writer, s));

        writer.WriteBoolean(bool1);
        writer.WriteBoolean(bool2);
        writer.WriteBoolean(bool3);
        writer.WriteBoolean(bool4);

        if (3 < revision)
        {
            writer.WriteBoolean(bool5);
        }

        if (revision > 1)
        {
            writer.WriteInt32(int1);
            writer.WriteInt32(int2);
            writer.WriteInt32(int3);
            writer.WriteInt32(int4);
        }

        if (revision > 2)
        {
            writer.WriteInt32(int5);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
