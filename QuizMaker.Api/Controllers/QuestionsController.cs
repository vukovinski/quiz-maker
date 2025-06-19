using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using QuizMaker.Models;


namespace QuizMaker.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class QuestionsController : ControllerBase
    {
        private readonly QuizMakerDbContext _dbContext;
        private readonly ILogger<QuestionsController> _logger;

        public QuestionsController(ILogger<QuestionsController> logger, QuizMakerDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
    }
}
