namespace SelectionControlLibrary;

/// <summary>
/// Параметры события <see cref="ComboBoxSelector.SelectedValueChanged"/>.
/// </summary>
public class SelectedValueChangedEventArgs : EventArgs
{
    /// <summary>
    /// Создаёт аргументы события смены выбранного значения.
    /// </summary>
    /// <param name="oldValue">Предыдущее выбранное значение.</param>
    /// <param name="newValue">Новое выбранное значение.</param>
    public SelectedValueChangedEventArgs(string oldValue, string newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }

    /// <summary>
    /// Предыдущее выбранное значение (пустая строка, если выбора не было).
    /// </summary>
    public string OldValue { get; }

    /// <summary>
    /// Новое выбранное значение (пустая строка, если выбор сброшен).
    /// </summary>
    public string NewValue { get; }
}
