using System.Net;
using Asp.NetCore6._0_LabourPest_Project.Presentation.Reviews;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace PublicWeb.Tests;

public class RecaptchaVerifierTests
{
    [Theory]
    [InlineData("{\"success\":true}", CaptchaVerification.Valid)]
    [InlineData("{ \"success\" : true }", CaptchaVerification.Valid)]
    [InlineData("{\"success\":false}", CaptchaVerification.Rejected)]
    [InlineData("{\"success\":false,\"error-codes\":[\"timeout-or-duplicate\"]}", CaptchaVerification.Expired)]
    [InlineData("{\"success\":false,\"error-codes\":[\"invalid-input-secret\"]}", CaptchaVerification.NotConfigured)]
    [InlineData("{\"success\":\"true\"}", CaptchaVerification.Unavailable)]
    [InlineData("{}", CaptchaVerification.Unavailable)]
    [InlineData("[]", CaptchaVerification.Unavailable)]
    [InlineData("<html>unavailable</html>", CaptchaVerification.Unavailable)]
    public async Task Structured_response_is_parsed_and_tokens_are_sent_only_in_POST_body(string json, CaptchaVerification expected)
    {
        var handler = new ControlledHttpHandler(json);
        Assert.Equal(expected, await Create(handler).VerifyAsync("test-token", default));
        Assert.Equal(1, handler.Calls);
        Assert.Equal(HttpMethod.Post, handler.Method);
        Assert.Equal("https://www.google.com/recaptcha/api/siteverify", handler.Url);
        Assert.Equal("secret=test-secret&response=test-token", handler.Body);
    }

    [Theory]
    [InlineData(null, "test-secret", CaptchaVerification.Missing)]
    [InlineData("", "test-secret", CaptchaVerification.Missing)]
    [InlineData("test-token", "", CaptchaVerification.NotConfigured)]
    public async Task Missing_inputs_never_send_an_HTTP_request(string? token, string secret, CaptchaVerification expected)
    {
        var handler = new ControlledHttpHandler("{}");
        Assert.Equal(expected, await Create(handler, secret).VerifyAsync(token, default));
        Assert.Equal(0, handler.Calls);
    }

    [Theory]
    [InlineData("timeout")]
    [InlineData("network")]
    [InlineData("http")]
    public async Task Transport_failures_fail_closed_without_retry(string failure)
    {
        var handler = new ControlledHttpHandler("{}") { Failure = failure };
        Assert.Equal(CaptchaVerification.Unavailable, await Create(handler).VerifyAsync("test-token", default));
        Assert.Equal(1, handler.Calls);
    }

    private static RecaptchaVerifier Create(ControlledHttpHandler handler, string secret = "test-secret") =>
        new(new HttpClient(handler) { Timeout = TimeSpan.FromMilliseconds(50) },
            Options.Create(new RecaptchaOptions { SecretKey = secret }), NullLogger<RecaptchaVerifier>.Instance);

    private sealed class ControlledHttpHandler : HttpMessageHandler
    {
        private readonly string json;
        public string? Failure { get; set; }
        public string? Url { get; private set; }
        public HttpMethod? Method { get; private set; }
        public string? Body { get; private set; }
        public int Calls { get; private set; }
        public ControlledHttpHandler(string json) => this.json = json;
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Calls++; Url = request.RequestUri!.ToString(); Method = request.Method;
            Body = await request.Content!.ReadAsStringAsync(cancellationToken);
            if (Failure == "timeout") await Task.Delay(Timeout.Infinite, cancellationToken);
            if (Failure == "network") throw new HttpRequestException("Controlled network failure");
            return new HttpResponseMessage(Failure == "http" ? HttpStatusCode.BadGateway : HttpStatusCode.OK) { Content = new StringContent(json) };
        }
    }
}
