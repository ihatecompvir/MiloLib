using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("Spotlight"), Description("Represents a beam and floorspot for venue modeling")]
public class Spotlight : Object
{
    public enum BeamShape
    {
        kBeamConic,
        kBeamRect,
        kBeamSheet,
        kBeamQuadXYZ,
        kBeamQuadZ
    }

    public class BeamDef
    {
        [Name("Is Cone"), Description("Whether this is a beam or a cone")]
        public bool isCone;
        [Name("Length"), Description("Length of the beam/cone")]
        public float length;
        [Name("Bottom Radius"), Description("Radius at the bottom of the beam/cone")]
        public float bottomRadius;
        [Name("Top Radius"), Description("Radius at the top of the beam/cone")]
        public float topRadius;
        [Name("Top Side Border"), Description("For beams, length of the side transparency border at the top of the beam")]
        public float topSideBorder;
        [Name("Bottom Side Border"), Description("For beams, length of the side transparency border at the bottom of the beam")]
        public float bottomSideBorder;
        [Name("Bottom Border"), Description("Length of the bottom transparency border")]
        public float bottomBorder;
        [Name("Material"), Description("The material to use for the beam/cone")]
        public Symbol material = Symbol.Empty;
        [MaxVersion(0x12)]
        public string oldMaterialName = string.Empty;
        [Name("Offset"), Description("Offset of beam along trajectory")]
        public float offset;
        [MaxVersion(9)]
        public HmxColor4 oldColor = new();
        [Name("Angle Offset"), Description("Amount to offset beam rotation (in degrees)")]
        public Vector2 targetOffset = new();
        [Name("Brighten"), Description("raise or lower intensity compared to og beams"), MinVersion(0x15)]
        public float brighten;
        [Name("X Section"), Description("cross section intensity override texture"), MinVersion(0x15)]
        public Symbol xSection = Symbol.Empty;
        [Name("Expand"), Description("expand or shrink the radii compared to og beams"), MinVersion(0x18)]
        public float expand;
        [Name("Shape"), Description("Shape of the beam"), MinVersion(0x1B)]
        public BeamShape shape;
        [Name("Cutouts"), Description("Objects that create cutout shadow in the beam."), MinVersion(0x19)]
        public List<Symbol> cutouts = new();
        [Name("Sections"), Description("Number of divisions along length"), MinVersion(0x20)]
        public int numSections;
        [Name("Segments"), Description("Number of divisions along width or around cone"), MinVersion(0x20)]
        public int numSegments;

        public void Read(EndianReader reader, ushort objectRevision)
        {
            isCone = reader.ReadBoolean();
            length = reader.ReadFloat();
            bottomRadius = reader.ReadFloat();
            topRadius = reader.ReadFloat();
            topSideBorder = reader.ReadFloat();
            bottomSideBorder = reader.ReadFloat();
            bottomBorder = reader.ReadFloat();
            material = Symbol.Read(reader);

            if (objectRevision == 0x12)
            {
                oldMaterialName = reader.ReadUTF8(0x80);
            }

            offset = reader.ReadFloat();

            if (objectRevision < 10)
            {
                oldColor.Read(reader);
            }

            targetOffset = new Vector2().Read(reader);

            if (objectRevision > 0x14)
            {
                brighten = reader.ReadFloat();
                xSection = Symbol.Read(reader);
            }

            if (objectRevision > 0x17)
            {
                expand = reader.ReadFloat();
            }

            if (objectRevision > 0x1A)
            {
                shape = (BeamShape)reader.ReadInt32();
            }

            if (objectRevision > 0x18)
            {
                cutouts = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }

            if (objectRevision > 0x1F)
            {
                numSections = reader.ReadInt32();
                numSegments = reader.ReadInt32();
            }
        }

