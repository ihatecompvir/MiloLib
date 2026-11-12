using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("BlendShapesDriver"), Description("A blend shape driver - sets the frame of a meshanim")]
public class BlendShapesDriver : Object
{
    [HideInInspector]
    private ushort revision = 1;
    [HideInInspector]
    private ushort altRevision;


    [Name("Target Mouth")]
    public Symbol targetMouth = Symbol.Empty;
    [Name("Blendmap Mouth")]
    public List<int> blendmapMouth = new();
    [Name("Target Eye")]
    public Symbol targetEye = Symbol.Empty;
    [Name("Blendmap Eye")]
    public List<int> blendmapEye = new();
    [Name("Target Brow")]
    public Symbol targetBrow = Symbol.Empty;
    [Name("Blendmap Brow")]
    public List<int> blendmapBrow = new();

    public new BlendShapesDriver Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        targetMouth = Symbol.Read(reader);
        blendmapMouth = AssetHelpers.ReadList(reader, () => reader.ReadInt32());
        targetEye = Symbol.Read(reader);
        blendmapEye = AssetHelpers.ReadList(reader, () => reader.ReadInt32());
        targetBrow = Symbol.Read(reader);
        blendmapBrow = AssetHelpers.ReadList(reader, () => reader.ReadInt32());
        base.Read(reader, false, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        Symbol.Write(writer, targetMouth);
        AssetHelpers.WriteList(writer, blendmapMouth, value => writer.WriteInt32(value));
        Symbol.Write(writer, targetEye);
        AssetHelpers.WriteList(writer, blendmapEye, value => writer.WriteInt32(value));
        Symbol.Write(writer, targetBrow);
        AssetHelpers.WriteList(writer, blendmapBrow, value => writer.WriteInt32(value));
        base.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
