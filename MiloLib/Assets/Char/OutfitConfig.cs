using MiloLib.Assets.Band;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("OutfitConfig"), Description("Configurable options for outfits")]
public class OutfitConfig : Object
{
    public class OldMatOption
    {
        [Name("Mat"), Description("Material with configurable colors/texture")]
        public Symbol mat = Symbol.Empty;
        [Name("Primary Palette"), Description("Colors for material color, shown to the user if set")]
        public Symbol primaryPalette = Symbol.Empty;
        [Name("Secondary Palette"), Description("Colors for material two_color, not shown to user unless primary_palette isn't set")]
        public Symbol secondaryPalette = Symbol.Empty;
        [Name("Textures"), Description("Textures to use instead of colors, must be same size as primary color palette!")]
        public List<Symbol> texs = new();

        public OldMatOption Read(EndianReader reader, uint revision)
        {
            mat = Symbol.Read(reader);
            primaryPalette = Symbol.Read(reader);
            if (revision != 0)
            {
                secondaryPalette = Symbol.Read(reader);
            }

            texs = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            Symbol.Write(writer, mat);
            Symbol.Write(writer, primaryPalette);
            if (revision != 0)
            {
                Symbol.Write(writer, secondaryPalette);
            }

            AssetHelpers.WriteList(writer, texs, (Symbol tex) => Symbol.Write(writer, tex));
        }
    }

    public class OldColorOption
    {
        [Name("Color Index"), Description("Index of color choice")]
        public int colorIndex;
        [Name("Mats"), Description("Color palette of first item in this list defines user's options in game")]
        public List<OldMatOption> matOptions = new();

        public OldColorOption Read(EndianReader reader, uint revision)
        {
            colorIndex = reader.ReadInt32();
            matOptions = AssetHelpers.ReadList(reader, () => new OldMatOption().Read(reader, revision));

            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            writer.WriteInt32(colorIndex);
            AssetHelpers.WriteList(writer, matOptions, (OldMatOption matOption) => matOption.Write(writer, revision));
        }
    }

    public class Overlay
    {
        [Name("Category"), Description("Where this overlay will go")]
        public int category;
        [Name("Texture"), Description("The overlay texture")]
        public Symbol texture = Symbol.Empty;

        public Overlay Read(EndianReader reader)
        {
            category = reader.ReadInt32();
            texture = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteInt32(category);
            Symbol.Write(writer, texture);
        }
    }
    public class Piercing
    {
        public class Piece
        {
            [Name("Attachment"), Description("Where piece attaches to skin, NULL for facial hair, suggest name spot_<x>_<y> where <x> is p for piercing, e for earring, etc.")]
            public Symbol attachment = Symbol.Empty;
            [Name("Highlight"), Description("When object is hilit, shows this piece")]
            public bool highlight;
            [Name("Vert"), Description("index of closest vert in the head mesh")]
            public int vert;
            public List<ushort> unkShorts = new();

            public int unkInt1;
            public int unkInt2;

            public bool unkBool1;

            public Matrix xfm = new();

            public List<ushort> unkUShorts = new();

            public Piece Read(EndianReader reader, uint revision)
            {
                if (revision > 0xF)
                {
                    vert = reader.ReadInt32();
                }
                else
                {
                    vert = -1;
                    unkInt1 = reader.ReadInt32();
                    unkInt2 = reader.ReadInt32();
                }

                if (revision < 0xF)
                {
                    unkBool1 = reader.ReadBoolean();
                }

                if (revision < 0xE)
                {
                    xfm.Read(reader);
                    unkUShorts = AssetHelpers.ReadList(reader, () => reader.ReadUInt16());
                }
                else
                {
                    attachment = Symbol.Read(reader);
                }

                unkShorts = AssetHelpers.ReadList(reader, () => reader.ReadUInt16());

                return this;
            }

            public void Write(EndianWriter writer, uint revision)
            {
                if (revision > 0xF)
                {
                    writer.WriteInt32(vert);
                }
                else
                {
                    writer.WriteInt32(unkInt1);
                    writer.WriteInt32(unkInt2);
                }

                if (revision < 0xF)
                {
                    writer.WriteBoolean(unkBool1);
                }

                if (revision < 0xE)
                {
                    xfm.Write(writer);
                    AssetHelpers.WriteList(writer, unkUShorts, (ushort unkUShort) => writer.WriteUInt16(unkUShort));
                }
                else
                {
                    Symbol.Write(writer, attachment);
                }

                AssetHelpers.WriteList(writer, unkShorts, (ushort unkShort) => writer.WriteUInt16(unkShort));
            }
        }

