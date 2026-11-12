using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("LightHue"), Description("Represents a hue translation for LightPresets")]
public class LightHue : Object
{
    public struct HueKey
    {
        public float frame;
        public Vector3 value;

        public void Read(EndianReader reader)
        {
            frame = reader.ReadFloat();
            value = new Vector3().Read(reader);
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteFloat(frame);
            value.Write(writer);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Path"), Description("BMP file comparable to /system/run/world/hue.bmp")]
    public Symbol path = Symbol.Empty;
    public List<HueKey> keys = new();

    public new LightHue Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        path = Symbol.Read(reader);

        keys = AssetHelpers.ReadList(reader, () => { HueKey key = new(); key.Read(reader); return key; });

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

        Symbol.Write(writer, path);

        AssetHelpers.WriteList(writer, keys, key => key.Write(writer));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
