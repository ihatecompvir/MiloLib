using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Synth;

[Name("SynthFader"), Description("A fader controls the volume level of one or more sound effects.")]
public class SynthFader : Object
{
    private ushort altRevision;
    private ushort revision;

    [Name("Level"), Description("volume level in dB")]
    public float level;

    [Name("Level Ease Type"), Description("Equation used to change volume. See http://deki/Projects/Tool_Projects/Milo/Flow/Easing_equations"), MinVersion(1)]
    public int levelEaseType;
    [Name("Level Ease Param"), Description("Optional parameter for falloff equation"), MinVersion(1)]
    public float levelEaseParam;
    [Name("Level Target"), Description("Target volume level in dB"), MinVersion(1)]
    public float levelTarget;
    [Name("Pan"), Description("Surround pan, between -4 and 4. Only applies to mono, non-3D sounds"), MinVersion(1)]
    public float pan;
    [Name("Pan Ease Type"), Description("Equation used to change pan. See http://deki/Projects/Tool_Projects/Milo/Flow/Easing_equations"), MinVersion(1)]
    public int panEaseType;
    [Name("Pan Ease Param"), Description("Optional parameter for falloff equation"), MinVersion(1)]
    public float panEaseParam;
    [Name("Pan Target"), Description("Target pan, between -4 and 4. Only applies to mono, non-3D sounds"), MinVersion(1)]
    public float panTarget;
    [Name("Transpose"), Description("Transpose, in half-steps. Only applies to sounds playing SynthSamples"), MinVersion(1)]
    public float transpose;
    [Name("Transpose Ease Type"), Description("Equation used to change transpose. See http://deki/Projects/Tool_Projects/Milo/Flow/Easing_equations"), MinVersion(1)]
    public int transposeEaseType;
    [Name("Transpose Ease Param"), Description("Optional parameter for falloff equation"), MinVersion(1)]
    public float transposeEaseParam;
    [Name("Transpose Target"), Description("Target transpose"), MinVersion(1)]
    public float transposeTarget;

    public new SynthFader Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        level = reader.ReadFloat();

        if (revision >= 1)
        {
            levelEaseType = reader.ReadInt32();
            levelEaseParam = reader.ReadFloat();
            levelTarget = reader.ReadFloat();
            pan = reader.ReadFloat();
            panEaseType = reader.ReadInt32();
            panEaseParam = reader.ReadFloat();
            panTarget = reader.ReadFloat();
            transpose = reader.ReadFloat();
            transposeEaseType = reader.ReadInt32();
            transposeEaseParam = reader.ReadFloat();
            transposeTarget = reader.ReadFloat();
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
        writer.WriteFloat(level);

        if (revision >= 1)
        {
            writer.WriteInt32(levelEaseType);
            writer.WriteFloat(levelEaseParam);
            writer.WriteFloat(levelTarget);
            writer.WriteFloat(pan);
            writer.WriteInt32(panEaseType);
            writer.WriteFloat(panEaseParam);
            writer.WriteFloat(panTarget);
            writer.WriteFloat(transpose);
            writer.WriteInt32(transposeEaseType);
            writer.WriteFloat(transposeEaseParam);
            writer.WriteFloat(transposeTarget);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
