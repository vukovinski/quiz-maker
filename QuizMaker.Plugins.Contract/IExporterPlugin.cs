namespace QuizMaker.Plugins.Contract;

public interface IExporterPlugin
{
    string Name { get; }
    (string MimeType, string FileName, byte[] Content) Export(string quizName, List<(string Question,string Answer)> questions);
}
