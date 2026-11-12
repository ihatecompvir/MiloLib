using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("PlotBeatAnims")]
public class PlotBeatAnims : Object
{
    public class BeatAnim
    {
        [Name("Anim"), Description("Animation to play.")]
        public Symbol anim = Symbol.Empty;
        [Name("Start Frame")]
        public float startFrame;
        [Name("End Frame")]
        public float endFrame;
        [Name("Frames Per Beat")]
        public float framesPerBeat;

        public BeatAnim Read(EndianReader reader)
        {
            anim = Symbol.Read(reader);
            startFrame = reader.ReadFloat();
            endFrame = reader.ReadFloat();
            framesPerBeat = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, anim);
            writer.WriteFloat(startFrame);
            writer.WriteFloat(endFrame);
            writer.WriteFloat(framesPerBeat);
        }
    }

    private ushort revision;
    private ushort altRevision;
    [Name("Peak Beat Anims")]
    public List<BeatAnim> peakBeatAnims = new();
    [Name("Normal Beat Anims")]
    public List<BeatAnim> normalBeatAnims = new();

    public new PlotBeatAnims Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        peakBeatAnims = AssetHelpers.ReadList(reader, () => new BeatAnim().Read(reader));
        normalBeatAnims = AssetHelpers.ReadList(reader, () => new BeatAnim().Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, peakBeatAnims, anim => anim.Write(writer));
        AssetHelpers.WriteList(writer, normalBeatAnims, anim => anim.Write(writer));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
