using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendPitchShift"), Description("real-time pitch shift effect")]
public class FxSendPitchShift : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Pitch Ratio"), Description("Pitch ratio (1 = normal, 2 = up one octave, etc.)")]
    public float ratio;

    public new FxSendPitchShift Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        ratio = reader.ReadFloat();

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

        writer.WriteFloat(ratio);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
