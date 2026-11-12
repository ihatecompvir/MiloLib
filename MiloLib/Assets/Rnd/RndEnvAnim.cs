using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("EnvAnim"), Description("An EnvAnim object animates Environ properties.")]
public class RndEnvAnim : Object
{
    public struct ColorKey
    {
        public float frame;
        public float r, g, b, a;

        public void Read(EndianReader reader)
        {
            r = reader.ReadFloat();
            g = reader.ReadFloat();
            b = reader.ReadFloat();
            a = reader.ReadFloat();
            frame = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteFloat(r);
            writer.WriteFloat(g);
            writer.WriteFloat(b);
            writer.WriteFloat(a);
            writer.WriteFloat(frame);
        }
    }

    public struct FogRangeKey
    {
        public float frame;
        public float start, end;

        public void Read(EndianReader reader)
        {
            start = reader.ReadFloat();
            end = reader.ReadFloat();
            frame = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteFloat(start);
            writer.WriteFloat(end);
            writer.WriteFloat(frame);
        }
    }

    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    public Symbol environ = Symbol.Empty;
    public List<ColorKey> ambientColorKeys = new();
    public Symbol keysOwner = Symbol.Empty;
    [MinVersion(2)]
    public List<ColorKey> fogColorKeys = new();
    [MinVersion(3)]
    public List<FogRangeKey> fogRangeKeys = new();

    public new RndEnvAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 3)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);

        environ = Symbol.Read(reader);

        ambientColorKeys = AssetHelpers.ReadList(reader, () => { ColorKey key = new(); key.Read(reader); return key; });

        keysOwner = Symbol.Read(reader);

        if (revision > 1)
        {
            fogColorKeys = AssetHelpers.ReadList(reader, () => { ColorKey key = new(); key.Read(reader); return key; });
        }

        if (revision > 2)
        {
            fogRangeKeys = AssetHelpers.ReadList(reader, () => { FogRangeKey key = new(); key.Read(reader); return key; });
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

        if (revision > 3)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);

        Symbol.Write(writer, environ);

        AssetHelpers.WriteList(writer, ambientColorKeys, key => key.Write(writer));

        Symbol.Write(writer, keysOwner);

        if (revision > 1)
        {
            AssetHelpers.WriteList(writer, fogColorKeys, key => key.Write(writer));
        }

        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, fogRangeKeys, key => key.Write(writer));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
