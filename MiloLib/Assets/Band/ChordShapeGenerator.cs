using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("ChordShapeGenerator"), Description("a tool for generating arbitrary real guitar chord shape gems from sample geometry")]
public class ChordShapeGenerator : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Chord Source Mesh"), Description("sample mesh from which to create chord meshes")]
    public Symbol chordSrcMesh = Symbol.Empty;

    [Name("Finger Source Mesh"), Description("sample mesh from which to create finger shape meshes"), MinVersion(1)]
    public Symbol fingerSrcMesh = Symbol.Empty;

    [Name("Base Cross Section"), Description("determines where the cross section will be taken to generate the open-string parts of the chord shape")]
    public Symbol baseXSection = Symbol.Empty;

    [Name("Contour Cross Section"), Description("determines where the cross section will be taken to generate the fretted parts of the chord shape")]
    public Symbol contourXSection = Symbol.Empty;

    [Name("Base Height"), Description("indicated where the top of the base is - everything above this height will be scaled according to the fret position.")]
    public Symbol baseHeight = Symbol.Empty;

    [Name("Num Slots"), Description("how many strings the instrument has")]
    public int numSlots;

    [Name("String Frets"), Description("the fret position on each string for this chord")]
    public List<int> stringFrets = new();

    [Name("String 0"), Description("position of the low E smasher")]
    public Symbol string0 = Symbol.Empty;

    [Name("String 1"), Description("position of the A smasher")]
    public Symbol string1 = Symbol.Empty;

    [Name("String 2"), Description("position of the D smasher")]
    public Symbol string2 = Symbol.Empty;

    [Name("String 3"), Description("position of the G smasher")]
    public Symbol string3 = Symbol.Empty;

    [Name("String 4"), Description("position of the B smasher")]
    public Symbol string4 = Symbol.Empty;

    [Name("String 5"), Description("position of the high E smasher")]
    public Symbol string5 = Symbol.Empty;

    [Name("Fret Heights"), Description("determines the height of the chord shape at a string according to the fret")]
    public List<float> fretHeights = new();

    [Name("Grade Distances"), Description("this value controls how steep the slant is between different fret numbers")]
    public List<float> gradeDistances = new();

    public new ChordShapeGenerator Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        chordSrcMesh = Symbol.Read(reader);

        if (revision != 0)
        {
            fingerSrcMesh = Symbol.Read(reader);
        }

        baseXSection = Symbol.Read(reader);
        contourXSection = Symbol.Read(reader);
        baseHeight = Symbol.Read(reader);
        numSlots = reader.ReadInt32();

        stringFrets = AssetHelpers.ReadList(reader, () => reader.ReadInt32());

        string0 = Symbol.Read(reader);
        string1 = Symbol.Read(reader);
        string2 = Symbol.Read(reader);
        string3 = Symbol.Read(reader);
        string4 = Symbol.Read(reader);
        string5 = Symbol.Read(reader);

        fretHeights = AssetHelpers.ReadList(reader, () => reader.ReadFloat());

        gradeDistances = AssetHelpers.ReadList(reader, () => reader.ReadFloat());

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        Symbol.Write(writer, chordSrcMesh);

        if (revision != 0)
        {
            Symbol.Write(writer, fingerSrcMesh);
        }

        Symbol.Write(writer, baseXSection);
        Symbol.Write(writer, contourXSection);
        Symbol.Write(writer, baseHeight);
        writer.WriteInt32(numSlots);

        AssetHelpers.WriteList(writer, stringFrets, fret => writer.WriteInt32(fret));

        Symbol.Write(writer, string0);
        Symbol.Write(writer, string1);
        Symbol.Write(writer, string2);
        Symbol.Write(writer, string3);
        Symbol.Write(writer, string4);
        Symbol.Write(writer, string5);

        AssetHelpers.WriteList(writer, fretHeights, height => writer.WriteFloat(height));

        AssetHelpers.WriteList(writer, gradeDistances, distance => writer.WriteFloat(distance));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
