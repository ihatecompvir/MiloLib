using MiloLib.Utils;
using MiloLib.Classes;

namespace MiloLib.Assets.Band;

public enum CharInstrumentType
{
    guitar,
    bass,
    drum,
    mic,
    keyboard,
    numInstruments
}

public enum PatchCategory
{
    //  kNone,
    //  kTattoo,
    //  kLogo,
    //  kAccessory,
    //  kFacePaint
}

[Name("Outfit Piece"), Description("A piece of an outfit. Defines the color and the Milo scene for the outfit.")]
public class OutfitPiece
{
    [Name("Name"), Description("The Milo scene that the outfit piece uses.")]
    public Symbol name = Symbol.Empty;
    [Name("Colors"), Description("The indices of the color palettes the outfit piece uses.")]
    public int[] colors = new int[3];

    public OutfitPiece Read(EndianReader reader, ushort revision)
    {
        name = Symbol.Read(reader);
        colors[0] = reader.ReadByte();
        colors[1] = reader.ReadByte();
        if (revision > 0xB)
        {
            colors[2] = reader.ReadByte();
        }

        return this;
    }

    public void Write(EndianWriter writer, ushort revision)
    {
        Symbol.Write(writer, name);
        writer.WriteByte((byte)colors[0]);
        writer.WriteByte((byte)colors[1]);
        if (revision > 0xB)
        {
            writer.WriteByte((byte)colors[2]);
        }
    }
}

[Name("Head"), Description("The configuration of the character's head.")]
public class Head
{
    [Name("Hide"), Description("if true, loads no head, for development")]
    public bool hide;
    [Name("Eye Color"), Description("eye color index")]
    public int eyeColor;
    [Name("Shape"), Description("shape of the head index")]
    public int shape;
    [Name("Chin"), Description("chin index 0-2")]
    public int chin;
    [Name("Chin Width"), Description("chin length 0-1")]
    public float chinWidth;
    [Name("Chin Height"), Description("chin height 0-1")]
    public float chinHeight;
    [Name("Jaw Width"), Description("jaw length 0-1")]
    public float jawWidth;
    [Name("Jaw Height"), Description("jaw height 0-1")]
    public float jawHeight;
    [Name("Nose"), Description("nose index")]
    public int nose;
    [Name("Nose Width"), Description("nose width 0-1")]
    public float noseWidth;
    [Name("Nose Height"), Description("nose height 0-1")]
    public float noseHeight;
    [Name("Eye"), Description("eye index")]
    public int eye;
    [Name("Eye Separation"), Description("eye separation 0-1")]
    public float eyeSeparation;
    [Name("Eye Height"), Description("eye height 0-1")]
    public float eyeHeight;
    [Name("Eye Rotation"), Description("eye rotation 0-1")]
    public float eyeRotation;
    [Name("Mouth"), Description("mouth index")]
    public int mouth;
    [Name("Mouth Width"), Description("mouth width 0-1")]
    public float mouthWidth;
    [Name("Mouth Height"), Description("mouth height 0-1")]
    public float mouthHeight;
    [Name("Brow Separation"), Description("eyebrow separation 0-1")]
    public float browSeparation;
    [Name("Brow Height"), Description("eyebrow height 0-1")]
    public float browHeight;

    public void Read(EndianReader reader, ushort revision)
    {
        if (revision > 7)
        {
            hide = reader.ReadBoolean();
        }

        eyeColor = reader.ReadInt32();
        shape = reader.ReadInt32();
        chin = reader.ReadInt32();
        if (revision > 6)
        {
            chinWidth = reader.ReadFloat();
            chinHeight = reader.ReadFloat();
        }
        jawWidth = reader.ReadFloat();
        jawHeight = reader.ReadFloat();
        nose = reader.ReadInt32();
        if (revision > 5)
        {
            noseWidth = reader.ReadFloat();
        }

        noseHeight = reader.ReadFloat();
        eye = reader.ReadInt32();
        eyeSeparation = reader.ReadFloat();
        eyeHeight = reader.ReadFloat();
        if (revision > 10)
        {
            eyeRotation = reader.ReadFloat();
        }

        mouth = reader.ReadInt32();
        mouthWidth = reader.ReadFloat();
        mouthHeight = reader.ReadFloat();
        if (revision > 8)
        {
            browSeparation = reader.ReadFloat();
        }

        browHeight = reader.ReadFloat();
    }

