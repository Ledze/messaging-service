using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Training.MessageDelivery.ASB;

namespace Training.MessageDelivery.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("messages-on-history/{subscription}")]
        [Authorize]
        public async Task<IActionResult> MessagesOnHistory(string subscription)
        {
            try
            {
                var messages = await _messageService.PeekTopicMessagesAsync(subscription);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving messages.");
            }
        }

        [HttpGet("subscriptions")]
        [Authorize]
        public async Task<IActionResult> GetSubscriptionsForTopic()
        {
            try
            {
                var subscriptions = await _messageService.GetSubscriptionsForTopicAsync();
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving subscriptions.");
            }
        }

        [HttpPost("send-message")]
        [Authorize]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return BadRequest("Message content cannot be empty.");
            }

            try
            {
                await _messageService.SendMessageAsync(message);
                return Ok("Message sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while sending the message.");
            }
        }
    }
}
