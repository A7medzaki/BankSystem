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

            var fontTitle = new XFont("Verdana", 18, XFontStyle.Bold);
            var fontContent = new XFont("Verdana", 12, XFontStyle.Regular);
            var blueBrush = new XSolidBrush(XColors.DodgerBlue);
            var blackBrush = new XSolidBrush(XColors.Black);
            var penBlue = new XPen(XColors.DodgerBlue, 3);

            double margin = 30;
            var rect = new XRect(margin, margin, page.Width - 2 * margin, page.Height - 2 * margin);
            gfx.DrawRectangle(penBlue, rect);

            string logoText = "STC";
            var sizeLogo = gfx.MeasureString(logoText, fontTitle);
            double logoX = (page.Width - sizeLogo.Width) / 2;
            double logoY = margin + 10;
            gfx.DrawString(logoText, fontTitle, blueBrush, new XPoint(logoX, logoY));

            double startY = logoY + sizeLogo.Height + 30;
            double lineSpacing = 25;

            gfx.DrawString("==== Transaction Receipt ====", fontContent, blackBrush, new XPoint(margin + 20, startY));
            startY += lineSpacing;

            gfx.DrawString($"Full Name      : {report.UserFullName}", fontContent, blackBrush, new XPoint(margin + 20, startY));
            startY += lineSpacing;
            gfx.DrawString($"Account Number : {report.AccountNumber}", fontContent, blackBrush, new XPoint(margin + 20, startY));
            startY += lineSpacing;

            gfx.DrawString($"Transaction    : {report.TransactionType}", fontContent, blackBrush, new XPoint(margin + 20, startY));
            startY += lineSpacing;

            gfx.DrawString($"Amount         : {report.Amount:C}", fontContent, blackBrush, new XPoint(margin + 20, startY));
            startY += lineSpacing;

            gfx.DrawString($"Date           : {report.Date:yyyy-MM-dd HH:mm:ss}", fontContent, blackBrush, new XPoint(margin + 20, startY));
            startY += lineSpacing;

            gfx.DrawString($"Reference No   : {report.ReferenceNumber}", fontContent, blackBrush, new XPoint(margin + 20, startY));
            startY += lineSpacing;

            gfx.DrawString($"Status         : {report.Status}", fontContent, blackBrush, new XPoint(margin + 20, startY));
            startY += lineSpacing + 10;

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }


    }
}
