using ComponentOrientedPlatform;
using System.Windows.Forms;

namespace ProductCatalog.Components.ProductDirectory;

/// <summary>
/// Компонент сложного справочника продуктов.
/// </summary>
public class ProductDirectoryComponent : IComponentContract
{
    public string Id => "ProductDirectory";

    public string Title => "Продукты";

    public ComponentSection Section => ComponentSection.Directories;

    public AccessLevel MinimumAccessLevel => AccessLevel.Basic;

    public UserControl CreateControl()
    {
        return new ProductDirectoryControl();
    }
}
