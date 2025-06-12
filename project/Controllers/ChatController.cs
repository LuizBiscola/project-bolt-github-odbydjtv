using ChatAppApi.DTOs;
using ChatAppApi.Models;
using ChatAppApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatAppApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost]
        public async Task<ActionResult<ChatDto>> CreateChat(CreateChatRequest request)
        {
            if (request.ParticipantUserIds == null || request.ParticipantUserIds.Count < 2)
            {
                return BadRequest("Um chat precisa de pelo menos 2 participantes");
            }

            try
            {
                var chat = await _chatService.CreateChatAsync(request.Name, request.ParticipantUserIds);
                return Ok(chat.ToDto());
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Ocorreu um erro ao criar o chat");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetAllChats()
        {
            var chats = await _chatService.GetAllChatsAsync();
            return Ok(chats.Select(c => c.ToDto()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChatDto>> GetChatById(int id)
        {
            var chat = await _chatService.GetChatByIdAsync(id);
            
            if (chat == null)
                return NotFound();
                
            return Ok(chat.ToDto());
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ChatDto>>> GetUserChats(int userId)
        {
            var chats = await _chatService.GetUserChatsAsync(userId);
            return Ok(chats.Select(c => c.ToDto()));
        }
    }

    public class CreateChatRequest
    {
        public string Name { get; set; } = string.Empty;
        public List<int> ParticipantUserIds { get; set; } = new List<int>();
    }
}