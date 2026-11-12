using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendSynapse"), Description("synapse effect")]
public class FxSendSynapse : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Amount"), Description("amount of correction")]
    public float amount;
    [Name("Proximity Effect"), Description("amount of correction dependence on error")]
    public float proximityEffect;
    [Name("Note1 Hz"), Description("target note 1 frequency")]
    public float note1Hz;
    [Name("Attack Smoothing"), Description("attack time ms for correction"), MinVersion(2)]
    public float attackSmoothing;
    [Name("Proximity Focus"), Description("focus for amount of correction dependence on error"), MinVersion(3)]
    public float proximityFocus;
    [Name("Release Smoothing"), Description("release time ms for correction"), MinVersion(4)]
    public float releaseSmoothing;
    [Name("Note2 Hz"), Description("target note 2 frequency"), MinVersion(5)]
    public float note2Hz;
    [Name("Note3 Hz"), Description("target note 3 frequency"), MinVersion(5)]
    public float note3Hz;
    [Name("Unison Trio"), Description("whether the three voices would sing in unison"), MinVersion(5)]
    public bool unisonTrio;

    public new FxSendSynapse Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        amount = reader.ReadFloat();
        proximityEffect = reader.ReadFloat();
        note1Hz = reader.ReadFloat();

        if (revision > 1)
        {
            attackSmoothing = reader.ReadFloat();
        }

        if (revision > 2)
        {
            proximityFocus = reader.ReadFloat();
        }

        if (revision > 3)
        {
            releaseSmoothing = reader.ReadFloat();
        }

        if (revision > 4)
        {
            note2Hz = reader.ReadFloat();
            note3Hz = reader.ReadFloat();
            unisonTrio = reader.ReadBoolean();
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

        writer.WriteFloat(amount);
        writer.WriteFloat(proximityEffect);
        writer.WriteFloat(note1Hz);

        if (revision > 1)
        {
            writer.WriteFloat(attackSmoothing);
        }

        if (revision > 2)
        {
            writer.WriteFloat(proximityFocus);
        }

        if (revision > 3)
        {
            writer.WriteFloat(releaseSmoothing);
        }

        if (revision > 4)
        {
            writer.WriteFloat(note2Hz);
            writer.WriteFloat(note3Hz);
            writer.WriteBoolean(unisonTrio);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
