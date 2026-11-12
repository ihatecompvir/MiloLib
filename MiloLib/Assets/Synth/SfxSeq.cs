using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("SfxSeq"), Description("Wrapper class for turning Sfx into Sequences. Will hopefully disappear soon.")]
public class SfxSeq : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Sequence"), MinVersion(2)]
    public Sfx.Sequence seq = new();

    [Name("SFX"), Description("Which sfx to play")]
    public Symbol sfx = Symbol.Empty;

    public new SfxSeq Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision >= 2)
        {
            seq.Read(reader, parent, entry);
        }

        sfx = Symbol.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision >= 2)
        {
            seq.Write(writer, false, parent, entry);
        }

        Symbol.Write(writer, sfx);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