    public void Write(EndianWriter writer, ushort revision)
    {
        if (revision > 7)
        {
            writer.WriteByte((byte)(hide ? 1 : 0));
        }

        writer.WriteInt32(eyeColor);
        writer.WriteInt32(shape);
        writer.WriteInt32(chin);
        if (revision > 6)
        {
            writer.WriteFloat(chinWidth);
            writer.WriteFloat(chinHeight);
        }
        writer.WriteFloat(jawWidth);
        writer.WriteFloat(jawHeight);
        writer.WriteInt32(nose);
        if (revision > 5)
        {
            writer.WriteFloat(noseWidth);
        }

        writer.WriteFloat(noseHeight);
        writer.WriteInt32(eye);
        writer.WriteFloat(eyeSeparation);
        writer.WriteFloat(eyeHeight);
        if (revision > 10)
        {
            writer.WriteFloat(eyeRotation);
        }

        writer.WriteInt32(mouth);
        writer.WriteFloat(mouthWidth);
        writer.WriteFloat(mouthHeight);
        if (revision > 8)
        {
            writer.WriteFloat(browSeparation);
        }

        writer.WriteFloat(browHeight);
    }
}

[Name("Outfit"), Description("The clothes the character is wearing.")]
public class Outfit
{
    [Name("Eyebrows"), Description("The name of the eyebrows Milo scene that this character uses.")]
    public OutfitPiece eyebrows = new();
    [Name("Face Hair"), Description("The name of the face hair Milo scene that this character uses.")]
    public OutfitPiece faceHair = new();
    [Name("Hair"), Description("The name of the hair Milo scene that this character uses.")]
    public OutfitPiece hair = new();
    [Name("Earrings"), Description("The name of the earrings Milo scene that this character uses.")]
    public OutfitPiece earrings = new();
    [Name("Glasses"), Description("The name of the glasses Milo scene that this character uses.")]
    public OutfitPiece glasses = new();
    [Name("Piercings"), Description("The name of the piercings Milo scene that this character uses.")]
    public OutfitPiece piercings = new();
    [Name("Feet"), Description("The name of the feet Milo scene that this character uses.")]
    public OutfitPiece feet = new();
    [Name("Hands"), Description("The name of the hands Milo scene that this character uses.")]
    public OutfitPiece hands = new();
    [Name("Legs"), Description("The name of the legs Milo scene that this character uses.")]
    public OutfitPiece legs = new();
    [Name("Rings"), Description("The name of the rings Milo scene that this character uses.")]
    public OutfitPiece rings = new();
    [Name("Torso"), Description("The name of the torso Milo scene that this character uses.")]
    public OutfitPiece torso = new();
    [Name("Wrist"), Description("The name of the wrist Milo scene that this character uses.")]
    public OutfitPiece wrist = new();

    public OutfitPiece oldPiece1 = new();
    public OutfitPiece oldPiece2 = new();
    public OutfitPiece oldPiece3 = new();


    public void Read(EndianReader reader, ushort revision)
    {
        if (revision < 5)
        {
            oldPiece1 = new OutfitPiece().Read(reader, revision);
        }
        else
        {
            eyebrows = new OutfitPiece().Read(reader, revision);
        }

        earrings = new OutfitPiece().Read(reader, revision);

        if (revision < 5)
        {
            oldPiece2 = new OutfitPiece().Read(reader, revision);
        }

        faceHair = new OutfitPiece().Read(reader, revision);
        glasses = new OutfitPiece().Read(reader, revision);
        hair = new OutfitPiece().Read(reader, revision);

        if (revision < 5)
        {
            oldPiece3 = new OutfitPiece().Read(reader, revision);
        }

        piercings = new OutfitPiece().Read(reader, revision);
        feet = new OutfitPiece().Read(reader, revision);
        hands = new OutfitPiece().Read(reader, revision);
        legs = new OutfitPiece().Read(reader, revision);
        rings = new OutfitPiece().Read(reader, revision);
        torso = new OutfitPiece().Read(reader, revision);
        wrist = new OutfitPiece().Read(reader, revision);
    }

