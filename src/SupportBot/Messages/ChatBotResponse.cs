using Microsoft.Bot.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupportBot.Messages
{
    public class ChatBotResponse
    {
        public ChatBotResponse(IEnumerable<IActivity> responses)
        {
            Responses = responses;
        }

        public IEnumerable<IActivity> Responses { get;}
    }
}
