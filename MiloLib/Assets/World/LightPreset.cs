using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("LightPreset"), Description("Represents an animated sequence of states of certain objects in the world. For now, we store states for Spotlight and Environment objects.")]
public class LightPreset : Object
{
    public class EnvironmentEntry
    {
        [Name("Ambient Color"), Description("Ambient color")]
        public HmxColor4 ambientColor = new();
        [Name("Fog Enable"), Description("Fog showing?")]
        public bool fogEnable;
        [Name("Fog Start"), Description("Intensity from smoke")]
        public float fogStart;
        [Name("Fog End"), Description("Intensity from smoke")]
        public float fogEnd;
        [Name("Fog Color"), Description("Intensity from smoke")]
        public HmxColor4 fogColor = new();

        public void Read(EndianReader reader)
        {
            ambientColor.Read(reader);
            fogEnable = reader.ReadBoolean();
            fogStart = reader.ReadFloat();
            fogEnd = reader.ReadFloat();
            fogColor.Read(reader);
        }

        public void Write(EndianWriter writer)
        {
            ambientColor.Write(writer);
            writer.WriteBoolean(fogEnable);
            writer.WriteFloat(fogStart);
            writer.WriteFloat(fogEnd);
            fogColor.Write(writer);
        }
    }

    public class EnvLightEntry
    {
        [Name("Rotation"), Description("Light transform")]
        public Vector4 rotationQuat = new();
        [Name("Position"), Description("Light's position")]
        public Vector3 position = new();
        [Name("Color"), Description("Light's color")]
        public HmxColor4 color = new();
        [Name("Range"), Description("Falloff distance for point lights")]
        public float range;
        [Name("Light Type"), Description("Light type")]
        public RndLight.Type lightType;

        public void Read(EndianReader reader)
        {
            rotationQuat = new Vector4().Read(reader);
            position = new Vector3().Read(reader);
            color.Read(reader);
            range = reader.ReadFloat();
            lightType = (RndLight.Type)reader.ReadInt32();
        }

        public void Write(EndianWriter writer)
        {
            rotationQuat.Write(writer);
            position.Write(writer);
            color.Write(writer);
            writer.WriteFloat(range);
            writer.WriteInt32((int)lightType);
        }
    }

    public class SpotlightEntry
    {
        [Name("Intensity"), Description("Intensity")]
        public float intensity;
        [Name("Rotation"), Description("Light transform")]
        public Vector4 rotationQuat = new();
        [Name("Color"), Description("Color")]
        public HmxColor4 color = new();
        [Name("Target"), Description("Target")]
        public Symbol target = Symbol.Empty;
        [Name("Subpart"), Description("Sub-part of the object the spotlight follows"), MaxVersion(0x12)]
        public Symbol oldSymbol = Symbol.Empty;
        [Name("Flare Enabled"), Description("Whether the flare is enabled")]
        public bool flareEnabled;
        [MaxVersion(8)]
        public int oldInt;

        [Name("Bottom Radius")]
        public float bottomRadius;

        public void Read(EndianReader reader, ushort presetRevision, bool greenDayLayout = false)
        {
            intensity = reader.ReadFloat();
            rotationQuat = new Vector4().Read(reader);
            color.Read(reader);
            target = Symbol.Read(reader);

            if (presetRevision < 0x13)
            {
                oldSymbol = Symbol.Read(reader);
            }

            if (presetRevision > 1)
            {
                flareEnabled = reader.ReadBoolean();
                if (presetRevision < 9)
                {
                    oldInt = reader.ReadInt32();
                }
            }
            if (greenDayLayout)
            {
                bottomRadius = reader.ReadFloat();
            }
        }

