namespace MyPath;

public class UserProfile
{
    public string UserName { get; set; } = "Ваше имя";
    public string Quote { get; set; } = "«Победа над собой начинается с победы над своей ленью.»";
    public string AvatarPath { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; } = DateTime.Now;

    // Сохраняем в файл
    public void Save()
    {
        var filePath = GetProfileFilePath();
        var json = System.Text.Json.JsonSerializer.Serialize(this);
        File.WriteAllText(filePath, json);
    }

    // Загружаем из файла
    public static UserProfile Load()
    {
        var filePath = GetProfileFilePath();
        if (File.Exists(filePath))
        {
            try
            {
                var json = File.ReadAllText(filePath);
                return System.Text.Json.JsonSerializer.Deserialize<UserProfile>(json) ?? new UserProfile();
            }
            catch
            {
                return new UserProfile();
            }
        }
        return new UserProfile();
    }

    private static string GetProfileFilePath()
    {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(documentsPath, "userprofile.json");
    }
}