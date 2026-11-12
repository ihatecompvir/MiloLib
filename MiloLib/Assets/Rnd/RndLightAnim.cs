using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("LightAnim"), Description("LightAnim objects animate light object properties using keyframe interpolation.")]
public class RndLightAnim : Object
{
    public struct ColorKey
    {
        [Name("Color"), Description("The color to animate to")]
        public HmxColor4 value;
        [Name("Frame"), Description("The frame")]
        public float frame;

        public void Read(EndianReader reader)
        {
            value = new HmxColor4().Read(reader);
            frame = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            (value ?? new HmxColor4()).Write(writer);
            writer.WriteFloat(frame);
        }
    }

    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    [Name("Light"), Description("The light whose color we will animate")]
    public Symbol light = Symbol.Empty;

    [MaxVersion(0)]
    public List<ColorKey> oldColorKeysBefore = new();

    [Name("Color Keys"), Description("The frames and corresponding color at each frame")]
    public List<ColorKey> colorKeys = new();

    [MaxVersion(0)]
    public List<ColorKey> oldColorKeysAfter = new();

    [Name("Keys Owner"), Description("Owner of the keys, usually myself")]
    public Symbol keysOwner = Symbol.Empty;

    public new RndLightAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 1)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);

        light = Symbol.Read(reader);

        if (revision < 1)
        {
            oldColorKeysBefore = ReadColorKeys(reader);
        }

        colorKeys = ReadColorKeys(reader);

        if (revision < 1)
        {
            oldColorKeysAfter = ReadColorKeys(reader);
        }

        keysOwner = Symbol.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision > 1)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);

        Symbol.Write(writer, light);

        if (revision < 1)
        {
            WriteColorKeys(writer, oldColorKeysBefore);
        }

        WriteColorKeys(writer, colorKeys);

        if (revision < 1)
        {
            WriteColorKeys(writer, oldColorKeysAfter);
        }

        Symbol.Write(writer, keysOwner);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private static List<ColorKey> ReadColorKeys(EndianReader reader)
    {
        return AssetHelpers.ReadList(reader, () => { ColorKey key = new(); key.Read(reader); return key; });
    }

    private static void WriteColorKeys(EndianWriter writer, List<ColorKey> keys)
    {
        AssetHelpers.WriteList(writer, keys, key => key.Write(writer));
    }
}
