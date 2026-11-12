using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.P9;

[Name("CrowdExtrasManager"), Description("A manager for crowd extras that occasionally perform animations (crowd surfers, stage divers, etc)")]
public class CrowdExtrasManager : Object
{
    public class Extra
    {
        [Name("Label"), Description("The identifier for this extra")]
        public Symbol label = Symbol.Empty;
        [Name("Character"), Description("The character to perform the animation")]
        public Symbol character = Symbol.Empty;
        [Name("Anim"), Description("Animation to play on the character")]
        public Symbol anim = Symbol.Empty;
        [Name("Loop"), Description("Keep looping until stopped")]
        public bool loop;

        public Extra Read(EndianReader reader)
        {
            label = Symbol.Read(reader);
            character = Symbol.Read(reader);
            anim = Symbol.Read(reader);
            loop = reader.ReadBoolean();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, label);
            Symbol.Write(writer, character);
            Symbol.Write(writer, anim);
            writer.WriteBoolean(loop);
        }
    }

    private ushort revision = 1;
    private ushort altRevision;
    [Name("Extras")]
    public List<Extra> extras = new();

    public new CrowdExtrasManager Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        extras = AssetHelpers.ReadList(reader, () => new Extra().Read(reader));
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
        AssetHelpers.WriteList(writer, extras, extra => extra.Write(writer));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
