using MiloLib.Assets.World;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets;

[Name("ObjectDir"), Description("An ObjectDir keeps track of a set of Objects. It can subdir or proxy in other ObjectDirs. To rename subdir or proxy files search for remap_objectdirs in system/run/config/objects.dta")]
public class ObjectDir : Object
{
    private Dictionary<Game.MiloGame, uint> gameRevisions = new()
    {
        // no dirs before this
        { Game.MiloGame.GuitarHero2_PS2, 16 },
        { Game.MiloGame.GuitarHero2_360, 17 },
        { Game.MiloGame.Phase, 17 },
        { Game.MiloGame.RockBand, 19 },
        { Game.MiloGame.RockBand2, 20 },
        { Game.MiloGame.LegoRockBand, 20 },
        { Game.MiloGame.TheBeatlesRockBand, 22 },
        { Game.MiloGame.GreenDayRockBand, 22 },
        { Game.MiloGame.RockBand3, 27 },
        { Game.MiloGame.DanceCentral, 27 },
        { Game.MiloGame.DanceCentral2, 28 },
        { Game.MiloGame.RockBandBlitz, 28 },
        { Game.MiloGame.DanceCentral3, 28 }
    };

    public enum ReferenceType : byte
    {
        kInlineNever = 0,
        kInlineCached = 1,
        kInlineAlways = 2,
        kInlineCachedShared = 3
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Viewport Count"), Description("The number of viewports."), MinVersion(2)]
    private uint viewportCount;

    [Name("Viewports"), MinVersion(2)]
    public List<Matrix> viewports = new();

    [Name("Current Viewport Index"), Description("The index of the current viewport."), MinVersion(2)]
    public uint currentViewportIdx;

    [Name("Inline Proxy"), Description("Can this proxy be inlined?"), MinVersion(20)]
    public bool inlineProxy;

    [MinVersion(28)]
    public ReferenceType inlineProxyType;

    [Name("Proxy Path"), Description("The path to the proxy."), MinVersion(13)]
    public Symbol proxyPath = Symbol.Empty;

    public virtual bool AllowsInlineProxy()
    {
        return true;
    }

    public bool HasInlineProxyDirectory
    {
        get
        {
            if (!AllowsInlineProxy())
            {
                return false;
            }

            if (revision >= 28 && inlineProxy && (inlineProxyType == ReferenceType.kInlineAlways || inlineProxyType == ReferenceType.kInlineCached))
            {
                return true;
            }

            if (proxyPath.value == "")
            {
                return false;
            }

            if (revision < 15)
            {
                return false;
            }

            if (revision == 15)
            {
                return oldInlineProxy;
            }

            if (revision < 19)
            {
                return oldInlineProxyMode != 0;
            }

            if (revision == 19)
            {
                return true;
            }

            return inlineProxy;
        }
    }

    [Name("Sub Directory Count"), Description("The number of subdirectories in the directory."), MinVersion(3)]
    private uint subDirCount;

    [Name("Sub Directories"), Description("Subdirectories of objects"), MinVersion(3)]
    public List<Symbol> subDirs = new();

    [Name("Inline Sub Directory"), Description("How is this inlined as a subdir? Note that when you change this, you must resave everything subdiring this file for it to take effect"), MinVersion(21)]
    public ReferenceType inlineSubDir;

    [Name("Inline Sub Directory Count"), Description("The number of inlined subdirectories in the directory."), MinVersion(21)]
    private uint inlineSubDirCount;

    [Name("Inline Sub Directory Names"), MinVersion(21)]
    public List<Symbol> inlineSubDirNames = new();

    [Name("Reference Types"), MinVersion(26)]
    public List<ReferenceType> referenceTypes = new();

    [Name("Reference Types Alt"), MinVersion(26)]
    public List<ReferenceType> referenceTypesAlt = new();

    [Name("Inline Sub Directories"), MinVersion(21)]
    public List<DirectoryMeta> inlineSubDirs = new();

    public List<bool> inlineCachedBooleans = new();

    [Name("Unknown String 1")]
    public Symbol unkString = Symbol.Empty;

    [Name("Unknown Camera Reference")]
    public Symbol unkCamReference = Symbol.Empty;

