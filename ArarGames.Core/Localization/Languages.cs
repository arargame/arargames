using System;
using System.Collections.Generic;

namespace ArarGames.Core.Localization;

/// <summary>
/// Provides metadata for all supported languages.
/// </summary>
public static class Languages
{
    /// <summary>
    /// Gets a dictionary containing metadata for all supported languages.
    /// </summary>
    public static Dictionary<LanguageCode, LanguageInfo> All { get; } = new()
    {
        { LanguageCode.En, new LanguageInfo(LanguageCode.En, "English", "English", ScriptFamily.Latin, false) },
        { LanguageCode.Tr, new LanguageInfo(LanguageCode.Tr, "Türkçe", "Turkish", ScriptFamily.Latin, false) },
        { LanguageCode.De, new LanguageInfo(LanguageCode.De, "Deutsch", "German", ScriptFamily.Latin, false) },
        { LanguageCode.Fr, new LanguageInfo(LanguageCode.Fr, "Français", "French", ScriptFamily.Latin, false) },
        { LanguageCode.Es, new LanguageInfo(LanguageCode.Es, "Español", "Spanish", ScriptFamily.Latin, false) },
        { LanguageCode.Ru, new LanguageInfo(LanguageCode.Ru, "Русский", "Russian", ScriptFamily.Cyrillic, false, "-Cyrillic") },
        { LanguageCode.Ar, new LanguageInfo(LanguageCode.Ar, "العربية", "Arabic", ScriptFamily.Arabic, true, "-Arabic") },
        { LanguageCode.Fa, new LanguageInfo(LanguageCode.Fa, "فارسی", "Persian", ScriptFamily.Arabic, true, "-Arabic") },
        { LanguageCode.He, new LanguageInfo(LanguageCode.He, "עברית", "Hebrew", ScriptFamily.Arabic, true, "-Hebrew") },
        { LanguageCode.Hi, new LanguageInfo(LanguageCode.Hi, "हिन्दी", "Hindi", ScriptFamily.Indic, false, "-Indic") },
        { LanguageCode.ZhCn, new LanguageInfo(LanguageCode.ZhCn, "简体中文", "Chinese (Simplified)", ScriptFamily.CJK, false, "-CJK") },
        { LanguageCode.Ja, new LanguageInfo(LanguageCode.Ja, "日本語", "Japanese", ScriptFamily.CJK, false, "-CJK") },
        { LanguageCode.Ko, new LanguageInfo(LanguageCode.Ko, "한국어", "Korean", ScriptFamily.CJK, false, "-CJK") }
        // Note: Extended languages can be added here following the same pattern.
    };
}
