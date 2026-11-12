using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("TextEntry")]
public class PhaseTextEntry : UIComponent
{
    private ushort revision = 1;
    private ushort altRevision;
    public float unkFloat1;
    public float unkFloat2;
    public HmxColor4 color = new();
    public uint unkSetting;
    public float textSize;

    private void ValidateLayout(DirectoryMeta parent)
    {
        if (parent.revision != 26 || revision != 1)
        {
            throw new NotSupportedException($"Unsupported Phase TextEntry layout: scene {parent.revision}, revision {revision}.");
        }
    }

    public new PhaseTextEntry Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        ValidateLayout(parent);
        base.Read(reader, false, parent, entry);
        unkFloat1 = reader.ReadFloat();
        unkFloat2 = reader.ReadFloat();
        color.Read(reader);
        unkSetting = reader.ReadUInt32();
        textSize = reader.ReadFloat();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateLayout(parent);
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        writer.WriteFloat(unkFloat1);
        writer.WriteFloat(unkFloat2);
        color.Write(writer);
        writer.WriteUInt32(unkSetting);
        writer.WriteFloat(textSize);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