    [Name("Unknown Object Reference 1"), MinVersion(2), MaxVersion(10)]
    public Symbol unkObjRef1 = Symbol.Empty;

    [Name("Current Camera"), MinVersion(4), MaxVersion(10)]
    public Symbol currentCamera = Symbol.Empty;

    [Name("Unknown String 3"), MinVersion(5), MaxVersion(5)]
    public Symbol unkString3 = Symbol.Empty;

    [HideInInspector, MinVersion(15), MaxVersion(15)]
    public bool oldInlineProxy;

    [HideInInspector, MinVersion(16), MaxVersion(18)]
    public int oldInlineProxyMode;

    [HideInInspector]
    public bool hasDeferredPostLoadFields;

    [MinVersion(2), MaxVersion(17)]
    public List<uint> unkInts = new();

    [MinVersion(26)]
    public uint unk1;
    [MinVersion(26)]
    public uint unk2;
    [MinVersion(26)]
    public byte[] alwaysInlineHash = Array.Empty<byte>();

    // this shouldn't be under ObjectDir
    // TODO: Put this under WorldInstance where it belongs
    public bool hasPersistentObjects = false;

    public bool RemoveInlineSubDir(DirectoryMeta inlineDir)
    {
        int metadataIndex = GetInlineSubDirMetadataIndex(inlineDir);
        if (metadataIndex < 0)
        {
            return inlineSubDirs.Remove(inlineDir);
        }

        RemoveInlineSubDirAtMetadataIndex(metadataIndex);
        return true;
    }

    private int GetInlineSubDirMetadataIndex(DirectoryMeta inlineDir)
    {
        int inlineIndex = 0;

        for (int metadataIndex = 0; metadataIndex < inlineSubDirNames.Count; metadataIndex++)
        {
            if (IsSharedCachedInlineSubDir(metadataIndex))
            {
                continue;
            }

            if (inlineIndex < inlineSubDirs.Count && ReferenceEquals(inlineSubDirs[inlineIndex], inlineDir))
            {
                return metadataIndex;
            }

            inlineIndex++;
        }

        int directIndex = inlineSubDirs.IndexOf(inlineDir);
        return directIndex >= 0 && directIndex < inlineSubDirNames.Count ? directIndex : -1;
    }

    private void RemoveInlineSubDirAtMetadataIndex(int metadataIndex)
    {
        int embeddedIndex = 0;
        int cachedBoolIndex = 0;
        for (int i = 0; i < metadataIndex; i++)
        {
            if (!IsSharedCachedInlineSubDir(i))
            {
                embeddedIndex++;
            }
            if (IsCachedInlineBoolean(i))
            {
                cachedBoolIndex++;
            }
        }
        bool isSharedCached = IsSharedCachedInlineSubDir(metadataIndex);
        bool hasCachedBoolean = IsCachedInlineBoolean(metadataIndex);

        inlineSubDirNames.RemoveAt(metadataIndex);

        if (referenceTypes.Count > metadataIndex)
        {
            referenceTypes.RemoveAt(metadataIndex);
        }

        if (referenceTypesAlt.Count > metadataIndex)
        {
            referenceTypesAlt.RemoveAt(metadataIndex);
        }

        if (!isSharedCached && embeddedIndex < inlineSubDirs.Count)
        {
            inlineSubDirs.RemoveAt(embeddedIndex);
        }

        if (hasCachedBoolean && cachedBoolIndex < inlineCachedBooleans.Count)
        {
            inlineCachedBooleans.RemoveAt(cachedBoolIndex);
        }
    }

    private bool IsSharedCachedInlineSubDir(int metadataIndex)
    {
        return referenceTypes.Count > metadataIndex &&
            referenceTypesAlt.Count > metadataIndex &&
            referenceTypes[metadataIndex] == ReferenceType.kInlineCachedShared &&
            referenceTypesAlt[metadataIndex] == ReferenceType.kInlineCached;
    }

    private bool IsCachedInlineBoolean(int metadataIndex)
    {
        return referenceTypes.Count > metadataIndex &&
            referenceTypesAlt.Count > metadataIndex &&
            referenceTypes[metadataIndex] == ReferenceType.kInlineCached &&
            referenceTypesAlt[metadataIndex] == ReferenceType.kInlineCached;
    }

