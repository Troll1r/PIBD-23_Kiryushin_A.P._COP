using ComponentOrientedPlatform;

namespace ComponentHost;

/// <summary>
/// Главное окно приложения: меню и вкладки с компонентами.
/// </summary>
public partial class MainForm : Form
{
    private readonly AccessLevel _userAccessLevel;
    private readonly List<IComponentContract> _components = [];

    public MainForm() : this("guest", AccessLevel.Minimal)
    {
    }

    public MainForm(string login, AccessLevel userAccessLevel)
    {
        InitializeComponent();
        _userAccessLevel = userAccessLevel;
        Text = $"Учет продуктов — {login} ({GetRoleName(userAccessLevel)})";
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (DesignMode)
        {
            return;
        }

        LoadComponents();
    }

    private void LoadComponents()
    {
        try
        {
            _components.AddRange(
                ComponentLoader.LoadConfiguredComponents(out List<string> errors));

            BuildMenu();

            if (errors.Count > 0)
            {
                MessageBox.Show(
                    string.Join(Environment.NewLine, errors),
                    "Ошибки загрузки компонентов",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка загрузки компонентов",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void BuildMenu()
    {
        directoriesToolStripMenuItem.DropDownItems.Clear();
        reportsToolStripMenuItem.DropDownItems.Clear();

        foreach (IComponentContract component in _components
                     .Where(component => component.MinimumAccessLevel <= _userAccessLevel)
                     .OrderBy(component => component.Title))
        {
            ToolStripMenuItem target = component.Section == ComponentSection.Directories
                ? directoriesToolStripMenuItem
                : reportsToolStripMenuItem;

            var menuItem = new ToolStripMenuItem(component.Title);
            menuItem.Click += (_, _) => OpenComponent(component);
            target.DropDownItems.Add(menuItem);
        }

        directoriesToolStripMenuItem.Visible =
            directoriesToolStripMenuItem.DropDownItems.Count > 0;
        reportsToolStripMenuItem.Visible =
            reportsToolStripMenuItem.DropDownItems.Count > 0;

        if (!directoriesToolStripMenuItem.Visible && !reportsToolStripMenuItem.Visible)
        {
            MessageBox.Show(
                "Для вашей роли нет доступных компонентов.",
                "Нет компонентов",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }

    private void OpenComponent(IComponentContract component)
    {
        try
        {
            string tabName = $"tabPage_{component.Id}";

            if (tabControl.TabPages.ContainsKey(tabName))
            {
                tabControl.SelectedTab = tabControl.TabPages[tabName];
                return;
            }

            UserControl control = component.CreateControl();
            control.Dock = DockStyle.Fill;

            var tabPage = new TabPage(component.Title)
            {
                Name = tabName,
                Padding = new Padding(3)
            };
            tabPage.Controls.Add(control);

            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка открытия компонента",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void TabControl_DoubleClick(object? sender, EventArgs e)
    {
        if (tabControl.SelectedTab is not { } tabPage)
        {
            return;
        }

        Point cursorPosition = tabControl.PointToClient(Cursor.Position);
        Rectangle selectedTabBounds = tabControl.GetTabRect(tabControl.SelectedIndex);

        if (!selectedTabBounds.Contains(cursorPosition))
        {
            return;
        }

        tabControl.TabPages.Remove(tabPage);
        tabPage.Dispose();
    }

    private static string GetRoleName(AccessLevel accessLevel)
    {
        return accessLevel switch
        {
            AccessLevel.Advanced => "администратор",
            AccessLevel.Basic => "пользователь",
            _ => "минимальный доступ"
        };
    }
}
