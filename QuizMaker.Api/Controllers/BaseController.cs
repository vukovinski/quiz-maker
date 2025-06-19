using Microsoft.AspNetCore.Mvc;

namespace QuizMaker.Api.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult TryRunRequestHandler(ILogger logger, Func<IActionResult> handler)
        {
            try
            {
                return handler();
            }
            catch (Exception e)
            {
                logger.LogError("An error occured while processing the request. Error: {errorMessage}", e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

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
