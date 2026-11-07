using ComponentOrientedPlatform;

namespace ComponentHost;

/// <summary>
/// Форма авторизации. Роль определяется только по логину, без хранения в БД.
/// </summary>
public partial class LoginForm : Form
{
    public LoginForm()
    {
        InitializeComponent();
    }

    public string Login { get; private set; } = string.Empty;

    public AccessLevel UserAccessLevel { get; private set; } = AccessLevel.Minimal;

    private void OkButton_Click(object? sender, EventArgs e)
    {
        string login = loginTextBox.Text.Trim();

        if (string.IsNullOrWhiteSpace(login))
        {
            MessageBox.Show(
                "Введите логин.",
                "Проверка данных",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            loginTextBox.Focus();
            return;
        }

        Login = login;
        UserAccessLevel = login.Equals("admin", StringComparison.OrdinalIgnoreCase)
            ? AccessLevel.Advanced
            : login.Equals("user", StringComparison.OrdinalIgnoreCase)
                ? AccessLevel.Basic
                : AccessLevel.Minimal;

        DialogResult = DialogResult.OK;
        Close();
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
