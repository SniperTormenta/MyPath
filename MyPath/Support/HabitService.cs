namespace MyPath;

public class HabitService
{
    private static HabitService _instance;
    public static HabitService Instance => _instance ??= new HabitService();

    public List<Habit> Habits { get; private set; } = new List<Habit>();

    public void AddHabit(Habit habit)
    {
        Habits.Add(habit);
    }

    public void RemoveHabit(string id)
    {
        var habit = Habits.FirstOrDefault(h => h.Id == id);
        if (habit != null)
            Habits.Remove(habit);
    }

    public void ToggleHabit(string id)
    {
        var habit = Habits.FirstOrDefault(h => h.Id == id);
        if (habit != null)
            habit.IsCompleted = !habit.IsCompleted;
    }

    public double GetTodayProgress()
    {
        if (Habits.Count == 0) return 0;
        var completedCount = Habits.Count(h => h.IsCompleted);
        return (double)completedCount / Habits.Count;
    }
}