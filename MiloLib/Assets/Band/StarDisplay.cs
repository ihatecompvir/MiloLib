using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;
using System.Text;

namespace MiloLib.Assets.Band;

[Name("StarDisplay"), Description("Star Display (used for showing stars in different formats)")]
public class StarDisplay : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    private bool UsesGreenDayLayout(DirectoryMeta parent)
    {
        // FUCK green day
        return parent.revision == 25 && revision == 1;
    }

    [Name("Other Difficulty Stars"), Description("whether or not to display stars from lesser difficulties as well")]
    public bool otherDifficultyStars;

    [Name("Stars"), Description("Current Stars")]
    public int stars;

    [Name("Total Stars"), Description("Total Stars")]
    public int totalStars;
    [Name("Force Mixed Mode"), Description("whether or not to force mixed mode (X/Y *)")]
    public bool forceMixedMode;
    [Name("Show Denominator"), Description("whether or not to denominator (when applicable)"), MinVersion(3)]
    public bool showDenominator;
    [Name("Show Empty Stars"), Description("whether or not to show empty stars"), MinVersion(3)]
    public bool showEmptyStars;
    [MinVersion(3), MaxVersion(4)]
    public int unkInt;
    [Name("Alignment"), Description("star alignment"), MinVersion(4)]
    public int alignment;
    [MaxVersion(5), MinVersion(5)]
    private string oldString1 = string.Empty;
    [MaxVersion(5), MinVersion(5)]
    private string oldString2 = string.Empty;
    [Name("Icon Override"), Description("Single-character icon symbol in place of star"), MinVersion(6)]
    public Symbol iconOverride = Symbol.Empty;
    [Name("Empty Icon Override"), Description("Single-character icon symbol in place of empty star"), MinVersion(6)]
    public Symbol emptyIconOverride = Symbol.Empty;

    public new StarDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 1 || UsesGreenDayLayout(parent))
        {
            stars = reader.ReadInt32();
        }

        totalStars = reader.ReadInt32();
        forceMixedMode = reader.ReadBoolean();
        if (UsesGreenDayLayout(parent))
        {
            otherDifficultyStars = reader.ReadBoolean();
        }

        if (revision >= 3)
        {
            showDenominator = reader.ReadBoolean();
            showEmptyStars = reader.ReadBoolean();
            if (revision < 5)
            {
                unkInt = reader.ReadInt32();
            }
        }
        if (revision >= 4)
        {
            alignment = reader.ReadInt32();
        }

        if (revision == 5)
        {
            oldString1 = ReadHmxString(reader);
            oldString2 = ReadHmxString(reader);
        }
        if (revision >= 6)
        {
            iconOverride = Symbol.Read(reader);
            emptyIconOverride = Symbol.Read(reader);
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

        if (revision != 1 || UsesGreenDayLayout(parent))
        {
            writer.WriteInt32(stars);
        }

        writer.WriteInt32(totalStars);
        writer.WriteBoolean(forceMixedMode);
        if (UsesGreenDayLayout(parent))
        {
            writer.WriteBoolean(otherDifficultyStars);
        }

        if (revision >= 3)
        {
            writer.WriteBoolean(showDenominator);
            writer.WriteBoolean(showEmptyStars);
            if (revision < 5)
            {
                writer.WriteInt32(unkInt);
            }
        }
        if (revision >= 4)
        {
            writer.WriteInt32(alignment);
        }

        if (revision == 5)
        {
            WriteHmxString(writer, oldString1);
            WriteHmxString(writer, oldString2);
        }
        if (revision >= 6)
        {
            Symbol.Write(writer, iconOverride);
            Symbol.Write(writer, emptyIconOverride);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private static string ReadHmxString(EndianReader reader)
    {
        int length = reader.ReadInt32();
        if (length < 0)
        {
            throw new InvalidDataException($"Invalid string length: {length}");
        }

        if (length == 0)
        {
            return string.Empty;
        }

        return Encoding.Latin1.GetString(reader.ReadBlock(length));
    }

    private static void WriteHmxString(EndianWriter writer, string value)
    {
        value ??= string.Empty;
        byte[] bytes = Encoding.Latin1.GetBytes(value);
        writer.WriteInt32(bytes.Length);
        writer.WriteBlock(bytes);
    }
}
