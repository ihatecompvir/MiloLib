using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharIKMidi"), Description("Moves an RndTransformable (bone) to another RndTransformable (spot) over time, blending from where it was relative to the parent of the spot.")]
public class CharIKMidi : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Bone"), Description("The bone to move")]
    public Symbol bone = Symbol.Empty;

    [MaxVersion(2)]
    public List<Symbol> unkSymbols = new();

    [Name("Current Spot"), Description("Spot to go to, zero indexed"), MinVersion(2), MaxVersion(3)]
    public Symbol currentSpot = Symbol.Empty;

    [Name("Anim Blend Weightable"), Description("Weightable to change animation between frets"), MinVersion(5)]
    public Symbol animBlender = Symbol.Empty;
    [Name("Anim Blend Max"), Description("Max weight for animation change"), MinVersion(5)]
    public float maxAnimBlend;

    public new CharIKMidi Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        bone = Symbol.Read(reader);

        if (revision < 3)
        {
            unkSymbols = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision == 2 || revision == 3)
        {
            currentSpot = Symbol.Read(reader);
        }
        if (revision > 4)
        {
            animBlender = Symbol.Read(reader);
            maxAnimBlend = reader.ReadFloat();
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

        Symbol.Write(writer, bone);

        if (revision < 3)
        {
            AssetHelpers.WriteList(writer, unkSymbols, (Symbol symbol) => Symbol.Write(writer, symbol));
        }

        if (revision == 2 || revision == 3)
        {
            Symbol.Write(writer, currentSpot);
        }

        if (revision > 4)
        {
            Symbol.Write(writer, animBlender);
            writer.WriteFloat(maxAnimBlend);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
