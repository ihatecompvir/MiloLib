using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("ParticleSys"), Description("A ParticleSys object generates, animates, and draws large numbers of similar sprites. Currently particles are rendered only as points on the PC.")]
public class RndParticleSys : Object
{
    public class PreservedParticle
    {
        public float unk1;
        public float unk2;
        public float unk3;
        public float unk4;
        public float unk5;
        public float unk6;
        public float unk7;
        public float unk8;
        public float unk9;

        public PreservedParticle Read(EndianReader reader, bool hasNinthValue)
        {
            unk1 = reader.ReadFloat();
            unk2 = reader.ReadFloat();
            unk3 = reader.ReadFloat();
            unk4 = reader.ReadFloat();
            unk5 = reader.ReadFloat();
            unk6 = reader.ReadFloat();
            unk7 = reader.ReadFloat();
            unk8 = reader.ReadFloat();
            if (hasNinthValue)
            {
                unk9 = reader.ReadFloat();
            }
            return this;
        }

        public void Write(EndianWriter writer, bool hasNinthValue)
        {
            writer.WriteFloat(unk1);
            writer.WriteFloat(unk2);
            writer.WriteFloat(unk3);
            writer.WriteFloat(unk4);
            writer.WriteFloat(unk5);
            writer.WriteFloat(unk6);
            writer.WriteFloat(unk7);
            writer.WriteFloat(unk8);
            if (hasNinthValue)
            {
                writer.WriteFloat(unk9);
            }
        }
    }
    private ushort altRevision;
    private ushort revision;

    public class Attractor
    {
        [Name("Attractor"), Description("Attracts/repulses particles")]
        public Symbol target = Symbol.Empty;
        [Name("Strength"), Description("Positive value means attract particles, negative means repulse")]
        public float strength = 1.0f;
    }

    [Name("Animate UVs"), Description("uses material texture as page tiles to animated through"), MinVersion(39)]
    public bool animateUVs;
    [Name("Tile Hold Time"), Description("speed in seconds it takes to switch to next tile"), MinVersion(39)]
    public float tileHoldTime;
    [Name("Num Tiles Across"), Description("number of tiles across in specified texture"), MinVersion(39)]
    public int numTilesAcross;
    [Name("Num Tiles Down"), Description("number of tiles down in specified texture"), MinVersion(39)]
    public int numTilesDown;
    [Name("Num Tiles Total"), Description("total number of tiles used for animation"), MinVersion(39)]
    public int numTilesTotal;
    [Name("Starting Tile"), Description("starting frame of animation"), MinVersion(39)]
    public int startingTile;
    [Name("Loop UV Anim"), Description("animation loops to beginning if true"), MinVersion(39)]
    public bool loopUVAnim;
    [Name("Random Anim Start"), Description("particle animation starts at random location"), MinVersion(39)]
    public bool randomAnimStart;
    [Name("Attractors"), Description("Add point forces which attract or repel particles"), MinVersion(40)]
    public List<Attractor> attractors = new();
    [Name("Birth Momentum"), Description("Add relative parent's momentum to each particle's initial speed. Fancy property must be true."), MinVersion(41)]
    public bool birthMomentum;
    [Name("Birth Momentum Amount"), Description("Amount of birth momentum to use. 0 is none, 1 is full amount, 2 is twice the momentum, -1 means use the opposite of the momentum relative to the motion parent. Fancy property must be true."), MinVersion(41)]
    public float birthMomentumAmount;

    [MinVersion(23)]
    public ObjectFields obj1 = new();
    [MinVersion(23)]
    public ObjectFields obj2 = new();

    public RndAnimatable anim = new();
    public RndTrans trans = new();
    public RndDrawable draw = new();

    [Name("Life"), Description("Frame range of particle life.")]
    public Vector2 life = new();

    [Name("Height Ratio"), Description("Ratio of screen height to width"), MinVersion(36)]
    public float heightRatio;

    [Name("Position Low"), Description("Min point and max point, in object coordinates, of box region that particles are emitted from.")]
    public Vector3 posLow = new();
    [Name("Position High"), Description("Min point and max point, in object coordinates, of box region that particles are emitted from.")]
    public Vector3 posHigh = new();

