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

    private sealed record ReleaseInfo(
        string Version,
        string Tag,
        string Name,
        string AssetUrl,
        string AssetName,
        string Sha256,
        bool Prerelease)
    {
        public string Display =>
            $"{Version}{(Prerelease ? " • Beta" : "")} — {Name}";
    }

    private sealed class InstallerForm : Form
    {
        private readonly Label latestValue;
        private readonly Label selectedValue;
        private readonly Label statusValue;
        private readonly ProgressBar progress;
        private readonly TextBox locationBox;
        private readonly ComboBox versionBox;
        private readonly Button installSelectedButton;
        private readonly Button installLatestButton;
        private readonly Button refreshButton;

        private List<ReleaseInfo> releases = new();
        private ReleaseInfo? latest;

        public InstallerForm()
        {
            Text = "VRCL Client Installer";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(820, 700);
            ClientSize = new Size(860, 740);
            BackColor = Color.FromArgb(18, 20, 26);
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 10F);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            ShowInTaskbar = true;

            try
            {
                var processPath = Environment.ProcessPath;
                Icon = !string.IsNullOrWhiteSpace(processPath)
                    ? System.Drawing.Icon.ExtractAssociatedIcon(processPath) ?? SystemIcons.Application
                    : SystemIcons.Application;
            }
            catch
            {
                Icon = SystemIcons.Application;
            }

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(28),
                ColumnCount = 1,
                RowCount = 8
            };

            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 88));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 82));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 138));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 112));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 108));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var header = new Panel { Dock = DockStyle.Fill };

            var logo = new PictureBox
            {
                Location = new Point(0, 2),
                Size = new Size(72, 72),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            try
            {
                var processPath = Environment.ProcessPath;
                if (!string.IsNullOrWhiteSpace(processPath))
                    logo.Image = System.Drawing.Icon.ExtractAssociatedIcon(processPath)?.ToBitmap();
            }
            catch { }

            header.Controls.Add(logo);
            header.Controls.Add(new Label
            {
                Text = "VRCL Client",
                Font = new Font("Segoe UI Semibold", 25F),
                AutoSize = true,
                Location = new Point(88, 0)
            });
            header.Controls.Add(new Label
            {
                Text = "Installer • choose any published VRCL release",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(175, 182, 196),
                AutoSize = true,
                Location = new Point(90, 45)
            });
            root.Controls.Add(header, 0, 0);

            var latestCard = MakeCard();
            latestCard.Controls.Add(MakeSectionTitle("LATEST RELEASE"));
            latestValue = new Label
            {
                Text = "Checking GitHub…",
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 13F),
                Location = new Point(18, 38)
            };
            latestCard.Controls.Add(latestValue);
            root.Controls.Add(latestCard, 0, 1);

            var infoCard = MakeCard();
            infoCard.Controls.Add(new Label
            {
                AutoSize = false,
                Location = new Point(18, 18),
                Size = new Size(770, 100),
                ForeColor = Color.FromArgb(205, 211, 222),
                Text =
                    "Choose the newest release or select a specific published version.\r\n\r\n" +
                    "The installer downloads the selected GitHub release, verifies its SHA-256 digest when available, " +
                    "and preserves the existing Data folder when updating."
            });
            root.Controls.Add(infoCard, 0, 2);

            var versionCard = MakeCard();
            versionCard.Controls.Add(MakeSectionTitle("VERSION TO INSTALL"));

            versionBox = new ComboBox
            {
                Location = new Point(18, 40),
                Size = new Size(610, 34),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(28, 31, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            versionBox.SelectedIndexChanged += (_, _) => UpdateSelectedRelease();
            versionCard.Controls.Add(versionBox);

            refreshButton = MakeButton("Refresh", 642, 38, 108, 36);
            refreshButton.Click += async (_, _) => await CheckReleasesAsync();
            versionCard.Controls.Add(refreshButton);

            selectedValue = new Label
            {
                Text = "Selected: —",
                AutoSize = true,
                ForeColor = Color.FromArgb(165, 174, 190),
                Location = new Point(18, 80)
            };
            versionCard.Controls.Add(selectedValue);
            root.Controls.Add(versionCard, 0, 3);

            var locationCard = MakeCard();
            locationCard.Controls.Add(MakeSectionTitle("INSTALL LOCATION"));

            locationBox = new TextBox
            {
                Location = new Point(18, 40),
                Width = 640,
                Height = 32,
                Text = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                    "VRCL Client"),
                BackColor = Color.FromArgb(28, 31, 40),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            locationCard.Controls.Add(locationBox);

            var browse = MakeButton("Browse", 670, 38, 92, 34);
            browse.Click += (_, _) => Browse();
            locationCard.Controls.Add(browse);

            locationCard.Controls.Add(new Label
            {
                Text = "VRCL Client will be installed into this folder.",
                AutoSize = true,
                ForeColor = Color.FromArgb(145, 157, 180),
                Location = new Point(18, 78)
            });

            root.Controls.Add(locationCard, 0, 4);

            var actions = new Panel { Dock = DockStyle.Fill };

            installSelectedButton = MakeButton("Install Selected Version", 0, 8, 235, 42);
            installSelectedButton.BackColor = Color.FromArgb(54, 108, 210);
            installSelectedButton.Click += async (_, _) => await InstallSelectedAsync();
            actions.Controls.Add(installSelectedButton);

            installLatestButton = MakeButton("Install Latest Version", 250, 8, 205, 42);
            installLatestButton.BackColor = Color.FromArgb(38, 43, 54);
            installLatestButton.Click += async (_, _) => await InstallLatestAsync();
            actions.Controls.Add(installLatestButton);

            root.Controls.Add(actions, 0, 5);

            progress = new ProgressBar
            {
                Location = new Point(0, 6),
                Size = new Size(770, 12)
            };
            root.Controls.Add(progress, 0, 6);

            statusValue = new Label
            {
                Text = "Starting…",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(170, 178, 192),
                Padding = new Padding(2, 8, 0, 0)
            };
            root.Controls.Add(statusValue, 0, 7);

            Shown += async (_, _) => await CheckReleasesAsync();
        }

        private Panel MakeCard() => new()
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(24, 27, 35),
            Padding = new Padding(18)
        };

        private Label MakeSectionTitle(string text) => new()
        {
            Text = text,
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 9F),
            ForeColor = Color.FromArgb(145, 157, 180),
            Location = new Point(18, 10)
        };

        private Button MakeButton(string text, int x, int y, int w, int h) => new()
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(w, h),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(38, 43, 54),
            ForeColor = Color.White
        };

        private void Browse()
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Choose the parent folder where VRCL Client should be installed."
            };

            var current = locationBox.Text.Trim();
            if (Directory.Exists(current))
            {
                var parent = Directory.GetParent(current);
                if (parent != null && Directory.Exists(parent.FullName))
                    dialog.SelectedPath = parent.FullName;
            }

            if (dialog.ShowDialog(this) == DialogResult.OK)
                locationBox.Text = Path.Combine(dialog.SelectedPath, "VRCL Client");
        }

        private async Task CheckReleasesAsync()
        {
            SetBusy(true);
            try
            {
                statusValue.Text = "Checking GitHub for published VRCL Client releases…";
                progress.Value = 0;

                releases = await GetReleasesAsync();
                latest = releases.FirstOrDefault();

                versionBox.Items.Clear();

                foreach (var release in releases)
                    versionBox.Items.Add(release);

                versionBox.DisplayMember = nameof(ReleaseInfo.Display);

                if (latest == null)
                {
                    latestValue.Text = "No compatible VRCL Client releases found.";
                    selectedValue.Text = "Selected: —";
                    statusValue.Text = "No published VRCL Client ZIP packages were found.";
                    return;
                }

                latestValue.Text = $"{latest.Version} • {latest.Name}";
                versionBox.SelectedIndex = 0;
                statusValue.Text = $"{releases.Count} compatible release(s) found. Ready to install.";
            }
            catch (Exception ex)
            {
                releases = new();
                latest = null;
                versionBox.Items.Clear();
                latestValue.Text = "GitHub check failed.";
                selectedValue.Text = "Selected: —";
                statusValue.Text = $"Could not retrieve releases: {ex.Message}";
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void UpdateSelectedRelease()
        {
            if (versionBox.SelectedItem is ReleaseInfo release)
            {
                selectedValue.Text = $"Selected: {release.Version} • {release.AssetName}";
            }
            else
            {
                selectedValue.Text = "Selected: —";
            }
        }

        private async Task InstallSelectedAsync()
        {
            if (versionBox.SelectedItem is not ReleaseInfo selected)
            {
                MessageBox.Show(
                    this,
                    "Select a VRCL Client version first.",
                    "VRCL Installer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            await InstallAsync(selected);
        }

        private async Task InstallLatestAsync()
        {
            if (latest == null)
            {
                await CheckReleasesAsync();
                if (latest == null)
                    return;
            }

            await InstallAsync(latest);
        }

        private static async Task<List<ReleaseInfo>> GetReleasesAsync()
        {
            using var client = CreateHttpClient();

            using var response = await client.GetAsync(ReleaseApi);
            response.EnsureSuccessStatusCode();

            using var doc = System.Text.Json.JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var found = new List<ReleaseInfo>();

            foreach (var release in doc.RootElement.EnumerateArray())
            {
                if (release.GetProperty("draft").GetBoolean())
                    continue;

                var tag = release.GetProperty("tag_name").GetString() ?? "";
                if (!TryParseVersion(tag, out var version))
                    continue;

                var prerelease =
                    release.TryGetProperty("prerelease", out var pre) &&
                    pre.GetBoolean();

                var releaseName =
                    release.GetProperty("name").GetString() ?? tag;

                foreach (var asset in release.GetProperty("assets").EnumerateArray())
                {
                    var name = asset.GetProperty("name").GetString() ?? "";
                    if (!IsClientPackageAsset(name, version))
                        continue;

                    var url =
                        asset.GetProperty("browser_download_url").GetString() ?? "";

                    if (string.IsNullOrWhiteSpace(url))
                        continue;

                    var digest =
                        asset.TryGetProperty("digest", out var d)
                            ? d.GetString() ?? ""
                            : "";

                    if (digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
                        digest = digest[7..];

                    found.Add(new ReleaseInfo(
                        version,
                        tag,
                        releaseName,
                        url,
                        name,
                        digest,
                        prerelease));

                    break;
                }
            }

            return found
                .OrderByDescending(r => r, ReleaseInfoComparer.Instance)
                .ToList();
        }

        private static bool IsClientPackageAsset(string name, string version)
        {
            if (!name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                return false;

            var expected1 = $"VRCL_Client_{version}.zip";
            var expected2 = $"VRCL_Client_v{version}.zip";

            return name.Equals(expected1, StringComparison.OrdinalIgnoreCase) ||
                   name.Equals(expected2, StringComparison.OrdinalIgnoreCase);
        }

        private async Task InstallAsync(ReleaseInfo release)
        {
            SetBusy(true);
            progress.Value = 0;

            try
            {
                var installRoot = locationBox.Text.Trim();

                if (string.IsNullOrWhiteSpace(installRoot))
                    throw new InvalidOperationException("Choose an installation location.");

                if (Path.GetFileName(
                        installRoot.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                    .Equals("Data", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "VRCL Client cannot be installed directly into a Data folder.");
                }

                Directory.CreateDirectory(installRoot);

                var tempZip = Path.Combine(
                    Path.GetTempPath(),
                    $"VRCL_Client_{Guid.NewGuid():N}.zip");

                var extractRoot = Path.Combine(
                    Path.GetTempPath(),
                    $"vrcl_extract_{Guid.NewGuid():N}");

                try
                {
                    statusValue.Text = $"Downloading {release.AssetName}…";
                    await DownloadAsync(release.AssetUrl, tempZip);

                    if (!string.IsNullOrWhiteSpace(release.Sha256))
                    {
                        statusValue.Text = "Verifying SHA-256…";
                        progress.Value = 0;

                        await using var stream = File.OpenRead(tempZip);
                        using var sha = SHA256.Create();

                        var actual = Convert.ToHexString(
                            await sha.ComputeHashAsync(stream))
                            .ToLowerInvariant();

                        if (!actual.Equals(
                                release.Sha256,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            throw new InvalidDataException(
                                "The downloaded package failed SHA-256 verification.");
                        }
                    }

                    statusValue.Text = "Extracting VRCL Client…";
                    progress.Value = 0;
                    Directory.CreateDirectory(extractRoot);

                    ZipFile.ExtractToDirectory(
                        tempZip,
                        extractRoot,
                        overwriteFiles: true);

                    var payload = FindPayload(extractRoot);

                    if (payload == null)
                    {
                        throw new InvalidDataException(
                            "The selected release package does not contain a valid VRCL Client folder.");
                    }

                    statusValue.Text = $"Installing {release.Version}…";
                    progress.Value = 0;

                    // Data is intentionally excluded so an existing user's settings,
                    // cache configuration, and other protected VRCL data remain untouched.
                    CopyDirectoryPreservingData(payload, installRoot);

                    ApplyVisibleHiddenLayout(installRoot);

                    var exe = Path.Combine(installRoot, "VRCL Client.exe");
                    if (!File.Exists(exe))
                    {
                        throw new InvalidDataException(
                            "VRCL Client.exe was not found after installation.");
                    }

                    progress.Value = 100;
                    statusValue.Text = $"VRCL Client {release.Version} installed successfully.";

                    var launchResult = MessageBox.Show(
                        this,
                        $"VRCL Client {release.Version} was installed successfully.\r\n\r\nLaunch it now?",
                        "VRCL Client",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information);

                    if (launchResult == DialogResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(exe)
                        {
                            UseShellExecute = true
                        });
                    }

                    Application.Exit();
                }
                finally
                {
                    TryDeleteDirectory(extractRoot);
                    TryDeleteFile(tempZip);
                }
            }
            catch (Exception ex)
            {
                statusValue.Text = $"Installation failed: {ex.Message}";

                MessageBox.Show(
                    this,
                    ex.Message,
                    "VRCL Installer",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private static string? FindPayload(string root)
        {
            // Do not depend on the versioned outer folder name.
            // Find the actual VRCL Client executable anywhere in the extracted package.
            var executable = Directory
                .GetFiles(root, "VRCL Client.exe", SearchOption.AllDirectories)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(executable))
                return Path.GetDirectoryName(executable);

            // Fallback for a normal folder named VRCL Client.
            var direct = Path.Combine(root, "VRCL Client");
            if (File.Exists(Path.Combine(direct, "VRCL Client.exe")))
                return direct;

            return Directory
                .GetDirectories(root, "VRCL Client", SearchOption.AllDirectories)
                .FirstOrDefault(p =>
                    File.Exists(Path.Combine(p, "VRCL Client.exe")));
        }

        private static void CopyDirectoryPreservingData(
            string source,
            string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (var dir in Directory.GetDirectories(
                         source,
                         "*",
                         SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(source, dir);

                if (IsProtectedDataPath(relative))
                    continue;

                Directory.CreateDirectory(Path.Combine(destination, relative));
            }

            foreach (var file in Directory.GetFiles(
                         source,
                         "*",
                         SearchOption.AllDirectories))
            {
                var relative = Path.GetRelativePath(source, file);

                if (IsProtectedDataPath(relative))
                    continue;

                var target = Path.Combine(destination, relative);
                var targetDirectory = Path.GetDirectoryName(target);

                if (!string.IsNullOrWhiteSpace(targetDirectory))
                    Directory.CreateDirectory(targetDirectory);

                File.Copy(file, target, overwrite: true);
            }
        }

        private static bool IsProtectedDataPath(string relativePath)
        {
            var normalized = relativePath.Replace(
                Path.AltDirectorySeparatorChar,
                Path.DirectorySeparatorChar);

            var firstPart = normalized
                .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();

            return firstPart?.Equals(
                "Data",
                StringComparison.OrdinalIgnoreCase) == true;
        }

        private static void ApplyVisibleHiddenLayout(string root)
        {
            if (!Directory.Exists(root))
                return;

            foreach (var file in Directory.GetFiles(
                         root,
                         "*",
                         SearchOption.AllDirectories))
            {
                var name = Path.GetFileName(file);
                var attrs = File.GetAttributes(file);

                if (name.Equals(
                        "VRCL Client.exe",
                        StringComparison.OrdinalIgnoreCase) ||
                    name.Equals(
                        "(READ ME).txt",
                        StringComparison.OrdinalIgnoreCase))
                {
                    File.SetAttributes(file, attrs & ~FileAttributes.Hidden);
                }
                else
                {
                    File.SetAttributes(file, attrs | FileAttributes.Hidden);
                }
            }

            foreach (var dir in Directory.GetDirectories(
                         root,
                         "*",
                         SearchOption.AllDirectories))
            {
                File.SetAttributes(
                    dir,
                    File.GetAttributes(dir) | FileAttributes.Hidden);
            }
        }

        private async Task DownloadAsync(string url, string destination)
        {
            using var client = CreateHttpClient();

            using var response = await client.GetAsync(
                url,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            var total = response.Content.Headers.ContentLength ?? -1;

            await using var input = await response.Content.ReadAsStreamAsync();
            await using var output = File.Create(destination);

            var buffer = new byte[128 * 1024];
            long done = 0;
            int read;

            while ((read = await input.ReadAsync(buffer)) > 0)
            {
                await output.WriteAsync(buffer.AsMemory(0, read));
                done += read;

                if (total > 0)
                {
                    progress.Value = (int)Math.Clamp(
                        done * 100 / total,
                        0,
                        100);
                }
            }
        }

        private static HttpClient CreateHttpClient()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10)
            };

            client.DefaultRequestHeaders.UserAgent.ParseAdd(
                "VRCL-Installer/2.0");

            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(
                    "application/vnd.github+json"));

            client.DefaultRequestHeaders.Add(
                "X-GitHub-Api-Version",
                "2022-11-28");

            return client;
        }

        private void SetBusy(bool busy)
        {
            installSelectedButton.Enabled = !busy && versionBox.Items.Count > 0;
            installLatestButton.Enabled = !busy && latest != null;
            refreshButton.Enabled = !busy;
            versionBox.Enabled = !busy && versionBox.Items.Count > 0;
            locationBox.Enabled = !busy;
        }

        private static bool TryParseVersion(
            string tag,
            out string normalized)
        {
            normalized = tag.StartsWith(
                "v",
                StringComparison.OrdinalIgnoreCase)
                ? tag[1..]
                : tag;

            var numeric = normalized;

            if (numeric.EndsWith(
                    "-release-beta",
                    StringComparison.OrdinalIgnoreCase))
            {
                numeric = numeric[..^13];
            }
            else if (numeric.EndsWith(
                         "-beta",
                         StringComparison.OrdinalIgnoreCase))
            {
                numeric = numeric[..^5];
            }

            return Version.TryParse(numeric, out _);
        }

        private sealed class ReleaseInfoComparer : IComparer<ReleaseInfo>
        {
            public static readonly ReleaseInfoComparer Instance = new();

            public int Compare(ReleaseInfo? x, ReleaseInfo? y)
            {
                if (ReferenceEquals(x, y))
                    return 0;

                if (x == null)
                    return -1;

                if (y == null)
                    return 1;

                var c = CompareVersions(x.Version, y.Version);
                if (c != 0)
                    return c;

                if (x.Prerelease != y.Prerelease)
                    return x.Prerelease ? -1 : 1;

                return string.Compare(
                    x.Tag,
                    y.Tag,
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        private static int CompareVersions(string a, string b)
        {
            static Version ParseNumeric(string value)
            {
                var s = value;

                if (s.EndsWith(
                        "-release-beta",
                        StringComparison.OrdinalIgnoreCase))
                    s = s[..^13];
                else if (s.EndsWith(
                             "-beta",
                             StringComparison.OrdinalIgnoreCase))
                    s = s[..^5];

                return Version.TryParse(s, out var parsed)
                    ? parsed
                    : new Version(0, 0);
            }

            return ParseNumeric(a).CompareTo(ParseNumeric(b));
        }

        private static void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }

        private static void TryDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch { }
        }
    }
}
