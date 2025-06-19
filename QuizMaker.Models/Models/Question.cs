namespace QuizMaker.Models
{
    public class Question
    {
        public int Id { get; set; }
        public string QuestionText { get; set; }
        public string AnswerText { get; set; }

        public ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
}
