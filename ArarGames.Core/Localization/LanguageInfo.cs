namespace ArarGames.Core.Localization;

/// <summary>
/// Represents metadata about a specific language.
/// </summary>
/// <param name="Code">The unique language code.</param>
/// <param name="NativeName">The name of the language in itself.</param>
/// <param name="EnglishName">The name of the language in English.</param>
/// <param name="ScriptFamily">The script family the language belongs to.</param>
/// <param name="IsRightToLeft">A value indicating whether the language is written right-to-left.</param>
/// <param name="FontSuffix">An optional suffix used for font loading.</param>
public record LanguageInfo(
    LanguageCode Code,
    string NativeName,
    string EnglishName,
    ScriptFamily ScriptFamily,
    bool IsRightToLeft,
    string FontSuffix = ""
);
