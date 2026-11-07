namespace PluginsContracts;

/// <summary>
/// Общая часть контрактов плагинов формирования документов.
/// </summary>
public interface IReportDocumentContract
{
    /// <summary>
    /// Формат создаваемого документа: docx, xlsx или pdf.
    /// </summary>
    string DocumentFormat { get; }
}

/// <summary>
/// Контракт создания документа с несколькими абзацами текста.
/// </summary>
public interface IReportDocumentWithContextTextsContract : IReportDocumentContract
{
    Task CreateDocumentAsync(
        string filePath,
        string header,
        List<string> paragraphs);
}

/// <summary>
/// Контракт создания документа с линейной диаграммой.
/// </summary>
public interface IReportDocumentWithChartLineContract : IReportDocumentContract
{
    Task CreateDocumentAsync(
        string filePath,
        string header,
        string chartTitle,
        Dictionary<string, List<(int Parameter, double Value)>> series);
}

/// <summary>
/// Контракт создания документа с круговой диаграммой.
/// </summary>
public interface IReportDocumentWithChartPieContract : IReportDocumentContract
{
    Task CreateDocumentAsync(
        string filePath,
        string header,
        string chartTitle,
        string seriesName,
        List<(string Parameter, double Value)> series);
}
