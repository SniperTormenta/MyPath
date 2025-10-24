using Microsoft.Maui.Controls.Shapes;

namespace MyPath;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        LoadHabits();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        RefreshHabits();
    }

    private async void OnAddHabitClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddHabitPage());
    }

    private async void OnStatisticsTapped(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StatisticsPage());
    }

    private async void OnProfileTapped(object sender, EventArgs e)
    {
        try
        {
            var profilePage = new ProfilePage();
            await Navigation.PushAsync(profilePage);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", "Не удалось открыть профиль", "OK");
        }
    }

    private void LoadHabits()
    {
        RefreshHabits();
    }

    private void RefreshHabits()
    {
        if (HabitsContainer == null || EmptyState == null)
            return;

        HabitsContainer.Children.Clear();

        var habits = HabitService.Instance.Habits;

        if (habits.Count == 0)
        {
            EmptyState.IsVisible = true;
            HabitsContainer.IsVisible = false;
            UpdateProgress();
            return;
        }

        EmptyState.IsVisible = false;
        HabitsContainer.IsVisible = true;

        foreach (var habit in habits)
        {
            var habitBorder = CreateHabitBorder(habit);
            HabitsContainer.Children.Add(habitBorder);
        }

        UpdateProgress();
    }

    private void UpdateProgress()
    {
        var progress = HabitService.Instance.GetTodayProgress();
        var progressPercent = (int)(progress * 100);

        ProgressLabel.Text = $"Прогресс за сегодня: {progressPercent}%";
        ProgressBox.WidthRequest = progress * 180;
    }

    private string FormatDuration(TimeSpan duration)
    {
        var parts = new List<string>();

        if (duration.Hours > 0) parts.Add($"{duration.Hours} ч");
        if (duration.Minutes > 0) parts.Add($"{duration.Minutes} мин");
        if (duration.Seconds > 0) parts.Add($"{duration.Seconds} сек");

        return parts.Count > 0 ? string.Join(" ", parts) : "0 сек";
    }

    private Border CreateHabitBorder(Habit habit)
    {
        var border = new Border
        {
            BackgroundColor = Color.FromArgb("#1E1E1E"),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
            Padding = 16,
            Stroke = Colors.Transparent
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
            }
        };

        // Левая часть - информация о привычке
        var infoStack = new VerticalStackLayout();

        infoStack.Children.Add(new Label
        {
            Text = habit.Category,
            TextColor = Color.FromArgb("#FFFFFF99"),
            FontSize = 14
        });

        infoStack.Children.Add(new Label
        {
            Text = habit.Title,
            TextColor = Colors.White,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold
        });

        if (habit.Duration != TimeSpan.Zero)
        {
            var durationText = FormatDuration(habit.Duration);
            infoStack.Children.Add(new Label
            {
                Text = durationText,
                TextColor = Color.FromArgb("#FFFFFF99"),
                FontSize = 14
            });
        }

        if (habit.ReminderDays.Count > 0)
        {
            infoStack.Children.Add(new Label
            {
                Text = $"Напоминание: {habit.ReminderDaysDisplay} в {habit.ReminderTimeDisplay}",
                TextColor = Color.FromArgb("#7EC6FF"),
                FontSize = 12
            });
        }

        Grid.SetColumn(infoStack, 0);
        grid.Children.Add(infoStack);

        // Правая часть - кнопка выполнения
        var completionButton = new Button
        {
            BackgroundColor = habit.IsCompleted ? habit.Color : Colors.Transparent,
            Text = "✓",
            TextColor = habit.IsCompleted ? Colors.White : Color.FromArgb("#FFFFFF99"),
            WidthRequest = 48,
            HeightRequest = 48,
            CornerRadius = 24,
            FontSize = 20
        };

        if (!habit.IsCompleted)
        {
            completionButton.BorderColor = Color.FromArgb("#FFFFFF99");
            completionButton.BorderWidth = 1;
        }

        completionButton.Clicked += (s, e) =>
        {
            HabitService.Instance.ToggleHabit(habit.Id);
            RefreshHabits();
        };

        Grid.SetColumn(completionButton, 1);
        grid.Children.Add(completionButton);

        border.Content = grid;
        return border;
    }
}