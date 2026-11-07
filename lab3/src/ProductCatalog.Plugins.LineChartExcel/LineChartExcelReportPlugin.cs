using ClosedXML.Excel;
using PluginsContracts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ProductCatalog.Plugins.LineChartExcel;

/// <summary>
/// Плагин формирования Excel-документа с линейной диаграммой.
/// Для варианта 13 каждая серия — категория продукта,
/// а единственное значение серии — количество отсутствующих продуктов.
/// </summary>
public class LineChartExcelReportPlugin : IReportDocumentWithChartLineContract
{
    public string DocumentFormat => "xlsx";

    public Task CreateDocumentAsync(
        string filePath,
        string header,
        string chartTitle,
        Dictionary<string, List<(int Parameter, double Value)>> series)
    {
        Validate(filePath, header, chartTitle, series);

        return Task.Run(() =>
        {
            List<(string Category, double Value)> items = series
                .Select(pair => (pair.Key, pair.Value[0].Value))
                .ToList();

            byte[] chartImage = CreateChartImage(items, chartTitle);

            using var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("Диаграмма");

            worksheet.Cell(1, 1).Value = header;
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;

            worksheet.Cell(2, 1).Value = chartTitle;
            worksheet.Cell(2, 1).Style.Font.FontSize = 12;

            worksheet.Cell(4, 1).Value = "Категория";
            worksheet.Cell(4, 2).Value = "Количество";
            worksheet.Cell(4, 1).Style.Font.Bold = true;
            worksheet.Cell(4, 2).Style.Font.Bold = true;

            for (int index = 0; index < items.Count; index++)
            {
                worksheet.Cell(index + 5, 1).Value = items[index].Category;
                worksheet.Cell(index + 5, 2).Value = items[index].Value;
            }

            worksheet.Column(1).Width = 35;
            worksheet.Column(2).Width = 14;

            using var imageStream = new MemoryStream(chartImage);
            worksheet.AddPicture(imageStream)
                .MoveTo(worksheet.Cell(4, 4))
                .WithSize(800, 400);

            workbook.SaveAs(filePath);
        });
    }

    private static byte[] CreateChartImage(
        List<(string Category, double Value)> items,
        string chartTitle)
    {
        const int width = 900;
        const int height = 450;
        const int left = 70;
        const int right = 30;
        const int top = 60;
        const int bottom = 100;

        using var bitmap = new Bitmap(width, height);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.White);

        using var titleFont = new Font("Segoe UI", 14, FontStyle.Bold);
        using var labelFont = new Font("Segoe UI", 9);
        using var axisPen = new Pen(Color.FromArgb(90, 90, 90), 1.5f);
        using var gridPen = new Pen(Color.FromArgb(220, 220, 220), 1);
        using var linePen = new Pen(Color.FromArgb(0, 114, 198), 2.5f);
        using var pointBrush = new SolidBrush(Color.FromArgb(0, 114, 198));
        using var textBrush = new SolidBrush(Color.FromArgb(40, 40, 40));

        var plot = new Rectangle(left, top, width - left - right, height - top - bottom);
        double maxValue = items.Count == 0 ? 1 : Math.Max(1, items.Max(item => item.Value));

        graphics.DrawString(
            chartTitle,
            titleFont,
            textBrush,
            new RectangleF(0, 12, width, 32),
            new StringFormat { Alignment = StringAlignment.Center });

        for (int step = 0; step <= 5; step++)
        {
            float y = plot.Bottom - (float)(plot.Height * step / 5.0);
            graphics.DrawLine(gridPen, plot.Left, y, plot.Right, y);
            graphics.DrawString(
                (maxValue * step / 5.0).ToString("0.##"),
                labelFont,
                textBrush,
                new PointF(left - 45, y - 8));
        }

        graphics.DrawLine(axisPen, plot.Left, plot.Top, plot.Left, plot.Bottom);
        graphics.DrawLine(axisPen, plot.Left, plot.Bottom, plot.Right, plot.Bottom);

        if (items.Count > 0)
        {
            var points = new PointF[items.Count];

            for (int index = 0; index < items.Count; index++)
            {
                float x = items.Count == 1
                    ? plot.Left + plot.Width / 2f
                    : plot.Left + (float)index * plot.Width / (items.Count - 1);

                float y = plot.Bottom - (float)(items[index].Value / maxValue * plot.Height);
                points[index] = new PointF(x, y);
            }

            if (points.Length > 1)
            {
                graphics.DrawLines(linePen, points);
            }

            foreach (PointF point in points)
            {
                graphics.FillEllipse(pointBrush, point.X - 4, point.Y - 4, 8, 8);
            }

            for (int index = 0; index < items.Count; index++)
            {
                string label = items[index].Category;
                if (label.Length > 18)
                {
                    label = label[..18] + "…";
                }

                GraphicsState state = graphics.Save();
                graphics.TranslateTransform(points[index].X, plot.Bottom + 8);
                graphics.RotateTransform(-35);
                graphics.DrawString(label, labelFont, textBrush, 0, 0);
                graphics.Restore(state);
            }
        }

        using var stream = new MemoryStream();
        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray();
    }

    private static void Validate(
        string filePath,
        string header,
        string chartTitle,
        Dictionary<string, List<(int Parameter, double Value)>> series)
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

        if (series == null)
        {
            throw new ArgumentNullException(nameof(series));
        }

        if (series.Count == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(series), "Список серий пуст.");
        }

        foreach (var pair in series)
        {
            if (pair.Value == null)
            {
                throw new ArgumentNullException(nameof(series), "Список серии не задан.");
            }

            if (pair.Value.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(series), "Список серии пуст.");
            }
        }

        int[] firstParameters = series.Values
            .First()
            .Select(point => point.Parameter)
            .OrderBy(parameter => parameter)
            .ToArray();

        foreach (var seriesValues in series.Values)
        {
            int[] parameters = seriesValues
                .Select(point => point.Parameter)
                .OrderBy(parameter => parameter)
                .ToArray();

            if (!parameters.SequenceEqual(firstParameters))
            {
                throw new ArgumentException(
                    "В разных сериях различаются параметры.",
                    nameof(series));
            }
        }
    }
}
