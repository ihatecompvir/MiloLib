using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharIKSliderMidi"), Description("Moves an RndTransformable (bone) to a percentage of the way between two spots.")]
public class CharIKSliderMidi : Object
{
    private ushort altRevision;
    private ushort revision;

    [MinVersion(2)]
    public CharWeightable weightable = new();

    [Name("Target"), Description("The bone to move")]
    public Symbol target = Symbol.Empty;
    [Name("First Spot"), Description("Spot at 0%")]
    public Symbol firstSpot = Symbol.Empty;
    [Name("Second Spot"), Description("Spot at 100%")]
    public Symbol secondSpot = Symbol.Empty;
    [Name("Tolerance"), Description("Only move if percentage changes more than this (0.0 - 1.0)")]
    public float tolerance;

    public new CharIKSliderMidi Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision >= 2)
        {
            weightable = weightable.Read(reader, false, parent, entry);
        }

        target = Symbol.Read(reader);
        firstSpot = Symbol.Read(reader);
        secondSpot = Symbol.Read(reader);
        tolerance = reader.ReadFloat();

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

        if (revision >= 2)
        {
            weightable.Write(writer, false, parent, entry);
        }

        Symbol.Write(writer, target);
        Symbol.Write(writer, firstSpot);
        Symbol.Write(writer, secondSpot);
        writer.WriteFloat(tolerance);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
