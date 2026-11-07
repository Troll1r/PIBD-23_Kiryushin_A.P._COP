#nullable enable

namespace ProductCatalog.Components.ProductDirectory;

partial class ProductEditForm
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
        labelName = new Label();
        nameTextBox = new TextBox();
        labelDescription = new Label();
        descriptionTextBox = new TextBox();
        labelCategory = new Label();
        categorySelector = new SelectionControlLibrary.ComboBoxSelector();
        labelQuantity = new Label();
        quantityInput = new SelectionControlLibrary.NullableIntInput();
        labelHint = new Label();
        saveButton = new Button();
        cancelButton = new Button();
        SuspendLayout();
        // 
        // labelName
        // 
        labelName.AutoSize = true;
        labelName.Location = new Point(20, 23);
        labelName.Name = "labelName";
        labelName.Size = new Size(72, 20);
        labelName.TabIndex = 0;
        labelName.Text = "Название:";
        // 
        // nameTextBox
        // 
        nameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        nameTextBox.Location = new Point(150, 20);
        nameTextBox.Name = "nameTextBox";
        nameTextBox.Size = new Size(340, 27);
        nameTextBox.TabIndex = 1;
        // 
        // labelDescription
        // 
        labelDescription.AutoSize = true;
        labelDescription.Location = new Point(20, 63);
        labelDescription.Name = "labelDescription";
        labelDescription.Size = new Size(75, 20);
        labelDescription.TabIndex = 2;
        labelDescription.Text = "Описание:";
        // 
        // descriptionTextBox
        // 
        descriptionTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        descriptionTextBox.Location = new Point(150, 60);
        descriptionTextBox.Multiline = true;
        descriptionTextBox.Name = "descriptionTextBox";
        descriptionTextBox.ScrollBars = ScrollBars.Vertical;
        descriptionTextBox.Size = new Size(340, 100);
        descriptionTextBox.TabIndex = 3;
        // 
        // labelCategory
        // 
        labelCategory.AutoSize = true;
        labelCategory.Location = new Point(20, 173);
        labelCategory.Name = "labelCategory";
        labelCategory.Size = new Size(84, 20);
        labelCategory.TabIndex = 4;
        labelCategory.Text = "Категория:";
        // 
        // categorySelector
        // 
        categorySelector.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        categorySelector.Location = new Point(150, 170);
        categorySelector.Name = "categorySelector";
        categorySelector.Size = new Size(340, 34);
        categorySelector.TabIndex = 5;
        // 
        // labelQuantity
        // 
        labelQuantity.AutoSize = true;
        labelQuantity.Location = new Point(20, 218);
        labelQuantity.Name = "labelQuantity";
        labelQuantity.Size = new Size(90, 20);
        labelQuantity.TabIndex = 6;
        labelQuantity.Text = "Количество:";
        // 
        // quantityInput
        // 
        quantityInput.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        quantityInput.Location = new Point(150, 215);
        quantityInput.Name = "quantityInput";
        quantityInput.Size = new Size(340, 30);
        quantityInput.TabIndex = 7;
        // 
        // labelHint
        // 
        labelHint.AutoSize = true;
        labelHint.ForeColor = SystemColors.GrayText;
        labelHint.Location = new Point(150, 255);
        labelHint.Name = "labelHint";
        labelHint.Size = new Size(314, 20);
        labelHint.TabIndex = 8;
        labelHint.Text = "Название и категория обязательны. Количество можно не указывать.";
        // 
        // saveButton
        // 
        saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        saveButton.Location = new Point(295, 300);
        saveButton.Name = "saveButton";
        saveButton.Size = new Size(95, 32);
        saveButton.TabIndex = 9;
        saveButton.Text = "Сохранить";
        saveButton.UseVisualStyleBackColor = true;
        saveButton.Click += SaveButton_Click;
        // 
        // cancelButton
        // 
        cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        cancelButton.Location = new Point(395, 300);
        cancelButton.Name = "cancelButton";
        cancelButton.Size = new Size(95, 32);
        cancelButton.TabIndex = 10;
        cancelButton.Text = "Отмена";
        cancelButton.UseVisualStyleBackColor = true;
        cancelButton.Click += CancelButton_Click;
        // 
        // ProductEditForm
        // 
        AcceptButton = saveButton;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        CancelButton = cancelButton;
        ClientSize = new Size(520, 360);
        Controls.Add(cancelButton);
        Controls.Add(saveButton);
        Controls.Add(labelHint);
        Controls.Add(quantityInput);
        Controls.Add(labelQuantity);
        Controls.Add(categorySelector);
        Controls.Add(labelCategory);
        Controls.Add(descriptionTextBox);
        Controls.Add(labelDescription);
        Controls.Add(nameTextBox);
        Controls.Add(labelName);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "ProductEditForm";
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Продукт";
        FormClosing += ProductEditForm_FormClosing;
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelName = null!;
    private TextBox nameTextBox = null!;
    private Label labelDescription = null!;
    private TextBox descriptionTextBox = null!;
    private Label labelCategory = null!;
    private SelectionControlLibrary.ComboBoxSelector categorySelector = null!;
    private Label labelQuantity = null!;
    private SelectionControlLibrary.NullableIntInput quantityInput = null!;
    private Label labelHint = null!;
    private Button saveButton = null!;
    private Button cancelButton = null!;
}
