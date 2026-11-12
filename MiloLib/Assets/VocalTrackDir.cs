using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("VocalTrackDir"), Description("band 2 dir for vocal track")]
public class VocalTrackDir : RndDir
{
    private static readonly HmxColor4 EmptyLyricColor = new();
    private bool hasTambourineReferences = true;

    public class LyricColor
    {
        public uint lyric;
        public HmxColor3 color = new();
        public uint unk;

        public LyricColor Read(EndianReader reader)
        {
            lyric = reader.ReadUInt32();
            color = new HmxColor3().Read(reader);
            unk = reader.ReadUInt32();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt32(lyric);
            color.Write(writer);
            writer.WriteUInt32(unk);
        }
    }

    public class LyricAlphaMap
    {
        public uint lyric;
        public float alpha;

        public LyricAlphaMap Read(EndianReader reader)
        {
            lyric = reader.ReadUInt32();
            alpha = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            writer.WriteUInt32(lyric);
            writer.WriteFloat(alpha);
        }
    }

    private ushort altRevision;
    private ushort revision;

    private ushort altTrackRevision;
    private ushort trackRevision;

    [MaxVersion(2)]
    public bool unkBool1;
    [MinVersion(2), MaxVersion(2)]
    public bool unkBool2;
    [MinVersion(2), MaxVersion(2)]
    public bool unkBool3;
    [MinVersion(2), MaxVersion(2)]
    public bool unkBool4;
    [MinVersion(2)]
    private uint lyricColorsCount;
    [MinVersion(2)]
    public List<LyricColor> lyricColors = new(32);
    [MaxVersion(1)]
    public List<HmxColor4> lyricColorsOld = new(14);
    [MinVersion(2)]
    private uint lyricAlphaMapCount;
    [MinVersion(2)]
    public List<LyricAlphaMap> lyricAlphaMaps = new(32);
    public Symbol vocalMics = Symbol.Empty;
    public Symbol unkSymbol1 = Symbol.Empty;
    public float minPitchRange;
    public float arrowSmoothing;
    private uint configurableObjectsCount;
    [Name("Configurable Objects"), Description("Objects which can change transform or showing per configuration.")]
    public List<Symbol> configurableObjects = new();
    [Name("Vox Configuration"), MinVersion(2)]
    public Symbol voxCfg = Symbol.Empty;
    [Name("Tambourine Smasher"), MinVersion(6)]
    public Symbol tambourineSmasher = Symbol.Empty;
    [Name("Tambourine Now Show Trig"), MinVersion(6)]
    public Symbol tambourineNowShowTrig = Symbol.Empty;
    [Name("Tambourine Now Hide Trig"), MinVersion(6)]
    public Symbol tambourineNowHideTrig = Symbol.Empty;
    [Name("Phrase Feedback Trig")]
    public Symbol phraseFeedbackTrig = Symbol.Empty;
    [Name("Spotlight Sparkles Only Trig")]
    public Symbol spotlightSparklesOnlyTrig = Symbol.Empty;
    [Name("Spotlight Phrase Success Trig")]
    public Symbol spotlightPhraseSuccessTrig = Symbol.Empty;
    public Symbol pitchArrow1 = Symbol.Empty;
    public Symbol pitchArrow2 = Symbol.Empty;
    public Symbol pitchArrow3 = Symbol.Empty;
    [Name("Pitch Window"), Description("are we displaying the note tube panel?"), MinVersion(2)]
    public bool pitchWindow;
    [Name("Pitch Window Height"), Description("height of the note tube panel"), MinVersion(2)]
    public float pitchWindowHeight;
    [Name("Pitch Window Mesh"), Description("the pitch window mesh"), MinVersion(2)]
    public Symbol pitchWindowMesh = Symbol.Empty;
    [Name("Pitch Window Overlay"), Description("the pitch window overlay mesh"), MinVersion(2)]
    public Symbol pitchWindowOverlay = Symbol.Empty;
    [Name("Lead Lyrics"), Description("are we showing the lead lyric panel?"), MinVersion(2)]
    public bool leadLyrics;
    [Name("Lead Lyric Height"), Description("controls the height of the lead lyric bar"), MinVersion(2)]
    public float leadLyricHeight;
    [Name("Lead Lyric Mesh"), Description("the lead lyric bar mesh"), MinVersion(2)]
    public Symbol leadLyricMesh = Symbol.Empty;
    [Name("Harmony Lyrics"), Description("are we showing the harmony lyric panel?"), MinVersion(2)]
    public bool harmLyrics;
    [Name("Harmony Lyric Height"), Description("controls the height of the harmony lyric bar"), MinVersion(2)]
    public float harmLyricHeight;
    [Name("Harmony Lyric Mesh"), Description("the harmony lyric bar mesh"), MinVersion(2)]
    public Symbol harmLyricMesh = Symbol.Empty;
    [Name("Left Deco Mesh"), MinVersion(2)]
    public Symbol leftDecoMesh = Symbol.Empty;
    [Name("Right Deco Mesh"), MinVersion(2)]
    public Symbol rightDecoMesh = Symbol.Empty;
    [Name("Now Bar Width"), Description("the width of the now bar mesh"), MinVersion(2)]
    public float nowBarWidth;
    [Name("Now Bar Mesh"), MinVersion(2)]
    public Symbol nowBarMesh = Symbol.Empty;
    [MinVersion(2)]
    public bool remoteVocals;
    [MinVersion(2), MaxVersion(2)]
    public float unkFloat1;
    [MinVersion(2)]
    public float trackLeftX;
    [MinVersion(2)]
    public float trackRightX;
    [MinVersion(2)]
    public float trackBottomZ;
    [MinVersion(2)]
    public float trackTopZ;
    [MinVersion(2)]
    public float pitchBottomZ;
    [MinVersion(2)]
    public float pitchTopZ;
    [MinVersion(2)]
    public float nowBarX;
    [Name("Pitch Guides"), MinVersion(2)]
    public Symbol pitchGuides = Symbol.Empty;
    [Name("Tube Style"), MinVersion(2)]
    public Symbol tubeStyle = Symbol.Empty;
    [Name("Arrow Style"), MinVersion(2)]
    public Symbol arrowStyle = Symbol.Empty;
    [Name("Font Style"), MinVersion(2)]
    public Symbol fontStyle = Symbol.Empty;
    [MinVersion(2)]
    public Symbol leadText = Symbol.Empty;
    [MinVersion(2)]
    public Symbol harmText = Symbol.Empty;
    [MinVersion(4)]
    public Symbol leadPhonemeText = Symbol.Empty;
    [MinVersion(4)]
    public Symbol harmPhonemeText = Symbol.Empty;
    public Symbol rangeScaleAnim = Symbol.Empty;
    public Symbol rangeOffsetAnim = Symbol.Empty;
    public Symbol tubeRangeGrp = Symbol.Empty;
    public Symbol tubeSpotlightGrp = Symbol.Empty;
    public Symbol tubeBack0Grp = Symbol.Empty;
    public Symbol tubeBack1Grp = Symbol.Empty;
    public Symbol tubeBack2Grp = Symbol.Empty;
    public Symbol tubeFront0Grp = Symbol.Empty;
    public Symbol tubeFront1Grp = Symbol.Empty;
    public Symbol tubeFront2Grp = Symbol.Empty;
    public Symbol tubeGlow0Grp = Symbol.Empty;
    public Symbol tubeGlow1Grp = Symbol.Empty;
    public Symbol tubeGlow2Grp = Symbol.Empty;
    public Symbol tubePhoneme0Grp = Symbol.Empty;
    public Symbol tubePhoneme1Grp = Symbol.Empty;
    public Symbol tubePhoneme2Grp = Symbol.Empty;
    public Symbol spotlightMat = Symbol.Empty;
    public Symbol leadBackMat = Symbol.Empty;
    public Symbol harm1BackMat = Symbol.Empty;
    public Symbol harm2BackMat = Symbol.Empty;
    public Symbol leadFrontMat = Symbol.Empty;
    public Symbol harm1FrontMat = Symbol.Empty;
    public Symbol harm2FrontMat = Symbol.Empty;
    public Symbol leadGlowMat = Symbol.Empty;
    public Symbol harm1GlowMat = Symbol.Empty;
    public Symbol harm2GlowMat = Symbol.Empty;
    public Symbol leadPhonemeMat = Symbol.Empty;
    public Symbol harm1PhonemeMat = Symbol.Empty;
    public Symbol harm2PhonemeMat = Symbol.Empty;
    public Symbol vocalsGrp = Symbol.Empty;
    public Symbol breGrp = Symbol.Empty;
    public Symbol leadBreGrp = Symbol.Empty;
    public Symbol harmonyBreGrp = Symbol.Empty;
    public Symbol pitchScrollGroup = Symbol.Empty;
    public Symbol leadLyricScrollGroup = Symbol.Empty;
    public Symbol harmonyLyricScrollGroup = Symbol.Empty;
    [MinVersion(2), MaxVersion(6)]
    public float unkFloat2;
    [MinVersion(2), MaxVersion(2)]
    public float unkFloat3;
    public float unkFloat4;
    public float unkFloat5;
    [Name("Lead Deploy Mat"), MinVersion(4)]
    public Symbol leadDeployMat = Symbol.Empty;
    [Name("Harmony Deploy Mat"), MinVersion(4)]
    public Symbol harmDeployMat = Symbol.Empty;
    public Symbol arrowFxDrawGrp = Symbol.Empty;
    public float unkFloat6;
    public float unkFloat7;
    public bool unkBool6;

