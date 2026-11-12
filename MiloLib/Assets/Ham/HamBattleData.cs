using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

struct Range
{
    int start;
    int end;

    public Range Read(EndianReader reader)
    {
        start = reader.ReadInt32();
        end = reader.ReadInt32();
        return this;
    }

    public void Write(EndianWriter writer)
    {
        writer.WriteInt32(start);
        writer.WriteInt32(end);
    }
    [Name("HamBattleData"), Description("Data used to construct a Dance Battle")]
    public class HamBattleData : Object
    {

        private ushort altRevision;
        private ushort revision;

        public class BattleStep
        {
            [Name("State"), Description("What's going on during this section of the battle. Options are: normal, minigame"), MinVersion(4)]
            Symbol state = Symbol.Empty;
            [Name("Player Flags"), Description("Which players are involved with this section")]
            PlayerFlags players;
            [Name("Music Range"), Description("Music loop start and end")]
            Range musicRange;
            [Name("Play Range"), Description("Playable range of the section")]
            Range playRange;
            [Name("Cam"), Description("Which camera cut to use for this section. Options are: '', Area1_NEAR, Area1_FAR, Area1_MOVEMENT, Area2_NEAR, Area2_FAR, Area2_MOVEMENT, DC_PLAYER_FREESTYLE"), MinVersion(2)]
            Symbol cam = Symbol.Empty;
            [Name("Non-play Action"), Description("What the non-dancer is doing. Options are: idle, dance, hide"), MinVersion(3)]
            Symbol nonplayAction = Symbol.Empty;

            public BattleStep Read(EndianReader reader, uint stepRev)
            {
                players = (PlayerFlags)reader.ReadUInt32();
                musicRange.Read(reader);
                playRange.Read(reader);
                if (stepRev > 1)
                {
                    cam = Symbol.Read(reader);
                }

                if (stepRev > 2)
                {
                    nonplayAction = Symbol.Read(reader);
                }

                if (stepRev > 3)
                {
                    state = Symbol.Read(reader);
                }

                return this;
            }

            public void Write(EndianWriter writer, uint stepRev)
            {
                writer.WriteUInt32((uint)players);
                musicRange.Write(writer);
                playRange.Write(writer);
                if (stepRev > 1)
                {
                    Symbol.Write(writer, cam);
                }

                if (stepRev > 2)
                {
                    Symbol.Write(writer, nonplayAction);
                }

                if (stepRev > 3)
                {
                    Symbol.Write(writer, state);
                }
            }
        }

        [Name("Steps"), Description("Steps for the dance battle")]
        public List<BattleStep> battleSteps = new();

        public new HamBattleData Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
        {
            AssetHelpers.ReadRevision(reader, out revision, out altRevision);

            base.Read(reader, false, parent, entry);

            battleSteps = AssetHelpers.ReadList(reader, () => new BattleStep().Read(reader, revision));

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

            AssetHelpers.WriteList(writer, battleSteps, (BattleStep step) => step.Write(writer, revision));

            if (standalone)
            {
                writer.WriteEndBytes();
            }
        }
    }
}
