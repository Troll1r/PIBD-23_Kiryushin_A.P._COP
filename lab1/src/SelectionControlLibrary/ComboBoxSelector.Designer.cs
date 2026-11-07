#nullable enable

namespace SelectionControlLibrary;

partial class ComboBoxSelector
{
    private System.ComponentModel.IContainer? components = null;

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором компонентов

    /// <summary>
    /// Обязательный метод для поддержки конструктора — не изменяйте
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
        comboBox = new ComboBox();
        SuspendLayout();
        // 
        // comboBox
        // 
        comboBox.Dock = DockStyle.Fill;
        comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        comboBox.FormattingEnabled = true;
        comboBox.Name = "comboBox";
        comboBox.Size = new Size(320, 28);
        comboBox.TabIndex = 0;
        comboBox.SelectedIndexChanged += ComboBox_SelectedIndexChanged;
        // 
        // ComboBoxSelector
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(comboBox);
        Name = "ComboBoxSelector";
        Size = new Size(320, 32);
        ResumeLayout(false);
    }

    #endregion

    private ComboBox comboBox = null!;
}
