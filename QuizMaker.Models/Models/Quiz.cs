namespace QuizMaker.Models
{
    public class Quiz
    {
        public int Id { get; set; }
        public string Title { get; set; }

        public ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
}
