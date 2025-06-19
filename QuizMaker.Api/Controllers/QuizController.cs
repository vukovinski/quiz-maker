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
        private readonly Lazy<EditQuizHandler> _editQuizHandler = new();
        private readonly Lazy<GetQuizzesHandler> _getQuizzesHandler = new();
        private readonly Lazy<CreateQuizHandler> _createQuizHandler = new();
        private readonly Lazy<DeleteQuizHandler> _deleteQuizHandler = new();

        public QuizController(ILogger<QuizController> logger, QuizMakerDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateQuiz(ApiVersion apiVersion, [FromBody] CreateQuizRequest request)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                if (request == null) return BadRequest("Request cannot be null.");
                var result = await _createQuizHandler.Activate().Run(_dbContext, request);
                if (result > 0) return CreatedAtAction(nameof(GetQuiz), new { apiVersion = apiVersion.ToString(), quizId = result }, new { Id = result });
                else return BadRequest("Invalid request data. Please check the input and try again.");
            });
        }

        [HttpPost("Edit")]
        public async Task<IActionResult> EditQuiz(ApiVersion apiVersion, [FromBody] EditQuizRequest request)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                if (request == null) return BadRequest("Request cannot be null.");
                await _editQuizHandler.Activate().Run(_dbContext, request);
                return Ok();
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetQuizzes(ApiVersion apiVersion, [FromBody] GetQuizzesRequest request)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                if (request == null) return BadRequest("Request cannot be null.");
                return Ok(await _getQuizzesHandler.Activate().Run(_dbContext, request));
            });
        }

        [HttpGet("{quizId}")]
        public async Task<IActionResult> GetQuiz(ApiVersion apiVersion, int quizId)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                var result = await _getQuizHandler.Activate().Run(_dbContext, quizId);
                if (result != null) return Ok(result);
                else return NotFound();
            });
        }

        [HttpDelete("{quizId}")]
        public async Task<IActionResult> DeleteQuiz(ApiVersion apiVersion, int quizId)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                var result = await _deleteQuizHandler.Activate().Run(_dbContext, quizId);
                if (result) return NoContent();
                else return NotFound();
            });
        }
    }
}
