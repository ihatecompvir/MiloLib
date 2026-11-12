using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("CutsceneDriver")]
public class CutsceneDriver : Object
{
    [HideInInspector]
    private ushort revision = 9;
    [HideInInspector]
    private ushort altRevision;

    public class Shadow
    {
        [Name("Target")]
        public Symbol target = Symbol.Empty;
        [Name("Self Shadow")]
        public bool selfShadow;
        [Name("Spot Cutout")]
        public bool spotCutout;
        [Name("Floor Shadow")]
        public bool floorShadow;

        public Shadow Read(EndianReader reader, ushort revision)
        {
            target = Symbol.Read(reader);
            selfShadow = reader.ReadBoolean();
            spotCutout = reader.ReadBoolean();
            floorShadow = reader.ReadBoolean();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, target);
            writer.WriteBoolean(selfShadow);
            writer.WriteBoolean(spotCutout);
            writer.WriteBoolean(floorShadow);
        }
    }

    public class CameraSetting
    {
        [Name("Near Plane"), Description("FAR TO NEAR RATIO MUST NOT EXCEED 1000, OR FAR PLANE WILL BE REDUCED")]
        public float nearPlane;
        [Name("Far Plane"), Description("FAR TO NEAR RATIO MUST NOT EXCEED 1000, OR FAR PLANE WILL BE REDUCED")]
        public float farPlane;
        [Name("Delay")]
        public float delay;

        public CameraSetting Read(EndianReader reader, ushort revision)
        {
            nearPlane = reader.ReadFloat();
            farPlane = reader.ReadFloat();
            delay = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            writer.WriteFloat(nearPlane);
            writer.WriteFloat(farPlane);
            writer.WriteFloat(delay);
        }
    }

    public class OutfitMod
    {
        [Name("Target")]
        public Symbol target = Symbol.Empty;
        [Name("Category")]
        public Symbol category = Symbol.Empty;
        [Name("Selected")]
        public Symbol selected = Symbol.Empty;
        [Name("Start Frame")]
        public float startFrame;
        [Name("End Frame"), Description("-1 indicates use the end of the CutsceneDriver")]
        public float endFrame;

        public OutfitMod Read(EndianReader reader, ushort revision)
        {
            target = Symbol.Read(reader);
            category = Symbol.Read(reader);
            selected = Symbol.Read(reader);
            startFrame = reader.ReadFloat();
            endFrame = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, target);
            Symbol.Write(writer, category);
            Symbol.Write(writer, selected);
            writer.WriteFloat(startFrame);
            writer.WriteFloat(endFrame);
        }
    }

    public class CrowdAnimation
    {
        [Name("Clip Set")]
        public Symbol clipSet = Symbol.Empty;
        [Name("Clip Group")]
        public Symbol clipGroup = Symbol.Empty;

        public CrowdAnimation Read(EndianReader reader, ushort revision)
        {
            clipSet = Symbol.Read(reader);
            clipGroup = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, clipSet);
            Symbol.Write(writer, clipGroup);
        }
    }

    public class Crowd
    {
        [Name("Crowd")]
        public Symbol crowd = Symbol.Empty;
        [Name("Crowd Rotate"), Description("How to rotate crowd")]
        public int crowdRotate;
        [Name("Anims")]
        public List<CrowdAnimation> anims = new();
        [Name("Start Frame")]
        public float startFrame;
        [Name("End Frame"), Description("-1 indicates use the end of the CutsceneDriver")]
        public float endFrame;

        public Crowd Read(EndianReader reader, ushort revision)
        {
            crowd = Symbol.Read(reader);
            crowdRotate = reader.ReadInt32();
            anims = AssetHelpers.ReadList(reader, () => new CrowdAnimation().Read(reader, revision));
            startFrame = reader.ReadFloat();
            endFrame = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, crowd);
            writer.WriteInt32(crowdRotate);
            AssetHelpers.WriteList(writer, anims, value => value.Write(writer, revision));
            writer.WriteFloat(startFrame);
            writer.WriteFloat(endFrame);
        }
    }

    public class FocalTarget
    {
        [Name("Entity"), Description("The entity to focus on")]
        public Symbol entity = Symbol.Empty;
        [Name("Subpart"), Description("The subpart of the entity to focus on")]
        public Symbol subpart = Symbol.Empty;

        public FocalTarget Read(EndianReader reader, ushort revision)
        {
            entity = Symbol.Read(reader);
            subpart = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, entity);
            Symbol.Write(writer, subpart);
        }
    }

    public class DepthOfFieldFrame
    {
        [Name("Frame")]
        public float frame;
        [Name("Enabled")]
        public bool enabled;
        [Name("Blend"), Description("Should this frame blend or step into the next frame")]
        public bool blend;
        [Name("Focal Target"), Description("The focal point when calculated depth of field")]
        public FocalTarget focalTarget = new();
        [Name("Focal Depth"), Description("The focal distance to use when no target set")]
        public float focalDepth;
        [Name("Blur Depth"), Description("Depth of the blur valley")]
        public float blurDepth;
        [Name("Max Blur"), Description("Maximum blurriness")]
        public float maxBlur;
        [Name("Min Blur"), Description("Minimum blurriness")]
        public float minBlur;
        [Name("Focus Blur Multiplier"), Description("Multiplier to control focal point distance from target")]
        public float focusBlurMultiplier;

        public DepthOfFieldFrame Read(EndianReader reader, ushort revision)
        {
            frame = reader.ReadFloat();
            enabled = reader.ReadBoolean();
            blend = reader.ReadBoolean();
            focalTarget = new FocalTarget().Read(reader, revision);
            if (revision > 8)
            {
                focalDepth = reader.ReadFloat();
            }
            blurDepth = reader.ReadFloat();
            maxBlur = reader.ReadFloat();
            minBlur = reader.ReadFloat();
            focusBlurMultiplier = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            writer.WriteFloat(frame);
            writer.WriteBoolean(enabled);
            writer.WriteBoolean(blend);
            focalTarget.Write(writer, revision);
            if (revision > 8)
            {
                writer.WriteFloat(focalDepth);
            }
            writer.WriteFloat(blurDepth);
            writer.WriteFloat(maxBlur);
            writer.WriteFloat(minBlur);
            writer.WriteFloat(focusBlurMultiplier);
        }
    }

    public class Animation
    {
        [Name("Anim")]
        public Symbol anim = Symbol.Empty;
        [Name("Delay")]
        public float delay;
        [Name("Enable")]
        public bool enable;
        [Name("Start")]
        public float start;
        [Name("End")]
        public float end;
        [Name("Scale")]
        public float scale;
        [Name("Loop")]
        public bool loop;
        [Name("Min Length")]
        public float minLength;

        public Animation Read(EndianReader reader, ushort revision)
        {
            anim = Symbol.Read(reader);
            delay = reader.ReadFloat();
            if (revision > 2)
            {
                enable = reader.ReadBoolean();
                start = reader.ReadFloat();
                end = reader.ReadFloat();
                scale = reader.ReadFloat();
                loop = reader.ReadBoolean();
                minLength = reader.ReadFloat();
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, anim);
            writer.WriteFloat(delay);
            if (revision > 2)
            {
                writer.WriteBoolean(enable);
                writer.WriteFloat(start);
                writer.WriteFloat(end);
                writer.WriteFloat(scale);
                writer.WriteBoolean(loop);
                writer.WriteFloat(minLength);
            }
        }
    }

    public class CharacterAnimation
    {
        [Name("Target")]
        public Symbol target = Symbol.Empty;
        [Name("Clip Set")]
        public Symbol clipSet = Symbol.Empty;
        [Name("Clip")]
        public Symbol clip = Symbol.Empty;
        [Name("Delay")]
        public float delay;

        public CharacterAnimation Read(EndianReader reader, ushort revision)
        {
            target = Symbol.Read(reader);
            clipSet = Symbol.Read(reader);
            clip = Symbol.Read(reader);
            delay = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, target);
            Symbol.Write(writer, clipSet);
            Symbol.Write(writer, clip);
            writer.WriteFloat(delay);
        }
    }

    public class Trigger
    {
        [Name("Proxy")]
        public Symbol proxy = Symbol.Empty;
        [Name("Trigger")]
        public Symbol trigger = Symbol.Empty;
        [Name("Delay")]
        public float delay;

        public Trigger Read(EndianReader reader, ushort revision)
        {
            if (revision > 2)
            {
                proxy = Symbol.Read(reader);
            }
            trigger = Symbol.Read(reader);
            delay = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            if (revision > 2)
            {
                Symbol.Write(writer, proxy);
            }
            Symbol.Write(writer, trigger);
            writer.WriteFloat(delay);
        }
    }

    public class DrawEvent
    {
        [Name("Target")]
        public Symbol target = Symbol.Empty;
        [Name("Delay")]
        public float delay;

        public DrawEvent Read(EndianReader reader, ushort revision)
        {
            target = Symbol.Read(reader);
            delay = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, target);
            writer.WriteFloat(delay);
        }
    }

    public class EnvironmentOverride
    {
        [Name("Target")]
        public Symbol target = Symbol.Empty;
        [Name("Env")]
        public Symbol env = Symbol.Empty;
        [Name("Delay")]
        public float delay;

        public EnvironmentOverride Read(EndianReader reader, ushort revision)
        {
            target = Symbol.Read(reader);
            env = Symbol.Read(reader);
            delay = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, target);
            Symbol.Write(writer, env);
            writer.WriteFloat(delay);
        }
    }

    public class PostProcess
    {
        [Name("Post Proc")]
        public Symbol postProc = Symbol.Empty;
        [Name("Delay")]
        public float delay;

        public PostProcess Read(EndianReader reader, ushort revision)
        {
            postProc = Symbol.Read(reader);
            delay = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, postProc);
            writer.WriteFloat(delay);
        }
    }

    public class Section
    {
        [Name("Override Framerate")]
        public bool overrideFramerate;
        [Name("New Framerate")]
        public float newFramerate;
        [Name("First Shot")]
        public Symbol firstShot = Symbol.Empty;
        [Name("Anims")]
        public List<Animation> anims = new();
        [Name("Char Anims")]
        public List<CharacterAnimation> charAnims = new();
        [Name("Triggers")]
        public List<Trigger> triggers = new();
        [Name("Shows")]
        public List<DrawEvent> shows = new();
        [Name("Hides")]
        public List<DrawEvent> hides = new();
        [Name("Env Overrides")]
        public List<EnvironmentOverride> envOverrides = new();
        [Name("Post Proc")]
        public List<PostProcess> postProc = new();

        public Section Read(EndianReader reader, ushort revision)
        {
            if (revision > 5)
            {
                overrideFramerate = reader.ReadBoolean();
                newFramerate = reader.ReadFloat();
            }
            firstShot = Symbol.Read(reader);
            anims = AssetHelpers.ReadList(reader, () => new Animation().Read(reader, revision));
            charAnims = AssetHelpers.ReadList(reader, () => new CharacterAnimation().Read(reader, revision));
            triggers = AssetHelpers.ReadList(reader, () => new Trigger().Read(reader, revision));
            shows = AssetHelpers.ReadList(reader, () => new DrawEvent().Read(reader, revision));
            hides = AssetHelpers.ReadList(reader, () => new DrawEvent().Read(reader, revision));
            if (revision > 1)
            {
                envOverrides = AssetHelpers.ReadList(reader, () => new EnvironmentOverride().Read(reader, revision));
                postProc = AssetHelpers.ReadList(reader, () => new PostProcess().Read(reader, revision));
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            if (revision > 5)
            {
                writer.WriteBoolean(overrideFramerate);
                writer.WriteFloat(newFramerate);
            }
            Symbol.Write(writer, firstShot);
            AssetHelpers.WriteList(writer, anims, value => value.Write(writer, revision));
            AssetHelpers.WriteList(writer, charAnims, value => value.Write(writer, revision));
            AssetHelpers.WriteList(writer, triggers, value => value.Write(writer, revision));
            AssetHelpers.WriteList(writer, shows, value => value.Write(writer, revision));
            AssetHelpers.WriteList(writer, hides, value => value.Write(writer, revision));
            if (revision > 1)
            {
                AssetHelpers.WriteList(writer, envOverrides, value => value.Write(writer, revision));
                AssetHelpers.WriteList(writer, postProc, value => value.Write(writer, revision));
            }
        }
    }

    [Name("Cut Frame Buffer")]
    public float cutFrameBuffer;
    [Name("Shadows")]
    public List<Shadow> shadows = new();
    [Name("Cam Settings")]
    public List<CameraSetting> camSettings = new();
    [Name("Outfit Mods")]
    public List<OutfitMod> outfitMods = new();
    [Name("Crowds")]
    public List<Crowd> crowds = new();
    [Name("DOF Frames")]
    public List<DepthOfFieldFrame> dofFrames = new();
    [Name("Sections")]
    public List<Section> sections = new();

    public new CutsceneDriver Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 9);
        base.Read(reader, false, parent, entry);

        if (revision > 3)
        {
            cutFrameBuffer = reader.ReadFloat();
        }
        if (revision > 1)
        {
            shadows = AssetHelpers.ReadList(reader, () => new Shadow().Read(reader, revision));
        }
        if (revision > 4)
        {
            camSettings = AssetHelpers.ReadList(reader, () => new CameraSetting().Read(reader, revision));
            outfitMods = AssetHelpers.ReadList(reader, () => new OutfitMod().Read(reader, revision));
        }
        if (revision > 6)
        {
            crowds = AssetHelpers.ReadList(reader, () => new Crowd().Read(reader, revision));
        }
        if (revision > 7)
        {
            dofFrames = AssetHelpers.ReadList(reader, () => new DepthOfFieldFrame().Read(reader, revision));
        }
        sections = AssetHelpers.ReadList(reader, () => new Section().Read(reader, revision));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 9);
        base.Write(writer, false, parent, entry);

        if (revision > 3)
        {
            writer.WriteFloat(cutFrameBuffer);
        }
        if (revision > 1)
        {
            AssetHelpers.WriteList(writer, shadows, value => value.Write(writer, revision));
        }
        if (revision > 4)
        {
            AssetHelpers.WriteList(writer, camSettings, value => value.Write(writer, revision));
            AssetHelpers.WriteList(writer, outfitMods, value => value.Write(writer, revision));
        }
        if (revision > 6)
        {
            AssetHelpers.WriteList(writer, crowds, value => value.Write(writer, revision));
        }
        if (revision > 7)
        {
            AssetHelpers.WriteList(writer, dofFrames, value => value.Write(writer, revision));
        }
        AssetHelpers.WriteList(writer, sections, value => value.Write(writer, revision));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