    [Name("Speed"), Description("Speed range, in world units per frame, of particles.")]
    public Vector2 speed = new();
    [Name("Pitch"), Description("Rotation range around the X axis")]
    public Vector2 pitch = new();
    [Name("Yaw"), Description("Rotation range around the Y axis")]
    public Vector2 yaw = new();
    [Name("Emit Rate"), Description("Frame range to generate particles.")]
    public Vector2 emitRate = new();

    [Name("Max Bursts"), Description("Maximum number of bursts"), MinVersion(33)]
    public uint maxBursts;
    [Name("Burst Interval"), Description("Time between bursts"), MinVersion(33)]
    public Vector2 burstInterval = new();
    [Name("Burst Peak"), Description("Peak rate of particles during a burst"), MinVersion(33)]
    public Vector2 burstPeak = new();
    [Name("Burst Length"), Description("Duration of the burst."), MinVersion(33)]
    public Vector2 burstLength = new();

    [Name("Start Size"), Description("Size range, in world units, of particles.")]
    public Vector2 startSize = new();

    [Name("Delta Size"), Description("Change in size of particles, in world units."), MinVersion(16)]
    public Vector2 deltaSize = new();

    [Name("Start Color Low"), Description("Random color ranges for start and end color of particles.")]
    public HmxColor4 startColorLow = new();
    [Name("Start Color High"), Description("Random color ranges for start and end color of particles.")]
    public HmxColor4 startColorHigh = new();

    [Name("End Color Low"), Description("Random color ranges for start and end color of particles.")]
    public HmxColor4 endColorLow = new();
    [Name("End Color High"), Description("Random color ranges for start and end color of particles.")]
    public HmxColor4 endColorHigh = new();

    [Name("Bounce"), Description("Specify a collide plane to reflect particles. Used to bounce particles off surfaces.")]
    public Symbol bounce = Symbol.Empty;

    public bool useBounce;

    [Name("Force Direction"), Description("Force direction in world coordinates, in units per frame added to each particle's velocity. Can be used for gravity.")]
    public Vector3 force = new();

    [Name("Material"), Description("material for particle system")]
    public Symbol material = Symbol.Empty;

    [MaxVersion(12)]
    public uint unkInt;
    [MinVersion(18)]
    public uint unkInt2;

    [Name("Grow Ratio"), Description("Controls the rate that the particle system grows."), MinVersion(18)]
    public float growRatio;
    [Name("Shrink Ratio"), Description("Controls the rate that the particle system shrinks."), MinVersion(18)]
    public float shrinkRatio;
    [Name("Mid Color Ratio"), Description("Controls the midpoint of the color transition."), MinVersion(18)]
    public float midColorRatio;

    [Name("Mid Color Low"), Description("The low color value when the particle reaches the mid point."), MinVersion(18)]
    public HmxColor4 midColorLow = new();
    [Name("Mid Color High"), Description("The high color value when the particle reaches the mid point."), MinVersion(18)]
    public HmxColor4 midColorHigh = new();

    [Name("Max Particles"), Description("maximum number of particles")]
    public uint maxParticles;

    [MinVersion(3), MaxVersion(6)]
    public uint unkInt3;
    [MinVersion(7), MaxVersion(12)]
    public uint unkInt4;
    public bool oldDepthMode;
    public Symbol oldParticleString = Symbol.Empty;

    [Name("Bubble Period"), Description("Controls the period of bubble effect"), MinVersion(4)]
    public Vector2 bubblePeriod = new();
    [Name("Bubble Size"), Description("Controls the size of the bubble effect"), MinVersion(4)]
    public Vector2 bubbleSize = new();
    [Name("Bubble"), Description("Enable/disable bubble effect"), MinVersion(4)]
    public bool bubble;

    [Name("Rotate"), Description("Enable/disable particle rotation"), MinVersion(30)]
    public bool rotate;

    [Name("Rotation Speed"), Description("Rotation speed of the particles"), MinVersion(30)]
    public Vector2 rotSpeed = new();
    [Name("Rotation Drag"), Description("Drag of the particle rotation"), MinVersion(30)]
    public float rotDrag;
    [Name("Rotation Random Direction"), Description("Use random rotation direction"), MinVersion(37)]
    public bool rotRandomDirection;

    [Name("Drag"), Description("Drag of the particles"), MinVersion(30)]
    public uint drag;

