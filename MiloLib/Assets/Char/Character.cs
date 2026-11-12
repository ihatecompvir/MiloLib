using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("Character"), Description("Base class for Character objects. Contains Geometry, Outfit Loaders, and LOD + Sphere concepts.")]
public class Character : RndDir
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        // no dirs before this
        { Game.MiloGame.GuitarHero2_PS2, 9 },
        { Game.MiloGame.GuitarHero2_360, 10 },
        { Game.MiloGame.RockBand, 12 },
        { Game.MiloGame.RockBand2, 12 },
        { Game.MiloGame.LegoRockBand, 12 },
        { Game.MiloGame.TheBeatlesRockBand, 15 },
        { Game.MiloGame.GreenDayRockBand, 15 },
        { Game.MiloGame.RockBand3, 18 },
        { Game.MiloGame.DanceCentral, 18 },
        { Game.MiloGame.DanceCentral2, 18 },
        { Game.MiloGame.RockBandBlitz, 21 },
        { Game.MiloGame.DanceCentral3, 21 }
    };

    public class LOD
    {
        [Name("Screen Size"), Description("when the unit sphere centered on the bounding sphere is smaller than this screen height fraction, it will draw the next lod")]
        public float screenSize;

        [Name("Group"), Description("group to show at this LOD. Drawables not in any lod group will be drawn at every LOD")]
        public Symbol group = Symbol.Empty;
        [Name("Transparency Group"), Description("translucency group to show at this LOD. Drawables in it are guaranteed to be drawn last.")]
        public Symbol transGroup = Symbol.Empty;

        [Name("Opaque"), Description("Opaque drawables to show at this LOD. Drawables not in any lod group will be drawn at every LOD")]
        public List<Symbol> opaque = new();

        [Name("Translucent"), Description("Translucent drawables to show at this LOD. Drawables in it are guaranteed to be drawn last.")]
        public List<Symbol> translucent = new();

        public LOD Read(EndianReader reader, uint revision)
        {
            screenSize = reader.ReadFloat();
            if (revision < 18)
            {
                group = Symbol.Read(reader);
                if (revision >= 15)
                {
                    transGroup = Symbol.Read(reader);
                }
            }
            else
            {
                opaque = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

                translucent = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            writer.WriteFloat(screenSize);
            if (revision < 18)
            {
                Symbol.Write(writer, group);
                if (revision >= 15)
                {
                    Symbol.Write(writer, transGroup);
                }
            }
            else
            {
                AssetHelpers.WriteList(writer, opaque, op => Symbol.Write(writer, op));

                AssetHelpers.WriteList(writer, translucent, trans => Symbol.Write(writer, trans));
            }
        }

        public override string ToString()
        {
            return $"{group} {transGroup} opaque: {opaque.Count} translucent: {translucent.Count}";
        }
    }

    public class CharacterTesting
    {
        private ushort altRevision;
        private ushort revision;
        [Name("Driver"), Description("The driver to animate")]
        public Symbol driver = Symbol.Empty;
        [Name("Clip 1"), Description("Clip to play")]
        public Symbol clip1 = Symbol.Empty;
        [Name("Clip 2"), Description("Clip to transition to, if any")]
        public Symbol clip2 = Symbol.Empty;
        [Name("Teleport To"), Description("Teleport to this Waypoint")]
        public Symbol teleportTo = Symbol.Empty;
        [MaxVersion(5)]
        public Symbol teleportFrom = Symbol.Empty;
        [Name("Walk Path"), Description("Walk path"), MinVersion(6)]
        public List<Symbol> walkPath = new();
        [Name("Distance Map"), Description("Displays the transition distance map between clip1 and clip2, raw means the raw graph, no nodes")]
        public Symbol distMap = Symbol.Empty;
        [Name("Transition"), Description("Which transition to use between clip1 and clip2")]
        public uint transition;
        [Name("Cycle Transition"), Description("Cycle through all the transitions")]
        public bool cycleTransition;
        public uint internalTransition;

        [MaxVersion(9)]
        public uint unk1;

        [Name("Metronome"), Description("Click on every beat transition")]
        public bool metronome;
        [Name("Zero Travel"), Description("Character does not travel, constantly zeros out position and facing")]
        public bool zeroTravel;
        [Name("Show Screen Size"), Description("graphically displays the screensize and lod next to the character")]
        public bool showScreenSize;
        [Name("Show Footextents"), Description("Show foot extents on all waypoints")]
        public bool footExtents;

        [Name("Clip2 Realtime"), Description("Force clip2 to be played in real time"), MaxVersion(14)]
        public bool clip2RealTime;

        [MinVersion(7), MaxVersion(10)]
        public Symbol oldClip = Symbol.Empty;
        [MinVersion(7), MaxVersion(10)]
        public int oldClipFlags;

        [MaxVersion(10), MinVersion(9)]
        public Symbol unkSymbol2 = Symbol.Empty;

        [Name("BPM"), Description("bpm to play clips at"), MaxVersion(14)]
        public uint bpm;

        [MaxVersion(0xB)]
        public Symbol unkSymbol = Symbol.Empty;

        public CharacterTesting Read(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
        {
            AssetHelpers.ReadRevision(reader, out revision, out altRevision);

            if (revision != 13)
            {
                driver = Symbol.Read(reader);
            }
            clip1 = Symbol.Read(reader);
            clip2 = Symbol.Read(reader);
            teleportTo = Symbol.Read(reader);
            if (revision < 6)
            {
                teleportFrom = Symbol.Read(reader);
            }
            else
            {
                walkPath = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            distMap = Symbol.Read(reader);

            if (revision < 6)
            {
                return this;
            }

            transition = reader.ReadUInt32();
            cycleTransition = reader.ReadBoolean();
            internalTransition = reader.ReadUInt32();

            if (revision < 10)
            {
                unk1 = reader.ReadUInt32();
            }

            metronome = reader.ReadBoolean();
            zeroTravel = reader.ReadBoolean();
            showScreenSize = reader.ReadBoolean();

            if (revision < 0xC)
            {
                unkSymbol = Symbol.Read(reader);
            }

            footExtents = reader.ReadBoolean();

            if (revision < 15)
            {
                clip2RealTime = reader.ReadBoolean();
                bpm = reader.ReadUInt32();
            }

            if (revision == 6)
            {
                return this;
            }

            if (revision >= 7 && revision <= 10)
            {
                oldClip = Symbol.Read(reader);
                oldClipFlags = reader.ReadInt32();
            }

            if (revision >= 9 && revision <= 10)
            {
                unkSymbol2 = Symbol.Read(reader);
            }

            return this;
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteRevision(writer, revision, altRevision);

            if (revision != 13)
            {
                Symbol.Write(writer, driver);
            }
            Symbol.Write(writer, clip1);
            Symbol.Write(writer, clip2);
            Symbol.Write(writer, teleportTo);
            if (revision < 6)
            {
                Symbol.Write(writer, teleportFrom);
            }
            else
            {
                AssetHelpers.WriteList(writer, walkPath, value => Symbol.Write(writer, value));
            }
            Symbol.Write(writer, distMap);

            if (revision < 6)
            {
                return;
            }

            writer.WriteUInt32(transition);
            writer.WriteBoolean(cycleTransition);
            writer.WriteUInt32(internalTransition);

            if (revision < 10)
            {
                writer.WriteUInt32(unk1);
            }

            writer.WriteBoolean(metronome);
            writer.WriteBoolean(zeroTravel);
            writer.WriteBoolean(showScreenSize);

            if (revision < 0xC)
            {
                Symbol.Write(writer, unkSymbol);
            }

            writer.WriteBoolean(footExtents);

            if (revision < 15)
            {
                writer.WriteBoolean(clip2RealTime);
                writer.WriteUInt32(bpm);
            }

            if (revision == 6)
            {
                return;
            }

            if (revision >= 7 && revision <= 10)
            {
                Symbol.Write(writer, oldClip);
                writer.WriteInt32(oldClipFlags);
            }

            if (revision >= 9 && revision <= 10)
            {
                Symbol.Write(writer, unkSymbol2);
            }
        }

        public static CharacterTesting New(ushort revision, ushort altRevision)
        {
            CharacterTesting characterTesting = new();
            characterTesting.revision = revision;
            characterTesting.altRevision = altRevision;
            return characterTesting;
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("LODs")]
    public List<LOD> lods = new();

    [Name("Shadow"), Description("Group containing shadow geometry")]
    public List<Symbol> shadows = new();

    [Name("Self Shadow"), Description("Whether this character should be self-shadowed."), MinVersion(3)]
    public bool selfShadow;

    [Name("Sphere Base"), Description("Base for bounding sphere, such as bone_pelvis.mesh"), MinVersion(5)]
    public Symbol sphereBase = Symbol.Empty;

    [Name("Bounding Sphere"), Description("bounding sphere for the character, fixed"), MinVersion(11)]
    public Sphere bounding = new();

    [Name("Frozen"), Description("if true, is frozen in place, no polling happens"), MinVersion(0xD)]
    public bool frozen;

    [Name("Force LOD"), Description("Forces LOD, kLODPerFrame is normal behavior of picking per frame, the others force the lod (0 is highest res lod, 2 is lowest res lod)"), MinVersion(0xF)]
    public int minLod;

    [Name("Translucency Group"), Description("translucency group to show independent of lod. Drawables in it are guaranteed to be drawn last."), MinVersion(0x11)]
    public Symbol translucentGroup = Symbol.Empty;

    [Name("Translucent"), Description("translucent objects to show independent of lod. Drawables in it are guaranteed to be drawn last.")]
    public List<Symbol> translucent = new();
    [Name("Showable Props"), Description("Props to show and hide for cut scenes"), MinVersion(19)]
    public List<Symbol> showableProps = new();

    [Name("Character Test"), Description("Test Character by animating it")]
    public CharacterTesting charTest = new();

    public Character(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    protected virtual bool ShouldTreatAsProxyForCharacterBody(DirectoryMeta.Entry? entry, bool standalone)
    {
        bool inlineRootProxy = standalone && inlineProxy && entry?.name.value == string.Empty;
        return (entry?.isProxy ?? false) || inlineRootProxy;
    }

    private bool UsesDrawableLists(DirectoryMeta parent)
    {
        // i hate these shitty one off functions like this but they avoid copy pasting these checks in with no context as to why, so theyre good for now
        return revision >= 18 && parent.revision >= 32;
    }

    public new Character Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        bool isProxyLike = ShouldTreatAsProxyForCharacterBody(entry, standalone);

        // these fields only present if dir is not proxied
        if (revision < 4 || !isProxyLike)
        {
            lods = AssetHelpers.ReadList(reader, () => new LOD().Read(reader, revision));

            shadows.Clear();
            if (UsesDrawableLists(parent))
            {
                shadows = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            else
            {
                shadows.Add(Symbol.Read(reader));
            }

            if (revision > 2)
            {
                selfShadow = reader.ReadBoolean();
            }

            if (revision > 4)
            {
                sphereBase = Symbol.Read(reader);
            }

            if (revision <= 9)
            {
                if (standalone)
                {
                    AssetHelpers.ReadEnd(reader, parent, entry, revision);
                }

                return this;
            }

            if (revision > 10)
            {
                bounding = new();
                bounding.Read(reader);
            }

            if (revision > 0xC)
            {
                frozen = reader.ReadBoolean();
            }

            if (revision > 0xE)
            {
                minLod = reader.ReadInt32();
            }

            if (revision > 0x10)
            {
                if (UsesDrawableLists(parent))
                {
                    translucent = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
                }
                else
                {
                    translucentGroup = Symbol.Read(reader);
                }
            }

            if (revision >= 19)
            {
                showableProps = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }

            charTest = new CharacterTesting().Read(reader, parent, entry);
        }
        else if (revision > 0xF)
        {
            charTest = new CharacterTesting().Read(reader, parent, entry);
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


        bool isProxyLike = ShouldTreatAsProxyForCharacterBody(entry, standalone);

        if (revision < 4 || !isProxyLike)
        {
            AssetHelpers.WriteList(writer, lods, lod => lod.Write(writer, revision));

            if (UsesDrawableLists(parent))
            {
                AssetHelpers.WriteList(writer, shadows, value => Symbol.Write(writer, value));
            }
            else if (shadows.Count > 0)
            {
                Symbol.Write(writer, shadows[0]);
            }
            else
            {
                Symbol.Write(writer, Symbol.Empty);
            }

            if (revision > 2)
            {
                writer.WriteBoolean(selfShadow);
            }

            if (revision > 4)
            {
                Symbol.Write(writer, sphereBase);
            }

            if (revision <= 9)
            {
                if (standalone)
                {
                    writer.WriteEndBytes();
                }

                return;
            }

            if (revision > 10)
            {
                bounding.Write(writer);
            }

            if (revision > 0xC)
            {
                writer.WriteBoolean(frozen);
            }

            if (revision > 0xE)
            {
                writer.WriteInt32(minLod);
            }

            if (revision > 0x10)
            {
                if (UsesDrawableLists(parent))
                {
                    AssetHelpers.WriteList(writer, translucent, value => Symbol.Write(writer, value));
                }
                else
                {
                    Symbol.Write(writer, translucentGroup);
                }
            }

            if (revision >= 19)
            {
                AssetHelpers.WriteList(writer, showableProps, value => Symbol.Write(writer, value));
            }

            charTest.Write(writer);
        }
        else if (revision > 0xF)
        {
            charTest.Write(writer);
        }


        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public static Character New(ushort revision, ushort altRevision)
    {
        Character character = new(revision, altRevision);
        character.revision = revision;
        character.altRevision = altRevision;
        return character;
    }




    public override bool IsDirectory()
    {
        return true;
    }
}