        [Name("Piercing"), Description("The piercing mesh, or a piercing bone Trans")]
        public Symbol piercing = Symbol.Empty;
        public Matrix transform = new();
        [Name("Reskin"), Description("When facial hair, reskins the [piercing] mesh to the face, only use for very close fitting facial hair")]
        public bool reskin;
        [Name("Pieces"), Description("discontinuous regions ofpolygons, for facial hair, only make one.")]
        public List<Piece> pieces = new();

        public int oldInt1;
        public int oldInt2;
        public Matrix oldTransform = new();

        public bool unkBool1;
        public bool unkBool2;

        public Piercing Read(EndianReader reader, uint revision)
        {
            piercing = Symbol.Read(reader);
            if (revision < 0xD)
            {
                pieces = new List<Piece>();
                Piece piece = new();
                oldInt1 = reader.ReadInt32();
                oldInt2 = reader.ReadInt32();
                piece.vert = -1;
                oldTransform.Read(reader);
                pieces.Add(piece);
            }
            else
            {
                transform.Read(reader);
                if (revision < 0xE)
                {
                    unkBool1 = reader.ReadBoolean();
                }

                if (revision == 0x10)
                {
                    unkBool2 = reader.ReadBoolean();
                }

                pieces = AssetHelpers.ReadList(reader, () => new Piece().Read(reader, revision));

                if (revision > 0x1A)
                {
                    reskin = reader.ReadBoolean();
                }
            }
            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            Symbol.Write(writer, piercing);
            if (revision < 0xD)
            {
                writer.WriteInt32(oldInt1);
                writer.WriteInt32(oldInt2);
                oldTransform.Write(writer);
            }
            else
            {
                transform.Write(writer);
                if (revision < 0xE)
                {
                    writer.WriteBoolean(unkBool1);
                }

                if (revision == 0x10)
                {
                    writer.WriteBoolean(unkBool2);
                }

                AssetHelpers.WriteList(writer, pieces, (Piece piece) => piece.Write(writer, revision));

                if (revision > 0x1A)
                {
                    writer.WriteBoolean(reskin);
                }
            }
        }
    }
    public class MeshAO
    {
        public class Seam
        {
            int idx;
            int coefficient;
            public Seam Read(EndianReader reader)
            {
                idx = reader.ReadInt32();
                coefficient = reader.ReadInt32();
                return this;
            }

            public void Write(EndianWriter writer)
            {
                writer.WriteInt32(idx);
                writer.WriteInt32(coefficient);
            }
        }

        [Name("Meshname"), Description("name of the mesh")]
        public Symbol meshName = Symbol.Empty;
        private uint coefficientCount;
        [Name("Coeffs"), Description("old colors, packed")]
        public List<int> coefficients = new();
        [Name("Seams")]
        public List<Seam> seams = new();

        public byte[] sha1Digest = new byte[20];

        public Symbol unkSym = Symbol.Empty;

        public MeshAO Read(EndianReader reader, uint revision)
        {
            meshName = Symbol.Read(reader);
            if (revision == 9 || revision == 10 || revision == 11 || revision == 12 || revision == 13 || revision == 14 || revision == 15 || revision == 16 || revision == 17 || revision == 18 || revision == 19 || revision == 20 || revision == 21 || revision == 22)
            {
                sha1Digest = reader.ReadBlock(20);
            }
            coefficientCount = reader.ReadUInt32();
            coefficients = new List<int>();
            for (int i = 0; i < coefficientCount; i++)
            {
                coefficients.Add(reader.ReadInt32());
            }

            seams = AssetHelpers.ReadList(reader, () => new Seam().Read(reader));

            if (revision > 0x18)
            {
                unkSym = Symbol.Read(reader);
            }

            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            Symbol.Write(writer, meshName);
            if (revision == 9 || revision == 10 || revision == 11 || revision == 12 || revision == 13 || revision == 14 || revision == 15 || revision == 16 || revision == 17 || revision == 18 || revision == 19 || revision == 20 || revision == 21 || revision == 22)
            {
                writer.WriteBlock(sha1Digest);
            }
            AssetHelpers.WriteList(writer, coefficients, (int coefficient) => writer.WriteInt32(coefficient));

            AssetHelpers.WriteList(writer, seams, (Seam seam) => seam.Write(writer));

            if (revision > 0x18)
            {
                Symbol.Write(writer, unkSym);
            }
        }
    }

