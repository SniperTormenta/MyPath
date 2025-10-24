using System.Text.Json;

namespace MyPath.Services;

public static class StorageService
{
    private const string PROFILE_KEY = "user_profile";
    private const string QUOTES_KEY = "custom_quotes";

    // Сохранение профиля
    public static async Task SaveProfile(UserProfile profile)
    {
        try
        {
            var json = JsonSerializer.Serialize(profile);
            await SecureStorage.SetAsync(PROFILE_KEY, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Save profile error: {ex.Message}");
        }
    }

    // Загрузка профиля
    public static async Task<UserProfile> LoadProfile()
    {
        try
        {
            var json = await SecureStorage.GetAsync(PROFILE_KEY);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<UserProfile>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load profile error: {ex.Message}");
        }

        // Профиль по умолчанию
        return new UserProfile
        {
            UserName = "Александр К.",
            Quote = "«Победа над собой начинается с победы над своей ленью.»"
        };
    }

    // Сохранение цитат
    public static async Task SaveQuotes(List<string> quotes)
    {
        try
        {
            var json = JsonSerializer.Serialize(quotes);
            Preferences.Set(QUOTES_KEY, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Save quotes error: {ex.Message}");
        }
    }

    // Загрузка цитат
    public static List<string> LoadQuotes()
    {
        try
        {
            var json = Preferences.Get(QUOTES_KEY, string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                return JsonSerializer.Deserialize<List<string>>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load quotes error: {ex.Message}");
        }

        // Цитаты по умолчанию
        return new List<string>
        {
            "«Победа над собой начинается с победы над своей ленью.»",
            "«Путь в тысячи километров начинается с одного шага.»",
            "«Самурай должен быть сильным, даже когда он слаб.»",
            "«Дисциплина — это путь к свободе.»",
            "«Каждое утро — новая битва, каждое действие — новый шаг.»",
            "«Сила не в отсутствии страха, а в умении действовать вопреки ему.»",
            "«Мастерство приходит через повторение, а не через случай.»",
            "«Терпение — оружие сильного, нетерпение — слабость слабого.»",
            "«Истинный путь виден только идущему.»",
            "«Сегодняшние усилия — завтрашние победы.»"
        };
    }
}