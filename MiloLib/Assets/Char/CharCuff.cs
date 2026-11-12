using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharCuff"), Description("A cuff used to constrain colliding outfits against each other. for example boots against pants. The widest cuff wins")]
public class CharCuff : Object
{
    public struct CuffShape
    {
        public float radius;

        public float offset;

        public void Read(EndianReader reader)
        {
            radius = reader.ReadFloat();
            offset = reader.ReadFloat();
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteFloat(radius);
            writer.WriteFloat(offset);
        }
    }

    private ushort altRevision;
    private ushort revision;

    public RndTrans trans = new();

    public CuffShape shape0;

    public CuffShape shape1;

    public CuffShape shape2;

    [Name("Outer Radius"), Description("Outside radius, should encompass the biggest thing on the outside, biggest one wins. For incompressible things like big boots should be the biggest part. For soft things like cloth should just be radius1"), MinVersion(2)]
    public float outerRadius;

    [Name("Open End"), Description("Is the inside open or closed, open is good for things like gauntlets"), MinVersion(3)]
    public bool openEnd;

    [Name("Bone"), Description("The bone of interest, like bone_R-knee for boot and pant cuffs"), MinVersion(4)]
    public Symbol bone = Symbol.Empty;

    [Name("Eccentricity"), Description("How much smaller to make the radius along Y, must be < 1"), MinVersion(5)]
    public float eccentricity;

    [Name("Category"), Description("The outfit category to cuff against, must be set to work"), MinVersion(6)]
    public Symbol category = Symbol.Empty;

    [Name("Ignore"), Description("meshes to never deform"), MinVersion(8)]
    public List<Symbol> ignore = new();

    public new CharCuff Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        trans.Read(reader, false, parent, entry);

        shape0.Read(reader);
        shape1.Read(reader);
        shape2.Read(reader);

        if (revision > 1)
        {
            outerRadius = reader.ReadFloat();
        }

        if (revision > 2)
        {
            openEnd = reader.ReadBoolean();
        }

        if (revision > 3)
        {
            bone = Symbol.Read(reader);
        }

        if (revision > 4)
        {
            eccentricity = reader.ReadFloat();
        }

        if (revision > 5)
        {
            category = Symbol.Read(reader);
        }

        if (revision > 7)
        {
            ignore = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
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
        trans.Write(writer, false, parent, entry);

        shape0.Write(writer);
        shape1.Write(writer);
        shape2.Write(writer);

        if (revision > 1)
        {
            writer.WriteFloat(outerRadius);
        }

        if (revision > 2)
        {
            writer.WriteBoolean(openEnd);
        }

        if (revision > 3)
        {
            Symbol.Write(writer, bone);
        }

        if (revision > 4)
        {
            writer.WriteFloat(eccentricity);
        }

        if (revision > 5)
        {
            Symbol.Write(writer, category);
        }

        if (revision > 7)
        {
            AssetHelpers.WriteList(writer, ignore, mesh => Symbol.Write(writer, mesh));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
