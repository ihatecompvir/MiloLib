using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharBoneDir"), Description("A CharBone container, acts as a resource file, storing skeleton and DOF for particular contexts")]
public class CharBoneDir : ObjectDir
{
    public class Recenter
    {
        [Name("Targets"), Description("bones to recenter, ie, bone_pelvis")]
        public List<Symbol> targets = new();

        [Name("Averages"), Description("bones to average to find the new center")]
        public List<Symbol> averages = new();

        [Name("Slide"), Description("Slide the character over the course of the clip. If false, just uses the start of the clip")]
        public bool slide;

        public Recenter Read(EndianReader reader)
        {
            targets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            averages = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            slide = reader.ReadBoolean();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteList(writer, targets, (Symbol target) => Symbol.Write(writer, target));
            AssetHelpers.WriteList(writer, averages, (Symbol average) => Symbol.Write(writer, average));
            writer.WriteBoolean(slide);
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Move Context"), Description("context in which character should move itself around via bone_facing.pos and bone_facing.rotz bones")]
    public uint moveContext;

    [Name("Bake Out Facing"), Description("if false, won't bake out facing, will just bake out position"), MinVersion(4)]
    public bool bakeOutFacing;

    [MaxVersion(2)]
    public bool unkBool;

    [Name("Recenter"), Description("Used to limit travel. Moves [targets] bones so that the average of the [average] bones will be at (0,0,0) at the start of the clip. If slide is true evaluates the [average] bones and the start of the clip and end of the clip, and recenters [targets] smoothly between those.")]
    public Recenter recenter = new();

    public CharBoneDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new CharBoneDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision < 2)
        {
            moveContext = reader.ReadBoolean() ? 1u : 0u;
        }
        else
        {
            moveContext = reader.ReadUInt32();
        }

        if (revision < 3)
        {
            unkBool = reader.ReadBoolean();
        }

        recenter = recenter.Read(reader);

        if (revision > 3)
        {
            bakeOutFacing = reader.ReadBoolean();
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

        if (revision < 2)
        {
            writer.WriteBoolean(moveContext == 1);
        }
        else
        {
            writer.WriteUInt32(moveContext);
        }

        if (revision < 3)
        {
            writer.WriteBoolean(unkBool);
        }

        recenter.Write(writer);

        if (revision > 3)
        {
            writer.WriteBoolean(bakeOutFacing);
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
