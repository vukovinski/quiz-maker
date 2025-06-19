using Asp.Versioning;
using QuizMaker.Models;
using QuizMaker.Shared;
using QuizMaker.Api.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace QuizMaker.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class QuizController : BaseController
    {
        private readonly QuizMakerDbContext _dbContext;
        private readonly ILogger<QuizController> _logger;
        private readonly Lazy<GetQuizHandler> _getQuizHandler = new();
        private readonly Lazy<GetQuizzesHandler> _getQuizzesHandler = new();
        private readonly Lazy<CreateQuizHandler> _createQuizHandler = new();

        public QuizController(ILogger<QuizController> logger, QuizMakerDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost("Create")]
        public IActionResult CreateQuiz(ApiVersion apiVersion, [FromBody] CreateQuizRequest request)
        {
            return TryRunRequestHandler(_logger, () =>
            {
                var result = _createQuizHandler.Activate().Run(_dbContext, request);
                if (result > 0) return CreatedAtAction(nameof(GetQuiz), new { apiVersion = apiVersion.ToString(), quizId = result }, new { Id = result });
                else return BadRequest("Invalid request data. Please check the input and try again.");
            });
        }

        [HttpPost]
        public IActionResult GetQuizzes(ApiVersion apiVersion, [FromBody] GetQuizzesRequest request)
        {
            return TryRunRequestHandler(_logger, () =>
            {
                return Ok(_getQuizzesHandler.Activate().Run(_dbContext, request));
            });
        }

        [HttpGet("{quizId}")]
        public IActionResult GetQuiz(ApiVersion apiVersion, int quizId)
        {
            return TryRunRequestHandler(_logger, () =>
            {
                var result = _getQuizHandler.Activate().Run(_dbContext, quizId);
                if (result != null) return Ok(result);
                else return NotFound();
            });
        }
    }
}
