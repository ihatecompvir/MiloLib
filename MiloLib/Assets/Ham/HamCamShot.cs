using MiloLib.Assets.World;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamCamShot"), Description("Hammer specific camera shot")]
public class HamCamShot : CamShot
{
    private ushort revision;
    private ushort altRevision;
    [Name("Targets")]
    public List<Target> targets = new();
    [Name("Zero Time"), Description("synchronization time for this camshot")]
    public float zeroTime;
    [Name("Min Time"), Description("30fps reg: minimum time this shot can last, DCuts: time past zero time in which the shot can be interupted")]
    public float minTime;
    [Name("Max Time"), Description("30fps maximum duration for this shot, 0 is infinite")]
    public float maxTime;
    [Name("Next Shots"), Description("Next camshots, in order")]
    public List<Symbol> nextShots = new();
    [Name("Player Flag"), Description("Flag to determine player configuration")]
    public int playerFlag;
    [Name("Master Anims"), Description("Anims set throughout this shot and any next shots Not valid entries for a next shot.")]
    public List<Symbol> masterAnims = new();

    public class Target
    {
        [Name("Target"), Description("Symbolic name of target")]
        public Symbol target = Symbol.Empty;
        [Name("Teleport"), Description("do we teleport this character?")]
        public bool teleport;
        [Name("To"), Description("the transform to teleport the character to")]
        public Matrix to = new();
        [Name("Anim Group"), Description("Name of CharClipGroup to play on character")]
        public Symbol animGroup = Symbol.Empty;
        [Name("Return"), Description("return to original position after shot?")]
        public bool returnToOrigin;
        [Name("Fast Forward"), Description("Fast forward chosen animation by this time, in camera units")]
        public float fastForward;
        [Name("Forward Event"), Description("Event to fastforward relative to")]
        public Symbol forwardEvent = Symbol.Empty;
        [Name("Self Shadow"), Description("should character cast a self shadow")]
        public bool selfShadow;
        public bool oldFlag1;
        public bool oldFlag2;
        [Name("Env Override"), Description("environment override for this target during this shot")]
        public Symbol environOverride = Symbol.Empty;
        [Name("Force LOD"), Description("Forces LOD, kLODPerFrame is normal behavior of picking per frame, the others force the lod (0 is highest res lod, 2 is lowest res lod)")]
        public sbyte forceLod = -1;

        public Target Read(EndianReader reader)
        {
            target = Symbol.Read(reader);
            teleport = reader.ReadBoolean();
            to.Read(reader);
            animGroup = Symbol.Read(reader);
            returnToOrigin = reader.ReadBoolean();
            fastForward = reader.ReadFloat();
            forwardEvent = Symbol.Read(reader);
            selfShadow = reader.ReadBoolean();
            oldFlag1 = reader.ReadBoolean();
            oldFlag2 = reader.ReadBoolean();
            environOverride = Symbol.Read(reader);
            forceLod = unchecked((sbyte)reader.ReadByte());
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, target);
            writer.WriteBoolean(teleport);
            to.Write(writer);
            Symbol.Write(writer, animGroup);
            writer.WriteBoolean(returnToOrigin);
            writer.WriteFloat(fastForward);
            Symbol.Write(writer, forwardEvent);
            writer.WriteBoolean(selfShadow);
            writer.WriteBoolean(oldFlag1);
            writer.WriteBoolean(oldFlag2);
            Symbol.Write(writer, environOverride);
            writer.WriteByte(unchecked((byte)forceLod));
        }
    }

    public new HamCamShot Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        targets = AssetHelpers.ReadList(reader, () => new Target().Read(reader));
        zeroTime = reader.ReadFloat();
        minTime = reader.ReadFloat();
        maxTime = reader.ReadFloat();
        nextShots = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        if (revision > 1)
        {
            playerFlag = reader.ReadInt32();
        }
        if (revision > 2)
        {
            masterAnims = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, targets, target => target.Write(writer));
        writer.WriteFloat(zeroTime);
        writer.WriteFloat(minTime);
        writer.WriteFloat(maxTime);
        AssetHelpers.WriteList(writer, nextShots, shot => Symbol.Write(writer, shot));
        if (revision > 1)
        {
            writer.WriteInt32(playerFlag);
        }
        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, masterAnims, anim => Symbol.Write(writer, anim));
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
