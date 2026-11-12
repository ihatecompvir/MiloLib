using MiloLib.Assets.Rnd;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

public abstract class Gh1UiComponent : Object
{
    private readonly ushort supportedRevision;
    private ushort revision;
    private ushort altRevision;

    public RndTrans trans = RndTrans.New(8, 0);
    public RndDrawable draw = RndDrawable.New(1, 0);

    protected Gh1UiComponent(ushort revision)
    {
        supportedRevision = revision;
        this.revision = revision;
    }

    public new Gh1UiComponent Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, supportedRevision);
        ValidateRevision();
        trans = new RndTrans().Read(reader, false, parent, entry);
        draw = new RndDrawable().Read(reader, false, parent, entry);
        ReadFields(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateRevision();
        AssetHelpers.WriteRevision(writer, revision, altRevision, supportedRevision);
        trans.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        WriteFields(writer);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private void ValidateRevision()
    {
        if (revision != supportedRevision)
        {
            throw new NotSupportedException($"Unsupported {GetType().Name} revision {revision}; expected {supportedRevision}.");
        }
    }

    protected abstract void ReadFields(EndianReader reader);
    protected abstract void WriteFields(EndianWriter writer);
}
