using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharWalk"), Description("Given a set of turns, walks, and stops, makes a character walk along waypoints, finishing up with a certain facing. There must be looping transitions for the walks, and transitions from turns to walks. Transitions from walks to stops are not needed as you may have to stop on a dime anyway. Instead the beat track is used to align walks and stops, so both have to be beat aligned to 2 beats, and the right foot needs to fall on the even beats.")]
public class CharWalk : Object
{
    private ushort altRevision;
    private ushort revision;

    public new CharWalk Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

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

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
