#nullable enable

namespace SelectionControlLibrary;

partial class TreeViewPresenter
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
        treeView = new TreeView();
        SuspendLayout();
        // 
        // treeView
        // 
        treeView.Dock = DockStyle.Fill;
        treeView.FullRowSelect = true;
        treeView.HideSelection = false;
        treeView.Location = new Point(0, 0);
        treeView.Name = "treeView";
        treeView.Size = new Size(320, 240);
        treeView.TabIndex = 0;
        // 
        // TreeViewPresenter
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(treeView);
        Name = "TreeViewPresenter";
        Size = new Size(320, 240);
        ResumeLayout(false);
    }

    #endregion

    private TreeView treeView = null!;
}
