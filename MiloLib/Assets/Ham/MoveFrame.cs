using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

public class OldNodeWeight
{
    public float weight1;
    public float rate1;
    public float weight2;
    public float rate2;
    public float oldEnabled;
    public bool enabled;

    public OldNodeWeight Read(EndianReader reader, ushort revision)
    {
        weight1 = reader.ReadFloat();
        rate1 = reader.ReadFloat();
        weight2 = reader.ReadFloat();
        rate2 = reader.ReadFloat();
        if (revision > 32)
        {
            oldEnabled = reader.ReadFloat();
        }
        else if (revision > 24)
        {
            enabled = reader.ReadBoolean();
        }
        return this;
    }

    public void Write(EndianWriter writer, ushort revision)
    {
        writer.WriteFloat(weight1);
        writer.WriteFloat(rate1);
        writer.WriteFloat(weight2);
        writer.WriteFloat(rate2);
        if (revision > 32)
        {
            writer.WriteFloat(oldEnabled);
        }
        else if (revision > 24)
        {
            writer.WriteBoolean(enabled);
        }
    }
}

public class Ham1NodeWeight
{
    bool unk0;
    float unk4, unk8, unkc, unk10;

    public Ham1NodeWeight Read(EndianReader reader)
    {
        unk4 = reader.ReadFloat();
        unk8 = reader.ReadFloat();
        unkc = reader.ReadFloat();
        unk10 = reader.ReadFloat();
        unk0 = reader.ReadBoolean();
        return this;
    }

    public bool IsDefault()
    {
        return unk0 == false && unk4 == -1.0f && unk8 == 1e+30f && unkc == -1.0f && unk10 == 1e+30f;
    }

    public override string ToString()
    {
        return $"Ham1NodeWeight: bool {unk0} perfectDist1 {unk4} rate1 {unk8} perfectDist2 {unkc} rate2 {unk10}\n";
    }

    public void Write(EndianWriter writer)
    {
        writer.WriteFloat(unk4);
        writer.WriteFloat(unk8);
        writer.WriteFloat(unkc);
        writer.WriteFloat(unk10);
        writer.WriteBoolean(unk0);
    }
}

public class Ham2FrameWeight
{
    float unk0;
    float[] unk4 = new float[4];
    float[] unk14 = new float[4];
    public float[] oldPrefix = new float[20];
    public float[] oldWeights = new float[8];

    public Ham2FrameWeight Read(EndianReader reader, ushort revision)
    {
        if (revision > 34)
        {
            unk0 = reader.ReadFloat();
        }
        int prefixCount = GetLegacyPrefixCount(revision);
        for (int i = 0; i < prefixCount; i++)
        {
            oldPrefix[i] = reader.ReadFloat();
        }
        if (revision < 37)
        {
            for (int i = 0; i < 8; i++)
            {
                oldWeights[i] = reader.ReadFloat();
            }
        }
        if (revision > 37)
        {
            for (int i = 0; i < 4; i++)
            {
                unk4[i] = reader.ReadFloat();
                unk14[i] = reader.ReadFloat();
            }
        }
        return this;
    }

    public override string ToString()
    {
        return $"Ham2FrameWeight: unk0 {unk0}; unk4 ( {unk4[0]},{unk4[1]},{unk4[2]},{unk4[3]} ); unk14 ( {unk14[0]},{unk14[1]},{unk14[2]},{unk14[3]} )\n";
    }

    public void Write(EndianWriter writer, ushort revision)
    {
        if (revision > 34)
        {
            writer.WriteFloat(unk0);
        }
        int prefixCount = GetLegacyPrefixCount(revision);
        for (int i = 0; i < prefixCount; i++)
        {
            writer.WriteFloat(oldPrefix[i]);
        }
        if (revision < 37)
        {
            for (int i = 0; i < 8; i++)
            {
                writer.WriteFloat(oldWeights[i]);
            }
        }
        if (revision > 37)
        {
            for (int i = 0; i < 4; i++)
            {
                writer.WriteFloat(unk4[i]);
                writer.WriteFloat(unk14[i]);
            }
        }
    }

    private static int GetLegacyPrefixCount(ushort revision)
    {
        if (revision == 32)
        {
            return 20;
        }
        if (revision == 31)
        {
            return 1;
        }
        return 0;
    }
}

public enum NodeCounts
{
    kNumHam1Nodes = 16,
    kMaxNumErrorNodes = 33
}

public class MoveFrame
{
    private ushort revision = 50;
    public float beat; // beat?
    public int unk4; // flags for ErrorNodeType (0x800000 = position, 0x400000 = displacement)
    public int numDC1Nodes;
    public int numHam2Nodes;
    // TODO: this 64 is actually indexed as a 2D array of some sort (16 by 4). 16 = num ham1 error nodes, but what's the 4 represent?
    Ham1NodeWeight[] ham1NodeWeights = new Ham1NodeWeight[64];
    Classes.Vector3[,] nodeWeights = new Classes.Vector3[33, 2];
    Classes.Vector3[,] nodeScales = new Classes.Vector3[33, 2];
    Classes.Vector3[,] nodesInverseScale = new Classes.Vector3[33, 2];
    Ham2FrameWeight[] frameWeights = new Ham2FrameWeight[2];
    public List<int> oldNodeIds = new();
    public List<KeyValuePair<Symbol, int>> oldNamedNodes = new();
    public List<List<OldNodeWeight>> oldNodeWeights = new();
    public List<Ham2FrameWeight> oldFrameWeights = new();

