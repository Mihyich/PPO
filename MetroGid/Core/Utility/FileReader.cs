namespace MetroGid.Core.Utility;

public static class FileReader
{
    public static string ReadAll(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"JSON файл не найден: {filePath}");
        }

        return File.ReadAllText(filePath);
    }
}