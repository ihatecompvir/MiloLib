using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("RankProgressBar"), Description("Rank Progress Bar")]
public class RankProgressBar : UIComponent
{
    private ushort revision;
    private ushort altRevision;
    public float timeLength;
    public float startPercent;
    public float endPercent;
    public Symbol fillSound = Symbol.Empty;
    public float startSoundTranspose;
    public float endSoundTranspose;

    public new RankProgressBar Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        timeLength = reader.ReadFloat();
        if (revision >= 2)
        {
            startPercent = reader.ReadFloat();
            endPercent = reader.ReadFloat();
            fillSound = Symbol.Read(reader);
            startSoundTranspose = reader.ReadFloat();
            endSoundTranspose = reader.ReadFloat();
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
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        writer.WriteFloat(timeLength);
        if (revision >= 2)
        {
            writer.WriteFloat(startPercent);
            writer.WriteFloat(endPercent);
            Symbol.Write(writer, fillSound);
            writer.WriteFloat(startSoundTranspose);
            writer.WriteFloat(endSoundTranspose);
        }
        base.Write(writer, false, parent, entry);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
