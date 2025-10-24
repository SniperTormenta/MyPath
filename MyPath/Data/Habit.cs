namespace MyPath;

public class Habit
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string Category { get; set; } = "Привычка";
    public bool IsCompleted { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public Color Color { get; set; } = Colors.Red;

    public List<DayOfWeek> ReminderDays { get; set; } = new();
    public TimeSpan ReminderTime { get; set; } = new TimeSpan(9, 0, 0);

    public string ReminderDaysDisplay
    {
        get
        {
            if (ReminderDays.Count == 0) return "Без напоминания";
            if (ReminderDays.Count == 7) return "Каждый день";

            var dayNames = new Dictionary<DayOfWeek, string>
            {
                [DayOfWeek.Monday] = "Пн",
                [DayOfWeek.Tuesday] = "Вт",
                [DayOfWeek.Wednesday] = "Ср",
                [DayOfWeek.Thursday] = "Чт",
                [DayOfWeek.Friday] = "Пт",
                [DayOfWeek.Saturday] = "Сб",
                [DayOfWeek.Sunday] = "Вс"
            };

            return string.Join(", ", ReminderDays.OrderBy(d => d).Select(d => dayNames[d]));
        }
    }

    public string ReminderTimeDisplay => $"{ReminderTime:hh\\:mm}";
}