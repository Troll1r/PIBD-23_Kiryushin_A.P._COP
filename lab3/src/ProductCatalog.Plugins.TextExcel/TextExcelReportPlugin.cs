using ClosedXML.Excel;
using PluginsContracts;

namespace ProductCatalog.Plugins.TextExcel;

/// <summary>
/// Плагин формирования Excel-документа с большим текстом.
/// Каждый переданный абзац выводится отдельной строкой.
/// </summary>
public class TextExcelReportPlugin : IReportDocumentWithContextTextsContract
{
    public string DocumentFormat => "xlsx";

    public Task CreateDocumentAsync(
        string filePath,
        string header,
        List<string> paragraphs)
    {
        Validate(filePath, header, paragraphs);

        return Task.Run(() =>
        {
            using var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Отчет");

            worksheet.Cell(1, 1).Value = header;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            worksheet.Range(1, 1, 1, 2).Merge();

            for (int index = 0; index < paragraphs.Count; index++)
            {
                IXLCell cell = worksheet.Cell(index + 3, 1);
                cell.Value = paragraphs[index];
                cell.Style.Alignment.WrapText = true;
                cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                worksheet.Row(index + 3).Height = 48;
            }

            worksheet.Column(1).Width = 110;
            workbook.SaveAs(filePath);
        });
    }

    private static void Validate(
        string filePath,
        string header,
        List<string> paragraphs)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (string.IsNullOrWhiteSpace(header))
        {
            throw new ArgumentNullException(nameof(header));
        }

        if (paragraphs == null)
        {
            throw new ArgumentNullException(nameof(paragraphs));
        }

        if (paragraphs.Count == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(paragraphs),
                "Список абзацев не может быть пустым.");
        }

        if (paragraphs.Any(string.IsNullOrWhiteSpace))
        {
            throw new ArgumentNullException(
                nameof(paragraphs),
                "В списке абзацев есть незаданный абзац.");
        }
    }
}
