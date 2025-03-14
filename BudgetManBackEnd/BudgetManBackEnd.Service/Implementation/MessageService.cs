using System.Text;
using System.Threading.Tasks;
using System.Web;
using BudgetManBackEnd.Service.Contract;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BudgetManBackEnd.Model.Request;
using BudgetManBackEnd.Model.Dto;
using MayNghien.Common.Helpers;
using MayNghien.Models.Response.Base;
using AutoMapper;
using BudgetManBackEnd.DAL.Contract;
using Microsoft.AspNetCore.Http;

namespace BudgetManBackEnd.Service.Implementation
{
    public class MessageService : IMessageService
    {
        private readonly IMoneyHolderRepository _moneyHolderRepository;
        private readonly IAccountInfoRepository _accountInfoRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMoneyHolderService _moneyHolderService;
        private readonly IServiceScopeFactory _serviceScopeFactory; // ✅ Use IServiceScopeFactory


        public MessageService(IServiceScopeFactory serviceScopeFactory, IMoneyHolderRepository moneyHolderRepository, IAccountInfoRepository accountInfoRepository, IMoneyHolderService moneyHolderService, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _moneyHolderRepository = moneyHolderRepository ?? throw new ArgumentNullException(nameof(moneyHolderRepository));
            _accountInfoRepository = accountInfoRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _moneyHolderService = moneyHolderService;
            _serviceScopeFactory = serviceScopeFactory;

        }

        public async Task<string> HandleMessage(string message, string? userId = null, bool isGroup = false)
        {
            var userMessage = message;
            var jsonResponse = await GetResponseFromWitAI(userMessage);
            string commandResult = string.Empty;

            if (string.IsNullOrEmpty(userId)) userId  = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            if (jsonResponse != null)
            {
                commandResult = ProcessWitAiResponse(jsonResponse, userId, isGroup);
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
        }

        protected static async Task<string> GetResponseFromEdenAI(string message, bool isGroup)
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

        protected static async Task<JObject> GetResponseFromWitAI(string message)
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

        protected string ProcessWitAiResponse(JObject jsonResponse, string userid, bool isGroup)
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
                        return GetBalance(userid);
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

        public string GetBalance(string userid)
        {
            
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope()) // ✅ Get fresh scope
                {
                    var moneyHolderRepository = scope.ServiceProvider.GetRequiredService<IMoneyHolderRepository>(); // ✅ New repository instance

                    var query = moneyHolderRepository.GetAll()
                        .Where(m => m.Account.UserId == userid && m.IsDeleted != true)
                        .ToList();

                    var firstItem = query.FirstOrDefault();
                    return firstItem?.Balance.ToString() ?? "Bạn chưa có ví tiền nào";
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
