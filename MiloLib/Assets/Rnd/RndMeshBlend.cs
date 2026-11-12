using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("MeshBlend")]
public class RndMeshBlend : Object
{
    [HideInInspector]
    private ushort revision = 1;
    [HideInInspector]
    private ushort altRevision;


    [Name("Target"), Description("The mesh we will change")]
    public Symbol target = Symbol.Empty;
    [Name("Weights Owner")]
    public Symbol weightsOwner = Symbol.Empty;
    [Name("Input Anim")]
    public Symbol inputAnim = Symbol.Empty;
    [Name("Input Meshes")]
    public List<Symbol> inputMeshes = new();
    [Name("Weights")]
    public List<float> weights = new();

    public new RndMeshBlend Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);

        target = Symbol.Read(reader);
        weightsOwner = Symbol.Read(reader);
        inputAnim = Symbol.Read(reader);
        inputMeshes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        weights = AssetHelpers.ReadList(reader, () => reader.ReadFloat());
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
        Symbol.Write(writer, weightsOwner);
        Symbol.Write(writer, inputAnim);
        AssetHelpers.WriteList(writer, inputMeshes, value => Symbol.Write(writer, value));
        AssetHelpers.WriteList(writer, weights, value => writer.WriteFloat(value));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
