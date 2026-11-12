using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("PlotDef")]
public class PlotDef : Object
{
    public class ObjectSet
    {
        [Name("Meshes")]
        public List<Symbol> meshes = new();
        public List<Symbol> secondaryMeshes = new();
        public List<Symbol> unkReferences = new();
        public List<Symbol> worldInstances = new();
        [Name("Anims")]
        public List<Symbol> anims = new();

        public ObjectSet Read(EndianReader reader)
        {
            meshes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            secondaryMeshes = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            unkReferences = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            worldInstances = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            anims = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            AssetHelpers.WriteList(writer, meshes, value => Symbol.Write(writer, value));
            AssetHelpers.WriteList(writer, secondaryMeshes, value => Symbol.Write(writer, value));
            AssetHelpers.WriteList(writer, unkReferences, value => Symbol.Write(writer, value));
            AssetHelpers.WriteList(writer, worldInstances, value => Symbol.Write(writer, value));
            AssetHelpers.WriteList(writer, anims, value => Symbol.Write(writer, value));
        }
    }

    private ushort revision = 15;
    private ushort altRevision;
    [Name("Intensity")]
    public int intensity;
    [Name("Only Once")]
    public bool onlyOnce;
    [Name("End")]
    public bool end;
    [Name("Use Hack Position")]
    public bool useHackPosition;
    [Name("Placement Settings")]
    public Symbol placementSettings = Symbol.Empty;
    [Name("Length")]
    public int length;
    [Name("Has Bridge")]
    public bool hasBridge;
    [Name("Section Boundary Restriction")]
    public Symbol sectionBoundaryRestriction = Symbol.Empty;
    [Name("Is Expensive")]
    public bool isExpensive;
    [Name("Environment")]
    public Symbol environment = Symbol.Empty;
    public ObjectSet objectSet0 = new();
    public ObjectSet objectSet1 = new();
    public ObjectSet objectSet2 = new();
    public ObjectSet objectSet3 = new();
    [Name("Char Anims")]
    public List<Symbol> charAnims = new();

    public new PlotDef Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 15);
        if (revision != 15)
        {
            throw new NotSupportedException($"Unsupported PlotDef revision {revision}.");
        }

        base.Read(reader, false, parent, entry);
        intensity = reader.ReadInt32();
        onlyOnce = reader.ReadBoolean();
        end = reader.ReadBoolean();
        useHackPosition = reader.ReadBoolean();
        placementSettings = Symbol.Read(reader);
        length = reader.ReadInt32();
        hasBridge = reader.ReadBoolean();
        sectionBoundaryRestriction = Symbol.Read(reader);
        isExpensive = reader.ReadBoolean();
        environment = Symbol.Read(reader);
        objectSet0.Read(reader);
        objectSet1.Read(reader);
        objectSet2.Read(reader);
        objectSet3.Read(reader);
        charAnims = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (revision != 15)
        {
            throw new NotSupportedException($"Unsupported PlotDef revision {revision}.");
        }

        AssetHelpers.WriteRevision(writer, revision, altRevision, 15);
        base.Write(writer, false, parent, entry);
        writer.WriteInt32(intensity);
        writer.WriteBoolean(onlyOnce);
        writer.WriteBoolean(end);
        writer.WriteBoolean(useHackPosition);
        Symbol.Write(writer, placementSettings);
        writer.WriteInt32(length);
        writer.WriteBoolean(hasBridge);
        Symbol.Write(writer, sectionBoundaryRestriction);
        writer.WriteBoolean(isExpensive);
        Symbol.Write(writer, environment);
        objectSet0.Write(writer);
        objectSet1.Write(writer);
        objectSet2.Write(writer);
        objectSet3.Write(writer);
        AssetHelpers.WriteList(writer, charAnims, value => Symbol.Write(writer, value));
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
