using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharDriverMidi"), Description("Uses an event list made from a parser to player animation on self.")]
public class CharDriverMidi : CharDriver
{
    private ushort altRevision;
    private ushort revision;

    [HideInInspector, MinVersion(2), MaxVersion(2)]
    public Symbol oldString = Symbol.Empty;

    [Name("Parser"), Description("Names of the midi parser to hook into"), MinVersion(4)]
    public Symbol parser = Symbol.Empty;

    [Name("Flag Parser"), Description("Secondary parser for flags"), MinVersion(5)]
    public Symbol flagParser = Symbol.Empty;

    [Name("Blend Override Pct"), Description("Blend override- set between 0-1 to change clip blending strategy"), MinVersion(6)]
    public float blendOverridePct;

    public new CharDriverMidi Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision < 7)
        {
            defaultClipOrGroup = Symbol.Read(reader);
        }

        if (revision == 2)
        {
            oldString = Symbol.Read(reader);
        }
        else if (revision > 3)
        {
            parser = Symbol.Read(reader);
        }

        if (revision > 4)
        {
            flagParser = Symbol.Read(reader);
        }

        if (revision > 5)
        {
            blendOverridePct = reader.ReadFloat();
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

        if (revision < 7)
        {
            Symbol.Write(writer, defaultClipOrGroup);
        }

        if (revision == 2)
        {
            Symbol.Write(writer, oldString);
        }
        else if (revision > 3)
        {
            Symbol.Write(writer, parser);
        }

        if (revision > 4)
        {
            Symbol.Write(writer, flagParser);
        }

        if (revision > 5)
        {
            writer.WriteFloat(blendOverridePct);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
