using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharIKHand"), Description("Pins a hand bone to another RndTransformable, bending the elbow to make it reach. Optionally aligns orientations and stretches")]
public class CharIKHand : Object
{
    public class IKTarget
    {
        [Name("Target"), Description("Where to move the hand to")]
        public Symbol target = Symbol.Empty;

        [Name("Extent"), Description("Distance along the negative z axis of the transform to snap to")]
        public float extent;

        public IKTarget Read(EndianReader reader)
        {
            target = Symbol.Read(reader);
            extent = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, target);
            writer.WriteFloat(extent);
        }
    }

    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();

    [Name("Hand"), Description("The hand to be moved, must be child of elbow")]
    public Symbol hand = Symbol.Empty;

    [Name("Finger"), Description("If non null, will be the thing that actually hits the target, the hand will be moved into such a location as to make it hit. You probably always want to turn on orientation in this case, as otherwise, the hand will be in a somewhat random orientation, which will probably mean that the finger will miss the mark."), MinVersion(5)]
    public Symbol finger = Symbol.Empty;

    [Name("Targets"), Description("Targets for the hand")]
    public List<IKTarget> targets = new();

    [Name("Orientation"), Description("Orient the hand to the dest")]
    public bool orientation;

    [Name("Stretch"), Description("Stretch the hand to the dest")]
    public bool stretch;

    [Name("Scalable"), Description("Recalculate bone length every frame, needed for bones which scale"), MinVersion(2)]
    public bool scalable;

    [Name("Move Elbow"), Description("Moves the elbow and shoulder to position the hand, if false, just teleports the hand"), MinVersion(4)]
    public bool moveElbow = true;

    [Name("Elbow Swing"), Description("Range to swing the elbow in radians to hit target, better looking suggest .7"), MinVersion(6)]
    public float elbowSwing;

    [Name("Always IK Elbow"), Description("Turn this on to do IK calcs even if weight is 0"), MinVersion(7)]
    public bool alwaysIKElbow;

    [Name("Constrain Wrist"), Description("Constrain the wrist rotation to be believable"), MinVersion(8)]
    public bool constrainWrist;

    [Name("Wrist Radians"), Description("Constrain wrist rotation to this angle (in radians)"), MinVersion(8)]
    public float wristRadians;

    [HideInInspector, MinVersion(9), MaxVersion(9)]
    public Symbol oldRev9String = Symbol.Empty;

    [HideInInspector, MinVersion(9), MaxVersion(9)]
    public bool oldRev9Bool;

    [Name("Elbow Collide"), Description("Collision sphere that elbow won't enter."), MinVersion(12)]
    public Symbol elbowCollide = Symbol.Empty;

    [Name("Clockwise"), Description("Choose the clockwise solution for the collision detection"), MinVersion(12)]
    public bool clockwise;

    [Name("Pull Shoulder"), Description("Are we allowed to pull the shoulder to reach goal, or do we lock the elbow when goal is too far?"), MinVersion(13)]
    public bool pullShoulder;

    public new CharIKHand Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        weightable = weightable.Read(reader, false, parent, entry);

        hand = Symbol.Read(reader);

        if (revision > 4)
        {
            finger = Symbol.Read(reader);
        }

        if (revision < 3)
        {
            targets = new List<IKTarget>(1)
            {
                new() { target = Symbol.Read(reader) }
            };
        }
        else if (revision < 0xB)
        {
            targets = AssetHelpers.ReadList(reader, () => new IKTarget { target = Symbol.Read(reader) });
        }
        else
        {
            targets = AssetHelpers.ReadList(reader, () => new IKTarget().Read(reader));
        }

        orientation = reader.ReadBoolean();
        stretch = reader.ReadBoolean();

        if (revision > 1)
        {
            scalable = reader.ReadBoolean();
        }
        else
        {
            scalable = false;
        }

        if (revision > 3)
        {
            moveElbow = reader.ReadBoolean();
        }
        else
        {
            moveElbow = true;
        }

        if (revision > 5)
        {
            elbowSwing = reader.ReadFloat();
        }
        else
        {
            elbowSwing = 0.0f;
        }

        if (revision > 6)
        {
            alwaysIKElbow = reader.ReadBoolean();
        }

        if (revision > 7)
        {
            constrainWrist = reader.ReadBoolean();
            wristRadians = reader.ReadFloat();
        }

        if (revision == 9)
        {
            oldRev9String = Symbol.Read(reader);
            oldRev9Bool = reader.ReadBoolean();
        }

        if (revision > 0xB)
        {
            elbowCollide = Symbol.Read(reader);
            clockwise = reader.ReadBoolean();
        }
        if (revision > 12)
        {
            pullShoulder = reader.ReadBoolean();
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

        Symbol.Write(writer, hand);

        if (revision > 4)
        {
            Symbol.Write(writer, finger);
        }

        if (revision < 3)
        {
            Symbol.Write(writer, targets.Count > 0 ? targets[0].target : Symbol.Empty);
        }
        else if (revision < 0xB)
        {
            AssetHelpers.WriteList(writer, targets, (IKTarget target) => Symbol.Write(writer, target.target));
        }
        else
        {
            AssetHelpers.WriteList(writer, targets, (IKTarget target) => target.Write(writer));
        }

        writer.WriteBoolean(orientation);
        writer.WriteBoolean(stretch);

        if (revision > 1)
        {
            writer.WriteBoolean(scalable);
        }

        if (revision > 3)
        {
            writer.WriteBoolean(moveElbow);
        }

        if (revision > 5)
        {
            writer.WriteFloat(elbowSwing);
        }

        if (revision > 6)
        {
            writer.WriteBoolean(alwaysIKElbow);
        }

        if (revision > 7)
        {
            writer.WriteBoolean(constrainWrist);
            writer.WriteFloat(wristRadians);
        }

        if (revision == 9)
        {
            Symbol.Write(writer, oldRev9String);
            writer.WriteBoolean(oldRev9Bool);
        }

        if (revision > 0xB)
        {
            Symbol.Write(writer, elbowCollide);
            writer.WriteBoolean(clockwise);
        }
        if (revision > 12)
        {
            writer.WriteBoolean(pullShoulder);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
