#nullable enable

namespace Lab1.TestApp;

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
        labelTitle = new Label();
        labelComponent = new Label();
        comboBoxSelector1 = new SelectionControlLibrary.ComboBoxSelector();
        groupBoxAdd = new GroupBox();
        labelValue = new Label();
        textBoxValue = new TextBox();
        buttonAdd = new Button();
        buttonDemo = new Button();
        buttonClear = new Button();
        groupBoxSelected = new GroupBox();
        labelSelected = new Label();
        textBoxSelected = new TextBox();
        buttonSet = new Button();
        buttonGet = new Button();
        labelHint = new Label();
        groupBoxLog = new GroupBox();
        listBoxLog = new ListBox();
        buttonClearLog = new Button();
        labelStatus = new Label();
        groupBoxAdd.SuspendLayout();
        groupBoxSelected.SuspendLayout();
        groupBoxLog.SuspendLayout();
        SuspendLayout();
        // 
        // labelTitle
        // 
        labelTitle.AutoSize = true;
        labelTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        labelTitle.Location = new Point(20, 12);
        labelTitle.Name = "labelTitle";
        labelTitle.Size = new Size(369, 21);
        labelTitle.TabIndex = 0;
        labelTitle.Text = "Лабораторная работа №1 — вариант 13 (оценка «3»)";
        // 
        // labelComponent
        // 
        labelComponent.AutoSize = true;
        labelComponent.Location = new Point(20, 48);
        labelComponent.Name = "labelComponent";
        labelComponent.Size = new Size(364, 20);
        labelComponent.TabIndex = 1;
        labelComponent.Text = "Компонент ComboBoxSelector (выпадающий список):";
        // 
        // comboBoxSelector1
        // 
        comboBoxSelector1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        comboBoxSelector1.Location = new Point(20, 74);
        comboBoxSelector1.Name = "comboBoxSelector1";
        comboBoxSelector1.Size = new Size(620, 34);
        comboBoxSelector1.TabIndex = 2;
        comboBoxSelector1.SelectedValueChanged += ComboBoxSelector_SelectedValueChanged;
        // 
        // groupBoxAdd
        // 
        groupBoxAdd.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxAdd.Controls.Add(labelValue);
        groupBoxAdd.Controls.Add(textBoxValue);
        groupBoxAdd.Controls.Add(buttonAdd);
        groupBoxAdd.Controls.Add(buttonDemo);
        groupBoxAdd.Controls.Add(buttonClear);
        groupBoxAdd.Location = new Point(20, 124);
        groupBoxAdd.Name = "groupBoxAdd";
        groupBoxAdd.Size = new Size(620, 130);
        groupBoxAdd.TabIndex = 3;
        groupBoxAdd.TabStop = false;
        groupBoxAdd.Text = "Добавление значения — метод AddValue(string)";
        // 
        // labelValue
        // 
        labelValue.AutoSize = true;
        labelValue.Location = new Point(15, 34);
        labelValue.Name = "labelValue";
        labelValue.Size = new Size(72, 20);
        labelValue.TabIndex = 0;
        labelValue.Text = "Значение:";
        // 
        // textBoxValue
        // 
        textBoxValue.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxValue.Location = new Point(95, 31);
        textBoxValue.Name = "textBoxValue";
        textBoxValue.Size = new Size(315, 27);
        textBoxValue.TabIndex = 1;
        // 
        // buttonAdd
        // 
        buttonAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonAdd.Location = new Point(430, 29);
        buttonAdd.Name = "buttonAdd";
        buttonAdd.Size = new Size(170, 32);
        buttonAdd.TabIndex = 2;
        buttonAdd.Text = "Добавить";
        buttonAdd.UseVisualStyleBackColor = true;
        buttonAdd.Click += ButtonAdd_Click;
        // 
        // buttonDemo
        // 
        buttonDemo.Location = new Point(95, 76);
        buttonDemo.Name = "buttonDemo";
        buttonDemo.Size = new Size(220, 34);
        buttonDemo.TabIndex = 3;
        buttonDemo.Text = "Заполнить примером";
        buttonDemo.UseVisualStyleBackColor = true;
        buttonDemo.Click += ButtonDemo_Click;
        // 
        // buttonClear
        // 
        buttonClear.Location = new Point(330, 76);
        buttonClear.Name = "buttonClear";
        buttonClear.Size = new Size(270, 34);
        buttonClear.TabIndex = 4;
        buttonClear.Text = "Очистить список (ClearValues)";
        buttonClear.UseVisualStyleBackColor = true;
        buttonClear.Click += ButtonClear_Click;
        // 
        // groupBoxSelected
        // 
        groupBoxSelected.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxSelected.Controls.Add(labelSelected);
        groupBoxSelected.Controls.Add(textBoxSelected);
        groupBoxSelected.Controls.Add(buttonSet);
        groupBoxSelected.Controls.Add(buttonGet);
        groupBoxSelected.Controls.Add(labelHint);
        groupBoxSelected.Location = new Point(20, 268);
        groupBoxSelected.Name = "groupBoxSelected";
        groupBoxSelected.Size = new Size(620, 130);
        groupBoxSelected.TabIndex = 4;
        groupBoxSelected.TabStop = false;
        groupBoxSelected.Text = "Свойство SelectedValue (set / get)";
        // 
        // labelSelected
        // 
        labelSelected.AutoSize = true;
        labelSelected.Location = new Point(15, 34);
        labelSelected.Name = "labelSelected";
        labelSelected.Size = new Size(72, 20);
        labelSelected.TabIndex = 0;
        labelSelected.Text = "Значение:";
        // 
        // textBoxSelected
        // 
        textBoxSelected.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        textBoxSelected.Location = new Point(95, 31);
        textBoxSelected.Name = "textBoxSelected";
        textBoxSelected.Size = new Size(315, 27);
        textBoxSelected.TabIndex = 1;
        // 
        // buttonSet
        // 
        buttonSet.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        buttonSet.Location = new Point(430, 29);
        buttonSet.Name = "buttonSet";
        buttonSet.Size = new Size(170, 32);
        buttonSet.TabIndex = 2;
        buttonSet.Text = "Установить (set)";
        buttonSet.UseVisualStyleBackColor = true;
        buttonSet.Click += ButtonSet_Click;
        // 
        // buttonGet
        // 
        buttonGet.Location = new Point(95, 76);
        buttonGet.Name = "buttonGet";
        buttonGet.Size = new Size(220, 34);
        buttonGet.TabIndex = 3;
        buttonGet.Text = "Получить (get)";
        buttonGet.UseVisualStyleBackColor = true;
        buttonGet.Click += ButtonGet_Click;
        // 
        // labelHint
        // 
        labelHint.AutoSize = true;
        labelHint.ForeColor = SystemColors.GrayText;
        labelHint.Location = new Point(330, 83);
        labelHint.Name = "labelHint";
        labelHint.Size = new Size(256, 20);
        labelHint.TabIndex = 4;
        labelHint.Text = "Для set/get используется поле выше.";
        // 
        // groupBoxLog
        // 
        groupBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        groupBoxLog.Controls.Add(listBoxLog);
        groupBoxLog.Controls.Add(buttonClearLog);
        groupBoxLog.Location = new Point(20, 412);
        groupBoxLog.Name = "groupBoxLog";
        groupBoxLog.Size = new Size(620, 185);
        groupBoxLog.TabIndex = 5;
        groupBoxLog.TabStop = false;
        groupBoxLog.Text = "Журнал событий SelectedValueChanged";
        // 
        // listBoxLog
        // 
        listBoxLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        listBoxLog.FormattingEnabled = true;
        listBoxLog.HorizontalScrollbar = true;
        listBoxLog.IntegralHeight = false;
        listBoxLog.Location = new Point(15, 28);
        listBoxLog.Name = "listBoxLog";
        listBoxLog.Size = new Size(590, 110);
        listBoxLog.TabIndex = 0;
        // 
        // buttonClearLog
        // 
        buttonClearLog.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonClearLog.Location = new Point(480, 145);
        buttonClearLog.Name = "buttonClearLog";
        buttonClearLog.Size = new Size(125, 30);
        buttonClearLog.TabIndex = 1;
        buttonClearLog.Text = "Очистить журнал";
        buttonClearLog.UseVisualStyleBackColor = true;
        buttonClearLog.Click += ButtonClearLog_Click;
        // 
        // labelStatus
        // 
        labelStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        labelStatus.AutoSize = true;
        labelStatus.Location = new Point(20, 607);
        labelStatus.Name = "labelStatus";
        labelStatus.Size = new Size(210, 20);
        labelStatus.TabIndex = 6;
        labelStatus.Text = "Элементов в списке: 0; выбранное значение: \"\"";
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(660, 635);
        Controls.Add(labelStatus);
        Controls.Add(groupBoxLog);
        Controls.Add(groupBoxSelected);
        Controls.Add(groupBoxAdd);
        Controls.Add(comboBoxSelector1);
        Controls.Add(labelComponent);
        Controls.Add(labelTitle);
        MinimumSize = new Size(676, 520);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Лабораторная работа №1 — Вариант 13";
        groupBoxAdd.ResumeLayout(false);
        groupBoxAdd.PerformLayout();
        groupBoxSelected.ResumeLayout(false);
        groupBoxSelected.PerformLayout();
        groupBoxLog.ResumeLayout(false);
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label labelTitle = null!;
    private Label labelComponent = null!;
    private SelectionControlLibrary.ComboBoxSelector comboBoxSelector1 = null!;
    private GroupBox groupBoxAdd = null!;
    private Label labelValue = null!;
    private TextBox textBoxValue = null!;
    private Button buttonAdd = null!;
    private Button buttonDemo = null!;
    private Button buttonClear = null!;
    private GroupBox groupBoxSelected = null!;
    private Label labelSelected = null!;
    private TextBox textBoxSelected = null!;
    private Button buttonSet = null!;
    private Button buttonGet = null!;
    private Label labelHint = null!;
    private GroupBox groupBoxLog = null!;
    private ListBox listBoxLog = null!;
    private Button buttonClearLog = null!;
    private Label labelStatus = null!;
}
