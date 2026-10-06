using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Asp.NetCore6._0_LabourPest_Project.Presentation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace PublicWeb.Tests;

public class PublicImageVariantsTests
{
    [Fact]
    public void Derivatives_are_used_only_for_matching_originals_and_existing_files()
    {
        var files = new Files();
        var original = new Asset("original image", DateTimeOffset.UtcNow);
        files.Items["canabicom/example.webp"] = original;
        files.Items["public-web/images/responsive/test-640.webp"] = new Asset("derivative", original.LastModified);
        files.Items["public-web/images/responsive/manifest.json"] = new Asset(JsonSerializer.Serialize(new Dictionary<string, object> {
            ["/canabicom/example.webp"] = new {
                sha256 = Convert.ToHexString(SHA256.HashData(original.Bytes)),
                variants = new[] { new { width = 640, url = "/public-web/images/responsive/test-640.webp" } }
            }
        }), original.LastModified);
        var images = new PublicImageVariants(new EnvironmentStub { WebRootFileProvider = files });
        Assert.Equal("/public-web/images/responsive/test-640.webp 640w", images.SrcSet("~/canabicom/example.webp"));
        Assert.Null(images.SrcSet("/canabicom/new-admin-image.webp"));

        // An administrator replaces an image at the same URL, with the same byte length.
        files.Items["canabicom/example.webp"] = new Asset("modified image", original.LastModified.AddSeconds(1));
        Assert.Null(images.SrcSet("/canabicom/example.webp"));
        files.Items["canabicom/example.webp"] = original;
        files.Items.Remove("public-web/images/responsive/test-640.webp");
        Assert.Null(images.SrcSet("/canabicom/example.webp"));
    }

    private sealed class Files : IFileProvider
    {
        public Dictionary<string, Asset> Items { get; } = new();
        public IFileInfo GetFileInfo(string subpath) => Items.TryGetValue(subpath, out var file) ? file : new NotFoundFileInfo(subpath);
        public IDirectoryContents GetDirectoryContents(string subpath) => NotFoundDirectoryContents.Singleton;
        public IChangeToken Watch(string filter) => NullChangeToken.Singleton;
    }
    private sealed class Asset : IFileInfo
    {
        public byte[] Bytes { get; }
        public Asset(string content, DateTimeOffset modified) { Bytes = Encoding.UTF8.GetBytes(content); LastModified = modified; }
        public bool Exists => true;
        public long Length => Bytes.Length;
        public string? PhysicalPath => null;
        public string Name => "test";
        public DateTimeOffset LastModified { get; }
        public bool IsDirectory => false;
        public Stream CreateReadStream() => new MemoryStream(Bytes);
    }
    private sealed class EnvironmentStub : IWebHostEnvironment
    {
        public string ApplicationName { get; set; } = "test";
        public string EnvironmentName { get; set; } = "Development";
        public string WebRootPath { get; set; } = "";
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = "";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
