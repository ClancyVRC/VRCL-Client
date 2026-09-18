using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace VRCLClient;

internal sealed class GitHubUpdateResult
{
    public bool Success { get; init; }
    public bool Configured { get; init; }
    public bool UpdateAvailable { get; init; }
    public bool PackageAvailable { get; init; }
    public string CurrentVersion { get; init; } = "";
    public string LatestVersion { get; init; } = "";
    public string ReleaseName { get; init; } = "";
    public string ReleaseUrl { get; init; } = "";
    public string DownloadUrl { get; init; } = "";
    public string AssetName { get; init; } = "";
    public string AssetDigest { get; init; } = "";
    public string Message { get; init; } = "";
}

internal static class GitHubUpdateChecker
{
    const string ApiBase = "https://api.github.com";
    static readonly HttpClient Client = CreateClient();

    static HttpClient CreateClient()
    {
        var c = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VRCL-Client", "1.9"));
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        c.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        return c;
    }

    public static async Task<GitHubUpdateResult> CheckAsync(string currentVersion, CancellationToken cancellationToken = default)
    {
        try
        {
            var configPath = VrclPaths.AppFile(Path.Combine("Update", "github_release_config.template.json"));
            if (!File.Exists(configPath)) return Fail(currentVersion, "Update configuration was not found.");

            using var cfgDoc = JsonDocument.Parse(await File.ReadAllTextAsync(configPath, cancellationToken));
            var root = cfgDoc.RootElement;
            if (!root.TryGetProperty("repository", out var repo)) return Fail(currentVersion, "GitHub repository configuration is missing.");
            var owner = repo.TryGetProperty("owner", out var ownerEl) ? ownerEl.GetString()?.Trim() ?? "" : "";
            var name = repo.TryGetProperty("name", out var nameEl) ? nameEl.GetString()?.Trim() ?? "" : "";
            if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(name))
                return new GitHubUpdateResult { Configured = false, CurrentVersion = currentVersion, Message = "GitHub repository is not configured." };

            var channel = currentVersion.Contains("-beta", StringComparison.OrdinalIgnoreCase) ? "beta" : "stable";
            var prerelease = channel.Equals("beta", StringComparison.OrdinalIgnoreCase);
            var assetPattern = "VRCL_Client_{version}.zip";
            if (root.TryGetProperty("channels", out var channels) && channels.TryGetProperty(channel, out var channelEl))
            {
                if (channelEl.TryGetProperty("prerelease", out var preEl)) prerelease = preEl.GetBoolean();
                if (channelEl.TryGetProperty("asset_pattern", out var patternEl)) assetPattern = patternEl.GetString() ?? assetPattern;
            }

            var url = $"{ApiBase}/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(name)}/releases?per_page=50";
            using var response = await Client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return Fail(currentVersion, $"GitHub returned {(int)response.StatusCode} ({response.ReasonPhrase}).");

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            JsonElement? selected = null;
            foreach (var release in doc.RootElement.EnumerateArray())
            {
                if (!release.TryGetProperty("draft", out var draft) || draft.GetBoolean()) continue;
                if (!release.TryGetProperty("prerelease", out var pre) || pre.GetBoolean() != prerelease) continue;
                var tag = GetTag(release);
                if (!IsSupportedVersion(tag)) continue;
                if (selected is null || CompareVersions(GetTag(selected.Value), tag) < 0) selected = release;
            }

            if (selected is null)
                return Fail(currentVersion, prerelease ? "No published beta release was found." : "No published stable release was found.");

            var latest = GetTag(selected.Value);
            var expectedAsset = assetPattern.Replace("{version}", latest, StringComparison.OrdinalIgnoreCase);
            var download = "";
            var digest = "";
            if (selected.Value.TryGetProperty("assets", out var assets))
            {
                foreach (var asset in assets.EnumerateArray())
                {
                    var assetName = asset.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "";
                    if (!string.Equals(assetName, expectedAsset, StringComparison.OrdinalIgnoreCase)) continue;
                    download = asset.TryGetProperty("browser_download_url", out var d) ? d.GetString() ?? "" : "";
                    digest = asset.TryGetProperty("digest", out var dg) ? dg.GetString() ?? "" : "";
                    break;
                }
            }

            var newer = CompareVersions(currentVersion, latest) < 0;
            var packageAvailable = !string.IsNullOrWhiteSpace(download);
            var message = newer
                ? packageAvailable ? $"Update available: {latest}" : $"Release {latest} is available, but its update package is missing."
                : "VRCL Client is up to date.";

            return new GitHubUpdateResult
            {
                Success = true,
                Configured = true,
                UpdateAvailable = newer,
                PackageAvailable = packageAvailable,
                CurrentVersion = currentVersion,
                LatestVersion = latest,
                ReleaseName = selected.Value.TryGetProperty("name", out var rn) ? rn.GetString() ?? latest : latest,
                ReleaseUrl = selected.Value.TryGetProperty("html_url", out var hu) ? hu.GetString() ?? "" : "",
                DownloadUrl = download,
                AssetName = expectedAsset,
                AssetDigest = digest,
                Message = message
            };
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            return Fail(currentVersion, "Update check was cancelled.");
        }
        catch (Exception ex)
        {
            return Fail(currentVersion, $"Update check failed: {ex.Message}");
        }
    }

    static GitHubUpdateResult Fail(string current, string message) => new() { CurrentVersion = current, Message = message };

    static string GetTag(JsonElement release) =>
        (release.TryGetProperty("tag_name", out var t) ? t.GetString() : "")?.TrimStart('v', 'V') ?? "0.0.0";

    static bool IsSupportedVersion(string v) =>
        Regex.IsMatch(v ?? "", @"^\d+\.\d+\.\d+(?:-beta(?:\.\d+)?)?$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    internal static int CompareVersions(string a, string b)
    {
        static (int Major, int Minor, int Patch, bool Beta, int BetaNum) Parse(string v)
        {
            var m = Regex.Match(v ?? "", @"^(\d+)\.(\d+)\.(\d+)(?:-beta(?:\.(\d+))?)?$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            if (!m.Success) return (0, 0, 0, false, 0);
            return (
                int.Parse(m.Groups[1].Value),
                int.Parse(m.Groups[2].Value),
                int.Parse(m.Groups[3].Value),
                m.Groups[4].Success,
                string.IsNullOrEmpty(m.Groups[4].Value) ? 0 : int.Parse(m.Groups[4].Value));
        }

        var x = Parse(a);
        var y = Parse(b);
        var c = x.Major.CompareTo(y.Major); if (c != 0) return c;
        c = x.Minor.CompareTo(y.Minor); if (c != 0) return c;
        c = x.Patch.CompareTo(y.Patch); if (c != 0) return c;
        if (x.Beta != y.Beta) return x.Beta ? -1 : 1;
        return x.BetaNum.CompareTo(y.BetaNum);
    }
}