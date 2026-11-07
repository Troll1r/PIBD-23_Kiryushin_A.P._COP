using PluginsContracts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using DrawingColor = System.Drawing.Color;
using DrawingImageFormat = System.Drawing.Imaging.ImageFormat;

namespace ProductCatalog.Plugins.PieChartPdf;

/// <summary>
/// Плагин формирования PDF-документа с круговой диаграммой.
/// </summary>
public class PieChartPdfReportPlugin : IReportDocumentWithChartPieContract
{
    static PieChartPdfReportPlugin()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public string DocumentFormat => "pdf";

    public Task CreateDocumentAsync(
        string filePath,
        string header,
        string chartTitle,
        string seriesName,
        List<(string Parameter, double Value)> series)
    {
        Validate(filePath, header, chartTitle, seriesName, series);

        return Task.Run(() =>
        {
            byte[] chartImage = CreatePieChartImage(series, chartTitle);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.DefaultTextStyle(style => style.FontSize(11));

                    page.Header().Column(column =>
                    {
                        column.Item().Text(header).FontSize(20).Bold();
                        column.Item().Text(chartTitle).FontSize(14);
                    });

                    page.Content().Column(column =>
                    {
                        column.Item().PaddingTop(12).Image(chartImage).FitWidth();
                        column.Item().PaddingTop(10).Text(seriesName).FontSize(12).Bold();

                        column.Item().PaddingTop(6).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.ConstantColumn(90);
                            });

                            table.Header(tableHeader =>
                            {
                                tableHeader.Cell().Text("Категория").Bold();
                                tableHeader.Cell().Text("Количество").Bold();
                            });

                            foreach (var (parameter, value) in series)
                            {
                                table.Cell().Text(parameter);
                                table.Cell().Text(value.ToString("0.##"));
                            }
                        });
                    });
                });
            });

            document.GeneratePdf(filePath);
        });
    }

    private static byte[] CreatePieChartImage(
        List<(string Parameter, double Value)> series,
        string chartTitle)
    {
        const int width = 800;
        const int height = 430;

        using var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(DrawingColor.White);

        using var titleFont = new Font("Segoe UI", 14, FontStyle.Bold);
        using var legendFont = new Font("Segoe UI", 9);
        using var textBrush = new SolidBrush(DrawingColor.FromArgb(40, 40, 40));
        using var outlinePen = new Pen(DrawingColor.White, 2);

        graphics.DrawString(
            chartTitle,
            titleFont,
            textBrush,
            new RectangleF(0, 12, width, 32),
            new StringFormat { Alignment = StringAlignment.Center });

        DrawingColor[] colors =
        [
            DrawingColor.FromArgb(0, 114, 198),
            DrawingColor.FromArgb(255, 153, 0),
            DrawingColor.FromArgb(55, 160, 90),
            DrawingColor.FromArgb(200, 60, 60),
            DrawingColor.FromArgb(120, 90, 180),
            DrawingColor.FromArgb(0, 160, 160),
            DrawingColor.FromArgb(150, 100, 50)
        ];

        double total = series.Sum(item => Math.Max(0, item.Value));
        var pieRect = new Rectangle(40, 70, 340, 340);

        if (total <= 0)
        {
            using var emptyBrush = new SolidBrush(DrawingColor.FromArgb(220, 220, 220));
            graphics.FillEllipse(emptyBrush, pieRect);
            graphics.DrawString(
                "Нет данных",
                legendFont,
                textBrush,
                new RectangleF(pieRect.X, pieRect.Y + pieRect.Height / 2f - 10, pieRect.Width, 20),
                new StringFormat { Alignment = StringAlignment.Center });
        }
        else
        {
            float startAngle = -90;

            for (int index = 0; index < series.Count; index++)
            {
                double value = Math.Max(0, series[index].Value);
                float sweepAngle = (float)(360 * value / total);

                using var brush = new SolidBrush(colors[index % colors.Length]);
                graphics.FillPie(brush, pieRect, startAngle, sweepAngle);
                graphics.DrawPie(outlinePen, pieRect, startAngle, sweepAngle);
                startAngle += sweepAngle;
            }
        }

        float legendX = 430;
        float legendY = 80;

        for (int index = 0; index < series.Count; index++)
        {
            using var brush = new SolidBrush(colors[index % colors.Length]);
            graphics.FillRectangle(brush, legendX, legendY + index * 26, 16, 16);

            string label = series[index].Parameter;
            if (label.Length > 30)
            {
                label = label[..30] + "…";
            }

            graphics.DrawString(
                $"{label}: {series[index].Value:0.##}",
                legendFont,
                textBrush,
                legendX + 24,
                legendY + index * 26 - 2);
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, DrawingImageFormat.Png);
        return stream.ToArray();
    }

    private static void Validate(
        string filePath,
        string header,
        string chartTitle,
        string seriesName,
        List<(string Parameter, double Value)> series)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentNullException(nameof(filePath));
        }

        if (string.IsNullOrWhiteSpace(header))
        {
            throw new ArgumentNullException(nameof(header));
        }

        if (string.IsNullOrWhiteSpace(chartTitle))
        {
            throw new ArgumentNullException(nameof(chartTitle));
        }

        if (string.IsNullOrWhiteSpace(seriesName))
        {
            throw new ArgumentNullException(nameof(seriesName));
        }

        if (series == null)
        {
            throw new ArgumentNullException(nameof(series));
        }

        if (series.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(series), "Список серий пуст.");
        }
    }
}
