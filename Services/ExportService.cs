using AgendaProElite.Models;
using AgendaProElite.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using OfficeOpenXml;
using System.Linq;

namespace AgendaProElite.Services;

public class ExportService
{
    private readonly DatabaseService _databaseService;

    public ExportService(DatabaseService databaseService)
    {
        _databaseService = databaseService;
        QuestPDF.Settings.License = LicenseType.Community;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<string> ExportToPDFAsync(DateTime startDate, DateTime endDate, string filePath)
    {
        var events = await _databaseService.GetEventsAsync(startDate, endDate);

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .Text("AgendaPro Elite - Calendario")
                    .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Text($"Período: {startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}")
                            .FontSize(14).Bold();

                        foreach (var eventItem in events)
                        {
                            column.Item().PaddingTop(10).Column(eventColumn =>
                            {
                                eventColumn.Item().Text(eventItem.Title).Bold().FontSize(14);
                                eventColumn.Item().Text($"Fecha: {eventItem.StartDate:g} - {eventItem.EndDate:g}");
                                if (!string.IsNullOrEmpty(eventItem.Description))
                                {
                                    eventColumn.Item().Text($"Descripción: {eventItem.Description}");
                                }
                                if (!string.IsNullOrEmpty(eventItem.Location))
                                {
                                    eventColumn.Item().Text($"Ubicación: {eventItem.Location}");
                                }
                            });
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("Página ");
                        x.CurrentPageNumber();
                        x.Span(" de ");
                        x.TotalPages();
                    });
            });
        });

        document.GeneratePdf(filePath);
        return filePath;
    }

    public async Task<string> ExportToExcelAsync(DateTime startDate, DateTime endDate, string filePath)
    {
        var events = await _databaseService.GetEventsAsync(startDate, endDate);

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Eventos");

        // Headers
        worksheet.Cells[1, 1].Value = "Título";
        worksheet.Cells[1, 2].Value = "Fecha Inicio";
        worksheet.Cells[1, 3].Value = "Fecha Fin";
        worksheet.Cells[1, 4].Value = "Descripción";
        worksheet.Cells[1, 5].Value = "Ubicación";
        worksheet.Cells[1, 6].Value = "Todo el día";

        // Style headers
        using (var range = worksheet.Cells[1, 1, 1, 6])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        }

        // Data
        for (int i = 0; i < events.Count; i++)
        {
            var row = i + 2;
            var eventItem = events[i];
            worksheet.Cells[row, 1].Value = eventItem.Title;
            worksheet.Cells[row, 2].Value = eventItem.StartDate;
            worksheet.Cells[row, 3].Value = eventItem.EndDate;
            worksheet.Cells[row, 4].Value = eventItem.Description;
            worksheet.Cells[row, 5].Value = eventItem.Location;
            worksheet.Cells[row, 6].Value = eventItem.IsAllDay ? "Sí" : "No";
        }

        // Auto-fit columns
        worksheet.Cells.AutoFitColumns();

        await File.WriteAllBytesAsync(filePath, package.GetAsByteArray());
        return filePath;
    }
}