    public override string ToString()
    {
        if (revision < 40)
        {
            return $"MoveFrame: beat {beat:0.00}, revision {revision}, legacy node lists {oldNodeWeights.Count}, legacy frame weights {oldFrameWeights.Count}";
        }
        string str = $"MoveFrame:\n";
        str += $"\tBeat: {beat.ToString("0.00")}";
        str += $"\tFlags: 0x{unk4.ToString("X")}";
        str += $"\tDC1 error nodes: {numDC1Nodes}; DC2/3: {numHam2Nodes}\n";
        str += PrintHam1NodeWeights();
        str += PrintHam2NodeWeights();
        str += PrintHam2NodeScales();
        str += $"\tUnmirrored {frameWeights[0]}";
        str += $"\tMirrored {frameWeights[1]}";
        return str;
    }

    private string PrintHam1NodeWeights()
    {
        string str = "\tHam1NodeWeights:";
        bool allDefaults = true;
        for (int i = 0; i < 64; i++)
        {
            if (!ham1NodeWeights[i].IsDefault())
            {
                allDefaults = false;
                break;
            }
        }
        if (allDefaults)
        {
            str += $" ALL DEFAULTS\n";
        }
        else
        {
            str += " Non-defaults:\n";
            for (int i = 0; i < 64; i++)
            {
                if (!ham1NodeWeights[i].IsDefault())
                {
                    str += $"\t\t{i}: {ham1NodeWeights[i]}";
                }
            }
        }
        return str;
    }

    private string PrintHam2NodeWeights()
    {
        string str = "";
        str += $"\tUnmirrored nodeWeights: ";
        bool allzeroes = true;

        for (int i = 0; i < 33; i++)
        {
            if (!nodeWeights[i, 0].IsZero())
            {
                allzeroes = false;
                break;
            }
        }
        if (allzeroes)
        {
            str += "ALL ZEROES\n";
        }
        else
        {
            str += "Non-zeroes:\n";
            for (int i = 0; i < 33; i++)
            {
                if (!nodeWeights[i, 0].IsZero())
                {
                    str += $"\t\t{i}: {nodeWeights[i, 0]}\n";
                }
            }
        }

        str += $"\tMirrored nodeWeights: ";
        allzeroes = true;
        for (int i = 0; i < 33; i++)
        {
            if (!nodeWeights[i, 1].IsZero())
            {
                allzeroes = false;
                break;
            }
        }
        if (allzeroes)
        {
            str += "ALL ZEROES\n";
        }
        else
        {
            str += "Non-zeroes:\n";
            for (int i = 0; i < 33; i++)
            {
                if (!nodeWeights[i, 1].IsZero())
                {
                    str += $"\t\t{i}: {nodeWeights[i, 1]}\n";
                }
            }
        }
        return str;
    }
    private string PrintHam2NodeScales()
    {
        string str = "";
        str += $"\tUnmirrored nodeScales: ";
        bool allzeroes = true;

        for (int i = 0; i < 33; i++)
        {
            if (!nodeScales[i, 0].IsZero())
            {
                allzeroes = false;
                break;
            }
        }
        if (allzeroes)
        {
            str += "ALL ZEROES\n";
        }
        else
        {
            str += "Non-zeroes:\n";
            for (int i = 0; i < 33; i++)
            {
                if (!nodeScales[i, 0].IsZero())
                {
                    str += $"\t\t{i}: {nodeScales[i, 0]}\n";
                }
            }
        }

        str += $"\tMirrored nodeScales: ";
        allzeroes = true;
        for (int i = 0; i < 33; i++)
        {
            if (!nodeScales[i, 1].IsZero())
            {
                allzeroes = false;
                break;
            }
        }
        if (allzeroes)
        {
            str += "ALL ZEROES\n";
        }
        else
        {
            str += "Non-zeroes:\n";
            for (int i = 0; i < 33; i++)
            {
                if (!nodeScales[i, 1].IsZero())
                {
                    str += $"\t\t{i}: {nodeScales[i, 1]}\n";
                }
            }
        }
        return str;
    }

