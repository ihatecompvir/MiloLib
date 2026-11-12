using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("DialogDisplay"), Description("Dialog Display (controls bones associated with a label)")]
public class DialogDisplay : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Dialog Label"), Description("label used to control dialog bones")]
    public Symbol dialogLabel = Symbol.Empty;
    [Name("Top Bone"), Description("top bone used to adjust dialog size")]
    public Symbol topBone = Symbol.Empty;
    [Name("Bottom Bone"), Description("bottom bone used to adjust dialog size")]
    public Symbol bottomBone = Symbol.Empty;

    public new DialogDisplay Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        dialogLabel = Symbol.Read(reader);
        topBone = Symbol.Read(reader);
        bottomBone = Symbol.Read(reader);

        base.Read(reader, false, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        Symbol.Write(writer, dialogLabel);
        Symbol.Write(writer, topBone);
        Symbol.Write(writer, bottomBone);

        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
