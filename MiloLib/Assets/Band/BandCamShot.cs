using MiloLib.Assets.World;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

[Name("BandCamShot"), Description("Band specific camera shot")]
public class BandCamShot : Object
{
    public class OldTrigger
    {
        [Name("Frame"), Description("Frame number for this trigger to go off")]
        public float frame;

        [Name("Trigger"), Description("Event trigger")]
        public Symbol trigger = Symbol.Empty;

        [Name("Trigger Group"), Description("Event trigger group")]
        public Symbol triggerGroup = Symbol.Empty;

        public OldTrigger Read(EndianReader reader, bool greenDayLayout = false)
        {
            frame = reader.ReadFloat();
            trigger = Symbol.Read(reader);
            if (greenDayLayout)
            {
                triggerGroup = Symbol.Read(reader);
            }

            return this;
        }

        public void Write(EndianWriter writer, bool greenDayLayout = false)
        {
            writer.WriteFloat(frame);
            Symbol.Write(writer, trigger);
            if (greenDayLayout)
            {
                Symbol.Write(writer, triggerGroup);
            }
        }
    }

    public class Target
    {
        [Name("Target"), Description("Symbolic name of target")]
        public Symbol target = Symbol.Empty;

        [Name("Teleport"), Description("do we teleport this character?"), MinVersion(11)]
        public byte teleport = 1;

        [Name("To"), Description("the transform to teleport the character to")]
        public Matrix xfm = new();

        [MaxVersion(0)]
        public Matrix oldExtraTf = new();

        [Name("Anim Group"), Description("CharClipGroup to play on character")]
        public Symbol animGroup = Symbol.Empty;

        [Name("Return"), Description("return to original position after shot?")]
        public byte returnVal = 1;

        [MinVersion(3), MaxVersion(28)]
        public bool oldBool;
        [MinVersion(3), MaxVersion(28)]
        public int oldBoolInt;

        [MinVersion(3), MaxVersion(11)]
        public int oldInt0C;

        [Name("Fast Forward"), Description("Fast forward chosen animation by this time, in camera units"), MinVersion(4)]
        public float fastForward;

        [Name("Forward Event"), Description("Event to fastforward relative to"), MinVersion(11)]
        public Symbol forwardEvent = Symbol.Empty;

        [Name("Self Shadow"), Description("should character cast a self shadow"), MinVersion(6)]
        public bool selfShadow;

        [MinVersion(7)]
        public byte unk1;
        [MinVersion(7)]
        public byte unk2 = 1;

        [Name("Hide"), Description("should the target be hidden"), MinVersion(32)]
        public byte hide;

        [MinVersion(8), MaxVersion(28)]
        public Symbol oldSym1 = Symbol.Empty;
        [MinVersion(8), MaxVersion(28)]
        public Symbol oldSym2 = Symbol.Empty;
        [MinVersion(9), MaxVersion(28)]
        public Symbol oldSym3 = Symbol.Empty;
        [MinVersion(9), MaxVersion(28)]
        public Symbol oldSym4 = Symbol.Empty;

        [Name("Env Override"), Description("environment override for this target during this shot"), MinVersion(10)]
        public Symbol envOverride = Symbol.Empty;

        [MinVersion(17), MaxVersion(28)]
        public bool oldBool1728;

        [Name("Force LOD"), Description("Forces LOD, kLODPerFrame is normal behavior of picking per frame, the others force the lod (0 is highest res lod, 2 is lowest res lod)"), MinVersion(21)]
        public int forceLod = -1;

        [Name("LOD Override"), Description("Force target to least-detailed LOD"), MinVersion(21), MaxVersion(21)]
        public bool lodOverride;

        [Name("Outfit Hides"), Description("Hide specified character outfits"), MinVersion(21), MaxVersion(21)]
        public uint outfitHides;

        [MinVersion(22), MaxVersion(28)]
        public Symbol oldStr1 = Symbol.Empty;
        [MinVersion(22), MaxVersion(28)]
        public Symbol oldStr2 = Symbol.Empty;
        [MinVersion(22), MaxVersion(28)]
        public int oldInt1;
        [MinVersion(22), MaxVersion(28)]
        public int oldInt2;
        [MinVersion(24), MaxVersion(28)]
        public int oldInt3;
        [MinVersion(24), MaxVersion(28)]
        public int oldInt4;
        [MinVersion(24), MaxVersion(28)]
        public int oldInt5;

