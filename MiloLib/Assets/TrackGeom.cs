using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("TrackGeom")]
public class TrackGeom : Object
{
    private ushort revision = 12;
    private ushort altRevision;
    public Rnd.RndDrawable draw = new();
    public Rnd.RndTrans trans = new();
    [Name("Track Type")]
    public int trackType;
    [Name("Spline")]
    public Symbol spline = Symbol.Empty;
    [Name("Material")]
    public Symbol material = Symbol.Empty;
    [Name("Geom Owner")]
    public Symbol geomOwner = Symbol.Empty;
    [Name("Segment Mesh")]
    public Symbol segmentMesh = Symbol.Empty;
    [Name("Segment Subdivisions")]
    public int segmentSubdivisions;
    [Name("Segments Per Ctrl Point")]
    public int segmentsPerCtrlPoint;
    [Name("Charge Effect")]
    public bool chargeEffect;
    [Name("Charge Effect Beat Pulse")]
    public float chargeEffectBeatPulse;
    [Name("Charge Effect Width Frac")]
    public float chargeEffectWidthFrac;
    [Name("Charge Effect Amplitude")]
    public float chargeEffectAmplitude;
    [Name("Charge Effect Freq")]
    public float chargeEffectFreq;
    [Name("Charge Effect Period Offset")]
    public float chargeEffectPeriodOffset;
    [Name("Charge Effect Scroll Speed")]
    public float chargeEffectScrollSpeed;
    [Name("Charge Effect Axis")]
    public Vector3 chargeEffectAxis = new();

    public new TrackGeom Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 12);
        if (revision != 12)
        {
            throw new NotSupportedException($"Unsupported TrackGeom revision {revision}.");
        }

        base.Read(reader, false, parent, entry);
        draw = new Rnd.RndDrawable().Read(reader, false, parent, entry);
        trans = new Rnd.RndTrans().Read(reader, false, parent, entry);
        trackType = reader.ReadInt32();
        spline = Symbol.Read(reader);
        material = Symbol.Read(reader);
        geomOwner = Symbol.Read(reader);
        segmentMesh = Symbol.Read(reader);
        segmentSubdivisions = reader.ReadInt32();
        segmentsPerCtrlPoint = reader.ReadInt32();
        chargeEffect = reader.ReadBoolean();
        chargeEffectBeatPulse = reader.ReadFloat();
        chargeEffectWidthFrac = reader.ReadFloat();
        chargeEffectAmplitude = reader.ReadFloat();
        chargeEffectFreq = reader.ReadFloat();
        chargeEffectPeriodOffset = reader.ReadFloat();
        chargeEffectScrollSpeed = reader.ReadFloat();
        chargeEffectAxis.Read(reader);
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (revision != 12)
        {
            throw new NotSupportedException($"Unsupported TrackGeom revision {revision}.");
        }

        AssetHelpers.WriteRevision(writer, revision, altRevision, 12);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        trans.Write(writer, false, parent, entry);
        writer.WriteInt32(trackType);
        Symbol.Write(writer, spline);
        Symbol.Write(writer, material);
        Symbol.Write(writer, geomOwner);
        Symbol.Write(writer, segmentMesh);
        writer.WriteInt32(segmentSubdivisions);
        writer.WriteInt32(segmentsPerCtrlPoint);
        writer.WriteBoolean(chargeEffect);
        writer.WriteFloat(chargeEffectBeatPulse);
        writer.WriteFloat(chargeEffectWidthFrac);
        writer.WriteFloat(chargeEffectAmplitude);
        writer.WriteFloat(chargeEffectFreq);
        writer.WriteFloat(chargeEffectPeriodOffset);
        writer.WriteFloat(chargeEffectScrollSpeed);
        chargeEffectAxis.Write(writer);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}