    public class MatSwap
    {
        [Name("Mat"), Description("The material to be colored by palettes")]
        public Symbol mat = Symbol.Empty;
        [Name("Resource Mat"), Description("optional resource material to be replaced with by [mat]")]
        public Symbol resourceMat = Symbol.Empty;
        public bool unkBool;
        [Name("Two Color Diffuse"), Description("Diffuse texture for two color, mat must have render to texture")]
        public Symbol twoColorDiffuse = Symbol.Empty;
        [Name("Two Color Interp"), Description("Two color interpolation, transparent means use color1, opaque means color2, blends")]
        public Symbol twoColorInterp = Symbol.Empty;
        [Name("Two Color Mask"), Description("Two color mask, transparent means do no color modulation, opaque means do full color modulation")]
        public Symbol twoColorMask = Symbol.Empty;
        [Name("Color1 Palette"), Description("Palette to use for mat color1")]
        public Symbol color1Palette = Symbol.Empty;
        int color1Option;
        [Name("Color2 Palette"), Description("Palette to use for mat color2, if NULL, assumes white")]
        public Symbol color2Palette = Symbol.Empty;
        int color2Option;
        [Name("Textures"), Description("Textures to use instead of colors, uses color1_option")]
        public List<Symbol> textures = new();

        public MatSwap Read(EndianReader reader, uint revision)
        {
            mat = Symbol.Read(reader);
            resourceMat = Symbol.Read(reader);
            if (revision < 5)
            {
                unkBool = reader.ReadBoolean();
            }
            else
            {
                twoColorDiffuse = Symbol.Read(reader);
                twoColorInterp = Symbol.Read(reader);
            }
            twoColorMask = Symbol.Read(reader);
            if (revision > 4)
            {
                color1Palette = Symbol.Read(reader);
                color1Option = reader.ReadInt32();
                color2Palette = Symbol.Read(reader);
                color2Option = reader.ReadInt32();
                textures = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            Symbol.Write(writer, mat);
            Symbol.Write(writer, resourceMat);
            if (revision < 5)
            {
                writer.WriteBoolean(unkBool);
            }
            else
            {
                Symbol.Write(writer, twoColorDiffuse);
                Symbol.Write(writer, twoColorInterp);
            }
            Symbol.Write(writer, twoColorMask);
            if (revision > 4)
            {
                Symbol.Write(writer, color1Palette);
                writer.WriteInt32(color1Option);
                Symbol.Write(writer, color2Palette);
                writer.WriteInt32(color2Option);
                AssetHelpers.WriteList(writer, textures, (Symbol texture) => Symbol.Write(writer, texture));
            }
        }

        public override string ToString()
        {
            return $"Mat: {mat} Resource Mat: {resourceMat} {twoColorDiffuse} {twoColorInterp} {twoColorMask} {color1Palette} {color1Option} {color2Palette} {color2Option} {textures.Count}";
        }
    }
    private ushort altRevision;
    private ushort revision;

    [Name("Colors"), MinVersion(5)]
    private int[] colors = new int[3];

    [Name("Compute AO"), Description("should this outfit config compute AO"), MinVersion(8)]
    public bool computeAO;

    [Name("Mat Swaps"), Description("Material swaps from resource file, and two color information"), MinVersion(4)]
    public List<MatSwap> matSwaps = new();

    [Description("All color/texture choices for this outfit in game"), MaxVersion(4)]
    public List<OldColorOption> oldColorOptions = new();

    [MinVersion(1), MaxVersion(1)]
    public List<Symbol> oldDirs = new();

    [Name("Mesh AOs"), MinVersion(7)]
    public List<MeshAO> meshAOs = new();

    [Name("Band Patch Meshes"), MinVersion(10)]
    public List<BandPatchMesh> bandPatchMeshes = new();

    [Name("Piercings"), MinVersion(12)]
    public List<Piercing> piercings = new();

