using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharInterest"), Description("An interest object for a character to look at")]
public class CharInterest : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndTrans trans = new();

    [Name("Max View Angle"), Description("In degrees, the maximum view cone angle for this object to be 'seen'")]
    public float maxViewAngle;
    [Name("Priority"), Description("An extra weight applied during scoring of this interest - use this to make it more or less important overall")]
    public float priority;
    [Name("Min Look Time"), Description("The minimum time you have to look at this object when its selected")]
    public float minLookTime;
    [Name("Max Look Time"), Description("The maximum allowable time to look at this object")]
    public float maxLookTime;
    [Name("Refractory Period"), Description("In secs, how long until this object can be looked at again")]
    public float refractoryPeriod;
    [Name("Dart Ruleset Override"), Description("if set, this dart ruleset will override the default one when looking at this interest object"), MinVersion(6)]
    public Symbol charEyeDartOverride = Symbol.Empty;
    [MinVersion(3)]
    public int categoryFlags;
    [Name("Override Min Target Distance"), Description("if true, we will override the minimum distance this target can be from the eyes using the value below"), MinVersion(5)]
    public bool overrideMinTargetDistance;
    [Name("Min Target Distance Override"), Description("the minimum distance, in inches, that this interest can be from the eyes. only applied if overrides_min_target_dist is true..."), MinVersion(5)]
    public float minTargetDistanceOverride;
    public float maxViewAngleCosine;

    [MaxVersion(5)]
    public Symbol unkSym = Symbol.Empty;

    [MinVersion(3), MaxVersion(3)]
    public byte unkByte;

    public new CharInterest Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        trans = trans.Read(reader, false, parent, entry);

        maxViewAngle = reader.ReadFloat();
        priority = reader.ReadFloat();
        minLookTime = reader.ReadFloat();
        maxLookTime = reader.ReadFloat();
        refractoryPeriod = reader.ReadFloat();

        // this was copy pasted ghidra output that i have yet to clean up, which is why it looks so weird
        if (((short)((revision + 0x10000)) - 2 <= 3))
        {
            unkSym = Symbol.Read(reader);
        }
        else if (((short)((revision + 0x10000)) > 5))
        {
            charEyeDartOverride = Symbol.Read(reader);
        }
        if (revision > 2)
        {
            categoryFlags = reader.ReadInt32();
            if (revision == 3)
            {
                unkByte = reader.ReadByte();
            }
        }
        if (revision > 4)
        {
            overrideMinTargetDistance = reader.ReadBoolean();
            minTargetDistanceOverride = reader.ReadFloat();
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

        trans.Write(writer, false, parent, null);

        writer.WriteFloat(maxViewAngle);
        writer.WriteFloat(priority);
        writer.WriteFloat(minLookTime);
        writer.WriteFloat(maxLookTime);
        writer.WriteFloat(refractoryPeriod);

        // this was copy pasted ghidra output that i have yet to clean up, which is why it looks so weird
        if (((short)((revision + 0x10000)) - 2 <= 3))
        {
            Symbol.Write(writer, unkSym);
        }
        else if (((short)((revision + 0x10000)) > 5))
        {
            Symbol.Write(writer, charEyeDartOverride);
        }

        if (revision > 2)
        {
            writer.WriteInt32(categoryFlags);
            if (revision == 3)
            {
                writer.WriteByte(unkByte);
            }
        }

        if (revision > 4)
        {
            writer.WriteBoolean(overrideMinTargetDistance);
            writer.WriteFloat(minTargetDistanceOverride);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
