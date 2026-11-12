using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.World;

[Name("WorldCrowd"), Description("A quickly-rendered bunch of instanced characters within an area")]
public class WorldCrowd : RndDrawable
{
    // TODO: this needs to be moved into RndMultiMesh when we have that, according to the decomp
    public class OldMultiMeshInstance
    {
        public Matrix oldXfm = new();
        public HmxColor4 oldColor = new();

        public OldMultiMeshInstance Read(EndianReader reader)
        {
            oldXfm = oldXfm.Read(reader);
            oldColor = oldColor.Read(reader);
            return this;
        }

        public void Write(EndianWriter writer)
        {
            oldXfm.Write(writer);
            oldColor.Write(writer);
        }
    }

    public class MultiMeshInstanceRevB
    {
        public Matrix xfm = new();
        public List<HmxColor4> colors = new();

        public MultiMeshInstanceRevB Read(EndianReader reader)
        {
            xfm = xfm.Read(reader);
            colors = AssetHelpers.ReadList(reader, () => new HmxColor4().Read(reader));
            return this;
        }

        public void Write(EndianWriter writer)
        {
            xfm.Write(writer);
            AssetHelpers.WriteList(writer, colors, color => color.Write(writer));
        }
    }

    public class CharDef
    {

        [Name("Character"), Description("The character to use as the archetype")]
        public Symbol character = Symbol.Empty;
        [Name("Height"), Description("The height at which to render the character")]
        public float height;
        [Name("Density"), Description("Density to place this character")]
        public float density;
        [Name("Radius"), Description("Collision radius of the character - characters won't be placed within this range")]
        public float radius;
        public bool useRandomColor;

        public CharDef Read(EndianReader reader, uint revision)
        {
            character = Symbol.Read(reader);
            height = reader.ReadFloat();
            density = reader.ReadFloat();
            if (revision > 1)
            {
                radius = reader.ReadFloat();
            }

            if (revision > 8)
            {
                useRandomColor = reader.ReadBoolean();
            }

            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            Symbol.Write(writer, character);
            writer.WriteFloat(height);
            writer.WriteFloat(density);
            if (revision > 1)
            {
                writer.WriteFloat(radius);
            }

            if (revision > 8)
            {
                writer.WriteBoolean(useRandomColor);
            }
        }

        public override string ToString()
        {
            return $"{character.value} - Height: {height}, Density: {density}, Radius: {radius}, RandomColor: {useRandomColor}";
        }
    }

    public class CharData
    {

        public CharDef def = new();

        public CharData Read(EndianReader reader, uint revision)
        {
            def.Read(reader, revision);
            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            def.Write(writer, revision);
        }

        public override string ToString()
        {
            return def.ToString();
        }
    }

    private ushort altRevision;
    private ushort revision;

    [Name("Placement Mesh"), Description("The placement mesh")]
    public Symbol placementMesh = Symbol.Empty;

    [MaxVersion(2)]
    public uint unkInt1;

    [Name("Num"), Description("Number of characters to place")]
    public int num;

    [MaxVersion(7)]
    public bool unkBool1;

    [MinVersion(5)]
    public uint modifyStamp;
    [Name("Force 3D Crowd"), Description("Makes crowd be 3D regardless of the CamShot"), MinVersion(13)]
    public bool force3DCrowd;

    [Name("Show 3D Only"), Description("Shows only the 3D crowd, but ONLY in Milo so you can more easily distinguish them from the 2d crowd"), MinVersion(6)]
    public bool show3DOnly;


    [Name("Environ"), Description("The environ to render the imposter billboards with"), MinVersion(7)]
    public Symbol environ = Symbol.Empty;
    [Name("Environ3D"), Description("The environ used when rendering the 3D crowd set by a cam shot"), MinVersion(10)]
    public Symbol environ3D = Symbol.Empty;
    [Name("Focus"), Description("Optional crowd facing focus when rotate is set to kCrowdRotateNone"), MinVersion(12)]
    public Symbol focus = Symbol.Empty;

    [Name("Character Force LOD"), MinVersion(15)]
    public int charForceLod = -1;
    [MinVersion(16)]
    public int oldValue16;

    private uint charCount;
    [Name("Characters"), Description("Character archetypes for the crowd")]
    public List<CharData> characters = new();

    [MinVersion(2), MaxVersion(8)]
    public List<List<Matrix>> oldTransformOnlyInstances = new();

    [MinVersion(9), MaxVersion(10)]
    public List<List<OldMultiMeshInstance>> oldMultiMeshInstances = new();

    [MinVersion(11), MaxVersion(13)]
    public List<List<MultiMeshInstanceRevB>> multiMeshInstancesRevB = new();

    [MinVersion(14)]
    public List<List<Matrix>> transforms = new();

    [MinVersion(1)]
    public Object highlightable = new();


    public new WorldCrowd Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        placementMesh = Symbol.Read(reader);

