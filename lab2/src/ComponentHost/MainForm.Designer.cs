#nullable enable

namespace ComponentHost;

partial class MainForm
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
        menuStrip = new MenuStrip();
        directoriesToolStripMenuItem = new ToolStripMenuItem();
        reportsToolStripMenuItem = new ToolStripMenuItem();
        tabControl = new TabControl();
        menuStrip.SuspendLayout();
        SuspendLayout();
        // 
        // menuStrip
        // 
        menuStrip.Items.AddRange(new ToolStripItem[] { directoriesToolStripMenuItem, reportsToolStripMenuItem });
        menuStrip.Location = new Point(0, 0);
        menuStrip.Name = "menuStrip";
        menuStrip.Size = new Size(1000, 28);
        menuStrip.TabIndex = 0;
        menuStrip.Text = "menuStrip";
        // 
        // directoriesToolStripMenuItem
        // 
        directoriesToolStripMenuItem.Name = "directoriesToolStripMenuItem";
        directoriesToolStripMenuItem.Size = new Size(108, 24);
        directoriesToolStripMenuItem.Text = "Справочники";
        // 
        // reportsToolStripMenuItem
        // 
        reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
        reportsToolStripMenuItem.Size = new Size(78, 24);
        reportsToolStripMenuItem.Text = "Отчеты";
        // 
        // tabControl
        // 
        tabControl.Dock = DockStyle.Fill;
        tabControl.Location = new Point(0, 28);
        tabControl.Name = "tabControl";
        tabControl.Size = new Size(1000, 572);
        tabControl.TabIndex = 1;
        tabControl.DoubleClick += TabControl_DoubleClick;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1000, 600);
        Controls.Add(tabControl);
        Controls.Add(menuStrip);
        MainMenuStrip = menuStrip;
        MinimumSize = new Size(800, 500);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Учет продуктов";
        menuStrip.ResumeLayout(false);
        menuStrip.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip = null!;
    private ToolStripMenuItem directoriesToolStripMenuItem = null!;
    private ToolStripMenuItem reportsToolStripMenuItem = null!;
    private TabControl tabControl = null!;
}
