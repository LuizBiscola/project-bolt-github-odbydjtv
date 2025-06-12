using ChatAppApi.DTOs;
using ChatAppApi.Models;
using ChatAppApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IChatService _chatService;

        public MessageController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> AddMessage(AddMessageRequest request)
        {
            try
            {
                var message = await _chatService.AddMessageToChatAsync(request.ChatId, request.SenderId, request.Content);
                return Ok(message.ToDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro ao enviar a mensagem");
            }
        }

        [HttpGet("chat/{chatId}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetChatMessages(
            int chatId, 
            [FromQuery] int take = 100, 
            [FromQuery] int skip = 0, 
            [FromQuery] int? beforeMessageId = null)
        {
            var messages = await _chatService.GetChatMessagesAsync(chatId, take, skip, beforeMessageId);
            return Ok(messages.Select(m => m.ToDto()));
        }
    }

    public class AddMessageRequest
    {
        public int ChatId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}