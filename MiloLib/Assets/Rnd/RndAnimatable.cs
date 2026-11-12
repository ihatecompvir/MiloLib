using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Animatable"), Description("Base class for animatable objects. Anim objects change their state or other objects.")]
public class RndAnimatable
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        { Game.MiloGame.Amplitude, 0 },
        { Game.MiloGame.EyeToyAntiGrav, 0 },
        { Game.MiloGame.GuitarHero, 0 },
        { Game.MiloGame.GuitarHero2_PS2, 4 },
        { Game.MiloGame.GuitarHero2_360, 4 },
        { Game.MiloGame.Phase, 4 },
        { Game.MiloGame.RockBand, 4 },
        { Game.MiloGame.RockBand2, 4 },
        { Game.MiloGame.LegoRockBand, 4 },
        { Game.MiloGame.TheBeatlesRockBand, 4 },
        { Game.MiloGame.GreenDayRockBand, 4 },
        { Game.MiloGame.RockBand3, 4 },
        { Game.MiloGame.DanceCentral, 4 },
        { Game.MiloGame.DanceCentral2, 4 },
        { Game.MiloGame.RockBandBlitz, 4 },
        { Game.MiloGame.DanceCentral3, 4 }
    };
    public class AnimEntry
    {
        public int operation;
        public float scale = 1.0f;
        public float offset;
        public float minFrame;
        public float maxFrame;
        public bool loop;
        public int oldValue1;
        public int oldValue2;
        public int oldValue3;

        public AnimEntry Read(EndianReader reader)
        {
            operation = reader.ReadInt32();
            switch (operation)
            {
                case 0:
                    scale = reader.ReadFloat();
                    offset = reader.ReadFloat();
                    break;
                case 1:
                    minFrame = reader.ReadFloat();
                    maxFrame = reader.ReadFloat();
                    loop = reader.ReadBoolean();
                    break;
                case 2:
                case 3:
                    oldValue1 = reader.ReadInt32();
                    oldValue2 = reader.ReadInt32();
                    break;
                case 4:
                    oldValue1 = reader.ReadInt32();
                    oldValue2 = reader.ReadInt32();
                    oldValue3 = reader.ReadInt32();
                    break;
            }
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32(operation);
            switch (operation)
            {
                case 0:
                    writer.WriteFloat(scale);
                    writer.WriteFloat(offset);
                    break;
                case 1:
                    writer.WriteFloat(minFrame);
                    writer.WriteFloat(maxFrame);
                    writer.WriteBoolean(loop);
                    break;
                case 2:
                case 3:
                    writer.WriteInt32(oldValue1);
                    writer.WriteInt32(oldValue2);
                    break;
                case 4:
                    writer.WriteInt32(oldValue1);
                    writer.WriteInt32(oldValue2);
                    writer.WriteInt32(oldValue3);
                    break;
            }
        }
    }
    public enum Rate
    {
        k30_fps,
        k480_fpb,
        k30_fps_ui,
        k1_fpb,
        k30_fps_tutorial
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Frame"), Description("Frame of animation"), MinVersion(2)]
    public float frame;

    [Name("Rate"), Description("Rate to animate")]
    public Rate rate;

    [MaxVersion(0)]
    public List<AnimEntry> animEntries = new();

    [MaxVersion(0)]
    public List<Symbol> anims = new();

    public override string ToString()
    {
        return $"RndAnimatable: revs({revision}, {altRevision})\tframe: {frame}, rate {rate}\n";
    }

    public RndAnimatable Read(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision > 1)
        {
            frame = reader.ReadFloat();
        }

        if (revision < 4)
        {
            if (revision > 2)
            {
                byte uc = reader.ReadByte();
                rate = uc == 0 ? Rate.k480_fpb : Rate.k30_fps;
            }
        }
        else
        {
            rate = (Rate)reader.ReadUInt32();
            return this;
        }

        if (revision < 1)
        {
            animEntries = AssetHelpers.ReadList(reader, () => new AnimEntry().Read(reader));

            anims = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        return this;
    }

    public void Write(EndianWriter writer)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);
        if (revision > 1)
        {
            writer.WriteFloat(frame);
        }

        if (revision < 4)
        {
            if (revision > 2)
            {
                writer.WriteByte((byte)(rate == Rate.k480_fpb ? 0 : 1));
            }
        }
        else
        {
            writer.WriteUInt32((uint)rate);
            return;
        }

        if (revision < 1)
        {
            AssetHelpers.WriteList(writer, animEntries, entry => entry.Write(writer));

            AssetHelpers.WriteList(writer, anims, anim => Symbol.Write(writer, anim));
        }
    }

    public static RndAnimatable New(ushort revision, ushort altRevision)
    {
        RndAnimatable anim = new();
        anim.revision = revision;
        anim.altRevision = altRevision;
        return anim;
    }
}
