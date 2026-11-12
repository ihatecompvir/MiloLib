using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Line"), Description("Line objects represent 3D lines with thickness, perspective, and optional end caps.")]
public class RndLine : Object
{
    public class Point
    {
        [Name("Position"), Description("Position of point")]
        public Vector3 position = new();

        [Name("Color"), Description("Color of point")]
        public HmxColor4 color = new();
    }

    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();
    public RndTrans trans = new();

    [MaxVersion(2)]
    private int oldObjectListValue;

    [MaxVersion(2)]
    public List<Symbol> oldObjects = new();

    [Name("Material"), Description("Material the line uses.")]
    public Symbol material = Symbol.Empty;

    [Name("Points")]
    public List<Point> points = new();

    [Name("Width"), Description("Width of the line")]
    public float width = 1.0f;

    [Name("Fold Angle"), Description("Degrees at which the line starts to fold"), MinVersion(1)]
    public float foldAngle;

    [Name("Has Caps"), Description("The line has end caps"), MinVersion(1)]
    public bool hasCaps;

    [Name("Line Pairs"), Description("Consider the [points] as an array of pairs, rather than a continuous line"), MinVersion(2)]
    public bool linePairs;

    public new RndLine Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        if (revision > 3)
        {
            base.Read(reader, false, parent, entry);
        }
        draw = new RndDrawable().Read(reader, false, parent, entry);
        if (revision < 3)
        {
            oldObjectListValue = reader.ReadInt32();
            oldObjects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        trans = new RndTrans().Read(reader, false, parent, entry);
        material = Symbol.Read(reader);
        points = AssetHelpers.ReadList(reader, () =>
        {
            Point point = new();
            point.position = new Vector3().Read(reader);
            point.color = new HmxColor4().Read(reader);
            return point;
        });
        width = reader.ReadFloat();
        if (revision > 0)
        {
            foldAngle = reader.ReadFloat();
            hasCaps = reader.ReadBoolean();
        }
        if (revision > 1)
        {
            linePairs = reader.ReadBoolean();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        if (revision > 3)
        {
            base.Write(writer, false, parent, entry);
        }
        draw.Write(writer, false, parent, entry);
        if (revision < 3)
        {
            writer.WriteInt32(oldObjectListValue);
            AssetHelpers.WriteList(writer, oldObjects, (Symbol legacyObject) => Symbol.Write(writer, legacyObject));
        }
        trans.Write(writer, false, parent, entry);
        Symbol.Write(writer, material);
        AssetHelpers.WriteList(writer, points, point =>
        {
            point.position.Write(writer);
            point.color.Write(writer);
        });
        writer.WriteFloat(width);
        if (revision > 0)
        {
            writer.WriteFloat(foldAngle);
            writer.WriteBoolean(hasCaps);
        }
        if (revision > 1)
        {
            writer.WriteBoolean(linePairs);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
