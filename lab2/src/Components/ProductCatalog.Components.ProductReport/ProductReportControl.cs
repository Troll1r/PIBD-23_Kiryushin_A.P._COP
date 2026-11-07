using ProductCatalog.Data;
using System.Data;

namespace ProductCatalog.Components.ProductReport;

/// <summary>
/// Отчёт: продукты выбранной категории в виде таблицы.
/// </summary>
public partial class ProductReportControl : UserControl
{
    private ProductRepository _productRepository = null!;
    private CategoryRepository _categoryRepository = null!;

    public ProductReportControl()
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

        _productRepository = new ProductRepository();
        _categoryRepository = new CategoryRepository();

        FillCategories();
        BuildReport();
    }

    private void FillCategories()
    {
        try
        {
            string? previouslySelected = categoryComboBox.SelectedItem as string;

            List<string> categories = _categoryRepository.GetAll();
            List<string> productCategories = _productRepository.GetDistinctCategories();

            List<string> allCategories = categories
                .Union(productCategories, StringComparer.OrdinalIgnoreCase)
                .OrderBy(category => category, StringComparer.CurrentCultureIgnoreCase)
                .ToList();

            categoryComboBox.BeginUpdate();
            try
            {
                categoryComboBox.Items.Clear();
                categoryComboBox.Items.AddRange(allCategories.Cast<object>().ToArray());

                if (previouslySelected != null
                    && allCategories.Contains(previouslySelected, StringComparer.Ordinal))
                {
                    categoryComboBox.SelectedItem = previouslySelected;
                }
                else if (categoryComboBox.Items.Count > 0)
                {
                    categoryComboBox.SelectedIndex = 0;
                }
            }
            finally
            {
                categoryComboBox.EndUpdate();
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BuildReport()
    {
        string category = categoryComboBox.SelectedItem as string ?? string.Empty;

        if (string.IsNullOrWhiteSpace(category))
        {
            dataGridViewReport.DataSource = null;
            labelCount.Text = "Найдено: 0";
            return;
        }

        try
        {
            List<Product> products = _productRepository.GetByCategory(category);

            var table = new DataTable();
            table.Columns.Add("Идентификатор", typeof(int));
            table.Columns.Add("Название", typeof(string));
            table.Columns.Add("Описание", typeof(string));
            table.Columns.Add("Категория", typeof(string));
            table.Columns.Add("Количество", typeof(string));

            foreach (Product product in products)
            {
                table.Rows.Add(
                    product.Id,
                    product.Name,
                    product.Description,
                    product.Category,
                    product.Quantity?.ToString() ?? "—");
            }

            dataGridViewReport.DataSource = table;
            labelCount.Text = $"Найдено: {products.Count}";
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void BuildButton_Click(object? sender, EventArgs e)
    {
        FillCategories();
        BuildReport();
    }

    private static void ShowError(Exception exception)
    {
        MessageBox.Show(
            exception.Message,
            "Ошибка построения отчёта",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
