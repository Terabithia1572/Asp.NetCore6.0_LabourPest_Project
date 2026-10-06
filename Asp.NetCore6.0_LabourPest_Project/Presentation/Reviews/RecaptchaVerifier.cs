using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Asp.NetCore6._0_LabourPest_Project.Presentation.Reviews;

public enum CaptchaVerification { Valid, Missing, Rejected, Expired, Unavailable, NotConfigured }

public interface IReviewCaptchaVerifier
{
    Task<CaptchaVerification> VerifyAsync(string? token, CancellationToken cancellationToken);
}

// This verifier belongs only to the MVC public review form.
public sealed class RecaptchaVerifier : IReviewCaptchaVerifier
{
    private readonly HttpClient client;
    private readonly RecaptchaOptions options;
    private readonly ILogger<RecaptchaVerifier> logger;

    public RecaptchaVerifier(HttpClient client, IOptions<RecaptchaOptions> options, ILogger<RecaptchaVerifier> logger)
    {
        this.client = client;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task<CaptchaVerification> VerifyAsync(string? token, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(token)) return CaptchaVerification.Missing;
        if (token.Length > 8192) return CaptchaVerification.Rejected;
        if (string.IsNullOrWhiteSpace(options.SecretKey) || string.IsNullOrWhiteSpace(options.SiteKey))
        {
            logger.LogError("Public review CAPTCHA configuration is missing.");
            return CaptchaVerification.NotConfigured;
        }
        try
        {
            // Never put secrets or response tokens in a URL, logs or retry policy.
            using var body = new FormUrlEncodedContent(new Dictionary<string, string> {
                ["secret"] = options.SecretKey, ["response"] = token
            });
            using var response = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", body, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Public review CAPTCHA HTTP status {Status}.", (int)response.StatusCode);
                return CaptchaVerification.Unavailable;
            }
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            if (json.RootElement.ValueKind != JsonValueKind.Object ||
                !json.RootElement.TryGetProperty("success", out var success) ||
                success.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
            {
                logger.LogWarning("Public review CAPTCHA returned an invalid response shape.");
                return CaptchaVerification.Unavailable;
            }
            if (success.GetBoolean()) return CaptchaVerification.Valid;
            if (json.RootElement.TryGetProperty("error-codes", out var codes) && codes.ValueKind == JsonValueKind.Array)
            {
                var errors = codes.EnumerateArray().Where(x => x.ValueKind == JsonValueKind.String).Select(x => x.GetString()).ToArray();
                if (errors.Contains("missing-input-secret") || errors.Contains("invalid-input-secret"))
                {
                    logger.LogError("Public review CAPTCHA rejected server configuration.");
                    return CaptchaVerification.NotConfigured;
                }
                if (errors.Contains("timeout-or-duplicate")) return CaptchaVerification.Expired;
            }
            return CaptchaVerification.Rejected;
        }
        catch (Exception exception) when (exception is HttpRequestException or OperationCanceledException or JsonException)
        {
            // Exception messages/bodies may contain transport data; log only the diagnostic type.
            logger.LogWarning("Public review CAPTCHA unavailable: {ExceptionType}.", exception.GetType().Name);
            return CaptchaVerification.Unavailable;
        }
    }
}