    [Name("Tex Blender"), Description("Optional tex blender, will recompose and compress output like our own"), MinVersion(19)]
    public Symbol texBlender = Symbol.Empty;
    [Name("Wrinkle Blender"), Description("Optional per frame wrinkle blender"), MinVersion(24)]
    public Symbol wrinkleBlender = Symbol.Empty;

    [Name("Band Logo"), Description("Mat that will get the band logo as diffuse texture"), MinVersion(21)]
    public Symbol bandLogo = Symbol.Empty;

    [Name("Overlays"), Description("System for overlaying translucent textures on top of existing patch areas, such as skin, must match the UV mappings for each one."), MinVersion(20)]
    public List<Overlay> overlays = new();

    [MinVersion(22)]
    public byte[] sha1Digest = new byte[20];






    public new OutfitConfig Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        if (revision > 4)
        {
            colors[0] = reader.ReadInt32();
            colors[1] = reader.ReadInt32();
            if (revision > 10)
            {
                colors[2] = reader.ReadInt32();
            }
        }

        if (revision > 3)
        {
            matSwaps = AssetHelpers.ReadList(reader, () => new MatSwap().Read(reader, revision));
        }

        if (revision < 5)
        {
            oldColorOptions = AssetHelpers.ReadList(reader, () => new OldColorOption().Read(reader, revision));

            if (revision != 0 && revision < 2)
            {
                oldDirs = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            }
        }

        if (revision > 6)
        {
            meshAOs = AssetHelpers.ReadList(reader, () => new MeshAO().Read(reader, revision));
        }
        if (revision > 7)
        {
            computeAO = reader.ReadBoolean();
        }

        if (revision > 9)
        {
            bandPatchMeshes = AssetHelpers.ReadList(reader, () => new BandPatchMesh().Read(reader, false, parent, entry));
        }

        if (revision > 0xB)
        {
            piercings = AssetHelpers.ReadList(reader, () => new Piercing().Read(reader, revision));
        }

        if (revision > 0x12)
        {
            texBlender = Symbol.Read(reader);
        }

        if (revision > 0x13)
        {
            overlays = AssetHelpers.ReadList(reader, () => new Overlay().Read(reader));
        }

        if (revision > 0x14)
        {
            bandLogo = Symbol.Read(reader);
        }

        if (revision > 0x15)
        {
            sha1Digest = reader.ReadBlock(20);
        }

        if (revision > 0x17)
        {
            wrinkleBlender = Symbol.Read(reader);
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);

        if (revision > 4)
        {
            writer.WriteInt32(colors[0]);
            writer.WriteInt32(colors[1]);
            if (revision > 10)
            {
                writer.WriteInt32(colors[2]);
            }
        }

        if (revision > 3)
        {
            AssetHelpers.WriteList(writer, matSwaps, (MatSwap matSwap) => matSwap.Write(writer, revision));
        }

        if (revision < 5)
        {
            AssetHelpers.WriteList(writer, oldColorOptions, (OldColorOption oldColorOption) => oldColorOption.Write(writer, revision));

            if (revision != 0 && revision < 2)
            {
                AssetHelpers.WriteList(writer, oldDirs, (Symbol dir) => Symbol.Write(writer, dir));
            }
        }

        if (revision > 6)
        {
            AssetHelpers.WriteList(writer, meshAOs, (MeshAO meshAO) => meshAO.Write(writer, revision));
        }
        if (revision > 7)
        {
            writer.WriteBoolean(computeAO);
        }

        if (revision > 9)
        {
            AssetHelpers.WriteList(writer, bandPatchMeshes, (BandPatchMesh bandPatchMesh) => bandPatchMesh.Write(writer, false, parent, entry));
        }

        if (revision > 0xB)
        {
            AssetHelpers.WriteList(writer, piercings, (Piercing piercing) => piercing.Write(writer, revision));
        }

        if (revision > 0x12)
        {
            Symbol.Write(writer, texBlender);
        }

        if (revision > 0x13)
        {
            AssetHelpers.WriteList(writer, overlays, (Overlay overlay) => overlay.Write(writer));
        }

        if (revision > 0x14)
        {
            Symbol.Write(writer, bandLogo);
        }

        if (revision > 0x15)
        {
            writer.WriteBlock(sha1Digest);
        }

        if (revision > 0x17)
        {
            Symbol.Write(writer, wrinkleBlender);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
