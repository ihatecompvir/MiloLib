using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.UI;

[Name("BandLabel"), Description("Label with color presets")]
public class BandLabel : UILabel
{
    private ushort altRevision;
    private ushort revision;

    public float unk1dcKey1Value;
    public float unk1dcKey1Frame;
    public float unk1dcKey2Value;
    public float unk1dcKey2Frame;
    public Symbol unk1e4 = Symbol.Empty;
    public string unk1e8 = "";
    public bool unk1f4;

    [Name("In Anim"), Description("anim used before label change"), MinVersion(17)]
    public Symbol inAnim = Symbol.Empty;
    [Name("Out Anim"), Description("anim used after label change"), MinVersion(17)]
    public Symbol outAnim = Symbol.Empty;

    [MinVersion(15), MaxVersion(15)]
    public Symbol oldLabelType = Symbol.Empty;

    [MinVersion(15), MaxVersion(15)]
    public List<int> dummies = new();

    [MinVersion(15), MaxVersion(15)]
    private uint oldLabelColorNum;

    [MinVersion(15), MaxVersion(15)]
    private ushort oldBandTextCompRevision;
    [MinVersion(15), MaxVersion(15)]
    private ushort oldBandTextCompAltRevision;

    [MinVersion(15), MaxVersion(15)]
    public int unkInt1;
    [MinVersion(15), MaxVersion(15)]
    public int unkInt2;
    [MinVersion(15), MaxVersion(15)]
    public int unkInt3;
    [MinVersion(15), MaxVersion(15)]
    public int unkInt4;

    [MaxVersion(6)]
    public Symbol unkSymbolRev6 = Symbol.Empty;

    [MinVersion(1), MaxVersion(12)]
    public bool unkBool87;
    [MinVersion(6), MaxVersion(9)]
    public bool unkBool88;

    [MaxVersion(14)]
    public int unkIntI;
    [MaxVersion(14)]
    public int unkIntJ;
    [MaxVersion(14)]
    public int unkIntK;
    [MaxVersion(14)]
    public int unkIntL;

    [MinVersion(10), MaxVersion(10)]
    public int unkInt50;
    [MinVersion(11), MaxVersion(13)]
    public int unkInt6C;

    [MaxVersion(9)]
    public int unkIntRevLt10;
    [MinVersion(11), MaxVersion(13)]
    public int unkIntRevLtE;

    [MinVersion(6), MaxVersion(13)]
    public float unkFloatWidth;
    [MinVersion(6), MaxVersion(13)]
    public float unkFloatHeight;
    [MinVersion(9), MaxVersion(14)]
    public float unkFloatColR;
    [MinVersion(9), MaxVersion(14)]
    public float unkFloatColG;
    [MinVersion(9), MaxVersion(14)]
    public float unkFloatColB;
    [MinVersion(9), MaxVersion(14)]
    public float unkFloatColA;
    [MinVersion(10), MaxVersion(12)]
    public float unkFloatKerning;
    [MinVersion(10), MaxVersion(12)]
    public float unkFloatTextSize;
    [MinVersion(5), MaxVersion(12)]
    public float unkFloatLeading;

    [MinVersion(3), MaxVersion(10)]
    public int oldUnknownInt;
    [MinVersion(3), MaxVersion(10)]
    public float jitterX;
    [MinVersion(3), MaxVersion(10)]
    public float jitterY;
    [MinVersion(3), MaxVersion(10)]
    public float jitterZ;

    public ushort BandLabelRevision => revision;
    public ushort BandLabelAltRevision => altRevision;
    public ushort OldBandTextCompRevision => oldBandTextCompRevision;
    public ushort OldBandTextCompAltRevision => oldBandTextCompAltRevision;
    public uint OldLabelColorCount => oldLabelColorNum;

