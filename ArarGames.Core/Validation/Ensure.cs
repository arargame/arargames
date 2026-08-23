using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ArarGames.Core.Validation;

/// <summary>
/// Provides guard clauses for parameter validation.
/// </summary>
public static class Ensure
{
    /// <summary>
    /// Ensures that the specified value is not null.
    /// </summary>
    public static void NotNull<T>([NotNull] T? value, [CallerArgumentExpression("value")] string paramName = "") where T : class
    {
        if (value is null)
        {
            throw new ArgumentNullException(paramName);
        }
    }

    /// <summary>
    /// Ensures that the specified string is not null or white space.
    /// </summary>
    public static void NotNullOrWhiteSpace([NotNull] string? value, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", paramName);
        }
    }

    /// <summary>
    /// Ensures that the specified condition is true.
    /// </summary>
    public static void IsTrue(bool condition, string message)
    {
        if (!condition)
        {
            throw new ArgumentException(message);
        }
    }

    /// <summary>
    /// Ensures that the specified condition is false.
    /// </summary>
    public static void IsFalse(bool condition, string message)
    {
        if (condition)
        {
            throw new ArgumentException(message);
        }
    }

    /// <summary>
    /// Ensures that the integer value falls within the specified range (inclusive).
    /// </summary>
    public static void InRange(int value, int min, int max, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value {value} is not between {min} and {max}.");
        }
    }

    /// <summary>
    /// Ensures that the float value falls within the specified range (inclusive).
    /// </summary>
    public static void InRange(float value, float min, float max, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value < min || value > max)
        {
            throw new ArgumentOutOfRangeException(paramName, $"Value {value} is not between {min} and {max}.");
        }
    }

    /// <summary>
    /// Ensures that the integer value is positive (greater than zero).
    /// </summary>
    public static void Positive(int value, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "Value must be positive.");
        }
    }

    /// <summary>
    /// Ensures that the float value is positive (greater than zero).
    /// </summary>
    public static void Positive(float value, [CallerArgumentExpression("value")] string paramName = "")
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(paramName, "Value must be positive.");
        }
    }
}
