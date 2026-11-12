using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("TriggerGroup"), Description("Used for firing triggers in song.anim.")]
public class RndTriggerGroup : RndGroup
{
    private ushort revision = 1;
    private ushort altRevision;

    [Name("Trigger Name"), Description("The trigger to call on all WorldInstance objects contained in this group")]
    public Symbol triggerName = Symbol.Empty;

    public new RndTriggerGroup Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        triggerName = Symbol.Read(reader);
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
        Symbol.Write(writer, triggerName);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
