using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandRetargetVignette"), Description("Handles Retargetting IK in a vignette")]
public class BandRetargetVignette : Object
{
    private ushort altRevision;
    private ushort revision;

    public Object secondObj = new();
    [MinVersion(1), MaxVersion(2)]
    public List<Symbol> oldStrings = new();
    [Name("Player"), Description("Player to create effector for"), MinVersion(2)]
    public Symbol player = Symbol.Empty;
    [Name("Bone"), Description("bone on player to create/edit effector for"), MinVersion(2)]
    public Symbol bone = Symbol.Empty;
    [Name("Prop"), Description("prop object/bone in scene to constrain"), MinVersion(4)]
    public Symbol prop = Symbol.Empty;

    public new BandRetargetVignette Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        secondObj = new Object().Read(reader, false, parent, entry);

        if (revision != 0 && revision < 3)
        {
            oldStrings = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 1)
        {
            player = Symbol.Read(reader);
            bone = Symbol.Read(reader);
            if (revision > 3)
            {
                prop = Symbol.Read(reader);
            }
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

        secondObj.Write(writer, false, parent, entry);

        if (revision != 0 && revision < 3)
        {
            AssetHelpers.WriteList(writer, oldStrings, s => Symbol.Write(writer, s));
        }

        if (revision > 1)
        {
            Symbol.Write(writer, player);
            Symbol.Write(writer, bone);
            if (revision > 3)
            {
                Symbol.Write(writer, prop);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
