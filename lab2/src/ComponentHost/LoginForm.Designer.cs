#nullable enable

namespace ComponentHost;

partial class LoginForm
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
        labelLogin = new Label();
        loginTextBox = new TextBox();
        labelRoles = new Label();
        okButton = new Button();
        cancelButton = new Button();
        SuspendLayout();
        // 
        // labelLogin
        // 
        labelLogin.AutoSize = true;
        labelLogin.Location = new Point(20, 25);
        labelLogin.Name = "labelLogin";
        labelLogin.Size = new Size(58, 20);
        labelLogin.TabIndex = 0;
        labelLogin.Text = "Логин:";
        // 
        // loginTextBox
        // 
        loginTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        loginTextBox.Location = new Point(100, 22);
        loginTextBox.Name = "loginTextBox";
        loginTextBox.Size = new Size(300, 27);
        loginTextBox.TabIndex = 1;
        // 
        // labelRoles
        // 
        labelRoles.AutoSize = true;
        labelRoles.ForeColor = SystemColors.GrayText;
        labelRoles.Location = new Point(20, 70);
        labelRoles.Name = "labelRoles";
        labelRoles.Size = new Size(323, 80);
        labelRoles.TabIndex = 2;
        labelRoles.Text = "Роли:\r\nadmin — полный доступ\r\nuser — справочники\r\nостальные — только категории";
        // 
        // okButton
        // 
        okButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        okButton.Location = new Point(220, 170);
        okButton.Name = "okButton";
        okButton.Size = new Size(95, 32);
        okButton.TabIndex = 3;
        okButton.Text = "Войти";
        okButton.UseVisualStyleBackColor = true;
        okButton.Click += OkButton_Click;
        // 
        // cancelButton
        // 
        cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        cancelButton.DialogResult = DialogResult.Cancel;
        cancelButton.Location = new Point(325, 170);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(95, 32);
        cancelButton.TabIndex = 4;
        cancelButton.Text = "Отмена";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += CancelButton_Click;
        // 
        // LoginForm
        // 
        AcceptButton = okButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(440, 220);
        Controls.Add(cancelButton);
        Controls.Add(okButton);
        Controls.Add(labelRoles);
        Controls.Add(loginTextBox);
        Controls.Add(labelLogin);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "LoginForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Вход в систему";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelLogin = null!;
    private TextBox loginTextBox = null!;
    private Label labelRoles = null!;
    private Button okButton = null!;
    private Button cancelButton = null!;
}
