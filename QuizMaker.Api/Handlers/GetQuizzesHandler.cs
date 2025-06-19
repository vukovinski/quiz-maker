using QuizMaker.Models;
using Microsoft.EntityFrameworkCore;

namespace QuizMaker.Api.Handlers
{
    public class GetQuizzesHandler
    {
        public List<GetQuizzesDto> Run(QuizMakerDbContext dbContext, GetQuizzesRequest request)
        {
            var queryable = dbContext.Quizzes.Include(q => q.QuizQuestions).AsNoTracking().AsQueryable();

            if (request.SortField == GetQuizzesRequest.QuizSortField.Title)
            {
                queryable = request.SortOrder == GetQuizzesRequest.QuizSortOrder.Ascending
                    ? queryable.OrderBy(q => q.Title)
                    : queryable.OrderByDescending(q => q.Title);
            }
            else if (request.SortField == GetQuizzesRequest.QuizSortField.CreatedDate)
            {
                queryable = request.SortOrder == GetQuizzesRequest.QuizSortOrder.Ascending
                    ? queryable.OrderBy(q => q.CreatedAt)
                    : queryable.OrderByDescending(q => q.CreatedAt);
            }

            return queryable
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(q => new GetQuizzesDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    CreatedDate = q.CreatedAt.UtcDateTime,
                    QuestionCount = q.QuizQuestions.Count
                })
                .ToList();
        }
    }

    public class GetQuizzesRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 25;

        public enum QuizSortOrder
        {
            Ascending,
            Descending
        }

        public enum QuizSortField
        {
            Title,
            CreatedDate
        }

        public QuizSortOrder SortOrder { get; set; } = QuizSortOrder.Descending;
        public QuizSortField SortField { get; set; } = QuizSortField.CreatedDate;
    }

    public class GetQuizzesDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public int QuestionCount { get; set; }
    }
}
