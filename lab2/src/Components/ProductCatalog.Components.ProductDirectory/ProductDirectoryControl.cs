using ProductCatalog.Data;

namespace ProductCatalog.Components.ProductDirectory;

/// <summary>
/// Сложный справочник продуктов: дерево и операции через контекстное меню.
/// </summary>
public partial class ProductDirectoryControl : UserControl
{
    private ProductRepository _productRepository = null!;
    private CategoryRepository _categoryRepository = null!;
    private ContextMenuStrip _contextMenu = null!;
    private ToolStripMenuItem _editMenuItem = null!;
    private ToolStripMenuItem _deleteMenuItem = null!;

    public ProductDirectoryControl()
    {
        InitializeComponent();
        BuildContextMenu();
        treeViewPresenter1.NodeContextMenu = _contextMenu;
        treeViewPresenter1.TreeKeyDown += TreeViewPresenter_TreeKeyDown;
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
        treeViewPresenter1.SetHierarchy("Category", "Quantity", "Name", "Id");
        ReloadProducts();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == (Keys.Control | Keys.A))
        {
            AddProduct();
            return true;
        }

        if (keyData == (Keys.Control | Keys.U))
        {
            EditProduct();
            return true;
        }

        if (keyData == (Keys.Control | Keys.D))
        {
            DeleteProduct();
            return true;
        }

        if (keyData == Keys.F5)
        {
            ReloadProducts();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void TreeViewPresenter_TreeKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.A)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            AddProduct();
            return;
        }

        if (e.Control && e.KeyCode == Keys.U)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            EditProduct();
            return;
        }

        if (e.Control && e.KeyCode == Keys.D)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            DeleteProduct();
            return;
        }

        if (e.KeyCode == Keys.F5)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            ReloadProducts();
        }
    }

    private void BuildContextMenu()
    {
        var addMenuItem = new ToolStripMenuItem("Добавить")
        {
            ShortcutKeyDisplayString = "Ctrl+A"
        };
        _editMenuItem = new ToolStripMenuItem("Редактировать")
        {
            ShortcutKeyDisplayString = "Ctrl+U"
        };
        _deleteMenuItem = new ToolStripMenuItem("Удалить")
        {
            ShortcutKeyDisplayString = "Ctrl+D"
        };
        var refreshMenuItem = new ToolStripMenuItem("Обновить")
        {
            ShortcutKeyDisplayString = "F5"
        };

        addMenuItem.Click += (_, _) => AddProduct();
        _editMenuItem.Click += (_, _) => EditProduct();
        _deleteMenuItem.Click += (_, _) => DeleteProduct();
        refreshMenuItem.Click += (_, _) => ReloadProducts();

        _contextMenu = new ContextMenuStrip();
        _contextMenu.Items.AddRange(
        [
            addMenuItem,
            _editMenuItem,
            _deleteMenuItem,
            new ToolStripSeparator(),
            refreshMenuItem
        ]);
        _contextMenu.Opening += ContextMenu_Opening;
    }

    private void ContextMenu_Opening(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        bool hasSelectedProduct = treeViewPresenter1.GetSelectedItem() is Product;
        _editMenuItem.Enabled = hasSelectedProduct;
        _deleteMenuItem.Enabled = hasSelectedProduct;
    }

    private void AddProduct()
    {
        try
        {
            if (_categoryRepository.GetAll().Count == 0)
            {
                MessageBox.Show(
                    "Сначала добавьте хотя бы одну категорию в справочнике «Категории продуктов».",
                    "Нет категорий",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
        }
        catch (Exception ex)
        {
            ShowError(ex);
            return;
        }

        using var form = new ProductEditForm(null);

        if (form.ShowDialog(this) != DialogResult.OK || form.Result == null)
        {
            return;
        }

        try
        {
            _productRepository.Add(form.Result);
            ReloadProducts();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void EditProduct()
    {
        if (treeViewPresenter1.GetSelectedItem() is not Product product)
        {
            MessageBox.Show(
                "Выберите продукт — конечный узел дерева с идентификатором.",
                "Продукт не выбран",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        using var form = new ProductEditForm(product);

        if (form.ShowDialog(this) != DialogResult.OK || form.Result == null)
        {
            return;
        }

        try
        {
            _productRepository.Update(form.Result);
            ReloadProducts();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void DeleteProduct()
    {
        if (treeViewPresenter1.GetSelectedItem() is not Product product)
        {
            MessageBox.Show(
                "Выберите продукт — конечный узел дерева с идентификатором.",
                "Продукт не выбран",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        DialogResult confirmation = MessageBox.Show(
            $"Удалить продукт «{product.Name}»?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmation != DialogResult.Yes)
        {
            return;
        }

        try
        {
            _productRepository.Delete(product.Id);
            ReloadProducts();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void ReloadProducts()
    {
        try
        {
            List<Product> products = _productRepository.GetAll();
            treeViewPresenter1.SetItems(products);
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private static void ShowError(Exception exception)
    {
        MessageBox.Show(
            exception.Message,
            "Ошибка работы со справочником продуктов",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
