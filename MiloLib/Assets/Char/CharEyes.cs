using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharEyes"), Description("Moves a bunch of lookats around")]
public class CharEyes : Object
{
    public class EyeDesc
    {
        [Name("Eye"), Description("Eye to retarget")]
        public Symbol eye = Symbol.Empty;

        [Name("Upper Lid"), Description("corresponding upper lid bone, must rotate about Z")]
        public Symbol upperLid = Symbol.Empty;

        [Name("Lower Lid"), Description("corresponding lower lid bone, must rotate about Z")]
        public Symbol lowerLid = Symbol.Empty;

        [Name("Upper Lid Blink"), Description("optional - child of upper_lid, placed at edge of upper lid geometry. It will be used to detect and resolve interpenetration of the lids")]
        public Symbol upperLidBlink = Symbol.Empty;

        [Name("Lower Lid Blink"), Description("optional - child of lower_lid, placed at edge of lower lid geometry. It will be used to detect and resolve interpenetration of the lids")]
        public Symbol lowerLidBlink = Symbol.Empty;

        public EyeDesc Read(EndianReader reader, ushort revision)
        {
            eye = Symbol.Read(reader);
            upperLid = Symbol.Read(reader);
            if (revision > 6)
            {
                lowerLid = Symbol.Read(reader);
            }
            if (revision > 0x11)
            {
                upperLidBlink = Symbol.Read(reader);
                lowerLidBlink = Symbol.Read(reader);
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, eye);
            Symbol.Write(writer, upperLid);
            if (revision > 6)
            {
                Symbol.Write(writer, lowerLid);
            }
            if (revision > 0x11)
            {
                Symbol.Write(writer, upperLidBlink);
                Symbol.Write(writer, lowerLidBlink);
            }
        }
    }

    public class CharInterestState
    {
        [Name("Interest")]
        public Symbol interest = Symbol.Empty;

