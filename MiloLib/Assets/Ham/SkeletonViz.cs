using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("SkeletonViz"), Description("Visualization of one natural input skeleton")]
public class SkeletonViz : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    public RndTrans trans = new();
    [Name("Use Physical Cam"), Description("Draw skeleton from Natal camera perspective?")]
    public bool usePhysicalCam;
    [Name("Axes Coord Sys"), Description("Which coordinate system axes to draw")]
    public int axesCoordSys;
    public int oldValue1;
    public int oldValue2;
    [Name("Physical Cam Rotation"), Description("Degrees to rotation physical camera around skeleton")]
    public float physicalCamRotation;
    public Symbol oldCharacter = Symbol.Empty;

    public new SkeletonViz Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 6);
        if (revision >= 6)
        {
            base.Read(reader, false, parent, entry);
        }
        draw.Read(reader, false, parent, entry);
        trans.Read(reader, false, parent, entry);
        if (revision >= 1)
        {
            usePhysicalCam = reader.ReadBoolean();
        }
        if (revision >= 2)
        {
            axesCoordSys = reader.ReadInt32();
        }
        if (revision == 3)
        {
            oldValue1 = reader.ReadInt32();
            oldValue2 = reader.ReadInt32();
        }
        if (altRevision > 0)
        {
            physicalCamRotation = reader.ReadFloat();
        }
        if (revision >= 5 && altRevision == 0)
        {
            oldCharacter = Symbol.Read(reader);
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
        if (revision >= 6)
        {
            base.Write(writer, false, parent, entry);
        }
        draw.Write(writer, false, parent, entry);
        trans.Write(writer, false, parent, entry);
        if (revision >= 1)
        {
            writer.WriteBoolean(usePhysicalCam);
        }
        if (revision >= 2)
        {
            writer.WriteInt32(axesCoordSys);
        }
        if (revision == 3)
        {
            writer.WriteInt32(oldValue1);
            writer.WriteInt32(oldValue2);
        }
        if (altRevision > 0)
        {
            writer.WriteFloat(physicalCamRotation);
        }
        if (revision >= 5 && altRevision == 0)
        {
            Symbol.Write(writer, oldCharacter);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
