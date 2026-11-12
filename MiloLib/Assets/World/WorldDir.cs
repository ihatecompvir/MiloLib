using MiloLib.Utils;
using MiloLib.Classes;
using MiloLib.Assets.UI;
using MiloLib.Assets.Rnd;

namespace MiloLib.Assets.World;

[Name("WorldDir"), Description("A WorldDir contains world objects.")]
public class WorldDir : PanelDir
{
    private ushort altRevision;
    private ushort revision;
    [Name("Fake HUD Filename"), Description("HUD Preview Dir"), MinVersion(10)]
    public Symbol fakeHUDFilename = Symbol.Empty;
    public class BitmapOverride
    {
        [Name("Original"), Description("Subdir texture to replace")]
        public Symbol original = Symbol.Empty;
        [Name("Replacement"), Description("Curdir texture to replace with")]
        public Symbol replacement = Symbol.Empty;
    }
    public class MatOverride
    {
        [Name("Mesh"), Description("Subdir mesh to modify")]
        public Symbol mesh = Symbol.Empty;
        [Name("Mat"), Description("Curdir material to set")]
        public Symbol mat = Symbol.Empty;
    }
    public class PresetOverride
    {
        [Name("Preset"), Description("Subdir preset to modify")]
        public Symbol preset = Symbol.Empty;
        [Name("Hue"), Description("Hue texture to use")]
        public Symbol hue = Symbol.Empty;
    }
    [Name("Preset Overrides"), MinVersion(15)]
    public List<PresetOverride> presetOverrides = new();

    public class CrowdOverride
    {
        [Name("Original")]
        public Symbol original = Symbol.Empty;

        [Name("Replacement")]
        public Symbol replacement = Symbol.Empty;
    }

    [Name("Crowd Overrides"), MinVersion(18), MaxVersion(18)]
    public List<CrowdOverride> crowdOverrides = new();

    private bool hasCrowdOverridePairs = true;

    private bool UsesLegoCrowdOverrides(DirectoryMeta parent)
    {
        return parent.revision == 25 && revision == 18 && hasCrowdOverridePairs;
    }

    private bool TryReadCrowdOverrides(EndianReader reader, bool standalone)
    {
        long start = reader.BaseStream.Position;
        try
        {
            uint count = reader.ReadUInt32();
            long remaining = reader.BaseStream.Length - reader.BaseStream.Position;
            if (count > remaining / 8)
            {
                throw new InvalidDataException("Crowd override count exceeds the remaining data.");
            }

            List<CrowdOverride> pairs = new();
            for (int i = 0; i < count; i++)
            {
                CrowdOverride pair = new();
                pair.original = Symbol.Read(reader);
                pair.replacement = Symbol.Read(reader);
                pairs.Add(pair);
            }
            if (standalone)
            {
                long end = reader.BaseStream.Position;
                if (altRevision > 0)
                {
                    reader.BaseStream.Position++;
                }

                uint expected = 0xADDEADDE;
                if (reader.Endianness == Endian.LittleEndian)
                {
                    expected = 0xDEADDEAD;
                }

                if (reader.ReadUInt32() != expected)
                {
                    throw new InvalidDataException("Crowd overrides do not end at the directory boundary.");
                }

                reader.BaseStream.Position = end;
            }
            crowdOverrides = pairs;
            return true;
        }
        catch (Exception exception) when (exception is InvalidDataException || exception is EndOfStreamException)
        {
            reader.BaseStream.Position = start;
            return false;
        }
    }

    [Name("Bitmap Overrides"), MinVersion(12)]
    public List<BitmapOverride> bitmapOverrides = new();

    [Name("Mat Overrides"), MinVersion(14)]
    public List<MatOverride> matOverrides = new();

    [Name("Hide Overrides"), Description("Subdir objects to hide"), MinVersion(12)]
    public List<Symbol> hideOverrides = new();
    [Name("Cam Shot Overrides"), Description("Subdir camshots to inhibit"), MinVersion(16)]
    public List<Symbol> camShotOverrides = new();
    [Name("PS3 Per Pixel Shows"), Description("Things to show when ps3_per_pixel on CamShot"), MinVersion(17)]
    public List<Symbol> ps3PerPixelShows = new();
    [Name("PS3 Per Pixel Hides"), Description("Things to hide when ps3_per_pixel on CamShot"), MinVersion(17)]
    public List<Symbol> ps3PerPixelHides = new();

