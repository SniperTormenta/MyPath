namespace MyPath.Services;

public static class NotificationService
{
    public static async Task ShowTestNotification(string title, string message)
    {
        try
        {
            // Вибрация (если доступна)
            try
            {
                if (Microsoft.Maui.Devices.DeviceInfo.Platform == DevicePlatform.Android)
                {
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
                }
            }
            catch { }

            // Показываем красивое уведомление
            await Application.Current.MainPage.DisplayAlert(
                $"🔔 {title}",
                $"{message}\n\n📱 Это тестовое уведомление",
                "OK");
        }
        catch (Exception ex)
        {
            // Fallback
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }
}