using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("Label3d"), Description("3d label")]
public class Label3d : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndTrans trans = new();
    public RndDrawable draw = new();

    [Name("Text"), Description("string to turn into 3d text")]
    public Symbol text = Symbol.Empty;

    [Name("Localize"), Description("Try to localize text"), MinVersion(1)]
    public bool localize;

    [Name("Resource"), Description("milo file with letter objects")]
    public Symbol resource = Symbol.Empty;

    [Name("Max Width"), Description("when length is greater than this value, start scaling the phrase")]
    public float maxWidth;

    [Name("All Caps"), Description("You know you make me want to SHOUT!")]
    public bool allCaps;

    [Name("Jitter Depth"), Description("jitter each letter in depth")]
    public float jitterDepth;

    [Name("Jitter Height"), Description("jitter each letter in height")]
    public float jitterHeight;

    public new Label3d Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        text = Symbol.Read(reader);
        resource = Symbol.Read(reader);
        if (revision > 0)
        {
            localize = reader.ReadBoolean();
        }

        maxWidth = reader.ReadFloat();
        allCaps = reader.ReadBoolean();
        jitterDepth = reader.ReadFloat();
        jitterHeight = reader.ReadFloat();

        objFields = new ObjectFields().Read(reader, parent, entry);
        trans = trans.Read(reader, false, parent, entry);
        draw = draw.Read(reader, false, parent, entry);


        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        Symbol.Write(writer, text);
        Symbol.Write(writer, resource);
        if (revision > 0)
        {
            writer.WriteBoolean(localize);
        }

        writer.WriteFloat(maxWidth);
        writer.WriteBoolean(allCaps);
        writer.WriteFloat(jitterDepth);
        writer.WriteFloat(jitterHeight);

        objFields.Write(writer, parent);
        trans.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
