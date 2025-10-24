using Microsoft.Maui.Controls.Shapes;

namespace MyPath;

public partial class AddHabitPage : ContentPage
{
    private Color _selectedColor = Colors.Red;
    private TimeSpan _selectedDuration = TimeSpan.Zero;
    private TimeSpan _reminderTime = new TimeSpan(9, 0, 0); // По умолчанию 9:00
    private bool _isAdvancedExpanded = false;
    private readonly HashSet<DayOfWeek> _selectedDays = new();

    private readonly List<Color> _availableColors =
    [
        Colors.Red,
        Colors.Blue,
        Colors.Green,
        Colors.Orange,
        Colors.Purple,
        Colors.Yellow,
        Colors.Brown,
        Colors.Gray
    ];

    private readonly List<string> _categories =
    [
        "Привычка", "Спорт", "Образование", "Здоровье", "Работа", "Отдых"
    ];

    // Словарь для связи дней недели с элементами UI
    private readonly Dictionary<DayOfWeek, Border> _dayBorders = new();

    public AddHabitPage()
    {
        InitializeComponent();

        CategoryPicker.ItemsSource = _categories;
        CategoryPicker.SelectedIndex = 0;
        CreateColorSelection();
        UpdateDurationDisplay();
        UpdateReminderTimeDisplay();
        InitializeDaySelection();

        // Подписываемся на тапы по дням недели
        SubscribeToDayClicks();
    }

    private void InitializeDaySelection()
    {
        // Связываем дни недели с элементами UI
        _dayBorders[DayOfWeek.Monday] = MondayBorder;
        _dayBorders[DayOfWeek.Tuesday] = TuesdayBorder;
        _dayBorders[DayOfWeek.Wednesday] = WednesdayBorder;
        _dayBorders[DayOfWeek.Thursday] = ThursdayBorder;
        _dayBorders[DayOfWeek.Friday] = FridayBorder;
        _dayBorders[DayOfWeek.Saturday] = SaturdayBorder;
        _dayBorders[DayOfWeek.Sunday] = SundayBorder;
    }

