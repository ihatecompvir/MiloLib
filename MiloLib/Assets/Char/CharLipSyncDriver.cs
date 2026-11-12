using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharLipSyncDriver"), Description("Drives lip sync animation")]
public class CharLipSyncDriver : Object
{
    private ushort altRevision;
    private ushort revision;
    private bool usesGreenDayLayout;

    public CharWeightable weightable = new();

    [Name("Bones"), Description("The CharBones object to add or blend into.")]
    public Symbol bones = Symbol.Empty;

    [Name("Clips"), Description("pointer to the visemes")]
    public Symbol clips = Symbol.Empty;

    [HideInInspector, MaxVersion(0)]
    public Symbol oldLipSyncFilePath = Symbol.Empty;

    [HideInInspector, MaxVersion(0)]
    public Symbol oldLipSyncString = Symbol.Empty;

    [Name("Lipsync"), Description("The lipsync file to use"), MinVersion(1)]
    public Symbol lipSync = Symbol.Empty;

    [Name("Test Clip"), Description("Test charclip to apply, does nothing else"), MinVersion(2)]
    public Symbol testClip = Symbol.Empty;

    [Name("Test Weight"), Description("weight to apply this clip with"), MinVersion(2)]
    public float testWeight;

    [Name("Override Clip"), Description("default clip to be used as the override - maybe be overriden programatically"), MinVersion(3)]
    public Symbol overrideClip = Symbol.Empty;

    [HideInInspector, MinVersion(3), MaxVersion(4)]
    public int oldOverrideInt;

    [Name("Override Options"), Description("an optional clipset that provides list of clips to override face with - viseme clipset is used otherwise"), MinVersion(3)]
    public Symbol overrideOptions = Symbol.Empty;

    [Name("Apply Override Additively"), Description("is the override clip applied addtively on top of face mocap? If false, it will blend."), MinVersion(4)]
    public bool applyOverrideAdditively;

    [Name("Override Weight"), Description("weight to blend override clip. this is mostly here for testing, because its likely to be set programatically."), MinVersion(6)]
    public float overrideWeight;

    [Name("Alternate Driver"), Description("This will be used instead of the song, if set"), MinVersion(7)]
    public Symbol alternateDriver = Symbol.Empty;

    [MinVersion(8), MaxVersion(8)]
    public Symbol oldRevision8Symbol = Symbol.Empty;

    public new CharLipSyncDriver Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        usesGreenDayLayout = revision == 8 && parent.revision == 25;

        base.Read(reader, false, parent, entry);
        weightable = weightable.Read(reader, false, parent, entry);

        bones = Symbol.Read(reader);
        clips = Symbol.Read(reader);

        if (revision < 1)
        {
            oldLipSyncFilePath = Symbol.Read(reader);
            oldLipSyncString = Symbol.Read(reader);
        }
        else
        {
            lipSync = Symbol.Read(reader);
        }

        if (revision > 1)
        {
            testClip = Symbol.Read(reader);
            testWeight = reader.ReadFloat();
        }

        if (revision > 2 && !usesGreenDayLayout)
        {
            overrideClip = Symbol.Read(reader);
            if (revision < 5)
            {
                oldOverrideInt = reader.ReadInt32();
            }

            overrideOptions = Symbol.Read(reader);
        }

        if (revision > 3 && !usesGreenDayLayout)
        {
            applyOverrideAdditively = reader.ReadBoolean();
        }

        if (revision > 5 && !usesGreenDayLayout)
        {
            overrideWeight = reader.ReadFloat();
        }

        if (usesGreenDayLayout)
        {
            oldRevision8Symbol = Symbol.Read(reader);
        }
        else if (revision > 6)
        {
            alternateDriver = Symbol.Read(reader);
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

        Symbol.Write(writer, bones);
        Symbol.Write(writer, clips);

        if (revision < 1)
        {
            Symbol.Write(writer, oldLipSyncFilePath);
            Symbol.Write(writer, oldLipSyncString);
        }
        else
        {
            Symbol.Write(writer, lipSync);
        }

        if (revision > 1)
        {
            Symbol.Write(writer, testClip);
            writer.WriteFloat(testWeight);
        }

        if (revision > 2 && !usesGreenDayLayout)
        {
            Symbol.Write(writer, overrideClip);
            if (revision < 5)
            {
                writer.WriteInt32(oldOverrideInt);
            }

            Symbol.Write(writer, overrideOptions);
        }

        if (revision > 3 && !usesGreenDayLayout)
        {
            writer.WriteBoolean(applyOverrideAdditively);
        }

        if (revision > 5 && !usesGreenDayLayout)
        {
            writer.WriteFloat(overrideWeight);
        }

        if (usesGreenDayLayout)
        {
            Symbol.Write(writer, oldRevision8Symbol);
        }
        else if (revision > 6)
        {
            Symbol.Write(writer, alternateDriver);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