    [Name("Test Light Preset 1"), Description("The first light preset to start"), MinVersion(19)]
    public Symbol testPreset1 = Symbol.Empty;
    [Name("Test Light Preset 2"), Description("The second light preset to start"), MinVersion(19)]
    public Symbol testPreset2 = Symbol.Empty;
    [Name("Test Animation Time"), Description("animation time in beats"), MinVersion(19)]
    public float testAnimationTime;
    [Name("HUD"), Description("hud to be drawn last"), MinVersion(20)]
    public Symbol hud = Symbol.Empty;
    [MinVersion(5), MaxVersion(5)]
    public Symbol camReference = Symbol.Empty;

    [MinVersion(11), MaxVersion(24)]
    public Matrix xfm = new();
    [MinVersion(7), MaxVersion(10)]
    public RndTrans camTrans = new();

    [MinVersion(2), MaxVersion(20)]
    public uint unkInt1;
    [MinVersion(2), MaxVersion(20)]
    public float unkFloat;

    [Name("Spotlight Drawer"), MinVersion(18), MaxVersion(21)]
    public Symbol unkSym = Symbol.Empty;

    [Name("Three D Send"), Description("FxSend applied to all 3DSound objects"), MinVersion(26), MaxVersion(26)]
    public Symbol oldFxSend = Symbol.Empty;
    [Name("Doppler Power"), Description("Change in frequency due to doppler shift is raised to this power. 0 is no doppler shift, 0.5 is half normal shift, 2 is twice normal, etc."), MinVersion(28)]
    public float dopplerPower;
    [Name("Listener"), Description("Position of the listener for 3D sounds. None defaults to the current camera."), MinVersion(29)]
    public Symbol listener = Symbol.Empty;
    [Name("Explicit Postproc"), Description("TRUE if we explicitly do the postprocing")]
    public bool explicitPostProc = true;


