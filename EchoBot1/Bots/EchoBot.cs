// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.22.0

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace EchoBot1.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var ret = "";

            if (turnContext.Activity.Text.Contains("/reset"))
            {
                //清除對話紀錄
                ChatHistoryManager.DeleteIsolatedStorageFile();
                ret = "我已經把之前的對談都給忘了!";
            }
            else
            {
                //取得歷史聊天紀錄
                var chatHistory = ChatHistoryManager.GetMessagesFromIsolatedStorage("UserA");
                //Call Azure OpenAI 取得回應
                ret = ChatGPT.getResponseFromGPT(turnContext.Activity.Text, chatHistory);
                //儲存當前聊天紀錄
                ChatHistoryManager.SaveMessageToIsolatedStorage(
                    System.DateTime.Now, "UserA", turnContext.Activity.Text, ret);
            }

            //var replyText = $"Echo: {turnContext.Activity.Text}";
            await turnContext.SendActivityAsync(MessageFactory.Text(ret, ret), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
    }
}
