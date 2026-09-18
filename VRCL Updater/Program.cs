using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace VRCLUpdater;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new UpdaterForm(args));
    }
}

internal sealed class UpdaterForm : Form
{
    readonly Label status = new();
    readonly ProgressBar progress = new();
    readonly Label detail = new();
    readonly string[] args;
    readonly CancellationTokenSource cts = new();

    public UpdaterForm(string[] args)
    {
        this.args = args;
        Text = "VRCL Updater";
        Width = 560;
        Height = 190;
        MinimumSize = new Size(560, 190);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = true;

        try
        {
            var icon = Path.Combine(AppContext.BaseDirectory, "vrcl_shell.ico");
            if (File.Exists(icon)) Icon = new Icon(icon);
        }
        catch { }

        status.Text = "Preparing VRCL update…";
        status.Font = new Font("Segoe UI", 11, FontStyle.Bold);
        status.AutoSize = false;
        status.SetBounds(22, 20, 500, 28);

        detail.AutoSize = false;
        detail.SetBounds(22, 52, 500, 42);

        progress.Minimum = 0;
        progress.Maximum = 100;
        progress.Value = 0;
        progress.SetBounds(22, 105, 500, 24);

        Controls.Add(status);
        Controls.Add(detail);
        Controls.Add(progress);
        Shown += async (_, _) => await RunAsync();
    }

