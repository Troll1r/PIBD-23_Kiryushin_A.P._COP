using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ProductCatalog.ProductsApi.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Text;
using PdfDocument = QuestPDF.Fluent.Document;
using WordDocument = DocumentFormat.OpenXml.Wordprocessing.Document;

namespace ProductCatalog.ProductsApi.Reports;

/// <summary>
/// Прямое формирование файлов отчётов без плагинной инфраструктуры.
/// </summary>
public static class ReportGenerator
{
    private static readonly string[] ChartColors =
    [
        "#0072C6",
        "#FF9900",
        "#37A05A",
        "#C83C3C",
        "#785AB4",
        "#00A0A0",
        "#966432"
    ];

    static ReportGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Excel по продуктам в наличии: название и описание в каждой строке.
    /// </summary>
    public static byte[] CreateInStockExcel(IReadOnlyList<Product> products)
    {
        using var workbook = new XLWorkbook();
        IXLWorksheet worksheet = workbook.Worksheets.Add("В наличии");

        worksheet.Cell(1, 1).Value = "Продукт";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 14;

        for (int index = 0; index < products.Count; index++)
        {
            Product product = products[index];
            IXLCell cell = worksheet.Cell(index + 2, 1);
            cell.Value = $"Название: {product.Name}{Environment.NewLine}Описание: {product.Description}";
            cell.Style.Alignment.WrapText = true;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            worksheet.Row(index + 2).Height = 48;
        }

        worksheet.Column(1).Width = 110;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// PDF с круговой диаграммой: сколько продуктов категории отсутствует.
    /// Диаграмма строится как SVG, поэтому работает и в Linux-контейнере.
    /// </summary>
    public static byte[] CreateOutOfStockPiePdf(
        IReadOnlyList<(string Category, int Count)> groups)
    {
        string pieSvg = BuildPieSvg(groups);

        var document = PdfDocument.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(style => style.FontSize(11));

                page.Header().Column(column =>
                {
                    column.Item().Text("Продукты, отсутствующие на складе")
                        .FontSize(20)
                        .Bold();
                    column.Item().Text("Количество отсутствующих продуктов по категориям")
                        .FontSize(14);
                });

                page.Content().Column(column =>
                {
                    column.Item().PaddingTop(12).Svg(pieSvg).FitWidth();

                    if (groups.Count == 0)
                    {
                        column.Item().PaddingTop(10).Text("Нет данных");
                    }
                    else
                    {
                        column.Item().PaddingTop(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(22);
                                columns.RelativeColumn();
                                columns.ConstantColumn(90);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Text(string.Empty);
                                header.Cell().Text("Категория").Bold();
                                header.Cell().Text("Количество").Bold();
                            });

                            for (int index = 0; index < groups.Count; index++)
                            {
                                table.Cell()
                                    .Height(16)
                                    .Background(ChartColors[index % ChartColors.Length]);
                                table.Cell().Text(groups[index].Category);
                                table.Cell().Text(groups[index].Count.ToString());
                            }
                        });
                    }
                });
            });
        });

        using var stream = new MemoryStream();
        document.GeneratePdf(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Word со всеми продуктами. Шапка — первая строка и первый столбец:
    /// по столбцу идентификаторы, в первой строке — остальные заголовки.
    /// </summary>
    public static byte[] CreateAllProductsWord(IReadOnlyList<Product> products)
    {
        using var stream = new MemoryStream();

        using (WordprocessingDocument document = WordprocessingDocument.Create(
                   stream,
                   WordprocessingDocumentType.Document,
                   true))
        {
            MainDocumentPart mainPart = document.AddMainDocumentPart();
            var body = new Body();

            body.Append(new Paragraph(
                new Run(
                    new RunProperties(
                        new Bold(),
                        new FontSize { Val = "28" }),
                    new Text("Все продукты"))));

            var table = new Table();
            table.AppendChild(new TableProperties(
                new TableBorders(
                    new TopBorder { Val = BorderValues.Single, Size = 4 },
                    new BottomBorder { Val = BorderValues.Single, Size = 4 },
                    new LeftBorder { Val = BorderValues.Single, Size = 4 },
                    new RightBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideHorizontalBorder { Val = BorderValues.Single, Size = 4 },
                    new InsideVerticalBorder { Val = BorderValues.Single, Size = 4 })));

            table.Append(new TableRow(
                CreateCell(string.Empty, true),
                CreateCell("Название", true),
                CreateCell("Категория", true),
                CreateCell("Количество", true)));

            foreach (Product product in products)
            {
                table.Append(new TableRow(
                    CreateCell(product.Id.ToString()),
                    CreateCell(product.Name),
                    CreateCell(product.Category),
                    CreateCell(product.Quantity?.ToString() ?? "отсутствует")));
            }

            body.Append(table);
            mainPart.Document = new WordDocument(body);
            mainPart.Document.Save();
        }

        return stream.ToArray();
    }

    private static TableCell CreateCell(string text, bool bold = false)
    {
        var run = new Run();

        if (bold)
        {
            run.AppendChild(new RunProperties(new Bold()));
        }

        run.AppendChild(new Text(text));
        return new TableCell(new Paragraph(run));
    }

    private static string BuildPieSvg(
        IReadOnlyList<(string Category, int Count)> groups)
    {
        const double width = 800;
        const double height = 430;
        const double centerX = 220;
        const double centerY = 220;
        const double radius = 170;

        var svg = new StringBuilder();
        svg.Append("<svg xmlns='http://www.w3.org/2000/svg' width='")
            .Append(width.ToString(CultureInfo.InvariantCulture))
            .Append("' height='")
            .Append(height.ToString(CultureInfo.InvariantCulture))
            .Append("' viewBox='0 0 ")
            .Append(width.ToString(CultureInfo.InvariantCulture))
            .Append(' ')
            .Append(height.ToString(CultureInfo.InvariantCulture))
            .Append("'>");
        svg.Append("<rect width='100%' height='100%' fill='white'/>");

        double total = groups.Sum(group => (double)Math.Max(0, group.Count));
        int nonEmptyGroups = groups.Count(group => group.Count > 0);

        if (total <= 0)
        {
            svg.Append("<circle cx='")
                .Append(centerX.ToString(CultureInfo.InvariantCulture))
                .Append("' cy='")
                .Append(centerY.ToString(CultureInfo.InvariantCulture))
                .Append("' r='")
                .Append(radius.ToString(CultureInfo.InvariantCulture))
                .Append("' fill='#DDDDDD'/>");
        }
        else if (nonEmptyGroups == 1)
        {
            int index = groups
                .Select((group, i) => (group, i))
                .First(item => item.group.Count > 0)
                .i;

            svg.Append("<circle cx='")
                .Append(centerX.ToString(CultureInfo.InvariantCulture))
                .Append("' cy='")
                .Append(centerY.ToString(CultureInfo.InvariantCulture))
                .Append("' r='")
                .Append(radius.ToString(CultureInfo.InvariantCulture))
                .Append("' fill='")
                .Append(ChartColors[index % ChartColors.Length])
                .Append("'/>");
        }
        else
        {
            double startAngle = -90;

            for (int index = 0; index < groups.Count; index++)
            {
                double value = Math.Max(0, groups[index].Count);

                if (value <= 0)
                {
                    continue;
                }

                double sweepAngle = 360 * value / total;
                double endAngle = startAngle + sweepAngle;

                (double x1, double y1) = PointOnCircle(
                    centerX, centerY, radius, startAngle);
                (double x2, double y2) = PointOnCircle(
                    centerX, centerY, radius, endAngle);

                int largeArc = sweepAngle > 180 ? 1 : 0;

                svg.Append("<path d='M ")
                    .Append(centerX.ToString(CultureInfo.InvariantCulture))
                    .Append(' ')
                    .Append(centerY.ToString(CultureInfo.InvariantCulture))
                    .Append(" L ")
                    .Append(x1.ToString("0.###", CultureInfo.InvariantCulture))
                    .Append(' ')
                    .Append(y1.ToString("0.###", CultureInfo.InvariantCulture))
                    .Append(" A ")
                    .Append(radius.ToString(CultureInfo.InvariantCulture))
                    .Append(' ')
                    .Append(radius.ToString(CultureInfo.InvariantCulture))
                    .Append(" 0 ")
                    .Append(largeArc)
                    .Append(" 1 ")
                    .Append(x2.ToString("0.###", CultureInfo.InvariantCulture))
                    .Append(' ')
                    .Append(y2.ToString("0.###", CultureInfo.InvariantCulture))
                    .Append(" Z' fill='")
                    .Append(ChartColors[index % ChartColors.Length])
                    .Append("' stroke='white' stroke-width='2'/>");

                startAngle = endAngle;
            }
        }

        svg.Append("</svg>");
        return svg.ToString();
    }

    private static (double X, double Y) PointOnCircle(
        double centerX,
        double centerY,
        double radius,
        double angleDegrees)
    {
        double radians = angleDegrees * Math.PI / 180;
        return (
            centerX + radius * Math.Cos(radians),
            centerY + radius * Math.Sin(radians));
    }
}