    public ObjectDir(ushort revision, ushort altRevision = 0)
    {
        this.revision = (ushort)revision;
        this.altRevision = (ushort)altRevision;
    }

    public new ObjectDir Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        if (revision < 22)
        {
            if (revision >= 2 && revision < 17)
            {
                objFields.Read(reader, parent, entry);
            }
        }
        else
        {
            AssetHelpers.ReadRevision(reader, out ushort objectRevision, out ushort objectAltRevision);
            objFields.SetObjectFieldsRevision(objectRevision, objectAltRevision);
            objFields.type = Symbol.Read(reader);
        }

        if (revision > 1)
        {

            if (revision >= 26)
            {
                if (revision == 26)
                {
                    unk1 = reader.ReadByte();
                }
                else
                {
                    unk1 = reader.ReadUInt32();
                }
                unk2 = reader.ReadUInt32();
                if (unk2 > reader.BaseStream.Length - reader.BaseStream.Position)
                {
                    throw new InvalidDataException("The ObjectDir inline hash exceeds the remaining stream.");
                }
                alwaysInlineHash = reader.ReadBlock(checked((int)unk2));
            }

            viewportCount = reader.ReadUInt32();

            // sanity check, there should be no more than 7 viewports
            if (viewportCount > 7)
            {
                throw new InvalidDataException("Viewport count is too high at " + reader.BaseStream.Position + ", ObjectDir is invalid");
            }

            for (int i = 0; i < viewportCount; i++)
            {
                Matrix viewport = new();
                viewport.Read(reader);
                viewports.Add(viewport);
                if (revision <= 17)
                {
                    unkInts.Add(reader.ReadUInt32());
                }
            }

            currentViewportIdx = reader.ReadUInt32();

            // sanity check, the current viewport index should be less than the viewport count but it can also be 0
            if (currentViewportIdx >= viewportCount + 1 && currentViewportIdx != 0)
            {
                throw new InvalidDataException("Current viewport index is invalid at " + reader.BaseStream.Position + ", ObjectDir is invalid");
            }
        }

        if (revision > 12)
        {
            if (revision > 19)
            {
                if (revision < 28)
                {
                    inlineProxy = reader.ReadBoolean();
                }
                else
                {
                    inlineProxyType = (ReferenceType)reader.ReadByte();
                    inlineProxy = inlineProxyType != ReferenceType.kInlineNever;
                }
            }
            proxyPath = Symbol.Read(reader);
        }

        if (revision >= 2 && revision < 11)
        {
            unkObjRef1 = Symbol.Read(reader);
        }

        if (revision >= 4 && revision < 11)
        {
            currentCamera = Symbol.Read(reader);
        }

        if (revision == 5)
        {
            unkString3 = Symbol.Read(reader);
        }

        if (revision > 2)
        {
            subDirCount = reader.ReadUInt32();

            // sanity check, there should be no more than 100 subdirs
            // TODO: double check if the game has an assert for this and use that if present
            if (subDirCount > 100)
            {
                throw new InvalidDataException("Subdir count is too high at " + reader.BaseStream.Position + ", ObjectDir is invalid");
            }

            for (int i = 0; i < subDirCount; i++)
            {
                Symbol subDir = Symbol.Read(reader);
                subDirs.Add(subDir);
            }

            if (revision >= 21)
            {
                inlineSubDir = (ReferenceType)reader.ReadByte();
                inlineSubDirCount = reader.ReadUInt32();

                // sanity check, there should be no more than 100 inlined subdirs
                if (inlineSubDirCount > 100)
                {
                    throw new InvalidDataException("Inlined subdir count is too high at " + reader.BaseStream.Position + ", ObjectDir is invalid");
                }

                if (inlineSubDirCount > 0)
                {
                    for (int i = 0; i < inlineSubDirCount; i++)
                    {
                        Symbol inlineSubDirName = Symbol.Read(reader);
                        inlineSubDirNames.Add(inlineSubDirName);
                    }

                    if (revision >= 26)
                    {
                        for (int i = 0; i < inlineSubDirCount; i++)
                        {
                            ReferenceType referenceType = (ReferenceType)reader.ReadByte();
                            referenceTypes.Add(referenceType);
                        }

                        for (int i = 0; i < inlineSubDirCount; i++)
                        {
                            ReferenceType referenceTypeAlt = (ReferenceType)reader.ReadByte();
                            referenceTypesAlt.Add(referenceTypeAlt);
                        }
                    }

                    for (int i = 0; i < inlineSubDirCount; i++)
                    {
                        DirectoryMeta inlinedSubDir = new();
                        inlinedSubDir.platform = parent.platform;
                        if (referenceTypes.Count > i && referenceTypesAlt.Count > i)
                        {
                            if (referenceTypes[i] == ReferenceType.kInlineCached && referenceTypesAlt[i] == ReferenceType.kInlineCached)
                            {
                                inlineCachedBooleans.Add(reader.ReadBoolean());
                            }
                        }
                        if (referenceTypes.Count > i && referenceTypesAlt.Count > i &&
                            referenceTypes[i] == ReferenceType.kInlineCachedShared && referenceTypesAlt[i] == ReferenceType.kInlineCached)
                        {
                        }
                        else
                        {
                            inlinedSubDir.Read(reader);
                            inlineSubDirs.Add(inlinedSubDir);
                        }
                    }

                }
            }
        }

