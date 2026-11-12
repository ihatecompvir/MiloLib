using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("HamCamTransform"), Description("Used to parent camera shots to separate areas. This allows camera shots to be shared between venues.")]
public class HamCamTransform : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Areas")]
    public List<TransformArea> areas = new();

    public class TransformCrowd
    {
        [Name("Crowd"), Description("The crowd to show for this shot")]
        public Symbol crowd = Symbol.Empty;
        [Name("Crowd Rotate"), Description("How to rotate crowd")]
        public int crowdRotate;

        public TransformCrowd Read(EndianReader reader)
        {
            crowd = Symbol.Read(reader);
            crowdRotate = reader.ReadInt32();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, crowd);
            writer.WriteInt32(crowdRotate);
        }
    }

    public class TransformArea
    {
        [Name("Area"), Description("New origin for area")]
        public Symbol area = Symbol.Empty;
        [Name("Camshots"), Description("Camera shots to be moved to this area")]
        public List<Symbol> camshots = new();
        [Name("Anims"), Description("Anim to be run for these cam shots")]
        public List<Symbol> anims = new();
        [Name("Crowds")]
        public List<TransformCrowd> crowds = new();
        [Name("Flow"), Description("Flow to execute for the setup of these camshots")]
        public Symbol flow = Symbol.Empty;

        public TransformArea Read(EndianReader reader, ushort revision)
        {
            area = Symbol.Read(reader);
            camshots = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            anims = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            if (revision > 1)
            {
                crowds = AssetHelpers.ReadList(reader, () => new TransformCrowd().Read(reader));
            }
            if (revision > 2)
            {
                flow = Symbol.Read(reader);
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, area);
            AssetHelpers.WriteList(writer, camshots, shot => Symbol.Write(writer, shot));
            AssetHelpers.WriteList(writer, anims, anim => Symbol.Write(writer, anim));
            if (revision > 1)
            {
                AssetHelpers.WriteList(writer, crowds, crowd => crowd.Write(writer));
            }
            if (revision > 2)
            {
                Symbol.Write(writer, flow);
            }
        }
    }

    public new HamCamTransform Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        areas = AssetHelpers.ReadList(reader, () => new TransformArea().Read(reader, revision));
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
        AssetHelpers.WriteList(writer, areas, area => area.Write(writer, revision));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
