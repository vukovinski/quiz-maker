namespace QuizMaker.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
}