    [MinVersion(2)]
    public float lastMin;
    [MinVersion(2)]
    public float lastMax;
    [MinVersion(2)]
    public float middleCZPos;
    [MinVersion(2)]
    public int tonic;

    [MaxVersion(1)]
    public uint unkInt1;
    [MaxVersion(1)]
    public uint unkInt2;
    [MaxVersion(1)]
    public float unkFloat8;
    [MaxVersion(1)]
    public uint unkInt3;

    [MaxVersion(2)]
    public Symbol unkSymbol2 = Symbol.Empty;
    [MaxVersion(2)]
    public Symbol unkSymbol3 = Symbol.Empty;
    [MaxVersion(2)]
    public Symbol unkSymbol4 = Symbol.Empty;
    [MaxVersion(2)]
    public Symbol unkSymbol5 = Symbol.Empty;

    [MaxVersion(2)]
    public Symbol unkSymbol6 = Symbol.Empty;
    [MaxVersion(2)]
    public Symbol unkSymbol7 = Symbol.Empty;
    [MaxVersion(2)]
    public Symbol unkSymbol8 = Symbol.Empty;
    [MaxVersion(2)]
    public Symbol unkSymbol9 = Symbol.Empty;

    [MaxVersion(1)]
    public HmxColor4 unkColor = new();
    [MaxVersion(1)]
    public HmxColor4 unkColor2 = new();

