#nullable enable

namespace SelectionControlLibrary;

partial class NullableIntInput
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
        checkBoxNoValue = new CheckBox();
        numericUpDownValue = new NumericUpDown();
        ((System.ComponentModel.ISupportInitialize)numericUpDownValue).BeginInit();
        SuspendLayout();
        // 
        // checkBoxNoValue
        // 
        checkBoxNoValue.AutoSize = true;
        checkBoxNoValue.Location = new Point(0, 6);
        checkBoxNoValue.Name = "checkBoxNoValue";
        checkBoxNoValue.Size = new Size(115, 24);
        checkBoxNoValue.TabIndex = 0;
        checkBoxNoValue.Text = "Нет значения";
        checkBoxNoValue.UseVisualStyleBackColor = true;
        checkBoxNoValue.CheckedChanged += CheckBoxNoValue_CheckedChanged;
        // 
        // numericUpDownValue
        // 
        numericUpDownValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        numericUpDownValue.Location = new Point(130, 3);
        numericUpDownValue.Maximum = new decimal(new int[] { 1000000, 0, 0, 0 });
        numericUpDownValue.Name = "numericUpDownValue";
        numericUpDownValue.Size = new Size(170, 27);
        numericUpDownValue.TabIndex = 1;
        numericUpDownValue.ThousandsSeparator = true;
        numericUpDownValue.ValueChanged += NumericUpDownValue_ValueChanged;
        // 
        // NullableIntInput
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(numericUpDownValue);
        Controls.Add(checkBoxNoValue);
        Name = "NullableIntInput";
        Size = new Size(300, 30);
        ((System.ComponentModel.ISupportInitialize)numericUpDownValue).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private CheckBox checkBoxNoValue = null!;
    private NumericUpDown numericUpDownValue = null!;
}
