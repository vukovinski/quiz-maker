using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

using QuizMaker.Models;
using QuizMaker.Shared;
using QuizMaker.Api.Handlers;


namespace QuizMaker.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class QuizController : ControllerBase
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
            return TryRunRequestHandler(() =>
            {
                var result = _createQuizHandler.Activate().Run(_dbContext, request);
                if (result > 0) return CreatedAtAction(nameof(GetQuiz), new { apiVersion = apiVersion.ToString(), quizId = result }, new { Id = result });
                else return BadRequest("Invalid request data. Please check the input and try again.");
            });
        }

        [HttpPost]
        public IActionResult GetQuizzes(ApiVersion apiVersion, [FromBody] GetQuizzesRequest request)
        {
            return TryRunRequestHandler(() =>
            {
                return Ok(_getQuizzesHandler.Activate().Run(_dbContext, request));
            });
        }

        [HttpGet("{quizId}")]
        public IActionResult GetQuiz(ApiVersion apiVersion, int quizId)
        {
            return TryRunRequestHandler(() =>
            {
                var result = _getQuizHandler.Activate().Run(_dbContext, quizId);
                if (result != null) return Ok(result);
                else return NotFound();
            });
        }

        private IActionResult TryRunRequestHandler(Func<IActionResult> handler)
        {
            try
            {
                return handler();
            }
            catch (Exception e)
            {
                _logger.LogError("An error occured while processing the request. Error: {errorMessage}", e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
