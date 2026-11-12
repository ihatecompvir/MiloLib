using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.Rnd.PropKey;


namespace MiloLib.Assets.Rnd;

[Name("ParticleSysAnim"), Description("Object that animates Particle System properties.")]
public class RndParticleSysAnim : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    public Symbol particleSys = Symbol.Empty;
    public Symbol keysOwner = Symbol.Empty;

    public List<ColorKey> startColorKeys = new();

    public List<ColorKey> endColorKeys = new();

    [MinVersion(2)]
    public List<Vec2Key> emitRateKeys = new();

    [MinVersion(2)]
    public List<Vec2Key> speedKeys = new();

    [MinVersion(2)]
    public List<Vec2Key> lifeKeys = new();

    [MinVersion(2)]
    public List<Vec2Key> startSizeKeys = new();

    [MaxVersion(1)]
    public List<FloatKey> fKeys = new();

    [MinVersion(1), MaxVersion(1)]
    public float unkFloat;



    public new RndParticleSysAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 2)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);

        particleSys = Symbol.Read(reader);

        startColorKeys = AssetHelpers.ReadList(reader, () => new ColorKey().Read(reader));

        endColorKeys = AssetHelpers.ReadList(reader, () => new ColorKey().Read(reader));

        if (revision < 2)
        {
            fKeys = AssetHelpers.ReadList(reader, () => new FloatKey().Read(reader));

            keysOwner = Symbol.Read(reader);
            if (revision == 1)
            {
                unkFloat = reader.ReadFloat();
            }
        }
        else
        {
            emitRateKeys = AssetHelpers.ReadList(reader, () => new Vec2Key().Read(reader));
            keysOwner = Symbol.Read(reader);
        }

        if (revision > 1)
        {
            speedKeys = AssetHelpers.ReadList(reader, () => new Vec2Key().Read(reader));
            lifeKeys = AssetHelpers.ReadList(reader, () => new Vec2Key().Read(reader));
            startSizeKeys = AssetHelpers.ReadList(reader, () => new Vec2Key().Read(reader));
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

        if (revision > 2)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);

        Symbol.Write(writer, particleSys);

        AssetHelpers.WriteList(writer, startColorKeys, (ColorKey colorKey) => colorKey.Write(writer));

        AssetHelpers.WriteList(writer, endColorKeys, (ColorKey colorKey) => colorKey.Write(writer));

        if (revision < 2)
        {
            AssetHelpers.WriteList(writer, fKeys, (FloatKey floatKey) => floatKey.Write(writer));
            Symbol.Write(writer, keysOwner);
            if (revision == 1)
            {
                writer.WriteFloat(unkFloat);
            }
        }
        else
        {
            AssetHelpers.WriteList(writer, emitRateKeys, (Vec2Key vec2Key) => vec2Key.Write(writer));
            Symbol.Write(writer, keysOwner);
        }

        if (revision > 1)
        {
            AssetHelpers.WriteList(writer, speedKeys, (Vec2Key vec2Key) => vec2Key.Write(writer));
            AssetHelpers.WriteList(writer, lifeKeys, (Vec2Key vec2Key) => vec2Key.Write(writer));
            AssetHelpers.WriteList(writer, startSizeKeys, (Vec2Key vec2Key) => vec2Key.Write(writer));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
