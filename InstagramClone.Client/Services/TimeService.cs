using System.Globalization;
using Humanizer;

namespace InstagramClone.Client.Services;

/// <summary>
/// Service for time-related formatting operations with localization support
/// Backed by Humanizer with explicit culture mapping and satellite language packages
/// </summary>
public class TimeService
{
    private static CultureInfo GetHumanizerCulture(CultureInfo culture)
    {
        // Map neutral cultures to specific ones to ensure the correct satellite assemblies are used
        return culture.Name switch
        {
            "fr" => new CultureInfo("fr-FR"),
            "ar" => new CultureInfo("ar-SA"),
            "en" => new CultureInfo("en-US"),
            _ => culture.IsNeutralCulture ? CultureInfo.CreateSpecificCulture(culture.Name) : culture
        };
    }

    /// <summary>
    /// Converts a DateTime to a humanized "time ago" string (e.g., "2 hours ago")
    /// Uses the current culture for localization
    /// </summary>
    public string GetTimeAgo(DateTime dateTime)
    {
        var culture = GetHumanizerCulture(CultureInfo.CurrentUICulture);
        return dateTime.Humanize(utcDate: true, culture: culture);
    }

    /// <summary>
    /// Converts a DateTime to a humanized "time ago" string with a specific culture
    /// </summary>
    public string GetTimeAgo(DateTime dateTime, CultureInfo culture)
    {
        return dateTime.Humanize();
    }

    /// <summary>
    /// Formats a DateTime for Instagram-style short format
    /// - Less than 1 week: "2h ago", "3d ago"
    /// - More than 1 week: "October 24"
    /// </summary>
    public string GetInstagramTimeAgo(DateTime dateTime)
    {
        var culture = GetHumanizerCulture(CultureInfo.CurrentUICulture);
        var timeSpan = DateTime.UtcNow - dateTime;
        if (timeSpan.TotalDays < 7)
        {
            return dateTime.Humanize();
        }
        return dateTime.ToString("MMMM d", culture);
    }

    /// <summary>
    /// Formats a DateTime for post detail view (uppercase, full format)
    /// e.g., "OCTOBER 24, 2024"
    /// </summary>
    public string GetPostDetailTime(DateTime dateTime)
    {
        var culture = CultureInfo.CurrentUICulture;
        return dateTime.ToString("MMMM d, yyyy", culture).ToUpper(culture);
    }

    // No custom formatter needed now that Humanizer is used directly
}