        if (revision < 3)
        {
            unkInt1 = reader.ReadUInt32();
        }

        num = reader.ReadInt32();

        if (revision < 8)
        {
            unkBool1 = reader.ReadBoolean();
        }

        charCount = reader.ReadUInt32();
        if (charCount > 0)
        {
            for (int i = 0; i < charCount; i++)
            {
                characters.Add(new CharData().Read(reader, revision));
            }
        }

        if (revision > 6)
        {
            environ = Symbol.Read(reader);
        }

        if (revision > 9)
        {
            environ3D = Symbol.Read(reader);
        }

        if (revision > 1)
        {
            if (revision < 9)
            {
                for (int i = 0; i < charCount; i++)
                {
                    oldTransformOnlyInstances.Add(AssetHelpers.ReadList(reader, () => new Matrix().Read(reader)));
                }
            }
            else if (revision < 0xB)
            {
                for (int i = 0; i < charCount; i++)
                {
                    oldMultiMeshInstances.Add(AssetHelpers.ReadList(reader, () => new OldMultiMeshInstance().Read(reader)));
                }
            }
            else if (revision < 0xE)
            {
                for (int i = 0; i < charCount; i++)
                {
                    multiMeshInstancesRevB.Add(AssetHelpers.ReadList(reader, () => new MultiMeshInstanceRevB().Read(reader)));
                }
            }
            else
            {
                for (int i = 0; i < charCount; i++)
                {
                    transforms.Add(AssetHelpers.ReadList(reader, () => new Matrix().Read(reader)));
                }
            }
        }

        if (revision > 4)
        {
            modifyStamp = reader.ReadUInt32();
        }

        if (revision > 0xC)
        {
            force3DCrowd = reader.ReadBoolean();
        }

        if (revision > 5)
        {
            show3DOnly = reader.ReadBoolean();
        }

        if (revision > 0xB)
        {
            focus = Symbol.Read(reader);
        }

        if (revision > 14)
        {
            charForceLod = reader.ReadInt32();
        }
        if (revision > 15)
        {
            oldValue16 = reader.ReadInt32();
        }

        if (revision != 0)
        {
            highlightable = highlightable.Read(reader, false, parent, entry);
        }

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public new void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision);

        ((RndDrawable)this).Write(writer, false, parent, null);

        Symbol.Write(writer, placementMesh);

        if (revision < 3)
        {
            writer.WriteUInt32(unkInt1);
        }

        writer.WriteInt32(num);

        if (revision < 8)
        {
            writer.WriteBoolean(unkBool1);
        }

        charCount = (uint)characters.Count;
        writer.WriteUInt32(charCount);
        foreach (var charData in characters)
        {
            charData.Write(writer, revision);
        }

        if (revision > 6)
        {
            Symbol.Write(writer, environ);
        }

        if (revision > 9)
        {
            Symbol.Write(writer, environ3D);
        }

        if (revision > 1)
        {
            if (revision < 9)
            {
                for (int i = 0; i < charCount; i++)
                {
                    if (i < oldTransformOnlyInstances.Count)
                    {
                        AssetHelpers.WriteList(writer, oldTransformOnlyInstances[i], xfm => xfm.Write(writer));
                    }
                    else
                    {
                        writer.WriteUInt32(0);
                    }
                }
            }
            else if (revision < 0xB)
            {
                for (int i = 0; i < charCount; i++)
                {
                    if (i < oldMultiMeshInstances.Count)
                    {
                        AssetHelpers.WriteList(writer, oldMultiMeshInstances[i], inst => inst.Write(writer));
                    }
                    else
                    {
                        writer.WriteUInt32(0);
                    }
                }
            }
            else if (revision < 0xE)
            {
                for (int i = 0; i < charCount; i++)
                {
                    if (i < multiMeshInstancesRevB.Count)
                    {
                        AssetHelpers.WriteList(writer, multiMeshInstancesRevB[i], inst => inst.Write(writer));
                    }
                    else
                    {
                        writer.WriteUInt32(0);
                    }
                }
            }
            else
            {
                for (int i = 0; i < charCount; i++)
                {
                    if (i < transforms.Count)
                    {
                        AssetHelpers.WriteList(writer, transforms[i], transform => transform.Write(writer));
                    }
                    else
                    {
                        writer.WriteUInt32(0);
                    }
                }
            }
        }

        if (revision > 4)
        {
            writer.WriteUInt32(modifyStamp);
        }

        if (revision > 0xC)
        {
            writer.WriteBoolean(force3DCrowd);
        }

        if (revision > 5)
        {
            writer.WriteBoolean(show3DOnly);
        }

        if (revision > 0xB)
        {
            Symbol.Write(writer, focus);
        }

        if (revision > 14)
        {
            writer.WriteInt32(charForceLod);
        }
        if (revision > 15)
        {
            writer.WriteInt32(oldValue16);
        }

        if (revision != 0)
        {
            highlightable.Write(writer, false, parent, entry);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
