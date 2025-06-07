using BankSystem.Data.Entities;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace BankSystem.Service.Services.ReportService
{
    public class ReportService : IReportService
    {
        public byte[] GenerateTransactionReceiptPdf(TransactionReport report)
        {
            using var document = new PdfDocument();
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12, XFontStyle.Regular);

            int y = 40;
            gfx.DrawString("==== Transaction Receipt ====", font, XBrushes.Black, new XPoint(40, y)); y += 30;
            gfx.DrawString($"Full Name      : {report.UserFullName}", font, XBrushes.Black, new XPoint(40, y)); y += 25;
            gfx.DrawString($"Account Number : {report.AccountNumber}", font, XBrushes.Black, new XPoint(40, y)); y += 25;
            gfx.DrawString($"Transaction    : {report.TransactionType}", font, XBrushes.Black, new XPoint(40, y)); y += 25;
            gfx.DrawString($"Amount         : {report.Amount:C}", font, XBrushes.Black, new XPoint(40, y)); y += 25;
            gfx.DrawString($"Date           : {report.Date}", font, XBrushes.Black, new XPoint(40, y)); y += 25;
            gfx.DrawString($"Reference No   : {report.ReferenceNumber}", font, XBrushes.Black, new XPoint(40, y)); y += 25;
            gfx.DrawString($"Status         : {report.Status}", font, XBrushes.Black, new XPoint(40, y)); y += 30;
            gfx.DrawString("=================================", font, XBrushes.Black, new XPoint(40, y));

            using var stream = new MemoryStream();
            document.Save(stream);
            return stream.ToArray();
        }

    }
}
