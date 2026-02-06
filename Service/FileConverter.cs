using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace searchEngineWebApp.Service
{
    public static class FileConverter
    {
        public static string ConvertPDFToText(string pdfPath, string name)
        {
            var text = new StringBuilder();
            using var documnet = PdfDocument.Open(pdfPath);
            foreach(var page in documnet.GetPages())
            {
                var words = page.GetWords();
                var lines = words
                            .GroupBy(w => Math.Round(w.BoundingBox.Bottom, 1)) // group by Y-axis
                            .OrderByDescending(g => g.Key);                    // top to bottom

                foreach (var line in lines)
                {
                    var lineText = string.Join(" ",
                        line.OrderBy(w => w.BoundingBox.Left)
                            .Select(w => w.Text));

                    text.AppendLine(lineText);
                }

                text.AppendLine(); // page break

            }

            File.WriteAllText($"{name}.txt", text.ToString());
            return text.ToString();
        }

    }
}
