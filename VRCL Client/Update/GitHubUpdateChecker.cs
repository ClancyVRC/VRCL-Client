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
        return c;
    }

    public static async Task<GitHubUpdateResult> CheckAsync(string currentVersion, CancellationToken cancellationToken = default)
    {
        try
        {
            var configPath = VrclPaths.AppFile(Path.Combine("Update", "github_release_config.template.json"));
            if (!File.Exists(configPath)) return Fail(currentVersion, "Update configuration was not found.");
            using var cfgDoc = JsonDocument.Parse(await File.ReadAllTextAsync(configPath, cancellationToken));
            var repo = cfgDoc.RootElement.GetProperty("repository");
            var owner = repo.GetProperty("owner").GetString()?.Trim() ?? "";
            var name = repo.GetProperty("name").GetString()?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(owner) || string.IsNullOrWhiteSpace(name))
                return new GitHubUpdateResult { Configured=false, CurrentVersion=currentVersion, Message="GitHub repository is not configured yet." };

            var url = $"https://api.github.com/repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(name)}/releases?per_page=30";
            using var response = await Client.GetAsync(url, cancellationToken);
            if (!response.IsSuccessStatusCode)
                return Fail(currentVersion, $"GitHub returned {(int)response.StatusCode} ({response.ReasonPhrase}).");

            using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
            JsonElement? selected = null;
            foreach (var release in doc.RootElement.EnumerateArray())
            {
                if (!release.TryGetProperty("draft", out var draft) || draft.GetBoolean() ||
                    !release.TryGetProperty("prerelease", out var pre) || !pre.GetBoolean()) continue;

                if (selected is null) selected = release;
                else if (CompareVersions(GetTag(selected.Value), GetTag(release)) < 0) selected = release;
            }

            if (selected is null) return Fail(currentVersion, "No published beta release was found.");

            var latest = GetTag(selected.Value);
            var download = "";
            if (selected.Value.TryGetProperty("assets", out var assets))
            {
                foreach (var asset in assets.EnumerateArray())
                {
                    var n = asset.GetProperty("name").GetString() ?? "";
                    if (string.Equals(n, $"VRCL_Client_{latest}.zip", StringComparison.OrdinalIgnoreCase))
                    {
                        download = asset.GetProperty("browser_download_url").GetString() ?? "";
                        break;
                    }
                }
            }

            var newer = CompareVersions(currentVersion, latest) < 0;
            return new GitHubUpdateResult
            {
                Success=true, Configured=true, UpdateAvailable=newer,
                CurrentVersion=currentVersion, LatestVersion=latest,
                ReleaseName=selected.Value.TryGetProperty("name",out var rn)?rn.GetString()??latest:latest,
                ReleaseUrl=selected.Value.TryGetProperty("html_url",out var hu)?hu.GetString()??"":"",
                DownloadUrl=download,
                Message=newer ? $"Update available: {latest}" : "VRCL Client is up to date."
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

    static GitHubUpdateResult Fail(string current, string message) =>
        new() { CurrentVersion=current, Message=message };

    static string GetTag(JsonElement release) =>
        (release.TryGetProperty("tag_name",out var t)?t.GetString():"")?.TrimStart('v','V') ?? "0.0.0";

    static int CompareVersions(string a,string b)
    {
        static (int Major,int Minor,int Patch,bool Beta,int BetaNum) Parse(string v)
        {
            var m=Regex.Match(v??"", @"^(\d+)\.(\d+)\.(\d+)(?:-beta(?:\.(\d+))?)?$", RegexOptions.IgnoreCase);
            if(!m.Success)return(0,0,0,false,0);
            return(int.Parse(m.Groups[1].Value),int.Parse(m.Groups[2].Value),int.Parse(m.Groups[3].Value),
                m.Groups[4].Success,string.IsNullOrEmpty(m.Groups[4].Value)?0:int.Parse(m.Groups[4].Value));
        }
        var x=Parse(a);var y=Parse(b);
        var c=x.Major.CompareTo(y.Major);if(c!=0)return c;
        c=x.Minor.CompareTo(y.Minor);if(c!=0)return c;
        c=x.Patch.CompareTo(y.Patch);if(c!=0)return c;
        if(x.Beta!=y.Beta)return x.Beta?-1:1;
        return x.BetaNum.CompareTo(y.BetaNum);
    }
}