    public MoveFrame Read(EndianReader reader, ushort revision)
    {
        this.revision = revision;
        beat = reader.ReadFloat();
        if (revision < 14)
        {
            throw new NotSupportedException($"MoveFrame revision {revision} predates the supported DC3 loader.");
        }
        if (revision > 43)
        {
            unk4 = reader.ReadInt32();
        }
        if (revision < 40)
        {
            ReadLegacy(reader, revision);
            return this;
        }
        numDC1Nodes = reader.ReadInt32(); // expecting this to be 16
        if (numDC1Nodes != 16)
        {
            throw new InvalidDataException($"Invalid Ham1 node count {numDC1Nodes}; expected 16.");
        }

        for (int i = 0; i < 64; i++)
        {
            ham1NodeWeights[i] = new Ham1NodeWeight().Read(reader);
        }

        for (int i = 0; i < 2; i++)
        {
            frameWeights[i] = new Ham2FrameWeight().Read(reader, revision);
        }

        numHam2Nodes = reader.ReadInt32(); // expecting this to be 33
        if (numHam2Nodes < 0 || numHam2Nodes > (reader.BaseStream.Length - reader.BaseStream.Position) / 8)
        {
            throw new InvalidDataException($"Invalid Ham2 node count {numHam2Nodes}.");
        }
        nodeWeights = new Classes.Vector3[Math.Max(33, numHam2Nodes), 2];
        nodeScales = new Classes.Vector3[Math.Max(33, numHam2Nodes), 2];

        for (int i = 0; i < 2; i++)
        { // unmirrored vs mirrored
            for (int j = 0; j < numHam2Nodes; j++)
            {
                if (revision < 41)
                {
                    float weight = reader.ReadFloat();
                    nodeWeights[j, i] = new Classes.Vector3(weight, weight, weight);
                }
                else
                {
                    nodeWeights[j, i] = new Classes.Vector3().Read(reader);
                }
                if (revision >= 46)
                {
                    nodeScales[j, i] = new Classes.Vector3().Read(reader);
                }
            }
        }

        return this;
    }

    public void Write(EndianWriter writer, ushort revision)
    {
        writer.WriteFloat(beat);
        if (revision < 14)
        {
            throw new NotSupportedException($"MoveFrame revision {revision} predates the supported DC3 loader.");
        }
        if (revision > 43)
        {
            writer.WriteInt32(unk4);
        }
        if (revision < 40)
        {
            WriteLegacy(writer, revision);
            return;
        }
        writer.WriteInt32(numDC1Nodes);

        for (int i = 0; i < 64; i++)
        {
            ham1NodeWeights[i].Write(writer);
        }

        for (int i = 0; i < 2; i++)
        {
            frameWeights[i].Write(writer, revision);
        }

        writer.WriteInt32(numHam2Nodes);

        for (int i = 0; i < 2; i++) // unmirrored vs mirrored
        {
            for (int j = 0; j < numHam2Nodes; j++)
            {
                if (revision < 41)
                {
                    writer.WriteFloat(nodeWeights[j, i].x);
                }
                else
                {
                    nodeWeights[j, i].Write(writer);
                }
                if (revision >= 46)
                {
                    nodeScales[j, i].Write(writer);
                }
            }
        }
    }

    private void ReadLegacy(EndianReader reader, ushort revision)
    {
        if (revision < 19)
        {
            int count = 4;
            if (revision >= 17)
            {
                count = 5;
            }
            for (int i = 0; i < count; i++)
            {
                oldNodeIds.Add(reader.ReadInt32());
            }
        }
        else if (revision < 24)
        {
            oldNamedNodes = AssetHelpers.ReadList(reader, () => new KeyValuePair<Symbol, int>(Symbol.Read(reader), reader.ReadInt32()));
        }
        int nodeListCount = GetLegacyNodeListCount(revision);
        for (int i = 0; i < nodeListCount; i++)
        {
            oldNodeWeights.Add(AssetHelpers.ReadList(reader, () => new OldNodeWeight().Read(reader, revision)));
        }
        int frameWeightCount = GetLegacyFrameWeightCount(revision);
        for (int i = 0; i < frameWeightCount; i++)
        {
            oldFrameWeights.Add(new Ham2FrameWeight().Read(reader, revision));
        }
    }

    private void WriteLegacy(EndianWriter writer, ushort revision)
    {
        if (revision < 19)
        {
            foreach (int nodeId in oldNodeIds)
            {
                writer.WriteInt32(nodeId);
            }
        }
        else if (revision < 24)
        {
            AssetHelpers.WriteList(writer, oldNamedNodes, node =>
            {
                Symbol.Write(writer, node.Key);
                writer.WriteInt32(node.Value);
            });
        }
        int nodeListCount = GetLegacyNodeListCount(revision);
        for (int i = 0; i < nodeListCount; i++)
        {
            AssetHelpers.WriteList(writer, oldNodeWeights[i], value => value.Write(writer, revision));
        }
        int frameWeightCount = GetLegacyFrameWeightCount(revision);
        for (int i = 0; i < frameWeightCount; i++)
        {
            oldFrameWeights[i].Write(writer, revision);
        }
    }

    private static int GetLegacyNodeListCount(ushort revision)
    {
        if (revision >= 30)
        {
            return 8;
        }
        if (revision >= 28)
        {
            return 4;
        }
        if (revision >= 24)
        {
            return 2;
        }
        return 0;
    }

    private static int GetLegacyFrameWeightCount(ushort revision)
    {
        if (revision >= 30)
        {
            return 4;
        }
        if (revision >= 29)
        {
            return 2;
        }
        return 0;
    }
}
