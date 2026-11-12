using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharLipSync"), Description("A full lipsync animation, basically a changing set of weights for a set of named visemes. Sampled at 30hz")]
public class CharLipSync : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Viseme Names"), Description("viseme names")]
    public List<Symbol> visemes = new();

    [Name("Keyframes"), Description("how many keyframes")]
    public int frames;

    private uint dataCount;
    public byte[] data = Array.Empty<byte>();

    [Name("PropAnim"), Description("PropAnim to control this lipsync"), MinVersion(1)]
    public Symbol propAnim = Symbol.Empty;

    public new CharLipSync Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        visemes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        frames = reader.ReadInt32();
        dataCount = reader.ReadUInt32();
        data = dataCount > 0 ? reader.ReadBlock((int)dataCount) : Array.Empty<byte>();
        if (revision == 1)
        {
            propAnim = Symbol.Read(reader);
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

        AssetHelpers.WriteList(writer, visemes, (Symbol sym) => Symbol.Write(writer, sym));

        writer.WriteInt32(frames);
        writer.WriteUInt32((uint)data.Length);
        writer.WriteBlock(data);
        if (revision == 1)
        {
            Symbol.Write(writer, propAnim);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
