using CoreFoundation;
using Plugin.Maui.ScreenSecurity.Handlers;
using UIKit;

namespace Plugin.Maui.ScreenSecurity.Platforms.MacCatalyst;

internal class ScreenRecordingProtectionManager
{
    internal static void HandleScreenRecordingProtection(bool enabled, string withColor = "", UIWindow? window = null)
    {
        UIScreen.Notifications.ObserveCapturedDidChange((sender, args) =>
        {
            try
            {
                if (UIScreen.MainScreen.Captured)
                {
                    if (enabled)
                        EnableScreenRecordingProtection(withColor, window);
                    else
                        DisableScreenRecordingProtection();
                }
                else
                    DisableScreenRecordingProtection();
            }
            catch (Exception ex)
            {
                ErrorsHandler.HandleException(nameof(HandleScreenRecordingProtection), ex);
            }
        });
    }

    private static void EnableScreenRecordingProtection(string withColor = "", UIWindow? window = null)
    {
        if (!string.IsNullOrEmpty(withColor))
            ColorProtectionManager.EnableColor(window, withColor);
        else
            BlurProtectionManager.EnableBlur(window, ThemeStyle.Light);
    }

    private static void DisableScreenRecordingProtection()
    {
        DispatchQueue.MainQueue.DispatchAsync(() =>
        {
            BlurProtectionManager.DisableBlur();

            ColorProtectionManager.DisableColor();
        });
    }
}