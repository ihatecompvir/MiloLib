using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("RhythmBattlePlayer"), Description("The state of a player in Rhythm Battle.")]
public class RhythmBattlePlayer : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Combo Position Anim"), Description("instruction display")]
    public Symbol comboPosAnim = Symbol.Empty;
    [Name("Combo Color Anim"), Description("instruction display")]
    public Symbol comboColorAnim = Symbol.Empty;
    [Name("Reset Combo Anim"), Description("instruction display")]
    public Symbol resetComboAnim = Symbol.Empty;
    [Name("2x Mult Anim"), Description("instruction display")]
    public Symbol multiplier2xAnim = Symbol.Empty;
    [Name("3x Mult Anim"), Description("instruction display")]
    public Symbol multiplier3xAnim = Symbol.Empty;
    [Name("4x Mult Anim"), Description("instruction display")]
    public Symbol multiplier4xAnim = Symbol.Empty;
    public Symbol oldLabel = Symbol.Empty;
    [Name("Score Label"), Description("instruction display")]
    public Symbol scoreLabel = Symbol.Empty;
    [Name("Player"), Description("which player is this")]
    public int player;
    [Name("Boxydir"), Description("override the world boxydir")]
    public Symbol boxyDir = Symbol.Empty;

    public new RhythmBattlePlayer Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 1);
        base.Read(reader, false, parent, entry);
        comboPosAnim = Symbol.Read(reader);
        comboColorAnim = Symbol.Read(reader);
        resetComboAnim = Symbol.Read(reader);
        multiplier2xAnim = Symbol.Read(reader);
        multiplier3xAnim = Symbol.Read(reader);
        multiplier4xAnim = Symbol.Read(reader);
        oldLabel = Symbol.Read(reader);
        scoreLabel = Symbol.Read(reader);
        player = reader.ReadInt32();
        if (revision >= 1)
        {
            boxyDir = Symbol.Read(reader);
        }
        if (standalone)
        {
            AssetHelpers.ReadEnd(reader, parent, entry, revision);
        }
        return this;
    }

    public override void Write(EndianWriter writer, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry? entry)
    {
        AssetHelpers.WriteRevision(writer, revision, altRevision, 1);
        base.Write(writer, false, parent, entry);
        Symbol.Write(writer, comboPosAnim);
        Symbol.Write(writer, comboColorAnim);
        Symbol.Write(writer, resetComboAnim);
        Symbol.Write(writer, multiplier2xAnim);
        Symbol.Write(writer, multiplier3xAnim);
        Symbol.Write(writer, multiplier4xAnim);
        Symbol.Write(writer, oldLabel);
        Symbol.Write(writer, scoreLabel);
        writer.WriteInt32(player);
        if (revision >= 1)
        {
            Symbol.Write(writer, boxyDir);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
