using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("SoftParticles"), Description("Renders the specified particles with soft edges.")]
public class RndSoftParticles : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();
    [Name("Particles"), Description("The list of particle objects to draw")]
    public List<Symbol> objList = new();
    [Name("Blend Mode"), Description("Blending to use for the particle system")]
    public int filler;

    public new RndSoftParticles Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        draw = draw.Read(reader, false, parent, entry);

        objList = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        filler = reader.ReadInt32();

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

        draw.Write(writer, false, parent, null);

        AssetHelpers.WriteList(writer, objList, s => Symbol.Write(writer, s));

        writer.WriteInt32(filler);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