        [Name("LOD Exception")]
        public bool lodException;

        public Target Read(EndianReader reader, ushort bandRev, bool legoLayout = false, ushort greenDayAltRevision = 0)
        {
            target = Symbol.Read(reader);
            if (bandRev > 0xA)
            {
                teleport = reader.ReadByte();
            }

            xfm = new Matrix().Read(reader);
            if (bandRev == 0)
            {
                oldExtraTf = new Matrix().Read(reader);
            }

            animGroup = Symbol.Read(reader);
            returnVal = reader.ReadByte();
            if (bandRev > 2 && bandRev < 0x1D)
            {
                oldBool = reader.ReadBoolean();
                oldBoolInt = reader.ReadInt32();
            }
            if (bandRev > 2 && bandRev < 0xC)
            {
                oldInt0C = reader.ReadInt32();
            }

            if (bandRev > 3)
            {
                fastForward = reader.ReadFloat();
            }

            if (bandRev > 0xA)
            {
                forwardEvent = Symbol.Read(reader);
            }

            if (bandRev > 5)
            {
                selfShadow = reader.ReadBoolean();
            }

            if (bandRev > 6)
            {
                unk1 = reader.ReadByte();
                unk2 = reader.ReadByte();
            }
            if (bandRev > 6 && bandRev > 0x1F)
            {
                hide = reader.ReadByte();
            }

            if (bandRev >= 8 && bandRev <= 28)
            {
                oldSym1 = Symbol.Read(reader);
                oldSym2 = Symbol.Read(reader);
                if (bandRev > 8)
                {
                    oldSym3 = Symbol.Read(reader);
                    oldSym4 = Symbol.Read(reader);
                }
            }
            if (bandRev > 9)
            {
                envOverride = Symbol.Read(reader);
            }

            if (bandRev >= 17 && bandRev <= 28)
            {
                oldBool1728 = reader.ReadBoolean();
            }

            if (legoLayout)
            {
                lodOverride = reader.ReadBoolean();
                outfitHides = reader.ReadUInt32();
            }
            else if (bandRev > 0x14)
            {
                if (bandRev < 0x1E)
                {
                    forceLod = reader.ReadInt32();
                }
                else
                {
                    forceLod = (sbyte)reader.ReadByte();
                }
            }
            if (bandRev >= 22 && bandRev <= 28)
            {
                oldStr1 = Symbol.Read(reader);
                oldStr2 = Symbol.Read(reader);
                oldInt1 = reader.ReadInt32();
                oldInt2 = reader.ReadInt32();
                if (bandRev > 0x17)
                {
                    oldInt3 = reader.ReadInt32();
                    oldInt4 = reader.ReadInt32();
                    oldInt5 = reader.ReadInt32();
                    if (greenDayAltRevision > 0)
                    {
                        lodException = reader.ReadBoolean();
                    }
                }
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort bandRev, bool legoLayout = false, ushort greenDayAltRevision = 0)
        {
            Symbol.Write(writer, target);
            if (bandRev > 0xA)
            {
                writer.WriteByte(teleport);
            }

            xfm.Write(writer);
            if (bandRev == 0)
            {
                oldExtraTf.Write(writer);
            }

            Symbol.Write(writer, animGroup);
            writer.WriteByte(returnVal);
            if (bandRev > 2 && bandRev < 0x1D)
            {
                writer.WriteBoolean(oldBool);
                writer.WriteInt32(oldBoolInt);
            }
            if (bandRev > 2 && bandRev < 0xC)
            {
                writer.WriteInt32(oldInt0C);
            }

            if (bandRev > 3)
            {
                writer.WriteFloat(fastForward);
            }

            if (bandRev > 0xA)
            {
                Symbol.Write(writer, forwardEvent);
            }

            if (bandRev > 5)
            {
                writer.WriteBoolean(selfShadow);
            }

            if (bandRev > 6)
            {
                writer.WriteByte(unk1);
                writer.WriteByte(unk2);
            }
            if (bandRev > 6 && bandRev > 0x1F)
            {
                writer.WriteByte(hide);
            }

            if (bandRev >= 8 && bandRev <= 28)
            {
                Symbol.Write(writer, oldSym1);
                Symbol.Write(writer, oldSym2);
                if (bandRev > 8)
                {
                    Symbol.Write(writer, oldSym3);
                    Symbol.Write(writer, oldSym4);
                }
            }
            if (bandRev > 9)
            {
                Symbol.Write(writer, envOverride);
            }

            if (bandRev >= 17 && bandRev <= 28)
            {
                writer.WriteBoolean(oldBool1728);
            }

            if (legoLayout)
            {
                writer.WriteBoolean(lodOverride);
                writer.WriteUInt32(outfitHides);
            }
            else if (bandRev > 0x14)
            {
                if (bandRev < 0x1E)
                {
                    writer.WriteInt32(forceLod);
                }
                else
                {
                    writer.WriteByte((byte)(sbyte)forceLod);
                }
            }
            if (bandRev >= 22 && bandRev <= 28)
            {
                Symbol.Write(writer, oldStr1);
                Symbol.Write(writer, oldStr2);
                writer.WriteInt32(oldInt1);
                writer.WriteInt32(oldInt2);
                if (bandRev > 0x17)
                {
                    writer.WriteInt32(oldInt3);
                    writer.WriteInt32(oldInt4);
                    writer.WriteInt32(oldInt5);
                    if (greenDayAltRevision > 0)
                    {
                        writer.WriteBoolean(lodException);
                    }
                }
            }
        }
    }