    [Name("Swing Arm Start"), Description("Start offset of swing arm"), MinVersion(32)]
    public Vector2 swingArmStart = new();
    [Name("Swing Arm End"), Description("End offset of swing arm"), MinVersion(32)]
    public Vector2 swingArmEnd = new();

    [Name("Align with Velocity"), Description("Align particle direction with its velocity"), MinVersion(32)]
    public bool alignWithVelocity;
    [Name("Stretch with Velocity"), Description("Stretch the particle along its velocity"), MinVersion(32)]
    public bool stretchWithVelocity;
    [Name("Constant Area"), Description("Maintain constant particle area while scaling"), MinVersion(32)]
    public bool constantArea;

    [Name("Stretch Scale"), Description("Amount to stretch the particle"), MinVersion(32)]
    public float stretchScale;

    [Name("Perspective Stretch"), Description("Enable/disable perspective stretch"), MinVersion(34)]
    public bool perspectiveStretch;

    [Name("Relative Motion"), Description("If [motion_parent] is set, is amount to make movement relative to it, 0-1, 1 being fully relative to the parent")]
    public float relativeMotion;

    [Name("Relative Parent"), Description("Makes particles move relative to this Trans"), MinVersion(27)]
    public Symbol relativeParent = Symbol.Empty;

    [Name("Mesh"), Description("The emitter mesh of the particle system"), MinVersion(19)]
    public Symbol mesh = Symbol.Empty;

    // this one is weird and not supported by the versioning system (revision over 30 or exactly 21, which doesn't work with Min and Max version) - just show it at all times
    [Name("Sub Samples"), Description("Subsamples of the emitter")]
    public uint subSamples;

    [Name("Frame Drive"), Description("Enable/disable frame driven animation"), MinVersion(28)]
    public bool frameDrive;

    [Name("Pause Offscreen"), Description("Freezes the particle motion when they are offscreen, CPU savings"), MinVersion(35)]
    public bool pauseOffscreen;

    [Name("Fast Forward"), Description("Spawns particles on enter, so we start out with particles there"), MinVersion(29)]
    public bool fastForward;

    [Name("Preserve Particles"), Description("Enable/disable particle perservation"), MinVersion(11)]
    public bool preserveParticles;

    private List<PreservedParticle> preservedParticles = new();

    public Plane plane;

    public List<Plane> planes = new();

    public Vector3 unkVector1 = new();
    public Vector3 unkVector2 = new();

    private bool HasNinthPreservedParticleValue(DirectoryMeta parent)
    {
        if (parent.platform == DirectoryMeta.Platform.PS2 && revision < 36)
        {
            return false;
        }

        if (parent.revision == 25 && revision == 30)
        {
            return false;
        }

        if (parent.revision == 10 && (revision == 12 || revision == 22))
        {
            return false;
        }

        return true;
    }

    public new RndParticleSys Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        // only read these if revision is greater than 22
        if (revision > 22)
        {
            obj1.Read(reader, parent, entry);
            if (revision > 27)
            {
                obj2.Read(reader, parent, entry);
            }
        }

