using QuizMaker.Models;

namespace QuizMaker.Api.Handlers
{
    public class DeleteQuizHandler
    {
        public async Task<bool> Run(QuizMakerDbContext dbContext, int quizId)
        {
            var quiz = await dbContext.Quizzes.FindAsync(quizId);
            if (quiz == null) return false;

            dbContext.QuizQuestions.RemoveRange(quiz.QuizQuestions);
            dbContext.Quizzes.Remove(quiz);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
