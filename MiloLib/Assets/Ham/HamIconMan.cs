using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamIconMan"), Description("Icon Man to render icon man in game")]
public class HamIconMan : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndAnimatable anim = new();
    public RndDrawable draw = new();
    [Name("Texture")]
    public Symbol texture = Symbol.Empty;
    [Name("Start Beat")]
    public float startBeat;
    [Name("End Beat")]
    public float endBeat;
    [Name("Offset")]
    public float offset;
    [Name("Difficulty"), Description("Difficulty for iconman to be dancing")]
    public int difficulty;

    public new HamIconMan Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        anim.Read(reader, parent, entry);
        draw.Read(reader, false, parent, entry);
        texture = Symbol.Read(reader);
        startBeat = reader.ReadFloat();
        endBeat = reader.ReadFloat();
        offset = reader.ReadFloat();
        if (revision >= 1)
        {
            difficulty = reader.ReadInt32();
        }
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
        anim.Write(writer);
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, texture);
        writer.WriteFloat(startBeat);
        writer.WriteFloat(endBeat);
        writer.WriteFloat(offset);
        if (revision >= 1)
        {
            writer.WriteInt32(difficulty);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
