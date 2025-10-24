using Microsoft.Maui.Storage;
using Microsoft.Maui.Graphics.Platform;

namespace MyPath.Services;

public static class ImageService
{
    public static async Task<string> PickAndSaveImageAsync()
    {
        try
        {
            var fileResult = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Выберите аватар",
                FileTypes = FilePickerFileType.Images
            });

            if (fileResult != null)
            {
                // Копируем файл в папку приложения
                var targetFile = Path.Combine(FileSystem.AppDataDirectory, $"avatar_{DateTime.Now.Ticks}.jpg");

                using (var sourceStream = await fileResult.OpenReadAsync())
                using (var targetStream = File.Create(targetFile))
                {
                    await sourceStream.CopyToAsync(targetStream);
                }

                // Ресайзим изображение до 300x300
                await ResizeImageAsync(targetFile, targetFile, 300, 300);

                return targetFile;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Image pick error: {ex.Message}");
        }

        return string.Empty;
    }

    private static async Task ResizeImageAsync(string sourcePath, string targetPath, int maxWidth, int maxHeight)
    {
        try
        {
            using var sourceStream = File.OpenRead(sourcePath);
            var image = PlatformImage.FromStream(sourceStream);

            if (image != null)
            {
                var resizedImage = image.Downsize(maxWidth, true);

                using var targetStream = File.Create(targetPath);
                await resizedImage.SaveAsync(targetStream, Microsoft.Maui.Graphics.ImageFormat.Jpeg, quality: 0.8f);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Image resize error: {ex.Message}");
            // Если ресайз не удался, оставляем оригинальный файл
        }
    }

    public static void DeleteImage(string imagePath)
    {
        try
        {
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete image error: {ex.Message}");
        }
    }
}