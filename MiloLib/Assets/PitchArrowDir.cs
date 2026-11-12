using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("PitchArrowDir"), Description("singer's vocal HUD pitch guide arrow")]
public class PitchArrowDir : RndDir
{

    private ushort altRevision;
    private ushort revision;

    public bool unk18c;
    [Name("Score"), Description("the score for the current frame, normalized between 0 and 1")]
    public float score;
    [Name("Harmony FX"), Description("extra bonus fx that play when nailing harmony with another part")]
    public float harmonyFX;
    [Name("Volume"), Description("arrow alpha driven by the mic volume in the current frame, normalized between 0 and 1")]
    public float volume;
    [Name("Tilt"), Description("the tilt of the arrow direction in degrees")]
    public float tilt;
    [Name("Color Fade"), Description("controls the saturation of the arrow color: 0 = max saturation, 1 = no saturation")]
    public float colorFade;
    [Name("Spotlight")]
    public bool spotlight;
    [Name("Deploying")]
    public bool deploying;
    [Name("Pitched")]
    public bool pitched;
    public bool unk1ab;
    [Name("Test Color")]
    public Symbol testColor = Symbol.Empty;
    [Name("Arrow Style"), MinVersion(2)]
    public int arrowStyle;
    public Symbol scoreAnim = Symbol.Empty;
    public Symbol harmonyFXAnim = Symbol.Empty;
    public Symbol volumeAnim = Symbol.Empty;
    public Symbol tiltAnim = Symbol.Empty;
    public Symbol colorAnim = Symbol.Empty;
    public Symbol colorFadeAnim = Symbol.Empty;
    public Symbol splitAnim = Symbol.Empty;
    public Symbol arrowStyleAnim = Symbol.Empty;
    public Symbol setPitchedTrig = Symbol.Empty;
    public Symbol setUnpitchedTrig = Symbol.Empty;
    public Symbol spotlightStartTrig = Symbol.Empty;
    public Symbol spotlightEndTrig = Symbol.Empty;
    public Symbol deployStartTrig = Symbol.Empty;
    public Symbol deployEndTrig = Symbol.Empty;
    public Symbol ghostGrp = Symbol.Empty;
    public Symbol ghostFadeAnim = Symbol.Empty;
    public Symbol arrowFXGrp = Symbol.Empty;
    public bool unk280;
    [Name("Spin Speed"), Description("controls the spin speed for helix particle effects. Can be animated"), MinVersion(1)]
    public float spinSpeed;
    [Name("Spin Anim"), Description("this anim will loop its loop range according to the value of spin_speed"), MinVersion(1)]
    public Symbol spinAnim = Symbol.Empty;
    [Name("Spin Rest Frame"), Description("frame where spin_anim should be when spin_speed is zero"), MinVersion(1)]
    public float spinRestFrame;
    [Name("Spin Begin Frame"), Description("loop start frame for spin_anim"), MinVersion(1)]
    public float spinBeginFrame;
    [Name("Spin End Frame"), Description("loop end frame for spin_anim"), MinVersion(1)]
    public float spinEndFrame;
    public uint arrowFXGrpCount;
    public List<Symbol> arrowFXGrpList;
    public uint ghostGrpCount;
    public List<Symbol> ghostGrpList;


    public PitchArrowDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new PitchArrowDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        pitched = reader.ReadBoolean();
        spotlight = reader.ReadBoolean();
        deploying = reader.ReadBoolean();
        score = reader.ReadFloat();
        harmonyFX = reader.ReadFloat();
        volume = reader.ReadFloat();
        tilt = reader.ReadFloat();
        if (revision < 2)
        {
            testColor = Symbol.Read(reader);
        }

        colorFade = reader.ReadFloat();
        if (revision >= 1 && !entry.isProxy)
        {
            spinSpeed = reader.ReadFloat();
            spinAnim = Symbol.Read(reader);
            spinBeginFrame = reader.ReadFloat();
            spinEndFrame = reader.ReadFloat();
            spinRestFrame = reader.ReadFloat();
        }
        if (revision >= 2)
        {
            arrowStyle = reader.ReadInt32();
            testColor = Symbol.Read(reader);
        }

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

        writer.WriteBoolean(pitched);
        writer.WriteBoolean(spotlight);
        writer.WriteBoolean(deploying);
        writer.WriteFloat(score);
        writer.WriteFloat(harmonyFX);
        writer.WriteFloat(volume);
        writer.WriteFloat(tilt);
        if (revision < 2)
        {
            Symbol.Write(writer, testColor);
        }
        writer.WriteFloat(colorFade);
        if (revision >= 1 && !entry.isProxy)
        {
            writer.WriteFloat(spinSpeed);
            Symbol.Write(writer, spinAnim);
            writer.WriteFloat(spinBeginFrame);
            writer.WriteFloat(spinEndFrame);
            writer.WriteFloat(spinRestFrame);
        }
        if (revision >= 2)
        {
            writer.WriteInt32(arrowStyle);
            Symbol.Write(writer, testColor);
        }

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
