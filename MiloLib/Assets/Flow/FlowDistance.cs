using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Flow;

[Name("FlowDistance"), Description("Runs children when two trans objects are within a range")]
public class FlowDistance : FlowNode
{
    private ushort revision;
    private ushort altRevision;
    [Name("One"), Description("First object to compare")]
    public Symbol one = Symbol.Empty;
    [Name("Two"), Description("Second object to compare")]
    public Symbol two = Symbol.Empty;
    [Name("Persistent"), Description("Is the node persistent?")]
    public bool persistent;
    [Name("Distance"), Description("Distance for comparison")]
    public float distance;
    [Name("Run In Range"), Description("Run children when closer than distance value")]
    public bool runInRange;
    [Name("Drive Intensity"), Description("Applies current distance to flow intensity, closer being higher intensity")]
    public bool driveIntensity;

    public new FlowDistance Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 0);
        base.Read(reader, false, parent, entry);
        one = Symbol.Read(reader);
        two = Symbol.Read(reader);
        persistent = reader.ReadBoolean();
        distance = reader.ReadFloat();
        runInRange = reader.ReadBoolean();
        driveIntensity = reader.ReadBoolean();
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 0);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, one);
        Symbol.Write(writer, two);
        writer.WriteBoolean(persistent);
        writer.WriteFloat(distance);
        writer.WriteBoolean(runInRange);
        writer.WriteBoolean(driveIntensity);
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
