﻿namespace QuizMaker
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string API_KEY_HEADER_NAME = "X-API-KEY";

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            if (context.Request.Path != "/openapi/v1.json")
            {
                if (!context.Request.Headers.TryGetValue(API_KEY_HEADER_NAME, out var extractedApiKey))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("API Key was not provided.");
                    return;
                }

                var apiKey = configuration.GetValue<string>("ApiKey")!;
                if (!apiKey.Equals(extractedApiKey))
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Unauthorized client.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
