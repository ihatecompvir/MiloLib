using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("PropAnim"), Description("Animate any properties on target object")]
public class RndPropAnim : Object
{
    public class PropKey
    {
        public interface IAnimEvent
        {
            float Pos { get; set; }
        }

        public struct AnimEventFloat : IAnimEvent
        {
            public float Value { get; set; }
            public float Pos { get; set; }
        }

        public struct AnimEventColor : IAnimEvent
        {
            public HmxColor4 Value { get; set; }
            public float Pos { get; set; }
        }

        public struct AnimEventObject : IAnimEvent
        {
            public Symbol Text1 { get; set; }
            public Symbol Text2 { get; set; }
            public float Pos { get; set; }
        }

        public struct AnimEventBool : IAnimEvent
        {
            public bool Value { get; set; }
            public float Pos { get; set; }
        }

        public struct AnimEventQuat : IAnimEvent
        {
            public MiloLib.Classes.Vector4 Value { get; set; }
            public float Pos { get; set; }
        }

        public struct AnimEventVector3 : IAnimEvent
        {
            public MiloLib.Classes.Vector3 Value { get; set; }
            public float Pos { get; set; }
        }

        public struct AnimEventSymbol : IAnimEvent
        {
            public Symbol Text { get; set; }
            public float Pos { get; set; }
        }

        public enum Interpolation : int
        {
            kStep,
            kLinear,
            kSpline,
            kSlerp,
            kHermite,
            kEaseIn,
            kEaseOut
        }

        public enum PropType : int
        {
            kPropFloat,
            kPropColor,
            kPropObject,
            kPropBool,
            kPropQuat,
            kPropVector3,
            kPropSymbol
        }

        public enum ExceptionID : int
        {
            kNoException,
            kTransQuat,
            kTransScale,
            kTransPos,
            kDirEvent,
            kHandleInterp,
            kMacro
        }

        public PropType type1;
        public PropType type2;

        public Symbol target = Symbol.Empty;

        [Name("Prop")]
        public ObjectFields.DTBParent dtb = new();

        [MinVersion(8)]
        public Interpolation interpolation;
        [MinVersion(10)]
        public Symbol interpHandler = Symbol.Empty;

        [MinVersion(11)]
        public ExceptionID exceptionType;

        [MinVersion(13)]
        public bool unkBool;

        [Name("Keys")]
        public List<IAnimEvent> keys = new();

