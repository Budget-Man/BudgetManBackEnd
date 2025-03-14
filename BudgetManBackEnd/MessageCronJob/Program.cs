using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using Newtonsoft.Json.Linq;
using System.Web;
using System.Security.Cryptography;
using Microsoft.Bot.Connector;
using static System.Net.Mime.MediaTypeNames;
using BudgetManBackEnd.Service;
using BudgetManBackEnd.Service.Contract;
using BudgetManBackEnd.Service.Implementation;
using Microsoft.Extensions.DependencyInjection;
using BudgetManBackEnd.DAL.Contract;
using BudgetManBackEnd.DAL.Implementation;

namespace BudgetManBackEnd.BotFramework
{
    public class MyBot : ActivityHandler
    {
        private readonly IMessageService _messageService;
        public MyBot(IMessageService messageService)
        {
            _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        }
        public static void Main(string[] args)
        {
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("");
            turnContext.SendActivityAsync(new Activity
                {
                    Type = ActivityTypes.Typing
                }, cancellationToken);
            try
            {
                var userMessage = turnContext.Activity.Text;
                string userId = "4d2d815f-4def-4a12-8dc8-860ac023254a";
                string response = await _messageService.HandleMessage(userMessage, userId);

                reply = MessageFactory.Text(response);
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            catch (Exception ex)
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Error: {ex.Message}"), cancellationToken);
            }
        }

        //protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        //{
        //    using (var scope = _scopeFactory.CreateScope()) // Creates a new scope per request
        //    {
        //        var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
        //        var reply = new Activity();

        //        try
        //        {
        //            // Send a "typing" activity to indicate the bot is preparing a response
        //            await turnContext.SendActivityAsync(new Activity
        //            {
        //                Type = ActivityTypes.Typing
        //            }, cancellationToken);

        //            var userMessage = turnContext.Activity.Text;

        //            bool isGroup = turnContext.Activity.Conversation.IsGroup ?? false;
        //            string textReply = string.Empty;
        //            //var meesageService = serviceProvider.GetService<IMessageService>();
        //            var userId = "4d2d815f-4def-4a12-8dc8-860ac023254a";
        //            textReply = await _messageService.HandleMessage(userMessage, userId);

        //            reply = MessageFactory.Text(textReply);
        //            if (isGroup)
        //            {
        //                var incomingMessageId = turnContext.Activity.Id;
        //                reply.ReplyToId = incomingMessageId;
        //                var originalMessage = turnContext.Activity;
        //                var authorName = originalMessage.From.Name ?? "Unknown User";
        //                var messageId = originalMessage.Id ?? "unknown";
        //                var timestamp = originalMessage.Timestamp.HasValue
        //                    ? new DateTimeOffset(originalMessage.Timestamp.Value.UtcDateTime).ToUnixTimeSeconds().ToString()
        //                    : "0";

        //                // Build the quote markup
        //                var quote = $"<quote authorname=\"{authorName}\" timestamp=\"{timestamp}\" " +
        //                            $"conversation=\"{originalMessage.Conversation.Id}\" messageid=\"{messageId}\" cuid=\"{Guid.NewGuid()}\">" +
        //                            $"<legacyquote>[{timestamp}] {authorName}: </legacyquote>" +
        //                            $"{originalMessage.Text}<legacyquote>\n\n&lt;&lt;&lt; </legacyquote></quote>";
        //                reply.Text = quote + reply.Text;
        //            }

        //            await turnContext.SendActivityAsync(reply, cancellationToken);
        //        }
        //        catch (Exception ex)
        //        {
        //            reply = MessageFactory.Text(ex.ToString());
        //            await turnContext.SendActivityAsync(reply, cancellationToken);
        //            Console.WriteLine($"Error sending message: {ex.Message}");
        //        }
        //    }
        //}
        protected override async Task OnConversationUpdateActivityAsync(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            try
            {
                var activity = turnContext.Activity;

                // Check if members were added to the conversation
                if (activity.MembersAdded != null && activity.MembersAdded.Count > 0)
                {
                    foreach (var member in activity.MembersAdded)
                    {
                        // Check if the added member is the bot itself
                        if (member.Id == activity.Recipient.Id)
                        {
                            await turnContext.SendActivityAsync(new Activity
                            {
                                Type = ActivityTypes.Typing
                            }, cancellationToken);
                            // Send a welcome message or perform initialization
                            await turnContext.SendActivityAsync(
                                MessageFactory.Text($"Hello everyone! I'm your bot. How can I assist you?"),
                                cancellationToken
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling ConversationUpdateActivity: {ex.Message}");
            }
        }
        
    }
}