using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamDirector"), Description("Hammer Director, sits in each song file and manages camera + scene changes")]
public class HamDirector : Object
{
    private ushort revision;
    private ushort altRevision;
    public Object poll = new();
    public RndDrawable draw = new();
    public Symbol oldSymbol = Symbol.Empty;
    public int oldValue;
    [Name("Practice Start"), Description("start frame of practice mode")]
    public Symbol practiceStart = Symbol.Empty;
    [Name("Practice End"), Description("end frame of practice mode")]
    public Symbol practiceEnd = Symbol.Empty;
    [Name("Blend Debug"), Description("If > 0, is which clip to show by itself rather than doing full blending")]
    public int blendDebug;
    [Name("No Transitions"), Description("If true, does not play transitions")]
    public bool noTransitions;
    [Name("Collision Checks"), Description("If true, check character collisions when picking cam shots")]
    public bool collisionChecks;
    [Name("Start Loop Margin"), Description("In practice mode, measures before practice_start until loop")]
    public int startLoopMargin;
    [Name("End Loop Margin"), Description("In practice mode, measures after practice_end until loop")]
    public int endLoopMargin;

    public new HamDirector Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);
        base.Read(reader, false, parent, entry);
        poll.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        if (revision < 7)
        {
            oldSymbol = Symbol.Read(reader);
        }
        if (revision < 8)
        {
            oldValue = reader.ReadInt32();
        }
        if (revision > 2)
        {
            practiceStart = Symbol.Read(reader);
            practiceEnd = Symbol.Read(reader);
        }
        if (revision > 3)
        {
            blendDebug = reader.ReadInt32();
        }
        if (revision > 4)
        {
            noTransitions = reader.ReadBoolean();
        }
        if (revision > 8)
        {
            collisionChecks = reader.ReadBoolean();
        }
        if (revision > 5)
        {
            startLoopMargin = reader.ReadInt32();
            endLoopMargin = reader.ReadInt32();
        }
        if (standalone)
        {
            uint expectedEndBytes = 0xDEADDEAD;
            if (reader.Endianness == Endian.BigEndian)
            {
                expectedEndBytes = 0xADDEADDE;
            }
            if (reader.ReadUInt32() != expectedEndBytes)
            {
                throw MiloLib.Exceptions.MiloAssetReadException.EndBytesNotFound(parent, entry, reader.BaseStream.Position, revision);
            }
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        writer.WriteUInt32((uint)((altRevision << 16) | revision));
        base.Write(writer, false, parent, entry);
        poll.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        if (revision < 7)
        {
            Symbol.Write(writer, oldSymbol);
        }
        if (revision < 8)
        {
            writer.WriteInt32(oldValue);
        }
        if (revision > 2)
        {
            Symbol.Write(writer, practiceStart);
            Symbol.Write(writer, practiceEnd);
        }
        if (revision > 3)
        {
            writer.WriteInt32(blendDebug);
        }
        if (revision > 4)
        {
            writer.WriteBoolean(noTransitions);
        }
        if (revision > 8)
        {
            writer.WriteBoolean(collisionChecks);
        }
        if (revision > 5)
        {
            writer.WriteInt32(startLoopMargin);
            writer.WriteInt32(endLoopMargin);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
