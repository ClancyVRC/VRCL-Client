using System.Reflection;

namespace VRCLClient;

internal static class VrclVersion
{
    public static string Current
    {
        get
        {
            var value = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            if (string.IsNullOrWhiteSpace(value))
                value = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
            return (value ?? "0.0.0").Split('+')[0];
        }
    }
}