    public WorldDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }


    public new WorldDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        hasCrowdOverridePairs = true;
        crowdOverrides.Clear();
        unkSym = Symbol.Empty;
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 0 && revision < 5)
        {
            cam = Symbol.Read(reader);
        }

        if (revision >= 2 && revision <= 20)
        {
            unkInt1 = reader.ReadUInt32();
            unkFloat = reader.ReadFloat();
        }

        if (revision > 9)
        {
            fakeHUDFilename = Symbol.Read(reader);
        }

        if (revision < 9)
        {
            if (revision > 7)
            {
                // OldLoadProxies
            }
            else if (revision > 2)
            {
                // OldLoadChars
            }
        }

        base.Read(reader, false, parent, entry);

        if (revision == 5)
        {
            camReference = Symbol.Read(reader);
        }

        if (revision < 0x19)
        {
            if (revision > 0xA)
            {
                xfm = xfm.Read(reader);
            }
            else if (revision > 6)
            {
                camTrans = camTrans.Read(reader, false, parent, entry);
            }
        }



        if (revision > 0xB)
        {
            hideOverrides = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            bitmapOverrides = AssetHelpers.ReadList(reader, () => new BitmapOverride { original = Symbol.Read(reader), replacement = Symbol.Read(reader) });
        }

        if (revision > 0xD)
        {
            matOverrides = AssetHelpers.ReadList(reader, () => new MatOverride { mesh = Symbol.Read(reader), mat = Symbol.Read(reader) });
        }

        if (revision > 0xE)
        {
            presetOverrides = AssetHelpers.ReadList(reader, () => new PresetOverride { preset = Symbol.Read(reader), hue = Symbol.Read(reader) });
        }

        if (revision > 0xF)
        {
            camShotOverrides = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (revision > 0x10 && revision != 0x17)
        {
            ps3PerPixelHides = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            ps3PerPixelShows = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }

        if (UsesLegoCrowdOverrides(parent))
        {
            hasCrowdOverridePairs = TryReadCrowdOverrides(reader, standalone);
            if (!hasCrowdOverridePairs)
            {
                unkSym = Symbol.Read(reader);
            }
        }
        else if (revision == 0x12 || revision == 0x13 || revision == 0x14 || revision == 0x15)
        {
            unkSym = Symbol.Read(reader);
        }

        if (revision > 0x12)
        {
            testPreset1 = Symbol.Read(reader);
            testPreset2 = Symbol.Read(reader);
            testAnimationTime = reader.ReadFloat();
        }

        if (revision > 0x13)
        {
            hud = Symbol.Read(reader);
        }

        if (revision == 26)
        {
            oldFxSend = Symbol.Read(reader);
        }
        if (revision >= 28)
        {
            dopplerPower = reader.ReadFloat();
        }
        if (revision >= 29)
        {
            listener = Symbol.Read(reader);
        }
        if (altRevision > 0)
        {
            explicitPostProc = reader.ReadBoolean();
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

        if (revision != 0 && revision < 5)
        {
            Symbol.Write(writer, cam);
        }

        if (revision >= 2 && revision <= 20)
        {
            writer.WriteUInt32(unkInt1);
            writer.WriteFloat(unkFloat);
        }

        if (revision > 9)
        {
            Symbol.Write(writer, fakeHUDFilename);
        }

        if (revision < 9)
        {
            if (revision > 7)
            {
                // OldLoadProxies
            }
            else if (revision > 2)
            {
                // OldLoadChars
            }
        }

        base.Write(writer, false, parent, entry);

        if (revision == 5)
        {
            Symbol.Write(writer, camReference);
        }

        if (revision < 0x19)
        {
            if (revision > 0xA)
            {
                xfm.Write(writer);
            }
            else if (revision > 6)
            {
                camTrans.Write(writer, false, parent, null);
            }
        }

        if (revision > 0xB)
        {
            AssetHelpers.WriteList(writer, hideOverrides, hideOverride => Symbol.Write(writer, hideOverride));

            AssetHelpers.WriteList(writer, bitmapOverrides, bitmapOverride => { Symbol.Write(writer, bitmapOverride.original); Symbol.Write(writer, bitmapOverride.replacement); });
        }

        if (revision > 0xD)
        {
            AssetHelpers.WriteList(writer, matOverrides, matOverride => { Symbol.Write(writer, matOverride.mesh); Symbol.Write(writer, matOverride.mat); });
        }

        if (revision > 0xE)
        {
            AssetHelpers.WriteList(writer, presetOverrides, presetOverride => { Symbol.Write(writer, presetOverride.preset); Symbol.Write(writer, presetOverride.hue); });
        }

        if (revision > 0xF)
        {
            AssetHelpers.WriteList(writer, camShotOverrides, camShot => Symbol.Write(writer, camShot));
        }

        if (revision > 0x10 && revision != 0x17)
        {
            AssetHelpers.WriteList(writer, ps3PerPixelHides, perPixelHide => Symbol.Write(writer, perPixelHide));

            AssetHelpers.WriteList(writer, ps3PerPixelShows, perPixelShow => Symbol.Write(writer, perPixelShow));
        }

        if (UsesLegoCrowdOverrides(parent))
        {
            AssetHelpers.WriteList(writer, crowdOverrides, crowdOverride => { Symbol.Write(writer, crowdOverride.original); Symbol.Write(writer, crowdOverride.replacement); });
        }
        else if (revision == 0x12 || revision == 0x13 || revision == 0x14 || revision == 0x15)
        {
            Symbol.Write(writer, unkSym);
        }

        if (revision > 0x12)
        {
            Symbol.Write(writer, testPreset1);
            Symbol.Write(writer, testPreset2);
            writer.WriteFloat(testAnimationTime);
        }

        if (revision > 0x13)
        {
            Symbol.Write(writer, hud);
        }

        if (revision == 26)
        {
            Symbol.Write(writer, oldFxSend);
        }
        if (revision >= 28)
        {
            writer.WriteFloat(dopplerPower);
        }
        if (revision >= 29)
        {
            Symbol.Write(writer, listener);
        }
        if (altRevision > 0)
        {
            writer.WriteBoolean(explicitPostProc);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }
}
