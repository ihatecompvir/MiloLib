using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("LabelNumberTicker"), Description("a ticker to control counting up or down for a given number based label")]
public class LabelNumberTicker : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Label"), Description("label to be shrink wrapped")]
    public Symbol label = Symbol.Empty;
    [Name("Desired Value"), Description("desired value")]
    public float desiredValue;
    [Name("Anim Time"), Description("Time taken for number ticking animation")]
    public float animTime;
    [Name("Anim Delay"), Description("delay before number ticking animation")]
    public float animDelay;
    [Name("Wrapper Text"), Description("Localization token to use for wrapper (must include %s)")]
    public Symbol wrapperText = Symbol.Empty;
    [Name("Acceleration"), Description("Higher number accelerates faster, 3 is a good number."), MinVersion(1)]
    public float acceleration;
    [Name("Tick Trigger"), Description("Event to trigger while counting up"), MinVersion(2)]
    public Symbol tickTrigger = Symbol.Empty;
    [Name("Tick Every"), Description("Trigger tick_trigger every time the count goes up by this much"), MinVersion(2)]
    public int tickEvery;

    public new LabelNumberTicker Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        label = Symbol.Read(reader);
        desiredValue = reader.ReadFloat();
        animTime = reader.ReadFloat();
        animDelay = reader.ReadFloat();
        wrapperText = Symbol.Read(reader);
        if (revision >= 1)
        {
            acceleration = reader.ReadFloat();
        }

        if (revision >= 2)
        {
            tickTrigger = Symbol.Read(reader);
            tickEvery = reader.ReadInt32();
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

        Symbol.Write(writer, label);
        writer.WriteFloat(desiredValue);
        writer.WriteFloat(animTime);
        writer.WriteFloat(animDelay);
        Symbol.Write(writer, wrapperText);
        if (revision >= 1)
        {
            writer.WriteFloat(acceleration);
        }

        if (revision >= 2)
        {
            Symbol.Write(writer, tickTrigger);
            writer.WriteInt32(tickEvery);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
