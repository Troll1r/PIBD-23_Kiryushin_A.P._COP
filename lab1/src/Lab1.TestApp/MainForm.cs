using SelectionControlLibrary;

namespace Lab1.TestApp;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        UpdateStatus();
    }

    private void ButtonAdd_Click(object? sender, EventArgs e)
    {
        try
        {
            string value = textBoxValue.Text;
            int countBefore = comboBoxSelector1.ItemCount;

            comboBoxSelector1.AddValue(value);

            bool added = comboBoxSelector1.ItemCount > countBefore;
            Log(added
                ? $"AddValue(\"{value}\") — значение добавлено."
                : $"AddValue(\"{value}\") — такая строка уже есть, добавление пропущено.");

            UpdateStatus();
        }
        catch (ArgumentException ex)
        {
            MessageBox.Show(
                ex.Message,
                "Некорректное значение",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ButtonDemo_Click(object? sender, EventArgs e)
    {
        string[] values =
        [
            "Значение 1",
            "Значение 2",
            "Значение 3",
            "Значение 4",
            "Значение 5"
        ];

        int countBefore = comboBoxSelector1.ItemCount;

        foreach (string value in values)
        {
            comboBoxSelector1.AddValue(value);
        }

        // Повторная передача уже существующей строки: AddValue не должен создавать дубликат.
        comboBoxSelector1.AddValue("Значение 1");

        int added = comboBoxSelector1.ItemCount - countBefore;
        Log($"Заполнение примером: добавлено новых строк — {added}, дубликаты пропущены.");

        UpdateStatus();
    }

    private void ButtonClear_Click(object? sender, EventArgs e)
    {
        int countBefore = comboBoxSelector1.ItemCount;
        comboBoxSelector1.ClearValues();

        Log($"ClearValues() — удалено строк: {countBefore}.");
        UpdateStatus();
    }

    private void ButtonSet_Click(object? sender, EventArgs e)
    {
        string requested = textBoxSelected.Text;
        comboBoxSelector1.SelectedValue = requested;

        string actual = comboBoxSelector1.SelectedValue;

        if (string.IsNullOrEmpty(requested))
        {
            Log("SelectedValue = \"\" — выбор сброшен.");
        }
        else if (actual == requested)
        {
            Log($"SelectedValue = \"{requested}\" — значение выбрано.");
        }
        else
        {
            Log($"SelectedValue = \"{requested}\" — строки нет в списке, выбор сброшен.");
        }

        UpdateStatus();
    }

    private void ButtonGet_Click(object? sender, EventArgs e)
    {
        string value = comboBoxSelector1.SelectedValue;
        textBoxSelected.Text = value;

        Log($"SelectedValue (get) = \"{value}\".");
        UpdateStatus();
    }

    private void ButtonClearLog_Click(object? sender, EventArgs e)
    {
        listBoxLog.Items.Clear();
    }

    private void ComboBoxSelector_SelectedValueChanged(
        object? sender,
        SelectedValueChangedEventArgs e)
    {
        Log($"SelectedValueChanged: \"{e.OldValue}\" -> \"{e.NewValue}\".");
        UpdateStatus();
    }

    private void Log(string message)
    {
        listBoxLog.Items.Add($"{DateTime.Now:HH:mm:ss}  {message}");
        listBoxLog.TopIndex = listBoxLog.Items.Count - 1;
    }

    private void UpdateStatus()
    {
        labelStatus.Text =
            $"Элементов в списке: {comboBoxSelector1.ItemCount}; " +
            $"выбранное значение: \"{comboBoxSelector1.SelectedValue}\"";
    }
}
