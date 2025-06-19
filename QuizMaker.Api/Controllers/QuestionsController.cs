using Asp.Versioning;
using QuizMaker.Shared;
using QuizMaker.Models;
using QuizMaker.Api.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace QuizMaker.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class QuestionsController : BaseController
    {
        private readonly QuizMakerDbContext _dbContext;
        private readonly ILogger<QuestionsController> _logger;
        private readonly Lazy<GetQuestionsHandler> _getQuestionsHandler = new();
        private readonly Lazy<SearchQuestionsHandler> _searchQuestionsHandler = new();

        public QuestionsController(ILogger<QuestionsController> logger, QuizMakerDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet("Search")]
        public async Task<IActionResult> SearchQuestions(ApiVersion apiVersion, [FromQuery] string searchText)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                if (string.IsNullOrWhiteSpace(searchText))
                {
                    return BadRequest("Search text cannot be empty.");
                }
                var questions = await _searchQuestionsHandler.Activate().Run(_dbContext, searchText);
                return Ok(questions);
            });
        }

        [HttpPost]
        public async Task<IActionResult> GetQuestions(ApiVersion apiVersion, [FromBody] GetQuestionsRequest request)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                if (request.Page <= 0 || request.PageSize <= 0)
                {
                    return BadRequest("Page and PageSize must be greater than zero.");
                }
                var questions = await _getQuestionsHandler.Activate().Run(_dbContext, request);
                return Ok(questions);
            });
        }
    }
}
