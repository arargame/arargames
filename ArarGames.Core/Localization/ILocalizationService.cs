using System;
using System.Collections.Generic;

namespace ArarGames.Core.Localization;

/// <summary>
/// Provides localization services for managing and retrieving translated texts.
/// </summary>
public interface ILocalizationService
{
    /// <summary>
    /// Gets the currently active language.
    /// </summary>
    LanguageCode CurrentLanguage { get; }

    /// <summary>
    /// Gets the list of supported languages.
    /// </summary>
    IReadOnlyList<LanguageCode> SupportedLanguages { get; }

    /// <summary>
    /// Sets the active language and optionally provides a custom loader for the localization JSON.
    /// </summary>
    void SetLanguage(LanguageCode code, Func<string, string>? jsonLoader = null);

    /// <summary>
    /// Retrieves a localized string by its key.
    /// </summary>
    string Get(string key);

    /// <summary>
    /// Retrieves a localized string by its key and formats it with the provided arguments.
    /// </summary>
    string Get(string key, params object[] args);

    /// <summary>
    /// Determines whether the specified key exists in the current localization data.
    /// </summary>
    bool HasKey(string key);

    /// <summary>
    /// Occurs when the active language has changed.
    /// </summary>
    event Action<LanguageCode>? LanguageChanged;
}
