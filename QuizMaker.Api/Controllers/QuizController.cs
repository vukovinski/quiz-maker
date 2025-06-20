using Asp.Versioning;
using QuizMaker.Models;
using QuizMaker.Shared;
using System.Reflection;
using QuizMaker.Api.Handlers;
using Microsoft.AspNetCore.Mvc;
using System.Composition.Hosting;
using QuizMaker.Plugins.Contract;
using Microsoft.EntityFrameworkCore;

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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetQuizzes(ApiVersion apiVersion, [FromBody] GetQuizzesRequest request)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                if (request == null) return BadRequest("Request cannot be null.");
                return Ok(await _getQuizzesHandler.Activate().Run(_dbContext, request));
            });
        }

        [HttpGet("{quizId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteQuiz(ApiVersion apiVersion, int quizId)
        {
            return await TryRunRequestHandler(_logger, async () =>
            {
                var result = await _deleteQuizHandler.Activate().Run(_dbContext, quizId);
                if (result) return NoContent();
                else return NotFound();
            });
        }

        [HttpGet("GetExporters")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetExporters(ApiVersion apiVersion)
        {
            var pluginPath = Path.Combine(AppContext.BaseDirectory, "Plugins");
            var assemblies = Directory
                .GetFiles(pluginPath, "*.dll")
                .Select(Assembly.LoadFrom)
                .ToList();

            var config = new ContainerConfiguration().WithAssemblies(assemblies);

            using (var container = config.CreateContainer())
            {
                var discoveredPluginNames = new List<string>();
                var plugins = container.GetExports<IExporterPlugin>();

                foreach (var plugin in plugins)
                {
                    discoveredPluginNames.Add(plugin.Name);
                }
                return Ok(new { Exporters = discoveredPluginNames });
            }
        }

        [HttpGet("Export/{quizId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Export(ApiVersion apiVersion, [FromQuery] string exporterName, int quizId)
        {
            var pluginPath = Path.Combine(AppContext.BaseDirectory, "Plugins");
            var assemblies = Directory
                .GetFiles(pluginPath, "*.dll")
                .Select(Assembly.LoadFrom)
                .ToList();

            var config = new ContainerConfiguration().WithAssemblies(assemblies);

            using (var container = config.CreateContainer())
            {
                var plugins = container.GetExports<IExporterPlugin>();

                foreach (var plugin in plugins)
                {
                    if (plugin.Name.Equals(exporterName, StringComparison.OrdinalIgnoreCase))
                    {
                        var quiz = await _dbContext.Quizzes.FindAsync(quizId);
                        if (quiz == null) return NotFound("Quiz not found.");
                        var quizQuestions = await _dbContext.QuizQuestions.Where(qq => qq.QuizId == quizId)
                            .Include(qq => qq.Question)
                            .OrderBy(qq => qq.OrdinalNumber)
                            .ToListAsync();
                        var quizQuestionsForExport = quizQuestions.Select(qq => (qq.Question.QuestionText, qq.Question.AnswerText)).ToList();
                        var exportData = plugin.Export(quiz.Title, quizQuestionsForExport);
                        return File(exportData.Content, exportData.MimeType, exportData.FileName);
                    }
                }
            }
            return BadRequest("Exporter not found. Please check the exporter name and try again.");
        }
    }
}
