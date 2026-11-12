using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharBone"), Description("A Character system bone, conveys degrees of freedom to animate in a CharClip, lives in CharBoneDir")]
public class CharBone : Object
{
    public class WeightContext
    {
        [Name("Context"), Description("context for this bone weight")]
        public int context;

        [Name("Weight"), Description("bone weight for this context")]
        public float weight;

        public WeightContext Read(EndianReader reader)
        {
            context = reader.ReadInt32();
            weight = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32(context);
            writer.WriteFloat(weight);
        }
    }

    private ushort altRevision;
    private ushort revision;

    private RndTrans embeddedTrans = new();

    [Name("Position Context"), Description("when to animate position")]
    public int positionContext;

    [Name("Scale Context"), Description("when to animate scale")]
    [MinVersion(2)]
    public int scaleContext;

    [Name("Rotation"), Description("what axis or axes to animate")]
    public int rotation;

    [Name("Rotation Context"), Description("when to use this rotation context")]
    [MinVersion(7)]
    public int rotationContext;

    [MinVersion(4)]
    [Name("Target"), Description("If set will exactly track this target during keyframe acquisition, used for IK bones")]
    public Symbol target = Symbol.Empty;

    [MinVersion(8)]
    [Name("Weights"), Description("array of contexts and corresponding bone weights")]
    public List<WeightContext> weights = new();

    [MinVersion(9)]
    [Name("Trans"), Description("Object to specify trans info")]
    public Symbol trans = Symbol.Empty;

    [MinVersion(10)]
    [Name("Bake Out As Top Level"), Description("When baking out facing, use this as a top-level bone (like the pelvis)")]
    public bool bakeOutAsTopLevel;

    private int discardedInt1;
    private int discardedInt2;
    private int ctxRedistribution;

    public new CharBone Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (parent.revision == 25 && revision == 1)
        {
            if (standalone)
            {
                uint expectedEndBytes;
                if (reader.Endianness == Endian.BigEndian)
                {
                    expectedEndBytes = 0xADDEADDE;
                }
                else
                {
                    expectedEndBytes = 0xDEADDEAD;
                }
                if (expectedEndBytes != reader.ReadUInt32())
                {
                    throw MiloLib.Exceptions.MiloAssetReadException.EndBytesNotFound(parent, entry, reader.BaseStream.Position, revision);
                }
            }
            return this;
        }

        if (revision < 9)
        {
            embeddedTrans.Read(reader, false, parent, entry);
        }

        if (revision > 6)
        {
            positionContext = reader.ReadInt32();
        }
        else
        {
            positionContext = reader.ReadBoolean() ? 1 : 0;
        }

        if (revision > 6)
        {
            scaleContext = reader.ReadInt32();
        }
        else if (revision > 1)
        {
            scaleContext = reader.ReadBoolean() ? 1 : 0;
        }

        rotation = reader.ReadInt32();

        if (revision < 5)
        {
            discardedInt1 = reader.ReadInt32();
        }

        if (revision > 6)
        {
            rotationContext = reader.ReadInt32();
        }

        if (revision >= 3 && revision <= 7)
        {
            discardedInt2 = reader.ReadInt32();
        }

        if (revision > 3)
        {
            target = Symbol.Read(reader);
        }

        if (revision == 6)
        {
            ctxRedistribution = reader.ReadInt32();
        }

        if (revision > 7)
        {
            weights = AssetHelpers.ReadList(reader, () => new WeightContext().Read(reader));
        }

        if (revision > 8)
        {
            trans = Symbol.Read(reader);
        }

        if (revision > 9)
        {
            bakeOutAsTopLevel = reader.ReadBoolean();
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

        if (parent.revision == 25 && revision == 1)
        {
            if (standalone)
            {
                writer.WriteEndBytes();
            }
            return;
        }

        if (revision < 9)
        {
            embeddedTrans.Write(writer, false, parent, entry);
        }

        if (revision > 6)
        {
            writer.WriteInt32(positionContext);
        }
        else
        {
            writer.WriteBoolean(positionContext != 0);
        }

        if (revision > 6)
        {
            writer.WriteInt32(scaleContext);
        }
        else if (revision > 1)
        {
            writer.WriteBoolean(scaleContext != 0);
        }

        writer.WriteInt32(rotation);

        if (revision < 5)
        {
            writer.WriteInt32(discardedInt1);
        }

        if (revision > 6)
        {
            writer.WriteInt32(rotationContext);
        }

        if (revision >= 3 && revision <= 7)
        {
            writer.WriteInt32(discardedInt2);
        }

        if (revision > 3)
        {
            Symbol.Write(writer, target);
        }

        if (revision == 6)
        {
            writer.WriteInt32(ctxRedistribution);
        }

        if (revision > 7)
        {
            AssetHelpers.WriteList(writer, weights, wc => wc.Write(writer));
        }

        if (revision > 8)
        {
            Symbol.Write(writer, trans);
        }

        if (revision > 9)
        {
            writer.WriteBoolean(bakeOutAsTopLevel);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
