using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band.UI;

[Name("BandButton"), Description("Button with app-specific features")]
public class BandButton : UIButton
{
    private ushort altRevision;
    private ushort revision;

    public Symbol focusAnim = Symbol.Empty;
    public Symbol pulseAnim = Symbol.Empty;

    [MinVersion(13), MaxVersion(15)]
    public Symbol meshPtr = Symbol.Empty;

    [MinVersion(3), MaxVersion(13)]
    public int discardedInt1;
    [MinVersion(3), MaxVersion(13)]
    public int discardedInt2;
    [MinVersion(3), MaxVersion(13)]
    public int discardedInt3;
    [MinVersion(3), MaxVersion(13)]
    public int discardedInt4;
    [MaxVersion(10)]
    public bool discardedBool1;
    [MinVersion(1), MaxVersion(7)]
    public bool discardedBool2;
    [MaxVersion(11)]
    public int discardedIntC;
    [MinVersion(8), MaxVersion(8)]
    public HmxColor4 discardedColor = new();
    [MaxVersion(4)]
    public Symbol discardedSymbol = Symbol.Empty;

    // LoadOldBandTextComp fields for rev 0xE
    [MinVersion(14), MaxVersion(14)]
    private ushort oldBandTextCompRevision;
    [MinVersion(14), MaxVersion(14)]
    private ushort oldBandTextCompAltRevision;
    [MinVersion(14), MaxVersion(14)]
    public int oldBandTextCompUnkInt1;
    [MinVersion(14), MaxVersion(14)]
    public int oldBandTextCompUnkInt2;
    [MinVersion(14), MaxVersion(14)]
    public int oldBandTextCompUnkInt3;
    [MinVersion(14), MaxVersion(14)]
    public int oldBandTextCompUnkInt4;
    [MinVersion(14), MaxVersion(14)]
    public Symbol oldLabelType = Symbol.Empty;
    [MinVersion(14), MaxVersion(14)]
    private uint oldLabelColorNum;
    [MinVersion(14), MaxVersion(14)]
    public List<int> oldBandTextCompDummies = new();

    public ushort BandButtonRevision => revision;
    public ushort BandButtonAltRevision => altRevision;
    public ushort OldBandTextCompRevision => oldBandTextCompRevision;
    public ushort OldBandTextCompAltRevision => oldBandTextCompAltRevision;
    public uint OldLabelColorCount => oldLabelColorNum;

