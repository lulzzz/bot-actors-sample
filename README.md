# Bot Builder V4 Actors Sample
Most people, when I ask them about bots tell me that it involves AI techniques like intent detection. 
They tend to forget that a good bot is all about a good conversation model.

Building a good conversation model for a bot can be quite hard. You need to handle the user input intelligently
and tie it into an intelligent flow that doesn't obstruct the user. 

One of the things that really works well for this is the actor pattern.
This sample demonstrates how to use this pattern to build a bot.

## Quickstart
To use this sample you need VS2017 or Visual Studio Code with .NET core 2.0 SDK installed.
Use the following commands to run the sample on your local machine:

 * `dotnet restore`
 * `dotnet build`
 
When these commands complete succesfully, run the following command to start the bot:

```
dotnet run
```

## How does this sample work
The application is a REST API that connects to the bot builder connector in Azure.
When a message from the bot channel comes in, it is routed to the bot framework adapter.

This adapter will take care of a number of things, before delivering it to the callback that is 
specified in the `Post` method of the `MessagesController` class.

The callback method passes the incoming activity on to the `ConversationDispatcher` actor. This actor
is a router that is capable of delivering messages to the right conversation. 

To pass an activity to the conversation dispatcher, we use the `Ask` method. By using `Ask` instead of 
`Tell` we tell the actor to give us an answer. The method won't continue until we have an answer.

The actors that handle user messages use `Tell` internally and don't wait for answers. This frees us
from the request/reply model which feels rather unnatural for a chatbot.

The conversation itself is a finite state machine actor. The `Conversation` class implements this.
In this class you'll find two states:

 * WaitingForUsers
 * WaitingForUserRequest
 
When new users join the conversation, we'll welcome them by sending back two message activities
to the sender of the activity. Which in this case is towards the callback method. After this is done,
the actor goes to the WaitingForUserRequest state. 

Now when the user sends a message he/she will automatically get an answer.
This answer is also passed back as an activity to the sender of the original activity.

Finally, when the callback method receives the activities it should use as a reply, it will send those through the original bot context.
