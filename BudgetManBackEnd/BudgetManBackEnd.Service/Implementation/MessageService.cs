using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using BudgetManBackEnd.Service.Contract;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BudgetManBackEnd.Model.Request;

namespace BudgetManBackEnd.Service.Implementation
{
    public class MessageService : IMessageService
    {
        public async Task<string> HandleMessage(string message, bool isGroup)
        {
            var userMessage = message;
            var jsonResponse = await GetResponseFromWitAI(userMessage);
            string commandResult = string.Empty;
            if (jsonResponse != null)
            {
                commandResult = await ProcessWitAiResponse(jsonResponse, isGroup);
            }

            //bool isGroup = turnContext.Activity.Conversation.IsGroup ?? false;
            string text = string.Empty;
            if (string.IsNullOrEmpty(commandResult))
            {
                text = await GetResponseFromEdenAI(userMessage, isGroup);
            }
            else
            {
                text = commandResult;
            }
            return text;
            //reply = MessageFactory.Text(text);
            //if (isGroup)
            //{
            //    var incomingMessageId = turnContext.Activity.Id;
            //    reply.ReplyToId = incomingMessageId;
            //    var originalMessage = turnContext.Activity;
            //    var authorName = originalMessage.From.Name ?? "Unknown User";
            //    var messageId = originalMessage.Id ?? "unknown";
            //    var timestamp = originalMessage.Timestamp.HasValue
            //        ? new DateTimeOffset(originalMessage.Timestamp.Value.UtcDateTime).ToUnixTimeSeconds().ToString()
            //        : "0";

            //    // Build the quote markup
            //    var quote = $"<quote authorname=\"{authorName}\" timestamp=\"{timestamp}\" " +
            //                $"conversation=\"{originalMessage.Conversation.Id}\" messageid=\"{messageId}\" cuid=\"{Guid.NewGuid()}\">" +
            //                $"<legacyquote>[{timestamp}] {authorName}: </legacyquote>" +
            //                $"{originalMessage.Text}<legacyquote>\n\n&lt;&lt;&lt; </legacyquote></quote>";
            //    reply.Text = quote + reply.Text;
            //}
        }

