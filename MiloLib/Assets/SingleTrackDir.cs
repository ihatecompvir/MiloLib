using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("SingleTrackDir")]
public class SingleTrackDir : RndDir
{
    private ushort revision;
    private ushort altRevision;
    [Name("Bonus Gem Tail Material")]
    public Symbol bonusGemTailMat = Symbol.Empty;

    public SingleTrackDir(ushort revision = 2, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    private void ValidateRevision()
    {
        if (revision != 2)
        {
            throw new NotSupportedException($"Unsupported SingleTrackDir revision {revision}.");
        }
    }

    public new SingleTrackDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        ValidateRevision();
        base.Read(reader, false, parent, entry);
        bonusGemTailMat = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateRevision();
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, bonusGemTailMat);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

