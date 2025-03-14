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

namespace BudgetManBackEnd.BotFramework
{
    public class MyBot : ActivityHandler
    {
        //protected readonly HttpClient client = new HttpClient();
        //private static string apiKey = "AIzaSyCUsEtDTL478LUgDTVKTqgxqoyEHuj-mW8";
        //private static string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";
        //private static string url = $"https://generativelanguage.googleapis.com/v1beta/models/tunedModels/budget-managment-9c5d6c3pw0yu:generateContent?key={apiKey}";


        //private readonly IBotFrameworkHttpAdapter _adapter;
        //private readonly IBot _bot;
        public MyBot()
        {
            //_adapter = adapter;
            //_bot = bot;
            //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

        }
        public static void Main(string[] args)
        {
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = new Activity();
            try
            {
                // Send a "typing" activity to indicate the bot is preparing a response
                await turnContext.SendActivityAsync(new Activity
                {
                    Type = ActivityTypes.Typing
                }, cancellationToken);

                var userMessage = turnContext.Activity.Text;


                //var jsonResponse = await GetResponseFromWitAI(userMessage);
                //string commandResult = string.Empty;
                //if (jsonResponse != null)
                //{
                //    commandResult = await ProcessWitAiResponse(jsonResponse, turnContext);
                //}

                bool isGroup = turnContext.Activity.Conversation.IsGroup ?? false;
                string textReply = string.Empty;

                

                reply = MessageFactory.Text(textReply);
                if (isGroup)
                {
                    var incomingMessageId = turnContext.Activity.Id;
                    reply.ReplyToId = incomingMessageId;
                    var originalMessage = turnContext.Activity;
                    var authorName = originalMessage.From.Name ?? "Unknown User";
                    var messageId = originalMessage.Id ?? "unknown";
                    var timestamp = originalMessage.Timestamp.HasValue
                        ? new DateTimeOffset(originalMessage.Timestamp.Value.UtcDateTime).ToUnixTimeSeconds().ToString()
                        : "0";

                    // Build the quote markup
                    var quote = $"<quote authorname=\"{authorName}\" timestamp=\"{timestamp}\" " +
                                $"conversation=\"{originalMessage.Conversation.Id}\" messageid=\"{messageId}\" cuid=\"{Guid.NewGuid()}\">" +
                                $"<legacyquote>[{timestamp}] {authorName}: </legacyquote>" +
                                $"{originalMessage.Text}<legacyquote>\n\n&lt;&lt;&lt; </legacyquote></quote>";
                    reply.Text = quote + reply.Text;
                }

                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            catch (Exception ex)
            {
                reply = MessageFactory.Text(ex.ToString());
                await turnContext.SendActivityAsync(reply, cancellationToken);
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }
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
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Register IBudgetService with its implementation
            services.AddScoped<IBudgetService, BudgetService>();

            return services.BuildServiceProvider();
        }
    }
}