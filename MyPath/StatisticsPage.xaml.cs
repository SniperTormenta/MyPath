using Microsoft.Maui.Controls.Shapes;

namespace MyPath;

public partial class StatisticsPage : ContentPage
{
    public int TotalHabits => HabitService.Instance.Habits.Count;
    public int CompletedToday => HabitService.Instance.Habits.Count(h => h.IsCompleted);
    public int TotalProgress => CalculateTotalProgress();
    public int MissedCount => TotalHabits - CompletedToday;

    public StatisticsPage()
    {
        InitializeComponent();
        BindingContext = this;
        LoadStatistics();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadStatistics();
    }

    private int CalculateTotalProgress()
    {
        if (TotalHabits == 0) return 0;

        // ������� ���������� �� ��� ����� (���������� ������)
        // � �������� ���������� ����� ���� �� ������� ����������
        var completedCount = HabitService.Instance.Habits.Count(h => h.IsCompleted);
        return (completedCount * 100) / TotalHabits;
    }

    private void LoadStatistics()
    {
        LoadHabitsProgress();
        LoadWeekChart();
        LoadTimeAnalytics();

        // ��������� ��������
        OnPropertyChanged(nameof(TotalHabits));
        OnPropertyChanged(nameof(CompletedToday));
        OnPropertyChanged(nameof(TotalProgress));
        OnPropertyChanged(nameof(MissedCount));
    }

    private void LoadHabitsProgress()
    {
        HabitsProgressContainer.Children.Clear();

        foreach (var habit in HabitService.Instance.Habits)
        {
            var progressFrame = CreateHabitProgressFrame(habit);
            HabitsProgressContainer.Children.Add(progressFrame);
        }
    }

