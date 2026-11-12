using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("TexBlender"), Description("Renderable texture used to composite pieces of texture maps based on the distance between bones or other animatiable objects")]
public class RndTexBlender : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();

    [Name("Base Map"), Description("The base texture map")]
    public Symbol baseMap = Symbol.Empty;
    [Name("Near Map"), Description("The texture map to use when the constraints are closer than the default distance")]
    public Symbol nearMap = Symbol.Empty;
    [Name("Far Map"), Description("The texture map to use when the constraints are further than the default distance")]
    public Symbol farMap = Symbol.Empty;
    [Name("Output Texture"), Description("The final result output texture")]
    public Symbol outputTexture = Symbol.Empty;
    [Name("Controllers"), Description("The list of controller objects used to render pieces of a mesh to the output texture")]
    public List<Symbol> controllerList = new();
    [Name("Owner"), Description("The owner of this texture blend. This is used to determine if the texture blend is visible. For example, if this texture blend is used in the head object of a character, set the owner to be the head object.")]
    public Symbol owner = Symbol.Empty;
    [Name("Controller Influence"), Description("Global strength of the blending effect for each controller"), MinVersion(2)]
    public float controllerInfluence;

    public new RndTexBlender Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        draw = draw.Read(reader, false, parent, entry);

        outputTexture = Symbol.Read(reader);
        baseMap = Symbol.Read(reader);
        nearMap = Symbol.Read(reader);
        farMap = Symbol.Read(reader);
        controllerList = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        owner = Symbol.Read(reader);
        if (revision > 1)
        {
            controllerInfluence = reader.ReadFloat();
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
        draw.Write(writer, false, parent, null);

        Symbol.Write(writer, outputTexture);
        Symbol.Write(writer, baseMap);
        Symbol.Write(writer, nearMap);
        Symbol.Write(writer, farMap);

        AssetHelpers.WriteList(writer, controllerList, controller => Symbol.Write(writer, controller));

        Symbol.Write(writer, owner);
        if (revision > 1)
        {
            writer.WriteFloat(controllerInfluence);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
