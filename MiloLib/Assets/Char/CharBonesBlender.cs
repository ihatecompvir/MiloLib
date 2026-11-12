using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharBonesBlender"), Description("Blends itself into another CharBones, clearing self each frame")]
public class CharBonesBlender : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Dest"), Description("CharBones to blend into")]
    public Symbol dest = Symbol.Empty;
    [Name("Clip Type"), Description("What type of clip we can accommodate"), MinVersion(2)]
    public Symbol clipType = Symbol.Empty;

    public new CharBonesBlender Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        dest = Symbol.Read(reader);

        if (revision > 1)
        {
            clipType = Symbol.Read(reader);
        }

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

        Symbol.Write(writer, dest);

        if (revision > 1)
        {
            Symbol.Write(writer, clipType);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
