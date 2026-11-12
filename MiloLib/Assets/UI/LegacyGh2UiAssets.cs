using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

public abstract class LegacyGh2UiAsset : Object
{
    private byte[] opaqueBytes = Array.Empty<byte>();

    protected void ReadOpaque(EndianReader reader, bool standalone)
    {
        if (!standalone)
        {
            throw new InvalidDataException("Legacy GH2 UI assets require a standalone end marker.");
        }
        opaqueBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
    }

    protected void WriteOpaque(EndianWriter writer, bool standalone)
    {
        writer.WriteBlock(opaqueBytes);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

[Name("CheckBox"), Description("UI CheckBox. Indicates on/off state.")]
public class CheckBox : LegacyGh2UiAsset
{
    public new CheckBox Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        ReadOpaque(reader, standalone);
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        WriteOpaque(writer, standalone);
    }
}

[Name("BandSlider"), Description("UI Slider implementation")]
public class BandSlider : LegacyGh2UiAsset
{
    public new BandSlider Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        ReadOpaque(reader, standalone);
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        WriteOpaque(writer, standalone);
    }
}

[Name("BandTextEntry"), Description("Text entry ui")]
public class BandTextEntry : LegacyGh2UiAsset
{
    public new BandTextEntry Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        ReadOpaque(reader, standalone);
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        WriteOpaque(writer, standalone);
    }
}
