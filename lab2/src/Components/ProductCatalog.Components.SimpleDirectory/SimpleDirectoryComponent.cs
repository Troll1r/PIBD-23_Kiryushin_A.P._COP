using ComponentOrientedPlatform;
using System.Windows.Forms;

namespace ProductCatalog.Components.SimpleDirectory;

/// <summary>
/// Компонент простого справочника категорий продуктов.
/// </summary>
public class SimpleDirectoryComponent : IComponentContract
{
    public string Id => "SimpleDirectory";

    public string Title => "Категории продуктов";

    public ComponentSection Section => ComponentSection.Directories;

    public AccessLevel MinimumAccessLevel => AccessLevel.Minimal;

    public UserControl CreateControl()
    {
        return new SimpleDirectoryControl();
    }
}
