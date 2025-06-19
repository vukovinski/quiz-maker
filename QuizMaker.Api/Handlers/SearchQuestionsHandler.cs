using QuizMaker.Models;

namespace QuizMaker.Api.Handlers
{
    public class SearchQuestionsHandler
    {
        public async Task<List<QuestionDto>> Run(QuizMakerDbContext dbContext, string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                throw new ArgumentException("Search text cannot be empty.", nameof(searchText));
            }
            var questions = await dbContext.GetQuestionsByPartialText(searchText);
            return questions.Select(q => new QuestionDto
            {
                Id = q.Id,
                Text = q.QuestionText,
                Answer = q.AnswerText

            }).ToList();
        }
    }
}
