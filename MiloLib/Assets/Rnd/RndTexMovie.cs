using System.Runtime.InteropServices;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("TexMovie"), Description("Renders bink movie to a texture.")]
public class RndTexMovie : Object
{
    public class Movie
    {
        [Name("Bink Movie File"), Description("Bink File")]
        public Symbol name = Symbol.Empty;
        public uint unk;
        public uint unk2;

        [Name("Draw Pre Clear"), Description("Renders the texture before the rest of the scene is rendered. Useful for rendering large textures.")]
        public bool unkBool;

        private uint byteCount;
        public List<byte> bytes = new();

        public bool fillWidth;
        [Name("Is Localized"), Description("Is this movie localized?")]
        public bool localized;
        public bool oldValue7;

        public Movie Read(EndianReader reader, uint revision, bool legoLayout = false)
        {
            name = Symbol.Read(reader);
            if (revision > 5)
            {
                localized = reader.ReadBoolean();
            }
            if (revision == 7)
            {
                oldValue7 = reader.ReadBoolean();
            }
            if (revision == 2 || (revision == 3 && legoLayout))
            {
                unkBool = reader.ReadBoolean();
                return this;
            }
            if (revision == 3)
            {
                return this;
            }
            unk = reader.ReadUInt32();
            unk2 = reader.ReadUInt32();

            byteCount = reader.ReadUInt32();
            bytes = new List<byte>(reader.ReadBlock((int)byteCount));

            return this;
        }

        public void Write(EndianWriter writer, uint revision, bool legoLayout = false)
        {
            Symbol.Write(writer, name);

            if (revision > 5)
            {
                writer.WriteBoolean(localized);
            }
            if (revision == 7)
            {
                writer.WriteBoolean(oldValue7);
            }

            if (revision == 2 || (revision == 3 && legoLayout))
            {
                writer.WriteBoolean(unkBool);
                return;
            }
            if (revision == 3)
            {
                return;
            }

            writer.WriteUInt32(unk);
            writer.WriteUInt32(unk2);

            writer.WriteUInt32((uint)bytes.Count);
            long dataStart = writer.Position;
            writer.WriteBlock(CollectionsMarshal.AsSpan(bytes));

            if (unk == 0xFFFFFFFF && (int)unk2 > 0)
            {
                writer.MarkChunks(dataStart, writer.Position, AssetHelpers.FileLoaderPieceSize);
            }
        }
    }

    private ushort altRevision;
    private ushort revision;

    private bool UsesLegoLayout(DirectoryMeta parent)
    {
        return parent.revision == 25 && revision == 3;
    }

    public RndDrawable draw = new();
    public Object obj = new();

    [Name("Output Texture"), Description("Texture to write to")]
    public Symbol outputTexture = Symbol.Empty;

    [Name("Loop"), Description("Loop this movie?")]
    public bool loop;

    public Movie movie = new();

    [Name("Triggered"), Description("Don't start the movie automatically, as triggered by an EventTrigger instead"), MinVersion(3), MaxVersion(3)]
    public bool triggered;

    public bool fillWidth;

    [Name("Preload"), Description("Load movie when object is loaded?"), MaxVersion(3)]
    public byte unk;

    public new RndTexMovie Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        objFields = objFields.Read(reader, parent, entry);

        draw = draw.Read(reader, false, parent, entry);
        obj = obj.Read(reader, false, parent, entry);

        if (altRevision != 0)
        {
            fillWidth = reader.ReadBoolean();
        }

        outputTexture = Symbol.Read(reader);
        loop = reader.ReadBoolean();

        if (revision < 4)
        {
            unk = reader.ReadByte();
        }

        movie = movie.Read(reader, revision, UsesLegoLayout(parent));

        if (UsesLegoLayout(parent))
        {
            triggered = reader.ReadBoolean();
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);
        objFields.Write(writer, parent);

        draw.Write(writer, false, parent, null);
        obj.Write(writer, false, parent, entry);

        if (altRevision != 0)
        {
            writer.WriteBoolean(fillWidth);
        }

        Symbol.Write(writer, outputTexture);

        writer.WriteBoolean(loop);

        if (revision < 4)
        {
            writer.WriteByte(unk);
        }

        movie.Write(writer, revision, UsesLegoLayout(parent));

        if (UsesLegoLayout(parent))
        {
            writer.WriteBoolean(triggered);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