    public void Write(EndianWriter writer, ushort revision)
    {
        if (revision < 5)
        {
            oldPiece1.Write(writer, revision);
        }
        else
        {
            eyebrows.Write(writer, revision);
        }

        earrings.Write(writer, revision);

        if (revision < 5)
        {
            oldPiece2.Write(writer, revision);
        }

        faceHair.Write(writer, revision);
        glasses.Write(writer, revision);
        hair.Write(writer, revision);

        if (revision < 5)
        {
            oldPiece3.Write(writer, revision);
        }

        piercings.Write(writer, revision);
        feet.Write(writer, revision);
        hands.Write(writer, revision);
        legs.Write(writer, revision);
        rings.Write(writer, revision);
        torso.Write(writer, revision);
        wrist.Write(writer, revision);
    }
}

[Name("Instrument Outfit"), Description("The instruments a character will use")]
public class InstrumentOutfit
{
    [Name("Guitar")]
    public OutfitPiece guitar = new();
    [Name("Bass")]
    public OutfitPiece bass = new();
    [Name("Drum")]
    public OutfitPiece drum = new();
    [Name("Mic")]
    public OutfitPiece mic = new();
    [Name("Keyboard")]
    public OutfitPiece keyboard = new();

    public InstrumentOutfit Read(EndianReader reader, ushort revision)
    {
        guitar = new OutfitPiece().Read(reader, revision);
        bass = new OutfitPiece().Read(reader, revision);
        drum = new OutfitPiece().Read(reader, revision);
        mic = new OutfitPiece().Read(reader, revision);
        keyboard = new OutfitPiece().Read(reader, revision);
        return this;
    }

    public void Write(EndianWriter writer, ushort revision)
    {
        guitar.Write(writer, revision);
        bass.Write(writer, revision);
        drum.Write(writer, revision);
        mic.Write(writer, revision);
        keyboard.Write(writer, revision);
    }
}

[Name("Patch"), Description("A patch is a texture that is applied to a mesh.")]
public class Patch
{
    [Name("Texture Index"), Description("texture index of profile, -1 means to interpret mesh_name as the actual patch mesh, which would live in color_palettes.milo")]
    public int texture;
    [Name("Category"), Description("Category of this patch")]
    public PatchCategory category;
    [Name("Mesh Name"), Description("name of placement mesh or mapping mesh. Valid placement meshes: placement_legs_L-back.mesh placement_legs_L-front.mesh placement_legs_R-back.mesh placement_legs_R-front.mesh placement_torso_back.mesh placement_torso_front.mesh placement_torso_L-lowerArm.mesh placement_torso_L-shoulder.mesh placement_torso_R-lowerArm.mesh placement_torso_R-shoulder.mesh")]
    public Symbol meshName = "";
    [Name("Rotation"), Description("in radians, about the uv space z axis.")]
    public float rotation;
    [Name("UV"), Description("UV in the mesh, u == -1 means mesh is mapping mesh.")]
    public Vector2 uv = new();
    [Name("Scale"), Description("local x and y scale factors.")]
    public Vector2 scale = new();
    public Patch Read(EndianReader reader)
    {
        texture = reader.ReadInt32();
        category = (PatchCategory)reader.ReadInt32();
        meshName = Symbol.Read(reader);
        uv = new Classes.Vector2().Read(reader);
        rotation = reader.ReadFloat();
        scale = new Classes.Vector2().Read(reader);

        return this;
    }
    public void Write(EndianWriter writer)
    {
        writer.WriteInt32(texture);
        writer.WriteInt32((int)category);
        Symbol.Write(writer, meshName);
        uv.Write(writer);
        writer.WriteFloat(rotation);
        scale.Write(writer);
    }
}
[Name("BandCharDesc"), Description("Band Character Description, contains all physical appearance attributes.")]
public class BandCharDesc : Object
{
    private ushort altRevision;
    private ushort revision;

    public ushort BandCharDescRevision => revision;
    public ushort BandCharDescAltRevision => altRevision;

    public void SetBandCharDescRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    [Name("Prefab"), Description("Prefab name if this is a non-editable prefab"), MinVersion(17)]
    public Symbol prefab = Symbol.Empty;

    [Name("Gender"), Description("take a wild guess")]
    public Symbol gender = Symbol.Empty;

    [Name("Skin Color"), Description("skin color, taken from skin.pal"), MinVersion(1)]
    public int skinColor;

    [Name("Head"), MinVersion(5)]
    public Head head = new();

    [Name("Instruments"), Description("instruments")]
    public InstrumentOutfit instruments = new();

    [Name("Outfit"), Description("clothing")]
    public Outfit outfit = new();

    [Name("Patches")]
    public List<Patch> patches = new();

