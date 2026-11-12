using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharDriver"), Description("Class to Drive, Schedule and Blend CharClips. Basically a stack, new ones are pushed onto the bottom, start playing at some point, and then pop off the ones above it once they are fully blended in.")]
public class CharDriver : Object
{
    public enum ApplyMode
    {
        Blend = 0,
        Add = 1,
        RotateTo = 2,
        BlendWeights = 3
    }

    private ushort altRevision;
    private ushort revision;

    [HideInInspector, MaxVersion(2)]
    public int oldInt;

    public CharWeightable weightable = new();

    [Name("Bones"), Description("The CharBones object to add or blend into.")]
    public Symbol bones = Symbol.Empty;

    [HideInInspector, MaxVersion(7)]
    public Symbol oldClipFilePath = Symbol.Empty;

    [Name("Clips"), Description("pointer to clips object")]
    public Symbol clips = Symbol.Empty;

    [Name("Blend Width"), Description("Width in beats to blend to the next clip"), MinVersion(9)]
    public float blendWidth;

    [Name("Realign"), Description("Realigns the clips to always be aligned with the beat in the measure"), MinVersion(2)]
    public bool realign;

    [Name("Apply"), Description("How to apply the driver to its bones")]
    public ApplyMode apply = ApplyMode.Blend;

    [Name("Clip Type"), Description("What type of clip we can blend"), MinVersion(10)]
    public Symbol clipType = Symbol.Empty;

    [Name("Test Clip"), Description("Pick a clip to play"), MinVersion(4)]
    public Symbol testClip = Symbol.Empty;

    [Name("Default Clip Or Group"), Description("Clip or Group played on enter by default"), MinVersion(12)]
    public Symbol defaultClipOrGroup = Symbol.Empty;

    [Name("Default Play Starved"), Description("If true, plays the default_clip_or_group whenever starved"), MinVersion(14)]
    public bool defaultPlayStarved;

    [Name("Play Multiple Clips"), Description("Turn on to set this driver to play multiple clips"), MinVersion(13)]
    public bool playMultipleClips;

    public new CharDriver Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        weightable = weightable.Read(reader, false, parent, entry);

        if (revision < 3)
        {
            oldInt = reader.ReadInt32();
        }

        bones = Symbol.Read(reader);

        if (revision < 8)
        {
            oldClipFilePath = Symbol.Read(reader);
            if (revision > 6 && string.IsNullOrEmpty(oldClipFilePath.value))
            {
                clips = Symbol.Read(reader);
            }
        }
        else
        {
            clips = Symbol.Read(reader);
        }

        if (revision > 8)
        {
            blendWidth = reader.ReadFloat();
        }

        if (revision > 1)
        {
            realign = reader.ReadBoolean();
        }
        else
        {
            realign = false;
        }

        if (revision > 5)
        {
            apply = (ApplyMode)reader.ReadInt32();
        }
        else if (revision > 4)
        {
            apply = reader.ReadBoolean() ? ApplyMode.Add : ApplyMode.Blend;
        }
        else
        {
            apply = ApplyMode.Blend;
        }

        if (revision > 9)
        {
            clipType = Symbol.Read(reader);
        }

        if (revision > 0xC)
        {
            playMultipleClips = reader.ReadBoolean();
        }

        if (revision > 3)
        {
            testClip = Symbol.Read(reader);
        }

        if (revision > 0xB)
        {
            defaultClipOrGroup = Symbol.Read(reader);
        }

        if (revision > 0xD)
        {
            defaultPlayStarved = reader.ReadBoolean();
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
        weightable.Write(writer, false, parent, entry);

        if (revision < 3)
        {
            writer.WriteInt32(oldInt);
        }

        Symbol.Write(writer, bones);

        if (revision < 8)
        {
            Symbol.Write(writer, oldClipFilePath);
            if (revision > 6 && string.IsNullOrEmpty(oldClipFilePath.value))
            {
                Symbol.Write(writer, clips);
            }
        }
        else
        {
            Symbol.Write(writer, clips);
        }

        if (revision > 8)
        {
            writer.WriteFloat(blendWidth);
        }

        if (revision > 1)
        {
            writer.WriteBoolean(realign);
        }

        if (revision > 5)
        {
            writer.WriteInt32((int)apply);
        }
        else if (revision > 4)
        {
            writer.WriteBoolean(apply != ApplyMode.Blend);
        }

        if (revision > 9)
        {
            Symbol.Write(writer, clipType);
        }

        if (revision > 0xC)
        {
            writer.WriteBoolean(playMultipleClips);
        }

        if (revision > 3)
        {
            Symbol.Write(writer, testClip);
        }

        if (revision > 0xB)
        {
            Symbol.Write(writer, defaultClipOrGroup);
        }

        if (revision > 0xD)
        {
            writer.WriteBoolean(defaultPlayStarved);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
