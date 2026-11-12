using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("SkeletonClip"), Description("An animated clip of a skeleton playable in milo")]
public class SkeletonClip : Object
{
    public class MoveRating
    {
        [Name("Move Name"), Description("Name of the HamMove")]
        public Symbol name = Symbol.Empty;
        [Name("Expected"), Description("Expected rating for this move")]
        public Symbol expected = Symbol.Empty;
        public int weighted;
        public Symbol oldWeighted = Symbol.Empty;

        public MoveRating Read(EndianReader reader, ushort revision, bool weightedIntegers)
        {
            name = Symbol.Read(reader);
            expected = Symbol.Read(reader);
            if (weightedIntegers)
            {
                weighted = reader.ReadInt32();
            }
            else if (revision > 8)
            {
                oldWeighted = Symbol.Read(reader);
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision, bool weightedIntegers)
        {
            Symbol.Write(writer, name);
            Symbol.Write(writer, expected);
            if (weightedIntegers)
            {
                writer.WriteInt32(weighted);
            }
            else if (revision > 8)
            {
                Symbol.Write(writer, oldWeighted);
            }
        }
    }

    private ushort revision;
    private ushort altRevision;
    public RndAnimatable anim = new();
    [Name("File"), Description("associated data file")]
    public Symbol file = Symbol.Empty;
    [Name("Move Ratings"), MinVersion(8)]
    public List<MoveRating> moveRatings = new();
    [MinVersion(1)]
    public int frameCounter;
    [Name("Autoplay"), Description("Automatically animate skeleton data regardless of the current frame."), MinVersion(1)]
    public bool autoplay;
    [MinVersion(1), MaxVersion(3)]
    public bool oldAutoplay;
    [MinVersion(2), MaxVersion(6)]
    public int oldValue1;
    [MinVersion(2), MaxVersion(6)]
    public int oldValue2;
    [Name("Default Rating"), Description("Default expected move rating when simulating playback"), MinVersion(6)]
    public Symbol defaultRating = Symbol.Empty;
    [Name("Weighted"), Description("Is this clip used in the filter weighting?")]
    public int weighted;
    [MinVersion(9)]
    public Symbol oldWeighted = Symbol.Empty;

    private bool UsesWeightedIntegers()
    {
        return altRevision > 0 || revision == 10;
    }

    public new SkeletonClip Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 10);
        anim.Read(reader, parent, entry);
        base.Read(reader, false, parent, entry);
        file = Symbol.Read(reader);
        if (revision > 7)
        {
            moveRatings = AssetHelpers.ReadList(reader, () => new MoveRating().Read(reader, revision, UsesWeightedIntegers()));
        }
        if (revision > 0)
        {
            frameCounter = reader.ReadInt32();
            autoplay = reader.ReadBoolean();
            if (revision < 4)
            {
                oldAutoplay = reader.ReadBoolean();
            }
        }
        if (revision > 1 && revision < 7)
        {
            oldValue1 = reader.ReadInt32();
            oldValue2 = reader.ReadInt32();
        }
        if (revision > 5)
        {
            defaultRating = Symbol.Read(reader);
        }
        if (UsesWeightedIntegers())
        {
            weighted = reader.ReadInt32();
        }
        else if (revision > 8)
        {
            oldWeighted = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 10);
        anim.Write(writer);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, file);
        if (revision > 7)
        {
            AssetHelpers.WriteList(writer, moveRatings, value => value.Write(writer, revision, UsesWeightedIntegers()));
        }
        if (revision > 0)
        {
            writer.WriteInt32(frameCounter);
            writer.WriteBoolean(autoplay);
            if (revision < 4)
            {
                writer.WriteBoolean(oldAutoplay);
            }
        }
        if (revision > 1 && revision < 7)
        {
            writer.WriteInt32(oldValue1);
            writer.WriteInt32(oldValue2);
        }
        if (revision > 5)
        {
            Symbol.Write(writer, defaultRating);
        }
        if (UsesWeightedIntegers())
        {
            writer.WriteInt32(weighted);
        }
        else if (revision > 8)
        {
            Symbol.Write(writer, oldWeighted);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