    [MaxVersion(1)]
    public float unkFloat9;

    [MaxVersion(1)]
    public float unkFloat10;

    [MinVersion(1), MaxVersion(1)]
    public float unkFloat11;

    [MinVersion(1), MaxVersion(1)]
    public float unkFloat12;
    [MaxVersion(1)]
    public float unkFloat13;

    [MaxVersion(1)]
    public float unkFloat14;

    [MaxVersion(1)]
    public float unkFloat15;
    public float unkFloat16;

    [MaxVersion(2)]
    public Symbol unkSymbol10 = Symbol.Empty;
    [MaxVersion(2)]
    public Symbol unkSymbol11 = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol unkSymbol12 = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol unkSymbol13 = Symbol.Empty;

    [MaxVersion(1)]
    public Symbol unkSymbol14 = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol unkSymbol15 = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol unkSymbol16 = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol unkSymbol17 = Symbol.Empty;

    [MaxVersion(2)]
    private uint colorsCount;
    [MaxVersion(2)]
    public List<HmxColor4> colors = new();

    [MaxVersion(1)]
    public List<float> floats = new();

    [MaxVersion(1)]
    public Symbol unkSymbol18 = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol unkSymbol19 = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol unkSymbol20 = Symbol.Empty;
    [MaxVersion(1)]
    public Symbol unkSymbol21 = Symbol.Empty;

    [MinVersion(2), MaxVersion(4)]
    public Symbol streakMeter1 = Symbol.Empty;
    [MinVersion(2), MaxVersion(4)]
    public Symbol streakMeter2 = Symbol.Empty;

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

