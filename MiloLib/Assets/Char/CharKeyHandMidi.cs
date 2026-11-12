using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharKeyHandMidi"), Description("Keyboard hookups to a midi parser, calculates finger destinations.")]
public class CharKeyHandMidi : Object
{
    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();

    [Name("IK Object"), Description("CharIKFingers object")]
    public Symbol ikObject = Symbol.Empty;

    [Name("First Spot"), Description("Spot at 0%")]
    public Symbol firstSpot = Symbol.Empty;

    [Name("Second Spot"), Description("Spot at 100%")]
    public Symbol secondSpot = Symbol.Empty;

    [Name("Is Right Hand"), Description("Does this run the right or left hand?"), MinVersion(2)]
    public bool isRightHand;

    public new CharKeyHandMidi Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        weightable = weightable.Read(reader, false, parent, entry);
        ikObject = Symbol.Read(reader);
        firstSpot = Symbol.Read(reader);
        secondSpot = Symbol.Read(reader);

        if (revision > 1)
        {
            isRightHand = reader.ReadBoolean();
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

        weightable.Write(writer, false, parent, entry);
        Symbol.Write(writer, ikObject);
        Symbol.Write(writer, firstSpot);
        Symbol.Write(writer, secondSpot);

        if (revision > 1)
        {
            writer.WriteBoolean(isRightHand);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