    public void SetBandLabelRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandLabel Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 0xB)
        {
            if (revision <= 6)
            {
                trans = trans.Read(reader, false, parent, entry);
                draw = draw.Read(reader, false, parent, entry);
            }
            else
            {
                base.Read(reader, false, parent, entry);
            }

            if (revision > 5)
            {
                if (revision < 10)
                {
                    unkBool88 = reader.ReadBoolean();
                    unkFloatWidth = reader.ReadFloat();
                    unkFloatHeight = reader.ReadFloat();
                }
                else
                {
                    unkInt50 = reader.ReadInt32();
                    unkFloatWidth = reader.ReadFloat();
                    unkFloatHeight = reader.ReadFloat();
                }
            }

            if (revision > 4)
            {
                unkFloatLeading = reader.ReadFloat();
            }

            if (revision > 3)
            {
                alignment = (TextAlignments)reader.ReadInt32();
            }

            if (revision > 2)
            {
                oldUnknownInt = reader.ReadInt32();
                jitterX = reader.ReadFloat();
                jitterY = reader.ReadFloat();
                jitterZ = reader.ReadFloat();
            }

            if (revision < 2)
            {
                unkIntI = reader.ReadInt32();
                unkIntJ = reader.ReadInt32();
                unkIntK = reader.ReadInt32();
                unkIntL = reader.ReadInt32();
            }

            if (revision <= 6)
            {
                unkSymbolRev6 = Symbol.Read(reader);
            }

            if (revision != 0)
            {
                unkBool87 = reader.ReadBoolean();
            }
            else
            {
                unkBool87 = false;
            }

            if (revision <= 6)
            {
                textToken = Symbol.Read(reader);
            }

            if (revision < 10)
            {
                unkIntRevLt10 = reader.ReadInt32();
            }

            if (revision > 8)
            {
                unkFloatColR = reader.ReadFloat();
                unkFloatColG = reader.ReadFloat();
                unkFloatColB = reader.ReadFloat();
                unkFloatColA = reader.ReadFloat();
            }

            if (revision > 9)
            {
                unkFloatKerning = reader.ReadFloat();
                unkFloatTextSize = reader.ReadFloat();
            }
        }
        else
        {
            base.Read(reader, false, parent, entry);

            if (revision < 0xE)
            {
                unkInt6C = reader.ReadInt32();
                unkFloatWidth = reader.ReadFloat();
                unkFloatHeight = reader.ReadFloat();
            }

            if (revision < 0xD)
            {
                unkFloatLeading = reader.ReadFloat();
                alignment = (TextAlignments)reader.ReadInt32();
            }

            if (revision < 0xF)
            {
                unkIntI = reader.ReadInt32();
                unkIntJ = reader.ReadInt32();
                unkIntK = reader.ReadInt32();
                unkIntL = reader.ReadInt32();
            }

            if (revision < 0xD)
            {
                unkBool87 = reader.ReadBoolean();
                unkFloatKerning = reader.ReadFloat();
                unkFloatTextSize = reader.ReadFloat();
            }

            if (revision < 0xE)
            {
                unkIntRevLtE = reader.ReadInt32();
            }

            if (revision < 0xF)
            {
                unkFloatColR = reader.ReadFloat();
                unkFloatColG = reader.ReadFloat();
                unkFloatColB = reader.ReadFloat();
                unkFloatColA = reader.ReadFloat();
            }
        }

        if (revision == 0xF)
        {
            LoadOldBandTextComp(reader);
        }

        if (revision >= 0x11)
        {
            inAnim = Symbol.Read(reader);
            outAnim = Symbol.Read(reader);
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }
    public void LoadOldBandTextComp(EndianReader reader)
    {
        AssetHelpers.ReadRevision(reader, out oldBandTextCompRevision, out oldBandTextCompAltRevision);

        if (oldBandTextCompRevision > 2)
        {
            return;
        }
        else
        {
            if (oldBandTextCompRevision < 1)
            {
                unkInt1 = reader.ReadInt32();
                unkInt2 = reader.ReadInt32();
                unkInt3 = reader.ReadInt32();
                unkInt4 = reader.ReadInt32();
            }
            oldLabelType = Symbol.Read(reader);
            if (oldLabelType == "custom_colors")
            {
                oldLabelColorNum = 4;
                if (oldBandTextCompRevision >= 2)
                {
                    oldLabelColorNum = reader.ReadUInt32();
                }

                for (int i = 0; i < oldLabelColorNum; i++)
                {
                    dummies.Add(reader.ReadInt32());
                }
            }
        }
    }

    public void WriteOldBandTextComp(EndianWriter writer)
    {
        AssetHelpers.WriteRevision(writer, oldBandTextCompRevision, oldBandTextCompAltRevision);
        if (oldBandTextCompRevision > 2)
        {
            return;
        }
        else
        {
            if (oldBandTextCompRevision < 1)
            {
                writer.WriteInt32(unkInt1);
                writer.WriteInt32(unkInt2);
                writer.WriteInt32(unkInt3);
                writer.WriteInt32(unkInt4);
            }
            Symbol.Write(writer, oldLabelType);
            if (oldLabelType == "custom_colors")
            {
                if (oldBandTextCompRevision >= 2)
                {
                    writer.WriteUInt32(oldLabelColorNum);
                }

                for (int i = 0; i < dummies.Count; i++)
                {
                    writer.WriteInt32(dummies[i]);
                }
            }
        }
    }
    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision < 0xB)
        {
            if (revision <= 6)
            {
                // Write Trans and Drawable members
                trans.Write(writer, false, parent, null);
                draw.Write(writer, false, parent, null);
            }
            else
            {
                base.Write(writer, false, parent, entry);
            }

            if (revision > 5)
            {
                if (revision < 10)
                {
                    writer.WriteBoolean(unkBool88);
                    writer.WriteFloat(unkFloatWidth);
                    writer.WriteFloat(unkFloatHeight);
                }
                else
                {
                    writer.WriteInt32(unkInt50);
                    writer.WriteFloat(unkFloatWidth);
                    writer.WriteFloat(unkFloatHeight);
                }
            }

            if (revision > 4)
            {
                writer.WriteFloat(unkFloatLeading);
            }

            if (revision > 3)
            {
                writer.WriteInt32((int)alignment);
            }

            if (revision > 2)
            {
                writer.WriteInt32(oldUnknownInt);
                writer.WriteFloat(jitterX);
                writer.WriteFloat(jitterY);
                writer.WriteFloat(jitterZ);
            }

            if (revision < 2)
            {
                writer.WriteInt32(unkIntI);
                writer.WriteInt32(unkIntJ);
                writer.WriteInt32(unkIntK);
                writer.WriteInt32(unkIntL);
            }

            if (revision <= 6)
            {
                Symbol.Write(writer, unkSymbolRev6);
            }

            if (revision != 0)
            {
                writer.WriteBoolean(unkBool87);
            }

            if (revision <= 6)
            {
                Symbol.Write(writer, textToken);
            }

            if (revision < 10)
            {
                writer.WriteInt32(unkIntRevLt10);
            }

            if (revision > 8)
            {
                writer.WriteFloat(unkFloatColR);
                writer.WriteFloat(unkFloatColG);
                writer.WriteFloat(unkFloatColB);
                writer.WriteFloat(unkFloatColA);
            }

            if (revision > 9)
            {
                writer.WriteFloat(unkFloatKerning);
                writer.WriteFloat(unkFloatTextSize);
            }
        }
        else
        {
            base.Write(writer, false, parent, entry);

            if (revision < 0xE)
            {
                writer.WriteInt32(unkInt6C);
                writer.WriteFloat(unkFloatWidth);
                writer.WriteFloat(unkFloatHeight);
            }

            if (revision < 0xD)
            {
                writer.WriteFloat(unkFloatLeading);
                writer.WriteInt32((int)alignment);
            }

            if (revision < 0xF)
            {
                writer.WriteInt32(unkIntI);
                writer.WriteInt32(unkIntJ);
                writer.WriteInt32(unkIntK);
                writer.WriteInt32(unkIntL);
            }

            if (revision < 0xD)
            {
                writer.WriteBoolean(unkBool87);
                writer.WriteFloat(unkFloatKerning);
                writer.WriteFloat(unkFloatTextSize);
            }

            if (revision < 0xE)
            {
                writer.WriteInt32(unkIntRevLtE);
            }

            if (revision < 0xF)
            {
                writer.WriteFloat(unkFloatColR);
                writer.WriteFloat(unkFloatColG);
                writer.WriteFloat(unkFloatColB);
                writer.WriteFloat(unkFloatColA);
            }
        }

        if (revision == 0xF)
        {
            WriteOldBandTextComp(writer);
        }

        if (revision >= 0x11)
        {
            Symbol.Write(writer, inAnim);
            Symbol.Write(writer, outAnim);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
