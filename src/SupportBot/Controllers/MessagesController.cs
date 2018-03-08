using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using SupportBot.Actors;
using SupportBot.Messages;

namespace SupportBot
{
    [Route("/api/messages")]
    public class MessagesController : Controller
    {
        private readonly BotFrameworkAdapter _botFrameworkAdapter;
        private readonly ConversationDispatcherProvider _conversationDispatcherProvider;
        private readonly ILogger _logger;

        public MessagesController(BotFrameworkAdapter botFrameworkAdapter,
                                  ILoggerFactory loggerFactory,
                                  ConversationDispatcherProvider conversationDispatcherProvider)
        {
            _botFrameworkAdapter = botFrameworkAdapter;
            _conversationDispatcherProvider = conversationDispatcherProvider;
            _logger = loggerFactory.CreateLogger<MessagesController>();
        }

        private async Task ReceiveCallback(IBotContext context)
        {
            var chatBotRequest = new ChatBotRequest(context.Request, context.TopIntent, context.Responses);
            var result = await _conversationDispatcherProvider.Value.Ask<ChatBotResponse>(chatBotRequest);
            
            foreach(var activity in result.Responses)
            {
                context.Reply(activity);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Activity activity)
        {
            try
            {
                var authorizationHeader = Request.Headers["Authorization"].FirstOrDefault();
                await _botFrameworkAdapter.ProcessActivty(authorizationHeader, activity, ReceiveCallback);

                return Accepted();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError(ex, "User is not authorized to access the bot.");
                return Unauthorized();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Invalid operation performed on the bot.");
                return NotFound(ex.Message);
            }
        }
    }
}