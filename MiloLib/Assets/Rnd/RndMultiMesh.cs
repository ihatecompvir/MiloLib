using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("MultiMesh"), Description("A MultiMesh object draws a simple Mesh in many places quickly. Create a multimesh by instancing the base mesh lots of times in Max, then using the multiobject wizard.")]
public class RndMultiMesh : Object
{
    private static readonly HmxColor4 DefaultLegacyColor = new();

    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();
    public Symbol mesh = Symbol.Empty;
    public List<Matrix> instances = new();
    [MinVersion(5)]
    public List<bool> instanceFlags = new();
    [MaxVersion(2)]
    public List<HmxColor4> oldInstanceColors = new();
    [MinVersion(3), MaxVersion(3)]
    public List<List<HmxColor4>> oldInstanceColorSets = new();
    [MinVersion(2), MaxVersion(3)]
    public byte oldTrailingValue;

    public new RndMultiMesh Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 0)
        {
            base.Read(reader, false, parent, entry);
        }

        draw = draw.Read(reader, false, parent, entry);

        mesh = Symbol.Read(reader);

        instances.Clear();
        instanceFlags.Clear();
        oldInstanceColors.Clear();
        oldInstanceColorSets.Clear();

        uint instanceCount = reader.ReadUInt32();

        for (int i = 0; i < instanceCount; i++)
        {
            if (revision >= 5)
            {
                instanceFlags.Add(reader.ReadBoolean());
            }

            instances.Add(new Matrix().Read(reader));

            if (revision < 3 && revision >= 2)
            {
                oldInstanceColors.Add(new HmxColor4().Read(reader));
            }
            else if (revision < 4 && revision >= 3)
            {
                oldInstanceColorSets.Add(AssetHelpers.ReadList(reader, () => new HmxColor4().Read(reader)));
            }
        }

        if (revision < 4 && revision >= 2)
        {
            oldTrailingValue = reader.ReadByte();
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

        if (revision != 0)
        {
            base.Write(writer, false, parent, entry);
        }

        draw.Write(writer, false, parent, null);

        Symbol.Write(writer, mesh);

        writer.WriteUInt32((uint)instances.Count);
        for (int i = 0; i < instances.Count; i++)
        {
            if (revision >= 5)
            {
                writer.WriteBoolean(i < instanceFlags.Count ? instanceFlags[i] : true);
            }

            instances[i].Write(writer);

            if (revision < 3 && revision >= 2)
            {
                HmxColor4 legacyColor = DefaultLegacyColor;
                if (i < oldInstanceColors.Count && oldInstanceColors[i] != null)
                {
                    legacyColor = oldInstanceColors[i];
                }
                legacyColor.Write(writer);
            }
            else if (revision < 4 && revision >= 3)
            {
                List<HmxColor4> colors = new();
                if (i < oldInstanceColorSets.Count && oldInstanceColorSets[i] != null)
                {
                    colors = oldInstanceColorSets[i];
                }
                AssetHelpers.WriteList(writer, colors, color => color.Write(writer));
            }
        }

        if (revision < 4 && revision >= 2)
        {
            writer.WriteByte(oldTrailingValue);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
