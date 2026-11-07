#nullable enable

namespace ProductCatalog.Components.ProductDirectory;

partial class ProductDirectoryControl
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
        treeViewPresenter1 = new SelectionControlLibrary.TreeViewPresenter();
        SuspendLayout();
        // 
        // treeViewPresenter1
        // 
        treeViewPresenter1.Dock = DockStyle.Fill;
        treeViewPresenter1.Location = new Point(0, 0);
        treeViewPresenter1.Name = "treeViewPresenter1";
        treeViewPresenter1.Size = new Size(500, 400);
        treeViewPresenter1.TabIndex = 0;
        // 
        // ProductDirectoryControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(treeViewPresenter1);
        Name = "ProductDirectoryControl";
        Size = new Size(500, 400);
        ResumeLayout(false);
    }

    #endregion

    private SelectionControlLibrary.TreeViewPresenter treeViewPresenter1 = null!;
}
