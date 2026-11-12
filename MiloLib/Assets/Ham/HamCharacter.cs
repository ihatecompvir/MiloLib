using MiloLib.Assets.Char;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamCharacter"), Description("Hammer main character class, can be configured to look like characters in /dancer")]
public class HamCharacter : Character
{
    private ushort revision;
    private ushort altRevision;
    [Name("Outfit"), Description("which character to look like")]
    public Symbol outfit = Symbol.Empty;
    [Name("Outfit Dir"), Description("where to load outfits from")]
    public Symbol outfitDir = Symbol.Empty;
    [Name("Show Box"), Description("Draws a 6 foot square box around the character teleport point")]
    public bool showBox;
    [Name("Poll When Hidden"), Description("Updates the character's animation even though showing is set to FALSE. Useful for rendering the character to a texture.")]
    public bool pollWhenHidden;
    [Name("Tex Blenders Active"), Description("True if the internal TexBlenders are working.")]
    public bool texBlendersActive;

    public HamCharacter(ushort revision = 0, ushort altRevision = 0) : base(0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new HamCharacter Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        outfit = Symbol.Read(reader);
        if (revision > 0)
        {
            outfitDir = Symbol.Read(reader);
        }
        if (revision > 1)
        {
            showBox = reader.ReadBoolean();
        }
        if (revision > 2)
        {
            pollWhenHidden = reader.ReadBoolean();
            texBlendersActive = reader.ReadBoolean();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, outfit);
        if (revision > 0)
        {
            Symbol.Write(writer, outfitDir);
        }
        if (revision > 1)
        {
            writer.WriteBoolean(showBox);
        }
        if (revision > 2)
        {
            writer.WriteBoolean(pollWhenHidden);
            writer.WriteBoolean(texBlendersActive);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
