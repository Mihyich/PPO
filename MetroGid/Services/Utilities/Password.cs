namespace MetroGid.Services.Utilities
{
    public static class Password
    {
        public static bool IsCorrect(string p) =>
            p.Length >= 4 && p.Length <= 255;

    }
}