using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace QuizMaker.Models
{
    public class QuizMakerDesignTimeDbContextFactory : IDesignTimeDbContextFactory<QuizMakerDbContext>
    {
        public QuizMakerDbContext CreateDbContext(string[] args)
        {
            var connectionString = "Host=localhost;Port=5432;Database=QuizMaker;Username=postgres;Password=kisikisi";
            var optionsBuilder = new DbContextOptionsBuilder<QuizMakerDbContext>();
            optionsBuilder.UseNpgsql(connectionString);
            return new QuizMakerDbContext(optionsBuilder.Options);
        }
    }
}