        public void Write(EndianWriter writer, ushort objectRevision)
        {
            writer.WriteBoolean(isCone);
            writer.WriteFloat(length);
            writer.WriteFloat(bottomRadius);
            writer.WriteFloat(topRadius);
            writer.WriteFloat(topSideBorder);
            writer.WriteFloat(bottomSideBorder);
            writer.WriteFloat(bottomBorder);
            Symbol.Write(writer, material);

            if (objectRevision == 0x12)
            {
                writer.WriteAscii(oldMaterialName, 0x80);
            }

            writer.WriteFloat(offset);

            if (objectRevision < 10)
            {
                oldColor.Write(writer);
            }

            targetOffset.Write(writer);

            if (objectRevision > 0x14)
            {
                writer.WriteFloat(brighten);
                Symbol.Write(writer, xSection);
            }

            if (objectRevision > 0x17)
            {
                writer.WriteFloat(expand);
            }

            if (objectRevision > 0x1A)
            {
                writer.WriteInt32((int)shape);
            }

            if (objectRevision > 0x18)
            {
                AssetHelpers.WriteList(writer, cutouts, item => Symbol.Write(writer, item));
            }

            if (objectRevision > 0x1F)
            {
                writer.WriteInt32(numSections);
                writer.WriteInt32(numSegments);
            }
        }
    }

    private ushort altRevision;
    private ushort revision;

    public RndDrawable draw = new();
    public RndTrans trans = new();

    [Name("Spot Scale"), Description("Scale of the floor disc")]
    public float spotScale;
    [Name("Spot Height"), Description("Height offset of the floor disc")]
    public float spotHeight;
    [Name("Beam")]
    public BeamDef beam = new();
    [MaxVersion(0x16)]
    public List<BeamDef> oldBeams = new();
    [Name("Light Can"), Description("Optional light can mesh to use")]
    public Symbol lightCanMesh = Symbol.Empty;
    [MaxVersion(0x15)]
    public Symbol oldLightCanGroup = Symbol.Empty;
    [Name("Target"), Description("Object to target spotlight")]
    public Symbol target = Symbol.Empty;
    [Name("Spot Target"), Description("Reference object for floor height, uses spot target if not set"), MinVersion(0x1D)]
    public Symbol spotTarget = Symbol.Empty;
    [Name("Light Can Offset"), Description("Offset of light can along beam trajectory")]
    public float lightCanOffset;
    [Name("Light Can Sort"), Description("Can't optimize render end of render batching of light can with others"), MinVersion(0x1F)]
    public bool lightCanSort;
    [Name("Color"), Description("Color of the spotlight")]
    public HmxColor4 color = new();
    [Name("Intensity"), Description("Intensity of the spotlight"), MinVersion(10)]
    public float intensity;
    [Name("Spot Material"), Description("Material to use for the floor spot")]
    public Symbol spotMaterial = Symbol.Empty;
    [MaxVersion(0x12)]
    public string oldSpotMaterialName = string.Empty;
    [Name("Damping Constant"), Description("0-1, controls how fast spotlight moves to reach target")]
    public float dampingConstant;
    [Name("Target Subpart"), Description("Subpart of the spotlight target."), MaxVersion(33)]
    public Symbol oldSymbol = Symbol.Empty;
    [Name("Flare Material"), Description("Material to use for the flare"), MinVersion(11)]
    public Symbol flareMaterial = Symbol.Empty;
    [MaxVersion(0x12), MinVersion(0x12)]
    public string oldFlareMaterialName = string.Empty;
    [Name("Flare Size"), Description("Size of the flare"), MinVersion(11)]
    public Vector2 flareSize = new();
    [Name("Flare Range"), Description("Range of the flare"), MinVersion(11)]
    public Vector2 flareRange = new();
    [Name("Flare Steps"), Description("Steps for the flare"), MinVersion(11)]
    public int flareSteps;
    [Name("Flare Offset"), Description("Offset of flare along spotlight trajectory"), MinVersion(11)]
    public float flareOffset;
    [Name("Flare Enabled"), Description("Whether the flare is enabled (keyframed by light presets)"), MinVersion(0xE)]
    public bool flareEnabled;
    [Name("Flare Visibility Test"), Description("Whether the flare performs a visiblity test (or is always visible)"), MinVersion(0xF)]
    public bool flareVisibilityTest;
    [Name("Lens Size"), Description("Size of the lens billboard"), MinVersion(0xC)]
    public float lensSize;
    [Name("Lens Offset"), Description("Offset of the lens billboard"), MinVersion(0xC)]
    public float lensOffset;
    [Name("Lens Material"), Description("Material to use for the lens"), MinVersion(0xC)]
    public Symbol lensMaterial = Symbol.Empty;
    [MaxVersion(0x12), MinVersion(0x12)]
    public string oldLensMaterialName = string.Empty;
    [Name("Additional Objects"), Description("Additional objects that should be drawn by the spotlight."), MinVersion(0xD)]
    public List<Symbol> additionalObjects = new();
    [Name("Slaves"), Description("Slave lights for projection lights and shadows"), MinVersion(0x1C)]
    public List<Symbol> slaves = new();
    [Name("Target Shadow"), Description("Whether the target should cast a shadow"), MinVersion(0x10)]
    public bool targetShadow;
    [Name("Animate Color From Preset"), Description("Whether this spotlight coloring should be animated by light presets."), MinVersion(0x11)]
    public bool animateColorFromPreset;
    [Name("Animate Orientation From Preset"), Description("Whether this spotlight position/rotation should be animated by light presets."), MinVersion(0x11)]
    public bool animateOrientationFromPreset;
    [Name("Color Owner"), Description("Master for light color and intensity"), MinVersion(0x1E)]
    public Symbol colorOwner = Symbol.Empty;

