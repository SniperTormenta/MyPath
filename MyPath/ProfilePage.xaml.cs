namespace MyPath;

public partial class ProfilePage : ContentPage
{
    private UserProfile _userProfile;

    public ProfilePage()
    {
        InitializeComponent();
        LoadProfileData();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadProfileData();
    }

    private async void LoadProfileData()
    {
        // Загружаем профиль
        _userProfile = await Services.StorageService.LoadProfile();

        // Обновляем UI
        NameEntry.Text = _userProfile.UserName;
        QuoteLabel.Text = _userProfile.Quote;

        if (!string.IsNullOrEmpty(_userProfile.AvatarPath) && File.Exists(_userProfile.AvatarPath))
        {
            AvatarImage.Source = _userProfile.AvatarPath;
        }
        else
        {
            AvatarImage.Source = "default_avatar.png";
        }

        // Загружаем статистику
        var habits = HabitService.Instance.Habits;
        var totalHabits = habits.Count;
        var completedToday = habits.Count(h => h.IsCompleted);

        TotalHabitsLabel.Text = totalHabits.ToString();
        CompletedTodayLabel.Text = completedToday.ToString();

        UpdateProgressBars();
    }

    private void UpdateProgressBars()
    {
        var habits = HabitService.Instance.Habits;
        var totalHabits = habits.Count;

        if (totalHabits > 0)
        {
            var maxHabits = 10;
            var totalProgress = Math.Min((double)totalHabits / maxHabits * 100, 100);
            TotalHabitsProgress.WidthRequest = totalProgress * 2;

            var completedToday = habits.Count(h => h.IsCompleted);
            var completedProgress = totalHabits > 0 ? (double)completedToday / totalHabits * 100 : 0;
            CompletedTodayProgress.WidthRequest = completedProgress * 2;
        }
        else
        {
            TotalHabitsProgress.WidthRequest = 0;
            CompletedTodayProgress.WidthRequest = 0;
        }
    }

    private async void OnSaveNameClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await DisplayAlert("Ошибка", "Введите имя", "OK");
            return;
        }

        _userProfile.UserName = NameEntry.Text.Trim();
        await Services.StorageService.SaveProfile(_userProfile);

        await DisplayAlert("Успех", "Имя сохранено", "OK");
    }

    private async 
    Task
