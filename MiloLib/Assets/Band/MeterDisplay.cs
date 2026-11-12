using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("MeterDisplay"), Description("Meter Display")]
public class MeterDisplay : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Show Text"), Description("whether or not to show text")]
    public bool showText;
    [Name("Current Value"), Description("current value of meter"), MinVersion(1)]
    public int currentValue;
    [Name("Max Value"), Description("max value of meter"), MinVersion(1)]
    public int maxValue;
    [Name("Percentage Text"), Description("whether or not to show text in percentage form"), MinVersion(2)]
    public bool percentageText;
    [Name("Anim Period"), Description("length of value change animation, in seconds"), MinVersion(3)]
    public float animPeriod;
    [Name("Hide Denominator"), Description("whether or not to hide denominator"), MinVersion(4)]
    public bool hideDenominator;
    [Name("Wrapper Text"), Description("Localization token to use for wrapper"), MinVersion(4)]
    public Symbol wrapperText = Symbol.Empty;

    [Name("Resource")]
    public Symbol resource = Symbol.Empty;

    public new MeterDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        showText = reader.ReadBoolean();
        if (revision >= 1)
        {
            currentValue = reader.ReadInt32();
            maxValue = reader.ReadInt32();
        }
        if (revision >= 2)
        {
            percentageText = reader.ReadBoolean();
        }

        if (revision >= 3)
        {
            animPeriod = reader.ReadFloat();
        }

        if (revision >= 4)
        {
            hideDenominator = reader.ReadBoolean();
            wrapperText = Symbol.Read(reader);
        }

        if (parent.revision >= 32)
        {
            resource = Symbol.Read(reader);
        }

        base.Read(reader, false, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        writer.WriteBoolean(showText);
        if (revision >= 1)
        {
            writer.WriteInt32(currentValue);
            writer.WriteInt32(maxValue);
        }
        if (revision >= 2)
        {
            writer.WriteBoolean(percentageText);
        }

        if (revision >= 3)
        {
            writer.WriteFloat(animPeriod);
        }

        if (revision >= 4)
        {
            writer.WriteBoolean(hideDenominator);
            Symbol.Write(writer, wrapperText);
        }

        if (parent.revision >= 32)
        {
            Symbol.Write(writer, resource);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