    [Name("Alpha"), MinVersion(33), MaxVersion(33)]
    public float alpha = 1.0f;

    private bool UsesLegoLayout(DirectoryMeta parent)
    {
        return parent.revision == 25 && revision == 33;
    }

    public new Spotlight Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 9)
        {
            throw new InvalidDataException("Unsupported Spotlight revision " + revision);
        }

        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        trans.Read(reader, false, parent, entry);

        spotScale = reader.ReadFloat();
        spotHeight = reader.ReadFloat();

        if (revision > 0x16)
        {
            beam.Read(reader, revision);
        }
        else
        {
            oldBeams = AssetHelpers.ReadList(reader, () => { BeamDef oldBeam = new(); oldBeam.Read(reader, revision); return oldBeam; });
            if (oldBeams.Count > 0)
            {
                beam = oldBeams[0];
            }
        }

        if (revision > 0x15)
        {
            lightCanMesh = Symbol.Read(reader);
        }
        else
        {
            oldLightCanGroup = Symbol.Read(reader);
        }

        target = Symbol.Read(reader);

        if (revision > 0x1C)
        {
            spotTarget = Symbol.Read(reader);
        }

        lightCanOffset = reader.ReadFloat();

        if (revision > 0x1E)
        {
            lightCanSort = reader.ReadBoolean();
        }

        color.Read(reader);

        if (revision > 9)
        {
            intensity = reader.ReadFloat();
        }

        spotMaterial = Symbol.Read(reader);
        if (revision == 0x12)
        {
            oldSpotMaterialName = reader.ReadUTF8(0x80);
        }

        dampingConstant = reader.ReadFloat();

        if (revision < 0x21 || UsesLegoLayout(parent))
        {
            oldSymbol = Symbol.Read(reader);
        }

        if (revision > 10)
        {
            flareMaterial = Symbol.Read(reader);
            if (revision > 0x11 && revision < 0x13)
            {
                oldFlareMaterialName = reader.ReadUTF8(0x80);
            }

            flareSize = new Vector2().Read(reader);
            flareRange = new Vector2().Read(reader);
            flareSteps = reader.ReadInt32();
            flareOffset = reader.ReadFloat();
        }

        if (revision > 0xD)
        {
            flareEnabled = reader.ReadBoolean();
        }

        if (revision > 0xE)
        {
            flareVisibilityTest = reader.ReadBoolean();
        }

        if (revision > 0xB)
        {
            lensSize = reader.ReadFloat();
            lensOffset = reader.ReadFloat();
            lensMaterial = Symbol.Read(reader);
        }

        if (revision == 0x12)
        {
            oldLensMaterialName = reader.ReadUTF8(0x80);
        }

        if (revision > 0xC)
        {
            additionalObjects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 0x1B)
        {
            slaves = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 0xF)
        {
            targetShadow = reader.ReadBoolean();
        }

        if (revision > 0x19)
        {
            animateColorFromPreset = reader.ReadBoolean();
            animateOrientationFromPreset = reader.ReadBoolean();
        }
        else if (revision > 0x10)
        {
            animateColorFromPreset = reader.ReadBoolean();
            animateOrientationFromPreset = animateColorFromPreset;
        }

        if (revision > 0x1D)
        {
            colorOwner = Symbol.Read(reader);
        }

        if (UsesLegoLayout(parent))
        {
            alpha = reader.ReadFloat();
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
        draw.Write(writer, false, parent, entry);
        trans.Write(writer, false, parent, entry);

        writer.WriteFloat(spotScale);
        writer.WriteFloat(spotHeight);

        if (revision > 0x16)
        {
            beam.Write(writer, revision);
        }
        else
        {
            AssetHelpers.WriteList(writer, oldBeams, oldBeam => oldBeam.Write(writer, revision));
        }

        Symbol.Write(writer, revision > 0x15 ? lightCanMesh : oldLightCanGroup);
        Symbol.Write(writer, target);

        if (revision > 0x1C)
        {
            Symbol.Write(writer, spotTarget);
        }

        writer.WriteFloat(lightCanOffset);

        if (revision > 0x1E)
        {
            writer.WriteBoolean(lightCanSort);
        }

        color.Write(writer);

        if (revision > 9)
        {
            writer.WriteFloat(intensity);
        }

        Symbol.Write(writer, spotMaterial);
        if (revision == 0x12)
        {
            writer.WriteAscii(oldSpotMaterialName, 0x80);
        }

        writer.WriteFloat(dampingConstant);

        if (revision < 0x21 || UsesLegoLayout(parent))
        {
            Symbol.Write(writer, oldSymbol);
        }

        if (revision > 10)
        {
            Symbol.Write(writer, flareMaterial);
            if (revision > 0x11 && revision < 0x13)
            {
                writer.WriteAscii(oldFlareMaterialName, 0x80);
            }

            flareSize.Write(writer);
            flareRange.Write(writer);
            writer.WriteInt32(flareSteps);
            writer.WriteFloat(flareOffset);
        }

        if (revision > 0xD)
        {
            writer.WriteBoolean(flareEnabled);
        }

        if (revision > 0xE)
        {
            writer.WriteBoolean(flareVisibilityTest);
        }

        if (revision > 0xB)
        {
            writer.WriteFloat(lensSize);
            writer.WriteFloat(lensOffset);
            Symbol.Write(writer, lensMaterial);
        }

        if (revision == 0x12)
        {
            writer.WriteAscii(oldLensMaterialName, 0x80);
        }

        if (revision > 0xC)
        {
            AssetHelpers.WriteList(writer, additionalObjects, item => Symbol.Write(writer, item));
        }

        if (revision > 0x1B)
        {
            AssetHelpers.WriteList(writer, slaves, item => Symbol.Write(writer, item));
        }

        if (revision > 0xF)
        {
            writer.WriteBoolean(targetShadow);
        }

        if (revision > 0x19)
        {
            writer.WriteBoolean(animateColorFromPreset);
            writer.WriteBoolean(animateOrientationFromPreset);
        }
        else if (revision > 0x10)
        {
            writer.WriteBoolean(animateColorFromPreset);
        }

        if (revision > 0x1D)
        {
            Symbol.Write(writer, colorOwner);
        }

        if (UsesLegoLayout(parent))
        {
            writer.WriteFloat(alpha);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
