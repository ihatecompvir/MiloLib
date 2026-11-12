using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("GemTrackDir"), Description("band 2 TrackDir for gem tracks")]
public class GemTrackDir : TrackDir
{
    private static readonly Symbol EmptySymbol = Symbol.Empty;

    private ushort altRevision;
    private ushort revision;

    private ushort altTrackRevision;
    private ushort trackRevision;

    [MaxVersion(8)]
    public int unkInt1;
    [Name("Effect Selector")]
    public Symbol effectsSelector = Symbol.Empty;

    public Symbol unkTex = Symbol.Empty;
    [Name("Surface Texture"), MaxVersion(0)]
    public Symbol surfaceTexture = Symbol.Empty;

    [Name("Surface Mesh")]
    public Symbol surfaceMesh = Symbol.Empty;
    [Name("Surface Mat")]
    public Symbol surfaceMat = Symbol.Empty;
    [Name("Track Env")]
    public Symbol trackEnv = Symbol.Empty;
    [Name("Game Cam")]
    public Symbol gameCam = Symbol.Empty;
    [Name("Bass Super Streak On Trig")]
    public Symbol bassSuperStreakOnTrig = Symbol.Empty;
    [Name("Bass Super Streak Off Trig")]
    public Symbol bassSuperStreakOffTrig = Symbol.Empty;
    [Name("Kick Drummer Trig")]
    public Symbol kickDrummerTrig = Symbol.Empty;
    [Name("Kick Drummer Reset Trig"), MinVersion(5)]
    public Symbol kickDrummerResetTrig = Symbol.Empty;
    [Name("Spotlight Phrase Success Trig")]
    public Symbol spotlightPhraseSuccessTrig = Symbol.Empty;
    [MaxVersion(11)]
    public Symbol unkTrig = Symbol.Empty;
    [MinVersion(8), MaxVersion(9)]
    public Symbol unkTrig2 = Symbol.Empty;
    [MinVersion(8), MaxVersion(9)]
    public Symbol unkTrig3 = Symbol.Empty;
    [Name("Peak State On Trig"), MinVersion(8)]
    public Symbol peakStateOnTrig = Symbol.Empty;
    [Name("Peak State Off Trig"), MinVersion(8)]
    public Symbol peakStateOffTrig = Symbol.Empty;
    [Name("Drum Fill Reset Trig")]
    public Symbol drumFillResetTrig = Symbol.Empty;
    [Name("Drum Mash 2nd Pass Activate Anim")]
    public Symbol drumMash2ndPassActivateAnim = Symbol.Empty;
    [Name("Drum Mash Hit Anim Grp")]
    public Symbol drumMashHitAnimGrp = Symbol.Empty;
    [Name("Fill Colors Grp")]
    public Symbol fillColorsGrp = Symbol.Empty;
    [Name("LOD Anim")]
    public Symbol lodAnim = Symbol.Empty;
    [Name("Rotater")]
    public Symbol rotator = Symbol.Empty;
    [MinVersion(6), MaxVersion(10)]
    public Symbol unkAnim = Symbol.Empty;
    [Name("Glow Widgets")]
    public List<Symbol> glowWidgets = new();

    [Name("Gem Mash Anims")]
    public List<Symbol> gemMashAnims = new();

    [Name("Drum Mash Anims")]
    public List<Symbol> drumMashAnims = new();

    [Name("Fill Hit Trigs")]
    public List<Symbol> fillHitTrigs = new();

    [Name("Real Guitar Mash Anims"), MinVersion(11)]
    public List<Symbol> realGuitarMashAnims = new();

    [Name("Fill Lane Anims"), MinVersion(12)]
    public List<Symbol> fillLaneAnims = new();

    [Name("Fret Number Positioning"), Description("the x offset from the center of an RG gem mesh where the fret number should go"), MinVersion(4)]
    public List<float> fretPosOffsets = new();

    [Name("Streak Meter Offset"), Description("The projected distance between the now bar and the streak meter"), MinVersion(2)]
    public float streakMeterOffset;
    [Name("Streak Meter Tilt"), Description("The tilt of the streak meter relative to game.cam, in degrees"), MinVersion(3)]
    public float streakMeterTilt;
    [Name("Chord Label Pos Offset"), Description("the x offset from the center of the track where the chord label should go"), MinVersion(7)]
    public float chordLabelPosOffset;

    public bool simulatedNet;
    public Symbol instrument = Symbol.Empty;

    public Symbol starPowerMeter = Symbol.Empty;
    public Symbol streakMeter = Symbol.Empty;

    public Symbol playerIntro = Symbol.Empty;
    public Symbol popupObject = Symbol.Empty;
    public Symbol playerFeedback = Symbol.Empty;
    public Symbol failedFeedback = Symbol.Empty;
    public Symbol endgameFeedback = Symbol.Empty;
    public Symbol retractTrig = Symbol.Empty;
    public Symbol resetTrig = Symbol.Empty;
    public Symbol deployTrig = Symbol.Empty;
    public Symbol stopDeployTrig = Symbol.Empty;
    public Symbol introTrig = Symbol.Empty;


