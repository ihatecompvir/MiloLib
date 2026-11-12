using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.P9;

[Name("P9WaypointConfiguration"), Description("P9 Waypoint Configuration, sits in each venue and points out appropriate waypoints for each band configuration")]
public class P9WaypointConfiguration : Object
{
    public class ConfigurationTransform
    {
        [Name("Transform")]
        public Matrix transform = new();
        [Name("Stand"), MinVersion(11)]
        public bool stand;
        private byte storedStand;
        [MinVersion(7), MaxVersion(10)]
        public Symbol oldPosition = Symbol.Empty;
        [Name("Radius"), MinVersion(12)]
        public float radius;
        [Name("Strict Radius Delta"), Description("if > 0, an absolute restriction applied to the existing radius"), MinVersion(12)]
        public float strictRadiusDelta;

        public ConfigurationTransform Read(EndianReader reader, ushort revision)
        {
            transform.Read(reader);
            if (revision >= 11)
            {
                storedStand = reader.ReadByte();
                stand = storedStand != 0;
            }
            else if (revision >= 7)
            {
                oldPosition = Symbol.Read(reader);
            }

            if (revision >= 12)
            {
                radius = reader.ReadFloat();
                strictRadiusDelta = reader.ReadFloat();
            }
            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            transform.Write(writer);
            if (revision >= 11)
            {
                byte outputStand = storedStand;
                if (stand != (storedStand != 0))
                {
                    outputStand = 0;
                    if (stand)
                    {
                        outputStand = 1;
                    }
                }
                writer.WriteByte(outputStand);
            }
            else if (revision >= 7)
            {
                Symbol.Write(writer, oldPosition);
            }

            if (revision >= 12)
            {
                writer.WriteFloat(radius);
                writer.WriteFloat(strictRadiusDelta);
            }
        }
    }

    public class ObjectTransforms
    {
        [Name("Trans"), Description("object to teleport around, don't mess with the top four.")]
        public Symbol trans = Symbol.Empty;
        [Name("Configurations")]
        public List<ConfigurationTransform> configurations = new();

        public ObjectTransforms Read(EndianReader reader, ushort revision, int count)
        {
            trans = Symbol.Read(reader);
            configurations = new List<ConfigurationTransform>(count);
            for (int index = 0; index < count; index++)
            {
                configurations.Add(new ConfigurationTransform().Read(reader, revision));
            }

            return this;
        }

        public void Write(EndianWriter writer, ushort revision)
        {
            Symbol.Write(writer, trans);
            foreach (ConfigurationTransform configuration in configurations)
            {
                configuration.Write(writer, revision);
            }
        }
    }

    private ushort revision = 12;
    private ushort altRevision;
    [Name("Configuration"), Description("Current Band Configuration. Click view or store to er, view or store")]
    public int configuration;
    public int configurationCount;
    [Name("Objects")]
    public List<ObjectTransforms> objects = new();
    [Name("Triggers"), Description("if set, the trigger to fire for this cfg. set the hit store_configuration button to set."), MinVersion(9)]
    public List<Symbol> triggers = new();

    public new P9WaypointConfiguration Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 12);
        ValidateRevision();
        base.Read(reader, false, parent, entry);
        configuration = reader.ReadInt32();
        configurationCount = reader.ReadInt32();
        ValidateConfigurationCount();
        objects = AssetHelpers.ReadList(reader, () => new ObjectTransforms().Read(reader, revision, configurationCount));
        triggers = new List<Symbol>();
        if (revision >= 9)
        {
            for (int index = 0; index < configurationCount; index++)
            {
                triggers.Add(Symbol.Read(reader));
            }
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }

        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        ValidateRevision();
        ValidateConfigurationCount();
        if (objects.Any(item => item.configurations.Count != configurationCount) || (revision >= 9 && triggers.Count != configurationCount))
        {
            throw new InvalidDataException("Waypoint transform and trigger counts must match the configuration count.");
        }

        AssetHelpers.WriteRevision(writer, revision, altRevision, 12);
        base.Write(writer, false, parent, entry);
        writer.WriteInt32(configuration);
        writer.WriteInt32(configurationCount);
        AssetHelpers.WriteList(writer, objects, item => item.Write(writer, revision));
        if (revision >= 9)
        {
            foreach (Symbol trigger in triggers)
            {
                Symbol.Write(writer, trigger);
            }
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private void ValidateRevision()
    {
        if (revision < 6 || revision > 12)
        {
            throw new NotSupportedException($"Unsupported P9WaypointConfiguration revision {revision}.");
        }
    }

    private void ValidateConfigurationCount()
    {
        if (configurationCount < 0 || configurationCount > 12)
        {
            throw new InvalidDataException($"Invalid waypoint configuration count {configurationCount}.");
        }
    }
}
