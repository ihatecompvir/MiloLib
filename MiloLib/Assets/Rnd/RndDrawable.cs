using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Draw"), Description("Base class for drawable objects. Draw objects either render polys or determine rendering state.")]
public class RndDrawable : Object
{
    public enum OverrideIncludeInDepthOnlyPass
    {
        kOverrideIncludeInDepthOnlyPass_None,
        kOverrideIncludeInDepthOnlyPass_Include,
        kOverrideIncludeInDepthOnlyPass_DontInclude
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Showing"), Description("Whether the object and its Draw children are drawn or collided with.")]
    public bool showing = true;

    [Name("Sphere"), Description("bounding sphere"), MinVersion(1)]
    public Sphere sphere = new();

    [Name("Draw Order"), Description("Draw order within proxies, lower numbers are drawn first, so assign numbers from the outside-in (unless translucent), to minimize overdraw. In groups, draw_order will be ignored unless you explicitly click the sort button."), MinVersion(3)]
    public float drawOrder;

    [Name("Override Include In Depth Only Pass"), Description("Whether or not this drawable overrides its default (usually for the class) value of being included in the depth only pass on Xbox 360"), MinVersion(4)]
    public OverrideIncludeInDepthOnlyPass overrideIncludeInDepthOnlyPass;

    [MaxVersion(1)]
    public List<Symbol> drawables = new();
    [MaxVersion(1)]
    public List<string> drawablesNullTerminated = new();

    [Name("Clip Planes"), Description("List of up to 6 transformable objects which specify user-defined clip planes (for a given trans, the plane will run through the trans position, and use the rotated z-axis for the plane normal)."), MinVersion(4)]
    public List<Symbol> clipPlanes = new();

    public new RndDrawable Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        showing = reader.ReadBoolean();

        if (revision < 2)
        {
            if (parent.revision <= 6)
            {
                drawablesNullTerminated = AssetHelpers.ReadList(reader, () => reader.ReadUTF8());
            }
            else
            {
                drawables = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
        }

        if (revision > 0)
        {
            sphere = new Sphere().Read(reader);
        }
        if (revision > 2)
        {
            drawOrder = reader.ReadFloat();
        }

        if (revision >= 4 && parent.revision == 25)
        {
            overrideIncludeInDepthOnlyPass = (OverrideIncludeInDepthOnlyPass)reader.ReadUInt32();
        }
        else if (revision >= 4)
        {
            clipPlanes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public new void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry = null)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        writer.WriteBoolean(showing);

        if (revision < 2)
        {
            if (parent.revision <= 6)
            {
                AssetHelpers.WriteList(writer, drawablesNullTerminated, drawable => writer.WriteUTF8(drawable));
            }
            else
            {
                AssetHelpers.WriteList(writer, drawables, drawable => Symbol.Write(writer, drawable));
            }
        }

        if (revision > 0)
        {
            sphere.Write(writer);
        }
        if (revision > 2)
        {
            writer.WriteFloat(drawOrder);
        }

        if (revision >= 4 && parent.revision == 25)
        {
            writer.WriteUInt32((uint)overrideIncludeInDepthOnlyPass);
        }
        else if (revision >= 4)
        {
            AssetHelpers.WriteList(writer, clipPlanes, (Symbol plane) => Symbol.Write(writer, plane));
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public static RndDrawable New(ushort revision, ushort altRevision)
    {
        RndDrawable drawable = new();
        drawable.revision = revision;
        drawable.altRevision = altRevision;
        return drawable;
    }
}
