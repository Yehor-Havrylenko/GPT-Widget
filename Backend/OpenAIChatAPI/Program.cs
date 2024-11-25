using OpenAIChatAPI.DTO;
using Service.Sender;

#pragma warning disable OPENAI001

var builder = WebApplication.CreateBuilder(args);
var apiKey = Environment.GetEnvironmentVariable("OPENAI_KEY");
var assistantId = Environment.GetEnvironmentVariable("OPENAI_ASSISTANT_ID");
if (string.IsNullOrEmpty(apiKey) && string.IsNullOrEmpty(assistantId))
{
    Console.WriteLine("API Key or Assistant ID is missing");
    return;
}
bool debugMode = bool.TrueString == Environment.GetEnvironmentVariable("DEBUG_MODE");
Console.WriteLine($"Debug Mode: {debugMode.ToString()}");

builder.Services.AddSingleton<MessageSender>();

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
        if (string.IsNullOrEmpty(request.Message) || string.IsNullOrWhiteSpace(request.session_id))
        {
            return Results.BadRequest("Message cannot be empty.");
        }

        var response = await chatApi.SendChatMessageAsync(request.Message, request.session_id);
        return Results.Ok(response);
    })
    .WithName("SendMessage");


await app.Services.GetRequiredService<MessageSender>()
    .InitializeAsync(apiKey, assistantId, debugMode);
app.Run();