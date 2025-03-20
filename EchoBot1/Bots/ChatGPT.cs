using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System;
using Newtonsoft.Json;

namespace EchoBot1.Bots
{
    
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum Role
    {
        assistant, user, system
    }

    public class ChatGPT
    {
        const string AzureOpenAIEndpoint = "https://ai-viderlien4132ai356023607191.openai.azure.com";  //👉replace it with your Azure OpenAI Endpoint
        const string AzureOpenAIModelName = "gpt-35-turbo-16k"; //👉repleace it with your Azure OpenAI Model Name
        const string AzureOpenAIToken = "2BOP8ZuqdSMzITEKPt6zGrexEKWHHkJHIh4uz6cKtY1UHwEBoXsGJQQJ99BAACi0881XJ3w3AAAAACOGeXi7"; //👉repleace it with your Azure OpenAI Token
        const string AzureOpenAIVersion = "2024-02-15-preview";  //👉replace  it with your Azure OpenAI Model Version

        public static string CallAzureOpenAIChatAPI(
            string endpoint, string modelName, string apiKey, string apiVersion, object requestData)
        {
            var client = new HttpClient();

            // 設定 API 網址
            var apiUrl = $"{endpoint}/openai/deployments/{modelName}/chat/completions?api-version={apiVersion}";

            // 設定 HTTP request headers
            client.DefaultRequestHeaders.Add("api-key", apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT heade
                                                                                                                // 將 requestData 物件序列化成 JSON 字串
            string jsonRequestData = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
            // 建立 HTTP request 內容
            var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");
            // 傳送 HTTP POST request
            var response = client.PostAsync(apiUrl, content).Result;
            // 取得 HTTP response 內容
            var responseContent = response.Content.ReadAsStringAsync().Result;
            var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(responseContent);
            return obj.choices[0].message.content.Value;
        }


        public static string getResponseFromGPT(string Message, List<Message> chatHistory)
        {
            //建立對話紀錄
            var messages = new List<ChatMessage>
                {
                    new ChatMessage {
                        role = Role.system ,
                        content = "今天是" + DateTime.Now.ToString() + @"
                            假設你是公司內部的人事系統輔助程式，透過對話完成相關作業，最終產生json內容回復。
                            json內容包含{""startdate"":""請假開始時間"",""enddate"":""請假結束時間"",
                                ""type"":""假種"",""agent"":""代理人""}
                            ----------------------"
                    }
                };
            //添加歷史對話紀錄
            foreach (var HistoryMessageItem in chatHistory)
            {
                //添加一組對話紀錄
                messages.Add(new ChatMessage()
                {
                    role = Role.user,
                    content = HistoryMessageItem.UserMessage
                });
                messages.Add(new ChatMessage()
                {
                    role = Role.assistant,
                    content = HistoryMessageItem.ResponseMessage
                });
            }
            messages.Add(new ChatMessage()
            {
                role = Role.user,
                content = Message
            });
            //回傳呼叫結果
            return ChatGPT.CallAzureOpenAIChatAPI(
                AzureOpenAIEndpoint, AzureOpenAIModelName, AzureOpenAIToken, AzureOpenAIVersion,
                new
                {
                    model = "gpt-3.5-turbo",
                    messages = messages
                }
                );
        }
    }

    public class ChatMessage
    {
        public Role role { get; set; }
        public string content { get; set; }
    }
}