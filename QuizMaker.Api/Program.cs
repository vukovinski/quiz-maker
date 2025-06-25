using QuizMaker;
using QuizMaker.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddApiVersioning().AddMvc();
builder.Services.AddDbContext<QuizMakerDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // http://localhost:5000/openapi/v1.json
    app.MapOpenApi();
}

app.UseMiddleware<ApiKeyMiddleware>();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();