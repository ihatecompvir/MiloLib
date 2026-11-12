using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendBitCrush"), Description("bit crush effect")]
public class FxSendBitCrush : FxSend
{
    private ushort revision = 1;
    private ushort altRevision;

    [Name("Amount"), Description("Amount of crushing [0,100]. This is really the Number of samples held.")]
    public float amount;

    public new FxSendBitCrush Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        amount = reader.ReadFloat();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        writer.WriteFloat(amount);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
