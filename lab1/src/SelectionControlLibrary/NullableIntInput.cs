using System.ComponentModel;

namespace SelectionControlLibrary;

/// <summary>
/// Визуальный компонент ввода целого числа с возможностью указать,
/// что значение не задано (null).
/// </summary>
[ToolboxItem(true)]
[Description("Ввод целого числа с возможностью указать отсутствие значения.")]
public partial class NullableIntInput : UserControl
{
    public NullableIntInput()
    {
        InitializeComponent();
        UpdateEnabledState();
    }

    /// <summary>
    /// Введённое значение. null, если установлен флажок «Нет значения».
    /// </summary>
    [Category("Лабораторная работа 2")]
    [Description("Введённое значение; null, если установлен флажок «Нет значения».")]
    [DefaultValue(null)]
    public int? Value
    {
        get => checkBoxNoValue.Checked ? null : (int)numericUpDownValue.Value;
        set
        {
            if (value.HasValue)
            {
                int clamped = Math.Clamp(
                    value.Value,
                    (int)numericUpDownValue.Minimum,
                    (int)numericUpDownValue.Maximum);

                checkBoxNoValue.Checked = false;
                numericUpDownValue.Value = clamped;
            }
            else
            {
                checkBoxNoValue.Checked = true;
            }

            UpdateEnabledState();
        }
    }

    /// <summary>
    /// Возникает при изменении значения или состояния флажка.
    /// </summary>
    [Category("Лабораторная работа 2")]
    [Description("Возникает при изменении значения или состояния флажка.")]
    public event EventHandler? ValueChanged;

    private void CheckBoxNoValue_CheckedChanged(object? sender, EventArgs e)
    {
        UpdateEnabledState();
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void NumericUpDownValue_ValueChanged(object? sender, EventArgs e)
    {
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateEnabledState()
    {
        numericUpDownValue.Enabled = !checkBoxNoValue.Checked;
    }
}