    private ushort altRevision;
    private ushort revision;

    // shit
    private bool UsesGreenDayLayout(DirectoryMeta parent)
    {
        // i hate green day
        return parent.revision == 25 && revision == 28;
    }

    private ushort GreenDayAltRevision(DirectoryMeta parent)
    {
        if (UsesGreenDayLayout(parent))
        {
            return altRevision;
        }
        // beatles wii has its own shit
        if (parent.revision == 25 && revision == 27)
        {
            return altRevision;
        }

        return 0;
    }

    [Name("DOF Tint")]
    public bool dofTint;
    [Name("DOF Tint Color")]
    public HmxColor4 dofTintColor = new();

    [Name("FPS Override"), Description("Force world to render at 20Hz when 3 or more gem tracks are in play (Wii only)"), MinVersion(21), MaxVersion(21)]
    public bool fpsOverride;

    private bool UsesLegoLayout(DirectoryMeta parent)
    {
        // LEGO gotta be special too
        return parent.revision == 25 && revision == 21;
    }

    public CamShot camShot = new();

    [Name("Targets")]
    public List<Target> targets = new();

    [MinVersion(2), MaxVersion(18)]
    public Symbol oldNextShot = Symbol.Empty;

    [Name("Zero Time"), Description("synchronization time for this camshot"), MinVersion(4)]
    public float zeroTime;

    [MinVersion(13), MaxVersion(13)]
    public bool oldPS3PerPixel;

    [Name("Triggers"), MinVersion(15), MaxVersion(28)]
    public List<OldTrigger> oldTriggers = new();

    [Name("Event At Start"), Description("Event Trigger to fire when this camshot starts"), MinVersion(16), MaxVersion(28)]
    public Symbol oldEventTrig1 = Symbol.Empty;
    [Name("Event At End"), Description("Event Trigger to fire when this camshot ends"), MinVersion(16), MaxVersion(28)]
    public Symbol oldEventTrig2 = Symbol.Empty;

    [Name("Min Time"), Description("30fps reg: minimum time this shot can last, DCuts: time past zero time in which the shot can be interupted"), MinVersion(18)]
    public int minTime;

    [Name("Max Time"), Description("30fps maximum duration for this shot, 0 is infinite"), MinVersion(18)]
    public int maxTime;

    [Name("Next Shots"), Description("Next camshots, in order"), MinVersion(19)]
    public List<Symbol> nextShots = new();

    [MinVersion(20), MaxVersion(30)]
    public int oldInt2030;

    [Name("Reset Event At Start"), Description("if true, resets the event_at_start when the camera shot is over"), MinVersion(23), MaxVersion(28)]
    public bool oldResetBool;

