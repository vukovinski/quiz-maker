using Microsoft.EntityFrameworkCore;

namespace QuizMaker.Models
{
    public class QuizMakerDbContext : DbContext
    {
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }

        public async Task<List<Question>> GetQuestionsByPartialText(string searchText)
        {
            return await Questions
                .FromSqlInterpolated($@"
                    SELECT * FROM ""Questions""
                    WHERE to_tsvector('simple', ""QuestionText"") @@ plainto_tsquery('simple', {searchText})")
                .ToListAsync();
        }

        public QuizMakerDbContext(DbContextOptions<QuizMakerDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.ToTable("Quizes");

                entity.HasKey(q => q.Id);

                entity.Property(q => q.Title)
                      .IsRequired()
                      .HasMaxLength(200);
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("Questions");

                entity.HasKey(q => q.Id);

                entity.Property(q => q.QuestionText)
                      .IsRequired()
                      .HasMaxLength(1000);

                entity.Property(q => q.AnswerText)
                      .IsRequired()
                      .HasMaxLength(1000);
            });

            modelBuilder.Entity<QuizQuestion>(entity =>
            {
                entity.ToTable("QuizQuestions");

                entity.HasKey(qq => new { qq.QuizId, qq.QuestionId });

                entity.HasOne(qq => qq.Quiz)
                      .WithMany(qq => qq.QuizQuestions)
                      .HasForeignKey(qq => qq.QuizId);

                entity.HasOne(qq => qq.Question)
                      .WithMany(qq => qq.QuizQuestions)
                      .HasForeignKey(qq => qq.QuestionId);

                entity.Property(qq => qq.OrdinalNumber)
                      .IsRequired();
            });
        }
    }
}
