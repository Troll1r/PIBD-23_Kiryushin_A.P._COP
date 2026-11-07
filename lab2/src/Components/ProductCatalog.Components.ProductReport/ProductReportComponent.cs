using ComponentOrientedPlatform;
using System.Windows.Forms;

namespace ProductCatalog.Components.ProductReport;

/// <summary>
/// Компонент отчёта по продуктам выбранной категории.
/// </summary>
public class ProductReportComponent : IComponentContract
{
    public string Id => "ProductReport";

    public string Title => "Отчет по категории продуктов";

    public ComponentSection Section => ComponentSection.Reports;

    public AccessLevel MinimumAccessLevel => AccessLevel.Advanced;

    public UserControl CreateControl()
    {
        return new ProductReportControl();
    }
}
