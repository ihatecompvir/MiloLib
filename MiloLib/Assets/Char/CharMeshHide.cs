using MiloLib.Classes;
using MiloLib.Utils;

namespace MiloLib.Assets.Char;

[Name("CharMeshHide"), Description("Hides meshes based on flags in other CharMeshHide.")]
public class CharMeshHide : Object
{
    public class Hide
    {
        public Symbol draw = Symbol.Empty;
        public int flags;
        [MinVersion(2)]
        public bool show;

        public Hide Read(EndianReader reader, uint revision)
        {
            draw = Symbol.Read(reader);
            flags = reader.ReadInt32();
            if (revision > 1)
            {
                show = reader.ReadBoolean();
            }

            return this;
        }

        public void Write(EndianWriter writer, uint revision)
        {
            Symbol.Write(writer, draw);
            writer.WriteInt32(flags);
            if (revision > 1)
            {
                writer.WriteBoolean(show);
            }
        }

        public override string ToString()
        {
            return $"{draw} flags: {flags} showing: {show}";
        }
    }

    [Flags]
    public enum HideOptions
    {
        None = 0,
        HideLongCoat = 1,
        HideLongDress = 2,
        HideLongSleeve = 4,
        HideMidSleeve = 8,
        HideFullSleeve = 16,
        HideHead = 32,
        HideLongGlove = 64,
        HideMask = 128,
        HideLongBoot = 256,
        HideShortSleeve = 512,
        HideShortBoot = 1024,
        HideLongPants = 2048,
        HideGlasses = 4096,
        HideVignette = 8192,
        HideSocks = 16384
    }

    private void SetFromFlags(int flags)
    {
        var options = (HideOptions)flags;

        hideLongCoat = options.HasFlag(HideOptions.HideLongCoat);
        hideLongDress = options.HasFlag(HideOptions.HideLongDress);
        hideLongSleeve = options.HasFlag(HideOptions.HideLongSleeve);
        hideMidSleeve = options.HasFlag(HideOptions.HideMidSleeve);
        hideFullSleeve = options.HasFlag(HideOptions.HideFullSleeve);
        hideHead = options.HasFlag(HideOptions.HideHead);
        hideLongGlove = options.HasFlag(HideOptions.HideLongGlove);
        hideMask = options.HasFlag(HideOptions.HideMask);
        hideLongBoot = options.HasFlag(HideOptions.HideLongBoot);
        hideShortSleeve = options.HasFlag(HideOptions.HideShortSleeve);
        hideShortBoot = options.HasFlag(HideOptions.HideShortBoot);
        hideLongPants = options.HasFlag(HideOptions.HideLongPants);
        hideGlasses = options.HasFlag(HideOptions.HideGlasses);
        hideVignette = options.HasFlag(HideOptions.HideVignette);
        hideSocks = options.HasFlag(HideOptions.HideSocks);
    }

    private int GetFlags()
    {
        var options = HideOptions.None;

        if (hideLongCoat)
        {
            options |= HideOptions.HideLongCoat;
        }

        if (hideLongDress)
        {
            options |= HideOptions.HideLongDress;
        }

        if (hideLongSleeve)
        {
            options |= HideOptions.HideLongSleeve;
        }

        if (hideMidSleeve)
        {
            options |= HideOptions.HideMidSleeve;
        }

        if (hideFullSleeve)
        {
            options |= HideOptions.HideFullSleeve;
        }

        if (hideHead)
        {
            options |= HideOptions.HideHead;
        }

        if (hideLongGlove)
        {
            options |= HideOptions.HideLongGlove;
        }

        if (hideMask)
        {
            options |= HideOptions.HideMask;
        }

        if (hideLongBoot)
        {
            options |= HideOptions.HideLongBoot;
        }

        if (hideShortSleeve)
        {
            options |= HideOptions.HideShortSleeve;
        }

        if (hideShortBoot)
        {
            options |= HideOptions.HideShortBoot;
        }

        if (hideLongPants)
        {
            options |= HideOptions.HideLongPants;
        }

        if (hideGlasses)
        {
            options |= HideOptions.HideGlasses;
        }

        if (hideVignette)
        {
            options |= HideOptions.HideVignette;
        }

        if (hideSocks)
        {
            options |= HideOptions.HideSocks;
        }

        const int knownBitsMask = (int)(
            HideOptions.HideLongCoat | HideOptions.HideLongDress | HideOptions.HideLongSleeve |
            HideOptions.HideMidSleeve | HideOptions.HideFullSleeve | HideOptions.HideHead |
            HideOptions.HideLongGlove | HideOptions.HideMask | HideOptions.HideLongBoot |
            HideOptions.HideShortSleeve | HideOptions.HideShortBoot | HideOptions.HideLongPants |
            HideOptions.HideGlasses | HideOptions.HideVignette | HideOptions.HideSocks
        );
        return (int)options | (flags & ~knownBitsMask);
    }


    private ushort altRevision;
    private ushort revision;

    public List<Hide> hides = new();

    public int flags;


    public bool hideLongCoat;
    public bool hideLongDress;
    public bool hideLongSleeve;
    public bool hideMidSleeve;
    public bool hideFullSleeve;
    public bool hideHead;
    public bool hideLongGlove;
    public bool hideMask;
    public bool hideLongBoot;
    public bool hideShortSleeve;
    public bool hideShortBoot;
    public bool hideLongPants;
    public bool hideGlasses;
    public bool hideVignette;
    public bool hideSocks;

    public new CharMeshHide Read(EndianReader reader, bool standalone, DirectoryMeta parent, DirectoryMeta.Entry entry)
    {
        AssetHelpers.ReadRevision(reader, out revision, out altRevision);

        base.Read(reader, false, parent, entry);

        flags = reader.ReadInt32();
        SetFromFlags(flags);

        hides = AssetHelpers.ReadList(reader, () => new Hide().Read(reader, revision));

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

        int flags = GetFlags();
        writer.WriteInt32(flags);

        AssetHelpers.WriteList(writer, hides, hide => hide.Write(writer, revision));

        if (standalone)
        {
            writer.WriteEndBytes();
        }
    }

}
