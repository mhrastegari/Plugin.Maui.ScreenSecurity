using CoreFoundation;
using Plugin.Maui.ScreenSecurity.Handlers;
using UIKit;

namespace Plugin.Maui.ScreenSecurity.Platforms.MacCatalyst;

internal class ScreenRecordingProtectionManager
{
    internal static void HandleScreenRecordingProtection(bool enabled, string withColor = "", UIWindow? window = null)
    {
        CheckScreenCapture(enabled, withColor, window);

        UIScreen.Notifications.ObserveCapturedDidChange((sender, args) =>
        {
            CheckScreenCapture(enabled, withColor, window);
        });
    }

    private static void CheckScreenCapture(bool enabled, string withColor, UIWindow? window)
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
            {
                DisableScreenRecordingProtection();
            }
        }
        catch (Exception ex)
        {
            ErrorsHandler.HandleException(nameof(CheckScreenCapture), ex);
        }
    }

    private static void EnableScreenRecordingProtection(string withColor = "", UIWindow? window = null)
    {
        if (string.IsNullOrEmpty(withColor) is false)
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
