using QuizMaker.Models;
using Microsoft.EntityFrameworkCore;

namespace QuizMaker.Api.Handlers
{
    public class GetQuizHandler
    {
        public async Task<GetQuizDto?> Run(QuizMakerDbContext dbContext, int quizId)
        {
            var quiz = await dbContext.Quizzes.Include(q => q.QuizQuestions).ThenInclude(qq => qq.Question).AsNoTracking().FirstOrDefaultAsync(q => q.Id == quizId);
            if (quiz == null) return null;
            return new GetQuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                CreatedDate = quiz.CreatedAt.UtcDateTime,
                Questions = quiz.QuizQuestions.Select(qq => new GetQuizDto.QuestionDto
                {
                    Id = qq.Question.Id,
                    QuestionText = qq.Question.QuestionText,
                    AnswerText = qq.Question.AnswerText,
                    OrdinalNumber = qq.OrdinalNumber

                }).OrderBy(qq => qq.OrdinalNumber).ToList()
            };
        }
    }

    public class GetQuizDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public required List<QuestionDto> Questions { get; set; }

        public class QuestionDto
        {
            public int Id { get; set; }
            public string QuestionText { get; set; }
            public string AnswerText { get; set; }
            public int OrdinalNumber { get; set; }
        }
    }
}