        anim.Read(reader, parent, entry);
        trans.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);

        life.Read(reader);

        if (35 < revision)
        {
            heightRatio = reader.ReadFloat();
        }

        posLow.Read(reader);
        posHigh.Read(reader);

        speed.Read(reader);
        pitch.Read(reader);
        yaw.Read(reader);
        emitRate.Read(reader);

        if (32 < revision)
        {
            maxBursts = reader.ReadUInt32();
            burstInterval.Read(reader);
            burstPeak.Read(reader);
            burstLength.Read(reader);
        }

        startSize.Read(reader);

        if (15 < revision)
        {
            deltaSize.Read(reader);
        }

        startColorLow.Read(reader);
        startColorHigh.Read(reader);

        endColorLow.Read(reader);
        endColorHigh.Read(reader);

        if (revision > 25)
        {
            bounce = Symbol.Read(reader);
        }
        else if (revision > 1)
        {
            useBounce = reader.ReadBoolean();

            if (revision > 11)
            {
                plane = plane.Read(reader);
            }
            else
            {
                unkVector1.Read(reader);
                unkVector2.Read(reader);
            }
        }
        else
        {
            planes = AssetHelpers.ReadList(reader, () => new Plane().Read(reader));
        }

        force.Read(reader);

        material = Symbol.Read(reader);

        if (revision < 18)
        {
            if (revision < 13)
            {
                unkInt = reader.ReadUInt32();
            }
        }
        else
        {
            unkInt2 = reader.ReadUInt32();
            growRatio = reader.ReadFloat();
            shrinkRatio = reader.ReadFloat();

            midColorRatio = reader.ReadFloat();

            midColorLow.Read(reader);
            midColorHigh.Read(reader);
        }

        maxParticles = reader.ReadUInt32();

        if (2 < revision)
        {
            if (revision < 7)
            {
                unkInt3 = reader.ReadUInt32();
            }
            else if (revision < 13)
            {
                unkInt4 = reader.ReadUInt32();
            }
        }

        if (3 < revision)
        {
            bubblePeriod.Read(reader);
            bubbleSize.Read(reader);
            bubble = reader.ReadBoolean();
        }

        if (29 < revision)
        {
            rotate = reader.ReadBoolean();
            rotSpeed.Read(reader);
            rotDrag = reader.ReadFloat();
            if (36 < revision)
            {
                rotRandomDirection = reader.ReadBoolean();
            }

            drag = reader.ReadUInt32();
        }

        if (31 < revision)
        {
            swingArmStart.Read(reader);
            swingArmEnd.Read(reader);

            alignWithVelocity = reader.ReadBoolean();
            stretchWithVelocity = reader.ReadBoolean();
            constantArea = reader.ReadBoolean();

            stretchScale = reader.ReadFloat();
        }

        if (33 < revision)
        {
            perspectiveStretch = reader.ReadBoolean();
        }

        if (parent.revision == 10 && revision == 12)
        {
            oldDepthMode = reader.ReadBoolean();
            oldParticleString = Symbol.Read(reader);
        }

        if (revision > 19)
        {
            relativeMotion = reader.ReadFloat();
        }
        else if (revision > 12)
        {
            relativeMotion = reader.ReadBoolean() ? 1.0f : 0.0f;
        }

        if (26 < revision)
        {
            relativeParent = Symbol.Read(reader);
        }

        if (18 < revision)
        {
            mesh = Symbol.Read(reader);
        }

        if (30 < revision || revision == 21)
        {
            subSamples = reader.ReadUInt32();
        }

        if (!(revision < 28))
        {
            frameDrive = reader.ReadBoolean();
        }

        if (!(revision < 35))
        {
            pauseOffscreen = reader.ReadBoolean();
        }

        if (!(revision < 29))
        {
            fastForward = reader.ReadBoolean();
        }

        if (revision >= 39)
        {
            animateUVs = reader.ReadBoolean();
            tileHoldTime = reader.ReadFloat();
            numTilesAcross = reader.ReadInt32();
            numTilesDown = reader.ReadInt32();
            numTilesTotal = reader.ReadInt32();
            startingTile = reader.ReadInt32();
            loopUVAnim = reader.ReadBoolean();
            randomAnimStart = reader.ReadBoolean();
        }
        if (revision >= 40)
        {
            attractors = AssetHelpers.ReadList(reader, () => new Attractor { target = Symbol.Read(reader), strength = reader.ReadFloat() });
        }
        if (revision >= 41)
        {
            birthMomentum = reader.ReadBoolean();
            birthMomentumAmount = reader.ReadFloat();
        }

        if (10 < revision)
        {
            preserveParticles = reader.ReadBoolean();
            if (preserveParticles)
            {
                bool hasNinthValue = HasNinthPreservedParticleValue(parent);
                preservedParticles = AssetHelpers.ReadList(reader, () => new PreservedParticle().Read(reader, hasNinthValue));
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

        // Match Read: only write these if revision is greater than 22
        if (revision > 22)
        {
            obj1.Write(writer, parent);
            if (revision > 27)
            {
                obj2.Write(writer, parent);
            }
        }

        anim.Write(writer);
        trans.Write(writer, false, parent, null);
        draw.Write(writer, false, parent, null);

        life.Write(writer);

        if (35 < revision)
        {
            writer.WriteFloat(heightRatio);
        }

        posLow.Write(writer);
        posHigh.Write(writer);

        speed.Write(writer);
        pitch.Write(writer);
        yaw.Write(writer);
        emitRate.Write(writer);

        if (32 < revision)
        {
            writer.WriteUInt32(maxBursts);
            burstInterval.Write(writer);
            burstPeak.Write(writer);
            burstLength.Write(writer);
        }

        startSize.Write(writer);

        if (15 < revision)
        {
            deltaSize.Write(writer);
        }

        startColorLow.Write(writer);
        startColorHigh.Write(writer);

        endColorLow.Write(writer);
        endColorHigh.Write(writer);

        if (revision > 25)
        {
            Symbol.Write(writer, bounce);
        }
        else if (revision > 1)
        {
            writer.WriteBoolean(useBounce);
            if (revision > 11)
            {
                plane.Write(writer);
            }
            else
            {
                unkVector1.Write(writer);
                unkVector2.Write(writer);
            }
        }
        else
        {
            AssetHelpers.WriteList(writer, planes, plane => plane.Write(writer));
        }

        force.Write(writer);

        Symbol.Write(writer, material);

        if (revision < 18)
        {
            if (revision < 13)
            {
                writer.WriteUInt32(unkInt);
            }
        }
        else
        {
            writer.WriteUInt32(unkInt2);
            writer.WriteFloat(growRatio);
            writer.WriteFloat(shrinkRatio);

            writer.WriteFloat(midColorRatio);

            midColorLow.Write(writer);
            midColorHigh.Write(writer);
        }

        writer.WriteUInt32(maxParticles);

        if (2 < revision)
        {
            if (revision < 7)
            {
                writer.WriteUInt32(unkInt3);
            }
            else if (revision < 13)
            {
                writer.WriteUInt32(unkInt4);
            }
        }

        if (3 < revision)
        {
            bubblePeriod.Write(writer);
            bubbleSize.Write(writer);
            writer.WriteBoolean(bubble);
        }

        if (29 < revision)
        {
            writer.WriteBoolean(rotate);
            rotSpeed.Write(writer);
            writer.WriteFloat(rotDrag);
            if (36 < revision)
            {
                writer.WriteBoolean(rotRandomDirection);
            }

            writer.WriteUInt32(drag);
        }

        if (31 < revision)
        {
            swingArmStart.Write(writer);
            swingArmEnd.Write(writer);

            writer.WriteBoolean(alignWithVelocity);
            writer.WriteBoolean(stretchWithVelocity);
            writer.WriteBoolean(constantArea);

            writer.WriteFloat(stretchScale);
        }

        if (33 < revision)
        {
            writer.WriteBoolean(perspectiveStretch);
        }

        if (parent.revision == 10 && revision == 12)
        {
            writer.WriteBoolean(oldDepthMode);
            Symbol.Write(writer, oldParticleString);
        }

        if (revision > 19)
        {
            writer.WriteFloat(relativeMotion);
        }
        else if (revision > 12)
        {
            writer.WriteBoolean(relativeMotion != 0.0f);
        }

        if (26 < revision)
        {
            Symbol.Write(writer, relativeParent);
        }

        if (18 < revision)
        {
            Symbol.Write(writer, mesh);
        }

        if (30 < revision || revision == 21)
        {
            writer.WriteUInt32(subSamples);
        }

        if (!(revision < 28))
        {
            writer.WriteBoolean(frameDrive);
        }

        if (!(revision < 35))
        {
            writer.WriteBoolean(pauseOffscreen);
        }

        if (!(revision < 29))
        {
            writer.WriteBoolean(fastForward);
        }

        if (revision >= 39)
        {
            writer.WriteBoolean(animateUVs);
            writer.WriteFloat(tileHoldTime);
            writer.WriteInt32(numTilesAcross);
            writer.WriteInt32(numTilesDown);
            writer.WriteInt32(numTilesTotal);
            writer.WriteInt32(startingTile);
            writer.WriteBoolean(loopUVAnim);
            writer.WriteBoolean(randomAnimStart);
        }
        if (revision >= 40)
        {
            AssetHelpers.WriteList(writer, attractors, attractor => { Symbol.Write(writer, attractor.target); writer.WriteFloat(attractor.strength); });
        }
        if (revision >= 41)
        {
            writer.WriteBoolean(birthMomentum);
            writer.WriteFloat(birthMomentumAmount);
        }

        if (10 < revision)
        {
            writer.WriteBoolean(preserveParticles);
            if (preserveParticles)
            {
                bool hasNinthValue = HasNinthPreservedParticleValue(parent);
                AssetHelpers.WriteList(writer, preservedParticles, particle => particle.Write(writer, hasNinthValue));
            }
        }


        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
