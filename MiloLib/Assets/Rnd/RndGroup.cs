using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Group"), Description("Represents a group of objects to which to propogate animation and messages.")]
public class RndGroup : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    public RndTrans trans = new();

    public RndDrawable draw = new();

    [Name("Objects"), Description("Objects in the group, in order of drawing and polling"), MinVersion(11)]
    public List<Symbol> objects = new();

    [Name("Environ"), MinVersion(11), MaxVersion(15)]
    public Symbol environ = Symbol.Empty;

    [Name("Draw Only"), Description("If set, only draws this member of the group"), MinVersion(13)]
    public Symbol drawOnly = Symbol.Empty;

    [Name("LOD"), Description("Object to draw instead below lod_screen_size"), MinVersion(12), MaxVersion(15)]
    public Symbol lod = Symbol.Empty;

    [Name("LOD Screen Size"), Description("Ratio of screen height for lod"), MinVersion(12), MaxVersion(15)]
    public float lodScreenSize;

    [Name("Sort In World"), Description("Sort by distance to current camera per frame. This has a CPU cost if there are many objects."), MinVersion(14)]
    public bool sortInWorld;

    [MinVersion(7), MaxVersion(7)]
    public Symbol unkSymbol = Symbol.Empty;
    [MinVersion(7), MaxVersion(7)]
    public float lodWidth;
    [MinVersion(7), MaxVersion(7)]
    public float lodHeight;

    public uint unkRev4_0;
    public uint unkRev4_1;
    public uint unkRev4_2;

    public uint phaseTail1;
    public uint phaseTail2;

    public new RndGroup Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 7)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = new RndAnimatable().Read(reader, parent, entry);
        trans = new RndTrans().Read(reader, false, parent, entry);
        draw = new RndDrawable().Read(reader, false, parent, entry);

        if (revision > 10)
        {
            objects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            if (parent.revision == 26 && revision == 13)
            {
                phaseTail1 = reader.ReadUInt32();
                phaseTail2 = reader.ReadUInt32();
                if (standalone)
                {
                    AssetHelpers.ReadEnd(reader, parent, entry, revision);
                }

                return this;
            }

            if (revision < 16)
            {
                environ = Symbol.Read(reader);
            }

            if (revision > 12)
            {
                drawOnly = Symbol.Read(reader);
            }
        }

        if (revision > 11 && revision < 16)
        {
            lod = Symbol.Read(reader);
            lodScreenSize = reader.ReadFloat();
        }
        else if (revision == 4)
        {
            unkRev4_0 = reader.ReadUInt32();

            objects = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            unkSymbol = Symbol.Read(reader);
            unkRev4_1 = reader.ReadUInt32();
            unkRev4_2 = reader.ReadUInt32();
        }

        if (revision == 7)
        {
            unkSymbol = Symbol.Read(reader);
            lodWidth = reader.ReadFloat();
            lodHeight = reader.ReadFloat();
        }

        if (revision > 13)
        {
            sortInWorld = reader.ReadBoolean();
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

        if (revision > 7)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);
        trans.Write(writer, false, parent, null);
        draw.Write(writer, false, parent, null);

        if (revision > 10)
        {
            AssetHelpers.WriteList(writer, objects, obj => Symbol.Write(writer, obj));

            if (parent.revision == 26 && revision == 13)
            {
                writer.WriteUInt32(phaseTail1);
                writer.WriteUInt32(phaseTail2);
                if (standalone)
                {
                    writer.WriteEndBytes();
                }

                return;
            }

            if (revision < 16)
            {
                Symbol.Write(writer, environ);
            }

            if (revision > 12)
            {
                Symbol.Write(writer, drawOnly);
            }
        }

        if (revision > 11 && revision < 16)
        {
            Symbol.Write(writer, lod);
            writer.WriteFloat(lodScreenSize);
        }
        else if (revision == 4)
        {
            writer.WriteUInt32(unkRev4_0);

            AssetHelpers.WriteList(writer, objects, obj => Symbol.Write(writer, obj));

            Symbol.Write(writer, unkSymbol);
            writer.WriteUInt32(unkRev4_1);
            writer.WriteUInt32(unkRev4_2);
        }

        if (revision == 7)
        {
            Symbol.Write(writer, unkSymbol);
            writer.WriteFloat(lodWidth);
            writer.WriteFloat(lodHeight);
        }

        if (revision > 13)
        {
            writer.WriteBoolean(sortInWorld);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public static RndGroup New(ushort revision, ushort altRevision)
    {
        RndGroup rndGroup = new();
        rndGroup.revision = revision;
        rndGroup.altRevision = altRevision;
        return rndGroup;
    }
}
