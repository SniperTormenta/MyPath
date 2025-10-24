namespace MyPath.Services;

public static class QuoteService
{
    private static List<string> _quotes = new();
    private static readonly Random _random = new();

    static QuoteService()
    {
        LoadQuotes();
    }

    public static void LoadQuotes()
    {
        _quotes = StorageService.LoadQuotes();
    }

    public static string GetRandomQuote()
    {
        if (_quotes.Count == 0)
            return "«Путь в тысячи километров начинается с одного шага.»";

        return _quotes[_random.Next(_quotes.Count)];
    }

    public static List<string> GetAllQuotes()
    {
        return new List<string>(_quotes);
    }

    public static void AddQuote(string quote)
    {
        if (!string.IsNullOrWhiteSpace(quote) && !_quotes.Contains(quote))
        {
            _quotes.Add(quote);
            StorageService.SaveQuotes(_quotes);
        }
    }

    public static void RemoveQuote(string quote)
    {
        _quotes.Remove(quote);
        StorageService.SaveQuotes(_quotes);
    }

    public static void UpdateQuotes(List<string> quotes)
    {
        _quotes = quotes.Where(q => !string.IsNullOrWhiteSpace(q)).Distinct().ToList();
        StorageService.SaveQuotes(_quotes);
    }
}