        if (revision < 19)
        {
            if (revision < 16)
            {
                if (revision > 14)
                {
                    oldInlineProxy = reader.ReadBoolean();
                }
            }
            else
            {
                oldInlineProxyMode = reader.ReadInt32();
            }
        }

        if (entry.isProxy && revision >= 15 && revision < 19 && HasInlineProxyDirectory)
        {
            hasDeferredPostLoadFields = true;
            return this;
        }


        // hack...sigh
        if (entry.type.value == "WorldInstance" &&
        this is WorldInstance worldInstanceRead && worldInstanceRead.WorldInstanceRevision != 0 &&
        !(this.referenceTypes.Count > 0 &&
            this.referenceTypesAlt.Count > 0 &&
            this.referenceTypes[0] == ReferenceType.kInlineCached &&
            this.referenceTypesAlt[0] == ReferenceType.kInlineCached))
        {
            hasPersistentObjects = reader.ReadBoolean();
            if (entry.isProxy)
            {
                return this;
            }
        }

        ReadObjectDirPostLoadFields(reader, parent, entry);

        if (standalone)
        {
            ReadAndVerifyEndBytes(reader, parent, entry);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        if (revision < 22)
        {
            if (revision >= 2 && revision < 17)
            {
                objFields.Write(writer, parent);
            }
        }
        else
        {
            AssetHelpers.WriteRevision(writer, objFields.ObjectFieldsRevision, objFields.ObjectFieldsAltRevision);
            Symbol.Write(writer, objFields.type);
        }

        if (revision > 1)
        {
            if (revision >= 26)
            {
                if (revision == 26)
                {
                    writer.WriteByte(checked((byte)unk1));
                }
                else
                {
                    writer.WriteUInt32(unk1);
                }
                unk2 = (uint)alwaysInlineHash.Length;
                writer.WriteUInt32(unk2);
                writer.WriteBlock(alwaysInlineHash);
            }

            writer.WriteUInt32((uint)viewports.Count);
            foreach (var viewport in viewports)
            {
                viewport.Write(writer);
                if (revision <= 17)
                {
                    writer.WriteUInt32(unkInts[viewports.IndexOf(viewport)]);
                }
            }

            writer.WriteUInt32(currentViewportIdx);
        }

        if (revision > 12)
        {
            if (revision > 19)
            {
                if (revision >= 28 && inlineProxy && inlineProxyType != ReferenceType.kInlineNever)
                {
                    writer.WriteByte((byte)inlineProxyType);
                }
                else
                {
                    writer.WriteBoolean(inlineProxy);
                }
            }

            Symbol.Write(writer, proxyPath);
        }

        if (revision >= 2 && revision < 11)
        {
            Symbol.Write(writer, unkObjRef1);
        }

        if (revision >= 4 && revision < 11)
        {
            Symbol.Write(writer, currentCamera);
        }

        if (revision == 5)
        {
            Symbol.Write(writer, unkString3);
        }

        if (revision > 2)
        {
            AssetHelpers.WriteList(writer, subDirs, subDir => Symbol.Write(writer, subDir));

            if (revision >= 21)
            {
                writer.WriteByte((byte)inlineSubDir);
                AssetHelpers.WriteList(writer, inlineSubDirNames, inlineSubDirName => Symbol.Write(writer, inlineSubDirName));

                if (revision >= 26)
                {
                    foreach (var referenceType in referenceTypes)
                    {
                        writer.WriteByte((byte)referenceType);
                    }

                    foreach (var referenceTypeAlt in referenceTypesAlt)
                    {
                        writer.WriteByte((byte)referenceTypeAlt);
                    }
                }


                int inlineIdx = 0;
                int cachedBoolIdx = 0;
                for (int i = 0; i < inlineSubDirNames.Count; i++)
                {
                    if (referenceTypes.Count > i && referenceTypesAlt.Count > i)
                    {
                        if (referenceTypes[i] == ReferenceType.kInlineCached && referenceTypesAlt[i] == ReferenceType.kInlineCached)
                        {
                            writer.WriteBoolean(cachedBoolIdx < inlineCachedBooleans.Count ? inlineCachedBooleans[cachedBoolIdx++] : false);
                        }
                    }
                    if (referenceTypes.Count > i && referenceTypesAlt.Count > i &&
                        referenceTypes[i] == ReferenceType.kInlineCachedShared && referenceTypesAlt[i] == ReferenceType.kInlineCached)
                    {
                    }
                    else
                    {
                        if (inlineIdx < inlineSubDirs.Count)
                        {
                            inlineSubDirs[inlineIdx++].Write(writer);
                        }
                    }
                }
            }
        }

        if (revision < 19)
        {
            if (revision < 16)
            {
                if (revision > 14)
                {
                    writer.WriteBoolean(oldInlineProxy);
                }
            }
            else
            {
                writer.WriteInt32(oldInlineProxyMode);
            }
        }

        if (entry != null && entry.isProxy && revision >= 15 && revision < 19 && HasInlineProxyDirectory)
        {
            hasDeferredPostLoadFields = true;
            return;
        }

        if (entry != null && entry.type.value == "WorldInstance" &&
            this is WorldInstance wiWrite && wiWrite.WorldInstanceRevision != 0 &&
            !(this.referenceTypes.Count > 0 &&
                this.referenceTypesAlt.Count > 0 &&
                this.referenceTypes[0] == ReferenceType.kInlineCached &&
                this.referenceTypesAlt[0] == ReferenceType.kInlineCached))
        {
            writer.WriteBoolean(hasPersistentObjects);
            if (entry.isProxy)
            {
                return;
            }
        }

        WriteObjectDirPostLoadFields(writer, parent);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    protected void ReadObjectDirPostLoadFields(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        unkString = Symbol.Read(reader);
        unkCamReference = Symbol.Read(reader);

        if (revision < 22)
        {
            if (revision > 16)
            {
                objFields.Read(reader, parent, entry);
            }
        }
        else
        {
            objFields.root.Read(reader);
            objFields.note = Symbol.Read(reader);
        }
    }

    protected void WriteObjectDirPostLoadFields(EndianWriter writer, DirectoryMeta parent)
    {
        Symbol.Write(writer, unkString);
        Symbol.Write(writer, unkCamReference);

        if (revision < 22)
        {
            if (revision > 16)
            {
                objFields.Write(writer, parent);
            }
        }
        else
        {
            objFields.root.Write(writer);
            Symbol.Write(writer, objFields.note);
        }
    }

    protected virtual void ReadDeferredPostLoadFields(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        ReadObjectDirPostLoadFields(reader, parent, entry);
    }

    protected virtual void WriteDeferredPostLoadFields(EndianWriter writer, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        WriteObjectDirPostLoadFields(writer, parent);
    }

    public void ReadDeferredPostLoad(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        ReadDeferredPostLoadFields(reader, parent, entry);

        if (standalone)
        {
            ReadAndVerifyEndBytes(reader, parent, entry);
        }
    }

    public void WriteDeferredPostLoad(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        WriteDeferredPostLoadFields(writer, parent, entry);

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    protected void ReadAndVerifyEndBytes(EndianReader reader, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadEnd(reader, parent, entry, revision);
    }

    public override bool IsDirectory()
    {
        return true;
    }
}
