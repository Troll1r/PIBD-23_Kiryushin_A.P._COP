#nullable enable

namespace ComponentHost;

partial class ExtensionsForm
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

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Обязательный метод для поддержки конструктора — не изменяйте
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
        textGroupBox = new GroupBox();
        textComboBox = new ComboBox();
        textButton = new Button();
        lineChartGroupBox = new GroupBox();
        lineChartComboBox = new ComboBox();
        lineChartButton = new Button();
        pieChartGroupBox = new GroupBox();
        pieChartComboBox = new ComboBox();
        pieChartButton = new Button();
        labelHint = new Label();
        closeButton = new Button();
        textGroupBox.SuspendLayout();
        lineChartGroupBox.SuspendLayout();
        pieChartGroupBox.SuspendLayout();
        SuspendLayout();
        // 
        // textGroupBox
        // 
        textGroupBox.Controls.Add(textButton);
        textGroupBox.Controls.Add(textComboBox);
        textGroupBox.Location = new Point(20, 20);
        textGroupBox.Name = "textGroupBox";
        textGroupBox.Size = new Size(640, 130);
        textGroupBox.TabIndex = 0;
        textGroupBox.TabStop = false;
        textGroupBox.Text = "Документ с текстом (Excel)";
        // 
        // textComboBox
        // 
        textComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        textComboBox.FormattingEnabled = true;
        textComboBox.Location = new Point(15, 35);
        textComboBox.Name = "textComboBox";
        textComboBox.Size = new Size(320, 28);
        textComboBox.TabIndex = 0;
        // 
        // textButton
        // 
        textButton.Enabled = false;
        textButton.Location = new Point(350, 33);
        textButton.Name = "textButton";
        textButton.Size = new Size(270, 32);
        textButton.TabIndex = 1;
        textButton.Text = "Создать документ...";
        textButton.UseVisualStyleBackColor = true;
        textButton.Click += TextButton_Click;
        // 
        // lineChartGroupBox
        // 
        lineChartGroupBox.Controls.Add(lineChartButton);
        lineChartGroupBox.Controls.Add(lineChartComboBox);
        lineChartGroupBox.Location = new Point(20, 160);
        lineChartGroupBox.Name = "lineChartGroupBox";
        lineChartGroupBox.Size = new Size(640, 130);
        lineChartGroupBox.TabIndex = 1;
        lineChartGroupBox.TabStop = false;
        lineChartGroupBox.Text = "Документ с линейной диаграммой (Excel)";
        // 
        // lineChartComboBox
        // 
        lineChartComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        lineChartComboBox.FormattingEnabled = true;
        lineChartComboBox.Location = new Point(15, 35);
        lineChartComboBox.Name = "lineChartComboBox";
        lineChartComboBox.Size = new Size(320, 28);
        lineChartComboBox.TabIndex = 0;
        // 
        // lineChartButton
        // 
        lineChartButton.Enabled = false;
        lineChartButton.Location = new Point(350, 33);
        lineChartButton.Name = "lineChartButton";
        lineChartButton.Size = new Size(270, 32);
        lineChartButton.TabIndex = 1;
        lineChartButton.Text = "Создать документ...";
        lineChartButton.UseVisualStyleBackColor = true;
        lineChartButton.Click += LineChartButton_Click;
        // 
        // pieChartGroupBox
        // 
        pieChartGroupBox.Controls.Add(pieChartButton);
        pieChartGroupBox.Controls.Add(pieChartComboBox);
        pieChartGroupBox.Location = new Point(20, 300);
        pieChartGroupBox.Name = "pieChartGroupBox";
        pieChartGroupBox.Size = new Size(640, 130);
        pieChartGroupBox.TabIndex = 2;
        pieChartGroupBox.TabStop = false;
        pieChartGroupBox.Text = "Документ с круговой диаграммой (PDF)";
        // 
        // pieChartComboBox
        // 
        pieChartComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        pieChartComboBox.FormattingEnabled = true;
        pieChartComboBox.Location = new Point(15, 35);
        pieChartComboBox.Name = "pieChartComboBox";
        pieChartComboBox.Size = new Size(320, 28);
        pieChartComboBox.TabIndex = 0;
        // 
        // pieChartButton
        // 
        pieChartButton.Enabled = false;
        pieChartButton.Location = new Point(350, 33);
        pieChartButton.Name = "pieChartButton";
        pieChartButton.Size = new Size(270, 32);
        pieChartButton.TabIndex = 1;
        pieChartButton.Text = "Создать документ...";
        pieChartButton.UseVisualStyleBackColor = true;
        pieChartButton.Click += PieChartButton_Click;
        // 
        // labelHint
        // 
        labelHint.AutoSize = true;
        labelHint.ForeColor = SystemColors.GrayText;
        labelHint.Location = new Point(20, 445);
        labelHint.Name = "labelHint";
        labelHint.Size = new Size(486, 20);
        labelHint.TabIndex = 3;
        labelHint.Text = "Плагины загружаются из папки, указанной в appsettings.json (PluginDirectory).";
        // 
        // closeButton
        // 
        closeButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        closeButton.DialogResult = DialogResult.Cancel;
        closeButton.Location = new Point(565, 470);
        closeButton.Name = "closeButton";
        closeButton.Size = new Size(95, 30);
        closeButton.TabIndex = 4;
        closeButton.Text = "Закрыть";
        closeButton.UseVisualStyleBackColor = true;
        // 
        // ExtensionsForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = closeButton;
        ClientSize = new Size(680, 520);
        Controls.Add(closeButton);
        Controls.Add(labelHint);
        Controls.Add(pieChartGroupBox);
        Controls.Add(lineChartGroupBox);
        Controls.Add(textGroupBox);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ExtensionsForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Расширения";
        textGroupBox.ResumeLayout(false);
        lineChartGroupBox.ResumeLayout(false);
        pieChartGroupBox.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private GroupBox textGroupBox = null!;
    private ComboBox textComboBox = null!;
    private Button textButton = null!;
    private GroupBox lineChartGroupBox = null!;
    private ComboBox lineChartComboBox = null!;
    private Button lineChartButton = null!;
    private GroupBox pieChartGroupBox = null!;
    private ComboBox pieChartComboBox = null!;
    private Button pieChartButton = null!;
    private Label labelHint = null!;
    private Button closeButton = null!;
}
