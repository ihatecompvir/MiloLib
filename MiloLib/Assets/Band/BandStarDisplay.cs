using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandStarDisplay"), Description("HUD element which displays up to 5 stars")]
public class BandStarDisplay : RndDir
{
    private ushort altRevision;
    private ushort revision;

    [Name("Star Type"), Description("The type of star to display"), MinVersion(1)]
    public Symbol starType = Symbol.Empty;
    [MinVersion(1), MaxVersion(1)]
    public Symbol oldStarType = Symbol.Empty;

    public BandStarDisplay(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandStarDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision == 1)
        {
            if (entry.isProxy)
            {
                starType = Symbol.Read(reader);
            }
            else
            {
                oldStarType = Symbol.Read(reader);
            }
        }
        else if (revision >= 2 && entry.isProxy)
        {
            starType = Symbol.Read(reader);
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
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision == 1)
        {
            if (entry != null && entry.isProxy)
            {
                Symbol.Write(writer, starType);
            }
            else
            {
                Symbol.Write(writer, oldStarType);
            }
        }
        else if (revision >= 2 && entry != null && entry.isProxy)
        {
            Symbol.Write(writer, starType);
        }

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }

}
