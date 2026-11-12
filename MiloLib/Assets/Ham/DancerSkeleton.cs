using MiloLib.Utils;
using Vector3 = MiloLib.Classes.Vector3;

namespace MiloLib.Assets.Ham;

public class DancerSkeleton
{
    public List<Vector3> camJointDisplacements = new(20);
    public List<Vector3> camJointPositions = new(20);
    public List<float> camBoneLengths = new(19);
    public int elapsedMs;
    public bool tracked;

    public override string ToString()
    {
        string str = "DancerSkeleton:\n";
        str += "Cam joint displacements:\n";
        for (int i = 0; i < 20; i++)
        {
            str += $"{(SkeletonJoint)i}: {camJointDisplacements[i]}\n";
        }
        str += "Cam joint positions:\n";
        for (int i = 0; i < 20; i++)
        {
            str += $"{(SkeletonJoint)i}: {camJointPositions[i]}\n";
        }
        //str += "Cam bone lengths:\n";
        //for(int i = 0; i < 19; i++) {
        //    str += $"{(SkeletonBone)i}: {mCamBoneLengths[i]}\n";
        //}
        str += $"Elapsed ms: {elapsedMs}\n";
        str += $"Tracked: {tracked}\n";
        return str;
    }
    public DancerSkeleton Read(EndianReader reader)
    {
        for (int i = 0; i < 20; i++)
        {
            camJointPositions.Add(new Vector3().Read(reader));
            camJointDisplacements.Add(new Vector3().Read(reader));
        }

        elapsedMs = reader.ReadInt32();
        tracked = reader.ReadBoolean();

        return this;
    }

    public void Write(EndianWriter writer)
    {
        for (int i = 0; i < 20; i++)
        {
            camJointPositions[i].Write(writer);
            camJointDisplacements[i].Write(writer);
        }

        writer.WriteInt32(elapsedMs);
        writer.WriteBoolean(tracked);
    }
}
