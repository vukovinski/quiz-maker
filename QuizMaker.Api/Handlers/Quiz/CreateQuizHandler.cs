using QuizMaker.Models;

namespace QuizMaker.Api.Handlers
{
    public class CreateQuizHandler
    {
        public async Task<int> Run(QuizMakerDbContext dbContext, CreateQuizRequest request)
        {
            if (request.ValidRequest())
            {
                var quiz = new Quiz { Title = request.Title };
                dbContext.Quizzes.Add(quiz);

                for (int i = 0; i < request.Questions.Count; i++)
                {
                    var question = request.Questions[i];
                    if (question.ValidNewQuestion())
                    {
                        var newQuestion = new Question
                        {
                            QuestionText = question.QuestionText!,
                            AnswerText = question.AnswerText!
                        };
                        dbContext.Questions.Add(newQuestion);
                        quiz.QuizQuestions.Add(new QuizQuestion
                        {
                            Quiz = quiz,
                            Question = newQuestion,
                            OrdinalNumber = i + 1
                        });
                    }
                    else if (question.ValidExistingQuestion())
                    {
                        quiz.QuizQuestions.Add(new QuizQuestion
                        {
                            Quiz = quiz,
                            QuestionId = question.Id!.Value,
                            OrdinalNumber = i + 1
                        });
                    }
                }
                await dbContext.SaveChangesAsync();
                return quiz.Id;
            }
            else return -1;
        }
    }

    public class CreateQuizRequest
    {
        public required string Title { get; set; }
        public required List<Question> Questions { get; set; }

        public class Question
        {
            public enum QuestionStatus
            {
                New,
                Existing
            }

            public QuestionStatus Status { get; set; }
            public int? Id { get; set; } = null;
            public string? QuestionText { get; set; } = null;
            public string? AnswerText { get; set; } = null;

            public bool ValidNewQuestion()
            {
                return Status == QuestionStatus.New && !string.IsNullOrEmpty(QuestionText) && !string.IsNullOrEmpty(AnswerText);
            }

            public bool ValidExistingQuestion()
            {
                return Status == QuestionStatus.Existing && Id.HasValue;
            }

            public bool ValidQuestion()
            {
                return ValidNewQuestion() || ValidExistingQuestion();
            }
        }

        public bool ValidQuestions()
        {
            return Questions.Select(q => q.ValidQuestion()).All(v => v) && Questions.Count > 0;
        }

        public bool ValidRequest()
        {
            return !string.IsNullOrEmpty(Title) && ValidQuestions();
        }
    }
}
