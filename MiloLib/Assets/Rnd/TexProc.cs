using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Rnd;

[Name("TexProc"), Description("Applies a shader to the specified texture.")]
public class TexProc : Object
{
    private ushort revision;
    private ushort altRevision;
    public RndDrawable draw = new();
    [Name("Input Texture"), Description("texture to read from")]
    public Symbol inputTex = Symbol.Empty;
    [Name("Output Texture"), Description("texture to write to")]
    public Symbol outputTex = Symbol.Empty;
    [Name("Shader"), Description("Shader to apply")]
    public int shaderType;
    public bool drawPreClear;
    [Name("Frequency"), Description("Wave frequency (for twirl shader only!)")]
    public float frequency;
    [Name("Amplitude"), Description("Wave amplitude (for twirl shader only!)")]
    public float amplitude;
    [Name("Amplitude Bump"), Description("Wave amplitude (for twirl shader only!)")]
    public float amplitudeBump;
    public float oldValue;
    [Name("Phase Vel"), Description("Wave phase velocity (change) (for twirl shader only!)")]
    public float phaseVel;

    public new TexProc Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        draw.Read(reader, false, parent, entry);
        inputTex = Symbol.Read(reader);
        outputTex = Symbol.Read(reader);
        shaderType = reader.ReadInt32();
        drawPreClear = reader.ReadBoolean();
        if (revision >= 2)
        {
            frequency = reader.ReadFloat();
            amplitude = reader.ReadFloat();
        }
        if (revision >= 3)
        {
            amplitudeBump = reader.ReadFloat();
        }
        if (revision == 2)
        {
            oldValue = reader.ReadFloat();
        }
        if (revision >= 2)
        {
            phaseVel = reader.ReadFloat();
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 3);
        base.Write(writer, false, parent, entry);
        draw.Write(writer, false, parent, entry);
        Symbol.Write(writer, inputTex);
        Symbol.Write(writer, outputTex);
        writer.WriteInt32(shaderType);
        writer.WriteBoolean(drawPreClear);
        if (revision >= 2)
        {
            writer.WriteFloat(frequency);
            writer.WriteFloat(amplitude);
        }
        if (revision >= 3)
        {
            writer.WriteFloat(amplitudeBump);
        }
        if (revision == 2)
        {
            writer.WriteFloat(oldValue);
        }
        if (revision >= 2)
        {
            writer.WriteFloat(phaseVel);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
