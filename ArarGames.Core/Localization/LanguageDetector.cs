using System;
using System.Globalization;

namespace ArarGames.Core.Localization;

/// <summary>
/// Provides utility methods for detecting the system language.
/// </summary>
public static class LanguageDetector
{
    /// <summary>
    /// Detects the language code matching the current UI culture of the system.
    /// </summary>
    /// <returns>The detected language code, or English as a default fallback.</returns>
    public static LanguageCode DetectLanguage()
    {
        var culture = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToLowerInvariant();
        
        if (Enum.TryParse<LanguageCode>(culture, true, out var code))
        {
            return code;
        }

        return LanguageCode.En;
    }
}
