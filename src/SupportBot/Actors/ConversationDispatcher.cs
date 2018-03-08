using Akka.Actor;
using Microsoft.Bot.Schema;
using SupportBot.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupportBot.Actors
{
    public class ConversationDispatcher: ReceiveActor
    {
        public ConversationDispatcher()
        {
            Receive<ChatBotRequest>(HandleMessage);
        }

        private bool HandleMessage(ChatBotRequest request)
        {
            var conversationRef = Context.Child(request.UserRequest.Conversation.Id);
            var activity = request.UserRequest;

            if(conversationRef.IsNobody())
            {
                conversationRef = Context.ActorOf(Props.Create<Conversation>(activity.Conversation.Id), activity.Conversation.Id);
            }

            conversationRef.Forward(request);

            return true;
        }
    }
}
