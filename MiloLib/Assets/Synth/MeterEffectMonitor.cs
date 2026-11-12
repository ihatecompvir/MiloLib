using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("MeterEffectMonitor"), Description("Monitor for FxMeterEffect to feed back channel data")]
public class MeterEffectMonitor : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Meter Effect"), Description("FxSendMeterEffect for this object to monitor")]
    public Symbol meterEffect = Symbol.Empty;

    public new MeterEffectMonitor Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        meterEffect = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, meterEffect);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