    public void LoadTrack(EndianReader reader, bool b1, bool b2, bool b3)
    {
        AssetHelpers.ReadRevision(reader, out trackRevision, out altTrackRevision);

        simulatedNet = reader.ReadBoolean();
        instrument = Symbol.Read(reader);

        if (trackRevision >= 1 && !b1)
        {
            starPowerMeter = Symbol.Read(reader);
            streakMeter = Symbol.Read(reader);
        }
        bool finalbool;
        if (trackRevision < 3)
        {
            finalbool = false;
            if (!b3 || !b1)
            {
                finalbool = true;
            }
        }
        else
        {
            finalbool = !b1;
        }
        if (finalbool)
        {
            playerIntro = Symbol.Read(reader);
            if (trackRevision < 1)
            {
                starPowerMeter = Symbol.Read(reader);
                streakMeter = Symbol.Read(reader);
            }
            popupObject = Symbol.Read(reader);
            playerFeedback = Symbol.Read(reader);
            failedFeedback = Symbol.Read(reader);
            if (trackRevision >= 2)
            {
                endgameFeedback = Symbol.Read(reader);
            }
        }
        if (!b1)
        {
            retractTrig = Symbol.Read(reader);
            resetTrig = Symbol.Read(reader);
            deployTrig = Symbol.Read(reader);
            stopDeployTrig = Symbol.Read(reader);
            introTrig = Symbol.Read(reader);
        }
        return;
    }

    public void SaveTrack(EndianWriter writer, bool b1, bool b2, bool b3)
    {
        AssetHelpers.WriteRevision(writer, trackRevision, altTrackRevision);

        writer.WriteBoolean(simulatedNet);
        Symbol.Write(writer, instrument);

        if (trackRevision >= 1 && !b1)
        {
            Symbol.Write(writer, starPowerMeter);
            Symbol.Write(writer, streakMeter);
        }
        bool finalbool;
        if (trackRevision < 3)
        {
            finalbool = false;
            if (!b3 || !b1)
            {
                finalbool = true;
            }
        }
        else
        {
            finalbool = !b1;
        }
        if (finalbool)
        {
            Symbol.Write(writer, playerIntro);
            if (trackRevision < 1)
            {
                Symbol.Write(writer, starPowerMeter);
                Symbol.Write(writer, streakMeter);
            }
            Symbol.Write(writer, popupObject);
            Symbol.Write(writer, playerFeedback);
            Symbol.Write(writer, failedFeedback);
            if (trackRevision >= 2)
            {
                Symbol.Write(writer, endgameFeedback);
            }
        }
        if (!b1)
        {
            Symbol.Write(writer, retractTrig);
            Symbol.Write(writer, resetTrig);
            Symbol.Write(writer, deployTrig);
            Symbol.Write(writer, stopDeployTrig);
            Symbol.Write(writer, introTrig);
        }
    }

    public GemTrackDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    private static Symbol GetFixedSymbol(List<Symbol> values, int index)
    {
        if (index < values.Count && values[index] != null)
        {
            return values[index];
        }

        return EmptySymbol;
    }

