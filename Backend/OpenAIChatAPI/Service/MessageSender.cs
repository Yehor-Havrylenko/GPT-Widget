using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;
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
        private Dictionary<string, AssistantThread> _sessionsThread;
        private bool _debug;

        public async Task InitializeAsync(string apiKey, string assistantId, bool debug = false)
        {
            _debug = debug;
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

        public async Task<MessageResponse?> SendChatMessageAsync(string message, string sessionId)
        {
            var result = new MessageResponse();
            var value = "";

            try
            {
                AssistantThread thread;

                if (_sessionsThread.ContainsKey(sessionId))
                {
                   if(_debug) Console.WriteLine("Using existing thread for session: " + sessionId);
                    thread = _sessionsThread[sessionId];
                    var messageContent = new List<MessageContent> { message };
                    await _assistantClient.CreateMessageAsync(thread, MessageRole.User, messageContent);
                }
                else
                {
                    if(_debug) Console.WriteLine("Creating new thread for session: " + sessionId);
                    thread = await CreateNewThreadAsync(message, sessionId);
                }

                value = await SendMessageFromThreadAsync(thread);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in SendChatMessageAsync: {e.Message}");
                throw;
            }

            if(_debug) Console.WriteLine($"Sending message: {message}");

            result.response_text = value;
            return string.IsNullOrEmpty(value) ? null : result;
        }

        private async Task<AssistantThread> CreateNewThreadAsync(string message, string sessionId)
        {
            var threadOptions = new ThreadCreationOptions
            {
                InitialMessages =
                {
                    new ThreadInitializationMessage(
                        MessageRole.User,
                        new List<MessageContent> { message })
                }
            };

            var thread = await _assistantClient.CreateThreadAsync(threadOptions);
            _sessionsThread[sessionId] = thread;
            if(_debug) Console.WriteLine($"Thread {sessionId} is created");

            return thread;
        }

        private async Task<string> SendMessageFromThreadAsync(AssistantThread thread)
        {
            var value = new StringBuilder();

            try
            {
                await foreach (var streamingUpdate in _assistantClient.CreateRunStreamingAsync(thread, _assistant))
                {
                    if(_debug) Console.WriteLine($"Streaming update received: {streamingUpdate.GetType().Name}");

                    if (streamingUpdate.UpdateKind == StreamingUpdateReason.RunCreated)
                    {
                        if(_debug) Console.WriteLine("--- Run Created ---");
                    }

                    if (streamingUpdate is MessageContentUpdate contentUpdate)
                    {
                        if(_debug) Console.WriteLine($"Content update received: {contentUpdate.Text}");
                        value.Append(contentUpdate.Text);
                    }
                    else
                    {
                        if(_debug) Console.WriteLine($"Unhandled update type: {streamingUpdate}");
                    }
                }

                var responseText = value.ToString();
                if(_debug) Console.WriteLine($"Final response: {responseText}");
                return responseText;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during streaming: {ex.Message}");
                return string.Empty;
            }
        }
    }
}
