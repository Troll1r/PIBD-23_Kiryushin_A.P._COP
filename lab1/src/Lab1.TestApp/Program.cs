namespace Lab1.TestApp;

static class Program
{
    /// <summary>
    /// Главная точка входа для приложения.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Настройка конфигурации приложения (стили, DPI и т. п.).
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }
}
