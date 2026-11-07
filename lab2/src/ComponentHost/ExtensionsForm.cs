using PluginsContracts;
using ProductCatalog.Data;

namespace ComponentHost;

/// <summary>
/// Форма выгрузки отчётов через плагины.
/// </summary>
public partial class ExtensionsForm : Form
{
    private ProductRepository _productRepository = null!;

    public ExtensionsForm()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (DesignMode)
        {
            return;
        }

        try
        {
            _productRepository = new ProductRepository();
        }
        catch (Exception ex)
        {
            ShowError(ex);
            textButton.Enabled = false;
            lineChartButton.Enabled = false;
            pieChartButton.Enabled = false;
            return;
        }

        PluginLoadResult plugins = PluginLoader.LoadConfiguredPlugins();

        FillComboBox(textComboBox, plugins.TextPlugins, textButton);
        FillComboBox(lineChartComboBox, plugins.LineChartPlugins, lineChartButton);
        FillComboBox(pieChartComboBox, plugins.PieChartPlugins, pieChartButton);

        if (plugins.Errors.Count > 0)
        {
            MessageBox.Show(
                string.Join(Environment.NewLine, plugins.Errors),
                "Ошибки загрузки плагинов",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private static void FillComboBox(
        ComboBox comboBox,
        List<PluginOption> plugins,
        Button button)
    {
        comboBox.BeginUpdate();
        try
        {
            comboBox.Items.Clear();

            foreach (PluginOption plugin in plugins)
            {
                comboBox.Items.Add(plugin);
            }
        }
        finally
        {
            comboBox.EndUpdate();
        }

        if (comboBox.Items.Count > 0)
        {
            comboBox.SelectedIndex = 0;
        }

        button.Enabled = comboBox.Items.Count > 0;
    }

    private async void TextButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (textComboBox.SelectedItem is not PluginOption option
                || option.Instance is not IReportDocumentWithContextTextsContract plugin)
            {
                MessageBox.Show(
                    "Плагин текстового документа не выбран.",
                    "Плагин не выбран",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<Product> products = _productRepository.GetAll()
                .Where(product => product.Quantity is > 0)
                .ToList();

            if (products.Count == 0)
            {
                MessageBox.Show(
                    "Нет продуктов в наличии. Добавьте продукты в справочнике.",
                    "Нет данных",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string? filePath = ShowSaveDialog(
                "Excel (*.xlsx)|*.xlsx",
                "products_in_stock.xlsx",
                "xlsx");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            List<string> paragraphs = products
                .Select(product =>
                    $"Название: {product.Name}{Environment.NewLine}Описание: {product.Description}")
                .ToList();

            await ExecutePluginAsync(
                () => plugin.CreateDocumentAsync(
                    filePath,
                    "Продукты в наличии",
                    paragraphs),
                textButton,
                filePath);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private async void LineChartButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (lineChartComboBox.SelectedItem is not PluginOption option
                || option.Instance is not IReportDocumentWithChartLineContract plugin)
            {
                MessageBox.Show(
                    "Плагин документа с линейной диаграммой не выбран.",
                    "Плагин не выбран",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<(string Category, double Count)> groups = GetOutOfStockGroups();

            if (groups.Count == 0)
            {
                MessageBox.Show(
                    "Нет продуктов, отсутствующих на складе.",
                    "Нет данных",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string? filePath = ShowSaveDialog(
                "Excel (*.xlsx)|*.xlsx",
                "products_line_chart.xlsx",
                "xlsx");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            var series = groups.ToDictionary(
                group => group.Category,
                group => new List<(int Parameter, double Value)>
                {
                    (1, group.Count)
                });

            await ExecutePluginAsync(
                () => plugin.CreateDocumentAsync(
                    filePath,
                    "Продукты, отсутствующие на складе",
                    "Количество отсутствующих продуктов по категориям",
                    series),
                lineChartButton,
                filePath);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private async void PieChartButton_Click(object? sender, EventArgs e)
    {
        try
        {
            if (pieChartComboBox.SelectedItem is not PluginOption option
                || option.Instance is not IReportDocumentWithChartPieContract plugin)
            {
                MessageBox.Show(
                    "Плагин документа с круговой диаграммой не выбран.",
                    "Плагин не выбран",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            List<(string Category, double Count)> groups = GetOutOfStockGroups();

            if (groups.Count == 0)
            {
                MessageBox.Show(
                    "Нет продуктов, отсутствующих на складе.",
                    "Нет данных",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string? filePath = ShowSaveDialog(
                "PDF (*.pdf)|*.pdf",
                "products_pie_chart.pdf",
                "pdf");

            if (string.IsNullOrWhiteSpace(filePath))
            {
                return;
            }

            List<(string Parameter, double Value)> series = groups
                .Select(group => (group.Category, group.Count))
                .ToList();

            await ExecutePluginAsync(
                () => plugin.CreateDocumentAsync(
                    filePath,
                    "Продукты, отсутствующие на складе",
                    "Количество отсутствующих продуктов по категориям",
                    "Количество продуктов",
                    series),
                pieChartButton,
                filePath);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private List<(string Category, double Count)> GetOutOfStockGroups()
    {
        return _productRepository.GetAll()
            .Where(product => product.Quantity is null or <= 0)
            .GroupBy(product => string.IsNullOrWhiteSpace(product.Category)
                ? "Без категории"
                : product.Category)
            .Select(group => (Category: group.Key, Count: (double)group.Count()))
            .OrderBy(group => group.Category)
            .ToList();
    }

    private string? ShowSaveDialog(
        string filter,
        string defaultFileName,
        string defaultExtension)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = filter,
            FileName = defaultFileName,
            DefaultExt = defaultExtension,
            AddExtension = true,
            OverwritePrompt = true
        };

        return dialog.ShowDialog(this) == DialogResult.OK
            ? dialog.FileName
            : null;
    }

    private async Task ExecutePluginAsync(
        Func<Task> action,
        Button button,
        string filePath)
    {
        button.Enabled = false;

        try
        {
            await action();

            MessageBox.Show(
                $"Документ сохранён:{Environment.NewLine}{filePath}",
                "Готово",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
        finally
        {
            button.Enabled = true;
        }
    }

    private static void ShowError(Exception exception)
    {
        MessageBox.Show(
            exception.Message,
            "Ошибка создания документа",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