        public PropKey Read(EndianReader reader, uint revision)
        {
            type1 = (PropType)reader.ReadInt32();
            type2 = (PropType)reader.ReadInt32();
            if (type1 < PropType.kPropFloat || type1 > PropType.kPropSymbol)
            {
                throw new InvalidDataException("PropAnim key type is invalid.");
            }

            target = Symbol.Read(reader);

            dtb.Read(reader);

            if (revision >= 8)
            {
                interpolation = (Interpolation)reader.ReadInt32();
            }

            if (revision > 9)
            {
                interpHandler = Symbol.Read(reader);
            }

            if (revision > 10)
            {
                exceptionType = (ExceptionID)reader.ReadInt32();
            }

            if (revision > 12)
            {
                unkBool = reader.ReadBoolean();
            }

            uint keysCount = reader.ReadUInt32();
            if (keysCount > (reader.BaseStream.Length - reader.BaseStream.Position) / 5)
            {
                throw new InvalidDataException("PropAnim key count exceeds the remaining data.");
            }

            keys = new List<IAnimEvent>((int)keysCount);
            switch (type1)
            {
                case PropType.kPropFloat:
                    for (int i = 0; i < keysCount; i++)
                    {
                        keys.Add(new AnimEventFloat { Value = reader.ReadFloat(), Pos = reader.ReadFloat() });
                    }
                    break;
                case PropType.kPropColor:
                    for (int i = 0; i < keysCount; i++)
                    {
                        keys.Add(new AnimEventColor { Value = new HmxColor4().Read(reader), Pos = reader.ReadFloat() });
                    }
                    break;
                case PropType.kPropObject:
                    for (int i = 0; i < keysCount; i++)
                    {
                        Symbol dir = revision > 8 ? Symbol.Read(reader) : Symbol.Empty;
                        keys.Add(new AnimEventObject { Text1 = dir, Text2 = Symbol.Read(reader), Pos = reader.ReadFloat() });
                    }
                    break;
                case PropType.kPropBool:
                    for (int i = 0; i < keysCount; i++)
                    {
                        keys.Add(new AnimEventBool { Value = reader.ReadBoolean(), Pos = reader.ReadFloat() });
                    }
                    break;
                case PropType.kPropQuat:
                    for (int i = 0; i < keysCount; i++)
                    {
                        keys.Add(new AnimEventQuat { Value = new MiloLib.Classes.Vector4().Read(reader), Pos = reader.ReadFloat() });
                    }
                    break;
                case PropType.kPropVector3:
                    for (int i = 0; i < keysCount; i++)
                    {
                        keys.Add(new AnimEventVector3 { Value = new MiloLib.Classes.Vector3().Read(reader), Pos = reader.ReadFloat() });
                    }
                    break;
                case PropType.kPropSymbol:
                    for (int i = 0; i < keysCount; i++)
                    {
                        keys.Add(new AnimEventSymbol { Text = Symbol.Read(reader), Pos = reader.ReadFloat() });
                    }
                    break;
            }


            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            writer.WriteInt32((int)type1);
            writer.WriteInt32((int)type2);
            Symbol.Write(writer, target);
            dtb.Write(writer);
            if (revision >= 8)
            {
                writer.WriteInt32((int)interpolation);
            }

            if (revision > 9)
            {
                Symbol.Write(writer, interpHandler);
            }

            if (revision > 10)
            {
                writer.WriteInt32((int)exceptionType);
            }

            if (revision > 12)
            {
                writer.WriteBoolean(unkBool);
            }

            writer.WriteUInt32((uint)keys.Count);
            switch (type1)
            {
                case PropType.kPropFloat:
                    foreach (AnimEventFloat key in keys)
                    {
                        writer.WriteFloat(key.Value);
                        writer.WriteFloat(key.Pos);
                    }
                    break;
                case PropType.kPropColor:
                    foreach (AnimEventColor key in keys)
                    {
                        key.Value.Write(writer);
                        writer.WriteFloat(key.Pos);
                    }
                    break;
                case PropType.kPropObject:
                    foreach (AnimEventObject key in keys)
                    {
                        if (revision > 8)
                        {
                            Symbol.Write(writer, key.Text1);
                        }

                        Symbol.Write(writer, key.Text2);
                        writer.WriteFloat(key.Pos);
                    }
                    break;
                case PropType.kPropBool:
                    foreach (AnimEventBool key in keys)
                    {
                        writer.WriteBoolean(key.Value);
                        writer.WriteFloat(key.Pos);
                    }
                    break;
                case PropType.kPropQuat:
                    foreach (AnimEventQuat key in keys)
                    {
                        key.Value.Write(writer);
                        writer.WriteFloat(key.Pos);
                    }
                    break;
                case PropType.kPropVector3:
                    foreach (AnimEventVector3 key in keys)
                    {
                        key.Value.Write(writer);
                        writer.WriteFloat(key.Pos);
                    }
                    break;
                case PropType.kPropSymbol:
                    foreach (AnimEventSymbol key in keys)
                    {
                        Symbol.Write(writer, key.Text);
                        writer.WriteFloat(key.Pos);
                    }
                    break;
            }
        }

        public override string ToString()
        {
            if (interpHandler.value == "")
            {
                return $"target: {target} type: {type1} interp: {interpolation} exceptionType: {exceptionType} numKeys: {keys.Count}";
            }
            else
            {
                return $"target: {target} type: {type1} interp: {interpolation} interpHandler: {interpHandler} exceptionType: {exceptionType} numKeys: {keys.Count}";
            }
        }


    }
    private ushort altRevision;
    private ushort revision;
    private bool loopBeforeKeys;

    public RndAnimatable anim = new();

    [Name("Loop"), Description("Do I self loop on SetFrame"), MinVersion(12)]
    public bool loop;

    [Name("Flow Labels"), Description("the names of possible flow labels you can place on this timeline (i.e. 'footstep')"), MinVersion(14)]
    public List<Symbol> flowLabels = new();

    [Name("Intensity"), Description("Scales all animation keyframe values by this #"), MinVersion(15)]
    public float intensity = 1.0f;

    [Name("Prop Keys")]
    public List<PropKey> propKeys = new();

    public new RndPropAnim Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        anim = anim.Read(reader, parent, entry);

        if (revision < 7)
        {
            ReadPre7(reader, standalone, parent, entry);
            return this;
        }

        long keysStart = reader.BaseStream.Position;
        loopBeforeKeys = false;
        try
        {
            ReadPropertyKeys(reader);
            if (standalone && revision == 12 && parent.revision == 25)
            {
                ValidateRevision12Boundary(reader);
            }
        }
        catch (Exception error) when (standalone && revision == 12 && parent.revision == 25 && (error is InvalidDataException || error is EndOfStreamException))
        {
            reader.BaseStream.Position = keysStart;
            loopBeforeKeys = true;
            ReadPropertyKeys(reader);
            ValidateRevision12Boundary(reader);
        }

