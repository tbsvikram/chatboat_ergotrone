using ErgotronChatbotApi.BAL.Interfaces;
using ErgotronChatbotApi.BAL.Services;
using ErgotronChatbotApi.Common.Utility;

namespace ErgotronChatbotApi
{
    public static class DependencyConfiguration
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IApiService, ApiService>();
            services.AddSingleton<IQuestionAnswerAIService, QuestionAnswerAIService>();
            services.AddSingleton<IChatbotService, ChatbotService>();
        }

        public static void ConfigureAppSetting(this IConfiguration Configuration)
        {
            AppSettings.AzureServiceKey = Convert.ToString(Configuration["Azure:AzureServiceKey"]);
            AppSettings.AzureApiEndPoint = Convert.ToString(Configuration["Azure:AzureApiEndPoint"]);
            AppSettings.ProjectName = Convert.ToString(Configuration["Azure:ProjectName"]);
            AppSettings.DeploymentName = Convert.ToString(Configuration["Azure:DeploymentName"]);
            AppSettings.ApiVersion = Convert.ToString(Configuration["Azure:ApiVersion"]);

            AppSettings.ApiEndPoint = Convert.ToString(Configuration["Chatbot:ApiEndPoint"]);

        }
    }
}
