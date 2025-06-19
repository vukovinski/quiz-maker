using QuizMaker.Models;
using Microsoft.EntityFrameworkCore;

namespace QuizMaker.Api.Handlers
{
    public class GetQuestionsHandler
    {
        public async Task<List<QuestionDto>> Run(QuizMakerDbContext dbContext, GetQuestionsRequest request)
        {
            return await dbContext.Questions
                .AsNoTracking()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.QuestionText,
                    Answer = q.AnswerText

                }).ToListAsync();
        }
    }

    public class GetQuestionsRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;
    }
}
