using MiloLib.Classes;
using MiloLib.Utils;
using static MiloLib.Assets.ObjectFields;

namespace MiloLib.Assets.Ham;

[Name("MoveGraph")]
public class MoveGraph : Object
{

    private ushort revision;
    private ushort altRevision;

    // Keyed by the name text, not by Symbol. Symbol does not override Equals, so a Symbol key would only match the exact same instance.
    public Dictionary<string, MoveParent> moveParents = new();
    public Dictionary<string, MoveVariant> moveVariants = new();

    public DTBArrayParent moveArray = new();

    public override string ToString()
    {
        string str = "MoveGraph:\n";
        str += $"Move parents ({moveParents.Count}):\n\n";
        int count = 1;
        foreach (var parent in moveParents)
        {
            str += $"MoveParent {count} of {moveParents.Count}: (\n";
            str += $"Move symbol: {parent.Key}\n";
            str += parent.Value;
            str += ")\n\n";
            count += 1;
        }

        str += $"Move variants ({moveVariants.Count}):\n\n";
        count = 1;
        foreach (var variant in moveVariants)
        {
            str += $"MoveVariant {count} of {moveVariants.Count}: (\n";
            str += $"Move symbol: {variant.Key}\n";
            str += variant.Value;
            str += ")\n\n";
            count += 1;
        }

        str += $"Move array:\n";
        for (int i = 0; i < moveArray.children.Count; i++)
        {
            str += moveArray.children[i].ToString() + "\n";
        }

        return str;
    }

    public new MoveGraph Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        int numParents = reader.ReadInt32();
        for (int i = 0; i < numParents; i++)
        {
            MoveParent moveParent = new();
            moveParent.Read(reader, this);
            moveParents[moveParent.name.value] = moveParent;
        }

        moveArray.Read(reader);

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        base.Write(writer, false, parent, entry);

        writer.WriteInt32(moveParents.Count);

        foreach (var moveParent in moveParents.Values)
        {
            moveParent.Write(writer);
        }

        if (moveArray.children == null)
        {
            moveArray.children = new List<DTBNode>();
        }
        moveArray.Write(writer);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
