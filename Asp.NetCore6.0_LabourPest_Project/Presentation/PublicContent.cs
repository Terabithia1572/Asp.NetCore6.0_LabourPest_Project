using System.Net;
using System.Text.RegularExpressions;

namespace Asp.NetCore6._0_LabourPest_Project.Presentation;

// Formatting helpers used only by the public Razor views.
public static class PublicContent
{
    public const string FallbackImage = "~/public-web/images/placeholder.svg";

    public static string Image(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return FallbackImage;
        var path = value.Trim();
        // Public imagery is served from existing local assets, never arbitrary profile URLs.
        if (path.Contains('\\') || path.Contains("..") || path.StartsWith("//")) return FallbackImage;
        if (path.StartsWith("/canabicom/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/labourpestcustomer/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("~/canabicom/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("~/labourpestcustomer/", StringComparison.OrdinalIgnoreCase)) return path;
        return FallbackImage;
    }

    public static string Excerpt(string? value, int limit = 170)
    {
        if (string.IsNullOrWhiteSpace(value)) return "";
        string text;
        try
        {
            text = WebUtility.HtmlDecode(Regex.Replace(value, "<[^>]*>", " ", RegexOptions.None, TimeSpan.FromMilliseconds(100)));
            text = Regex.Replace(text, @"\s+", " ", RegexOptions.None, TimeSpan.FromMilliseconds(100)).Trim();
        }
        catch (RegexMatchTimeoutException)
        {
            // Malformed content must not break a listing. Razor still encodes this text.
            text = value.Trim();
        }
        if (text.Length <= limit) return text;
        var end = text.LastIndexOf(' ', limit);
        return text[..(end > limit / 2 ? end : limit)].TrimEnd() + "…";
    }
}
