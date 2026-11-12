using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharLookAt"), Description("Makes a source point at dest by rotating a pivot, points along Y axis of source")]
public class CharLookAt : Object
{
    private ushort altRevision;
    private ushort revision;

    public CharWeightable weightable = new();

    [Name("Source"), Description("If non null, the bone which looks at along its Y axis, otherwise equal to the pivot")]
    public Symbol source = Symbol.Empty;

    [Name("Pivot"), Description("The thing that pivots")]
    public Symbol pivot = Symbol.Empty;

    [Name("Target"), Description("The thing to look at")]
    public Symbol target = Symbol.Empty;

    [Name("Half Time"), Description("Seconds of lag when moving the source")]
    public float halfTime;

    [Name("Min Yaw"), Description("Degrees of min allowable yaw, looking left")]
    public float minYaw;

    [Name("Max Yaw"), Description("Degrees of max allowable yaw, looking right")]
    public float maxYaw;

    [Name("Min Pitch"), Description("Degrees of min allowable pitch, looking down")]
    public float minPitch;

    [Name("Max Pitch"), Description("Degrees of max allowable pitch, looking up")]
    public float maxPitch;

    [Name("Min Weight Yaw"), Description("Degrees of yaw to start auto-weight, -1 means no auto-weight"), MinVersion(2)]
    public float minWeightYaw;

    [Name("Max Weight Yaw"), Description("Degrees of yaw to stop auto-weight, must be greater than mMinWeightYaw"), MinVersion(2)]
    public float maxWeightYaw;

    [Name("Weight Yaw Speed"), Description("Max speed in weight/sec that the auto-weight can change"), MinVersion(2)]
    public float weightYawSpeed;

    [Name("Allow Roll"), Description("If true allows rolling, if false, keeps the local pivot z axis down to prevent rolling. Eyeballs can't roll, for instance, but heads can."), MinVersion(3)]
    public bool allowRoll = true;

    [Name("Enable Jitter"), Description("If enabled, high frequency noise is added to pitch and/or yaw each frame"), MinVersion(4)]
    public bool enableJitter;

    [Name("Pitch Jitter Limit"), Description("if enable_jitter is on, random noise from [-pitch_jitter_limit, pitch_jitter_limit] (in degrees) is applied to the pitch of the lookat"), MinVersion(4)]
    public float pitchJitterLimit;

    [Name("Yaw Jitter Limit"), Description("if enable_jitter is on, random noise from [-yaw_jitter_limit, yaw_jitter_limit] (in degrees) is applied to the yaw of the lookat"), MinVersion(4)]
    public float yawJitterLimit;

    [Name("Source Radius"), Description("radius in degrees of filtered source motion that's allowed through"), MinVersion(5)]
    public float sourceRadius;

    public new CharLookAt Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);
        weightable = weightable.Read(reader, false, parent, entry);

        source = Symbol.Read(reader);
        pivot = Symbol.Read(reader);
        target = Symbol.Read(reader);
        halfTime = reader.ReadFloat();
        minYaw = reader.ReadFloat();
        maxYaw = reader.ReadFloat();
        minPitch = reader.ReadFloat();
        maxPitch = reader.ReadFloat();

        if (revision > 1)
        {
            minWeightYaw = reader.ReadFloat();
            maxWeightYaw = reader.ReadFloat();
            weightYawSpeed = reader.ReadFloat();
        }

        if (revision < 3)
        {
            allowRoll = true;
        }
        else
        {
            allowRoll = reader.ReadBoolean();
        }

        if (revision < 4)
        {
            enableJitter = false;
            pitchJitterLimit = 0.0f;
            yawJitterLimit = 0.0f;
        }
        else
        {
            enableJitter = reader.ReadBoolean();
            pitchJitterLimit = reader.ReadFloat();
            yawJitterLimit = reader.ReadFloat();
        }

        if (revision > 4)
        {
            sourceRadius = reader.ReadFloat();
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
        weightable.Write(writer, false, parent, entry);

        Symbol.Write(writer, source);
        Symbol.Write(writer, pivot);
        Symbol.Write(writer, target);
        writer.WriteFloat(halfTime);
        writer.WriteFloat(minYaw);
        writer.WriteFloat(maxYaw);
        writer.WriteFloat(minPitch);
        writer.WriteFloat(maxPitch);

        if (revision > 1)
        {
            writer.WriteFloat(minWeightYaw);
            writer.WriteFloat(maxWeightYaw);
            writer.WriteFloat(weightYawSpeed);
        }

        if (revision >= 3)
        {
            writer.WriteBoolean(allowRoll);
        }

        if (revision >= 4)
        {
            writer.WriteBoolean(enableJitter);
            writer.WriteFloat(pitchJitterLimit);
            writer.WriteFloat(yawJitterLimit);
        }

        if (revision > 4)
        {
            writer.WriteFloat(sourceRadius);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
