using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Drawing;
using System.Windows.Forms;

namespace VRCLInstaller;

internal static class Program
{
    private const string ReleaseApi = "https://api.github.com/repos/ClancyVRC/VRCL-Client/releases?per_page=100";

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new InstallerForm());
    }

    private sealed record ReleaseInfo(string Version, string Tag, string Name, string AssetUrl, string AssetName, string Sha256);

    private sealed class InstallerForm : Form
    {
        private readonly Label latestValue, statusValue;
        private readonly ProgressBar progress;
        private readonly TextBox locationBox;
        private readonly Button installButton, retryButton;
        private ReleaseInfo? latest;

        public InstallerForm()
        {
            Text = "VRCL Client Installer";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(760, 620);
            ClientSize = new Size(820, 660);
            BackColor = Color.FromArgb(18, 20, 26);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(28), ColumnCount = 1, RowCount = 7 };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 95));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var header = new Panel { Dock = DockStyle.Fill };
            var title = new Label { Text = "VRCL Client", Font = new Font("Segoe UI Semibold", 25F), AutoSize = true, Location = new Point(0, 2) };
            header.Controls.Add(title);
            header.Controls.Add(new Label {
                Text = "Installer • always installs the newest published GitHub release",
                Font = new Font("Segoe UI", 10F), ForeColor = Color.FromArgb(175, 182, 196),
                AutoSize = true, Location = new Point(2, 48)
            });
            root.Controls.Add(header, 0, 0);

            var releaseCard = MakeCard();
            releaseCard.Controls.Add(MakeSectionTitle("LATEST RELEASE"));
            latestValue = new Label { Text = "Checking GitHub…", AutoSize = true, Font = new Font("Segoe UI Semibold", 13F), Location = new Point(18, 38) };
            releaseCard.Controls.Add(latestValue);
            root.Controls.Add(releaseCard, 0, 1);

            var readme = MakeCard();
            readme.Controls.Add(MakeSectionTitle("READ ME"));
            readme.Controls.Add(new Label {
                AutoSize = false, Location = new Point(18, 38), Size = new Size(720, 100),
                ForeColor = Color.FromArgb(205, 211, 222),
                Text = "VRCL Installer checks GitHub every time it starts and installs the newest published VRCL Client package it can find.\r\n\r\n" +
                       "The selected location is the parent folder. The installer creates a VRCL Client folder inside it. Existing Data/settings are preserved.\r\n\r\n" +
                       "Repository: github.com/ClancyVRC/VRCL-Client"
            });
            root.Controls.Add(readme, 0, 2);

            var location = MakeCard();
            location.Controls.Add(MakeSectionTitle("INSTALL LOCATION"));
            locationBox = new TextBox {
                Location = new Point(18, 40), Width = 610, Height = 32,
                Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "VRCL Client - VRC Client v1.0.0 - (main files)"),
                BackColor = Color.FromArgb(28, 31, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle
            };
            location.Controls.Add(locationBox);
            var browse = MakeButton("Browse", 638, 38, 92, 34);
            browse.Click += (_, _) => Browse();
            location.Controls.Add(browse);
            root.Controls.Add(location, 0, 3);

            var actions = new Panel { Dock = DockStyle.Fill };
            installButton = MakeButton("Install Latest VRCL Client", 0, 8, 230, 42);
            installButton.BackColor = Color.FromArgb(54, 108, 210);
            installButton.Click += async (_, _) => await InstallAsync();
            actions.Controls.Add(installButton);
            retryButton = MakeButton("Retry GitHub Check", 244, 8, 170, 42);
            retryButton.Click += async (_, _) => await CheckLatestAsync();
            actions.Controls.Add(retryButton);
            root.Controls.Add(actions, 0, 4);

            progress = new ProgressBar { Location = new Point(0, 6), Size = new Size(730, 12) };
            root.Controls.Add(progress, 0, 5);
            statusValue = new Label { Text = "Starting…", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(170, 178, 192), Padding = new Padding(2, 8, 0, 0) };
            root.Controls.Add(statusValue, 0, 6);

            Shown += async (_, _) => await CheckLatestAsync();
        }

        private Panel MakeCard() => new() { Dock = DockStyle.Fill, BackColor = Color.FromArgb(24, 27, 35), Padding = new Padding(18) };
        private Label MakeSectionTitle(string text) => new() { Text = text, AutoSize = true, Font = new Font("Segoe UI Semibold", 9F), ForeColor = Color.FromArgb(145, 157, 180), Location = new Point(18, 10) };
        private Button MakeButton(string text, int x, int y, int w, int h) => new() {
            Text = text, Location = new Point(x, y), Size = new Size(w, h), FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(38, 43, 54), ForeColor = Color.White
        };

        private void Browse()
        {
            using var dialog = new FolderBrowserDialog { Description = "Choose the parent folder where VRCL Client should be installed." };
            if (Directory.Exists(locationBox.Text)) dialog.SelectedPath = Directory.GetParent(locationBox.Text)?.FullName ?? locationBox.Text;
            if (dialog.ShowDialog(this) == DialogResult.OK)
                locationBox.Text = Path.Combine(dialog.SelectedPath, "VRCL Client - VRC Client v1.0.0 - (main files)");
        }

        private async Task CheckLatestAsync()
        {
            SetBusy(true);
            try {
                statusValue.Text = "Checking GitHub for the newest compatible VRCL release…";
                latest = await FindLatestReleaseAsync();
                if (latest == null) {
                    latestValue.Text = "No compatible published release found.";
                    statusValue.Text = "No matching VRCL_Client_<version>.zip package was found.";
                    return;
                }
                latestValue.Text = $"{latest.Name} • {latest.AssetName}";
                statusValue.Text = $"Ready to install {latest.Version}.";
            } catch (Exception ex) {
                latest = null;
                latestValue.Text = "GitHub check failed.";
                statusValue.Text = $"Could not retrieve the latest release: {ex.Message}";
            } finally { SetBusy(false); }
        }

        private static async Task<ReleaseInfo?> FindLatestReleaseAsync()
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("VRCL-Installer/1.0");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
            using var response = await client.GetAsync(ReleaseApi);
            response.EnsureSuccessStatusCode();
            using var doc = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());

            ReleaseInfo? best = null;
            foreach (var release in doc.RootElement.EnumerateArray()) {
                if (release.GetProperty("draft").GetBoolean()) continue;
                var tag = release.GetProperty("tag_name").GetString() ?? "";
                if (!TryParseVersion(tag, out var version)) continue;

                foreach (var asset in release.GetProperty("assets").EnumerateArray()) {
                    var name = asset.GetProperty("name").GetString() ?? "";
                    if (!name.Equals($"VRCL_Client_{version}.zip", StringComparison.OrdinalIgnoreCase)) continue;
                    var digest = asset.TryGetProperty("digest", out var d) ? (d.GetString() ?? "") : "";
                    if (digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase)) digest = digest[7..];

                    var candidate = new ReleaseInfo(version, tag, release.GetProperty("name").GetString() ?? tag,
                        asset.GetProperty("browser_download_url").GetString() ?? "", name, digest);
                    if (best == null || CompareVersions(version, best.Version) > 0) best = candidate;
                }
            }
            return best;
        }

        private async Task InstallAsync()
        {
            if (latest == null) { await CheckLatestAsync(); if (latest == null) return; }
            SetBusy(true);
            try {
                var parent = locationBox.Text.Trim();
                if (string.IsNullOrWhiteSpace(parent)) throw new InvalidOperationException("Choose an installation location.");

                // FIX: the selected path is the parent; the actual application root is always <parent>\VRCL Client.
                var installRoot = Path.Combine(parent, "VRCL Client");
                Directory.CreateDirectory(installRoot);

                var tempZip = Path.Combine(Path.GetTempPath(), $"VRCL_Client_{Guid.NewGuid():N}.zip");
                var extractRoot = Path.Combine(Path.GetTempPath(), $"vrcl_extract_{Guid.NewGuid():N}");
                try {
                    statusValue.Text = $"Downloading {latest.AssetName}…";
                    await DownloadAsync(latest.AssetUrl, tempZip);

                    if (!string.IsNullOrWhiteSpace(latest.Sha256)) {
                        statusValue.Text = "Verifying SHA-256…";
                        await using var stream = File.OpenRead(tempZip);
                        using var sha = SHA256.Create();
                        var actual = Convert.ToHexString(await sha.ComputeHashAsync(stream)).ToLowerInvariant();
                        if (!actual.Equals(latest.Sha256, StringComparison.OrdinalIgnoreCase))
                            throw new InvalidDataException("The downloaded package failed SHA-256 verification.");
                    }

                    statusValue.Text = "Extracting VRCL Client…";
                    Directory.CreateDirectory(extractRoot);
                    ZipFile.ExtractToDirectory(tempZip, extractRoot, true);

                    var payload = FindPayload(extractRoot);
                    if (payload == null) throw new InvalidDataException("The release package does not contain a valid VRCL Client folder.");

                    statusValue.Text = "Installing application files…";
                    CopyDirectory(payload, installRoot);

                    statusValue.Text = "Applying installed-folder layout…";
                    ApplyVisibleHiddenLayout(installRoot);

                    var exe = Path.Combine(installRoot, "VRCL Client.exe");
                    if (!File.Exists(exe)) throw new InvalidDataException("VRCL Client.exe was not found after installation.");

                    progress.Value = 100;
                    statusValue.Text = "Installation complete.";
                    if (MessageBox.Show(this, "VRCL Client was installed successfully.\r\n\r\nLaunch it now?", "VRCL Client",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                        Process.Start(new ProcessStartInfo(exe) { UseShellExecute = true });
                }
                finally {
                    TryDeleteDirectory(extractRoot);
                    TryDeleteFile(tempZip);
                }
            } catch (Exception ex) {
                statusValue.Text = $"Installation failed: {ex.Message}";
                MessageBox.Show(this, ex.Message, "VRCL Installer", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally { SetBusy(false); }
        }

        private static string? FindPayload(string root)
        {
            var direct = Path.Combine(root, "VRCL Client");
            if (File.Exists(Path.Combine(direct, "VRCL Client.exe"))) return direct;
            return Directory.GetDirectories(root, "VRCL Client", SearchOption.AllDirectories)
                .FirstOrDefault(p => File.Exists(Path.Combine(p, "VRCL Client.exe")));
        }

        private static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(Path.Combine(destination, Path.GetRelativePath(source, dir)));

            foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories)) {
                var target = Path.Combine(destination, Path.GetRelativePath(source, file));
                Directory.CreateDirectory(Path.GetDirectoryName(target)!);
                File.Copy(file, target, true);
            }
        }

        private static void ApplyVisibleHiddenLayout(string root)
        {
            foreach (var file in Directory.GetFiles(root, "*", SearchOption.AllDirectories)) {
                var name = Path.GetFileName(file);
                var attrs = File.GetAttributes(file);
                if (name.Equals("VRCL Client.exe", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("(READ ME).txt", StringComparison.OrdinalIgnoreCase))
                    File.SetAttributes(file, attrs & ~FileAttributes.Hidden);
                else
                    File.SetAttributes(file, attrs | FileAttributes.Hidden);
            }

            foreach (var dir in Directory.GetDirectories(root, "*", SearchOption.AllDirectories))
                File.SetAttributes(dir, File.GetAttributes(dir) | FileAttributes.Hidden);
        }

        private async Task DownloadAsync(string url, string destination)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("VRCL-Installer/1.0");
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            response.EnsureSuccessStatusCode();
            var total = response.Content.Headers.ContentLength ?? -1;
            await using var input = await response.Content.ReadAsStreamAsync();
            await using var output = File.Create(destination);
            var buffer = new byte[128 * 1024];
            long done = 0; int read;
            while ((read = await input.ReadAsync(buffer)) > 0) {
                await output.WriteAsync(buffer.AsMemory(0, read));
                done += read;
                if (total > 0) progress.Value = (int)Math.Clamp(done * 100 / total, 0, 100);
            }
        }

        private void SetBusy(bool busy) { installButton.Enabled = !busy; retryButton.Enabled = !busy; }

        private static bool TryParseVersion(string tag, out string normalized)
        {
            normalized = tag.StartsWith("v", StringComparison.OrdinalIgnoreCase) ? tag[1..] : tag;
            return normalized.EndsWith("-release-beta", StringComparison.OrdinalIgnoreCase) ||
                   normalized.EndsWith("-beta", StringComparison.OrdinalIgnoreCase) ||
                   Version.TryParse(normalized, out _);
        }

        private static int CompareVersions(string a, string b)
        {
            static (Version v, int suffix) Parse(string s) {
                int suffix = 0;
                if (s.EndsWith("-release-beta", StringComparison.OrdinalIgnoreCase)) { suffix = 1; s = s[..^13]; }
                else if (s.EndsWith("-beta", StringComparison.OrdinalIgnoreCase)) s = s[..^5];
                return (Version.TryParse(s, out var v) ? v : new Version(0, 0), suffix);
            }
            var x = Parse(a); var y = Parse(b);
            var c = x.v.CompareTo(y.v);
            return c != 0 ? c : x.suffix.CompareTo(y.suffix);
        }

        private static void TryDeleteFile(string path) { try { if (File.Exists(path)) File.Delete(path); } catch { } }
        private static void TryDeleteDirectory(string path) { try { if (Directory.Exists(path)) Directory.Delete(path, true); } catch { } }
    }
}
