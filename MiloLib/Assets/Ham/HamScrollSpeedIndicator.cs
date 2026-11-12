using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamScrollSpeedIndicator"), Description("Resource object for scrolly stuff")]
public class HamScrollSpeedIndicator : RndDir
{
    private ushort revision;
    private ushort altRevision;
    [Name("Enter Anim")]
    public Symbol enterAnim = Symbol.Empty;
    [Name("Exit Anim")]
    public Symbol exitAnim = Symbol.Empty;
    [Name("Indicator Anim")]
    public Symbol indicatorAnim = Symbol.Empty;
    [Name("Slow Scroll Threshold Frame"), Description("Frame at which the bar reaches the slow-scroll indicator")]
    public float slowScrollThresholdFrame;
    [Name("Fast Scroll Threshold Frame"), Description("Frame at which the bar reaches the fast-scroll indicator")]
    public float fastScrollThresholdFrame;

    public HamScrollSpeedIndicator(ushort revision = 0, ushort altRevision = 0) : base(0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new HamScrollSpeedIndicator Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        if (revision >= 1)
        {
            enterAnim = Symbol.Read(reader);
            exitAnim = Symbol.Read(reader);
            indicatorAnim = Symbol.Read(reader);
        }
        if (revision >= 2)
        {
            slowScrollThresholdFrame = reader.ReadFloat();
            fastScrollThresholdFrame = reader.ReadFloat();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        base.Write(writer, false, parent, entry);
        if (revision >= 1)
        {
            Symbol.Write(writer, enterAnim);
            Symbol.Write(writer, exitAnim);
            Symbol.Write(writer, indicatorAnim);
        }
        if (revision >= 2)
        {
            writer.WriteFloat(slowScrollThresholdFrame);
            writer.WriteFloat(fastScrollThresholdFrame);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