    [MinVersion(28), MaxVersion(28)]
    public List<Symbol> oldAnims1C = new();

    public new BandCamShot Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 4)
        {
            camShot = new CamShot().Read(reader, false, parent, entry);
        }

        targets = AssetHelpers.ReadList(reader, () => new Target().Read(reader, revision, UsesLegoLayout(parent), GreenDayAltRevision(parent)));

        if ((revision >= 2 && revision <= 18) || UsesLegoLayout(parent))
        {
            oldNextShot = Symbol.Read(reader);
        }

        if (revision > 3)
        {
            zeroTime = reader.ReadFloat();
        }

        if (revision <= 4)
        {
            camShot = new CamShot().Read(reader, false, parent, entry);
        }

        if (revision == 0xD)
        {
            oldPS3PerPixel = reader.ReadBoolean();
        }

        if (revision >= 15 && revision <= 28)
        {
            oldTriggers = AssetHelpers.ReadList(reader, () => new OldTrigger().Read(reader, UsesGreenDayLayout(parent)));
        }

        if (revision >= 16 && revision <= 28)
        {
            oldEventTrig1 = Symbol.Read(reader);
            oldEventTrig2 = Symbol.Read(reader);
        }

        if (revision > 0x11)
        {
            minTime = reader.ReadInt32();
            maxTime = reader.ReadInt32();
        }

        if (revision > 0x12 && !UsesLegoLayout(parent))
        {
            nextShots = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision >= 20 && revision <= 30 && !UsesLegoLayout(parent))
        {
            oldInt2030 = reader.ReadInt32();
        }

        if (UsesLegoLayout(parent))
        {
            fpsOverride = reader.ReadBoolean();
        }
        if (revision >= 23 && revision <= 28)
        {
            oldResetBool = reader.ReadBoolean();
        }

        if (revision == 0x1C && !UsesGreenDayLayout(parent))
        {
            oldAnims1C = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (GreenDayAltRevision(parent) > 1)
        {
            dofTint = reader.ReadBoolean();
            dofTintColor.Read(reader);
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

        if (revision > 4)
        {
            camShot.Write(writer, false, parent, entry);
        }

        AssetHelpers.WriteList(writer, targets, t => t.Write(writer, revision, UsesLegoLayout(parent), GreenDayAltRevision(parent)));

        if ((revision >= 2 && revision <= 18) || UsesLegoLayout(parent))
        {
            Symbol.Write(writer, oldNextShot);
        }

        if (revision > 3)
        {
            writer.WriteFloat(zeroTime);
        }

        if (revision <= 4)
        {
            camShot.Write(writer, false, parent, entry);
        }

        if (revision == 0xD)
        {
            writer.WriteBoolean(oldPS3PerPixel);
        }

        if (revision >= 15 && revision <= 28)
        {
            AssetHelpers.WriteList(writer, oldTriggers, t => t.Write(writer, UsesGreenDayLayout(parent)));
        }

        if (revision >= 16 && revision <= 28)
        {
            Symbol.Write(writer, oldEventTrig1);
            Symbol.Write(writer, oldEventTrig2);
        }

        if (revision > 0x11)
        {
            writer.WriteInt32(minTime);
            writer.WriteInt32(maxTime);
        }

        if (revision > 0x12 && !UsesLegoLayout(parent))
        {
            AssetHelpers.WriteList(writer, nextShots, s => Symbol.Write(writer, s));
        }

        if (revision >= 20 && revision <= 30 && !UsesLegoLayout(parent))
        {
            writer.WriteInt32(oldInt2030);
        }

        if (UsesLegoLayout(parent))
        {
            writer.WriteBoolean(fpsOverride);
        }
        if (revision >= 23 && revision <= 28)
        {
            writer.WriteBoolean(oldResetBool);
        }

        if (revision == 0x1C && !UsesGreenDayLayout(parent))
        {
            AssetHelpers.WriteList(writer, oldAnims1C, s => Symbol.Write(writer, s));
        }

        if (GreenDayAltRevision(parent) > 1)
        {
            writer.WriteBoolean(dofTint);
            dofTintColor.Write(writer);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
