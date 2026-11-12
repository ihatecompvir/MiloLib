using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

// i fucking hate this
[Name("WorldInstance"), Description("Shared instance of a RndDir")]
public class WorldInstance : RndDir
{
    [Name("Persistent Objects")]
    public class PersistentObjects
    {
        public Symbol unkString = Symbol.Empty;
        public Symbol unkCamReference = Symbol.Empty;
        public ObjectFields objFields = new();

        public RndAnimatable anim = new();
        public RndDrawable draw = new();
        public RndTrans trans = new();

        [Name("Environ")]
        public Symbol environ = Symbol.Empty;
        [Name("Test Event"), Description("Test event")]
        public Symbol testEvent = Symbol.Empty;

        private uint stringTableCount;
        private uint stringTableSize;

        public List<DirectoryMeta.Entry> perObjs = new();

        public bool hasPostLoadFields = false;

        private static bool IsEndBytes(EndianReader reader)
        {
            uint expected = reader.Endianness == Endian.BigEndian ? 0xADDEADDE : 0xDEADDEAD;
            long pos = reader.BaseStream.Position;
            uint val = reader.ReadUInt32();
            reader.BaseStream.Position = pos;
            return val == expected;
        }

        public PersistentObjects Read(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry, uint revision)
        {
            if (entry.isProxy)
            {
                if (!IsEndBytes(reader))
                {
                    hasPostLoadFields = true;

                    unkString = Symbol.Read(reader);
                    unkCamReference = Symbol.Read(reader);
                    objFields.root.Read(reader);
                    objFields.note = Symbol.Read(reader);

                    anim = anim.Read(reader, parent, entry);
                    draw = draw.Read(reader, false, parent, entry);
                    trans = trans.Read(reader, false, parent, entry);

                    environ = Symbol.Read(reader);
                    testEvent = Symbol.Read(reader);

                    if (revision > 1)
                    {
                        if (revision > 2)
                        {
                            stringTableCount = reader.ReadUInt32();
                            stringTableSize = reader.ReadUInt32();
                        }

                        perObjs = AssetHelpers.ReadList(reader, () => new DirectoryMeta.Entry(Symbol.Read(reader).value, Symbol.Read(reader).value, null));

                        for (int i = 0; i < perObjs.Count; i++)
                        {
                            switch (perObjs[i].type.value)
                            {
                                case "Mesh":
                                    perObjs[i].obj = new RndMesh().Read(reader, false, parent, perObjs[i]);
                                    break;
                                default:
                                    throw new Exception("Unknown persistent object type " + perObjs[i].type.value + " in WorldInstance");
                            }
                        }
                    }
                }
            }
            if ((reader.Endianness == Endian.BigEndian ? 0xADDEADDE : 0xDEADDEAD) != reader.ReadUInt32())
            {
                throw new Exception("Got to end of persistent objects but didn't find the expected end bytes, read likely did not succeed");
            }

            return this;
        }

        public void Write(EndianWriter writer, DirectoryMeta parent, DirectoryMeta.Entry? entry, uint revision)
        {
            if (hasPostLoadFields)
            {
                Symbol.Write(writer, unkString);
                Symbol.Write(writer, unkCamReference);
                objFields.root.Write(writer);
                Symbol.Write(writer, objFields.note);

                anim.Write(writer);
                draw.Write(writer, false, parent, null);
                trans.Write(writer, false, parent, null);

                Symbol.Write(writer, environ);
                Symbol.Write(writer, testEvent);

                if (revision > 1)
                {
                    if (revision > 2)
                    {
                        writer.WriteUInt32(stringTableCount);
                        writer.WriteUInt32(stringTableSize);
                    }

                    AssetHelpers.WriteList(writer, perObjs, perObj => { Symbol.Write(writer, perObj.type); Symbol.Write(writer, perObj.name); });

                    for (int i = 0; i < perObjs.Count; i++)
                    {
                        switch (perObjs[i].type.value)
                        {
                            case "Mesh":
                                ((RndMesh)perObjs[i].obj).Write(writer, false, parent, perObjs[i]);
                                break;
                            default:
                                throw new Exception("Unknown persistent object type " + perObjs[i].type.value + " in WorldInstance");
                        }
                    }
                }
            }

            writer.WriteEndBytes();
        }
    }

    private ushort altRevision;
    private ushort revision;

    public ushort WorldInstanceRevision => revision;
    public ushort WorldInstanceAltRevision => altRevision;

    public void SetWorldInstanceRevision(ushort revision, ushort altRevision = 0)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    [Name("Instance File"), Description("Which file we instance, only set in instances"), MinVersion(1)]
    public Symbol filePath = Symbol.Empty;

    [MaxVersion(0)]
    public Symbol dir = Symbol.Empty;

    public PersistentObjects persistentObjects = new();
    public DirectoryMeta? inlineProxyDirectory;


    public WorldInstance(ushort revision, ushort altRevision = 0) : base(revision, altRevision)
    {
        this.revision = revision;
        this.altRevision = altRevision;
    }

    public new WorldInstance Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {

        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision != 0)
        {
            filePath = Symbol.Read(reader);
        }
        else
        {
            dir = Symbol.Read(reader);
        }

        base.Read(reader, false, parent, entry);

        if (standalone && !entry.isProxy)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision != 0)
        {
            Symbol.Write(writer, filePath);
        }
        else
        {
            Symbol.Write(writer, dir);
        }

        base.Write(writer, false, parent, entry);

        if (standalone && !entry.isProxy)
        {
            writer.WriteEndBytes();
        }
    }

    public override bool IsDirectory()
    {
        return true;
    }

}
