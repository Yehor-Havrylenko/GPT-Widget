using System.Diagnostics.CodeAnalysis;
using System.Text;
using OpenAI;
using OpenAI.Assistants;
using OpenAIChatAPI.DTO;

namespace Service.Sender
{
    [Experimental("OPENAI001")]
    public class MessageSender
    {
        private OpenAIClient _client;
        private Assistant _assistant;
        private AssistantClient _assistantClient;
        private IDatabaseHandler _databaseHandler;
        private bool _debug;
        

        public async Task InitializeAsync(string apiKey, string assistantId,IDatabaseHandler databaseHandler, bool debug = false)
        {
            _debug = debug;
            try
            {
                _databaseHandler = databaseHandler;
                if (_debug) Console.WriteLine("Initializing OpenAI client...");
                _client = new OpenAIClient(apiKey);
                _assistantClient = _client.GetAssistantClient();
                _assistant = await _assistantClient.GetAssistantAsync(assistantId);
                Console.WriteLine("Assistant initialized successfully.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error initializing assistant: {e.Message}");
                throw;
            }
        }

        public async Task<MessageResponse?> SendChatMessageAsync(string message, string sessionId)
        {
            if (_debug) Console.WriteLine($"Received message: {message}, Session ID: {sessionId}");

            try
            {
                var threadId = await _databaseHandler.GetThreadBySessionAsync(sessionId);
                AssistantThread thread;

                if (!string.IsNullOrEmpty(threadId))
                {
                    if (_debug) Console.WriteLine($"Using existing thread for session: {sessionId}, Thread ID: {threadId}");
                    thread = await _assistantClient.GetThreadAsync(threadId);
                }
                else
                {
                    if (_debug) Console.WriteLine($"No existing thread for session: {sessionId}. Creating a new thread...");
                    thread = await CreateNewThreadAsync(message, sessionId);
                }

                var response = await SendMessageFromThreadAsync(thread, message);

                if (_debug) Console.WriteLine("Saving communication history...");
                await _databaseHandler.SaveCommunicationHistoryAsync(thread.Id, message, "User", DateTime.UtcNow);
                await _databaseHandler.SaveCommunicationHistoryAsync(thread.Id, response, "Bot", DateTime.UtcNow);

                if (_debug) Console.WriteLine("Message processing completed successfully.");
                return new MessageResponse { response_text = response };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in SendChatMessageAsync: {e.Message}");
                throw;
            }
        }

        private async Task<AssistantThread> CreateNewThreadAsync(string message, string sessionId)
        {
            if (_debug) Console.WriteLine($"Creating a new thread for session: {sessionId}...");

            var threadOptions = new ThreadCreationOptions
            {
                InitialMessages =
                {
                   new ThreadInitializationMessage(MessageRole.User, new List<MessageContent> { message })
                }
            };

            var thread = await _assistantClient.CreateThreadAsync(threadOptions);

            var now = DateTime.UtcNow;

            if (_debug) Console.WriteLine($"Saving new thread: {thread.Value.Id} for session: {sessionId}");
            await _databaseHandler.SaveSessionThreadAsync(sessionId, thread.Value.Id, now, now);

            return thread;
        }

        private async Task<string> SendMessageFromThreadAsync(AssistantThread thread, string userMessage)
        {
            if (_debug) Console.WriteLine($"Sending message via thread: {thread.Id}");

            var messageContent = new List<MessageContent> { userMessage };
            await _assistantClient.CreateMessageAsync(thread, MessageRole.User, messageContent);

            var responseBuilder = new StringBuilder();

            await foreach (var update in _assistantClient.CreateRunStreamingAsync(thread, _assistant))
            {
                if (_debug && update.UpdateKind == StreamingUpdateReason.RunCreated)
                {
                    Console.WriteLine("--- Run Created ---");
                }

                if (update is MessageContentUpdate contentUpdate)
                {
                    responseBuilder.Append(contentUpdate.Text);
                }
            }

            var response = responseBuilder.ToString();

            if (_debug) Console.WriteLine($"Final response: {response}");
            return response;
        }

    }
}
