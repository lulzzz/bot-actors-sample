using Akka.Actor;
using Microsoft.Bot.Schema;
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
            Receive<Activity>(HandleMessage);
        }

        private bool HandleMessage(Activity activity)
        {
            var conversationRef = Context.Child(activity.Conversation.Id);

            if(conversationRef.IsNobody())
            {
                conversationRef = Context.ActorOf(Props.Create<Conversation>(activity.Conversation.Id), activity.Conversation.Id);
            }

            conversationRef.Forward(activity);

            return true;
        }
    }
}