    public VocalTrackDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

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

    public new VocalTrackDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        long bodyStart = reader.BaseStream.Position;
        bool canRetry = parent.revision == 25 && revision == 6 && !entry.isProxy;
        hasTambourineReferences = true;
        while (true)
        {
            try
            {
                ReadBody(reader, entry);
                LoadTrack(reader, entry.isProxy, false, true);
                if (standalone)
                {
                    AssetHelpers.ReadEnd(reader, parent, entry, revision);
                }

                return this;
            }
            catch (Exception ex) when (canRetry && (ex is IOException || ex is InvalidDataException || ex is MiloLib.Exceptions.MiloAssetReadException))
            {
                // Beatles doesn't have the tamborine refs but has the same asset revision, so we must do some dumb shit to catch this
                reader.BaseStream.Position = bodyStart;
                hasTambourineReferences = false;
                canRetry = false;
            }
        }
    }

    private static uint ReadBodyCount(EndianReader reader, int minimumBytes)
    {
        uint count = reader.ReadUInt32();
        if (count > (reader.BaseStream.Length - reader.BaseStream.Position) / minimumBytes)
        {
            throw new InvalidDataException($"Invalid VocalTrackDir list count {count}.");
        }

        return count;
    }

    private void ReadBody(EndianReader reader, DirectoryMeta.Entry entry)
    {
        colors.Clear();
        lyricColors.Clear();
        lyricAlphaMaps.Clear();
        tambourineSmasher = Symbol.Empty;
        tambourineNowShowTrig = Symbol.Empty;
        tambourineNowHideTrig = Symbol.Empty;

        if (!entry.isProxy)
        {
            if (revision < 2)
            {
                unkInt1 = reader.ReadUInt32();
                unkInt2 = reader.ReadUInt32();
                unkFloat8 = reader.ReadFloat();
                unkInt3 = reader.ReadUInt32();

                configurableObjectsCount = ReadBodyCount(reader, 4);
                configurableObjects = new List<Symbol>();
                for (uint i = 0; i < configurableObjectsCount; i++)
                {
                    configurableObjects.Add(Symbol.Read(reader));
                }


                for (int i = 0; i < 14; i++)
                {
                    lyricColorsOld.Add(new HmxColor4().Read(reader));
                }

                unkSymbol2 = Symbol.Read(reader);
                unkSymbol3 = Symbol.Read(reader);
                unkSymbol4 = Symbol.Read(reader);
                unkSymbol5 = Symbol.Read(reader);

                unkSymbol6 = Symbol.Read(reader);
                unkSymbol7 = Symbol.Read(reader);
                unkSymbol8 = Symbol.Read(reader);
                unkSymbol9 = Symbol.Read(reader);


                unkColor = new HmxColor4().Read(reader);
                unkColor2 = new HmxColor4().Read(reader);

                unkFloat9 = reader.ReadFloat();
                unkFloat10 = reader.ReadFloat();

                if (revision >= 1)
                {
                    unkFloat11 = reader.ReadFloat();
                    unkFloat12 = reader.ReadFloat();
                }

                unkFloat13 = reader.ReadFloat();

                unkSymbol1 = Symbol.Read(reader);

                unkBool1 = reader.ReadBoolean();

                minPitchRange = reader.ReadFloat();
                arrowSmoothing = reader.ReadFloat();

                unkFloat14 = reader.ReadFloat();
                unkFloat15 = reader.ReadFloat();

                colors = AssetHelpers.ReadList(reader, () => new HmxColor4().Read(reader));

                floats = AssetHelpers.ReadList(reader, () => reader.ReadFloat());

                unkSymbol10 = Symbol.Read(reader);
                unkSymbol11 = Symbol.Read(reader);
                unkSymbol12 = Symbol.Read(reader);
                unkSymbol13 = Symbol.Read(reader);

                unkSymbol14 = Symbol.Read(reader);
                unkSymbol15 = Symbol.Read(reader);
                unkSymbol16 = Symbol.Read(reader);
                unkSymbol17 = Symbol.Read(reader);


                phraseFeedbackTrig = Symbol.Read(reader);
                spotlightSparklesOnlyTrig = Symbol.Read(reader);
                spotlightPhraseSuccessTrig = Symbol.Read(reader);

                unkSymbol18 = Symbol.Read(reader);
                unkSymbol19 = Symbol.Read(reader);
                unkSymbol20 = Symbol.Read(reader);
                unkSymbol21 = Symbol.Read(reader);
            }
            else
            {
                configurableObjectsCount = ReadBodyCount(reader, 4);
                configurableObjects = new List<Symbol>();
                for (uint i = 0; i < configurableObjectsCount; i++)
                {
                    configurableObjects.Add(Symbol.Read(reader));
                }

                voxCfg = Symbol.Read(reader);
                unkSymbol1 = Symbol.Read(reader);
                minPitchRange = reader.ReadFloat();
                arrowSmoothing = reader.ReadFloat();

                if (revision < 3)
                {
                    unkFloat1 = reader.ReadFloat();
                }

                if (revision < 7)
                {
                    unkFloat2 = reader.ReadFloat();
                }

                if (revision < 3)
                {
                    colorsCount = ReadBodyCount(reader, 16);
                    for (int i = 0; i < colorsCount; i++)
                    {
                        colors.Add(new HmxColor4().Read(reader));
                    }

                    unkSymbol2 = Symbol.Read(reader);
                    unkSymbol3 = Symbol.Read(reader);
                    unkSymbol4 = Symbol.Read(reader);
                    unkSymbol5 = Symbol.Read(reader);
                }
                else if (revision >= 6 && hasTambourineReferences)
                {
                    tambourineSmasher = Symbol.Read(reader);
                    tambourineNowShowTrig = Symbol.Read(reader);
                    tambourineNowHideTrig = Symbol.Read(reader);
                }

                phraseFeedbackTrig = Symbol.Read(reader);
                spotlightSparklesOnlyTrig = Symbol.Read(reader);
                spotlightPhraseSuccessTrig = Symbol.Read(reader);

                lyricColorsCount = ReadBodyCount(reader, 20);
                for (int i = 0; i < lyricColorsCount; i++)
                {
                    LyricColor lyricColor = new LyricColor().Read(reader);
                    lyricColors.Add(lyricColor);
                }

                lyricAlphaMapCount = ReadBodyCount(reader, 8);
                for (int i = 0; i < lyricAlphaMapCount; i++)
                {
                    LyricAlphaMap lyricAlphaMap = new LyricAlphaMap().Read(reader);
                    lyricAlphaMaps.Add(lyricAlphaMap);
                }

                if (revision < 5)
                {
                    streakMeter1 = Symbol.Read(reader);
                    streakMeter2 = Symbol.Read(reader);
                }



                pitchWindow = reader.ReadBoolean();
                pitchWindowHeight = reader.ReadFloat();
                pitchWindowMesh = Symbol.Read(reader);
                pitchWindowOverlay = Symbol.Read(reader);
                leadLyrics = reader.ReadBoolean();
                leadLyricHeight = reader.ReadFloat();
                leadLyricMesh = Symbol.Read(reader);
                harmLyrics = reader.ReadBoolean();
                harmLyricHeight = reader.ReadFloat();
                harmLyricMesh = Symbol.Read(reader);

                if (revision < 3)
                {
                    unkSymbol6 = Symbol.Read(reader);
                    unkSymbol7 = Symbol.Read(reader);
                    unkSymbol8 = Symbol.Read(reader);
                    unkSymbol9 = Symbol.Read(reader);

                    unkFloat3 = reader.ReadFloat();
                    unkBool1 = reader.ReadBoolean();
                }

                leftDecoMesh = Symbol.Read(reader);
                rightDecoMesh = Symbol.Read(reader);
                nowBarWidth = reader.ReadFloat();
                if (revision < 3)
                {
                    unkBool2 = reader.ReadBoolean();
                }

                nowBarMesh = Symbol.Read(reader);

                remoteVocals = reader.ReadBoolean();
                trackLeftX = reader.ReadFloat();
                trackRightX = reader.ReadFloat();
                trackBottomZ = reader.ReadFloat();
                trackTopZ = reader.ReadFloat();
                pitchBottomZ = reader.ReadFloat();
                pitchTopZ = reader.ReadFloat();
                nowBarX = reader.ReadFloat();
                pitchGuides = Symbol.Read(reader);
                tubeStyle = Symbol.Read(reader);
                arrowStyle = Symbol.Read(reader);
                fontStyle = Symbol.Read(reader);

                leadText = Symbol.Read(reader);
                harmText = Symbol.Read(reader);

                if (revision >= 4)
                {
                    leadPhonemeText = Symbol.Read(reader);
                    harmPhonemeText = Symbol.Read(reader);
                }

                lastMin = reader.ReadFloat();
                lastMax = reader.ReadFloat();
                middleCZPos = reader.ReadFloat();
                tonic = reader.ReadInt32();

                if (revision < 3)
                {
                    unkSymbol10 = Symbol.Read(reader);
                    unkSymbol11 = Symbol.Read(reader);
                    unkBool3 = reader.ReadBoolean();
                    unkBool4 = reader.ReadBoolean();
                }



                rangeScaleAnim = Symbol.Read(reader);
                rangeOffsetAnim = Symbol.Read(reader);

                if (revision >= 4)
                {
                    leadDeployMat = Symbol.Read(reader);
                    harmDeployMat = Symbol.Read(reader);
                }
            }
        }

    }


    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);

        if (!entry.isProxy)
        {
            if (revision < 2)
            {
                writer.WriteUInt32(unkInt1);
                writer.WriteUInt32(unkInt2);
                writer.WriteFloat(unkFloat8);
                writer.WriteUInt32(unkInt3);

                AssetHelpers.WriteList(writer, configurableObjects, obj => Symbol.Write(writer, obj));

                for (int i = 0; i < 14; i++)
                {
                    HmxColor4 lyricColor = EmptyLyricColor;
                    if (i < lyricColorsOld.Count && lyricColorsOld[i] != null)
                    {
                        lyricColor = lyricColorsOld[i];
                    }
                    lyricColor.Write(writer);
                }

                Symbol.Write(writer, unkSymbol2);
                Symbol.Write(writer, unkSymbol3);
                Symbol.Write(writer, unkSymbol4);
                Symbol.Write(writer, unkSymbol5);

                Symbol.Write(writer, unkSymbol6);
                Symbol.Write(writer, unkSymbol7);
                Symbol.Write(writer, unkSymbol8);
                Symbol.Write(writer, unkSymbol9);

                unkColor.Write(writer);
                unkColor2.Write(writer);

                writer.WriteFloat(unkFloat9);
                writer.WriteFloat(unkFloat10);

                if (revision >= 1)
                {
                    writer.WriteFloat(unkFloat11);
                    writer.WriteFloat(unkFloat12);
                }

                writer.WriteFloat(unkFloat13);
                Symbol.Write(writer, unkSymbol1);
                writer.WriteBoolean(unkBool1);

                writer.WriteFloat(minPitchRange);
                writer.WriteFloat(arrowSmoothing);
                writer.WriteFloat(unkFloat14);
                writer.WriteFloat(unkFloat15);

                AssetHelpers.WriteList(writer, colors, color => color.Write(writer));

                AssetHelpers.WriteList(writer, floats, value => writer.WriteFloat(value));

                Symbol.Write(writer, unkSymbol10);
                Symbol.Write(writer, unkSymbol11);
                Symbol.Write(writer, unkSymbol12);
                Symbol.Write(writer, unkSymbol13);

                Symbol.Write(writer, unkSymbol14);
                Symbol.Write(writer, unkSymbol15);
                Symbol.Write(writer, unkSymbol16);
                Symbol.Write(writer, unkSymbol17);

                Symbol.Write(writer, phraseFeedbackTrig);
                Symbol.Write(writer, spotlightSparklesOnlyTrig);
                Symbol.Write(writer, spotlightPhraseSuccessTrig);

                Symbol.Write(writer, unkSymbol18);
                Symbol.Write(writer, unkSymbol19);
                Symbol.Write(writer, unkSymbol20);
                Symbol.Write(writer, unkSymbol21);
            }
            else
            {
                AssetHelpers.WriteList(writer, configurableObjects, obj => Symbol.Write(writer, obj));

                Symbol.Write(writer, voxCfg);
                Symbol.Write(writer, unkSymbol1);
                writer.WriteFloat(minPitchRange);
                writer.WriteFloat(arrowSmoothing);

                if (revision < 3)
                {
                    writer.WriteFloat(unkFloat1);
                }

                if (revision < 7)
                {
                    writer.WriteFloat(unkFloat2);
                }

                if (revision < 3)
                {
                    AssetHelpers.WriteList(writer, colors, color => color.Write(writer));

                    Symbol.Write(writer, unkSymbol2);
                    Symbol.Write(writer, unkSymbol3);
                    Symbol.Write(writer, unkSymbol4);
                    Symbol.Write(writer, unkSymbol5);
                }
                else if (revision >= 6 && hasTambourineReferences)
                {
                    Symbol.Write(writer, tambourineSmasher);
                    Symbol.Write(writer, tambourineNowShowTrig);
                    Symbol.Write(writer, tambourineNowHideTrig);
                }

                Symbol.Write(writer, phraseFeedbackTrig);
                Symbol.Write(writer, spotlightSparklesOnlyTrig);
                Symbol.Write(writer, spotlightPhraseSuccessTrig);

                AssetHelpers.WriteList(writer, lyricColors, lyricColor => lyricColor.Write(writer));

                AssetHelpers.WriteList(writer, lyricAlphaMaps, lyricAlphaMap => lyricAlphaMap.Write(writer));

                if (revision < 5)
                {
                    Symbol.Write(writer, streakMeter1);
                    Symbol.Write(writer, streakMeter2);
                }

                writer.WriteBoolean(pitchWindow);
                writer.WriteFloat(pitchWindowHeight);
                Symbol.Write(writer, pitchWindowMesh);
                Symbol.Write(writer, pitchWindowOverlay);
                writer.WriteBoolean(leadLyrics);
                writer.WriteFloat(leadLyricHeight);
                Symbol.Write(writer, leadLyricMesh);
                writer.WriteBoolean(harmLyrics);
                writer.WriteFloat(harmLyricHeight);
                Symbol.Write(writer, harmLyricMesh);

                if (revision < 3)
                {
                    Symbol.Write(writer, unkSymbol6);
                    Symbol.Write(writer, unkSymbol7);
                    Symbol.Write(writer, unkSymbol8);
                    Symbol.Write(writer, unkSymbol9);

                    writer.WriteFloat(unkFloat3);
                    writer.WriteBoolean(unkBool1);
                }

                Symbol.Write(writer, leftDecoMesh);
                Symbol.Write(writer, rightDecoMesh);
                writer.WriteFloat(nowBarWidth);
                if (revision < 3)
                {
                    writer.WriteBoolean(unkBool2);
                }

                Symbol.Write(writer, nowBarMesh);

                writer.WriteBoolean(remoteVocals);
                writer.WriteFloat(trackLeftX);
                writer.WriteFloat(trackRightX);
                writer.WriteFloat(trackBottomZ);
                writer.WriteFloat(trackTopZ);
                writer.WriteFloat(pitchBottomZ);
                writer.WriteFloat(pitchTopZ);
                writer.WriteFloat(nowBarX);
                Symbol.Write(writer, pitchGuides);
                Symbol.Write(writer, tubeStyle);
                Symbol.Write(writer, arrowStyle);
                Symbol.Write(writer, fontStyle);

                Symbol.Write(writer, leadText);
                Symbol.Write(writer, harmText);

                if (revision >= 4)
                {
                    Symbol.Write(writer, leadPhonemeText);
                    Symbol.Write(writer, harmPhonemeText);
                }

                writer.WriteFloat(lastMin);
                writer.WriteFloat(lastMax);
                writer.WriteFloat(middleCZPos);
                writer.WriteInt32(tonic);

                if (revision < 3)
                {
                    Symbol.Write(writer, unkSymbol10);
                    Symbol.Write(writer, unkSymbol11);
                    writer.WriteBoolean(unkBool3);
                    writer.WriteBoolean(unkBool4);
                }

                Symbol.Write(writer, rangeScaleAnim);
                Symbol.Write(writer, rangeOffsetAnim);

                if (revision >= 4)
                {
                    Symbol.Write(writer, leadDeployMat);
                    Symbol.Write(writer, harmDeployMat);
                }
            }
        }

        SaveTrack(writer, entry.isProxy, false, true);

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
