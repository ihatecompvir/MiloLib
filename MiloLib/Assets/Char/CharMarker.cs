using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharMarker"), Description("An character placement marker")]
public class CharMarker : Object
{
    [HideInInspector]
    private ushort revision = 1;
    [HideInInspector]
    private ushort altRevision;

    public RndTrans trans = new();




    public new CharMarker Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        trans.Read(reader, false, parent, entry);


        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        trans.Write(writer, false, parent, entry);


        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
