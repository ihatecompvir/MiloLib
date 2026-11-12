using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharIKFingers"), Description("Pins fingers to world positions")]
public class CharIKFingers : Object
{
    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();

    [Name("Is Right Hand"), Description("Does this run the right or left hand?"), MinVersion(2)]
    public bool isRightHand;

    [Name("Output Trans"), Description("This trans will be set to the desired hand position."), MinVersion(3)]
    public Symbol outputTrans = Symbol.Empty;

    [Name("Keyboard Ref Bone"), Description("A keyboard bone so we can calculate in local space. use rh/lh targets."), MinVersion(4)]
    public Symbol keyboardRefBone = Symbol.Empty;

    [Name("Hand Keyboard Offset"), Description("Starting hand offset from keyboard."), MinVersion(5)]
    public Vector3 handKeyboardOffset = new();

    [Name("Hand Thumb Rotation"), Description("how much to rotate the hand (radians) when thumb is engaged"), MinVersion(5)]
    public float handThumbRotation;

    [Name("Hand Pinky Rotation"), Description("how much to rotate the hand (radians) when pinky is engaged"), MinVersion(5)]
    public float handPinkyRotation;

    [Name("Hand Move Forward"), Description("how much to move forward when pinky or thumb is engaged"), MinVersion(5)]
    public float handMoveForward;

    [Name("Hand Dest Offset"), Description("x offset for right/left hands from average destination position for fingers"), MinVersion(5)]
    public float handDestOffset;

    public new CharIKFingers Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        weightable = weightable.Read(reader, false, parent, entry);

        if (revision > 1)
        {
            isRightHand = reader.ReadBoolean();
        }

        if (revision > 2)
        {
            outputTrans = Symbol.Read(reader);
        }

        if (revision > 3)
        {
            keyboardRefBone = Symbol.Read(reader);
        }

        if (revision > 4)
        {
            handKeyboardOffset = new Vector3().Read(reader);
            handThumbRotation = reader.ReadFloat();
            handPinkyRotation = reader.ReadFloat();
            handMoveForward = reader.ReadFloat();
            handDestOffset = reader.ReadFloat();
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

        if (revision > 1)
        {
            writer.WriteBoolean(isRightHand);
        }

        if (revision > 2)
        {
            Symbol.Write(writer, outputTrans);
        }

        if (revision > 3)
        {
            Symbol.Write(writer, keyboardRefBone);
        }

        if (revision > 4)
        {
            handKeyboardOffset.Write(writer);
            writer.WriteFloat(handThumbRotation);
            writer.WriteFloat(handPinkyRotation);
            writer.WriteFloat(handMoveForward);
            writer.WriteFloat(handDestOffset);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
