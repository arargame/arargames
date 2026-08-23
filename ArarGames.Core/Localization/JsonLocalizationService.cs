using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using ArarGames.Core.Logging;

namespace ArarGames.Core.Localization;

/// <summary>
/// A JSON-based implementation of the localization service.
/// </summary>
public class JsonLocalizationService : ILocalizationService
{
    private readonly Func<string, string> _defaultJsonLoader;
    private Dictionary<string, string> _currentDictionary = new();
    private Dictionary<string, string> _fallbackDictionary = new();

    /// <inheritdoc />
    public LanguageCode CurrentLanguage { get; private set; }

    /// <inheritdoc />
    public IReadOnlyList<LanguageCode> SupportedLanguages => [.. Languages.All.Keys];

    /// <inheritdoc />
    public event Action<LanguageCode>? LanguageChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLocalizationService"/> class.
    /// </summary>
    /// <param name="defaultJsonLoader">A function that takes a language code as a string and returns the JSON content.</param>
    public JsonLocalizationService(Func<string, string> defaultJsonLoader)
    {
        _defaultJsonLoader = defaultJsonLoader;
        
        // Ensure standard formatting globally
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }

    /// <inheritdoc />
    public void SetLanguage(LanguageCode code, Func<string, string>? jsonLoader = null)
    {
        var loader = jsonLoader ?? _defaultJsonLoader;
        
        try
        {
            // Load requested language
            string json = loader(code.ToString());
            _currentDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            
            // Load English as fallback if current is not English
            if (code != LanguageCode.En)
            {
                try
                {
                    string enJson = loader(LanguageCode.En.ToString());
                    _fallbackDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(enJson) ?? new Dictionary<string, string>();
                }
                catch
                {
                    _fallbackDictionary = new Dictionary<string, string>();
                }
            }
            else
            {
                _fallbackDictionary = _currentDictionary;
            }

            CurrentLanguage = code;
            LanguageChanged?.Invoke(CurrentLanguage);
        }
        catch (Exception ex)
        {
            GameLog.Error($"Failed to load language '{code}'. Exception: {ex.Message}").ConsoleWriter();
            // Silent failure, rely on fallbacks or key names
        }
    }

    /// <inheritdoc />
    public string Get(string key)
    {
        if (string.IsNullOrEmpty(key)) return string.Empty;

        if (_currentDictionary.TryGetValue(key, out var value))
            return value;

        if (_fallbackDictionary.TryGetValue(key, out var fallbackValue))
            return fallbackValue;

        return key; // Ultimate fallback is the key itself
    }

    /// <inheritdoc />
    public string Get(string key, params object[] args)
    {
        var format = Get(key);
        try
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }
        catch
        {
            return format; // Return unformatted if argument mismatch occurs
        }
    }

    /// <inheritdoc />
    public bool HasKey(string key)
    {
        return _currentDictionary.ContainsKey(key) || _fallbackDictionary.ContainsKey(key);
    }
}
