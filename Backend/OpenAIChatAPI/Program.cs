using OpenAIChatAPI.DTO;
using Service;
using Service.Databases;
using Service.Sender;

#pragma warning disable OPENAI001

var builder = WebApplication.CreateBuilder(args);

var apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");
var assistantId = Environment.GetEnvironmentVariable("OPENAI_ASSISTANT_ID");
var connectionString = Environment.GetEnvironmentVariable("MARIADB_CONNECTION_STRING");

if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(assistantId))
{
    Console.WriteLine("API Key or Assistant ID is missing.");
    return;
}

if (string.IsNullOrEmpty(connectionString))
{
    Console.WriteLine("MYSQL_CONNECTION_STRING is missing.");
    return;
}

bool debugMode = string.Equals(Environment.GetEnvironmentVariable("DEBUG_MODE"), "true", StringComparison.OrdinalIgnoreCase);
Console.WriteLine($"Debug Mode: {debugMode}");

builder.Services.AddSingleton<MessageSender>();
builder.Services.AddSingleton<IDatabaseHandler>(_ => new MariaDB(connectionString));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors();

app.MapPost("/chat/sendmessage", async (MessageRequest request, MessageSender chatApi) =>
{
    if (string.IsNullOrEmpty(request.Message))
    {
        return Results.BadRequest("Message cannot be empty.");
    }
    if (string.IsNullOrWhiteSpace(request.session_id))
    {
        return Results.BadRequest("Session ID cannot be empty.");
    }

    var response = await chatApi.SendChatMessageAsync(request.Message, request.session_id);
    return Results.Ok(response);
})
.WithName("SendMessage");

try
{
    await app.Services.GetRequiredService<MessageSender>()
        .InitializeAsync(apiKey, assistantId, app.Services.GetRequiredService<IDatabaseHandler>(), debugMode);
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred during initialization: {ex.Message}");
    return;
}

app.Run();
