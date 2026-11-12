using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharTrigger")]
public class CharTrigger : EventTrigger
{
    [HideInInspector]
    private ushort revision = 1;
    [HideInInspector]
    private ushort altRevision;


    [Name("Target")]
    public Symbol target = Symbol.Empty;
    [Name("Clip Set")]
    public Symbol clipSet = Symbol.Empty;
    [Name("Clip")]
    public Symbol clip = Symbol.Empty;

    public new CharTrigger Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);

        target = Symbol.Read(reader);
        clipSet = Symbol.Read(reader);
        clip = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);

        Symbol.Write(writer, target);
        Symbol.Write(writer, clipSet);
        Symbol.Write(writer, clip);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
