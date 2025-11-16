using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace MyPath.Services;

public class NotificationService
{
    public async Task<bool> RequestNotificationPermission()
    {
        try
        {
            // Просто проверяем доступность вибрации
            return Vibration.Default.IsSupported;
        }
        catch (Exception ex)
        {
            return true;
        }
    }

    public async Task ShowTestNotification(string title, string message)
    {
        try
        {
            // Вибрация
            try
            {
                if (Vibration.Default.IsSupported)
                    Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(200));
            }
            catch { }

            // Красивый алерт
            await Application.Current.MainPage.DisplayAlert(
                $"🔔 {title}",
                $"{message}\n\n" +
                $"────────────────────\n" +
                $"📱 Тестовое уведомление\n" +
                $"⏰ {DateTime.Now:HH:mm}\n" +
                $"────────────────────",
                "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }
    }

    public void CancelAllNotifications()
    {
        // Пустая реализация
    }
}