using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharWeightSetter"), Description("Sets its own weight by pushing flags through a driver to see what fraction of them it has.")]
public class CharWeightSetter : Object
{
    private ushort altRevision;
    private ushort revision;

    [MinVersion(2)]
    public CharWeightable weightable = new();

    [Name("Driver"), Description("The Driver to monitor")]
    public Symbol driver = Symbol.Empty;
    [Name("Base"), Description("If driver not set, uses this to get base weight"), MinVersion(6)]
    public Symbol baseObj = Symbol.Empty;

    [Name("Max Weights"), Description("Weight can't be less than this weightable"), MinVersion(8)]
    public List<Symbol> maxWeights = new();
    [Name("Min Weights"), Description("Weight can't be greater than this weightable"), MinVersion(7)]
    public List<Symbol> minWeights = new();

    [Name("Flags"), Description("Which clip flags to look for")]
    public int flags;
    [Name("Offset"), Description("Constant offset to apply to the weight."), MinVersion(4)]
    public float offset;
    [Name("Scale"), Description("Constant scale to apply to the weight."), MinVersion(4)]
    public float scale;
    [Name("Base Weight"), Description("The base weight that the real weight is derived from"), MinVersion(5)]
    public float baseWeight;
    [Name("Beats Per Weight"), Description("how many beats it should take to change the weight from 0 to 1"), MinVersion(5)]
    public float beatsPerWeight;

    [MaxVersion(1)]
    public List<Symbol> weightables = new();

    [MinVersion(3), MaxVersion(3)]
    public bool unkBool;


    public new CharWeightSetter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision > 1)
        {
            weightable = weightable.Read(reader, false, parent, entry);
        }

        driver = Symbol.Read(reader);
        flags = reader.ReadInt32();

        if (revision < 3)
        {

        }
        else if (revision < 4)
        {
            unkBool = reader.ReadBoolean();
        }
        else
        {
            offset = reader.ReadFloat();
            scale = reader.ReadFloat();
        }

        if (revision < 2)
        {
            weightables = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision > 4)
        {
            baseWeight = reader.ReadFloat();
            beatsPerWeight = reader.ReadFloat();
        }
        if (revision > 5)
        {
            baseObj = Symbol.Read(reader);
        }
        if (revision > 8)
        {
            minWeights = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            maxWeights = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        else
        {
            if (revision > 6)
            {
                minWeights.Add(Symbol.Read(reader));
            }

            if (revision > 7)
            {
                maxWeights.Add(Symbol.Read(reader));
            }
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

        if (revision > 1)
        {
            weightable.Write(writer, false, parent, entry);
        }

        Symbol.Write(writer, driver);
        writer.WriteInt32(flags);


        if (revision < 3)
        {

        }
        else if (revision < 4)
        {
            writer.WriteBoolean(unkBool);
        }
        else
        {
            writer.WriteFloat(offset);
            writer.WriteFloat(scale);
        }

        if (revision < 2)
        {
            AssetHelpers.WriteList(writer, weightables, oldWeightable => Symbol.Write(writer, oldWeightable));
        }

        if (revision > 4)
        {
            writer.WriteFloat(baseWeight);
            writer.WriteFloat(beatsPerWeight);
        }

        if (revision > 5)
        {
            Symbol.Write(writer, baseObj);
        }

        if (revision > 8)
        {
            AssetHelpers.WriteList(writer, minWeights, minWeight => Symbol.Write(writer, minWeight));
            AssetHelpers.WriteList(writer, maxWeights, maxWeight => Symbol.Write(writer, maxWeight));
        }
        else
        {
            if (revision > 6)
            {
                Symbol.Write(writer, minWeights[0]);
            }

            if (revision > 7)
            {
                Symbol.Write(writer, maxWeights[0]);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
