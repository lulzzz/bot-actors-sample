using Akka.Actor;
using Microsoft.Bot.Schema;
using SupportBot.Messages;

namespace SupportBot.Actors
{
    public enum ConversationState
    {
        Initial,
        WaitingForUserRequest
    }

    public class ConversationData
    {

    }

    public class Conversation : FSM<ConversationState, ConversationData>
    {
        private readonly string _conversationId;

        public Conversation(string conversationId)
        {
            StartWith(ConversationState.Initial, new ConversationData());

            When(ConversationState.Initial, state =>
            {
                var request = state.FsmEvent as ChatBotRequest;
                var activity = (Activity)request.UserRequest;

                if (activity.Type == ActivityTypes.ConversationUpdate)
                {
                    foreach (var member in activity.AsConversationUpdateActivity().MembersAdded)
                    {
                        if (member.Id == activity.Recipient.Id)
                        {
                            Sender.Tell(new ChatBotResponse(new[]
                            {
                                activity.CreateReply("Hey, I'm the new HR bot. You ca ask me anything about HR related stuff."),
                                activity.CreateReply("How can I help you today?")
                            }));
                        }
                    }

                    return GoTo(ConversationState.WaitingForUserRequest);
                }

                return Stay().Using(state.StateData);
            });

            When(ConversationState.WaitingForUserRequest, state =>
            {
                var request = state.FsmEvent as ChatBotRequest;
                var activity = (Activity)request.UserRequest;

                Sender.Tell(new ChatBotResponse(new[]
                {
                    activity.CreateReply("Oh um, I had too much coffee or too little coffee. Either way, I have no idea what to do here.")
                }));

                return Stay().Using(state.StateData);
            });

            Initialize();
            this._conversationId = conversationId;
        }
    }
}