        public void Write(EndianWriter writer, ushort presetRevision, bool greenDayLayout = false)
        {
            writer.WriteFloat(intensity);
            rotationQuat.Write(writer);
            color.Write(writer);
            Symbol.Write(writer, target);

            if (presetRevision < 0x13)
            {
                Symbol.Write(writer, oldSymbol);
            }

            if (presetRevision > 1)
            {
                writer.WriteBoolean(flareEnabled);
                if (presetRevision < 9)
                {
                    writer.WriteInt32(oldInt);
                }
            }
            if (greenDayLayout)
            {
                writer.WriteFloat(bottomRadius);
            }
        }
    }

    public class SpotlightDrawerEntry
    {
        [Name("Base Intensity"), Description("Intensity of smokeless beam")]
        public float baseIntensity;
        [Name("Smoke Intensity"), Description("Intensity from smoke")]
        public float smokeIntensity;
        [Name("Total"), Description("Global intensity scale")]
        public float totalIntensity;
        [Name("Light Influence"), Description("The amount the spotlights will influence the real lighting of the world")]
        public float lightInfluence;

        public void Read(EndianReader reader, ushort presetRevision, bool greenDayLayout = false)
        {
            baseIntensity = reader.ReadFloat();
            smokeIntensity = reader.ReadFloat();
            totalIntensity = reader.ReadFloat();
            lightInfluence = 1.0f;
            if (presetRevision > 0xF && !greenDayLayout)
            {
                lightInfluence = reader.ReadFloat();
            }
        }

        public void Write(EndianWriter writer, ushort presetRevision, bool greenDayLayout = false)
        {
            writer.WriteFloat(baseIntensity);
            writer.WriteFloat(smokeIntensity);
            writer.WriteFloat(totalIntensity);
            if (presetRevision > 0xF && !greenDayLayout)
            {
                writer.WriteFloat(lightInfluence);
            }
        }
    }

    public class Keyframe
    {
        [Name("Description"), Description("Description of the keyframe")]
        public string description = string.Empty;
        [Name("Duration"), Description("Duration of the keyframe")]
        public float duration;
        [Name("Fade Out"), Description("Fade-out time of the keyframe")]
        public float fadeOutTime;
        [Name("Spotlight Entries")]
        public List<SpotlightEntry> spotlightEntries = new();
        [Name("Environment Entries")]
        public List<EnvironmentEntry> environmentEntries = new();
        [Name("Light Entries")]
        public List<EnvLightEntry> lightEntries = new();
        [Name("Spotlight Drawer Entries")]
        public List<SpotlightDrawerEntry> spotlightDrawerEntries = new();
        [Name("Video Venue Postproc"), Description("postproc used in video venues")]
        public Symbol videoVenuePostProc = Symbol.Empty;
        [Name("Triggers"), Description("Triggers to fire")]
        public List<Symbol> triggers = new();
        [Name("LED"), MinVersion(0xC), MaxVersion(0x15)]
        public int[] stageKit = Array.Empty<int>();

        public void Read(EndianReader reader, ushort presetRevision, bool greenDayLayout = false)
        {
            if (presetRevision == 0xE)
            {
                description = ReadString(reader);
                spotlightEntries = AssetHelpers.ReadList(reader, () => { SpotlightEntry entry = new(); entry.Read(reader, presetRevision); return entry; });
                environmentEntries = AssetHelpers.ReadList(reader, () => { EnvironmentEntry entry = new(); entry.Read(reader); return entry; });
                lightEntries = AssetHelpers.ReadList(reader, () => { EnvLightEntry entry = new(); entry.Read(reader); return entry; });
                spotlightDrawerEntries = AssetHelpers.ReadList(reader, () => { SpotlightDrawerEntry entry = new(); entry.Read(reader, presetRevision); return entry; });
                stageKit = ReadStageKit(reader);
                return;
            }

            duration = reader.ReadFloat();
            fadeOutTime = reader.ReadFloat();
            spotlightEntries = AssetHelpers.ReadList(reader, () => { SpotlightEntry entry = new(); entry.Read(reader, presetRevision, greenDayLayout); return entry; });
            environmentEntries = AssetHelpers.ReadList(reader, () => { EnvironmentEntry entry = new(); entry.Read(reader); return entry; });
            lightEntries = AssetHelpers.ReadList(reader, () => { EnvLightEntry entry = new(); entry.Read(reader); return entry; });

            if (presetRevision > 5)
            {
                description = ReadString(reader);
            }

            if (presetRevision > 9)
            {
                spotlightDrawerEntries = AssetHelpers.ReadList(reader, () => { SpotlightDrawerEntry entry = new(); entry.Read(reader, presetRevision, greenDayLayout); return entry; });
            }

            if (presetRevision > 0x11 && presetRevision < 0x16 && !greenDayLayout)
            {
                videoVenuePostProc = Symbol.Read(reader);
            }

            if (presetRevision > 0x13)
            {
                triggers = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }

            if (presetRevision > 0xB && presetRevision < 0x16)
            {
                stageKit = ReadStageKit(reader);
            }
        }

