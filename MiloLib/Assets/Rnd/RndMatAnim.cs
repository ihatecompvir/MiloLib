using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.Rnd.PropKey;

namespace MiloLib.Assets.Rnd;

[Name("MatAnim"), Description("MatAnim objects animate material properties.")]
public class RndMatAnim : Object
{

    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    [Name("Material"), Description("The material to animate.")]
    public Symbol material = Symbol.Empty;

    [Name("Keys Owner"), Description("The owner of the keys.")]
    public Symbol keysOwner = Symbol.Empty;

    [Name("Color Keys"), Description("The color keys of the material animation.")]
    public List<ColorKey> colorKeys = new();

    [Name("Alpha Keys"), Description("The alpha keys of the material animation.")]
    public List<FloatKey> alphaKeys = new();

    [Name("Transform Keys"), Description("The transform keys of the material animation."), MinVersion(7)]
    public List<Vec3Key> transKeys = new();

    [Name("Scale Keys"), Description("The scale keys of the material animation."), MinVersion(7)]
    public List<Vec3Key> scaleKeys = new();

    [Name("Rotation Keys"), Description("The rotation keys of the material animation."), MinVersion(7)]
    public List<Vec3Key> rotKeys = new();

    [Name("Texture Keys"), Description("The texture keys of the material animation."), MinVersion(7)]
    public List<SymbolKey> texKeys = new();

    [MinVersion(2), MaxVersion(4)]
    public List<ColorKey> colorKeys1 = new();
    [MinVersion(2), MaxVersion(2)]
    public List<ColorKey> colorKeys2 = new();
    [MinVersion(2), MaxVersion(3)]
    public List<ColorKey> colorKeys3 = new();

    public List<List<Vec3Key>> vec3Keys1PerStage = new();
    public List<List<Vec3Key>> vec3Keys2PerStage = new();
    public List<List<Vec3Key>> vec3Keys3PerStage = new();

    public List<List<SymbolKey>> symbolKeysPerStage = new();

    private uint stageCount;

    private byte[] gh2OpaqueBytes = Array.Empty<byte>();




    public new RndMatAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (parent.revision == 25 && standalone && revision < 7)
        {
            gh2OpaqueBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
            return this;
        }

        if (revision > 5)
        {
            base.objFields.Read(reader, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);

        material = Symbol.Read(reader);

        if (revision < 7)
        {
            stageCount = reader.ReadUInt32();
            vec3Keys1PerStage = new List<List<Vec3Key>>(stageCount <= int.MaxValue ? (int)stageCount : 0);
            vec3Keys2PerStage = new List<List<Vec3Key>>(stageCount <= int.MaxValue ? (int)stageCount : 0);
            vec3Keys3PerStage = new List<List<Vec3Key>>(stageCount <= int.MaxValue ? (int)stageCount : 0);
            symbolKeysPerStage = new List<List<SymbolKey>>(stageCount <= int.MaxValue ? (int)stageCount : 0);
            for (int i = 0; i < stageCount; i++)
            {
                if (revision != 0)
                {
                    vec3Keys1PerStage.Add(AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader)));
                    vec3Keys2PerStage.Add(AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader)));
                    vec3Keys3PerStage.Add(AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader)));
                }
                if (revision > 1)
                {
                    symbolKeysPerStage.Add(AssetHelpers.ReadList(reader, () => new SymbolKey().Read(reader)));
                }
            }
        }

        keysOwner = Symbol.Read(reader);

        if (revision > 1)
        {
            if (revision < 5)
            {
                colorKeys1 = AssetHelpers.ReadList(reader, () => new ColorKey().Read(reader));
            }
            if (revision < 3)
            {
                colorKeys2 = AssetHelpers.ReadList(reader, () => new ColorKey().Read(reader));
            }
            colorKeys = AssetHelpers.ReadList(reader, () => new ColorKey().Read(reader));

            if (revision < 4)
            {
                colorKeys3 = AssetHelpers.ReadList(reader, () => new ColorKey().Read(reader));
            }

            alphaKeys = AssetHelpers.ReadList(reader, () => new FloatKey().Read(reader));
        }

        if (revision > 6 && !(parent.revision == 26 && revision == 8))
        {
            transKeys = AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader));

            scaleKeys = AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader));

            rotKeys = AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader));

        }

        if (revision > 6)
        {
            texKeys = AssetHelpers.ReadList(reader, () => new SymbolKey().Read(reader));
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

        if (parent.revision == 25 && standalone && revision < 7)
        {
            writer.WriteBlock(gh2OpaqueBytes);
            writer.WriteEndBytes();
            return;
        }

        if (revision > 5)
        {
            base.objFields.Write(writer, parent);
        }

        anim.Write(writer);

        Symbol.Write(writer, material);

        if (revision < 7)
        {
            writer.WriteUInt32(stageCount);

            for (int i = 0; i < stageCount; i++)
            {
                if (revision != 0)
                {
                    var stageKeys1 = i < vec3Keys1PerStage.Count ? vec3Keys1PerStage[i] : new List<Vec3Key>();
                    AssetHelpers.WriteList(writer, stageKeys1, (Vec3Key vec3Key) => vec3Key.Write(writer));

                    var stageKeys2 = i < vec3Keys2PerStage.Count ? vec3Keys2PerStage[i] : new List<Vec3Key>();
                    AssetHelpers.WriteList(writer, stageKeys2, (Vec3Key vec3Key) => vec3Key.Write(writer));

                    var stageKeys3 = i < vec3Keys3PerStage.Count ? vec3Keys3PerStage[i] : new List<Vec3Key>();
                    AssetHelpers.WriteList(writer, stageKeys3, (Vec3Key vec3Key) => vec3Key.Write(writer));
                }

                if (revision > 1)
                {
                    var stageSymKeys = i < symbolKeysPerStage.Count ? symbolKeysPerStage[i] : new List<SymbolKey>();
                    AssetHelpers.WriteList(writer, stageSymKeys, (SymbolKey symbolKey) => symbolKey.Write(writer));
                }
            }
        }

        Symbol.Write(writer, keysOwner);

        if (revision > 1)
        {
            if (revision < 5)
            {
                AssetHelpers.WriteList(writer, colorKeys1, colorKey => colorKey.Write(writer));
            }
            if (revision < 3)
            {
                AssetHelpers.WriteList(writer, colorKeys2, colorKey => colorKey.Write(writer));
            }
            AssetHelpers.WriteList(writer, colorKeys, colorKey => colorKey.Write(writer));

            if (revision < 4)
            {
                AssetHelpers.WriteList(writer, colorKeys3, colorKey => colorKey.Write(writer));
            }

            AssetHelpers.WriteList(writer, alphaKeys, alphaKey => alphaKey.Write(writer));
        }

        if (revision > 6 && !(parent.revision == 26 && revision == 8))
        {
            AssetHelpers.WriteList(writer, transKeys, transKey => transKey.Write(writer));

            AssetHelpers.WriteList(writer, scaleKeys, scaleKey => scaleKey.Write(writer));

            AssetHelpers.WriteList(writer, rotKeys, rotKey => rotKey.Write(writer));

        }

        if (revision > 6)
        {
            AssetHelpers.WriteList(writer, texKeys, texKey => texKey.Write(writer));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
