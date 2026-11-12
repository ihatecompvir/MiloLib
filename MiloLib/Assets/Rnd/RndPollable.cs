using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Poll"), Description("Abstract base class for pollable RND objects")]
public class RndPollable : Object
{
    public Symbol exit = Symbol.Empty;
    public Symbol enter = Symbol.Empty;

    public new RndPollable Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        exit = Symbol.Read(reader);
        enter = Symbol.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        Symbol.Write(writer, exit);
        Symbol.Write(writer, enter);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
