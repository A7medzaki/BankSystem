using BankSystem.Service.Services.CheckService;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Globalization;

public class ChequeService : IChequeService
{
    public async Task<byte[]> GenerateChequePdfAsync(string fromAccountName, string toName, decimal amount, string chequeNumber)
    {
        var document = new PdfDocument();
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);

        var font = new XFont("Verdana", 14, XFontStyle.Regular);
        var boldFont = new XFont("Verdana", 14, XFontStyle.Bold);

        // 🧾 بيانات الشيك
        gfx.DrawString("Bank Cheque", boldFont, XBrushes.Black, new XPoint(250, 50));
        gfx.DrawString($"Cheque No: {chequeNumber}", font, XBrushes.Black, new XPoint(400, 100));
        gfx.DrawString($"From: {fromAccountName}", font, XBrushes.Black, new XPoint(150, 150));
        gfx.DrawString($"To: {toName}", font, XBrushes.Black, new XPoint(150, 180));
        gfx.DrawString($"Amount: {amount:C}", boldFont, XBrushes.DarkGreen, new XPoint(150, 210));

        gfx.DrawString($"In Words: {NumberToWords((int)amount)} Pounds", font, XBrushes.Black, new XPoint(150, 240));
        gfx.DrawString($"Date: {DateTime.Now.ToShortDateString()}", font, XBrushes.Black, new XPoint(150, 270));

        // 🧾 تحويل المستند لـ byte[]
        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }

    private string NumberToWords(int number)
    {
        if (number == 0)
            return "zero";

        if (number < 0)
            return "minus " + NumberToWords(Math.Abs(number));

        string[] unitsMap = new[]
        { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
          "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen",
          "seventeen", "eighteen", "nineteen" };

        string[] tensMap = new[]
        { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy",
          "eighty", "ninety" };

        var words = "";

        if ((number / 1000) > 0)
        {
            words += NumberToWords(number / 1000) + " thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += NumberToWords(number / 100) + " hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
                words += "and ";

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
        }

        return words;
    }
}