        protected async Task<string> GetResponseFromEdenAI(string message, bool isGroup)
        {
            string result = string.Empty;
            HttpClient client = new HttpClient();
            var url = "https://api.edenai.run/v2/aiproducts/askyoda/v2/319f0e47-42fb-4028-9d26-c77282882454/ask_llm";
            var apiKey = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiZmI0NTAzNTYtOWE4Ni00OWJlLWJhZGEtMzIyZjc2ODU0ZTYxIiwidHlwZSI6ImFwaV90b2tlbiJ9.wB1ZEz30xEtKmZTlSY3y8bmLsUlBKfHdJk9LdFo7_nY";

            var payload = new BudgetManBackEnd.Model.Request.EdenAiPayload
            {
                query = message,
                llm_provider = "openai",
                llm_model = "gpt-4",
                k = 5,
                //filter_documents = new { metadata1 = "value1" },
                max_tokens = 100,
                temperature = 0.5
            };
            if (isGroup)
            {
                payload.conversation_id = "942f3ae5-59f5-4712-8a1a-0cd0aad08e7f";
            }
            var jsonContent = JsonConvert.SerializeObject(payload);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            client.DefaultRequestHeaders.Add("Authorization", apiKey);

            try
            {
                HttpResponseMessage response = await client.PostAsync(url, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    JObject responseObject = JObject.Parse(responseData);
                    result = responseObject["result"].ToString();
                    Console.WriteLine(responseData);
                    return result;
                }
                else
                {
                    var responseMesssage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, {responseMesssage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return result;
        }

        protected async Task<JObject> GetResponseFromWitAI(string message)
        {
            var baseUrl = "https://api.wit.ai/message";
            var versionParam = "v=20241216";
            var queryParam = "q=" + HttpUtility.UrlEncode(message);
            var apiKey = "Bearer VLWLLXQ2F6UDM3LKHBYRR2XR5POTFSMB";
            var apiUrl = $"{baseUrl}?{versionParam}&{queryParam}";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", apiKey);

            try
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response JSON:");
                    Console.WriteLine(responseData);

                    // Parse JSON response
                    JObject jsonResponse = JObject.Parse(responseData);
                    if (jsonResponse["intents"] != null || jsonResponse["intents"].Count() > 0)
                    {
                        return jsonResponse;
                    }
                    // Extract and display intents

                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
            return null;
        }

        //private Dictionary<string, Action<object[]>> intentActions = new Dictionary<string, Action<object[]>>()
        //{
        //    { "get_weather", parameters => GetRandomPersonInGroup((ITurnContext<IMessageActivity>)parameters[0], (string)parameters[1]) },
        //    //{ "book_flight", parameters => BookFlight((string)parameters[0], (string)parameters[1]) },
        //    //{ "set_reminder", parameters => SetReminder((string)parameters[0], (string)parameters[1]) }
        //};

        protected async Task<string> ProcessWitAiResponse(JObject jsonResponse, bool isGroup)
        {
            var intents = jsonResponse["intents"];
            Console.WriteLine("\nIntents:");
            //var a = nameof(GetRandomPersonInGroup);
            var intent = intents.First?["name"]?.ToString();
            {
                //Console.WriteLine($"- Name: {intent["name"]}, Confidence: {intent["confidence"]}");
                switch (intent)
                {
                    //case nameof(GetRandomPersonInGroup):
                    //    var entitiyParameter = jsonResponse["entities"].First?.First?.First?["value"];
                    //    int numberPerson = 1;
                    //    if (entitiyParameter != null) numberPerson = (int)entitiyParameter;

                    //    return await GetRandomPersonInGroup(turnContext, numberPerson);
                    case nameof(GetBalance):
                        return await GetBalance(isGroup);
                    default:
                        Console.WriteLine("Intent not recognized.");
                        return string.Empty;
                }


                // Extract and display entities
                var entities = jsonResponse["entities"];
                Console.WriteLine("\nEntities:");
                //foreach (var entity in entities)
                //{
                //    Console.WriteLine($"- Entity Type: {entity.Key}");
                //    foreach (var detail in entity.Value)
                //    {
                //        Console.WriteLine($"  - Value: {detail["value"]}, Confidence: {detail["confidence"]}");
                //    }
                //}

            }
            //if (intentActions.ContainsKey(intent))
            //{
            //    return intentActions[intent](parameters);
            //}
            //else
            //{
            //    Console.WriteLine("Intent not recognized.");
            //    return string.Empty;
            //}
        }

        //protected static async Task<string> GetRandomPersonInGroup(ITurnContext<IMessageActivity> turnContext, int number = 1)
        //{
        //    //string result = string.Empty;
        //    //if (turnContext.Activity.Conversation?.IsGroup == true)
        //    {
        //        try
        //        {
        //            // Retrieve the list of conversation members
        //            var conversationMembers = await turnContext.TurnState.Get<IConnectorClient>()
        //                .Conversations.GetConversationMembersAsync(turnContext.Activity.Conversation.Id);
        //            var test = await turnContext.TurnState.Get<IConnectorClient>().Conversations.GetConversationMembersWithHttpMessagesAsync(turnContext.Activity.Conversation.Id);
        //            // Extract names of all members
        //            var memberIds = conversationMembers
        //                .Where(member => !string.IsNullOrEmpty(member.Id)) // Exclude empty names
        //                .Select(member => member.Id)
        //                .ToList();

        //            // Check if we have any valid members
        //            if (memberIds.Count == 0)
        //            {
        //                return "No participants found in the group.";
        //            }

        //            // Select random members
        //            var random = new Random();
        //            var selectedMembers = new List<string>();

        //            for (int i = 0; i < Math.Min(number, memberIds.Count); i++)
        //            {
        //                int randomIndex = random.Next(memberIds.Count);
        //                selectedMembers.Add(memberIds[randomIndex]);
        //                memberIds.RemoveAt(randomIndex); // Avoid picking duplicates
        //            }

        //            // Return selected member names
        //            return $"Selected {number} participant(s): {string.Join(", ", selectedMembers)}";
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error retrieving group members: {ex.Message}");
        //            return string.Empty;
        //        }
        //    }
        //    return string.Empty;
        //}

        //private static ServiceProvider ConfigureServices()
        //{
        //    var services = new ServiceCollection();

        //    // Register IBudgetService with its implementation
        //    services.AddScoped<IBudgetService, BudgetService>();

        //    return services.BuildServiceProvider();
        //}

        protected static async Task<string> GetBalance(bool isGroup = false)
        {

            //var serviceProvider = ConfigureServices();
            //var budgetService = serviceProvider.GetService<IBudgetService>();

            try
            {
                if (isGroup)
                {
                    return "123456789";
                    Guid budgetId = new Guid("asd");
                    //var budget = budgetService.GetBudget(budgetId);
                    //return budget.Data.Balance.ToString();
                }
                else
                {
                    return "123456789";
                    Guid budgetId = new Guid("asd");
                    //var budget = budgetService.GetBudget(budgetId);
                    //return budget.Data.Balance.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error GetBalance: {ex.Message}");
                return ex.Message;
            }
            return string.Empty;
        }

    }
}
