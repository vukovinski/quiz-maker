namespace QuizMaker.Api.Handlers
{
    public class QuestionDto
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public required string Answer { get; set; }
    }
}