    [Name("Height"), Description("Height of character, 0 - 1")]
    public float height;
    [Name("Weight"), Description("Weight of character, 0 - 1")]
    public float weight;
    [Name("Muscle"), Description("Muscle of character, 0 - 1")]
    public float muscle;

    public int unk224;

    [MinVersion(1), MaxVersion(4)]
    public uint head1;
    [MinVersion(1), MaxVersion(4)]
    public uint head2;

    internal bool skipBase;
    private Outfit oldOutfit = new();
    private int oldBuild = 0;
    private bool oldBool;
    private Symbol oldInstrumentSymbol = Symbol.Empty;


    public new BandCharDesc Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (!skipBase)
        {
            base.Read(reader, false, parent, entry);
        }

        if (revision > 0x10)
        {
            prefab = Symbol.Read(reader);
        }

        gender = Symbol.Read(reader);

        if (revision != 0)
        {
            skinColor = reader.ReadInt32();
            if (revision < 5)
            {
                head1 = reader.ReadUInt32();
                head2 = reader.ReadUInt32();
            }
            else
            {
                head = new Head();
                head.Read(reader, revision);
            }
        }

        outfit = new Outfit();
        outfit.Read(reader, revision);

        if (revision < 10)
        {
            oldOutfit = new Outfit();
            oldOutfit.Read(reader, revision);
        }

        if (revision > 1)
        {
            int build = 0;
            if (revision < 0xF)
            {
                build = reader.ReadInt32();
                oldBuild = build;
            }

            if (revision > 0xD)
            {
                height = reader.ReadFloat();
            }
            else
            {
                float[] heights = { 0.5f, 1.0f, 0.0f };
                height = heights[Math.Abs(build % 3)];
                build /= 3;
            }

            if (revision > 0xE)
            {
                weight = reader.ReadFloat();
                muscle = reader.ReadFloat();
            }
            else
            {
                float[] weights = { 0.5f, 1.0f, 0.5f, 0.0f, 0.5f };
                float[] muscles = { 0.5f, 0.5f, 1.0f, 0.5f, 0.0f };
                if (build >= 0 && build < weights.Length)
                {
                    weight = weights[build];
                    muscle = muscles[build];
                }
            }

            if (revision < 3)
            {
                oldBool = reader.ReadBoolean();
            }
        }

        if (revision > 3)
        {
            if (revision < 0x10)
            {
                oldInstrumentSymbol = Symbol.Read(reader);
            }

            instruments = new InstrumentOutfit();
            instruments.Read(reader, revision);
        }

        patches = new List<Patch>();
        if (revision > 0xC)
        {
            int count = reader.ReadInt32();

            if (count < 0 || count > 100)
            {
                throw new InvalidDataException("There are an invalid number of patches in the BandCharDesc, cannot read.");
            }

            for (int i = 0; i < count; i++)
            {
                Patch patch = new();
                patch.Read(reader);
                patches.Add(patch);
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
        if (!skipBase)
        {
            base.Write(writer, false, parent, entry);
        }

        // prefab is only written if revision > 0x10 (16)
        if (revision > 0x10)
        {
            Symbol.Write(writer, prefab);
        }

        Symbol.Write(writer, gender);

        // skinColor and head are only written if revision != 0
        if (revision != 0)
        {
            writer.WriteInt32(skinColor);

            // For revision < 5, write head1 and head2 (uint32s)
            // For revision >= 5, write head (Head object)
            if (revision < 5)
            {
                writer.WriteUInt32(head1);
                writer.WriteUInt32(head2);
            }
            else
            {
                head.Write(writer, revision);
            }
        }

        outfit.Write(writer, revision);

        if (revision < 10)
        {
            oldOutfit.Write(writer, revision);
        }

        if (revision > 1)
        {
            if (revision < 0xF)
            {
                writer.WriteInt32(oldBuild);
            }

            if (revision > 0xD)
            {
                writer.WriteFloat(height);
            }

            if (revision > 0xE)
            {
                writer.WriteFloat(weight);
                writer.WriteFloat(muscle);
            }

            if (revision < 3)
            {
                writer.WriteBoolean(oldBool);
            }
        }

        if (revision > 3)
        {
            if (revision < 0x10)
            {
                Symbol.Write(writer, oldInstrumentSymbol);
            }

            instruments.Write(writer, revision);
        }

        if (revision > 0xC)
        {
            AssetHelpers.WriteList(writer, patches, (Patch patch) => patch.Write(writer));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