        public void Write(EndianWriter writer, ushort presetRevision, bool greenDayLayout = false)
        {
            if (presetRevision == 0xE)
            {
                WriteString(writer, description);
                AssetHelpers.WriteList(writer, spotlightEntries, e => e.Write(writer, presetRevision));
                AssetHelpers.WriteList(writer, environmentEntries, e => e.Write(writer));
                AssetHelpers.WriteList(writer, lightEntries, e => e.Write(writer));
                AssetHelpers.WriteList(writer, spotlightDrawerEntries, e => e.Write(writer, presetRevision));
                WriteStageKit(writer, stageKit);
                return;
            }

            writer.WriteFloat(duration);
            writer.WriteFloat(fadeOutTime);
            AssetHelpers.WriteList(writer, spotlightEntries, e => e.Write(writer, presetRevision, greenDayLayout));
            AssetHelpers.WriteList(writer, environmentEntries, e => e.Write(writer));
            AssetHelpers.WriteList(writer, lightEntries, e => e.Write(writer));

            if (presetRevision > 5)
            {
                WriteString(writer, description);
            }

            if (presetRevision > 9)
            {
                AssetHelpers.WriteList(writer, spotlightDrawerEntries, e => e.Write(writer, presetRevision, greenDayLayout));
            }

            if (presetRevision > 0x11 && presetRevision < 0x16 && !greenDayLayout)
            {
                Symbol.Write(writer, videoVenuePostProc);
            }

            if (presetRevision > 0x13)
            {
                AssetHelpers.WriteList(writer, triggers, item => Symbol.Write(writer, item));
            }

            if (presetRevision > 0xB && presetRevision < 0x16)
            {
                WriteStageKit(writer, stageKit);
            }
        }
    }

    private ushort altRevision;
    private ushort revision;

    private bool UsesGreenDayLayout(DirectoryMeta parent)
    {
        return parent.revision == 25 && revision == 18;
    }

    [Name("Group Of Lights")]
    public Symbol groupOfLights = Symbol.Empty;
    [Name("Property Flags")]
    public uint propertyFlags;

    public RndAnimatable anim = new();

