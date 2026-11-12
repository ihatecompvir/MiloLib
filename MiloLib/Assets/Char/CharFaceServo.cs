using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharFaceServo"), Description("BonesMeshes for facial blending")]
public class CharFaceServo : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Clips"), Description("pointer to visemes, must contain Blink and Base")]
    public Symbol clips = Symbol.Empty;

    [Name("Clip Type"), Description("Which clip type it can support"), MinVersion(4)]
    public Symbol clipType = Symbol.Empty;

    [Name("Blink Clip Left"), Description("Blink clip, used to close the left eye"), MinVersion(1)]
    public Symbol blinkClipLeftName = Symbol.Empty;

    [Name("Blink Clip Right"), Description("Blink clip, used to close the right eye"), MinVersion(2)]
    public Symbol blinkClipRightName = Symbol.Empty;

    [Name("Blink Clip Left 2"), Description("A second clip that contributes to closing the left eye"), MinVersion(3)]
    public Symbol blinkClipLeftName2 = Symbol.Empty;

    [Name("Blink Clip Right 2"), Description("A second clip that contributes to closing the right eye"), MinVersion(3)]
    public Symbol blinkClipRightName2 = Symbol.Empty;

    public new CharFaceServo Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        clips = Symbol.Read(reader);

        if (revision > 3)
        {
            clipType = Symbol.Read(reader);
        }

        if (revision != 0)
        {
            blinkClipLeftName = Symbol.Read(reader);
        }

        if (revision > 1)
        {
            blinkClipRightName = Symbol.Read(reader);
        }

        if (revision > 2)
        {
            blinkClipLeftName2 = Symbol.Read(reader);
            blinkClipRightName2 = Symbol.Read(reader);
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

        Symbol.Write(writer, clips);

        if (revision > 3)
        {
            Symbol.Write(writer, clipType);
        }

        if (revision != 0)
        {
            Symbol.Write(writer, blinkClipLeftName);
        }

        if (revision > 1)
        {
            Symbol.Write(writer, blinkClipRightName);
        }

        if (revision > 2)
        {
            Symbol.Write(writer, blinkClipLeftName2);
            Symbol.Write(writer, blinkClipRightName2);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
