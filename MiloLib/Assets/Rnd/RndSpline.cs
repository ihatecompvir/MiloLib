using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("Spline"), Description("sexy sexy cubic spline")]
public class RndSpline : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Ctrl Points")]
    public List<CtrlPoint> ctrlPoints = new();
    [Name("Manual")]
    public bool manual;
    [Name("Pulse Length")]
    public float pulseLength;
    [Name("Pulse Amplitude")]
    public float pulseAmplitude;
    [Name("Start Ctrl Point"), Description("Control point to start drawing from (set -1 to start from the beginning)")]
    public int startCtrlPoint;
    [Name("End Ctrl Point"), Description("Control point to stop drawing at (set -1 to stop at the end)")]
    public int endCtrlPoint;
    [Name("Y Offset")]
    public float yOffset;
    [Name("Y Per Ctrl Point")]
    public float yPerCtrlPoint;
    [Name("Verticality"), Description("How vertical this spline tries to keep things; 1=vertical, 0=perpendicular to the ground"), MinVersion(2)]
    public float verticality;
    public bool globalDefault;
    public class CtrlPoint
    {
        [Name("Pos")]
        public Vector3 position = new();
        [Name("Roll")]
        public float roll;

        public CtrlPoint Read(EndianReader reader)
        {
            position.Read(reader);
            roll = reader.ReadFloat();
            return this;
        }

        public void Write(EndianWriter writer)
        {
            position.Write(writer);
            writer.WriteFloat(roll);
        }
    }

    public new RndSpline Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 2);
        base.Read(reader, false, parent, entry);
        ctrlPoints = AssetHelpers.ReadList(reader, () => new CtrlPoint().Read(reader));
        manual = reader.ReadBoolean();
        if (revision >= 1)
        {
            pulseLength = reader.ReadFloat();
            pulseAmplitude = reader.ReadFloat();
        }
        startCtrlPoint = reader.ReadInt32();
        endCtrlPoint = reader.ReadInt32();
        yOffset = reader.ReadFloat();
        yPerCtrlPoint = reader.ReadFloat();
        if (revision >= 2)
        {
            verticality = reader.ReadFloat();
        }

        globalDefault = reader.ReadBoolean();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 2);
        base.Write(writer, false, parent, entry);
        AssetHelpers.WriteList(writer, ctrlPoints, item => item.Write(writer));
        writer.WriteBoolean(manual);
        if (revision >= 1)
        {
            writer.WriteFloat(pulseLength);
            writer.WriteFloat(pulseAmplitude);
        }
        writer.WriteInt32(startCtrlPoint);
        writer.WriteInt32(endCtrlPoint);
        writer.WriteFloat(yOffset);
        writer.WriteFloat(yPerCtrlPoint);
        if (revision >= 2)
        {
            writer.WriteFloat(verticality);
        }

        writer.WriteBoolean(globalDefault);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
