using System.ComponentModel;

namespace SelectionControlLibrary;

/// <summary>
/// Визуальный компонент выбора — выпадающий список строк.
/// Вариант 13: список заполняется методом, принимающим непустую строку.
/// </summary>
[ToolboxItem(true)]
[Description("Выпадающий список строк с возможностью выбора одного значения.")]
public partial class ComboBoxSelector : UserControl
{
    private string _previousSelectedValue = string.Empty;

    /// <summary>
    /// Создаёт компонент и инициализирует дочерние элементы управления.
    /// </summary>
    public ComboBoxSelector()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Выбранное значение. Если ничего не выбрано, возвращается пустая строка.
    /// При установке выбирает строку, если она есть в списке; иначе сбрасывает выбор.
    /// </summary>
    [Category("Лабораторная работа 1")]
    [Description("Выбранное значение. Пустая строка, если ничего не выбрано.")]
    [DefaultValue("")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string SelectedValue
    {
        get => comboBox.SelectedItem as string ?? string.Empty;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                comboBox.SelectedIndex = -1;
                return;
            }

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (string.Equals(comboBox.Items[i]?.ToString(), value, StringComparison.Ordinal))
                {
                    comboBox.SelectedIndex = i;
                    return;
                }
            }

            // Строки нет в списке — выбранного значения не остаётся.
            comboBox.SelectedIndex = -1;
        }
    }

    /// <summary>
    /// Количество строк в списке.
    /// </summary>
    [Browsable(false)]
    public int ItemCount => comboBox.Items.Count;

    /// <summary>
    /// Возникает при изменении выбранного значения в выпадающем списке.
    /// </summary>
    [Category("Лабораторная работа 1")]
    [Description("Возникает при изменении выбранного значения.")]
    public event EventHandler<SelectedValueChangedEventArgs>? SelectedValueChanged;

    /// <summary>
    /// Добавляет непустую строку в список, если такой строки ещё нет.
    /// </summary>
    /// <param name="value">Добавляемое значение.</param>
    /// <exception cref="ArgumentException">
    /// Выбрасывается, если значение <see langword="null"/>, пустое или состоит только из пробелов.
    /// </exception>
    public void AddValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Значение не может быть пустым или состоять только из пробелов.",
                nameof(value));
        }

        if (!comboBox.Items.Contains(value))
        {
            comboBox.Items.Add(value);
        }
    }

    /// <summary>
    /// Очищает список значений.
    /// </summary>
    public void ClearValues()
    {
        comboBox.Items.Clear();
    }

    private void ComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        string newValue = SelectedValue;

        if (_previousSelectedValue == newValue)
        {
            return;
        }

        string oldValue = _previousSelectedValue;
        _previousSelectedValue = newValue;

        SelectedValueChanged?.Invoke(
            this,
            new SelectedValueChangedEventArgs(oldValue, newValue));
    }
}
