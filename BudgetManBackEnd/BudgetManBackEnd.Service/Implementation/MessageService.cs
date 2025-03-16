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
using BudgetManBackEnd.DAL.Implementation;
using BudgetManBackEnd.DAL.Models.Entity;
using System.Text.RegularExpressions;

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

            if (string.IsNullOrEmpty(userId)) userId = ClaimHelper.GetClainByName(_httpContextAccessor, "UserId");
            if (jsonResponse != null)
            {
                commandResult = await ProcessWitAiResponse(message, jsonResponse, userId, isGroup);
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

        protected static async Task<string> GetResponseFromGemini(string userMessage, string systemMessage)
        {
            string result = string.Empty;
            var apiKey = "AIzaSyCqMFpfwgGjuhu7a_VnFtkGNw6qmKdJ6RI";
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}";
            try
            {
                using (var client = new HttpClient())
                {
                    // Set content type header
                    //client.DefaultRequestHeaders.Add("Content-Type", "application/json");

                    // Create the request payload
                    var requestBody = new
                    {
                        system_instruction = new
                        {
                            parts = new
                            {
                                text = systemMessage
                            }
                        },
                        contents = new
                        {
                            parts = new
                            {
                                text = userMessage
                            }
                        }
                    };

                    // Serialize the request body to JSON
                    string jsonPayload = JsonConvert.SerializeObject(requestBody);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Make the POST request
                    HttpResponseMessage response = await client.PostAsync(url, content);

                    // Ensure the request was successful
                    response.EnsureSuccessStatusCode();

                    // Read and return the response
                    string responseString = await response.Content.ReadAsStringAsync();
                    var jsonObject = JsonConvert.DeserializeObject<dynamic>(responseString);
                    string extractedText = jsonObject.candidates[0].content.parts[0].text;

                    return extractedText;
                }
            }
            catch (HttpRequestException ex)
            {
                return $"Error calling Gemini API: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected error: {ex.Message}";
            }
        }

        protected static async Task<string> GetResponseFromEdenAI(string message, bool isGroup)
        {
            string result = string.Empty;
            HttpClient client = new HttpClient();
            var url = "https://api.edenai.run/v2/aiproducts/askyoda/v2/a765c4b2-7e1a-4fdb-87b4-9b313c4b11f9/ask_llm";
            var apiKey = "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjoiY2NhMjIzY2ItZjk3Yi00ZWZmLWIyNDMtOTdmNDkwNjc3ZmFhIiwidHlwZSI6ImFwaV90b2tlbiJ9.bI7IuquxD4kx463H3-0PQKFlH6IHNAoC6fdRODZpfeU";

            var payload = new BudgetManBackEnd.Model.Request.EdenAiPayload
            {
                query = message,
                llm_provider = "meta",
                llm_model = "llama3-1-405b-instruct-v1:0",
                k = 3,
                //filter_documents = new { metadata1 = "value1" },
                max_tokens = 100,
                temperature = 0.5
            };
            //if (isGroup)
            //{
            //    payload.conversation_id = "942f3ae5-59f5-4712-8a1a-0cd0aad08e7f";
            //}
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
                    return responseMesssage;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return ex.Message;
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

        protected async Task<string> ProcessWitAiResponse(string message, JObject jsonResponse, string userid, bool isGroup)
        {
            var intents = jsonResponse["intents"];
            Console.WriteLine("\nIntents:");
            //var a = nameof(GetRandomPersonInGroup);
            var intent = intents.First?["name"]?.ToString();
            {
                //Console.WriteLine($"- Name: {intent["name"]}, Confidence: {intent["confidence"]}");
                JToken? entitiyParameter;
                switch (intent)
                {
                    //case nameof(GetRandomPersonInGroup):
                    //    var entitiyParameter = jsonResponse["entities"].First?.First?.First?["value"];
                    //    int numberPerson = 1;
                    //    if (entitiyParameter != null) numberPerson = (int)entitiyParameter;

                    //    return await GetRandomPersonInGroup(turnContext, numberPerson);
                    case nameof(GetBalance):
                        var balance = GetBalance(userid);
                        balance = "số tiền/balance: " + balance;
                        var response = await GetResponseFromGemini(message, balance);
                        return response;
                    case nameof(AddIncome):
                        entitiyParameter = jsonResponse["entities"].First?.First?.First?["value"];
                        return AddIncome(message, userid, (string)entitiyParameter);
                    case nameof(AddExpense):
                        entitiyParameter = jsonResponse["entities"].First?.First?.First?["value"];
                        return AddExpense(message, userid, (string)entitiyParameter);
                        
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

        public static string FormatCurrency(string number, string currencyCode = "vnđ")
        {
            // Remove any existing formatting and convert to long
            if (long.TryParse(new string(number.Where(char.IsDigit).ToArray()), out long parsedNumber))
            {
                return string.Format("{0:N0}{1}", parsedNumber, currencyCode).Replace(",", ".");
            }
            return number; // Return original string if parsing fails
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
                    string result = null;
                    if (firstItem != null && firstItem?.Balance != null)
                    {
                        result = FormatCurrency(firstItem?.Balance.ToString());
                    }
                    return result ?? "Bạn chưa có ví tiền nào";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error GetBalance: {ex.Message}");
                return ex.Message;
            }
            return string.Empty;
        }

        public static double ConvertMoneyStringToDouble(string moneyString)
        {
            if (string.IsNullOrWhiteSpace(moneyString))
                throw new ArgumentException("The money string cannot be empty.");

            // Normalize the string: remove extra spaces, convert to lowercase
            moneyString = moneyString.Trim().ToLower();

            // Remove unnecessary characters (e.g., currency symbols like "đ", "vnd", "$")
            moneyString = moneyString.Replace("đ", "").Replace("vnd", "").Replace("$", "").Trim();

            // Handle combined formats like "1m5" or "2m5"
            var combinedRegex = new Regex(@"([\d\.]+)m([\d\.]+)?");
            if (combinedRegex.IsMatch(moneyString))
            {
                var match = combinedRegex.Match(moneyString);
                double millions = double.Parse(match.Groups[1].Value); // Part before "m"
                double thousands = match.Groups[2].Success ? double.Parse(match.Groups[2].Value) : 0; // Part after "m"

                // Convert: millions * 1,000,000 + thousands * 100,000 (assuming "5" means 500k in "1m5")
                return (millions * 1000000) + (thousands * 100000);
            }

            // Handle standard format (e.g., "10k", "5 million")
            var standardRegex = new Regex(@"([\d\.]+)\s*([^\d\s\.]+)?");
            var standardMatch = standardRegex.Match(moneyString);

            if (!standardMatch.Success)
                throw new ArgumentException("Invalid money string format.");

            // Get the number part (may include decimal point)
            string numberPart = standardMatch.Groups[1].Value;
            string unitPart = standardMatch.Groups[2].Success ? standardMatch.Groups[2].Value : "";

            // Convert the number part to double
            if (!double.TryParse(numberPart, out double number))
                throw new ArgumentException("Unable to convert the number part to double.");

            // Handle the unit
            double multiplier = 1; // Default is 1 if no unit is provided
            switch (unitPart)
            {
                // Vietnamese units
                case "k":
                    multiplier = 1000; // Thousand
                    break;
                case "nghin":
                case "nghìn":
                case "ngan":
                case "ngàn":
                    multiplier = 1000; // Thousand
                    break;
                case "xi":
                case "xị":
                    multiplier = 100000; // 100 thousand
                    break;
                case "cu":
                case "củ":
                    multiplier = 1000000; // Million
                    break;
                case "m": // "m" alone means million
                    multiplier = 1000000; // Million
                    break;

                // English units
                case "thousand":
                    multiplier = 1000; // Thousand
                    break;
                case "million":
                    multiplier = 1000000; // Million
                    break;
                case "b":
                case "billion":
                    multiplier = 1000000000; // Billion
                    break;

                default:
                    // If no unit or unrecognized unit, keep the original number
                    break;
            }

            // Return the result
            return number * multiplier;
        }

        public string AddIncome(string message, string userId, string amountText)
        {
            var amount = ConvertMoneyStringToDouble(amountText);
            var income = new Income();
            income.Id = Guid.NewGuid();
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope()) // ✅ Get fresh scope
                {
                    var accountInfoRepository = scope.ServiceProvider.GetRequiredService<IAccountInfoRepository>();
                    // Materialize the query immediately
                    var accountInfos = accountInfoRepository.FindBy(m => m.UserId == userId).ToList();

                    if (!accountInfos.Any())
                    {
                        return "Cannot find Account Info by this user";
                    }

                    var accountInfo = accountInfos.First();
                    income.AccountId = accountInfo.Id;

                    var moneyHolderRepository = scope.ServiceProvider.GetRequiredService<IMoneyHolderRepository>();
                    // Materialize the query immediately
                    var moneyHolders = moneyHolderRepository.GetAll()
                        .Where(m => m.Account.UserId == userId && m.IsDeleted != true)
                        .ToList();

                    var moneyHolder = moneyHolders.FirstOrDefault();
                    if (moneyHolder == null)
                    {
                        // Handle case where no money holder is found
                        return "No money holder found";
                    }

                    moneyHolder.Balance += amount;
                    moneyHolderRepository.Edit(moneyHolder);

                    income.MoneyHolder = moneyHolder;
                    income.Name = message;
                    income.Amount = amount;

                    var incomeRepository = scope.ServiceProvider.GetRequiredService<IIncomeRepository>();
                    incomeRepository.Add(income, accountInfo.Name);

                }
                string reply = $"Đã thêm {amountText} vào ngân sách";
                return reply;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error GetBalance: {ex.Message}");
                return ex.Message;
            }
            return string.Empty;
        }

        public string AddExpense(string message, string userId, string amountText)
        {
            var amount = ConvertMoneyStringToDouble(amountText);
            var expense = new MoneySpend();
            expense.Id = Guid.NewGuid();
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope()) // ✅ Get fresh scope
                {
                    var accountInfoRepository = scope.ServiceProvider.GetRequiredService<IAccountInfoRepository>();
                    // Materialize the query immediately
                    var accountInfos = accountInfoRepository.FindBy(m => m.UserId == userId).ToList();

                    if (!accountInfos.Any())
                    {
                        return "Cannot find Account Info by this user";
                    }

                    var accountInfo = accountInfos.First();
                    expense.AccountId = accountInfo.Id;

                    var budgetRepository = scope.ServiceProvider.GetRequiredService<IBudgetRepository>();
                    // Materialize the query immediately
                    var budgets = budgetRepository.GetAll()
                        .Where(m => m.Account.UserId == userId && m.IsDeleted != true)
                        .ToList();
                    var budget = budgets.FirstOrDefault();
                    if (budget == null)
                    {
                        return "Cannot find Budget by this user";
                    }
                    expense.BudgetId = budget.Id;

                    var moneyHolderRepository = scope.ServiceProvider.GetRequiredService<IMoneyHolderRepository>();
                    // Materialize the query immediately
                    var moneyHolders = moneyHolderRepository.GetAll()
                        .Where(m => m.Account.UserId == userId && m.IsDeleted != true)
                        .ToList();

                    var moneyHolder = moneyHolders.FirstOrDefault();
                    if (moneyHolder == null)
                    {
                        // Handle case where no money holder is found
                        return "No money holder found";
                    }

                    moneyHolder.Balance -= amount;
                    moneyHolderRepository.Edit(moneyHolder);

                    expense.MoneyHolder = moneyHolder;
                    expense.Reason = message;
                    expense.Amount = amount;

                    var moneySpendRepository = scope.ServiceProvider.GetRequiredService<IMoneySpendRepository>();
                    moneySpendRepository.Add(expense, accountInfo.Name);

                }
                string reply = $"Đã trừ {amountText} vào ngân sách";
                return reply;


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
