using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Schulprojekt.Models;

public interface IPdfService
{
    byte[] CreateInvoicePdf(Order order);
}

public class PdfService : IPdfService
{
    [Obsolete]
    public byte[] CreateInvoicePdf(Order order)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));
                page.Header().Element(ComposeHeader(order));
                page.Content().Element(ComposeContent(order));
                page.Footer().AlignRight().Text(x =>
                {
                    x.Span("Seite ");
                    x.CurrentPageNumber();
                    x.Span(" / ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    [Obsolete]
    private Action<IContainer> ComposeHeader(Order order) => container =>
    {
        container.Row(row =>
        {
            row.RelativeColumn().Stack(stack =>
            {
                stack.Item().Text("Dein Shop Name").Style(Typography.Title);
                stack.Item().Text("Adresse • UID • IBAN");
                stack.Item().Text($"Rechnung {order.InvoiceNumber ?? "-"}").SemiBold().FontSize(16);
                stack.Item().Text($"Datum: {order.OrderDate:dd.MM.yyyy}");
            });

            row.ConstantColumn(120).AlignRight().Stack(stack =>
            {
                // Optional: Logo
                // stack.Item().Image("wwwroot/images/logo.png").FitWidth();
                stack.Item().Text("Rechnung").Bold().FontSize(22);
            });
        });
    };

    [Obsolete]
    private Action<IContainer> ComposeContent(Order order) => container =>
    {
        container.Stack(stack =>
        {
            // Kunde + Versand
            stack.Item().PaddingBottom(10).Row(row =>
            {
                row.RelativeColumn().Stack(s =>
                {
                    s.Item().Text("Kunde").Bold();
                    s.Item().Text($"{order.User?.UserName ?? "-"}");
                    s.Item().Text($"{order.User?.Email ?? "-"}");
                });

                row.RelativeColumn().Stack(s =>
                {
                    s.Item().Text("Lieferadresse").Bold();
                    s.Item().Text($"{order.ShippingInfo?.Address ?? "-"}");
                    s.Item().Text($"{order.ShippingInfo?.PostalCode} {order.ShippingInfo?.City}");
                    s.Item().Text($"{order.ShippingInfo?.Country}");
                });

                row.RelativeColumn().Stack(s =>
                {
                    s.Item().Text("Zahlung").Bold();
                    s.Item().Text($"{order.Payment?.Method ?? "-"}");
                    s.Item().Text($"Status: {order.Status ?? "-"}");
                });
            });

            // Positionen
            stack.Item().PaddingVertical(5).Element(ComposeItemsTable(order));

            // Summen
            stack.Item().PaddingTop(10).AlignRight().Stack(s =>
            {
                s.Item().Text($"Gesamt: {order.TotalAmount:C}").Bold().FontSize(14);
            });

            stack.Item().PaddingTop(20).Text("Vielen Dank für deinen Einkauf!").Italic();
            stack.Item().PaddingTop(5).Text("Rechtlicher Hinweis, Widerrufsbelehrung oder Impressum hier ergänzen.").FontSize(9);
        });
    };

    private Action<IContainer> ComposeItemsTable(Order order) => container =>
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(5);  // Produkt
                columns.RelativeColumn(2);  // Menge
                columns.RelativeColumn(3);  // Einzelpreis
                columns.RelativeColumn(3);  // Gesamt
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Element(CellHeader).Text("Produkt");
                header.Cell().Element(CellHeader).AlignRight().Text("Menge");
                header.Cell().Element(CellHeader).AlignRight().Text("Einzelpreis");
                header.Cell().Element(CellHeader).AlignRight().Text("Gesamt");
            });

            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    var name = item.Product?.Name ?? $"Produkt #{item.ProductId}";
                    var qty = item.Quantity;
                    var unit = item.UnitPrice;
                    var total = unit * qty;

                    table.Cell().Element(CellBody).Text(name);
                    table.Cell().Element(CellBody).AlignRight().Text(qty.ToString());
                    table.Cell().Element(CellBody).AlignRight().Text(unit.ToString("C"));
                    table.Cell().Element(CellBody).AlignRight().Text(total.ToString("C"));
                }
            }

            // Optional: Linie unter der Tabelle
            table.Cell().ColumnSpan(4).PaddingTop(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
        });

        static IContainer CellHeader(IContainer container) =>
            container.DefaultTextStyle(x => x.SemiBold()).Padding(4).Background(Colors.Grey.Lighten3).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);

        static IContainer CellBody(IContainer container) =>
            container.Padding(4);
    };
}

static class Typography
{
    public static TextStyle Title => TextStyle.Default.SemiBold().FontSize(18);
}
