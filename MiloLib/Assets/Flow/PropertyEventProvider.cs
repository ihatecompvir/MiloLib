using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("PropertyEventProvider"), Description("Object which provides property based events for flow.")]
public class PropertyEventProvider : Object
{
    private ushort revision;
    private ushort altRevision;

    public new PropertyEventProvider Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        if (revision < 1 || GetType() == typeof(PropertyEventProvider))
        {
            base.Read(reader, false, parent, entry);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        if (revision < 1 || GetType() == typeof(PropertyEventProvider))
        {
            base.Write(writer, false, parent, entry);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
