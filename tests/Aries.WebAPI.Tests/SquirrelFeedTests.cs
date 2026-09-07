using System.Net;
using System.Text.Json;
using Xunit;

namespace Aries.WebAPI.Tests
{
    public class SquirrelFeedTests
    {
        [Fact]
        public async Task Updates_index_is_anonymous()
        {
            using var factory = new AriesApiFactory();
            factory.WriteMinimalFeed();
            using var client = factory.CreateClient();

            using var response = await client.GetAsync("/updates");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var files = doc.RootElement.GetProperty("files").EnumerateArray().Select(e => e.GetString()).ToArray();
            Assert.Contains("RELEASES", files);
            Assert.Contains("CapaPresentacion-1.0.0-full.nupkg", files);
        }

        [Fact]
        public async Task Releases_is_served_without_jwt_like_squirrel()
        {
            using var factory = new AriesApiFactory();
            factory.WriteMinimalFeed();
            using var client = factory.CreateClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, "/updates/RELEASES");
            using var response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var text = (await response.Content.ReadAsStringAsync()).Trim();
            Assert.Matches(@"^[0-9a-f]{40}\s+\S+-full\.nupkg\s+\d+$", text);

            var parts = text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            var pkgName = parts[1];
            var size = long.Parse(parts[2]);
            var onDisk = new FileInfo(Path.Combine(factory.UpdatesRoot, pkgName));
            Assert.True(onDisk.Exists);
            Assert.Equal(size, onDisk.Length);
        }

        [Fact]
        public async Task Nupkg_body_matches_and_is_not_html()
        {
            using var factory = new AriesApiFactory();
            var nupkg = factory.WriteMinimalFeed();
            using var client = factory.CreateClient();

            using var response = await client.GetAsync("/updates/" + nupkg);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("squirrel-nupkg-fixture", await response.Content.ReadAsStringAsync());
            var media = response.Content.Headers.ContentType?.MediaType ?? "";
            Assert.DoesNotContain("html", media, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("no-cache", response.Headers.CacheControl?.ToString() ?? "", StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task Missing_nupkg_is_404()
        {
            using var factory = new AriesApiFactory();
            factory.WriteMinimalFeed();
            using var client = factory.CreateClient();

            using var response = await client.GetAsync("/updates/CapaPresentacion-9.9.9-full.nupkg");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Releases_missing_is_404_before_first_publish()
        {
            using var factory = new AriesApiFactory();
            using var client = factory.CreateClient();

            using var response = await client.GetAsync("/updates/RELEASES");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
