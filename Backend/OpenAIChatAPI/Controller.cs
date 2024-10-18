using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Service.Sender;

namespace OpenAIChatAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Experimental("OPENAI001")]
    public class ChatController : ControllerBase
    {
        private readonly MessageSender _messageSender;

        public ChatController(MessageSender messageSender)
        {
            _messageSender = messageSender;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] string message, string session_id)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message cannot be empty.");
            }

            var response = await _messageSender.SendChatMessageAsync(message, session_id);
            return Ok(response);
        }
    }
}