        public CharInterestState Read(EndianReader reader)
        {
            interest = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, interest);
        }
    }

    public class LegacyInterestEntry
    {
        [HideInInspector]
        public Symbol target = Symbol.Empty;

        [HideInInspector]
        public int value;

        public LegacyInterestEntry Read(EndianReader reader)
        {
            target = Symbol.Read(reader);
            value = reader.ReadInt32();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, target);
            writer.WriteInt32(value);
        }
    }

    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();

    [Name("Eyes")]
    public List<EyeDesc> eyes = new();

    [HideInInspector, MinVersion(3), MaxVersion(4)]
    public Symbol oldTransformable = Symbol.Empty;

    [HideInInspector, MinVersion(4), MaxVersion(8)]
    public List<LegacyInterestEntry> oldInterests = new();

    [Name("Interests"), MinVersion(9)]
    public List<CharInterestState> interests = new();

    [Name("Face Servo"), Description("the CharFaceServo if any, used to allow blinks through the eyelid following"), MinVersion(5)]
    public Symbol faceServo = Symbol.Empty;

    [Name("Camera Weight"), Description("The weight setter for eyes tracking the camera"), MinVersion(8)]
    public Symbol cameraWeight = Symbol.Empty;

    [MinVersion(10)]
    public int defaultInterestCategories;

    [Name("View Direction"), Description("optional bone that serves as the reference for which direction the character is looking. If not set, one of the eyes will be used"), MinVersion(11)]
    public Symbol viewDirection = Symbol.Empty;

    [Name("Head Lookat"), Description("optionally supply a head lookat to inform eyes what the head is doing. used primarily to coordinate eye lookats with head ones..."), MinVersion(12)]
    public Symbol headLookAt = Symbol.Empty;

    [Name("Max Extrapolation"), Description("in degrees, the maximum angle we can offset the current view direction when extrapolating for generated interests"), MinVersion(13)]
    public float maxExtrapolation;

    [Name("Min Target Dist"), Description("the minimum distance, in inches, that this interest can be from the eyes. If the interest is less than this distance, the eyes look in the same direction, but projected out to this distance. May be overridden per interest object."), MinVersion(14)]
    public float minTargetDist;

    [Name("Ulid Track Up"), Description("affects rotation applied to upper lid when eyes rotate up"), MinVersion(15)]
    public float upperLidTrackUp;

    [Name("Ulid Track Down"), Description("affects rotation applied to upper lid when eyes rotate down"), MinVersion(15)]
    public float upperLidTrackDown;

    [Name("Llid Track Up"), Description("translates lower lids up/down when eyes rotate up"), MinVersion(15)]
    public float lowerLidTrackUp;

    [Name("Llid Track Up Out"), Description("translates lower lids in/out when eyes rotate up"), MinVersion(17), MaxVersion(17)]
    public float lowerLidTrackUpOut;

    [HideInInspector, MinVersion(15), MaxVersion(16)]
    public int oldLowerLidTrackInt1;

    [Name("Llid Track Down"), Description("translates lower lids up/down when eyes rotate down"), MinVersion(15)]
    public float lowerLidTrackDown;

    [Name("Llid Track Down Out"), Description("translates lower lids in/out when eyes rotate down"), MinVersion(17), MaxVersion(17)]
    public float lowerLidTrackDownOut;

    [HideInInspector, MinVersion(15), MaxVersion(16)]
    public int oldLowerLidTrackInt2;

    [HideInInspector, MinVersion(16), MaxVersion(17)]
    public uint tbrbUnknownRev16Plus1;

    [HideInInspector, MinVersion(16), MaxVersion(17)]
    public uint tbrbUnknownRev16Plus2;

    [HideInInspector, MinVersion(17), MaxVersion(17)]
    public bool tbrbUnknownRev17Bool;

    [Name("Llid Track Rotate"), Description("if checked, lower lid tracking is done by rotation instead of translation"), MinVersion(18)]
    public bool lowerLidTrackRotate;

    public new CharEyes Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision > 5)
        {
            weightable = weightable.Read(reader, false, parent, entry);
        }

        if (revision > 4)
        {
            eyes = AssetHelpers.ReadList(reader, () => new EyeDesc().Read(reader, revision));
        }
        else
        {
            eyes = AssetHelpers.ReadList(reader, () => new EyeDesc { eye = Symbol.Read(reader) });
        }

        if (revision == 3 || revision == 4)
        {
            oldTransformable = Symbol.Read(reader);
        }

        oldInterests = new List<LegacyInterestEntry>();
        interests = new List<CharInterestState>();

        if (revision >= 4 && revision <= 8)
        {
            oldInterests = AssetHelpers.ReadList(reader, () => new LegacyInterestEntry().Read(reader));
        }
        else if (revision > 8)
        {
            interests = AssetHelpers.ReadList(reader, () => new CharInterestState().Read(reader));
        }

        if (revision > 4)
        {
            faceServo = Symbol.Read(reader);
        }

        if (revision > 7)
        {
            cameraWeight = Symbol.Read(reader);
        }

        if (revision > 9)
        {
            defaultInterestCategories = reader.ReadInt32();
        }

        if (revision > 10)
        {
            viewDirection = Symbol.Read(reader);
        }

        if (revision > 0xB)
        {
            headLookAt = Symbol.Read(reader);
        }

        if (revision > 0xC)
        {
            maxExtrapolation = reader.ReadFloat();
        }

        if (revision > 0xD)
        {
            minTargetDist = reader.ReadFloat();
        }

        if (revision > 0xE)
        {
            upperLidTrackUp = reader.ReadFloat();
            upperLidTrackDown = reader.ReadFloat();
            lowerLidTrackUp = reader.ReadFloat();

            if (revision < 0x11)
            {
                oldLowerLidTrackInt1 = reader.ReadInt32();
                lowerLidTrackDown = reader.ReadFloat();
                oldLowerLidTrackInt2 = reader.ReadInt32();
            }
            else if (revision == 0x11)
            {
                lowerLidTrackUpOut = reader.ReadFloat();
                lowerLidTrackDown = reader.ReadFloat();
                lowerLidTrackDownOut = reader.ReadFloat();
            }
            else
            {
                lowerLidTrackDown = reader.ReadFloat();
            }
        }

        if (revision > 0xF && revision < 0x12)
        {
            tbrbUnknownRev16Plus1 = reader.ReadUInt32();
            tbrbUnknownRev16Plus2 = reader.ReadUInt32();
        }

        if (revision == 0x11)
        {
            tbrbUnknownRev17Bool = reader.ReadBoolean();
        }
        else if (revision > 0x11)
        {
            lowerLidTrackRotate = reader.ReadBoolean();
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

        if (revision > 5)
        {
            weightable.Write(writer, false, parent, entry);
        }

        if (revision > 4)
        {
            AssetHelpers.WriteList(writer, eyes, (EyeDesc eye) => eye.Write(writer, revision));
        }
        else
        {
            AssetHelpers.WriteList(writer, eyes, (EyeDesc eye) => Symbol.Write(writer, eye.eye));
        }

        if (revision == 3 || revision == 4)
        {
            Symbol.Write(writer, oldTransformable);
        }

        if (revision >= 4 && revision <= 8)
        {
            AssetHelpers.WriteList(writer, oldInterests, (LegacyInterestEntry legacyInterest) => legacyInterest.Write(writer));
        }
        else if (revision > 8)
        {
            AssetHelpers.WriteList(writer, interests, (CharInterestState interest) => interest.Write(writer));
        }

        if (revision > 4)
        {
            Symbol.Write(writer, faceServo);
        }

        if (revision > 7)
        {
            Symbol.Write(writer, cameraWeight);
        }

        if (revision > 9)
        {
            writer.WriteInt32(defaultInterestCategories);
        }

        if (revision > 10)
        {
            Symbol.Write(writer, viewDirection);
        }

        if (revision > 0xB)
        {
            Symbol.Write(writer, headLookAt);
        }

        if (revision > 0xC)
        {
            writer.WriteFloat(maxExtrapolation);
        }

        if (revision > 0xD)
        {
            writer.WriteFloat(minTargetDist);
        }

        if (revision > 0xE)
        {
            writer.WriteFloat(upperLidTrackUp);
            writer.WriteFloat(upperLidTrackDown);
            writer.WriteFloat(lowerLidTrackUp);

            if (revision < 0x11)
            {
                writer.WriteInt32(oldLowerLidTrackInt1);
                writer.WriteFloat(lowerLidTrackDown);
                writer.WriteInt32(oldLowerLidTrackInt2);
            }
            else if (revision == 0x11)
            {
                writer.WriteFloat(lowerLidTrackUpOut);
                writer.WriteFloat(lowerLidTrackDown);
                writer.WriteFloat(lowerLidTrackDownOut);
            }
            else
            {
                writer.WriteFloat(lowerLidTrackDown);
            }
        }

        if (revision > 0xF && revision < 0x12)
        {
            writer.WriteUInt32(tbrbUnknownRev16Plus1);
            writer.WriteUInt32(tbrbUnknownRev16Plus2);
        }

        if (revision == 0x11)
        {
            writer.WriteBoolean(tbrbUnknownRev17Bool);
        }
        else if (revision > 0x11)
        {
            writer.WriteBoolean(lowerLidTrackRotate);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
