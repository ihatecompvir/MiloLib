using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("MicInputArrow"), Description("Mic Input Arrow")]
public class MicInputArrow : UIComponent
{
    private ushort altRevision;
    private ushort revision;

    [Name("Arrow Num"), Description("Arrow Number")]
    public int arrowNum;

    public new MicInputArrow Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        arrowNum = reader.ReadInt32();

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

        writer.WriteInt32(arrowNum);

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