    [Name("Keyframes")]
    public List<Keyframe> keyframes = new();
    [HideInInspector]
    public bool hasKeyframeVectorHeader;
    [HideInInspector]
    public uint keyframeVectorHeader;
    public List<Symbol> spotlights = new();
    public List<Symbol> environments = new();
    public List<Symbol> lights = new();
    [Name("Looping"), Description("Whether this preset loops its animation")]
    public bool looping;
    [Name("Category"), Description("Category for preset-picking")]
    public Symbol category = Symbol.Empty;
    [MaxVersion(0x10)]
    public List<Symbol> oldCategories = new();
    [MaxVersion(6)]
    public string oldWarning = string.Empty;
    [Name("Select Triggers"), Description("Triggers to fire upon selection")]
    public List<Symbol> selectTriggers = new();
    [MaxVersion(4)]
    public bool oldHasExtraKeyframe;
    [MaxVersion(4)]
    public Keyframe? oldExtraKeyframe;
    [MaxVersion(4)]
    public string oldString = string.Empty;
    [Name("Legacy Fade In"), Description("How long this preset should fade in from the previous one")]
    public float oldFadeIn;
    [Name("Minimum Excitement"), Description("Minimum excitement level for this preset.")]
    public int oldDummy1;
    [Name("Maximum Excitement"), Description("Maximum excitement level for this preset."), MinVersion(3)]
    public int oldDummy2;
    [Name("Manual"), Description("Whether this is a manual keyframe (keyframes controlled by MIDI)")]
    public bool manual;
    [Name("Locked"), Description("Whether the keyframes are locked (no editing allowed)")]
    public bool locked;
    [Name("Platform Only"), Description("Limit this shot to given platform")]
    public int platformOnly;
    public List<Symbol> spotlightDrawers = new();
    [MaxVersion(0xB), MinVersion(0xB)]
    public int[] oldRev11Dummies = Array.Empty<int>();

