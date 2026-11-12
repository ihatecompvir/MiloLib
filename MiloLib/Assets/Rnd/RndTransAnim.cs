using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.Rnd.PropKey;

namespace MiloLib.Assets.Rnd;

[Name("TransAnim"), Description("TransAnim objects animate the position, rotation, and scale of transformable objects.")]
public class RndTransAnim : Object
{
    private ushort altRevision;
    private ushort revision;

    public RndAnimatable anim = new();

    public Symbol trans = Symbol.Empty;
    public Symbol keysOwner = Symbol.Empty;

    [MaxVersion(5)]
    public RndDrawable draw = new();

    public List<QuatKey> rotKeys = new();

    public List<Vec3Key> transKeys = new();

    public List<Vec3Key> scaleKeys = new();

    public bool transSpline;
    public bool scaleSpline;
    public bool rotSlerp;
    public bool rotSpline;
    public bool repeatTrans;
    public int unusedPreRev3;
    public bool followPath;
    private byte[] gh2OpaqueBytes = Array.Empty<byte>();

    public new RndTransAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (parent.revision <= 25 && standalone && (revision < 6 || revision > 7))
        {
            gh2OpaqueBytes = BinaryParsingUtils.ReadUntilEndMarkerAllocating(reader);
            return this;
        }

        if (revision > 4)
        {
            base.Read(reader, false, parent, entry);
        }

        anim = anim.Read(reader, parent, entry);

        if (revision < 6)
        {
            draw = draw.Read(reader, false, parent, entry);
        }

        trans = Symbol.Read(reader);

        if (revision > 2)
        {
            rotKeys = AssetHelpers.ReadList(reader, () => new QuatKey().Read(reader));

            transKeys = AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader));
        }

        keysOwner = Symbol.Read(reader);

        if (revision < 3)
        {
            transKeys = AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader));

            rotKeys = AssetHelpers.ReadList(reader, () => new QuatKey().Read(reader));

            unusedPreRev3 = reader.ReadInt32();
        }

        if (revision > 3)
        {
            transSpline = reader.ReadBoolean();
        }
        else
        {
            transSpline = reader.ReadInt32() != 0;
        }

        repeatTrans = reader.ReadBoolean();

        if (revision > 3)
        {
            scaleKeys = AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader));
            scaleSpline = reader.ReadBoolean();
        }
        else if (revision > 0)
        {
            if (revision != 2)
            {
                scaleKeys = AssetHelpers.ReadList(reader, () => new Vec3Key().Read(reader));
            }
            if (revision < 3)
            {
                // todo
            }
            scaleSpline = reader.ReadInt32() != 0;
        }

        if (revision > 1)
        {
            followPath = reader.ReadBoolean();
        }
        else
        {
            followPath = (rotKeys.Count == 0 && transKeys.Count > 1);
        }

        if (revision > 3)
        {
            rotSlerp = reader.ReadBoolean();
        }

        if (revision > 6)
        {
            rotSpline = reader.ReadBoolean();
        }

        if (standalone)
        {
            if ((reader.Endianness == Endian.BigEndian ? 0xADDEADDE : 0xDEADDEAD) != reader.ReadUInt32())
            {
                throw new Exception("Did not find end bytes, read likely did not succeed");
            }
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (parent.revision <= 25 && standalone && (revision < 6 || revision > 7))
        {
            writer.WriteBlock(gh2OpaqueBytes);
            writer.WriteEndBytes();
            return;
        }

        if (revision > 4)
        {
            base.Write(writer, false, parent, entry);
        }

        anim.Write(writer);

        if (revision < 6)
        {
            draw.Write(writer, false, parent);
        }

        Symbol.Write(writer, trans);

        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, rotKeys, qk => qk.Write(writer));

            AssetHelpers.WriteList(writer, transKeys, vk => vk.Write(writer));
        }

        Symbol.Write(writer, keysOwner);

        if (revision < 3)
        {
            AssetHelpers.WriteList(writer, transKeys, vk => vk.Write(writer));

            AssetHelpers.WriteList(writer, rotKeys, qk => qk.Write(writer));

            writer.WriteInt32(unusedPreRev3);
        }

        if (revision > 3)
        {
            writer.WriteBoolean(transSpline);
        }
        else
        {
            writer.WriteInt32(transSpline ? 1 : 0);
        }

        writer.WriteBoolean(repeatTrans);

        if (revision > 3)
        {
            AssetHelpers.WriteList(writer, scaleKeys, vk => vk.Write(writer));

            writer.WriteBoolean(scaleSpline);
        }
        else if (revision > 0)
        {
            if (revision != 2)
            {
                AssetHelpers.WriteList(writer, scaleKeys, vk => vk.Write(writer));
            }
            if (revision < 3)
            {
                // todo
            }
            writer.WriteInt32(scaleSpline ? 1 : 0);
        }

        if (revision > 1)
        {
            writer.WriteBoolean(followPath);
        }

        if (revision > 3)
        {
            writer.WriteBoolean(rotSlerp);
        }

        if (revision > 6)
        {
            writer.WriteBoolean(rotSpline);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
