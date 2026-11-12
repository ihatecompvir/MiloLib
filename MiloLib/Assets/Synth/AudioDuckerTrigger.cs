using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("AudioDuckerTrigger"), Description("This object helps triggering audio duckers that are not attached to anything.")]
public class AudioDuckerTrigger : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Duckers")]
    public List<Sound.AudioDucker> duckers = new();

    public new AudioDuckerTrigger Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        duckers = AssetHelpers.ReadList(reader, () => new Sound.AudioDucker().Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, duckers, item => item.Write(writer));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
