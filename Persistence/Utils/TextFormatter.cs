namespace Persistence.Utils;

public static class TextFormatter
{
    public static string GetArgument(string text, string command)
    {
        return text.Replace(command, string.Empty); 
    }

    public static string GetDateFormated(DateTime dateTime)
    {
        return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
    }
}
