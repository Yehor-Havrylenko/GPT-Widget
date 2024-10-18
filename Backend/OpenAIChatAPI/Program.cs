using Service.Sender;

#pragma warning disable OPENAI001

var builder = WebApplication.CreateBuilder(args);

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
app.Services.GetRequiredService<MessageSender>().InitializeAsync(Environment.GetEnvironmentVariable("OPENAI_KEY"),
    Environment.GetEnvironmentVariable("OPENAI_ASSISTANT_ID"));
app.Run();