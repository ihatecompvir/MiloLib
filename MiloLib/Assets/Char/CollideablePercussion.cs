using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CollideablePercussion"), Description("Plays animations based on collision detection between a stick and a percussion instrument, overriding the instruments midi driver.")]
public class CollideablePercussion : Object
{
    private ushort revision = 1;
    private ushort altRevision;
    [Name("Stick Collision Set"), Description("CollideableSet of sticks that collide with instruments.")]
    public Symbol stickCollisionSet = Symbol.Empty;
    [Name("Instrument Collision Set"), Description("CollideableSet set of percussion instruments.")]
    public Symbol instrumentCollisionSet = Symbol.Empty;

    public new CollideablePercussion Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        // seems that collideable shapes don't serialize the Object
        stickCollisionSet = Symbol.Read(reader);
        instrumentCollisionSet = Symbol.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        Symbol.Write(writer, stickCollisionSet);
        Symbol.Write(writer, instrumentCollisionSet);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
