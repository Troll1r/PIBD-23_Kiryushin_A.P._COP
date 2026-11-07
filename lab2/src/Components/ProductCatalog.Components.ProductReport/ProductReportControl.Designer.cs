#nullable enable

namespace ProductCatalog.Components.ProductReport;

partial class ProductReportControl
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
        panelFilter = new Panel();
        labelCategory = new Label();
        categoryComboBox = new ComboBox();
        buildButton = new Button();
        labelCount = new Label();
        dataGridViewReport = new DataGridView();
        panelFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewReport).BeginInit();
        SuspendLayout();
        // 
        // panelFilter
        // 
        panelFilter.Controls.Add(labelCount);
        panelFilter.Controls.Add(buildButton);
        panelFilter.Controls.Add(categoryComboBox);
        panelFilter.Controls.Add(labelCategory);
        panelFilter.Dock = DockStyle.Top;
        panelFilter.Location = new Point(0, 0);
        panelFilter.Name = "panelFilter";
        panelFilter.Size = new Size(800, 52);
        panelFilter.TabIndex = 0;
        // 
        // labelCategory
        // 
        labelCategory.AutoSize = true;
        labelCategory.Location = new Point(10, 16);
        labelCategory.Name = "labelCategory";
        labelCategory.Size = new Size(84, 20);
        labelCategory.TabIndex = 0;
        labelCategory.Text = "Категория:";
        // 
        // categoryComboBox
        // 
        categoryComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
        categoryComboBox.FormattingEnabled = true;
        categoryComboBox.Location = new Point(90, 12);
        categoryComboBox.Name = "categoryComboBox";
        categoryComboBox.Size = new Size(250, 28);
        categoryComboBox.TabIndex = 1;
        // 
        // buildButton
        // 
        buildButton.Location = new Point(350, 10);
        buildButton.Name = "buildButton";
        buildButton.Size = new Size(130, 30);
        buildButton.TabIndex = 2;
        buildButton.Text = "Сформировать";
        buildButton.UseVisualStyleBackColor = true;
        buildButton.Click += BuildButton_Click;
        // 
        // labelCount
        // 
        labelCount.AutoSize = true;
        labelCount.Location = new Point(500, 16);
        labelCount.Name = "labelCount";
        labelCount.Size = new Size(78, 20);
        labelCount.TabIndex = 3;
        labelCount.Text = "Найдено: 0";
        // 
        // dataGridViewReport
        // 
        dataGridViewReport.AllowUserToAddRows = false;
        dataGridViewReport.AllowUserToDeleteRows = false;
        dataGridViewReport.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridViewReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewReport.Dock = DockStyle.Fill;
        dataGridViewReport.Location = new Point(0, 52);
        dataGridViewReport.Name = "dataGridViewReport";
        dataGridViewReport.ReadOnly = true;
        dataGridViewReport.RowHeadersVisible = false;
        dataGridViewReport.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridViewReport.Size = new Size(800, 398);
        dataGridViewReport.TabIndex = 1;
        // 
        // ProductReportControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(dataGridViewReport);
        Controls.Add(panelFilter);
        Name = "ProductReportControl";
        Size = new Size(800, 450);
        panelFilter.ResumeLayout(false);
        panelFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dataGridViewReport).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private Panel panelFilter = null!;
    private Label labelCategory = null!;
    private ComboBox categoryComboBox = null!;
    private Button buildButton = null!;
    private Label labelCount = null!;
    private DataGridView dataGridViewReport = null!;
}
