using System.Windows.Forms;

namespace ComponentOrientedPlatform;

/// <summary>
/// Раздел меню, в котором отображается компонент.
/// </summary>
public enum ComponentSection
{
    Directories,
    Reports
}

/// <summary>
/// Уровень доступа пользователя. Значения упорядочены по возрастанию прав.
/// </summary>
public enum AccessLevel
{
    Minimal = 1,
    Basic = 2,
    Advanced = 3
}

/// <summary>
/// Контракт визуального компонента приложения.
/// </summary>
public interface IComponentContract
{
    /// <summary>
    /// Уникальный идентификатор компонента.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Заголовок пункта меню и вкладки.
    /// </summary>
    string Title { get; }

    /// <summary>
    /// Раздел меню, в котором должен появиться компонент.
    /// </summary>
    ComponentSection Section { get; }

    /// <summary>
    /// Минимальный уровень доступа, необходимый для компонента.
    /// </summary>
    AccessLevel MinimumAccessLevel { get; }

    /// <summary>
    /// Создаёт новый экземпляр пользовательского элемента управления.
    /// </summary>
    UserControl CreateControl();
}
