using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharIKFoot"), Description("Remedial foot skate ik, not yet ready for prime time.")]
public class CharIKFoot : CharIKHand
{
    private ushort revision;
    private ushort altRevision;
    public Symbol oldSymbol = Symbol.Empty;
    public int oldValue1;
    public int oldValue2;
    public int oldValue3;
    [Name("Data"), Description("which bone to use to get the IK data from")]
    public Symbol data = Symbol.Empty;
    [Name("Data Index"), Description("Which element index to use (x=0, y=1, z=2)")]
    public int dataIndex;

    public new CharIKFoot Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 6);
        base.Read(reader, false, parent, entry);
        if (revision < 6)
        {
            oldSymbol = Symbol.Read(reader);
        }
        if (revision < 5)
        {
            if (revision > 1)
            {
                oldValue1 = reader.ReadInt32();
            }
            if (revision > 2)
            {
                oldValue2 = reader.ReadInt32();
            }
            if (revision > 3)
            {
                oldValue3 = reader.ReadInt32();
            }
        }
        else
        {
            data = Symbol.Read(reader);
            dataIndex = reader.ReadInt32();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 6);
        base.Write(writer, false, parent, entry);
        if (revision < 6)
        {
            Symbol.Write(writer, oldSymbol);
        }
        if (revision < 5)
        {
            if (revision > 1)
            {
                writer.WriteInt32(oldValue1);
            }
            if (revision > 2)
            {
                writer.WriteInt32(oldValue2);
            }
            if (revision > 3)
            {
                writer.WriteInt32(oldValue3);
            }
        }
        else
        {
            Symbol.Write(writer, data);
            writer.WriteInt32(dataIndex);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
