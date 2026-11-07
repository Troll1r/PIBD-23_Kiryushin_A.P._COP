using ProductCatalog.Data;

namespace ProductCatalog.Components.SimpleDirectory;

/// <summary>
/// Простой справочник категорий: одна колонка, редактирование прямо в таблице.
/// </summary>
public partial class SimpleDirectoryControl : UserControl
{
    private CategoryRepository _repository = null!;

    public SimpleDirectoryControl()
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

        _repository = new CategoryRepository();
        LoadCategories();
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        if (keyData == Keys.Insert)
        {
            AddNewRow();
            return true;
        }

        if (keyData == Keys.Delete && !dataGridView.IsCurrentCellInEditMode)
        {
            DeleteSelectedRows();
            return true;
        }

        if (keyData == Keys.F5)
        {
            LoadCategories();
            return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    private void LoadCategories()
    {
        try
        {
            List<string> categories = _repository.GetAll();

            dataGridView.Rows.Clear();

            foreach (string category in categories)
            {
                int rowIndex = dataGridView.Rows.Add(category);
                dataGridView.Rows[rowIndex].Tag = category;
            }

            dataGridView.ClearSelection();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private void AddNewRow()
    {
        dataGridView.EndEdit();

        int rowIndex = dataGridView.Rows.Add(string.Empty);

        BeginInvoke(new Action(() =>
        {
            if (rowIndex >= dataGridView.Rows.Count)
            {
                return;
            }

            dataGridView.CurrentCell = dataGridView.Rows[rowIndex].Cells[0];
            dataGridView.FirstDisplayedScrollingRowIndex = rowIndex;
            dataGridView.Focus();
            dataGridView.BeginEdit(true);
        }));
    }

    private void DeleteSelectedRows()
    {
        List<DataGridViewRow> rows = dataGridView.SelectedRows
            .Cast<DataGridViewRow>()
            .ToList();

        if (rows.Count == 0 && dataGridView.CurrentRow != null)
        {
            rows.Add(dataGridView.CurrentRow);
        }

        if (rows.Count == 0)
        {
            return;
        }

        DialogResult confirmation = MessageBox.Show(
            $"Удалить выбранные записи ({rows.Count})?",
            "Подтверждение удаления",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirmation != DialogResult.Yes)
        {
            return;
        }

        foreach (DataGridViewRow row in rows)
        {
            if (row.Tag is string category)
            {
                try
                {
                    _repository.Delete(category);
                }
                catch (Exception ex)
                {
                    ShowError(ex);
                    return;
                }
            }

            dataGridView.Rows.Remove(row);
        }
    }

    private void DataGridView_CellValidating(
        object? sender,
        DataGridViewCellValidatingEventArgs e)
    {
        if (e.ColumnIndex != 0)
        {
            return;
        }

        string value = e.FormattedValue?.ToString()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(value))
        {
            e.Cancel = true;
            dataGridView.Rows[e.RowIndex].ErrorText =
                "Название категории не может быть пустым.";
        }
        else
        {
            dataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
        }
    }

    private void DataGridView_CellEndEdit(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.ColumnIndex != 0 || e.RowIndex < 0)
        {
            return;
        }

        DataGridViewRow row = dataGridView.Rows[e.RowIndex];
        string newName = row.Cells[0].Value?.ToString()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(newName))
        {
            return;
        }

        string? oldName = row.Tag as string;

        try
        {
            if (oldName == null)
            {
                _repository.Add(newName);
                row.Tag = newName;
            }
            else if (!string.Equals(oldName, newName, StringComparison.Ordinal))
            {
                _repository.Update(oldName, newName);
                row.Tag = newName;
            }

            row.Cells[0].Value = newName;
            row.ErrorText = string.Empty;
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ошибка сохранения",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);

            if (oldName != null)
            {
                row.Cells[0].Value = oldName;
            }
            else
            {
                dataGridView.Rows.Remove(row);
            }
        }
    }

    private static void ShowError(Exception exception)
    {
        MessageBox.Show(
            exception.Message,
            "Ошибка работы со справочником",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}
