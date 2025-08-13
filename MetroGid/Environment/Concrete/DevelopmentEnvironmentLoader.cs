using dotenv.net;
using MetroGid.Environment.Interfaces;

namespace MetroGid.Environment.Concrete;

public class DevelopmentEnvironmentLoader : IEnvironmentLoader
{
    public void Load()
    {
        string solutionRoot = FindEnvRoot() ??
            throw new FileNotFoundException("Не найден конфигурационный файл переменные окружения .env");

        string envPath = Path.Combine(solutionRoot, ".env");

        if (File.Exists(envPath))
            DotEnv.Load(new DotEnvOptions(envFilePaths: [envPath]));
        else
            throw new FileNotFoundException("Не найден конфигурационный файл переменные окружения .env");
    }

    private static string? FindEnvRoot()
    {
        System.IO.DirectoryInfo? current = new(Directory.GetCurrentDirectory());
        
        while (current != null)
        {
            if (current.GetFiles("*.env").Length > 0)
                return current.FullName;

            current = current.Parent;
        }

        return null;
    }
}