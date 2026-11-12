using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band.UI;

[Name("BandList"), Description("Band specific UIList")]
public class BandList : UIList
{
    public class HighlightObjects
    {
        [Name("Target Object"), Description("The object to attach to the highlight")]
        public Symbol targetObject = Symbol.Empty;
        [Name("X Offset"), Description("x offset from list position")]
        public float xOffset;
        [Name("Y Offset"), Description("y offset from list position")]
        public float yOffset;
        [Name("Z Offset"), Description("z offset from list position")]
        public float zOffset;

        public HighlightObjects Read(EndianReader reader)
        {
            targetObject = Symbol.Read(reader);
            xOffset = reader.ReadFloat();
            yOffset = reader.ReadFloat();
            zOffset = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, targetObject);
            writer.WriteFloat(xOffset);
            writer.WriteFloat(yOffset);
            writer.WriteFloat(zOffset);
        }
    }
    private ushort altRevision;
    private ushort revision;

    [Name("Focus Anim"), Description("Animation to play on a selected entry to transition into and out of focus"), MinVersion(18)]
    public Symbol focusAnim = Symbol.Empty;
    [Name("Pulse Anim"), Description("Animation to play on a selected entry after focus is played - focus anim must exist"), MinVersion(18)]
    public Symbol pulseAnim = Symbol.Empty;

    [Name("Reveal Anim"), Description("animation to play on each entry when list is revealed"), MinVersion(19)]
    public Symbol revealAnim = Symbol.Empty;
    [Name("Conceal Anim"), Description("animation to play on each entry when list is concealed"), MinVersion(19)]
    public Symbol concealAnim = Symbol.Empty;
    [Name("Reveal Sound"), Description("sound to play on each entry when list is revealed"), MinVersion(21)]
    public Symbol revealSound = Symbol.Empty;
    [Name("Conceal Sound"), Description("sound to play on each entry when list is concealed"), MinVersion(21)]
    public Symbol concealSound = Symbol.Empty;

    [Name("Reveal Sound Delay"), Description("delay for sound to play on each entry when list is revealed"), MinVersion(21)]
    public float revealSoundDelay;
    [Name("Conceal Sound Delay"), Description("delay for sound to play on each entry when list is concealed"), MinVersion(21)]
    public float concealSoundDelay;
    [Name("Reveal Start Delay"), Description("delay before playing reveal animation"), MinVersion(19)]
    public float revealStartDelay;
    [Name("Reveal Entry Delay"), Description("delay between list entries playing reveal animation"), MinVersion(19)]
    public float revealEntryDelay;
    [Name("Reveal Scale"), Description("amount to scale reveal animation"), MinVersion(20)]
    public float revealScale;
    [Name("Conceal Start Delay"), Description("delay before playing conceal animation"), MinVersion(19)]
    public float concealStartDelay;
    [Name("Conceal Entry Delay"), Description("delay between list entries playing conceal animation"), MinVersion(19)]
    public float concealEntryDelay;
    [Name("Conceal Scale"), Description("amount to scale conceal animation"), MinVersion(20)]
    public float concealScale;
    [Name("Auto Reveal"), Description("Whether or not to start revealed"), MinVersion(20)]
    public bool autoReveal;

    [Name("Highlight Objects"), MinVersion(22)]
    public List<HighlightObjects> highlightObjects = new();

    public new BandList Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision <= 0x11)
        {
            ReadWithExternalRev(reader, revision, false, parent, entry);
        }
        else
        {
            base.Read(reader, false, parent, entry);
        }

        if (revision >= 0x12)
        {
            focusAnim = Symbol.Read(reader);
            pulseAnim = Symbol.Read(reader);
        }

        if (revision >= 0x13)
        {
            revealAnim = Symbol.Read(reader);
            revealStartDelay = reader.ReadFloat();
            revealEntryDelay = reader.ReadFloat();
            concealAnim = Symbol.Read(reader);
            concealStartDelay = reader.ReadFloat();
            concealEntryDelay = reader.ReadFloat();
        }
        if (revision >= 0x14)
        {
            revealScale = reader.ReadFloat();
            concealScale = reader.ReadFloat();
            autoReveal = reader.ReadBoolean();
        }

        if (revision >= 0x15)
        {
            revealSound = Symbol.Read(reader);
            concealSound = Symbol.Read(reader);
            revealSoundDelay = reader.ReadFloat();
            concealSoundDelay = reader.ReadFloat();
        }
        if (revision >= 0x16)
        {
            highlightObjects = AssetHelpers.ReadList(reader, () => new HighlightObjects().Read(reader));
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

        if (revision <= 0x11)
        {
            WriteWithoutRev(writer, false, parent, entry);
        }
        else
        {
            base.Write(writer, false, parent, entry);
        }

        if (revision >= 0x12)
        {
            Symbol.Write(writer, focusAnim);
            Symbol.Write(writer, pulseAnim);
        }

        if (revision >= 0x13)
        {
            Symbol.Write(writer, revealAnim);
            writer.WriteFloat(revealStartDelay);
            writer.WriteFloat(revealEntryDelay);
            Symbol.Write(writer, concealAnim);
            writer.WriteFloat(concealStartDelay);
            writer.WriteFloat(concealEntryDelay);
        }

        if (revision >= 0x14)
        {
            writer.WriteFloat(revealScale);
            writer.WriteFloat(concealScale);
            writer.WriteBoolean(autoReveal);
        }

        if (revision >= 0x15)
        {
            Symbol.Write(writer, revealSound);
            Symbol.Write(writer, concealSound);
            writer.WriteFloat(revealSoundDelay);
            writer.WriteFloat(concealSoundDelay);
        }

        if (revision >= 0x16)
        {
            AssetHelpers.WriteList(writer, highlightObjects, highlightObject => highlightObject.Write(writer));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
