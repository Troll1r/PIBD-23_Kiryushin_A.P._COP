namespace ComponentHost;

static class Program
{
    /// <summary>
    /// Главная точка входа приложения.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var loginForm = new LoginForm();

        if (loginForm.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        Application.Run(new MainForm(loginForm.Login, loginForm.UserAccessLevel));
    }
}
