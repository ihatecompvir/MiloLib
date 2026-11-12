using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CollideableMidiOverride"), Description("An object that overrides a CharMidiDriver when used in conjuction with a CollideablePercussion.")]
public class CollideableMidiOverride : Object
{
    public class VelocityEntry
    {
        [Name("Char Clip"), Description("CharClip that should be mapped to this velocity.")]
        public Symbol charClip = Symbol.Empty;
        [Name("Velocity Threshold"), Description("Threshold at which velocity will activate the corresponding clip.")]
        public float velocityThreshold;

        public VelocityEntry Read(EndianReader reader)
        {
            charClip = Symbol.Read(reader);
            velocityThreshold = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, charClip);
            writer.WriteFloat(velocityThreshold);
        }
    }

    private ushort revision = 1;
    private ushort altRevision;
    [Name("Driver"), Description("CharDriverMidi object that is overridden.")]
    public Symbol driver = Symbol.Empty;
    [Name("Velocity Thresholds"), Description("Bucket of velocity thresholds, maps a velocity to a CharClip animation to play.")]
    public List<VelocityEntry> velocityThresholds = new();

    public new CollideableMidiOverride Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        // seems that collideable shapes don't serialize the Object
        driver = Symbol.Read(reader);
        velocityThresholds = AssetHelpers.ReadList(reader, () => new VelocityEntry().Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        Symbol.Write(writer, driver);
        AssetHelpers.WriteList(writer, velocityThresholds, threshold => threshold.Write(writer));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
