using MiloLib.Utils;
using MiloLib.Classes;
using MiloLib.Assets.UI;

namespace MiloLib.Assets;

[Name("TrackDir"), Description("Base class for track system. Contains configuration for track speed, length, slot positions. Manages TrackWidget instances.")]
public class TrackDir : PanelDir
{
    private ushort altRevision;
    private ushort revision;

    [Name("Draw Group"), Description("Should contain everything to draw (except widget resources)"), MinVersion(1)]
    public Symbol drawGroup = Symbol.Empty;
    [Name("Anim Group"), Description("Animated at rate where frame=y position of now bar"), MinVersion(2)]
    public Symbol animGroup = Symbol.Empty;

    [Name("Y Per Second"), Description("World units widgets move per second"), MinVersion(1)]
    public float yPerSecond;
    [Name("Top Y"), Description("Distance where widgets are pushed onto track"), MinVersion(1)]
    public float topY;
    [Name("Bottom Y"), Description("Distance where widgets are pruned from track"), MinVersion(1)]
    public float bottomY;

    [Name("Slots"), Description("Offset from track center for each slot"), MinVersion(3)]
    public List<Matrix> slots = new();

    [Name("Warn On Resort"), Description("WARN if widget instances are added out of order? (can be off for prototyping)"), MinVersion(5)]
    public bool warnOnResort;

    [Name("Widget"), Description("Widget to test"), MinVersion(4)]
    public Symbol testWidget = Symbol.Empty;
    [Name("Slot"), Description("Slot to add widgets in"), MinVersion(4)]
    public int testSlot;

    [Name("Y Per Beat Easy"), Description("World units the smasher moves per beat on easy difficulty.")]
    public float yPerBeatEasy;
    [Name("Y Per Beat Medium"), Description("World units the smasher moves per beat on medium difficulty.")]
    public float yPerBeatMedium;
    [Name("Y Per Beat Hard"), Description("World units the smasher moves per beat on hard difficulty.")]
    public float yPerBeatHard;
    [Name("Y Per Beat Expert"), Description("World units the smasher moves per beat on expert difficulty.")]
    public float yPerBeatExpert;
    [Name("World Camera")]
    public Symbol worldCamera = Symbol.Empty;
    [Name("Smasher Transform")]
    public Symbol smasherTrans = Symbol.Empty;

    public TrackDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new TrackDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (parent.revision == 32 && revision == 12)
        {
            if (!entry.isProxy)
            {
                yPerBeatEasy = reader.ReadFloat();
                yPerBeatMedium = reader.ReadFloat();
                yPerBeatHard = reader.ReadFloat();
                yPerBeatExpert = reader.ReadFloat();
                worldCamera = Symbol.Read(reader);
                smasherTrans = Symbol.Read(reader);
            }
            if (standalone)
            {
                AssetHelpers.ReadEnd(reader, parent, entry, revision);
            }

            return this;
        }

        if (!entry.isProxy)
        {
            if (revision != 0)
            {
                drawGroup = Symbol.Read(reader);
                if (revision > 1)
                {
                    animGroup = Symbol.Read(reader);
                }

                yPerSecond = reader.ReadFloat();
                topY = reader.ReadFloat();
                bottomY = reader.ReadFloat();
            }
            if (revision > 2)
            {
                if (revision > 5)
                {
                    slots = AssetHelpers.ReadList(reader, () => new Matrix().Read(reader));
                }
                else
                {
                    // i think this is right?
                    slots = AssetHelpers.ReadList(reader, () => new Matrix { m41 = reader.ReadFloat(), m43 = reader.ReadFloat() });
                }
            }
            if (revision > 4)
            {
                warnOnResort = reader.ReadBoolean();
            }

            if (revision > 3)
            {
                testWidget = Symbol.Read(reader);
                testSlot = reader.ReadInt32();
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

        if (parent.revision == 32 && revision == 12)
        {
            if (!entry.isProxy)
            {
                writer.WriteFloat(yPerBeatEasy);
                writer.WriteFloat(yPerBeatMedium);
                writer.WriteFloat(yPerBeatHard);
                writer.WriteFloat(yPerBeatExpert);
                Symbol.Write(writer, worldCamera);
                Symbol.Write(writer, smasherTrans);
            }
            if (standalone)
            {
                writer.WriteEndBytes();
            }

            return;
        }

        if (!entry.isProxy)
        {
            if (revision != 0)
            {
                Symbol.Write(writer, drawGroup);
                if (revision > 1)
                {
                    Symbol.Write(writer, animGroup);
                }

                writer.WriteFloat(yPerSecond);
                writer.WriteFloat(topY);
                writer.WriteFloat(bottomY);
            }
            if (revision > 2)
            {
                if (revision > 5)
                {
                    AssetHelpers.WriteList(writer, slots, slot => slot.Write(writer));
                }
                else
                {
                    AssetHelpers.WriteList(writer, slots, slot => { writer.WriteFloat(slot.m41); writer.WriteFloat(slot.m43); });
                }
            }
            if (revision > 4)
            {
                writer.WriteBoolean(warnOnResort);
            }

            if (revision > 3)
            {
                Symbol.Write(writer, testWidget);
                writer.WriteInt32(testSlot);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }

}