    public new GemTrackDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 9)
        {
            unkInt1 = reader.ReadInt32();
            effectsSelector = Symbol.Read(reader);
            if (revision < 1)
            {
                surfaceTexture = Symbol.Read(reader);
            }
        }
        if (!entry.isProxy)
        {
            if (revision >= 9)
            {
                effectsSelector = Symbol.Read(reader);
            }

            surfaceMesh = Symbol.Read(reader);
            surfaceMat = Symbol.Read(reader);
            trackEnv = Symbol.Read(reader);
            gameCam = Symbol.Read(reader);
            bassSuperStreakOnTrig = Symbol.Read(reader);
            bassSuperStreakOffTrig = Symbol.Read(reader);
            kickDrummerTrig = Symbol.Read(reader);
            spotlightPhraseSuccessTrig = Symbol.Read(reader);
            if (revision < 0xC)
            {
                unkTrig = Symbol.Read(reader);
            }

            drumFillResetTrig = Symbol.Read(reader);
            drumMash2ndPassActivateAnim = Symbol.Read(reader);
            drumMashHitAnimGrp = Symbol.Read(reader);
            fillColorsGrp = Symbol.Read(reader);
            lodAnim = Symbol.Read(reader);
            rotator = Symbol.Read(reader);
            glowWidgets = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));

            for (int i = 0; i < 5; i++)
            {
                gemMashAnims.Add(Symbol.Read(reader));
            }

            if (revision >= 6 && revision <= 10)
            {
                unkAnim = Symbol.Read(reader);
            }

            for (int i = 1; i < 5; i++)
            {
                drumMashAnims.Add(Symbol.Read(reader));
            }

            for (int i = 0; i < 3; i++)
            {
                fillHitTrigs.Add(Symbol.Read(reader));
            }

            if (revision >= 11)
            {
                for (int i = 0; i < 6; i++)
                {
                    realGuitarMashAnims.Add(Symbol.Read(reader));
                }
            }
        }

        // RB2 proxy data stores this float, RB3 proxy data doesn;t
        if (revision >= 2 && (!entry.isProxy || revision <= 3))
        {
            streakMeterOffset = reader.ReadFloat();
        }

        if (!entry.isProxy)
        {
            if (revision >= 3)
            {
                streakMeterTilt = reader.ReadFloat();
            }

            if (revision >= 4)
            {
                fretPosOffsets = AssetHelpers.ReadList(reader, () => reader.ReadFloat());
            }
            if (revision >= 5)
            {
                kickDrummerResetTrig = Symbol.Read(reader);
            }

            if (revision >= 7)
            {
                chordLabelPosOffset = reader.ReadFloat();
            }

            if (revision >= 8)
            {
                if (revision < 10)
                {
                    unkTrig2 = Symbol.Read(reader);
                    unkTrig3 = Symbol.Read(reader);
                }
                peakStateOnTrig = Symbol.Read(reader);
                peakStateOffTrig = Symbol.Read(reader);
            }

            if (revision >= 12)
            {
                for (int i = 1; i < 5; i++)
                {
                    fillLaneAnims.Add(Symbol.Read(reader));
                }
            }


        }

        LoadTrack(reader, entry.isProxy, false, false);
        base.Read(reader, false, parent, entry);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision < 9)
        {
            writer.WriteInt32(unkInt1);
            Symbol.Write(writer, effectsSelector);
            if (revision < 1)
            {
                Symbol.Write(writer, surfaceTexture);
            }
        }

        if (!entry.isProxy)
        {
            if (revision >= 9)
            {
                Symbol.Write(writer, effectsSelector);
            }
            Symbol.Write(writer, surfaceMesh);
            Symbol.Write(writer, surfaceMat);
            Symbol.Write(writer, trackEnv);
            Symbol.Write(writer, gameCam);
            Symbol.Write(writer, bassSuperStreakOnTrig);
            Symbol.Write(writer, bassSuperStreakOffTrig);
            Symbol.Write(writer, kickDrummerTrig);
            Symbol.Write(writer, spotlightPhraseSuccessTrig);
            if (revision < 0xC)
            {
                Symbol.Write(writer, unkTrig);
            }
            Symbol.Write(writer, drumFillResetTrig);
            Symbol.Write(writer, drumMash2ndPassActivateAnim);
            Symbol.Write(writer, drumMashHitAnimGrp);
            Symbol.Write(writer, fillColorsGrp);
            Symbol.Write(writer, lodAnim);
            Symbol.Write(writer, rotator);

            AssetHelpers.WriteList(writer, glowWidgets, glowWidget => Symbol.Write(writer, glowWidget));

            for (int i = 0; i < 5; i++)
            {
                Symbol.Write(writer, GetFixedSymbol(gemMashAnims, i));
            }

            if (revision >= 6 && revision <= 10)
            {
                Symbol.Write(writer, unkAnim);
            }

            for (int i = 0; i < 4; i++)
            {
                Symbol.Write(writer, GetFixedSymbol(drumMashAnims, i));
            }

            for (int i = 0; i < 3; i++)
            {
                Symbol.Write(writer, GetFixedSymbol(fillHitTrigs, i));
            }

            if (revision >= 11)
            {
                for (int i = 0; i < 6; i++)
                {
                    Symbol.Write(writer, GetFixedSymbol(realGuitarMashAnims, i));
                }
            }
        }

        // RB2 proxy data stores this float, RB3 proxy data does not
        if (revision >= 2 && (!entry.isProxy || revision <= 3))
        {
            writer.WriteFloat(streakMeterOffset);
        }

        if (!entry.isProxy)
        {
            if (revision >= 3)
            {
                writer.WriteFloat(streakMeterTilt);
            }

            if (revision >= 4)
            {
                AssetHelpers.WriteList(writer, fretPosOffsets, fretPosOffset => writer.WriteFloat(fretPosOffset));
            }

            if (revision >= 5)
            {
                Symbol.Write(writer, kickDrummerResetTrig);
            }
            if (revision >= 7)
            {
                writer.WriteFloat(chordLabelPosOffset);
            }
            if (revision >= 8)
            {
                if (revision < 10)
                {
                    Symbol.Write(writer, unkTrig2);
                    Symbol.Write(writer, unkTrig3);
                }
                Symbol.Write(writer, peakStateOnTrig);
                Symbol.Write(writer, peakStateOffTrig);
            }

            if (revision >= 12)
            {
                for (int i = 0; i < 4; i++)
                {
                    Symbol.Write(writer, GetFixedSymbol(fillLaneAnims, i));
                }
            }
        }

        SaveTrack(writer, entry.isProxy, false, false);
        base.Write(writer, false, parent, entry);

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
