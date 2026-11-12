using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("WorldReflection"), Description("Reflects all drawables in draws.")]
public class WorldReflection : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndTrans trans = new();
    public RndDrawable draw = new();

    [Name("Vertical Stretch"), Description("How far to stretch vertically")]
    public float verticalStretch;
    [Name("Draws"), Description("things to draw in the reflection, in this order")]
    public List<Symbol> draws = new();

    [Name("Hide List"), Description("List of objects to hide in the reflection, shows them when reflection has finished drawing."), MinVersion(2)]
    public List<Symbol> hideList = new();
    [Name("Show List"), Description("List of objects to show in the reflection, hides them when reflection has finished drawing."), MinVersion(2)]
    public List<Symbol> showList = new();

    [Name("LOD Chars"), Description("Set LOD to 1 on these reflected characters"), MinVersion(3)]
    public List<Symbol> lodChars = new();

    public new WorldReflection Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        trans = trans.Read(reader, false, parent, entry);
        draw = draw.Read(reader, false, parent, entry);

        verticalStretch = reader.ReadFloat();

        draws = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        if (revision > 1)
        {
            hideList = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            showList = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 2)
        {
            lodChars = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
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
        trans.Write(writer, false, parent, null);
        draw.Write(writer, false, parent, null);

        writer.WriteFloat(verticalStretch);

        AssetHelpers.WriteList(writer, draws, draw => Symbol.Write(writer, draw));

        if (revision > 1)
        {
            AssetHelpers.WriteList(writer, hideList, hide => Symbol.Write(writer, hide));

            AssetHelpers.WriteList(writer, showList, show => Symbol.Write(writer, show));
        }

        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, lodChars, lod => Symbol.Write(writer, lod));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
