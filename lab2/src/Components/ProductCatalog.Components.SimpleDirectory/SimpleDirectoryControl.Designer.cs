#nullable enable

namespace ProductCatalog.Components.SimpleDirectory;

partial class SimpleDirectoryControl
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
        dataGridView = new DataGridView();
        categoryColumn = new DataGridViewTextBoxColumn();
        ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
        SuspendLayout();
        // 
        // dataGridView
        // 
        dataGridView.AllowUserToAddRows = false;
        dataGridView.AllowUserToDeleteRows = false;
        dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridView.Columns.AddRange(new DataGridViewColumn[] { categoryColumn });
        dataGridView.Dock = DockStyle.Fill;
        dataGridView.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
        dataGridView.Location = new Point(0, 0);
        dataGridView.MultiSelect = true;
        dataGridView.Name = "dataGridView";
        dataGridView.RowHeadersVisible = false;
        dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dataGridView.Size = new Size(400, 300);
        dataGridView.TabIndex = 0;
        dataGridView.CellEndEdit += DataGridView_CellEndEdit;
        dataGridView.CellValidating += DataGridView_CellValidating;
        // 
        // categoryColumn
        // 
        categoryColumn.HeaderText = "Категория";
        categoryColumn.Name = "CategoryColumn";
        // 
        // SimpleDirectoryControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(dataGridView);
        Name = "SimpleDirectoryControl";
        Size = new Size(400, 300);
        ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private DataGridView dataGridView = null!;
    private DataGridViewTextBoxColumn categoryColumn = null!;
}
