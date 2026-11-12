using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharIKScale"), Description("Rescales a local position of a bone")]
public class CharIKScale : Object
{
    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();

    [Name("Dest"), Description("The bone to be scaled")]
    public Symbol dest = Symbol.Empty;

    [Name("Scale"), Description("Scale to apply")]
    public float scale;

    [Name("Secondary Targets"), Description("Apply remainder weight to these targets"), MinVersion(2)]
    public List<Symbol> secondaryTargets = new();

    [Name("Bottom Height"), Description("If dest starts out at or below this height, weight will be 0"), MinVersion(3)]
    public float bottomHeight;
    [Name("Top Height"), Description("If dest starts out at or above this height, weight will be 1"), MinVersion(3)]
    public float topHeight;
    [Name("Auto Weight"), Description("Automatically determine weight from top & bottom heights"), MinVersion(3)]
    public bool autoWeight;

    public new CharIKScale Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        weightable = weightable.Read(reader, false, parent, entry);

        dest = Symbol.Read(reader);

        scale = reader.ReadFloat();

        if (revision > 1)
        {
            secondaryTargets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 2)
        {
            autoWeight = reader.ReadBoolean();
            bottomHeight = reader.ReadFloat();
            topHeight = reader.ReadFloat();
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

        Symbol.Write(writer, dest);
        writer.WriteFloat(scale);

        if (revision > 1)
        {
            AssetHelpers.WriteList(writer, secondaryTargets, target => Symbol.Write(writer, target));
        }

        if (revision > 2)
        {
            writer.WriteBoolean(autoWeight);
            writer.WriteFloat(bottomHeight);
            writer.WriteFloat(topHeight);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
