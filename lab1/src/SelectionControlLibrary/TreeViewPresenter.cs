using System.ComponentModel;
using System.Reflection;

namespace SelectionControlLibrary;

/// <summary>
/// Визуальный компонент вывода списка в виде дерева.
/// Иерархия задаётся именами публичных свойств или полей объекта.
/// </summary>
[ToolboxItem(true)]
[Description("Дерево, заполняемое объектами по иерархии свойств или полей.")]
public partial class TreeViewPresenter : UserControl
{
    private string[] _hierarchy = [];

    public TreeViewPresenter()
    {
        InitializeComponent();
        treeView.MouseDown += TreeView_MouseDown;
        treeView.KeyDown += TreeView_KeyDown;
    }

    /// <summary>
    /// Возникает при нажатии клавиши, когда фокус находится на дереве.
    /// </summary>
    [Browsable(false)]
    public event KeyEventHandler? TreeKeyDown;

    /// <summary>
    /// Контекстное меню, привязанное к внутреннему дереву.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ContextMenuStrip? NodeContextMenu
    {
        get => treeView.ContextMenuStrip;
        set => treeView.ContextMenuStrip = value;
    }

    /// <summary>
    /// Задаёт последовательность свойств/полей для уровней дерева.
    /// </summary>
    public void SetHierarchy(params string[] propertyNames)
    {
        ArgumentNullException.ThrowIfNull(propertyNames);

        if (propertyNames.Length == 0)
        {
            throw new ArgumentException("Иерархия не может быть пустой.", nameof(propertyNames));
        }

        if (propertyNames.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentException(
                "Имена свойств или полей не могут быть пустыми.",
                nameof(propertyNames));
        }

        _hierarchy = propertyNames.ToArray();
        treeView.Nodes.Clear();
    }

    /// <summary>
    /// Полностью заменяет содержимое дерева переданными объектами.
    /// </summary>
    public void SetItems(IEnumerable<object> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        treeView.BeginUpdate();
        try
        {
            treeView.Nodes.Clear();

            foreach (object item in items)
            {
                AddItemCore(item);
            }

            treeView.ExpandAll();
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    /// <summary>
    /// Добавляет один объект в дерево.
    /// </summary>
    public void AddItem(object item)
    {
        ArgumentNullException.ThrowIfNull(item);

        treeView.BeginUpdate();
        try
        {
            AddItemCore(item);
        }
        finally
        {
            treeView.EndUpdate();
        }
    }

    /// <summary>
    /// Очищает дерево.
    /// </summary>
    public void ClearItems()
    {
        treeView.Nodes.Clear();
    }

    /// <summary>
    /// Возвращает объект, привязанный к выбранному конечному узлу.
    /// </summary>
    public object? GetSelectedItem()
    {
        return treeView.SelectedNode?.Tag;
    }

    private void AddItemCore(object item)
    {
        if (_hierarchy.Length == 0)
        {
            throw new InvalidOperationException(
                "Сначала задайте иерархию через метод SetHierarchy.");
        }

        TreeNodeCollection nodes = treeView.Nodes;
        TreeNode? currentNode = null;

        foreach (string memberName in _hierarchy)
        {
            string text = GetDisplayValue(item, memberName);
            TreeNode? node = FindNode(nodes, text);

            if (node == null)
            {
                node = new TreeNode(text);
                nodes.Add(node);
            }

            currentNode = node;
            nodes = node.Nodes;
        }

        if (currentNode != null)
        {
            currentNode.Tag = item;
        }
    }

    private static TreeNode? FindNode(TreeNodeCollection nodes, string text)
    {
        foreach (TreeNode node in nodes)
        {
            if (string.Equals(node.Text, text, StringComparison.Ordinal))
            {
                return node;
            }
        }

        return null;
    }

    private static string GetDisplayValue(object item, string memberName)
    {
        Type type = item.GetType();
        object? value;

        PropertyInfo? property = type.GetProperty(
            memberName,
            BindingFlags.Public | BindingFlags.Instance);

        if (property != null)
        {
            value = property.GetValue(item);
        }
        else
        {
            FieldInfo? field = type.GetField(
                memberName,
                BindingFlags.Public | BindingFlags.Instance);

            if (field == null)
            {
                throw new ArgumentException(
                    $"У типа {type.Name} нет публичного свойства или поля «{memberName}».",
                    nameof(memberName));
            }

            value = field.GetValue(item);
        }

        if (value == null)
        {
            return "—";
        }

        string text = value.ToString() ?? string.Empty;
        return string.IsNullOrWhiteSpace(text) ? "—" : text;
    }

    private void TreeView_MouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button != MouseButtons.Right)
        {
            return;
        }

        TreeNode? node = treeView.GetNodeAt(e.Location);
        if (node != null)
        {
            treeView.SelectedNode = node;
        }
    }

    private void TreeView_KeyDown(object? sender, KeyEventArgs e)
    {
        TreeKeyDown?.Invoke(this, e);
    }
}
