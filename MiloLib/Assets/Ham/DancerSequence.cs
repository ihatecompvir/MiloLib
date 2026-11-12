using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;
using Vector3 = MiloLib.Classes.Vector3;

namespace MiloLib.Assets.Ham;

[Name("DancerSequence"), Description("Linear sequence of DancerFrame structs, animatable for preview in milo")]
public class DancerSequence : Object
{

    private ushort revision;
    private ushort altRevision;

    public RndAnimatable rndAnimatable = new();

    public class DancerFrame
    {
        [MinVersion(1)]
        public short unk0;
        [MinVersion(1)]
        public short unk2;
        [MaxVersion(0)]
        public int dummyRev0;
        [MinVersion(7), MaxVersion(7)]
        public int[] jointDummies = new int[20];

        [MinVersion(7)]
        public DancerSkeleton skeleton = new();
    }

    public List<DancerFrame> dancerFrames = new();

    public override string ToString()
    {
        string str = $"DancerSequence: revs: ({revision}, {altRevision})\n";
        str += $"DancerFrames ({dancerFrames.Count}):\n\n";
        str += $"Showing the first 50 frames for performance purposes:\n";
        for (int i = 0; i < 50; i++)
        {
            str += $"DancerFrame {i} of {dancerFrames.Count}:\n";
            str += $"unk0 {dancerFrames[i].unk0} unk2 {dancerFrames[i].unk2}\n";
            str += dancerFrames[i].skeleton + "\n";
        }
        return str;
    }

    public new DancerSequence Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        // Read revision
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);
        // Base Read
        base.Read(reader, false, parent, entry);
        rndAnimatable.Read(reader, parent, entry);

        // DancerSequence length
        int numFrames = reader.ReadInt32();

        for (int i = 0; i < numFrames; i++)
        {
            DancerFrame curFrame = new();
            dancerFrames.Add(curFrame);

            // Unknown data
            if (revision == 0)
            {
                curFrame.dummyRev0 = reader.ReadInt32();
                curFrame.unk2 = -1;
                curFrame.unk0 = -1;
            }
            else
            {
                curFrame.unk0 = reader.ReadInt16();
                curFrame.unk2 = reader.ReadInt16();
            }

            DancerSkeleton skeleton = curFrame.skeleton;
            if (revision < 7)
            {
                // non-relevant stuff here, ignore lol
            }
            else
            {
                for (int k = 0; k < 20; k++)
                {
                    Vector3 vpos = new Vector3().Read(reader);
                    Vector3 vdisp = new Vector3().Read(reader);
                    skeleton.camJointPositions.Add(vpos);
                    skeleton.camJointDisplacements.Add(vdisp);
                    if (revision < 8)
                    {
                        curFrame.jointDummies[k] = reader.ReadInt32();
                    }
                }
                int elapsed = reader.ReadInt32();
                skeleton.elapsedMs = elapsed;
            }
        }



        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        // Revision
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        // Base Write
        base.Write(writer, false, parent, entry);
        rndAnimatable.Write(writer);

        // DancerSequence length
        writer.WriteInt32(dancerFrames.Count);

        foreach (DancerFrame curFrame in dancerFrames)
        {
            // Unknown data
            if (revision == 0)
            {
                writer.WriteInt32(curFrame.dummyRev0);
            }
            else
            {
                writer.WriteInt16(curFrame.unk0);
                writer.WriteInt16(curFrame.unk2);
            }

            DancerSkeleton skeleton = curFrame.skeleton;
            if (revision < 7)
            {
                // non-relevant stuff here, ignore lol
            }
            else
            {
                while (skeleton.camJointPositions.Count < 20)
                {
                    skeleton.camJointPositions.Add(new Vector3());
                }
                while (skeleton.camJointDisplacements.Count < 20)
                {
                    skeleton.camJointDisplacements.Add(new Vector3());
                }

                for (int k = 0; k < 20; k++)
                {
                    skeleton.camJointPositions[k].Write(writer);
                    skeleton.camJointDisplacements[k].Write(writer);
                    if (revision < 8)
                    {
                        writer.WriteInt32(curFrame.jointDummies[k]);
                    }
                }
                writer.WriteInt32(skeleton.elapsedMs);
            }
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
