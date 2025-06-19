using QuizMaker.Plugins.Contract;
using System.ComponentModel.Composition;

namespace QuizMaker.Plugins.CsvExporter;

[Export(typeof(IExporterPlugin))]
public class CsvExporter : IExporterPlugin
{
    public string Name => "CSV Exporter";

    public (string MimeType, string FileName, byte[] Content) Export(string quizName, List<(string Question, string Answer)> questions)
    {
        var mimeType = "text/csv";
        var fileName = $"{quizName}.csv";
        var content = questions.Select(p => $"{p.Question},{p.Answer}\n").Aggregate("", (l, r) => l + r);
        var contentBytes = System.Text.Encoding.UTF8.GetBytes(content);

        return (mimeType, fileName, contentBytes);
    }
}
