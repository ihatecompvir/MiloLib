using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("RandomGroupSeq"), Description("Plays one or more of its child sequences, selected at random.")]
public class RandomGroupSeq : GroupSeq
{
    private ushort altRevision;
    private ushort revision;

    [Name("Number of Simultaneous Sequences"), Description("Number of children to play simultaneously")]
    public uint numSimultaneous;
    [Name("Allow Repeats"), Description("If false, you will never hear the same sequence again until all have played (only if num_simul is 1)"), MinVersion(2)]
    public bool allowRepeats;

    public new RandomGroupSeq Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        numSimultaneous = reader.ReadUInt32();
        if (revision >= 2)
        {
            allowRepeats = reader.ReadBoolean();
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

        base.Write(writer, false, parent, entry);

        writer.WriteUInt32(numSimultaneous);
        if (revision >= 2)
        {
            writer.WriteBoolean(allowRepeats);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
