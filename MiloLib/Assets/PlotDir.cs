using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("PlotDir")]
public class PlotDir : RndDir
{
    private ushort revision;
    private ushort altRevision;
    [Name("Flat")]
    public bool flat;
    [Name("Section Type")]
    public int sectionType;

    public PlotDir(ushort revision = 0, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    private void ValidateRevision()
    {
        if (revision != 0)
        {
            throw new NotSupportedException($"Unsupported PlotDir revision {revision}.");
        }
    }

    public new PlotDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        ValidateRevision();
        base.Read(reader, false, parent, entry);
        flat = reader.ReadBoolean();
        sectionType = reader.ReadInt32();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateRevision();
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        writer.WriteBoolean(flat);
        writer.WriteInt32(sectionType);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

