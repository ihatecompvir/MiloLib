using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("TransConstraint"), Description("Object for setting up lazy transform parenting")]
public class TransConstraint : Object
{
    private ushort revision;
    private ushort altRevision;
    public Object poll = new();
    public Object highlight = new();
    [Name("Parent"), Description("The trans parent")]
    public Symbol parentTrans = Symbol.Empty;
    [Name("Child"), Description("The trans child")]
    public Symbol childTrans = Symbol.Empty;
    [Name("Static Cube"), Description("Dimensions of cube in which child transform is not updated")]
    public Vector3 staticCube = new();
    public bool[] tracks = new bool[3];
    [Name("Speed"), Description("Speed of tracking in inches/second")]
    public float speed;
    public bool oldFlag;
    [Name("Affect Scale"), Description("Whether or not to interpolate scale along with position")]
    public bool affectScale;
    [Name("Use UI Time"), Description("Whether or not to use UI timing. Default is to use game time.")]
    public bool useUiTime;

    public new TransConstraint Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 4);
        base.Read(reader, false, parent, entry);
        poll.Read(reader, false, parent, entry);
        highlight.Read(reader, false, parent, entry);
        parentTrans = Symbol.Read(reader);
        childTrans = Symbol.Read(reader);
        staticCube.Read(reader);
        for (int axis = 0; axis < 3; axis++)
        {
            tracks[axis] = reader.ReadBoolean();
        }
        if (revision > 0)
        {
            speed = reader.ReadFloat();
        }
        if (revision > 1)
        {
            if (revision <= 3)
            {
                oldFlag = reader.ReadBoolean();
            }
            affectScale = reader.ReadBoolean();
        }
        if (revision > 2)
        {
            useUiTime = reader.ReadBoolean();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 4);
        base.Write(writer, false, parent, entry);
        poll.Write(writer, false, parent, entry);
        highlight.Write(writer, false, parent, entry);
        Symbol.Write(writer, parentTrans);
        Symbol.Write(writer, childTrans);
        staticCube.Write(writer);
        for (int axis = 0; axis < 3; axis++)
        {
            writer.WriteBoolean(tracks[axis]);
        }
        if (revision > 0)
        {
            writer.WriteFloat(speed);
        }
        if (revision > 1)
        {
            if (revision <= 3)
            {
                writer.WriteBoolean(oldFlag);
            }
            writer.WriteBoolean(affectScale);
        }
        if (revision > 2)
        {
            writer.WriteBoolean(useUiTime);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