    private Border CreateHabitProgressFrame(Habit habit)
    {
        var border = new Border
        {
            BackgroundColor = Color.FromArgb("#2E2E2E"),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
            Padding = new Thickness(12),
            Stroke = Colors.Transparent
        };

        var grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) }
            }
        };

        // �������� ��������
        var nameLabel = new Label
        {
            Text = habit.Title,
            TextColor = Colors.White,
            FontSize = 14,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(nameLabel, 0);
        grid.Children.Add(nameLabel);

        // ������� ����������
        var progressLabel = new Label
        {
            Text = habit.IsCompleted ? "100%" : "0%",
            TextColor = habit.IsCompleted ? Color.FromArgb("#7EC6FF") : Color.FromArgb("#FF5E5E"),
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        Grid.SetColumn(progressLabel, 1);
        grid.Children.Add(progressLabel);

        // ��������-��� - ���� ������
        var progressBarContainer = new Grid
        {
            BackgroundColor = Color.FromArgb("#1E1E1E"),
            HeightRequest = 8,
            VerticalOptions = LayoutOptions.Center,
            Padding = 0
        };

        var progressFill = new BoxView
        {
            Color = habit.IsCompleted ? Color.FromArgb("#7EC6FF") : Color.FromArgb("#FF5E5E"),
            WidthRequest = habit.IsCompleted ? progressBarContainer.Width : 0, // ������ �� ��� ������
            HeightRequest = 8,
            HorizontalOptions = LayoutOptions.Start,
            CornerRadius = 4
        };

        progressBarContainer.Children.Add(progressFill);
        Grid.SetColumn(progressBarContainer, 2);
        grid.Children.Add(progressBarContainer);

        // ��������� ������ ����� ���������
        progressBarContainer.SizeChanged += (s, e) =>
        {
            if (habit.IsCompleted)
            {
                progressFill.WidthRequest = progressBarContainer.Width;
            }
        };

        border.Content = grid;
        return border;
    }

    private void LoadWeekChart()
    {
        // ������� ������ ������� (����� �������� ����)
        for (int i = WeekChartContainer.Children.Count - 1; i >= 0; i--)
        {
            var child = WeekChartContainer.Children[i];
            // ���������, ��� ��� Border � ��������� � ������ ������
            if (child is Border border && Grid.GetRow(border) == 0)
                WeekChartContainer.Children.RemoveAt(i);
        }

        // �������� �������� �������� �� ������
        var weekProgress = CalculateRealWeekProgress();

        for (int i = 0; i < 7; i++)
        {
            var progress = weekProgress[i];
            var height = (progress / 100.0) * 80; // ������ �������

            var column = new Border
            {
                BackgroundColor = Color.FromArgb("#7EC6FF"),
                Stroke = Colors.Transparent,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(4, 4, 0, 0) },
                VerticalOptions = LayoutOptions.End,
                HeightRequest = height,
                Margin = new Thickness(2, 0, 2, 0),
                Opacity = GetDayOpacity(i) // ��������� ��������� ���
            };

            // ��������� ��������� � ���������
            var tooltip = new Label
            {
                Text = $"{progress}%",
                TextColor = Colors.White,
                FontSize = 8,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Start,
                Rotation = -90,
                Margin = new Thickness(0, 5, 0, 0)
            };

            column.Content = tooltip;

            // ������������� ������� � Grid
            Grid.SetRow(column, 0);
            Grid.SetColumn(column, i);
            WeekChartContainer.Children.Add(column);
        }
    }

    private int[] CalculateRealWeekProgress()
    {
        var weekProgress = new int[7];
        var today = DateTime.Today;
        var currentDayOfWeek = (int)today.DayOfWeek;

        // ����������� DayOfWeek � ��� ������ (0=��, 6=��)
        var russianDayOfWeek = currentDayOfWeek == 0 ? 6 : currentDayOfWeek - 1;

        for (int i = 0; i < 7; i++)
        {
            if (i < russianDayOfWeek)
            {
                // ��������� ��� - ��������� ������ (� �������� ���������� ����� �� �������)
                weekProgress[i] = new Random().Next(0, 100);
            }
            else if (i == russianDayOfWeek)
            {
                // ������� - �������� ��������
                weekProgress[i] = TotalProgress;
            }
            else
            {
                // ������� ��� - 0%
                weekProgress[i] = 0;
            }
        }

        return weekProgress;
    }

    private double GetDayOpacity(int dayIndex)
    {
        var today = DateTime.Today;
        var currentDayOfWeek = (int)today.DayOfWeek;
        var russianDayOfWeek = currentDayOfWeek == 0 ? 6 : currentDayOfWeek - 1;

        if (dayIndex < russianDayOfWeek)
            return 0.6; // ��������� ��� - ��������������
        else if (dayIndex == russianDayOfWeek)
            return 1.0; // ������� - ��������� �������
        else
            return 0.3; // ������� ��� - ����� ����������
    }

    private void LoadTimeAnalytics()
    {
        var totalTime = CalculateTotalTime();
        var averageTime = CalculateAverageTime();
        var mostProductive = GetMostProductiveHabit();

        TotalTimeLabel.Text = FormatTime(totalTime);
        AverageTimeLabel.Text = FormatTime(averageTime);
        MostProductiveLabel.Text = mostProductive ?? "��� ������";
    }

    private TimeSpan CalculateTotalTime()
    {
        var total = TimeSpan.Zero;
        foreach (var habit in HabitService.Instance.Habits)
        {
            if (habit.IsCompleted)
            {
                total = total.Add(habit.Duration);
            }
        }
        return total;
    }

    private TimeSpan CalculateAverageTime()
    {
        var completedHabits = HabitService.Instance.Habits.Count(h => h.IsCompleted);
        if (completedHabits == 0) return TimeSpan.Zero;

        var totalTime = CalculateTotalTime();
        return TimeSpan.FromMinutes(totalTime.TotalMinutes / completedHabits);
    }

    private string GetMostProductiveHabit()
    {
        var productiveHabit = HabitService.Instance.Habits
            .Where(h => h.IsCompleted && h.Duration > TimeSpan.Zero)
            .OrderByDescending(h => h.Duration)
            .FirstOrDefault();

        return productiveHabit?.Title ?? "��� ������";
    }

    private string FormatTime(TimeSpan time)
    {
        if (time.TotalHours >= 1)
            return $"{(int)time.TotalHours} � {time.Minutes} ���";
        else if (time.TotalMinutes >= 1)
            return $"{(int)time.TotalMinutes} ���";
        else
            return $"{time.Seconds} ���";
    }
}