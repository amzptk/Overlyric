namespace Overlyric.App;

/// <summary>
/// The user's lyric colours: parsing the <c>#RRGGBB</c> text the config stores, and the swatches the
/// settings window offers. An empty string is a real value - "follow the palette" - so a config written
/// before this existed draws exactly what it drew before.
/// </summary>
internal static class LyricColor
{
    /// <summary>
    /// The swatches offered in Settings → Appearance, as opaque 0xFFRRGGBB. Bright enough to read over
    /// the dark backdrop and over a game; anything else can be typed into config.json by hand.
    /// </summary>
    public static readonly (string Name, uint Argb)[] Swatches =
    [
        ("White", 0xFFFFFFFF),
        ("Yellow", 0xFFFFE066),
        ("Orange", 0xFFFFA94D),
        ("Red", 0xFFFF6B6B),
        ("Pink", 0xFFFF8AD8),
        ("Purple", 0xFFB794FF),
        ("Blue", 0xFF7CC4FF),
        ("Green", 0xFF7CF2A6)
    ];

    /// <summary>
    /// Parse <c>#RRGGBB</c> (the leading '#' optional, case-insensitive) into opaque 0xFFRRGGBB.
    /// Anything else - including empty, which means "use the palette" - returns null.
    /// </summary>
    public static uint? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        var span = text.AsSpan().Trim();

        if (span.Length > 0 && span[0] == '#')
        {
            span = span[1..];
        }

        if (span.Length != 6)
        {
            return null;
        }

        uint rgb = 0;

        foreach (var c in span)
        {
            int digit = c switch
            {
                >= '0' and <= '9' => c - '0',
                >= 'a' and <= 'f' => c - 'a' + 10,
                >= 'A' and <= 'F' => c - 'A' + 10,
                _ => -1
            };

            if (digit < 0)
            {
                return null;
            }

            rgb = (rgb << 4) | (uint)digit;
        }

        return 0xFF000000u | rgb;
    }

    /// <summary>The canonical stored form: <c>#RRGGBB</c>, upper case.</summary>
    public static string Format(uint argb) => $"#{argb & 0x00FFFFFF:X6}";

    /// <summary>
    /// Repair a hand-edited value: a valid colour comes back canonical, anything else becomes empty,
    /// which follows the palette rather than refusing to start.
    /// </summary>
    public static string Normalise(string? text) => Parse(text) is { } argb ? Format(argb) : string.Empty;
}
