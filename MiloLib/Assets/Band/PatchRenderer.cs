using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("PatchRenderer"), Description("Special texture renderer for character patches")]
public class PatchRenderer : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndTexRenderer texRenderer = new();

    [Name("Test Mode"), Description("Blank/test modes for milo")]
    public Symbol testMode = Symbol.Empty;

    [Name("Position"), Description("Position of patch, controls which cam shot we use")]
    public Symbol position = Symbol.Empty;

    [Name("Back Mat"), Description("background material for patch"), MinVersion(1)]
    public Symbol backMat = Symbol.Empty;

    [Name("Overlay Mat"), Description("Overlay material for patch"), MinVersion(1)]
    public Symbol overlayMat = Symbol.Empty;

    public new PatchRenderer Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        texRenderer = texRenderer.Read(reader, false, parent, entry);

        testMode = Symbol.Read(reader);
        position = Symbol.Read(reader);

        if (revision != 0)
        {
            backMat = Symbol.Read(reader);
            overlayMat = Symbol.Read(reader);
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

        texRenderer.Write(writer, false, parent, entry);

        Symbol.Write(writer, testMode);
        Symbol.Write(writer, position);

        if (revision != 0)
        {
            Symbol.Write(writer, backMat);
            Symbol.Write(writer, overlayMat);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
