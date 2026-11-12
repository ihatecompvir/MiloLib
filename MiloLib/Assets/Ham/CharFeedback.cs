using MiloLib.Assets.Rnd;
using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("CharFeedback"), Description("Drawable for on-character filter feedback/visualization")]
public class CharFeedback : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    [Name("Target"), Description("Character for displaying filter feedback")]
    public Symbol target = Symbol.Empty;
    public Symbol oldMesh = Symbol.Empty;
    public int oldMeshValue;
    public Symbol oldAnim = Symbol.Empty;
    public Symbol oldLine = Symbol.Empty;
    public Symbol oldColor1 = Symbol.Empty;
    public Symbol oldColor2 = Symbol.Empty;
    public int oldLimbValue;
    public Symbol oldLimbSymbol = Symbol.Empty;
    [Name("Test Limbs"), Description("Test limb(s) for debugging")]
    public int testLimbs;
    [Name("Fail Trigger Secs"), Description("Min time a node must be failing to start flash")]
    public float failTriggerSecs;
    [Name("Min Fail Secs"), Description("Min time a node must be flashing to stop")]
    public float minFailSecs;
    [Name("Fail Mat"), Description("Mat to show on the character")]
    public Symbol failMat = Symbol.Empty;
    [Name("Fade Secs"), Description("Time (secs) to fade in/out")]
    public float fadeSecs;
    public int oldFadeValue;

    public new CharFeedback Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 10);
        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        target = Symbol.Read(reader);
        if (revision < 3)
        {
            if (revision > 0)
            {
                oldMesh = Symbol.Read(reader);
                oldMeshValue = reader.ReadInt32();
            }
            if (revision > 1)
            {
                oldAnim = Symbol.Read(reader);
            }
        }
        if (revision > 2)
        {
            if (revision < 6)
            {
                oldLine = Symbol.Read(reader);
                oldColor1 = Symbol.Read(reader);
                oldColor2 = Symbol.Read(reader);
            }
            else if (revision < 8)
            {
                oldLimbValue = reader.ReadInt32();
            }
            else if (revision < 9)
            {
                oldLimbSymbol = Symbol.Read(reader);
            }
            else
            {
                testLimbs = reader.ReadInt32();
            }
        }
        if (revision > 3)
        {
            failTriggerSecs = reader.ReadFloat();
        }
        if (revision > 6)
        {
            minFailSecs = reader.ReadFloat();
        }
        if (revision > 4)
        {
            failMat = Symbol.Read(reader);
        }
        if (revision > 8)
        {
            fadeSecs = reader.ReadFloat();
            if (revision < 10)
            {
                oldFadeValue = reader.ReadInt32();
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
        AssetHelpers.WriteRevision(writer, revision, altRevision, 10);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, target);
        if (revision < 3)
        {
            if (revision > 0)
            {
                Symbol.Write(writer, oldMesh);
                writer.WriteInt32(oldMeshValue);
            }
            if (revision > 1)
            {
                Symbol.Write(writer, oldAnim);
            }
        }
        if (revision > 2)
        {
            if (revision < 6)
            {
                Symbol.Write(writer, oldLine);
                Symbol.Write(writer, oldColor1);
                Symbol.Write(writer, oldColor2);
            }
            else if (revision < 8)
            {
                writer.WriteInt32(oldLimbValue);
            }
            else if (revision < 9)
            {
                Symbol.Write(writer, oldLimbSymbol);
            }
            else
            {
                writer.WriteInt32(testLimbs);
            }
        }
        if (revision > 3)
        {
            writer.WriteFloat(failTriggerSecs);
        }
        if (revision > 6)
        {
            writer.WriteFloat(minFailSecs);
        }
        if (revision > 4)
        {
            Symbol.Write(writer, failMat);
        }
        if (revision > 8)
        {
            writer.WriteFloat(fadeSecs);
            if (revision < 10)
            {
                writer.WriteInt32(oldFadeValue);
            }
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
