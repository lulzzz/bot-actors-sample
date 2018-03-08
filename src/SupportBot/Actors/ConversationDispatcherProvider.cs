using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupportBot.Actors
{
    public class ConversationDispatcherProvider
    {
        private readonly IActorRef _actorRef;

        public ConversationDispatcherProvider(ActorSystem actorSystem)
        {
            _actorRef = actorSystem.ActorOf(Props.Create<ConversationDispatcher>(), "conversation-dispatcher");
        }

        public IActorRef Value => _actorRef;
    }
}
