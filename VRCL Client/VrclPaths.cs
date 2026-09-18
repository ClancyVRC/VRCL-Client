using System;
using System.IO;

namespace VRCLClient;

/// <summary>
/// Central application/data paths. App files live beside the executable;
/// user data lives in the separate Data folder and must be preserved by updates.
/// </summary>
internal static class VrclPaths
{
    public static string AppDirectory => AppContext.BaseDirectory;
    public static string DataDirectory => Path.Combine(AppDirectory, "Data");
    public static string SettingsFile => Path.Combine(DataDirectory, "settings.json");
    public static string ThemesDirectory => Path.Combine(AppDirectory, "Themes");

    public static string AppFile(string fileName) => Path.Combine(AppDirectory, fileName);
    public static string DataFile(string fileName) => Path.Combine(DataDirectory, fileName);
    public static string ThemeFile(string fileName) => Path.Combine(ThemesDirectory, fileName);
}