    async Task RunAsync()
    {
        try
        {
            var opt = ParseArgs(args);
            if (string.IsNullOrWhiteSpace(opt.Url) || string.IsNullOrWhiteSpace(opt.TargetDirectory))
                throw new InvalidOperationException("The updater was started without a valid update package or target directory.");
            if (!Uri.TryCreate(opt.Url, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
                throw new InvalidOperationException("Update downloads must use HTTPS.");
            if (!IsAllowedGitHubUrl(uri))
                throw new InvalidOperationException("The update download is not a trusted GitHub release URL.");

            var target = Path.GetFullPath(opt.TargetDirectory);
            if (!Directory.Exists(target) || !File.Exists(Path.Combine(target, "VRCL Client.exe")))
                throw new InvalidOperationException("The selected VRCL Client installation could not be verified.");

            if (opt.ProcessId > 0)
                await WaitForProcessExitAsync(opt.ProcessId, TimeSpan.FromSeconds(60));

            var work = Path.Combine(Path.GetTempPath(), "VRCL-Updater", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(work);
            try
            {
                var zip = Path.Combine(work, "update.zip");
                var extract = Path.Combine(work, "extract");
                SetStatus("Downloading update…", opt.Version);
                await DownloadAsync(uri, zip, cts.Token);
                if (!string.IsNullOrWhiteSpace(opt.Sha256))
                {
                    SetStatus("Verifying update package…", "Checking release checksum");
                    var actual = await ComputeSha256Async(zip, cts.Token);
                    if (!string.Equals(actual, opt.Sha256, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("The downloaded update failed its SHA-256 checksum.");
                }
                else
                {
                    SetStatus("Verifying update package…", "Checking ZIP structure");
                }

                ZipFile.ExtractToDirectory(zip, extract, overwriteFiles: false);
                var payload = FindPayloadRoot(extract, opt.Version);
                ValidatePayload(payload);
                var deleteList = ReadDeleteManifest(payload);

                SetStatus("Installing update…", "Preserving your Data folder");
                progress.Style = ProgressBarStyle.Marquee;
                var selfUpdate = await Task.Run(() => ApplyPayload(payload, target, deleteList), cts.Token);
                progress.Style = ProgressBarStyle.Continuous;
                progress.Value = 100;

                if (selfUpdate is not null)
                {
                    await ScheduleSelfUpdate(selfUpdate, target);
                    return;
                }

                SetStatus("Update complete.", string.IsNullOrWhiteSpace(opt.Version) ? "Starting VRCL Client…" : $"VRCL Client {opt.Version} is ready.");
                await Task.Delay(500);
                StartClient(target);
            }
            finally
            {
                TryDeleteDirectory(work);
            }
            Close();
        }
        catch (Exception ex)
        {
            progress.Style = ProgressBarStyle.Continuous;
            progress.Value = 0;
            SetStatus("Update failed.", ex.Message);
            MessageBox.Show(this, "VRCL Client could not be updated.\n\n" + ex.Message, "VRCL Updater", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Close();
        }
    }

    async Task DownloadAsync(Uri uri, string destination, CancellationToken token)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(5) };
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("VRCL-Updater", "1.0"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
        using var response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, token);
        response.EnsureSuccessStatusCode();
        await using var input = await response.Content.ReadAsStreamAsync(token);
        await using var output = new FileStream(destination, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1024 * 64, true);
        var total = response.Content.Headers.ContentLength ?? -1;
        var read = 0L;
        var buffer = new byte[1024 * 64];
        while (true)
        {
            var n = await input.ReadAsync(buffer, token);
            if (n == 0) break;
            await output.WriteAsync(buffer.AsMemory(0, n), token);
            read += n;
            if (total > 0) progress.Value = Math.Clamp((int)(read * 100 / total), 0, 100);
        }
    }

    static string FindPayloadRoot(string extract, string version)
    {
        var candidates = new List<string>();
        if (!string.IsNullOrWhiteSpace(version))
            candidates.Add(Path.Combine(extract, $"VRCL_Client_{version}", "VRCL Client"));
        candidates.Add(Path.Combine(extract, "VRCL Client"));
        candidates.AddRange(Directory.GetDirectories(extract, "VRCL Client", SearchOption.AllDirectories));
        foreach (var c in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
            if (Directory.Exists(c) && File.Exists(Path.Combine(c, "VRCL Client.exe"))) return c;
        throw new InvalidDataException("The update package does not contain a valid VRCL Client payload.");
    }

    static void ValidatePayload(string payload)
    {
        foreach (var file in Directory.EnumerateFiles(payload, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(payload, file);
            if (relative.Equals("Data", StringComparison.OrdinalIgnoreCase) ||
                relative.StartsWith("Data" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The release package attempted to include protected Data files.");
        }
        if (!File.Exists(Path.Combine(payload, "VRCL Client.exe")))
            throw new InvalidDataException("VRCL Client.exe is missing from the update payload.");
        if (!File.Exists(Path.Combine(payload, "VRCL Updater.exe")))
            throw new InvalidDataException("VRCL Updater.exe is missing from the update payload.");
    }

    static List<string> ReadDeleteManifest(string payload)
    {
        foreach (var manifest in new[]
        {
            Path.Combine(payload, "update_manifest.json"),
            Path.Combine(payload, "Update", "update_manifest.json")
        })
        {
            if (!File.Exists(manifest)) continue;
            using var doc = JsonDocument.Parse(File.ReadAllText(manifest));
            if (!doc.RootElement.TryGetProperty("delete", out var items) || items.ValueKind != JsonValueKind.Array)
                return new List<string>();
            return items.EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.String)
                .Select(x => x.GetString() ?? "")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();
        }
        return new List<string>();
    }

    static string? ApplyPayload(string payload, string target, IReadOnlyList<string> deleteList)
    {
        string? stagedUpdater = null;
        foreach (var dir in Directory.EnumerateDirectories(payload, "*", SearchOption.AllDirectories))
        {
            var rel = Path.GetRelativePath(payload, dir);
            if (rel.Equals("Data", StringComparison.OrdinalIgnoreCase) ||
                rel.StartsWith("Data" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) continue;
            Directory.CreateDirectory(Path.Combine(target, rel));
        }

        foreach (var file in Directory.EnumerateFiles(payload, "*", SearchOption.AllDirectories))
        {
            var rel = Path.GetRelativePath(payload, file);
            if (rel.Equals("Data", StringComparison.OrdinalIgnoreCase) ||
                rel.StartsWith("Data" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) continue;

            var destination = Path.Combine(target, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);

            if (string.Equals(Path.GetFileName(destination), "VRCL Updater.exe", StringComparison.OrdinalIgnoreCase))
            {
                stagedUpdater = Path.Combine(Path.GetTempPath(), "VRCL-Updater", Guid.NewGuid().ToString("N") + ".exe");
                Directory.CreateDirectory(Path.GetDirectoryName(stagedUpdater)!);
                File.Copy(file, stagedUpdater, overwrite: true);
                continue;
            }

            File.SetAttributes(destination, FileAttributes.Normal);
            File.Copy(file, destination, overwrite: true);
        }

        foreach (var relative in deleteList)
        {
            var normalized = relative.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar).TrimStart(Path.DirectorySeparatorChar);
            if (normalized.Equals("Data", StringComparison.OrdinalIgnoreCase) ||
                normalized.StartsWith("Data" + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The update manifest attempted to delete protected Data files.");

            var destination = Path.GetFullPath(Path.Combine(target, normalized));
            var targetRoot = Path.GetFullPath(target).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            if (!destination.StartsWith(targetRoot, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The update manifest contains a path outside the VRCL installation.");

            if (File.Exists(destination))
            {
                File.SetAttributes(destination, FileAttributes.Normal);
                File.Delete(destination);
            }
            else if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }
        }

        return stagedUpdater;
    }

    async Task ScheduleSelfUpdate(string stagedUpdater, string target)
    {
        var updater = Path.Combine(target, "VRCL Updater.exe");
        var client = Path.Combine(target, "VRCL Client.exe");
        var pid = Environment.ProcessId;
        var script = Path.Combine(Path.GetTempPath(), $"VRCL_Updater_SelfUpdate_{pid}.ps1");
        var qScript = script.Replace("'", "''");
        var qUpdater = updater.Replace("'", "''");
        var qStaged = stagedUpdater.Replace("'", "''");
        var qClient = client.Replace("'", "''");
        var qTarget = target.Replace("'", "''");
        var body = $"$pid={pid}; $deadline=(Get-Date).AddSeconds(60); while(Get-Process -Id $pid -ErrorAction SilentlyContinue -and (Get-Date) -lt $deadline) {{ Start-Sleep -Milliseconds 250 }}; Move-Item -LiteralPath '{qStaged}' -Destination '{qUpdater}' -Force; Start-Process -FilePath '{qClient}' -WorkingDirectory '{qTarget}'; Remove-Item -LiteralPath '{qScript}' -Force -ErrorAction SilentlyContinue";

        File.WriteAllText(script, body, Encoding.UTF8);
        SetStatus("Finishing update…", "Refreshing the bundled updater and restarting VRCL Client.");

        var psi = new ProcessStartInfo(
            "powershell.exe",
            $"-NoProfile -NonInteractive -WindowStyle Hidden -ExecutionPolicy Bypass -File \"{script}\"")
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            WorkingDirectory = target
        };

        Process.Start(psi);
        await Task.Delay(250);
        Close();
    }

    static async Task<string> ComputeSha256Async(string file, CancellationToken token)
    {
        await using var stream = File.OpenRead(file);
        var hash = await SHA256.HashDataAsync(stream, token);
        return Convert.ToHexString(hash);
    }

    async Task WaitForProcessExitAsync(int pid, TimeSpan timeout)
    {
        try
        {
            using var process = Process.GetProcessById(pid);
            SetStatus("Waiting for VRCL Client to close…", "The update will continue automatically.");
            var sw = Stopwatch.StartNew();
            while (!process.HasExited)
            {
                if (sw.Elapsed > timeout)
                    throw new TimeoutException("VRCL Client did not close within the update timeout.");
                await Task.Delay(250);
            }
        }
        catch (ArgumentException) { }
    }

    static void StartClient(string target)
    {
        var exe = Path.Combine(target, "VRCL Client.exe");
        Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true, WorkingDirectory = target });
    }

    void SetStatus(string title, string extra)
    {
        if (InvokeRequired) { BeginInvoke(() => SetStatus(title, extra)); return; }
        status.Text = title;
        detail.Text = extra;
    }

    static bool IsAllowedGitHubUrl(Uri uri)
    {
        var host = uri.Host.TrimEnd('.');
        return host.Equals("github.com", StringComparison.OrdinalIgnoreCase) ||
               host.Equals("objects.githubusercontent.com", StringComparison.OrdinalIgnoreCase) ||
               host.EndsWith(".githubusercontent.com", StringComparison.OrdinalIgnoreCase);
    }

    static Options ParseArgs(string[] args)
    {
        var o = new Options();
        for (var i = 0; i < args.Length; i++)
        {
            var a = args[i];
            if (a.Equals("--url", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) o.Url = args[++i];
            else if (a.Equals("--version", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) o.Version = args[++i];
            else if (a.Equals("--target", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) o.TargetDirectory = args[++i];
            else if (a.Equals("--pid", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length && int.TryParse(args[++i], out var pid)) o.ProcessId = pid;
            else if (a.Equals("--sha256", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length) o.Sha256 = (args[++i] ?? "").Replace("sha256:", "", StringComparison.OrdinalIgnoreCase).Trim();
        }
        return o;
    }

    static void TryDeleteDirectory(string path)
    {
        try { if (Directory.Exists(path)) Directory.Delete(path, recursive: true); } catch { }
    }

    sealed class Options
    {
        public string Url { get; set; } = "";
        public string Version { get; set; } = "";
        public string TargetDirectory { get; set; } = "";
        public int ProcessId { get; set; }
        public string Sha256 { get; set; } = "";
    }
}