    public new LightPreset Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision != 0xE)
        {
            anim.Read(reader, parent, entry);
            int keyframeCount = reader.ReadInt32();
            hasKeyframeVectorHeader = keyframeCount < 0;
            if (hasKeyframeVectorHeader)
            {
                keyframeCount = checked(-keyframeCount);
                keyframeVectorHeader = reader.ReadUInt32();
            }
            keyframes = new List<Keyframe>(keyframeCount);
            for (int i = 0; i < keyframeCount; i++)
            {
                Keyframe keyframe = new();
                keyframe.Read(reader, revision, UsesGreenDayLayout(parent));
                keyframes.Add(keyframe);
            }
        }
        else
        {
            Keyframe keyframe = new();
            keyframe.Read(reader, revision);
            keyframes.Add(keyframe);
        }

        spotlights = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        environments = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        lights = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

        if (revision < 5)
        {
            oldHasExtraKeyframe = reader.ReadBoolean();
            if (oldHasExtraKeyframe)
            {
                oldExtraKeyframe = new Keyframe();
                oldExtraKeyframe.Read(reader, revision);
            }
        }

        if (revision != 0xE)
        {
            looping = reader.ReadBoolean();
        }

        category = Symbol.Read(reader);

        if (revision != 0xE && (revision < 0x11 || UsesGreenDayLayout(parent)))
        {
            oldCategories = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision < 7)
        {
            oldWarning = ReadString(reader);
        }
        else if (revision < 0x15)
        {
            Symbol trigger = Symbol.Read(reader);
            if (!string.IsNullOrEmpty(trigger.value))
            {
                selectTriggers.Add(trigger);
            }
        }
        else
        {
            selectTriggers = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision < 5)
        {
            oldString = ReadString(reader);
        }

        if (revision != 0xE)
        {
            if (revision < 22)
            {
                oldFadeIn = reader.ReadFloat();
            }

            if (revision != 0 && (revision < 0x11 || UsesGreenDayLayout(parent)))
            {
                oldDummy1 = reader.ReadInt32();
            }

            if ((revision >= 3 && revision <= 16) || UsesGreenDayLayout(parent))
            {
                oldDummy2 = reader.ReadInt32();
            }
        }

        if (revision > 3)
        {
            if (revision != 0xE)
            {
                manual = reader.ReadBoolean();
            }

            locked = reader.ReadBoolean();
        }

        if (revision > 0xC)
        {
            platformOnly = reader.ReadInt32();
        }

        if (revision > 9)
        {
            spotlightDrawers = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (UsesGreenDayLayout(parent))
        {
            groupOfLights = Symbol.Read(reader);
            propertyFlags = reader.ReadUInt32();
        }

        if (revision == 0xB)
        {
            oldRev11Dummies = new int[8];
            for (int i = 0; i < oldRev11Dummies.Length; i++)
            {
                oldRev11Dummies[i] = reader.ReadInt32();
            }
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

        if (revision != 0xE)
        {
            anim.Write(writer);
            if (hasKeyframeVectorHeader && keyframes.Count > 0)
            {
                writer.WriteInt32(-keyframes.Count);
                writer.WriteUInt32(keyframeVectorHeader);
            }
            else
            {
                writer.WriteInt32(keyframes.Count);
            }
            foreach (Keyframe keyframe in keyframes)
            {
                keyframe.Write(writer, revision, UsesGreenDayLayout(parent));
            }
        }
        else
        {
            (keyframes.Count > 0 ? keyframes[0] : new Keyframe()).Write(writer, revision);
        }

        AssetHelpers.WriteList(writer, spotlights, item => Symbol.Write(writer, item));
        AssetHelpers.WriteList(writer, environments, item => Symbol.Write(writer, item));
        AssetHelpers.WriteList(writer, lights, item => Symbol.Write(writer, item));

        if (revision < 5)
        {
            writer.WriteBoolean(oldHasExtraKeyframe);
            if (oldHasExtraKeyframe)
            {
                (oldExtraKeyframe ?? new Keyframe()).Write(writer, revision);
            }
        }

        if (revision != 0xE)
        {
            writer.WriteBoolean(looping);
        }

        Symbol.Write(writer, category);

        if (revision != 0xE && (revision < 0x11 || UsesGreenDayLayout(parent)))
        {
            AssetHelpers.WriteList(writer, oldCategories, item => Symbol.Write(writer, item));
        }

        if (revision < 7)
        {
            WriteString(writer, oldWarning);
        }
        else if (revision < 0x15)
        {
            Symbol.Write(writer, selectTriggers.Count > 0 ? selectTriggers[0] : Symbol.Empty);
        }
        else
        {
            AssetHelpers.WriteList(writer, selectTriggers, item => Symbol.Write(writer, item));
        }

        if (revision < 5)
        {
            WriteString(writer, oldString);
        }

        if (revision != 0xE)
        {
            if (revision < 22)
            {
                writer.WriteFloat(oldFadeIn);
            }

            if (revision != 0 && (revision < 0x11 || UsesGreenDayLayout(parent)))
            {
                writer.WriteInt32(oldDummy1);
            }

            if ((revision >= 3 && revision <= 16) || UsesGreenDayLayout(parent))
            {
                writer.WriteInt32(oldDummy2);
            }
        }

        if (revision > 3)
        {
            if (revision != 0xE)
            {
                writer.WriteBoolean(manual);
            }

            writer.WriteBoolean(locked);
        }

        if (revision > 0xC)
        {
            writer.WriteInt32(platformOnly);
        }

        if (revision > 9)
        {
            AssetHelpers.WriteList(writer, spotlightDrawers, item => Symbol.Write(writer, item));
        }

        if (UsesGreenDayLayout(parent))
        {
            Symbol.Write(writer, groupOfLights);
            writer.WriteUInt32(propertyFlags);
        }

        if (revision == 0xB)
        {
            int[] dummies = oldRev11Dummies.Length == 8 ? oldRev11Dummies : new int[8];
            foreach (int dummy in dummies)
            {
                writer.WriteInt32(dummy);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private static string ReadString(EndianReader reader)
    {
        return Symbol.Read(reader).value;
    }

    private static void WriteString(EndianWriter writer, string value)
    {
        Symbol.Write(writer, value ?? string.Empty);
    }

    private static int[] ReadStageKit(EndianReader reader)
    {
        int[] values = new int[9];
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = reader.ReadInt32();
        }

        return values;
    }

    private static void WriteStageKit(EndianWriter writer, int[] values)
    {
        int[] stageKit = values.Length == 9 ? values : new int[9];
        foreach (int value in stageKit)
        {
            writer.WriteInt32(value);
        }
    }
}
