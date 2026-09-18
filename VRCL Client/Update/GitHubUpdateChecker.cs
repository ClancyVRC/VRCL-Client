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
    public string CurrentVersion { get; init; } = "";
    public string LatestVersion { get; init; } = "";
    public string ReleaseName { get; init; } = "";
    public string ReleaseUrl { get; init; } = "";
    public string DownloadUrl { get; init; } = "";
    public string Sha256 { get; init; } = "";
    public string Message { get; init; } = "";
}

internal static class GitHubUpdateChecker
{
    static readonly HttpClient Client = CreateClient();

    static HttpClient CreateClient()
    {
        var c = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
        c.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VRCL-Client", "1.0"));
        c.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        c.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        return c;
    }

    public static async Task<GitHubUpdateResult> CheckAsync(string currentVersion, CancellationToken cancellationToken = default)
    {
        try
        {
            var configPath = VrclPaths.AppFile(Path.Combine("Update", "github_release_config.template.json"));
            if (!File.Exists(configPath))
                return Fail(currentVersion, "Update configuration was not found.");

            using var cfgDoc = JsonDocument.Parse(await File.ReadAllTextAsync(configPath, cancellationToken));
            var repo = cfgDoc.RootElement.GetProperty("repository");
            var owner = repo.GetProperty("owner").GetString()?.Trim() ?? "";
            var name = repo.GetProperty("name").GetString()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(name))
                return new GitHubUpdateResult { Configured = false, CurrentVersion = currentVersion, Message = "GitHub repository is not configured yet." };

            var url = $"https://api.github.com/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(name)}/releases?per_page=100";
            using var response = await Client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return Fail(currentVersion, $"GitHub returned {(int)response.StatusCode} ({response.ReasonPhrase}).");

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            JsonElement? selected = null;

            foreach (var release in doc.RootElement.EnumerateArray())
            {
                if (!release.TryGetProperty("draft", out var draft) || draft.GetBoolean())
                    continue;
                if (!release.TryGetProperty("prerelease", out var pre) || !pre.GetBoolean())
                    continue;

                var tag = GetTag(release);
                if (!IsSupportedVersion(tag))
                    continue;

                var assetName = $"VRCL_Client_{tag}.zip";
                if (!HasAsset(release, assetName))
                    continue;

                if (selected is null || CompareVersions(GetTag(selected.Value), tag) < 0)
                    selected = release;
            }

            if (selected is null)
                return Fail(currentVersion, "No published beta release with a matching VRCL client ZIP was found.");

            var latest = GetTag(selected.Value);
            var expectedAsset = $"VRCL_Client_{latest}.zip";
            var download = "";
            var sha256 = "";

            if (selected.Value.TryGetProperty("assets", out var assets))
            {
                foreach (var asset in assets.EnumerateArray())
                {
                    var n = asset.GetProperty("name").GetString() ?? "";
                    if (!string.Equals(n, expectedAsset, StringComparison.OrdinalIgnoreCase))
                        continue;

                    download = asset.GetProperty("browser_download_url").GetString() ?? "";
                    if (asset.TryGetProperty("digest", out var digestElement))
                    {
                        var digest = digestElement.GetString() ?? "";
                        if (digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
                            digest = digest["sha256:".Length..];
                        sha256 = digest.Trim();
                    }
                    break;
                }
            }

            var newer = CompareVersions(currentVersion, latest) < 0;
            return new GitHubUpdateResult
            {
                Success = true,
                Configured = true,
                UpdateAvailable = newer,
                CurrentVersion = currentVersion,
                LatestVersion = latest,
                ReleaseName = selected.Value.TryGetProperty("name", out var rn) ? rn.GetString() ?? latest : latest,
                ReleaseUrl = selected.Value.TryGetProperty("html_url", out var hu) ? hu.GetString() ?? "" : "",
                DownloadUrl = download,
                Sha256 = sha256,
                Message = newer ? $"Update available: {latest}" : "VRCL Client is up to date."
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

    static bool HasAsset(JsonElement release, string expectedName)
    {
        if (!release.TryGetProperty("assets", out var assets))
            return false;

        foreach (var asset in assets.EnumerateArray())
        {
            if (string.Equals(asset.GetProperty("name").GetString(), expectedName, StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    static GitHubUpdateResult Fail(string current, string message) =>
        new() { CurrentVersion = current, Message = message };

    static string GetTag(JsonElement release) =>
        (release.TryGetProperty("tag_name", out var t) ? t.GetString() : "")?.TrimStart('v', 'V') ?? "0.0.0";

    static bool IsSupportedVersion(string version) =>
        Regex.IsMatch(version ?? "", @"^\d+\.\d+\.\d+(?:-beta(?:\.\d+)?|-release-beta(?:\.\d+)?)?$", RegexOptions.IgnoreCase);

    static int CompareVersions(string a, string b)
    {
        static (int Major, int Minor, int Patch, int PreRank, int PreNum) Parse(string v)
        {
            var m = Regex.Match(v ?? "", @"^(\d+)\.(\d+)\.(\d+)(?:-(beta|release-beta)(?:\.(\d+))?)?$", RegexOptions.IgnoreCase);
            if (!m.Success)
                return (0, 0, 0, 0, 0);

            var pre = m.Groups[4].Value.ToLowerInvariant();
            var rank = pre switch
            {
                "beta" => 1,
                "release-beta" => 1,
                _ => 0
            };

            var num = string.IsNullOrEmpty(m.Groups[5].Value) ? 0 : int.Parse(m.Groups[5].Value);
            return (int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value), int.Parse(m.Groups[3].Value), rank, num);
        }

        var x = Parse(a);
        var y = Parse(b);

        var c = x.Major.CompareTo(y.Major); if (c != 0) return c;
        c = x.Minor.CompareTo(y.Minor); if (c != 0) return c;
        c = x.Patch.CompareTo(y.Patch); if (c != 0) return c;
        c = x.PreRank.CompareTo(y.PreRank); if (c != 0) return c;
        return x.PreNum.CompareTo(y.PreNum);
    }
}
