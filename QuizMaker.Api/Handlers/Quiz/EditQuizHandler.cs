using QuizMaker.Models;

namespace QuizMaker.Api.Handlers
{
    public class EditQuizHandler
    {
        public async Task Run(QuizMakerDbContext dbContext, EditQuizRequest request)
        {
            if (request.ValidRequest())
            {
                var quiz = await dbContext.Quizzes.FindAsync(request.QuizId);
                if (quiz == null)
                    throw new KeyNotFoundException($"Quiz with Id {request.QuizId} not found.");
                quiz.Title = request.Title;
                
                for (int i = 0; i < request.Questions.Count; i++)
                {
                    var questionDto = request.Questions[i];
                    if (questionDto.ValidNewQuestion())
                    {
                        var newQuestion = new Question
                        {
                            QuestionText = questionDto.Text!,
                            AnswerText = questionDto.AnswerText!
                        };
                        dbContext.Questions.Add(newQuestion);
                        quiz.QuizQuestions.Add(new QuizQuestion { Quiz = quiz, Question = newQuestion, OrdinalNumber = i + 1 });
                    }
                    else if (questionDto.ValidExistingQuestion())
                    {
                        var existingQuestion = await dbContext.Questions.FindAsync(questionDto.QuestionId);
                        if (existingQuestion == null)
                            throw new KeyNotFoundException($"Question with Id {questionDto.QuestionId} not found.");
                        var existingRelation = quiz.QuizQuestions.FirstOrDefault(q => q.QuestionId == questionDto.QuestionId);
                        if (existingRelation == null)
                            quiz.QuizQuestions.Add(new QuizQuestion { Quiz = quiz, Question = existingQuestion, OrdinalNumber = i + 1 });
                        else existingRelation.OrdinalNumber = i + 1;
                    }
                }
                await dbContext.SaveChangesAsync();
            }
        }
    }

    public class EditQuizRequest
    {
        public int QuizId { get; set; }
        public required string Title { get; set; }
        public required List<QuestionDto> Questions { get; set; }

        public class QuestionDto
        {
            public int? QuestionId { get; set; } = null;
            public string? Text { get; set; } = null;
            public string? AnswerText { get; set; } = null;

            public enum QuestionStatus
            {
                New,
                Existing
            }
            public QuestionStatus Status { get; set; }

            public bool ValidNewQuestion()
            {
                return Status == QuestionStatus.New && !string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(AnswerText);
            }

            public bool ValidExistingQuestion()
            {
                return Status == QuestionStatus.Existing && QuestionId.HasValue;
            }

            public bool ValidQuestion()
            {
                return ValidNewQuestion() || ValidExistingQuestion();
            }
        }

        public bool ValidRequest()
        {
            return QuizId > 0 && !string.IsNullOrEmpty(Title) && Questions != null && Questions.All(q => q.ValidQuestion());
        }
    }
}
