using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Ham;

[Name("RhythmBattle"), Description("Competition between two RhythmBattlePlayers.")]
public class RhythmBattle : Object
{
    private ushort revision;
    private ushort altRevision;
    [Name("Command Label"), Description("instruction display")]
    public Symbol commandLabel = Symbol.Empty;
    [Name("Player0"), Description("player 0 object")]
    public Symbol playerOne = Symbol.Empty;
    [Name("Player1"), Description("player 1 object")]
    public Symbol playerTwo = Symbol.Empty;
    public Symbol oldGroup1 = Symbol.Empty;
    public Symbol oldGroup2 = Symbol.Empty;
    public Symbol oldAnim1 = Symbol.Empty;
    public Symbol oldAnim2 = Symbol.Empty;
    public Symbol oldAnim3 = Symbol.Empty;
    public Symbol oldLabel1 = Symbol.Empty;
    public Symbol oldLabel2 = Symbol.Empty;
    public Symbol oldLabel3 = Symbol.Empty;
    public Symbol oldLabel4 = Symbol.Empty;
    [Name("Full KTB"), Description("is this keep the beat, or just groove tech experience")]
    public bool fullKTB;

    public new RhythmBattle Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision, 3);
        base.Read(reader, false, parent, entry);
        if (revision >= 2)
        {
            commandLabel = Symbol.Read(reader);
            playerOne = Symbol.Read(reader);
            playerTwo = Symbol.Read(reader);
        }
        if (revision == 1)
        {
            oldGroup1 = Symbol.Read(reader);
            oldGroup2 = Symbol.Read(reader);
            oldAnim1 = Symbol.Read(reader);
            oldAnim2 = Symbol.Read(reader);
            oldAnim3 = Symbol.Read(reader);
            oldLabel1 = Symbol.Read(reader);
            oldLabel2 = Symbol.Read(reader);
            oldLabel3 = Symbol.Read(reader);
            oldLabel4 = Symbol.Read(reader);
        }
        if (revision >= 3)
        {
            fullKTB = reader.ReadBoolean();
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
        if (revision >= 2)
        {
            Symbol.Write(writer, commandLabel);
            Symbol.Write(writer, playerOne);
            Symbol.Write(writer, playerTwo);
        }
        if (revision == 1)
        {
            Symbol.Write(writer, oldGroup1);
            Symbol.Write(writer, oldGroup2);
            Symbol.Write(writer, oldAnim1);
            Symbol.Write(writer, oldAnim2);
            Symbol.Write(writer, oldAnim3);
            Symbol.Write(writer, oldLabel1);
            Symbol.Write(writer, oldLabel2);
            Symbol.Write(writer, oldLabel3);
            Symbol.Write(writer, oldLabel4);
        }
        if (revision >= 3)
        {
            writer.WriteBoolean(fullKTB);
        }
        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }
}
