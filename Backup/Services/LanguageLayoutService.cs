using Microsoft.Win32;

namespace Backup.Services;

public static class LanguageLayoutService
{
    public static void DisableLanguageShortcut()
    {
        using var key = Registry.CurrentUser.CreateSubKey(@"Keyboard Layout\Toggle");

        key.SetValue("Hotkey", "3", RegistryValueKind.String);
        key.SetValue("Language Hotkey", "3", RegistryValueKind.String);
        key.SetValue("Layout Hotkey", "3", RegistryValueKind.String);
    }
}