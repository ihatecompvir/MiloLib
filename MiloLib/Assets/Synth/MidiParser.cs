using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("MidiParser"), Description("Parses midi files into messages to send to other objects or itself")]
public class MidiParser : Object
{
    public new MidiParser Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        base.Read(reader, standalone, parent, entry);
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        base.Write(writer, standalone, parent, entry);
    }
}
