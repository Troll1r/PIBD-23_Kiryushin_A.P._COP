using ProductCatalog.Data;

namespace ProductCatalog.Components.ProductDirectory;

/// <summary>
/// Форма добавления и редактирования продукта.
/// </summary>
public partial class ProductEditForm : Form
{
    private Product? _product;
    private CategoryRepository _categoryRepository = null!;
    private string _initialName = string.Empty;
    private string _initialDescription = string.Empty;
    private string _initialCategory = string.Empty;
    private int? _initialQuantity;
    private bool _saved;

    public ProductEditForm()
    {
        InitializeComponent();
    }

    public ProductEditForm(Product? product) : this()
    {
        _product = product;
        Text = product == null ? "Добавление продукта" : "Редактирование продукта";

        _categoryRepository = new CategoryRepository();
        LoadCategories();

        if (product != null)
        {
            nameTextBox.Text = product.Name;
            descriptionTextBox.Text = product.Description;
            quantityInput.Value = product.Quantity;
        }

        CaptureInitialState();
    }

    /// <summary>
    /// Результат редактирования. Заполняется только при сохранении.
    /// </summary>
    public Product? Result { get; private set; }

    private void LoadCategories()
    {
        try
        {
            foreach (string category in _categoryRepository.GetAll())
            {
                categorySelector.AddValue(category);
            }

            string? currentCategory = _product?.Category;

            if (!string.IsNullOrWhiteSpace(currentCategory))
            {
                categorySelector.AddValue(currentCategory);
                categorySelector.SelectedValue = currentCategory;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка загрузки категорий",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }

    private void CaptureInitialState()
    {
        _initialName = nameTextBox.Text.Trim();
        _initialDescription = descriptionTextBox.Text.Trim();
        _initialCategory = categorySelector.SelectedValue;
        _initialQuantity = quantityInput.Value;
    }

    private bool IsDirty()
    {
        return !string.Equals(nameTextBox.Text.Trim(), _initialName, StringComparison.Ordinal)
            || !string.Equals(descriptionTextBox.Text.Trim(), _initialDescription, StringComparison.Ordinal)
            || !string.Equals(categorySelector.SelectedValue, _initialCategory, StringComparison.Ordinal)
            || quantityInput.Value != _initialQuantity;
    }

    private void ProductEditForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (_saved || !IsDirty())
        {
            return;
        }

        DialogResult result = MessageBox.Show(
            "Есть несохранённые изменения. Закрыть форму без сохранения?",
            "Несохранённые изменения",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (result != DialogResult.Yes)
        {
            e.Cancel = true;
        }
    }

    private void SaveButton_Click(object? sender, EventArgs e)
    {
        string name = nameTextBox.Text.Trim();
        string category = categorySelector.SelectedValue;

        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show(
                "Введите название продукта.",
                "Проверка данных",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            nameTextBox.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(category))
        {
            MessageBox.Show(
                "Выберите категорию продукта.",
                "Проверка данных",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            categorySelector.Focus();
            return;
        }

        Result = new Product
        {
            Id = _product?.Id ?? 0,
            Name = name,
            Description = descriptionTextBox.Text.Trim(),
            Category = category,
            Quantity = quantityInput.Value
        };

        _saved = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void CancelButton_Click(object? sender, EventArgs e)
    {
        Close();
    }
}
