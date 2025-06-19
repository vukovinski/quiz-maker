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
    public class QuestionsController : ControllerBase
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
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return BadRequest("Search text cannot be empty.");
            }
            try
            {
                var questions = await _searchQuestionsHandler.Activate().Run(_dbContext, searchText);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching for questions.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetQuestions(ApiVersion apiVersion, [FromBody] GetQuestionsRequest request)
        {
            try
            {
                var questions = await _getQuestionsHandler.Activate().Run(_dbContext, request);
                return Ok(questions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving questions.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error. Please try again later.");
            }
        }
    }
}
