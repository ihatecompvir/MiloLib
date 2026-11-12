using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Band;

public class BandPatchMesh
{


    public class MeshPair
    {
        public class PatchPair
        {
            public Symbol patch = Symbol.Empty;
            public Symbol tex = Symbol.Empty;

            public PatchPair Read(EndianReader reader)
            {
                patch = Symbol.Read(reader);
                tex = Symbol.Read(reader);
                return this;
            }

            public void Write(EndianWriter writer)
            {
                Symbol.Write(writer, patch);
                Symbol.Write(writer, tex);
            }
        }

        public Symbol mesh = Symbol.Empty;
        public List<PatchPair> patchPairs = new();

        public class MeshVert
        {

        }

        public MeshPair Read(EndianReader reader)
        {
            mesh = Symbol.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            Symbol.Write(writer, mesh);
        }


    }

    private ushort altRevision;
    private ushort revision;

    public List<MeshPair> meshPairs = new();

    [MinVersion(3)]
    public bool renderTo;

    public Symbol source = Symbol.Empty;

    [MinVersion(4)]
    public int category;

    [MaxVersion(0)]
    public Symbol unkSymbol1 = Symbol.Empty;
    [MaxVersion(3)]
    public Symbol unkSymbol2 = Symbol.Empty;

    [MinVersion(2), MaxVersion(2)]
    public Symbol unkSymbol3 = Symbol.Empty;

    public BandPatchMesh Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        source = Symbol.Read(reader);

        if (revision > 3)
        {
            meshPairs = AssetHelpers.ReadList(reader, () => new MeshPair().Read(reader));
        }
        else
        {
            meshPairs.Add(new MeshPair().Read(reader));
        }
        if (revision < 1)
        {
            unkSymbol1 = Symbol.Read(reader);
        }

        if (revision < 4)
        {
            unkSymbol2 = Symbol.Read(reader);
        }

        if (revision > 1)
        {
            if (revision > 2)
            {
                renderTo = reader.ReadBoolean();
            }
            else
            {
                unkSymbol3 = Symbol.Read(reader);
            }
        }

        if (revision > 3)
        {
            category = reader.ReadInt32();
        }

        return this;
    }

    public void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        Symbol.Write(writer, source);

        if (revision > 3)
        {
            AssetHelpers.WriteList(writer, meshPairs, (MeshPair meshPair) => meshPair.Write(writer));
        }
        else
        {
            meshPairs[0].Write(writer);
        }

        if (revision < 1)
        {
            Symbol.Write(writer, unkSymbol1);
        }

        if (revision < 4)
        {
            Symbol.Write(writer, unkSymbol2);
        }

        if (revision > 1)
        {
            if (revision > 2)
            {
                writer.WriteBoolean(renderTo);
            }
            else
            {
                Symbol.Write(writer, unkSymbol3);
            }
        }

        if (revision > 3)
        {
            writer.WriteInt32(category);
        }
    }

}
