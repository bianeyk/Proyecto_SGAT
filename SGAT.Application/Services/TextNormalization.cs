using System.Text.RegularExpressions;

namespace SGAT.Application.Services;

internal static partial class TextNormalization
{
    public static string CollapseWhitespace(string value)
        => MultipleWhitespace().Replace(value.Trim(), " ");

    public static string? CollapseOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : CollapseWhitespace(value);

    public static string? TrimOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    [GeneratedRegex(@"\s+")]
    private static partial Regex MultipleWhitespace();
}
