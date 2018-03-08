using SupportBot.Actors;
using System;
using Xunit;
using Akka.TestKit.Xunit2;
using Akka.TestKit;
using Akka.Actor;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace SupportBot.Tests
{
    public class BehavesLikeConversation : TestKit
    {
        private TestFSMRef<Conversation, ConversationState, ConversationData> _conversation;

        public BehavesLikeConversation() : base()
        {
            var actorProps = Props.Create<Conversation>(Guid.NewGuid().ToString());
            _conversation = new TestFSMRef<Conversation, ConversationState, ConversationData>(Sys, actorProps);
        }

        protected TestFSMRef<Conversation, ConversationState, ConversationData> Conversation => _conversation;

        protected Activity CreateConversationUpdate()
        {
            var conversation = new ConversationAccount();

            var conversationUpdate = Activity.CreateConversationUpdateActivity();
            var userAccount = new ChannelAccount(Guid.NewGuid().ToString(), "User");
            var botAccount = new ChannelAccount(Guid.NewGuid().ToString(), "Bot");

            conversationUpdate.MembersAdded.Add(userAccount);
            conversationUpdate.Recipient = userAccount;
            conversationUpdate.From = botAccount;
            conversationUpdate.Conversation = conversation;

            return (Activity)conversationUpdate;
        }

        protected Activity CreateMessage(string text)
        {
            var conversation = new ConversationAccount();

            var message = Activity.CreateMessageActivity();

            var userAccount = new ChannelAccount(Guid.NewGuid().ToString(), "User");
            var botAccount = new ChannelAccount(Guid.NewGuid().ToString(), "Bot");

            message.Text = text;
            message.Conversation = conversation;
            message.From = userAccount;
            message.Recipient = botAccount;

            return (Activity)message;
        }
    }

    public class GivenConversationIsWaitingForUsers : BehavesLikeConversation
    {
        [Fact]
        public void WhenReceivesConversationUpdateThenTransitionsToWaitingForUserRequest()
        {
            var conversationUpdate = CreateConversationUpdate();

            Conversation.SetState(ConversationState.Initial);
            Conversation.Tell(conversationUpdate);

            Assert.Equal(ConversationState.WaitingForUserRequest, Conversation.StateName);
        }

        [Fact]
        public void WhenReceivesConversationUpdateThenSendsTwoMessages()
        {
            var responseProbe = CreateTestProbe();
            var conversationUpdate = CreateConversationUpdate();

            Conversation.SetState(ConversationState.Initial);
            Conversation.Tell(conversationUpdate, responseProbe.Ref);

            responseProbe.ExpectMsg<Activity[]>(TimeSpan.FromMilliseconds(100));
        }
    }

    public class GivenConversationIsWaitingForUserRequest : BehavesLikeConversation
    {
        [Fact]
        public void WhenReceivesUserMessageSendsWittyReply()
        {
            var responseProbe = CreateTestProbe();

            Conversation.SetState(ConversationState.WaitingForUserRequest);
            Conversation.Tell(CreateMessage("Test message"), responseProbe.Ref);

            responseProbe.ExpectMsg<Activity[]>(TimeSpan.FromMilliseconds(100));
        }

        [Fact]
        public void WhenReceivesUserMessageRemainsInWaitingForUserRequestState()
        {
            Conversation.SetState(ConversationState.WaitingForUserRequest);
            Conversation.Tell(CreateMessage("Test message"));

            Assert.Equal(ConversationState.WaitingForUserRequest, Conversation.StateName);
        }
    }
}
