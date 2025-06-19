using Microsoft.AspNetCore.Mvc;

namespace QuizMaker.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected async Task<IActionResult> TryRunRequestHandler(ILogger logger, Func<Task<IActionResult>> handler)
        {
            try
            {
                return await handler();
            }
            catch (Exception e)
            {
                logger.LogError("An error occured while processing the request. Error: {errorMessage}", e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