OnChangePhotoClicked(object sender, EventArgs e)
    {
        try
        {
            // Удаляем старую фотку если есть
            if (!string.IsNullOrEmpty(_userProfile.AvatarPath))
            {
                Services.ImageService.DeleteImage(_userProfile.AvatarPath);
            }

            // Выбираем новую фотку
            var newAvatarPath = await Services.ImageService.PickAndSaveImageAsync();

            if (!string.IsNullOrEmpty(newAvatarPath))
            {
                _userProfile.AvatarPath = newAvatarPath;
                await Services.StorageService.SaveProfile(_userProfile);

                AvatarImage.Source = newAvatarPath;
                await DisplayAlert("Успех", "Фото обновлено", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", "Не удалось обновить фото", "OK");
        }
    }

    private async void OnAvatarTapped(object sender, EventArgs e)
    {
        await OnChangePhotoClicked(sender, e);
    }

    private void OnNewQuoteClicked(object sender, EventArgs e)
    {
        var newQuote = Services.QuoteService.GetRandomQuote();
        QuoteLabel.Text = newQuote;
        _userProfile.Quote = newQuote;
        _ = Services.StorageService.SaveProfile(_userProfile);
    }

    private async void OnMyQuotesClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Мои цитаты", "Раздел в разработке", "OK");
    }

    private async void OnTestNotificationClicked(object sender, EventArgs e)
    {
        try
        {
            // Визуальная обратная связь
            TestNotificationButton.BackgroundColor = Color.FromArgb("#4CAF50");
            TestNotificationButton.Text = "⏳ Отправка...";

            var habits = HabitService.Instance.Habits;

            if (habits.Count == 0)
            {
                await Services.NotificationService.ShowTestNotification(
                    "😔 Нет привычек",
                    "«Путь в тысячи километров начинается с первого шага. Самое время добавить первую привычку!»"
                );
            }
            else
            {
                var random = new Random();
                var randomHabit = habits[random.Next(habits.Count)];
                var notification = GetRandomNotification(randomHabit.Title);

                await Services.NotificationService.ShowTestNotification(notification.Title, notification.Message);
            }

            // Возвращаем исходный вид
            TestNotificationButton.BackgroundColor = Color.FromArgb("#666666");
            TestNotificationButton.Text = "🔔 Тестовое уведомление";
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", "Не удалось отправить уведомление", "OK");
            TestNotificationButton.BackgroundColor = Color.FromArgb("#666666");
            TestNotificationButton.Text = "🔔 Тестовое уведомление";
        }
    }

    private (string Title, string Message) GetRandomNotification(string habitName)
    {
        var notifications = new List<(string Title, string Message)>
        {
            ($"⏰ Напоминание: {habitName}", $"Самое время выполнить '{habitName}'. Не откладывай на потом!"),
            ($"🎯 Время для {habitName}", $"Твоя привычка '{habitName}' ждет тебя. Сделай это сейчас!"),
            ($"💪 {habitName} зовет!", $"Не пропускай '{habitName}'. Каждый шаг важен на пути самурая!"),
            ($"🌟 Пора заняться {habitName}", $"Привычка '{habitName}' - это твой вклад в лучшее будущее!"),
            ($"📅 Напоминание о {habitName}", $"Помни о своей цели! '{habitName}' ждет выполнения."),
            ($"⚡ Время действовать!", $"Привычка '{habitName}' требует внимания. Не подведи себя!"),
            ($"🎖️ Вызов принят: {habitName}", $"Самурай не отступает! Выполни '{habitName}' прямо сейчас!"),
            ($"🌅 Утренний ритуал: {habitName}", $"Начни день с '{habitName}'. Это задаст тон всему дню!"),
            ($"🌙 Вечерняя практика: {habitName}", $"Заверши день с '{habitName}'. Иди спать победителем!"),
            ($"🔄 Не прерывай цепь: {habitName}", $"Твоя серия под угрозой! Выполни '{habitName}' сегодня!")
        };

        var random = new Random();
        return notifications[random.Next(notifications.Count)];
    }

    private async void OnExportDataClicked(object sender, EventArgs e)
    {
        var habits = HabitService.Instance.Habits;

        if (habits.Count == 0)
        {
            await DisplayAlert("Экспорт данных", "Нет данных для экспорта", "OK");
            return;
        }

        try
        {
            var report = new System.Text.StringBuilder();
            report.AppendLine("=== ОТЧЕТ ПРИВЫЧЕК ===");
            report.AppendLine($"Дата экспорта: {DateTime.Now:dd.MM.yyyy HH:mm}");
            report.AppendLine($"Всего привычек: {habits.Count}");
            report.AppendLine();

            foreach (var habit in habits)
            {
                report.AppendLine($"📌 {habit.Title}");
                report.AppendLine($"   Категория: {habit.Category}");
                report.AppendLine($"   Длительность: {FormatDuration(habit.Duration)}");
                report.AppendLine($"   Статус: {(habit.IsCompleted ? "✅ Выполнено" : "❌ Не выполнено")}");
                if (habit.ReminderDays.Count > 0)
                {
                    report.AppendLine($"   Напоминание: {habit.ReminderDaysDisplay} в {habit.ReminderTimeDisplay}");
                }
                report.AppendLine();
            }

            await DisplayAlert("Экспорт данных", report.ToString(), "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", "Не удалось экспортировать данные", "OK");
        }
    }

    private async void OnResetProgressClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert(
            "Сброс прогресса",
            "Вы уверены, что хотите сбросить весь прогресс? Это действие нельзя отменить.",
            "Да, сбросить",
            "Отмена");

        if (result)
        {
            HabitService.Instance.Habits.Clear();
            LoadProfileData();
            await DisplayAlert("Сброс прогресса", "Весь прогресс сброшен", "OK");
        }
    }

    private string FormatDuration(TimeSpan duration)
    {
        if (duration == TimeSpan.Zero) return "Не задано";

        var parts = new List<string>();
        if (duration.Hours > 0) parts.Add($"{duration.Hours} ч");
        if (duration.Minutes > 0) parts.Add($"{duration.Minutes} мин");
        if (duration.Seconds > 0) parts.Add($"{duration.Seconds} сек");

        return parts.Count > 0 ? string.Join(" ", parts) : "0 сек";
    }
}