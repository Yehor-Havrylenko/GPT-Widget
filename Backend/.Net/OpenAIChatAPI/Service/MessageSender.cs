using System.ClientModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using OpenAI;
using OpenAI.Assistants;

namespace Service.Sender;

[Experimental("OPENAI001")]
public class MessageSender
{
    private OpenAIClient _client;
    private Assistant _assistant;
    private AssistantClient _assistantClient;
    private Dictionary<string, AssistantThread> _sessionsThread;

    public async Task InitializeAsync(string apiKey, string assistantId)
    {
        try
        {
            _client = new OpenAIClient(apiKey);
            _assistantClient = _client.GetAssistantClient();
            _assistant = await _assistantClient.GetAssistantAsync(assistantId);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error initializing assistant: {e.Message}");
            throw;
        }

        _sessionsThread = new Dictionary<string, AssistantThread>();
        Console.WriteLine("Assistant is Initialized");
    }

    public Task<MessageResponse?> SendChatMessageAsync(string message, string sessionId)
    {
        var result = new MessageResponse();

        var value = "";
        AssistantThread thread;
        if (_sessionsThread.ContainsKey(sessionId))
        {
            thread = _sessionsThread[sessionId];
            List<MessageContent> messageContent = new List<MessageContent>();
            messageContent.Add(message);
            var value1 = _assistantClient.CreateMessage(thread, MessageRole.User, messageContent);
            _assistantClient.GetMessage(thread.Id, value1.Value.Id);
        }
        else
        {
            thread = CreateNewThread(message, sessionId);
            value = SendMessageFromNewThread(thread);
        }


        result.response_text = value;

        return Task.FromResult(result.response_text.Length == 0 ? null : result);
    }

    private AssistantThread CreateNewThread(string message, string sessionId)
    {
        var threadOptions = new ThreadCreationOptions()
        {
            InitialMessages =
            {
                new ThreadInitializationMessage(
                    MessageRole.User,
                    new List<MessageContent> { message })
            }
        };
        var result = _assistantClient.CreateThread(threadOptions);
        _sessionsThread.Add(sessionId, result);
        Console.WriteLine($"Thread {sessionId} is created");
        return result;
    }

    private string SendMessageFromNewThread(AssistantThread thread)
    {
        var value = new StringBuilder();

        CollectionResult<StreamingUpdate> streamingUpdates =
            _assistantClient.CreateRunStreaming(thread, _assistant);
        foreach (var streamingUpdate in streamingUpdates)
        {
            if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
            {
                Console.WriteLine($"--- Run started! ---");
            }

            if (streamingUpdate is MessageContentUpdate contentUpdate)
            {
                Console.Write(contentUpdate.Text);
                value.Append(contentUpdate.Text);
            }
        }

        return value.ToString();
    }
}