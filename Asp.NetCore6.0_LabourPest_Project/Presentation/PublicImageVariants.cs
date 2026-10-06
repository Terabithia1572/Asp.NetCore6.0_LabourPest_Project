using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;

namespace Asp.NetCore6._0_LabourPest_Project.Presentation;

// Known derivatives are used only while the current administration-managed original matches.
// New or changed originals immediately fall back to the existing src; no frozen image mapping.
public sealed class PublicImageVariants
{
    private readonly IWebHostEnvironment environment;
    private readonly Dictionary<string, ImageEntry> entries = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, (long Size, DateTime Modified, bool Matches)> verified = new();
    public PublicImageVariants(IWebHostEnvironment environment)
    {
        this.environment = environment;
        var file = environment.WebRootFileProvider.GetFileInfo("public-web/images/responsive/manifest.json");
        if (!file.Exists) return;
        try
        {
            using var stream = file.CreateReadStream();
            entries = JsonSerializer.Deserialize<Dictionary<string, ImageEntry>>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? entries;
        }
        catch (Exception error) when (error is IOException or JsonException) { /* Original src remains usable. */ }
    }

    public string? SrcSet(string source)
    {
        source = source.StartsWith('~') ? source[1..] : source;
        if (!entries.TryGetValue(source, out var entry)) return null;
        var original = environment.WebRootFileProvider.GetFileInfo(source.TrimStart('/'));
        if (!original.Exists) return null;
        try
        {
            if (!verified.TryGetValue(source, out var cached) || cached.Size != original.Length || cached.Modified != original.LastModified.UtcDateTime)
            {
                using var stream = original.CreateReadStream();
                using var sha = SHA256.Create();
                var matches = Convert.ToHexString(sha.ComputeHash(stream)).Equals(entry.Sha256, StringComparison.OrdinalIgnoreCase);
                cached = (original.Length, original.LastModified.UtcDateTime, matches);
                verified[source] = cached;
            }
            if (!cached.Matches || entry.Variants.Length == 0 || entry.Variants.Any(v =>
                !v.Url.StartsWith("/public-web/images/responsive/", StringComparison.Ordinal) ||
                v.Url.Contains("..") || !environment.WebRootFileProvider.GetFileInfo(v.Url.TrimStart('/')).Exists)) return null;
            return string.Join(", ", entry.Variants.Select(v => $"{v.Url} {v.Width}w"));
        }
        catch (IOException) { return null; }
    }
    public sealed class ImageEntry
    {
        public string Sha256 { get; set; } = "";
        public Variant[] Variants { get; set; } = Array.Empty<Variant>();
    }
    public sealed class Variant
    {
        public int Width { get; set; }
        public string Url { get; set; } = "";
    }
}
