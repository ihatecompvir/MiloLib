using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandPowerMeterDir"), Description("Overdrive power meter")]
public class BandPowerMeterDir : RndDir
{
    private ushort revision;
    private ushort altRevision;

    [Name("Test Energy")]
    public float testEnergy;
    public float unkFloat;
    [Name("State")]
    public Symbol state = Symbol.Empty;
    [Name("Vocal Meter")]
    public bool vocalMeter;

    public BandPowerMeterDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandPowerMeterDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        if (revision > 0)
        {
            throw new NotSupportedException($"Unsupported BandPowerMeterDir revision {revision}.");
        }
        testEnergy = reader.ReadFloat();
        unkFloat = reader.ReadFloat();
        state = Symbol.Read(reader);
        vocalMeter = reader.ReadBoolean();
        base.Read(reader, false, parent, entry);
        if (standalone)
        {
            ReadAndVerifyEndBytes(reader, parent, entry);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        writer.WriteFloat(testEnergy);
        writer.WriteFloat(unkFloat);
        Symbol.Write(writer, state);
        writer.WriteBoolean(vocalMeter);
        base.Write(writer, false, parent, entry);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
