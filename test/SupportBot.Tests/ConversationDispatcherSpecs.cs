using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using Microsoft.Bot.Schema;
using SupportBot.Actors;
using SupportBot.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SupportBot.Tests
{
    public class BehavesLikeDispatcher : TestKit
    {
        private readonly IActorRef _dispatcherRef;

        public BehavesLikeDispatcher()
        {
            _dispatcherRef = new TestActorRef<ConversationDispatcher>(Sys, Props.Create<ConversationDispatcher>());
        }

        protected Activity CreateConversationUpdate(string conversationId)
        {
            var conversation = new ConversationAccount(id: conversationId);

            var conversationUpdate = Activity.CreateConversationUpdateActivity();
            var userAccount = new ChannelAccount(Guid.NewGuid().ToString(), "User");
            var botAccount = new ChannelAccount(Guid.NewGuid().ToString(), "Bot");

            conversationUpdate.MembersAdded.Add(userAccount);
            conversationUpdate.Recipient = userAccount;
            conversationUpdate.From = botAccount;
            conversationUpdate.Conversation = conversation;

            return (Activity)conversationUpdate;
        }

        protected Activity CreateMessage(string conversationId, string text)
        {
            var conversation = new ConversationAccount(id: conversationId);

            var message = Activity.CreateMessageActivity();

            var userAccount = new ChannelAccount(Guid.NewGuid().ToString(), "User");
            var botAccount = new ChannelAccount(Guid.NewGuid().ToString(), "Bot");

            message.Text = text;
            message.Conversation = conversation;
            message.From = userAccount;
            message.Recipient = botAccount;

            return (Activity)message;
        }

        protected IActorRef Dispatcher => _dispatcherRef;
    }

    public class WhenConversingWithBot : BehavesLikeDispatcher
    {
        [Fact]
        public void ConversationFlowHappensBetweenUserAndBot()
        {
            var conversationId = Guid.NewGuid().ToString();
            var testProbe = CreateTestProbe(Sys);

            Dispatcher.Tell(new ChatBotRequest(CreateConversationUpdate(conversationId), null, new IActivity[] { }), testProbe);

            testProbe.ExpectMsg<ChatBotResponse>(TimeSpan.FromMilliseconds(100));

            Dispatcher.Tell(new ChatBotRequest(CreateMessage(conversationId, "test message"), null,new IActivity[] { }), testProbe);

            testProbe.ExpectMsg<ChatBotResponse>(TimeSpan.FromMilliseconds(100));
        }
    }
}
