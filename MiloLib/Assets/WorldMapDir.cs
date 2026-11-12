using MiloLib.Assets.UI;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("WorldMapDir"), Description("A panel dir for the world tour map.")]
public class WorldMapDir : PanelDir
{
    private ushort altRevision;
    private ushort revision;

    public float unkFloat1;
    public Symbol postProcess = Symbol.Empty;
    public float unkFloat2;
    public float unkFloat3;
    public float unkFloat4;
    public float unkFloat5;
    [Name("Do Culling"), Description("Should hidden towns be culled?"), MinVersion(2)]
    public bool unkBool;
    [Name("Town"), Description("Select a town to rotate the world to it."), MinVersion(2)]
    public Symbol currentTown = Symbol.Empty;

    public WorldMapDir(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new WorldMapDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, ushort.MaxValue);
        base.Read(reader, false, parent, entry);
        unkFloat1 = reader.ReadFloat();
        postProcess = Symbol.Read(reader);
        unkFloat2 = reader.ReadFloat();
        unkFloat3 = reader.ReadFloat();
        unkFloat4 = reader.ReadFloat();
        unkFloat5 = reader.ReadFloat();

        if (revision > 1)
        {
            unkBool = reader.ReadBoolean();
            currentTown = Symbol.Read(reader);
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, ushort.MaxValue);
        base.Write(writer, false, parent, entry);
        writer.WriteFloat(unkFloat1);
        Symbol.Write(writer, postProcess);
        writer.WriteFloat(unkFloat2);
        writer.WriteFloat(unkFloat3);
        writer.WriteFloat(unkFloat4);
        writer.WriteFloat(unkFloat5);

        if (revision > 1)
        {
            writer.WriteBoolean(unkBool);
            Symbol.Write(writer, currentTown);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }
}