    private void SubscribeToDayClicks()
    {
        foreach (var (day, border) in _dayBorders)
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnDayTapped(day);
            border.GestureRecognizers.Add(tapGesture);
        }
    }

    private void OnDayTapped(DayOfWeek day)
    {
        if (_selectedDays.Contains(day))
        {
            _selectedDays.Remove(day);
            UpdateDayAppearance(day, false);
        }
        else
        {
            _selectedDays.Add(day);
            UpdateDayAppearance(day, true);
        }
    }

    private void UpdateDayAppearance(DayOfWeek day, bool isSelected)
    {
        if (_dayBorders.TryGetValue(day, out var border))
        {
            border.BackgroundColor = isSelected ? _selectedColor : Color.FromArgb("#2E2E2E");
            border.Stroke = isSelected ? _selectedColor : Color.FromArgb("#666666");

            if (border.Content is Label label)
            {
                label.TextColor = isSelected ? Colors.White : Color.FromArgb("#FFFFFF99");
            }
        }
    }

    private void OnSelectAllDaysClicked(object sender, EventArgs e)
    {
        _selectedDays.Clear();
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            _selectedDays.Add(day);
            UpdateDayAppearance(day, true);
        }
    }

    private void OnClearDaysClicked(object sender, EventArgs e)
    {
        _selectedDays.Clear();
        foreach (var day in Enum.GetValues<DayOfWeek>())
        {
            UpdateDayAppearance(day, false);
        }
    }
    private async void OnReminderTimeClicked(object sender, EventArgs e)
    {
        var timePickerPage = new TimePickerPage(_reminderTime);

        timePickerPage.Disappearing += (s, args) =>
        {
            if (timePickerPage.SelectedTime != TimeSpan.Zero)
            {
                _reminderTime = timePickerPage.SelectedTime;
                UpdateReminderTimeDisplay();
            }
        };

        await Navigation.PushModalAsync(timePickerPage);
    }

    private void UpdateReminderTimeDisplay()
    {
        ReminderTimeLabel.Text = $"Время: {_reminderTime:hh\\:mm}";
    }

    private void CreateColorSelection()
    {
        foreach (var color in _availableColors)
        {
            var border = new Border
            {
                BackgroundColor = color,
                WidthRequest = 40,
                HeightRequest = 40,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20) },
                Padding = 0,
                Stroke = Colors.Transparent,
                Margin = new Thickness(5)
            };

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => OnColorSelected(color, border);
            border.GestureRecognizers.Add(tapGesture);

            ColorsContainer.Add(border);
        }
    }

    private void OnColorSelected(Color color, Border selectedBorder)
    {
        _selectedColor = color;

        // Обновляем внешний вид выбранных дней
        foreach (var day in _selectedDays)
        {
            UpdateDayAppearance(day, true);
        }

        foreach (var child in ColorsContainer.Children)
        {
            if (child is Border border)
            {
                border.Stroke = border == selectedBorder ? Colors.White : Colors.Transparent;
                border.StrokeThickness = border == selectedBorder ? 2 : 0;
            }
        }
    }

    private async void OnDurationClicked(object sender, EventArgs e)
    {
        var timeString = await DisplayPromptAsync(
            "Длительность привычки",
            "Введите длительность в формате ЧЧ:ММ:СС\nПример: 00:30:00 для 30 минут",
            "OK",
            "Отмена",
            _selectedDuration == TimeSpan.Zero ? "00:30:00" : _selectedDuration.ToString(@"hh\:mm\:ss"),
            -1,
            Keyboard.Numeric,
            _selectedDuration.ToString(@"hh\:mm\:ss"));

        if (!string.IsNullOrEmpty(timeString) && TimeSpan.TryParse(timeString, out var duration))
        {
            _selectedDuration = duration;
            UpdateDurationDisplay();
        }
        else if (!string.IsNullOrEmpty(timeString))
        {
            await DisplayAlert("Ошибка", "Некорректный формат времени", "OK");
        }
    }

    private void UpdateDurationDisplay()
    {
        if (_selectedDuration == TimeSpan.Zero)
        {
            DurationLabel.Text = "Не задано";
            DurationLabel.TextColor = Color.FromArgb("#FFFFFF99");
        }
        else
        {
            var hours = _selectedDuration.Hours;
            var minutes = _selectedDuration.Minutes;
            var seconds = _selectedDuration.Seconds;

            var parts = new List<string>();

            if (hours > 0) parts.Add($"{hours} ч");
            if (minutes > 0) parts.Add($"{minutes} мин");
            if (seconds > 0) parts.Add($"{seconds} сек");

            if (parts.Count == 0)
                parts.Add("0 сек");

            DurationLabel.Text = string.Join(" ", parts);
            DurationLabel.TextColor = Colors.White;
        }
    }

    private void OnAdvancedSettingsTapped(object sender, TappedEventArgs e)
    {
        _isAdvancedExpanded = !_isAdvancedExpanded;
        AdvancedSettingsContent.IsVisible = _isAdvancedExpanded;
        ExpandCollapseIcon.Text = _isAdvancedExpanded ? "▼" : "▶";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TitleEntry.Text))
        {
            await DisplayAlert("Ошибка", "Введите название привычки", "OK");
            return;
        }

        var habit = new Habit
        {
            Title = TitleEntry.Text.Trim(),
            Description = DescriptionEditor.Text?.Trim() ?? string.Empty,
            Duration = _selectedDuration,
            Category = CategoryPicker.SelectedItem?.ToString() ?? "Привычка",
            Color = _selectedColor,
            ReminderDays = _selectedDays.ToList(),
            ReminderTime = _reminderTime
        };

        HabitService.Instance.AddHabit(habit);
        await Navigation.PopAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}