    public void SetBandButtonRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new BandButton Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 8)
        {
            if (revision <= 4)
            {
                trans = trans.Read(reader, false, parent, entry);
                draw = draw.Read(reader, false, parent, entry);
            }
            else
            {
                base.Read(reader, false, parent, entry);
            }
            if (revision > 2)
            {
                discardedInt1 = reader.ReadInt32();
                discardedInt2 = reader.ReadInt32();
                discardedInt3 = reader.ReadInt32();
                discardedInt4 = reader.ReadInt32();
            }
            if (revision <= 4)
            {
                discardedSymbol = Symbol.Read(reader);
            }

            if (revision < 8)
            {
                discardedBool1 = reader.ReadBoolean();
            }
            if (revision != 0)
            {
                discardedBool2 = reader.ReadBoolean();
            }

            width = reader.ReadFloat();
            height = reader.ReadFloat();

            if (revision <= 4)
            {
                textToken = Symbol.Read(reader);
            }
            if (revision > 5)
            {
                alignment = (UILabel.TextAlignments)reader.ReadInt32();
            }
        }
        else if (revision == 8)
        {
            base.Read(reader, false, parent, entry);

            fitType = (UILabel.LabelFitTypes)reader.ReadInt32();
            width = reader.ReadFloat();
            height = reader.ReadFloat();
            leading = reader.ReadFloat();
            alignment = (UILabel.TextAlignments)reader.ReadInt32();
            discardedInt1 = reader.ReadInt32();
            discardedInt2 = reader.ReadInt32();
            discardedInt3 = reader.ReadInt32();
            discardedInt4 = reader.ReadInt32();
            discardedBool1 = reader.ReadBoolean();
            discardedColor = new HmxColor4().Read(reader);
            kerning = reader.ReadFloat();
            textSize = reader.ReadFloat();
        }
        else
        {
            base.Read(reader, false, parent, entry);
            if (revision < 0xC)
            {
                fitType = (UILabel.LabelFitTypes)reader.ReadInt32();
                width = reader.ReadFloat();
                height = reader.ReadFloat();
            }

            if (revision < 0xB)
            {
                leading = reader.ReadFloat();
                alignment = (UILabel.TextAlignments)reader.ReadInt32();
            }

            if (revision < 0xE)
            {
                discardedInt1 = reader.ReadInt32();
                discardedInt2 = reader.ReadInt32();
                discardedInt3 = reader.ReadInt32();
                discardedInt4 = reader.ReadInt32();
            }

            if (revision < 0xB)
            {
                discardedBool1 = reader.ReadBoolean();
                kerning = reader.ReadFloat();
                textSize = reader.ReadFloat();
            }
        }

        if (revision < 0xC)
        {
            discardedIntC = reader.ReadInt32();
        }

        if (revision == 0xE)
        {
            LoadOldBandTextComp(reader);
        }

        if (revision >= 0xD && revision <= 0xF)
        {
            meshPtr = Symbol.Read(reader);
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
                oldBandTextCompUnkInt1 = reader.ReadInt32();
                oldBandTextCompUnkInt2 = reader.ReadInt32();
                oldBandTextCompUnkInt3 = reader.ReadInt32();
                oldBandTextCompUnkInt4 = reader.ReadInt32();
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
                    oldBandTextCompDummies.Add(reader.ReadInt32());
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
                writer.WriteInt32(oldBandTextCompUnkInt1);
                writer.WriteInt32(oldBandTextCompUnkInt2);
                writer.WriteInt32(oldBandTextCompUnkInt3);
                writer.WriteInt32(oldBandTextCompUnkInt4);
            }
            Symbol.Write(writer, oldLabelType);
            if (oldLabelType == "custom_colors")
            {
                if (oldBandTextCompRevision >= 2)
                {
                    writer.WriteUInt32(oldLabelColorNum);
                }
                for (int i = 0; i < oldBandTextCompDummies.Count; i++)
                {
                    writer.WriteInt32(oldBandTextCompDummies[i]);
                }

            }
        }
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision < 8)
        {
            if (revision <= 4)
            {
                trans.Write(writer, false, parent, null);
                draw.Write(writer, false, parent, null);
            }
            else
            {
                base.Write(writer, false, parent, entry);
            }
            if (revision > 2)
            {
                writer.WriteInt32(discardedInt1);
                writer.WriteInt32(discardedInt2);
                writer.WriteInt32(discardedInt3);
                writer.WriteInt32(discardedInt4);
            }
            if (revision <= 4)
            {
                Symbol.Write(writer, discardedSymbol);
            }
            if (revision < 8)
            {
                writer.WriteBoolean(discardedBool1);
            }
            if (revision != 0)
            {
                writer.WriteBoolean(discardedBool2);
            }

            writer.WriteFloat(width);
            writer.WriteFloat(height);

            if (revision <= 4)
            {
                Symbol.Write(writer, textToken);
            }
            if (revision > 5)
            {
                writer.WriteInt32((int)alignment);
            }
        }
        else if (revision == 8)
        {
            base.Write(writer, false, parent, entry);


            writer.WriteInt32((int)fitType);
            writer.WriteFloat(width);
            writer.WriteFloat(height);
            writer.WriteFloat(leading);
            writer.WriteInt32((int)alignment);
            writer.WriteInt32(discardedInt1);
            writer.WriteInt32(discardedInt2);
            writer.WriteInt32(discardedInt3);
            writer.WriteInt32(discardedInt4);
            writer.WriteBoolean(discardedBool1);
            discardedColor.Write(writer);
            writer.WriteFloat(kerning);
            writer.WriteFloat(textSize);
        }
        else
        {
            base.Write(writer, false, parent, entry);
            if (revision < 0xC)
            {
                writer.WriteInt32((int)fitType);
                writer.WriteFloat(width);
                writer.WriteFloat(height);
            }

            if (revision < 0xB)
            {
                writer.WriteFloat(leading);
                writer.WriteInt32((int)alignment);
            }

            if (revision < 0xE)
            {
                writer.WriteInt32(discardedInt1);
                writer.WriteInt32(discardedInt2);
                writer.WriteInt32(discardedInt3);
                writer.WriteInt32(discardedInt4);
            }

            if (revision < 0xB)
            {
                writer.WriteBoolean(discardedBool1);
                writer.WriteFloat(kerning);
                writer.WriteFloat(textSize);
            }

        }

        if (revision < 0xC)
        {
            writer.WriteInt32(discardedIntC);
        }

        if (revision == 0xE)
        {
            WriteOldBandTextComp(writer);
        }

        if (revision >= 0xD && revision <= 0xF)
        {
            Symbol.Write(writer, meshPtr);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