        if (revision >= 14)
        {
            flowLabels = AssetHelpers.ReadList(reader, () => Symbol.Read(reader));
        }
        if (revision >= 15)
        {
            intensity = reader.ReadFloat();
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

        base.Write(writer, false, parent, entry);

        anim.Write(writer);

        if (revision < 7)
        {
            WritePre7(writer, standalone, parent, entry);
            return;
        }

        if (loopBeforeKeys)
        {
            writer.WriteBoolean(loop);
        }

        AssetHelpers.WriteList(writer, propKeys, propKey => propKey.Write(writer, revision));

        if (revision >= 12 && !loopBeforeKeys)
        {
            writer.WriteBoolean(loop);
        }

        if (revision >= 14)
        {
            AssetHelpers.WriteList(writer, flowLabels, value => Symbol.Write(writer, value));
        }
        if (revision >= 15)
        {
            writer.WriteFloat(intensity);
        }

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private void ReadPropertyKeys(EndianReader reader)
    {
        if (loopBeforeKeys)
        {
            byte value = reader.ReadByte();
            if (value > 1)
            {
                throw new InvalidDataException("PropAnim loop flag must be zero or one.");
            }

            loop = value == 1;
        }

        uint propKeysCount = reader.ReadUInt32();
        if (propKeysCount > (reader.BaseStream.Length - reader.BaseStream.Position) / 17)
        {
            throw new InvalidDataException("PropAnim property count exceeds the remaining data.");
        }

        propKeys = new List<PropKey>((int)propKeysCount);
        for (int i = 0; i < propKeysCount; i++)
        {
            PropKey propKey = new();
            propKey.Read(reader, revision);
            propKeys.Add(propKey);
        }

        if (revision >= 12 && !loopBeforeKeys)
        {
            loop = reader.ReadBoolean();
        }
    }

    private static void ValidateRevision12Boundary(EndianReader reader)
    {
        long position = reader.BaseStream.Position;
        try
        {
            uint expected = 0xDEADDEAD;
            if (reader.Endianness == Endian.BigEndian)
            {
                expected = 0xADDEADDE;
            }

            if (reader.ReadUInt32() != expected)
            {
                throw new InvalidDataException("PropAnim revision 12 does not end at the expected marker.");
            }
        }
        finally
        {
            reader.BaseStream.Position = position;
        }
    }

    private void ReadPre7(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        Symbol sharedTarget = Symbol.Empty;
        if (revision < 2)
        {
            sharedTarget = Symbol.Read(reader);
        }

        propKeys = AssetHelpers.ReadList(reader, () => ReadPre7PropKey(reader, sharedTarget));

        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
    }

    private PropKey ReadPre7PropKey(EndianReader reader, Symbol sharedTarget)
    {
        PropKey propKey = new();

        if (revision >= 2)
        {
            propKey.target = Symbol.Read(reader);
        }
        else
        {
            propKey.target = sharedTarget;
        }

        if (revision < 1)
        {
            Symbol sym = Symbol.Read(reader);
            propKey.dtb = new ObjectFields.DTBParent
            {
                hasTree = true,
                id = 0,
                children = new List<ObjectFields.DTBNode>
                {
                    new() { type = ObjectFields.NodeType.Symbol, value = sym }
                }
            };
        }
        else
        {
            propKey.dtb.Read(reader);
        }

        if (revision < 3)
        {
            propKey.type1 = PropKey.PropType.kPropFloat;
            propKey.type2 = PropKey.PropType.kPropFloat;

            propKey.keys = AssetHelpers.ReadList<PropKey.IAnimEvent>(reader, () => new PropKey.AnimEventFloat { Value = reader.ReadFloat(), Pos = reader.ReadFloat() });
        }
        else
        {
            int animType = reader.ReadInt32();
            propKey.type1 = (PropKey.PropType)animType;
            propKey.type2 = (PropKey.PropType)animType;

            List<PropKey.IAnimEvent> floatKeys = AssetHelpers.ReadList<PropKey.IAnimEvent>(reader, () => new PropKey.AnimEventFloat { Value = reader.ReadFloat(), Pos = reader.ReadFloat() });

            List<PropKey.IAnimEvent> colorKeys = AssetHelpers.ReadList<PropKey.IAnimEvent>(reader, () => new PropKey.AnimEventColor { Value = new HmxColor4().Read(reader), Pos = reader.ReadFloat() });

            List<PropKey.IAnimEvent> objKeys = new();
            if (revision > 3)
            {
                objKeys = AssetHelpers.ReadList<PropKey.IAnimEvent>(reader, () => new PropKey.AnimEventObject { Text1 = Symbol.Empty, Text2 = Symbol.Read(reader), Pos = reader.ReadFloat() });
            }

            List<PropKey.IAnimEvent> boolKeys = new();
            if (revision > 4)
            {
                boolKeys = AssetHelpers.ReadList<PropKey.IAnimEvent>(reader, () => new PropKey.AnimEventBool { Value = reader.ReadBoolean(), Pos = reader.ReadFloat() });
            }

            List<PropKey.IAnimEvent> quatKeys = new();
            if (revision > 5)
            {
                quatKeys = AssetHelpers.ReadList<PropKey.IAnimEvent>(reader, () => new PropKey.AnimEventQuat { Value = new MiloLib.Classes.Vector4().Read(reader), Pos = reader.ReadFloat() });
            }

            switch (propKey.type1)
            {
                case PropKey.PropType.kPropFloat:
                    propKey.keys.AddRange(floatKeys);
                    break;
                case PropKey.PropType.kPropColor:
                    propKey.keys.AddRange(colorKeys);
                    break;
                case PropKey.PropType.kPropObject:
                    propKey.keys.AddRange(objKeys);
                    break;
                case PropKey.PropType.kPropBool:
                    propKey.keys.AddRange(boolKeys);
                    break;
                case PropKey.PropType.kPropQuat:
                    propKey.keys.AddRange(quatKeys);
                    break;
            }
        }

        propKey.interpolation = (propKey.type1 == PropKey.PropType.kPropObject || propKey.type1 == PropKey.PropType.kPropBool)
            ? PropKey.Interpolation.kStep
            : PropKey.Interpolation.kLinear;

        return propKey;
    }

    private void WritePre7(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        if (revision < 2)
        {
            Symbol.Write(writer, propKeys.Count > 0 ? propKeys[0].target : Symbol.Empty);
        }

        AssetHelpers.WriteList(writer, propKeys, propKey => WritePre7PropKey(writer, propKey));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

    private void WritePre7PropKey(EndianWriter writer, PropKey propKey)
    {
        if (revision >= 2)
        {
            Symbol.Write(writer, propKey.target);
        }

        if (revision < 1)
        {
            if (propKey.dtb.hasTree && propKey.dtb.children != null && propKey.dtb.children.Count > 0)
            {
                Symbol.Write(writer, (Symbol)propKey.dtb.children[0].value);
            }
            else
            {
                Symbol.Write(writer, Symbol.Empty);
            }
        }
        else
        {
            propKey.dtb.Write(writer);
        }

        if (revision < 3)
        {
            AssetHelpers.WriteList(writer, propKey.keys, key => { PropKey.AnimEventFloat floatKey = (PropKey.AnimEventFloat)key; writer.WriteFloat(floatKey.Value); writer.WriteFloat(floatKey.Pos); });
        }
        else
        {
            writer.WriteInt32((int)propKey.type1);

            if (propKey.type1 == PropKey.PropType.kPropFloat)
            {
                AssetHelpers.WriteList(writer, propKey.keys, key => { PropKey.AnimEventFloat floatKey = (PropKey.AnimEventFloat)key; writer.WriteFloat(floatKey.Value); writer.WriteFloat(floatKey.Pos); });
            }
            else
            {
                writer.WriteUInt32(0);
            }

            if (propKey.type1 == PropKey.PropType.kPropColor)
            {
                AssetHelpers.WriteList(writer, propKey.keys, key => { PropKey.AnimEventColor colorKey = (PropKey.AnimEventColor)key; colorKey.Value.Write(writer); writer.WriteFloat(colorKey.Pos); });
            }
            else
            {
                writer.WriteUInt32(0);
            }

            if (revision > 3)
            {
                if (propKey.type1 == PropKey.PropType.kPropObject)
                {
                    AssetHelpers.WriteList(writer, propKey.keys, key => { PropKey.AnimEventObject objKey = (PropKey.AnimEventObject)key; Symbol.Write(writer, objKey.Text2); writer.WriteFloat(objKey.Pos); });
                }
                else
                {
                    writer.WriteUInt32(0);
                }
            }

            if (revision > 4)
            {
                if (propKey.type1 == PropKey.PropType.kPropBool)
                {
                    AssetHelpers.WriteList(writer, propKey.keys, key => { PropKey.AnimEventBool boolKey = (PropKey.AnimEventBool)key; writer.WriteBoolean(boolKey.Value); writer.WriteFloat(boolKey.Pos); });
                }
                else
                {
                    writer.WriteUInt32(0);
                }
            }

            if (revision > 5)
            {
                if (propKey.type1 == PropKey.PropType.kPropQuat)
                {
                    AssetHelpers.WriteList(writer, propKey.keys, key => { PropKey.AnimEventQuat quatKey = (PropKey.AnimEventQuat)key; quatKey.Value.Write(writer); writer.WriteFloat(quatKey.Pos); });
                }
                else
                {
                    writer.WriteUInt32(0);
                }
            }
        }
    }
}
