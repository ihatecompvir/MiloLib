using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("FxSendDistortion"), Description("distortion effect")]
public class FxSendDistortion : FxSend
{
    private ushort altRevision;
    private ushort revision;

    [Name("Drive"), Description("amount of drive")]
    public float drive;

    public new FxSendDistortion Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        drive = reader.ReadFloat();

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

        writer.WriteFloat(drive);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
